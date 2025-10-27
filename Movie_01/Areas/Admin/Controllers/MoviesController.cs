using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.Core.Entities;
using MovieApp.Infrastructure.Data;
using MovieApp.ViewModels;

namespace MovieApp.Areas.Admin.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MoviesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string searchString, int? categoryId, int? cinemaId, MovieStatus? status)
        {
            var moviesQuery = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchString))
            {
                moviesQuery = moviesQuery.Where(m => m.Name.Contains(searchString) || m.Description.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                moviesQuery = moviesQuery.Where(m => m.CategoryId == categoryId.Value);
            }

            if (cinemaId.HasValue)
            {
                moviesQuery = moviesQuery.Where(m => m.CinemaId == cinemaId.Value);
            }

            if (status.HasValue)
            {
                moviesQuery = moviesQuery.Where(m => m.Status == status.Value);
            }

            var movies = await moviesQuery.OrderByDescending(m => m.Id).ToListAsync();

            // Pass filter data to view
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Cinemas = new SelectList(await _context.Cinemas.ToListAsync(), "Id", "Name");
            ViewBag.SearchString = searchString;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SelectedCinema = cinemaId;
            ViewBag.SelectedStatus = status;

            return View(movies);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.SubImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Cinemas = new SelectList(_context.Cinemas, "Id", "Name");
            ViewBag.Actors = new MultiSelectList(_context.Actors, "Id", "Name");
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                var movie = new Movie
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Status = model.Status,
                    ReleaseDateTime = model.ReleaseDateTime,
                    Duration = model.Duration,
                    CategoryId = model.CategoryId,
                    CinemaId = model.CinemaId
                };

                // Upload Main Image
                if (model.MainImageFile != null && model.MainImageFile.Length > 0)
                {
                    movie.MainImage = await UploadImage(model.MainImageFile, "movies");
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();

                // Add Actors (Many-to-Many)
                if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
                {
                    foreach (var actorId in model.SelectedActorIds)
                    {
                        _context.MovieActors.Add(new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                // Upload Sub Images
                if (model.SubImageFiles != null && model.SubImageFiles.Any())
                {
                    foreach (var imageFile in model.SubImageFiles)
                    {
                        if (imageFile.Length > 0)
                        {
                            var imageUrl = await UploadImage(imageFile, "movies/gallery");
                            _context.MovieImages.Add(new MovieImage
                            {
                                MovieId = movie.Id,
                                ImageUrl = imageUrl
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "تم إضافة الفيلم بنجاح";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            ViewBag.Cinemas = new SelectList(_context.Cinemas, "Id", "Name", model.CinemaId);
            ViewBag.Actors = new MultiSelectList(_context.Actors, "Id", "Name", model.SelectedActorIds);
            return View(model);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.SubImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

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

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
            ViewBag.Cinemas = new SelectList(_context.Cinemas, "Id", "Name", movie.CinemaId);
            ViewBag.Actors = new MultiSelectList(_context.Actors, "Id", "Name", viewModel.SelectedActorIds);

            return View(viewModel);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var movie = await _context.Movies
                        .Include(m => m.MovieActors)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (movie == null)
                    {
                        return NotFound();
                    }

                    movie.Name = model.Name;
                    movie.Description = model.Description;
                    movie.Price = model.Price;
                    movie.Status = model.Status;
                    movie.ReleaseDateTime = model.ReleaseDateTime;
                    movie.Duration = model.Duration;
                    movie.CategoryId = model.CategoryId;
                    movie.CinemaId = model.CinemaId;

                    // Upload new main image if provided
                    if (model.MainImageFile != null && model.MainImageFile.Length > 0)
                    {
                        // Delete old image
                        if (!string.IsNullOrEmpty(movie.MainImage))
                        {
                            DeleteImage(movie.MainImage);
                        }
                        movie.MainImage = await UploadImage(model.MainImageFile, "movies");
                    }

                    // Update Actors
                    _context.MovieActors.RemoveRange(movie.MovieActors);
                    if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
                    {
                        foreach (var actorId in model.SelectedActorIds)
                        {
                            _context.MovieActors.Add(new MovieActor
                            {
                                MovieId = movie.Id,
                                ActorId = actorId
                            });
                        }
                    }

                    // Upload new sub images if provided
                    if (model.SubImageFiles != null && model.SubImageFiles.Any())
                    {
                        foreach (var imageFile in model.SubImageFiles)
                        {
                            if (imageFile.Length > 0)
                            {
                                var imageUrl = await UploadImage(imageFile, "movies/gallery");
                                _context.MovieImages.Add(new MovieImage
                                {
                                    MovieId = movie.Id,
                                    ImageUrl = imageUrl
                                });
                            }
                        }
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث الفيلم بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(model.Id))
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

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            ViewBag.Cinemas = new SelectList(_context.Cinemas, "Id", "Name", model.CinemaId);
            ViewBag.Actors = new MultiSelectList(_context.Actors, "Id", "Name", model.SelectedActorIds);
            return View(model);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.SubImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.SubImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie != null)
            {
                // Delete main image
                if (!string.IsNullOrEmpty(movie.MainImage))
                {
                    DeleteImage(movie.MainImage);
                }

                // Delete sub images
                foreach (var subImage in movie.SubImages)
                {
                    DeleteImage(subImage.ImageUrl);
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الفيلم بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE Sub Image (AJAX)
        [HttpPost]
        public async Task<IActionResult> DeleteSubImage(int id)
        {
            var subImage = await _context.MovieImages.FindAsync(id);
            if (subImage != null)
            {
                DeleteImage(subImage.ImageUrl);
                _context.MovieImages.Remove(subImage);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
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