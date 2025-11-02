using MovieApp.Core.Entities;
using MovieApp.Core.ViewModels;

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


        //Task<Movie> CreateMovieAsync(MovieViewModel model);
        //Task<Movie> UpdateMovieAsync(int id, MovieViewModel model);



    }
}

   