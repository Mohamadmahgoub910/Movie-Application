// ═══════════════════════════════════════════════════════════
// ActorService Implementation
// ═══════════════════════════════════════════════════════════

using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using MovieApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
public class ActorService : IActorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public ActorService(IUnitOfWork unitOfWork,
                       ApplicationDbContext context,
                       IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _fileService = fileService;
    }

    public async Task<IEnumerable<Actor>> GetAllActorsAsync()
    {
        return await _context.Actors
            .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
            .ToListAsync();
    }

    public async Task<Actor?> GetActorByIdAsync(int id)
    {
        return await _unitOfWork.Actors.GetByIdAsync(id);
    }

    public async Task<Actor?> GetActorWithMoviesAsync(int id)
    {
        return await _context.Actors
            .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
                    .ThenInclude(m => m.Category)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Actor> CreateActorAsync(Actor actor)
    {
        await _unitOfWork.Actors.AddAsync(actor);
        await _unitOfWork.SaveChangesAsync();
        return actor;
    }

    public async Task<Actor> UpdateActorAsync(Actor actor)
    {
        _unitOfWork.Actors.Update(actor);
        await _unitOfWork.SaveChangesAsync();
        return actor;
    }

    public async Task DeleteActorAsync(int id)
    {
        var actor = await GetActorByIdAsync(id);
        if (actor == null)
            throw new Exception("الممثل غير موجود");

        // Delete profile picture
        if (!string.IsNullOrEmpty(actor.ProfilePicture))
        {
            _fileService.DeleteFile(actor.ProfilePicture);
        }

        _unitOfWork.Actors.Delete(actor);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Actor>> GetTopActorsByMovieCountAsync(int count)
    {
        return await _context.Actors
            .Include(a => a.MovieActors)
            .OrderByDescending(a => a.MovieActors.Count)
            .Take(count)
            .ToListAsync();
    }
}