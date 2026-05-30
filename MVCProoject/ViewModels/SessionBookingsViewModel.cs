using MVC.Models;

namespace MVC.ViewModels
{
    public class SessionBookingsViewModel
    {
        public GymSession Session { get; set; } = null!;
        public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
