/// <summary>
/// Analytics Service Interface
/// </summary>
using MovieApp.Core.ViewModels;

public interface IAnalyticsService
{
    Task<AnalyticsViewModel> GetAnalyticsDashboardAsync();
    Task<DashboardViewModel> GetDashboardStatisticsAsync();
}