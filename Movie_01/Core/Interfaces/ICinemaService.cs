
using MovieApp.Core.Entities;

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

