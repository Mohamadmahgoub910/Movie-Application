using MovieApp.Core.Entities;

namespace MovieApp.Core.Interfaces
{
    /// <summary>
    /// Unit of Work Pattern
    /// SOLID: Single Responsibility - manages all repositories & transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // ═══ Repositories ═══

        IRepository<Movie> Movies { get; }
        IRepository<Category> Categories { get; }
        IRepository<Actor> Actors { get; }
        IRepository<Cinema> Cinemas { get; }
        IRepository<MovieActor> MovieActors { get; }
        IRepository<MovieImage> MovieImages { get; }

        // ═══ Transaction Management ═══

        /// <summary>
        /// Save all changes to database
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Begin transaction
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commit transaction
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Rollback transaction
        /// </summary>
        Task RollbackAsync();
    }
}