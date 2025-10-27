using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Areas.Public.Controllers
{
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
