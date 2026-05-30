using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.ViewModels
{
    public class MembershipCreateViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a member.")]
        public int MemberId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a plan.")]
        public int PlanId { get; set; }

        public IEnumerable<SelectListItem> Members { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Plans { get; set; } = new List<SelectListItem>();
    }
}
