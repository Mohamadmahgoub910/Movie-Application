// ═══════════════════════════════════════════════════════════
// CinemaService Implementation
// ═══════════════════════════════════════════════════════════

using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using MovieApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
public class CinemaService : ICinemaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public CinemaService(IUnitOfWork unitOfWork,
                        ApplicationDbContext context,
                        IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _fileService = fileService;
    }

    public async Task<IEnumerable<Cinema>> GetAllCinemasAsync()
    {
        return await _context.Cinemas
            .Include(c => c.Movies)
            .ToListAsync();
    }

    public async Task<Cinema?> GetCinemaByIdAsync(int id)
    {
        return await _unitOfWork.Cinemas.GetByIdAsync(id);
    }

    public async Task<Cinema?> GetCinemaWithMoviesAsync(int id)
    {
        return await _context.Cinemas
            .Include(c => c.Movies)
                .ThenInclude(m => m.Category)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cinema> CreateCinemaAsync(Cinema cinema)
    {
        await _unitOfWork.Cinemas.AddAsync(cinema);
        await _unitOfWork.SaveChangesAsync();
        return cinema;
    }

    public async Task<Cinema> UpdateCinemaAsync(Cinema cinema)
    {
        _unitOfWork.Cinemas.Update(cinema);
        await _unitOfWork.SaveChangesAsync();
        return cinema;
    }

    public async Task DeleteCinemaAsync(int id)
    {
        var cinema = await GetCinemaWithMoviesAsync(id);
        if (cinema == null)
            throw new Exception("السينما غير موجودة");

        if (cinema.Movies.Any())
            throw new Exception("لا يمكن حذف السينما لأنها تحتوي على أفلام");

        // Delete logo
        if (!string.IsNullOrEmpty(cinema.Logo))
        {
            _fileService.DeleteFile(cinema.Logo);
        }

        _unitOfWork.Cinemas.Delete(cinema);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> CanDeleteCinemaAsync(int id)
    {
        return !await _unitOfWork.Movies.AnyAsync(m => m.CinemaId == id);
    }
}
