using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Models;

namespace MVC.ViewModels
{
    public class BookingCreateViewModel
    {
        [Range(1, int.MaxValue)]
        public int SessionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a member.")]
        public int MemberId { get; set; }

        public GymSession? Session { get; set; }
        public IEnumerable<SelectListItem> Members { get; set; } = new List<SelectListItem>();
    }
}
