using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class CreateSessionViewModel
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 25, ErrorMessage = "Capacity must be between 1 and 25")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date & Time")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Trainer is required")]
        [Display(Name = "Trainer")]
        [Range(1, int.MaxValue, ErrorMessage = "Trainer is required")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        [Range(1, int.MaxValue, ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public IEnumerable<TrainerSelectViewModel> Trainers { get; set; } = new List<TrainerSelectViewModel>();
        public IEnumerable<CategorySelectViewModel> Categories { get; set; } = new List<CategorySelectViewModel>();
    }
}
