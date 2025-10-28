// ═══════════════════════════════════════════════════════════
// Areas/Admin/Controllers/HomeController.cs
// ═══════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Interfaces;

namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public HomeController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // GET: /Admin or /Admin/Home/Index
        public async Task<IActionResult> Index()
        {
            var dashboardData = await _analyticsService.GetDashboardStatisticsAsync();
            return View(dashboardData);
        }
    }
}
