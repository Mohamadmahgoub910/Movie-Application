using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Core.Entities;
using MovieApp.Infrastructure.Data;
using MovieApp.Core.ViewModels;

namespace MovieApp.Areas.Public.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                // Statistics
                TotalMovies = await _context.Movies.CountAsync(),
                NowShowingMovies = await _context.Movies.CountAsync(m => m.Status == MovieStatus.NowShowing),
                ComingSoonMovies = await _context.Movies.CountAsync(m => m.Status == MovieStatus.ComingSoon),
                TotalActors = await _context.Actors.CountAsync(),
                TotalCinemas = await _context.Cinemas.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),

                // Recent Movies
                RecentMovies = await _context.Movies
                    .Include(m => m.Category)
                    .Include(m => m.Cinema)
                    .Include(m => m.MovieActors)
                        .ThenInclude(ma => ma.Actor)
                    .OrderByDescending(m => m.Id)
                    .Take(8)
                    .ToListAsync(),

                // Upcoming Movies
                UpcomingMovies = await _context.Movies
                    .Include(m => m.Category)
                    .Include(m => m.Cinema)
                    .Where(m => m.Status == MovieStatus.ComingSoon)
                    .OrderBy(m => m.ReleaseDateTime)
                    .Take(5)
                    .ToListAsync(),

                // Movies by Category
                MoviesByCategory = await _context.Categories
                    .Select(c => new CategoryMovieCount
                    {
                        CategoryName = c.Name,
                        MovieCount = c.Movies.Count
                    })
                    .OrderByDescending(c => c.MovieCount)
                    .ToListAsync()
            };

            return View(dashboardViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}