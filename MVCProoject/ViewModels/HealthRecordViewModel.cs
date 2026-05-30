using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class HealthRecordViewModel
    {
        [Required(ErrorMessage = "Height is required.")]
        [Range(40, 250, ErrorMessage = "Height must be between 40 and 250 cm.")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        [Range(20, 300, ErrorMessage = "Weight must be between 20 and 300 kg.")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Blood type is required.")]
        [StringLength(3)]
        public string BloodType { get; set; } = null!;

        [StringLength(250)]
        public string? Note { get; set; }
    }
}
