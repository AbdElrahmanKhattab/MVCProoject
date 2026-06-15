using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.ViewModels;

namespace MVC.Controllers
{
    public class MemberPlanController : Controller
    {
        private readonly GymDbContext _context;

        public MemberPlanController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var memberships = await _context.Memberships
                .Include(x => x.Member)
                .Include(x => x.Plan)
                .Where(x => x.EndDate > now)
                .OrderBy(x => x.EndDate)
                .ToListAsync();

            return View(memberships);
        }

        public async Task<IActionResult> Create()
        {
            return View(await BuildCreateViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembershipCreateViewModel viewModel)
        {
            var now = DateTime.Now;
            var member = await _context.Members.FindAsync(viewModel.MemberId);
            var plan = await _context.Plans.FindAsync(viewModel.PlanId);

            if (member is null)
            {
                ModelState.AddModelError(nameof(viewModel.MemberId), "Selected member does not exist.");
            }

            if (plan is null)
            {
                ModelState.AddModelError(nameof(viewModel.PlanId), "Selected plan does not exist.");
            }
            else if (!plan.IsActive)
            {
                ModelState.AddModelError(nameof(viewModel.PlanId), "Only active plans can be assigned.");
            }

            var hasActiveMembership = await _context.Memberships
                .AnyAsync(x => x.MemberId == viewModel.MemberId && x.EndDate > now);

            if (hasActiveMembership)
            {
                ModelState.AddModelError(nameof(viewModel.MemberId), "This member already has an active membership.");
            }

            if (!ModelState.IsValid || member is null || plan is null)
            {
                return View(await BuildCreateViewModelAsync(viewModel));
            }

            var startDate = now;
            _context.Memberships.Add(new Membership
            {
                MemberId = member.Id,
                PlanId = plan.Id,
                StartDate = startDate,
                EndDate = startDate.AddDays(plan.DurationDays)
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Membership created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var membership = await _context.Memberships.FindAsync(id);

            if (membership is null)
            {
                TempData["Error"] = "Membership does not exist.";
                return RedirectToAction(nameof(Index));
            }

            if (membership.EndDate <= DateTime.Now)
            {
                TempData["Error"] = "Only active memberships can be deleted.";
                return RedirectToAction(nameof(Index));
            }

            _context.Memberships.Remove(membership);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Membership cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<MembershipCreateViewModel> BuildCreateViewModelAsync(MembershipCreateViewModel? selected = null)
        {
            var selectedMemberId = selected?.MemberId ?? 0;
            var selectedPlanId = selected?.PlanId ?? 0;

            var members = await _context.Members
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Id == selectedMemberId))
                .ToListAsync();

            var plans = await _context.Plans
                .Where(x => x.IsActive)
                .OrderBy(x => x.Price)
                .Select(x => new SelectListItem($"{x.Name} - {x.Price:0.##} EGP", x.Id.ToString(), x.Id == selectedPlanId))
                .ToListAsync();

            return new MembershipCreateViewModel
            {
                MemberId = selected?.MemberId ?? 0,
                PlanId = selected?.PlanId ?? 0,
                Members = members,
                Plans = plans
            };
        }
    }
}
