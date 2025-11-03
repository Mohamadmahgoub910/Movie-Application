using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Interfaces;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class ActorsController : Controller
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        // GET: Public/Actors
        public async Task<IActionResult> Index()
        {
            var actors = await _actorService.GetAllActorsAsync();
            return View(actors);
        }

        // GET: Public/Actors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var actor = await _actorService.GetActorWithMoviesAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }
    }
}