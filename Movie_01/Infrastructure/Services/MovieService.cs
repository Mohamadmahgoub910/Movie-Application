using Microsoft.EntityFrameworkCore;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using MovieApp.Application.ViewModels;
using MovieApp.Infrastructure.Data;

namespace MovieApp.Infrastructure.Services
{
    /// <summary>
    /// Movie Service Implementation
    /// SOLID: Single Responsibility - handles movie business logic only
    /// SOLID: Dependency Inversion - depends on IUnitOfWork abstraction
    /// </summary>
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public MovieService(IUnitOfWork unitOfWork,
                           ApplicationDbContext context,
                           IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _fileService = fileService;
        }

        // ═══ Read Operations ═══

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            return await _unitOfWork.Movies.GetByIdAsync(id);
        }

        public async Task<Movie?> GetMovieWithDetailsAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.SubImages)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByCategoryAsync(int categoryId)
        {
            return await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Where(m => m.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByCinemaAsync(int cinemaId)
        {
            return await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Where(m => m.CinemaId == cinemaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByStatusAsync(MovieStatus status)
        {
            return await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Where(m => m.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm)
        {
            return await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Where(m => m.Name.Contains(searchTerm) ||
                           m.Description.Contains(searchTerm))
                .ToListAsync();
        }

        // ═══ Write Operations ═══

        public async Task<Movie> CreateMovieAsync(MovieViewModel model)
        {
            // 1. Create Movie entity
            var movie = new Movie
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Status = model.Status,
                ReleaseDateTime = model.ReleaseDateTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                CinemaId = model.CinemaId
            };

            // 2. Upload main image
            if (model.MainImageFile != null)
            {
                movie.MainImage = await _fileService.UploadFileAsync(
                    model.MainImageFile,
                    "movies"
                );
            }

            // 3. Add movie
            await _unitOfWork.Movies.AddAsync(movie);
            await _unitOfWork.SaveChangesAsync();

            // 4. Add actors
            if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
            {
                var movieActors = model.SelectedActorIds.Select(actorId => new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                });

                await _unitOfWork.MovieActors.AddRangeAsync(movieActors);
                await _unitOfWork.SaveChangesAsync();
            }

            // 5. Upload sub images
            if (model.SubImageFiles != null && model.SubImageFiles.Any())
            {
                var imageUrls = await _fileService.UploadFilesAsync(
                    model.SubImageFiles,
                    "movies/gallery"
                );

                var movieImages = imageUrls.Select(url => new MovieImage
                {
                    MovieId = movie.Id,
                    ImageUrl = url
                });

                await _unitOfWork.MovieImages.AddRangeAsync(movieImages);
                await _unitOfWork.SaveChangesAsync();
            }

            return movie;
        }

        public async Task<Movie> UpdateMovieAsync(int id, MovieViewModel model)
        {
            var movie = await GetMovieWithDetailsAsync(id);
            if (movie == null)
                throw new Exception("الفيلم غير موجود");

            // 1. Update basic info
            movie.Name = model.Name;
            movie.Description = model.Description;
            movie.Price = model.Price;
            movie.Status = model.Status;
            movie.ReleaseDateTime = model.ReleaseDateTime;
            movie.Duration = model.Duration;
            movie.CategoryId = model.CategoryId;
            movie.CinemaId = model.CinemaId;

            // 2. Update main image if provided
            if (model.MainImageFile != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(movie.MainImage))
                {
                    _fileService.DeleteFile(movie.MainImage);
                }

                // Upload new image
                movie.MainImage = await _fileService.UploadFileAsync(
                    model.MainImageFile,
                    "movies"
                );
            }

            // 3. Update actors
            var existingActors = movie.MovieActors.ToList();
            _unitOfWork.MovieActors.DeleteRange(existingActors);

            if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
            {
                var movieActors = model.SelectedActorIds.Select(actorId => new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                });

                await _unitOfWork.MovieActors.AddRangeAsync(movieActors);
            }

            // 4. Add new sub images if provided
            if (model.SubImageFiles != null && model.SubImageFiles.Any())
            {
                var imageUrls = await _fileService.UploadFilesAsync(
                    model.SubImageFiles,
                    "movies/gallery"
                );

                var movieImages = imageUrls.Select(url => new MovieImage
                {
                    MovieId = movie.Id,
                    ImageUrl = url
                });

                await _unitOfWork.MovieImages.AddRangeAsync(movieImages);
            }

            _unitOfWork.Movies.Update(movie);
            await _unitOfWork.SaveChangesAsync();

            return movie;
        }

        public async Task DeleteMovieAsync(int id)
        {
            var movie = await GetMovieWithDetailsAsync(id);
            if (movie == null)
                throw new Exception("الفيلم غير موجود");

            // Delete main image
            if (!string.IsNullOrEmpty(movie.MainImage))
            {
                _fileService.DeleteFile(movie.MainImage);
            }

            // Delete sub images
            var imagePaths = movie.SubImages.Select(img => img.ImageUrl).ToList();
            _fileService.DeleteFiles(imagePaths);

            // Delete movie (cascade delete will handle relationships)
            _unitOfWork.Movies.Delete(movie);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}