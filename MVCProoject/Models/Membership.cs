namespace MVC.Models
{
    public class Membership
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Member Member { get; set; } = null!;
        public Plan Plan { get; set; } = null!;

        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
    }
}
