using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Interfaces;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Public/Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: Public/Categories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryWithMoviesAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
    }
}