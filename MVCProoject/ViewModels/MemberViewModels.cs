using System.ComponentModel.DataAnnotations;
using MVC.Models;

namespace MVC.ViewModels
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Gender Gender { get; set; }
        public string Phone { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = null!;
        public string? PlanName { get; set; }
        public DateTime? MembershipStartDate { get; set; }
        public DateTime? MembershipEndDate { get; set; }
    }

    public class MemberToUpdateViewModel
    {
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string Name { get; set; } = null!;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = null!;

        [Required, Phone, StringLength(30)]
        public string Phone { get; set; } = null!;

        [Required, StringLength(20)]
        public string BuildingNumber { get; set; } = null!;

        [Required, StringLength(60)]
        public string City { get; set; } = null!;

        [Required, StringLength(100)]
        public string Street { get; set; } = null!;
    }
}
