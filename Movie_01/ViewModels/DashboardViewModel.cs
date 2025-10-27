using MovieApp.Core.Entities;

namespace MovieApp.ViewModels
{
    public class DashboardViewModel
    {
        // Statistics
        public int TotalMovies { get; set; }
        public int NowShowingMovies { get; set; }
        public int ComingSoonMovies { get; set; }
        public int TotalActors { get; set; }
        public int TotalCinemas { get; set; }
        public int TotalCategories { get; set; }

        // Recent Movies
        public List<Movie> RecentMovies { get; set; } = new List<Movie>();

        // Upcoming Movies
        public List<Movie> UpcomingMovies { get; set; } = new List<Movie>();

        // Movies by Category
        public List<CategoryMovieCount> MoviesByCategory { get; set; } = new List<CategoryMovieCount>();
    }

    public class CategoryMovieCount
    {
        public string CategoryName { get; set; }
        public int MovieCount { get; set; }
    }
}