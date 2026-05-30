using System.ComponentModel.DataAnnotations;
using MVC.Models;

namespace MVC.ViewModels
{
    public class CreateMemberViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be a valid Egyptian mobile number.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Building number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Building number must be greater than zero.")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        [StringLength(150)]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100)]
        public string City { get; set; } = null!;

        [Required]
        public HealthRecordViewModel HealthRecord { get; set; } = new();
    }
}
