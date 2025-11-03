using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Interfaces;

namespace MovieApp.Areas.Public.Controllers
{
    [Area("Public")]
    public class HomeController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public HomeController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _analyticsService.GetDashboardStatisticsAsync();
            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}