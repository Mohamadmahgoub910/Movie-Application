using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Entities;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ICategoryService _categoryService;
        private readonly ICinemaService _cinemaService;

        public MoviesController(
            IMovieService movieService,
            ICategoryService categoryService,
            ICinemaService cinemaService)
        {
            _movieService = movieService;
            _categoryService = categoryService;
            _cinemaService = cinemaService;
        }

        // GET: Public/Movies
        public async Task<IActionResult> Index(string? search, int? categoryId, int? cinemaId, MovieStatus? status)
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Cinemas = await _cinemaService.GetAllCinemasAsync();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentCinema = cinemaId;
            ViewBag.CurrentStatus = status;

            IEnumerable<Movie> movies;

            if (!string.IsNullOrEmpty(search))
            {
                movies = await _movieService.SearchMoviesAsync(search);
            }
            else if (categoryId.HasValue)
            {
                movies = await _movieService.GetMoviesByCategoryAsync(categoryId.Value);
            }
            else if (cinemaId.HasValue)
            {
                movies = await _movieService.GetMoviesByCinemaAsync(cinemaId.Value);
            }
            else if (status.HasValue)
            {
                movies = await _movieService.GetMoviesByStatusAsync(status.Value);
            }
            else
            {
                movies = await _movieService.GetAllMoviesAsync();
            }

            return View(movies);
        }

        // GET: Public/Movies/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.GetMovieWithDetailsAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
    }
}