using MVC.Models;

namespace MVC.ViewModels
{
    public class MemberSessionIndexViewModel
    {
        public IEnumerable<SessionCardViewModel> UpcomingSessions { get; set; } = new List<SessionCardViewModel>();
        public IEnumerable<SessionCardViewModel> OngoingSessions { get; set; } = new List<SessionCardViewModel>();
    }

    public class SessionCardViewModel
    {
        public GymSession Session { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int BookedCount { get; set; }
        public int AvailableSlots => Session.Capacity - BookedCount;
        public TimeSpan Duration => Session.EndDate - Session.StartDate;
    }
}
