using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class HealthRecordViewModel
    {
        public int Id { get; set; }
        public int MemberId { get; set; }

        [Required, Range(50, 250)]
        public decimal Height { get; set; }

        [Required, Range(20, 300)]
        public decimal Weight { get; set; }

        [Required, Display(Name = "Blood Type")]
        public string BloodType { get; set; } = null!;

        [StringLength(500)]
        public string? Note { get; set; }
    }
}
