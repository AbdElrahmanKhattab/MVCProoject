using MVC.ViewModels;

namespace MVC.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetDashboardAsync();
    }
}
