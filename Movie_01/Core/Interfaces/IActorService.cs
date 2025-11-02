
using MovieApp.Core.Entities;

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
