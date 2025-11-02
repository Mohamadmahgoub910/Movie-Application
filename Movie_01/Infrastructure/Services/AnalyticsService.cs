using Microsoft.EntityFrameworkCore;
using MovieApp.Core.Entities;
using MovieApp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using MovieApp.Core.ViewModels; // الـ namespace الصح

namespace MovieApp.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsDashboardAsync()
        {
            var movies = (await _unitOfWork.Movies.GetAllAsync()).ToList();
            var actors = (await _unitOfWork.Actors.GetAllAsync()).ToList();
            var cinemas = (await _unitOfWork.Cinemas.GetAllAsync()).ToList();
            var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();

            var viewModel = new AnalyticsViewModel
            {
                // Main Statistics
                TotalMovies = movies.Count,
                NowShowingMovies = movies.Count(m => m.Status == MovieStatus.NowShowing),
                ComingSoonMovies = movies.Count(m => m.Status == MovieStatus.ComingSoon),
                EndedMovies = movies.Count(m => m.Status == MovieStatus.Ended),

                // Price Statistics
                TotalRevenue = movies.Sum(m => m.Price),
                AveragePrice = movies.Any() ? movies.Average(m => m.Price) : 0,
                HighestPrice = movies.Any() ? movies.Max(m => m.Price) : 0,
                LowestPrice = movies.Any() ? movies.Min(m => m.Price) : 0,

                // Counts
                TotalActors = actors.Count,
                TotalCinemas = cinemas.Count,
                TotalCategories = categories.Count,

                // Movies by Status
                MoviesByStatus = movies
                    .GroupBy(m => m.Status)
                    .Select(g => new MovieStatusCount
                    {
                        StatusName = g.Key == MovieStatus.NowShowing ? "يُعرض الآن" :
                                    g.Key == MovieStatus.ComingSoon ? "قريباً" : "انتهى العرض",
                        Count = g.Count()
                    })
                    .ToList(),

                // Movies by Category
                MoviesByCategory = categories
                    .Select(c => new CategoryMovieCount
                    {
                        CategoryName = c.Name,
                        MovieCount = c.Movies?.Count ?? 0
                    })
                    .OrderByDescending(c => c.MovieCount)
                    .ToList(),

                // Movies by Cinema
                MoviesByCinema = cinemas
                    .Select(c => new CinemaMovieCount
                    {
                        CinemaName = c.Name,
                        MovieCount = c.Movies?.Count ?? 0,
                        TotalRevenue = c.Movies?.Sum(m => m.Price) ?? 0
                    })
                    .OrderByDescending(c => c.MovieCount)
                    .ToList(),

                // Top Movies by Price
                TopMoviesByPrice = movies
                    .OrderByDescending(m => m.Price)
                    .Take(10)
                    .ToList(),

                // All Movies
                AllMovies = movies
                    .OrderByDescending(m => m.ReleaseDateTime)
                    .ToList(),

                // Recent Movies
                RecentMovies = movies
                    .OrderByDescending(m => m.Id)
                    .Take(8)
                    .ToList(),

                // Upcoming Movies
                UpcomingMovies = movies
                    .Where(m => m.Status == MovieStatus.ComingSoon)
                    .OrderBy(m => m.ReleaseDateTime)
                    .Take(5)
                    .ToList(),

                // Top Actors
                TopActors = actors
                    .Select(a => new ActorMovieCount
                    {
                        ActorName = a.Name,
                        MovieCount = a.MovieActors?.Count ?? 0,
                        ProfilePicture = a.ProfilePicture
                    })
                    .OrderByDescending(a => a.MovieCount)
                    .Take(10)
                    .ToList(),

                // Monthly Revenue
                MonthlyRevenue = movies
                    .Where(m => m.ReleaseDateTime >= DateTime.Now.AddMonths(-6))
                    .GroupBy(m => new { m.ReleaseDateTime.Year, m.ReleaseDateTime.Month })
                    .Select(g => new MonthlyData
                    {
                        Month = g.Key.Month + "/" + g.Key.Year,
                        MovieCount = g.Count(),
                        Revenue = g.Sum(m => m.Price)
                    })
                    .OrderBy(m => m.Month)
                    .ToList()
            };

            return viewModel;
        }

        public async Task<DashboardViewModel> GetDashboardStatisticsAsync()
        {
            var movies = (await _unitOfWork.Movies.GetAllAsync()).ToList();
            var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();

            var viewModel = new DashboardViewModel
            {
                TotalMovies = movies.Count,
                NowShowingMovies = movies.Count(m => m.Status == MovieStatus.NowShowing),
                ComingSoonMovies = movies.Count(m => m.Status == MovieStatus.ComingSoon),
                TotalActors = (await _unitOfWork.Actors.GetAllAsync()).Count(),
                TotalCinemas = (await _unitOfWork.Cinemas.GetAllAsync()).Count(),
                TotalCategories = categories.Count,

                RecentMovies = movies
                    .OrderByDescending(m => m.Id)
                    .Take(8)
                    .ToList(),

                UpcomingMovies = movies
                    .Where(m => m.Status == MovieStatus.ComingSoon)
                    .OrderBy(m => m.ReleaseDateTime)
                    .Take(5)
                    .ToList(),

                MoviesByCategory = categories
                    .Select(c => new CategoryMovieCount
                    {
                        CategoryName = c.Name,
                        MovieCount = c.Movies?.Count ?? 0
                    })
                    .OrderByDescending(c => c.MovieCount)
                    .ToList()
            };

            return viewModel;
        }
    }
}