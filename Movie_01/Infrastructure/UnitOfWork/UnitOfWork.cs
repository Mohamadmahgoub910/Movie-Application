using Microsoft.EntityFrameworkCore.Storage;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using MovieApp.Infrastructure.Data;
using MovieApp.Infrastructure.Repositories;

namespace MovieApp.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Unit of Work Implementation
    /// SOLID: Single Responsibility - manages repositories and transactions
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization of repositories
        private IRepository<Movie>? _movies;
        private IRepository<Category>? _categories;
        private IRepository<Actor>? _actors;
        private IRepository<Cinema>? _cinemas;
        private IRepository<MovieActor>? _movieActors;
        private IRepository<MovieImage>? _movieImages;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // ═══ Repository Properties (Lazy Loading) ═══

        public IRepository<Movie> Movies
        {
            get
            {
                _movies ??= new Repository<Movie>(_context);
                return _movies;
            }
        }

        public IRepository<Category> Categories
        {
            get
            {
                _categories ??= new Repository<Category>(_context);
                return _categories;
            }
        }

        public IRepository<Actor> Actors
        {
            get
            {
                _actors ??= new Repository<Actor>(_context);
                return _actors;
            }
        }

        public IRepository<Cinema> Cinemas
        {
            get
            {
                _cinemas ??= new Repository<Cinema>(_context);
                return _cinemas;
            }
        }

        public IRepository<MovieActor> MovieActors
        {
            get
            {
                _movieActors ??= new Repository<MovieActor>(_context);
                return _movieActors;
            }
        }

        public IRepository<MovieImage> MovieImages
        {
            get
            {
                _movieImages ??= new Repository<MovieImage>(_context);
                return _movieImages;
            }
        }

        // ═══ Transaction Management ═══

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // ═══ Dispose Pattern ═══

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}