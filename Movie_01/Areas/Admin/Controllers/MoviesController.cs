using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.Core.Interfaces;
using MovieApp.Application.ViewModels;
using MovieApp.Infrastructure.Data;

namespace MovieApp.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin Movies Controller
    /// SOLID: Dependency Inversion - depends on abstractions (IMovieService)
    /// SOLID: Single Responsibility - handles HTTP requests/responses only
    /// </summary>
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ICategoryService _categoryService;
        private readonly ICinemaService _cinemaService;
        private readonly IActorService _actorService;
        private readonly ApplicationDbContext _context;

        public MoviesController(
            IMovieService movieService,
            ICategoryService categoryService,
            ICinemaService cinemaService,
            IActorService actorService,
            ApplicationDbContext context)
        {
            _movieService = movieService;
            _categoryService = categoryService;
            _cinemaService = cinemaService;
            _actorService = actorService;
            _context = context;
        }

        // ═══ INDEX - List all movies ═══
        // GET: /Admin/Movies/Index
        public async Task<IActionResult> Index(string searchString, int? categoryId, int? cinemaId)
        {
            var movies = await _movieService.GetAllMoviesAsync();

            // Apply filters
            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m =>
                    m.Name.Contains(searchString) ||
                    m.Description.Contains(searchString)
                );
            }

            if (categoryId.HasValue)
            {
                movies = movies.Where(m => m.CategoryId == categoryId);
            }

            if (cinemaId.HasValue)
            {
                movies = movies.Where(m => m.CinemaId == cinemaId);
            }

            // ViewBag for filters
            ViewBag.Categories = new SelectList(
                await _categoryService.GetAllCategoriesAsync(),
                "Id",
                "Name"
            );
            ViewBag.Cinemas = new SelectList(
                await _cinemaService.GetAllCinemasAsync(),
                "Id",
                "Name"
            );
            ViewBag.SearchString = searchString;

            return View(movies);
        }

        // ═══ DETAILS ═══
        // GET: /Admin/Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = await _movieService.GetMovieWithDetailsAsync(id.Value);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        // ═══ CREATE GET ═══
        // GET: /Admin/Movies/Create
        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View();
        }

        // ═══ CREATE POST ═══
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _movieService.CreateMovieAsync(model);
                    TempData["Success"] = "تم إضافة الفيلم بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"خطأ: {ex.Message}";
                }
            }

            await LoadViewBagData();
            return View(model);
        }

        // ═══ EDIT GET ═══
        // GET: /Admin/Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = await _movieService.GetMovieWithDetailsAsync(id.Value);
            if (movie == null)
                return NotFound();

            // Convert Movie to MovieViewModel
            var viewModel = new MovieViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                Status = movie.Status,
                ReleaseDateTime = movie.ReleaseDateTime,
                Duration = movie.Duration,
                CategoryId = movie.CategoryId,
                CinemaId = movie.CinemaId,
                MainImage = movie.MainImage,
                SelectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList(),
                ExistingSubImages = movie.SubImages.ToList()
            };

            await LoadViewBagData();
            return View(viewModel);
        }

        // ═══ EDIT POST ═══
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _movieService.UpdateMovieAsync(id, model);
                    TempData["Success"] = "تم تحديث الفيلم بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"خطأ: {ex.Message}";
                }
            }

            await LoadViewBagData();
            return View(model);
        }

        // ═══ DELETE GET ═══
        // GET: /Admin/Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = await _movieService.GetMovieWithDetailsAsync(id.Value);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        // ═══ DELETE POST ═══
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _movieService.DeleteMovieAsync(id);
                TempData["Success"] = "تم حذف الفيلم بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // ═══ AJAX - Delete Sub Image ═══
        [HttpPost]
        public async Task<IActionResult> DeleteSubImage(int id)
        {
            try
            {
                var image = await _context.MovieImages.FindAsync(id);
                if (image != null)
                {
                    _context.MovieImages.Remove(image);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        // ═══ Helper Method ═══
        private async Task LoadViewBagData()
        {
            ViewBag.Categories = new SelectList(
                await _categoryService.GetAllCategoriesAsync(),
                "Id",
                "Name"
            );

            ViewBag.Cinemas = new SelectList(
                await _cinemaService.GetAllCinemasAsync(),
                "Id",
                "Name"
            );

            ViewBag.Actors = new MultiSelectList(
                await _actorService.GetAllActorsAsync(),
                "Id",
                "Name"
            );
        }
    }
}