using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;

namespace MovieApp.Controllers
{
    public class CinemasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CinemasController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Cinemas
        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas
                .Include(c => c.Movies)
                .ToListAsync();
            return View(cinemas);
        }

        // GET: Cinemas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas
                .Include(c => c.Movies)
                    .ThenInclude(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // GET: Cinemas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cinemas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile? logoFile)
        {
            if (ModelState.IsValid)
            {
                // Upload Logo
                if (logoFile != null && logoFile.Length > 0)
                {
                    cinema.Logo = await UploadImage(logoFile, "cinemas");
                }

                _context.Add(cinema);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إضافة السينما بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        // GET: Cinemas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        // POST: Cinemas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema cinema, IFormFile? logoFile)
        {
            if (id != cinema.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Upload new logo if provided
                    if (logoFile != null && logoFile.Length > 0)
                    {
                        // Delete old logo
                        if (!string.IsNullOrEmpty(cinema.Logo))
                        {
                            DeleteImage(cinema.Logo);
                        }
                        cinema.Logo = await UploadImage(logoFile, "cinemas");
                    }

                    _context.Update(cinema);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث السينما بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CinemaExists(cinema.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        // GET: Cinemas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas
                .Include(c => c.Movies)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // POST: Cinemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema != null)
            {
                // Check if cinema has movies
                var hasMovies = await _context.Movies.AnyAsync(m => m.CinemaId == id);
                if (hasMovies)
                {
                    TempData["Error"] = "لا يمكن حذف السينما لأنها تحتوي على أفلام";
                    return RedirectToAction(nameof(Index));
                }

                // Delete logo
                if (!string.IsNullOrEmpty(cinema.Logo))
                {
                    DeleteImage(cinema.Logo);
                }

                _context.Cinemas.Remove(cinema);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف السينما بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CinemaExists(int id)
        {
            return _context.Cinemas.Any(e => e.Id == id);
        }

        // Helper method to upload images
        private async Task<string> UploadImage(IFormFile file, string folder)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folder);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/{folder}/{uniqueFileName}";
        }

        // Helper method to delete images
        private void DeleteImage(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
        }
    }
}