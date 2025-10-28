// ═══════════════════════════════════════════════════════════
// AnalyticsController
// ═══════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Interfaces;

[Area("Admin")]
public class AnalyticsController : Controller
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task<IActionResult> Dashboard()
    {
        var viewModel = await _analyticsService.GetAnalyticsDashboardAsync();
        return View(viewModel);
    }
}
