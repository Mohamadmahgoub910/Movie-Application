// ═══════════════════════════════════════════════════════════
// ActorsController
// ═══════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ActorsController : Controller
{
    private readonly IActorService _actorService;
    private readonly IFileService _fileService;

    public ActorsController(IActorService actorService, IFileService fileService)
    {
        _actorService = actorService;
        _fileService = fileService;
    }

    public async Task<IActionResult> Index()
    {
        var actors = await _actorService.GetAllActorsAsync();
        return View(actors);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var actor = await _actorService.GetActorWithMoviesAsync(id.Value);
        if (actor == null) return NotFound();
        return View(actor);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Actor actor, IFormFile? profilePictureFile)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (profilePictureFile != null)
                {
                    actor.ProfilePicture = await _fileService.UploadFileAsync(profilePictureFile, "actors");
                }

                await _actorService.CreateActorAsync(actor);
                TempData["Success"] = "تم إضافة الممثل بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ: {ex.Message}";
            }
        }
        return View(actor);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var actor = await _actorService.GetActorByIdAsync(id.Value);
        if (actor == null) return NotFound();
        return View(actor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Actor actor, IFormFile? profilePictureFile)
    {
        if (id != actor.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existingActor = await _actorService.GetActorByIdAsync(id);
                if (existingActor == null) return NotFound();

                if (profilePictureFile != null)
                {
                    if (!string.IsNullOrEmpty(existingActor.ProfilePicture))
                    {
                        _fileService.DeleteFile(existingActor.ProfilePicture);
                    }
                    actor.ProfilePicture = await _fileService.UploadFileAsync(profilePictureFile, "actors");
                }
                else
                {
                    actor.ProfilePicture = existingActor.ProfilePicture;
                }

                await _actorService.UpdateActorAsync(actor);
                TempData["Success"] = "تم تحديث الممثل بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ: {ex.Message}";
            }
        }
        return View(actor);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var actor = await _actorService.GetActorWithMoviesAsync(id.Value);
        if (actor == null) return NotFound();
        return View(actor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _actorService.DeleteActorAsync(id);
            TempData["Success"] = "تم حذف الممثل بنجاح";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"خطأ: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}