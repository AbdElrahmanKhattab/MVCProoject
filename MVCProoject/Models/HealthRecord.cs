namespace MVC.Models
{
    public class HealthRecord
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string BloodType { get; set; } = null!;
        public string? Note { get; set; }
        public Member Member { get; set; } = null!;
    }
}
