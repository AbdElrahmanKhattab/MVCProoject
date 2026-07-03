using Microsoft.EntityFrameworkCore;
using MVC.Repositories;
using MVC.ViewModels;

namespace MVC.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalyticsViewModel> GetDashboardAsync()
        {
            var now = DateTime.Now;

            return new AnalyticsViewModel
            {
                TotalMembers = await _unitOfWork.Members.Query().CountAsync(),
                ActiveMembers = await _unitOfWork.Memberships.Query()
                    .Where(x => x.EndDate > now)
                    .Select(x => x.MemberId)
                    .Distinct()
                    .CountAsync(),
                TotalTrainers = await _unitOfWork.Trainers.Query().CountAsync(),
                UpcomingSessions = await _unitOfWork.Sessions.Query().CountAsync(x => x.StartDate > now),
                OngoingSessions = await _unitOfWork.Sessions.Query().CountAsync(x => x.StartDate <= now && x.EndDate >= now),
                CompletedSessions = await _unitOfWork.Sessions.Query().CountAsync(x => x.EndDate < now)
            };
        }
    }
}
