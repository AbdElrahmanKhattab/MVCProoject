using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class PlanEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Plan name is required.")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 3650, ErrorMessage = "Duration must be greater than zero.")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 100000, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }
}
