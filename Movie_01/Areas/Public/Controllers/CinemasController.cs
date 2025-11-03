using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Interfaces;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class CinemasController : Controller
    {
        private readonly ICinemaService _cinemaService;

        public CinemasController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        // GET: Public/Cinemas
        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaService.GetAllCinemasAsync();
            return View(cinemas);
        }

        // GET: Public/Cinemas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _cinemaService.GetCinemaWithMoviesAsync(id);

            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }
    }
}