namespace MVC.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int GymSessionId { get; set; }
        public DateTime BookingDate { get; set; }
        public bool IsAttended { get; set; }

        public Member Member { get; set; } = null!;
        public GymSession GymSession { get; set; } = null!;
    }
}
