namespace MVC.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string BuildingNumber { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? PhotoFileName { get; set; }
        public string? PhotoContentType { get; set; }
        public byte[]? PhotoData { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public HealthRecord? HealthRecord { get; set; }
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

