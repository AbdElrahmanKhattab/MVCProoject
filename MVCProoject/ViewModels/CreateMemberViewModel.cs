using System.ComponentModel.DataAnnotations;
using MVC.Models;

namespace MVC.ViewModels
{
    public class CreateMemberViewModel
    {
        [Required]
        public IFormFile Photo { get; set; } = null!;

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = null!;

        [Required, Phone, StringLength(30)]
        public string Phone { get; set; } = null!;

        [Required, DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-18);

        [Required]
        public Gender Gender { get; set; }

        [Required, StringLength(20)]
        public string BuildingNumber { get; set; } = null!;

        [Required, StringLength(100)]
        public string Street { get; set; } = null!;

        [Required, StringLength(60)]
        public string City { get; set; } = null!;

        public HealthRecordViewModel HealthRecordViewModel { get; set; } = new();
    }
}
