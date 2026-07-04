namespace MVC.Models
{
    public class GymSession
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string TrainerName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
