using MovieApp.Models;

namespace MovieApp.ViewModels
{
    public class AnalyticsViewModel
    {
        // Main Statistics
        public int TotalMovies { get; set; }
        public int NowShowingMovies { get; set; }
        public int ComingSoonMovies { get; set; }
        public int EndedMovies { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal HighestPrice { get; set; }
        public decimal LowestPrice { get; set; }

        // Counts
        public int TotalActors { get; set; }
        public int TotalCinemas { get; set; }
        public int TotalCategories { get; set; }

        // Movies by Status (for charts)
        public List<MovieStatusCount> MoviesByStatus { get; set; } = new();

        // Movies by Category (for charts)
        public List<CategoryMovieCount> MoviesByCategory { get; set; } = new();

        // Movies by Cinema
        public List<CinemaMovieCount> MoviesByCinema { get; set; } = new();

        // Top Movies by Price
        public List<Movie> TopMoviesByPrice { get; set; } = new();

        // Recent Movies
        public List<Movie> RecentMovies { get; set; } = new();

        // Upcoming Movies
        public List<Movie> UpcomingMovies { get; set; } = new();

        // Movies List (All)
        public List<Movie> AllMovies { get; set; } = new();

        // Top Actors (by movie count)
        public List<ActorMovieCount> TopActors { get; set; } = new();

        // Monthly Revenue Data (for charts)
        public List<MonthlyData> MonthlyRevenue { get; set; } = new();
    }

    public class MovieStatusCount
    {
        public string StatusName { get; set; }
        public int Count { get; set; }
    }

    public class CinemaMovieCount
    {
        public string CinemaName { get; set; }
        public int MovieCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ActorMovieCount
    {
        public string ActorName { get; set; }
        public int MovieCount { get; set; }
        public string ProfilePicture { get; set; }
    }

    public class MonthlyData
    {
        public string Month { get; set; }
        public int MovieCount { get; set; }
        public decimal Revenue { get; set; }
    }
}