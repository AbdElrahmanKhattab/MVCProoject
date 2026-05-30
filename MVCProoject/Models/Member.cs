namespace MVC.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int BuildingNumber { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public string? Photo { get; set; }

        public HealthRecord HealthRecord { get; set; } = null!;
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
