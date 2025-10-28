// ═══════════════════════════════════════════════════════════
// CinemasController
// ═══════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;

[Area("Admin")]
public class CinemasController : Controller
{
    private readonly ICinemaService _cinemaService;
    private readonly IFileService _fileService;

    public CinemasController(ICinemaService cinemaService, IFileService fileService)
    {
        _cinemaService = cinemaService;
        _fileService = fileService;
    }

    public async Task<IActionResult> Index()
    {
        var cinemas = await _cinemaService.GetAllCinemasAsync();
        return View(cinemas);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var cinema = await _cinemaService.GetCinemaWithMoviesAsync(id.Value);
        if (cinema == null) return NotFound();
        return View(cinema);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cinema cinema, IFormFile? logoFile)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (logoFile != null)
                {
                    cinema.Logo = await _fileService.UploadFileAsync(logoFile, "cinemas");
                }

                await _cinemaService.CreateCinemaAsync(cinema);
                TempData["Success"] = "تم إضافة السينما بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ: {ex.Message}";
            }
        }
        return View(cinema);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var cinema = await _cinemaService.GetCinemaByIdAsync(id.Value);
        if (cinema == null) return NotFound();
        return View(cinema);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cinema cinema, IFormFile? logoFile)
    {
        if (id != cinema.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existingCinema = await _cinemaService.GetCinemaByIdAsync(id);
                if (existingCinema == null) return NotFound();

                if (logoFile != null)
                {
                    if (!string.IsNullOrEmpty(existingCinema.Logo))
                    {
                        _fileService.DeleteFile(existingCinema.Logo);
                    }
                    cinema.Logo = await _fileService.UploadFileAsync(logoFile, "cinemas");
                }
                else
                {
                    cinema.Logo = existingCinema.Logo;
                }

                await _cinemaService.UpdateCinemaAsync(cinema);
                TempData["Success"] = "تم تحديث السينما بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ: {ex.Message}";
            }
        }
        return View(cinema);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var cinema = await _cinemaService.GetCinemaWithMoviesAsync(id.Value);
        if (cinema == null) return NotFound();
        return View(cinema);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _cinemaService.DeleteCinemaAsync(id);
            TempData["Success"] = "تم حذف السينما بنجاح";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"خطأ: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
