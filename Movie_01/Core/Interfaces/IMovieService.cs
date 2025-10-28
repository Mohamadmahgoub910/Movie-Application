using MovieApp.Core.Entities;
using MovieApp.Application.ViewModels;

namespace MovieApp.Core.Interfaces
{
    /// <summary>
    /// Movie Service Interface
    /// SOLID: Interface Segregation - specific to Movie operations
    /// </summary>
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int id);
        Task<Movie?> GetMovieWithDetailsAsync(int id);
        Task<IEnumerable<Movie>> GetMoviesByCategoryAsync(int categoryId);
        Task<IEnumerable<Movie>> GetMoviesByCinemaAsync(int cinemaId);
        Task<IEnumerable<Movie>> GetMoviesByStatusAsync(MovieStatus status);
        Task<Movie> CreateMovieAsync(MovieViewModel model);
        Task<Movie> UpdateMovieAsync(int id, MovieViewModel model);
        Task DeleteMovieAsync(int id);
        Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm);
    }

    /// <summary>
    /// Category Service Interface
    /// </summary>
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category?> GetCategoryWithMoviesAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task<bool> CanDeleteCategoryAsync(int id);
    }

    /// <summary>
    /// Actor Service Interface
    /// </summary>
    public interface IActorService
    {
        Task<IEnumerable<Actor>> GetAllActorsAsync();
        Task<Actor?> GetActorByIdAsync(int id);
        Task<Actor?> GetActorWithMoviesAsync(int id);
        Task<Actor> CreateActorAsync(Actor actor);
        Task<Actor> UpdateActorAsync(Actor actor);
        Task DeleteActorAsync(int id);
        Task<IEnumerable<Actor>> GetTopActorsByMovieCountAsync(int count);
    }

    /// <summary>
    /// Cinema Service Interface
    /// </summary>
    public interface ICinemaService
    {
        Task<IEnumerable<Cinema>> GetAllCinemasAsync();
        Task<Cinema?> GetCinemaByIdAsync(int id);
        Task<Cinema?> GetCinemaWithMoviesAsync(int id);
        Task<Cinema> CreateCinemaAsync(Cinema cinema);
        Task<Cinema> UpdateCinemaAsync(Cinema cinema);
        Task DeleteCinemaAsync(int id);
        Task<bool> CanDeleteCinemaAsync(int id);
    }

    /// <summary>
    /// File Service Interface
    /// SOLID: Single Responsibility - handles file operations only
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Upload single file
        /// </summary>
        Task<string> UploadFileAsync(IFormFile file, string folder);

        /// <summary>
        /// Upload multiple files
        /// </summary>
        Task<List<string>> UploadFilesAsync(List<IFormFile> files, string folder);

        /// <summary>
        /// Delete file
        /// </summary>
        void DeleteFile(string filePath);

        /// <summary>
        /// Delete multiple files
        /// </summary>
        void DeleteFiles(List<string> filePaths);

        /// <summary>
        /// Check if file exists
        /// </summary>
        bool FileExists(string filePath);
    }

    /// <summary>
    /// Analytics Service Interface
    /// </summary>
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetAnalyticsDashboardAsync();
        Task<DashboardViewModel> GetDashboardStatisticsAsync();
    }
}