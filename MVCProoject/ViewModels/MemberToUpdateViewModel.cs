using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
    public class MemberToUpdateViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Photo { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be a valid Egyptian mobile number.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Building number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Building number must be greater than zero.")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        [StringLength(150)]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100)]
        public string City { get; set; } = null!;
    }
}
