// ═══════════════════════════════════════════════════════════
// AnalyticsService Implementation
// ═══════════════════════════════════════════════════════════

using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using MovieApp.Infrastructure.Data;
using MovieApp.ViewModels;

public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _context;

    public AnalyticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsViewModel> GetAnalyticsDashboardAsync()
    {
        var viewModel = new AnalyticsViewModel
        {
            // Main Statistics
            TotalMovies = await _context.Movies.CountAsync(),
            NowShowingMovies = await _context.Movies.CountAsync(m => m.Status == MovieStatus.NowShowing),
            ComingSoonMovies = await _context.Movies.CountAsync(m => m.Status == MovieStatus.ComingSoon),
            EndedMovies = await _context.Movies.CountAsync(m => m.Status == MovieStatus.Ended),

            // Price Statistics
            TotalRevenue = await _context.Movies.SumAsync(m => m.Price),
            AveragePrice = await _context.Movies.AverageAsync(m => m.Price),
            HighestPrice = await _context.Movies.MaxAsync(m => m.Price),
            LowestPrice = await _context.Movies.MinAsync(m => m.Price),

            // Counts
            TotalActors = await _context.Actors.CountAsync(),
            TotalCinemas = await _context.Cinemas.CountAsync(),
            TotalCategories = await _context.Categories.CountAsync(),

            // Movies by Status
            MoviesByStatus = await _context.Movies
                .GroupBy(m => m.Status)
                .Select(g => new MovieStatusCount
                {
                    StatusName = g.Key == MovieStatus.NowShowing ? "يُعرض الآن" :
                                g.Key == MovieStatus.ComingSoon ? "قريباً" : "انتهى العرض",
                    Count = g.Count()
                })
                .ToListAsync(),

            // Movies by Category
            MoviesByCategory = await _context.Categories
                .Select(c => new CategoryMovieCount
                {
                    CategoryName = c.Name,
                    MovieCount = c.Movies.Count
                })
                .OrderByDescending(c => c.MovieCount)
                .ToListAsync(),

            // Movies by Cinema
            MoviesByCinema = await _context.Cinemas
                .Select(c => new CinemaMovieCount
                {
                    CinemaName = c.Name,
                    MovieCount = c.Movies.Count,
                    TotalRevenue = c.Movies.Sum(m => m.Price)
                })
                .OrderByDescending(c => c.MovieCount)
                .ToListAsync(),

            // Top Movies by Price
            TopMoviesByPrice = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .OrderByDescending(m => m.Price)
                .Take(10)
                .ToListAsync(),

            // All Movies
            AllMovies = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .OrderByDescending(m => m.ReleaseDateTime)
                .ToListAsync(),

            // Top Actors
            TopActors = await _context.Actors
                .Select(a => new ActorMovieCount
                {
                    ActorName = a.Name,
                    MovieCount = a.MovieActors.Count,
                    ProfilePicture = a.ProfilePicture
                })
                .OrderByDescending(a => a.MovieCount)
                .Take(10)
                .ToListAsync(),

            // Monthly Revenue
            MonthlyRevenue = (await _context.Movies
                .Where(m => m.ReleaseDateTime >= DateTime.Now.AddMonths(-6))
                .GroupBy(m => new { m.ReleaseDateTime.Year, m.ReleaseDateTime.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MovieCount = g.Count(),
                    Revenue = g.Sum(m => m.Price)
                })
                .ToListAsync())
                .Select(x => new MonthlyData
                {
                    Month = x.Month + "/" + x.Year,
                    MovieCount = x.MovieCount,
                    Revenue = x.Revenue
                })
                .OrderBy(m => m.Month)
                .ToList()
        };

        return viewModel;
    }

    public async Task<DashboardViewModel> GetDashboardStatisticsAsync()
    {
        var viewModel = new DashboardViewModel
        {
            TotalMovies = await _context.Movies.CountAsync(),
            NowShowingMovies = await _context.Movies.CountAsync(m => m.Status == MovieStatus.NowShowing),
            ComingSoonMovies = await _context.Movies.CountAsync(m => m.Status == MovieStatus.ComingSoon),
            TotalActors = await _context.Actors.CountAsync(),
            TotalCinemas = await _context.Cinemas.CountAsync(),
            TotalCategories = await _context.Categories.CountAsync(),

            RecentMovies = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .OrderByDescending(m => m.Id)
                .Take(8)
                .ToListAsync(),

            UpcomingMovies = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Where(m => m.Status == MovieStatus.ComingSoon)
                .OrderBy(m => m.ReleaseDateTime)
                .Take(5)
                .ToListAsync(),

            MoviesByCategory = await _context.Categories
                .Select(c => new CategoryMovieCount
                {
                    CategoryName = c.Name,
                    MovieCount = c.Movies.Count
                })
                .OrderByDescending(c => c.MovieCount)
                .ToListAsync()
        };

        return viewModel;
    }
}
}