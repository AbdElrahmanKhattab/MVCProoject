using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.ViewModels;

namespace MVC.Controllers
{
    public class PlanController : Controller
    {
        private readonly GymDbContext _context;

        public PlanController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _context.Plans
                .OrderBy(plan => plan.Price)
                .ToListAsync();

            return View(plans);
        }

        public async Task<IActionResult> Details(int id)
        {
            var plan = await _context.Plans.FindAsync(id);

            if (plan is null)
            {
                return NotFound();
            }

            return View(plan);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _context.Plans.FindAsync(id);

            if (plan is null)
            {
                return NotFound();
            }

            return View(new PlanEditViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                IsActive = plan.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlanEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var plan = await _context.Plans.FindAsync(id);

            if (plan is null)
            {
                return NotFound();
            }

            var hasActiveMemberships = await _context.Memberships
                .AnyAsync(membership => membership.PlanId == id && membership.EndDate > DateTime.Now);

            if (hasActiveMemberships && !model.IsActive)
            {
                ModelState.AddModelError(nameof(model.IsActive), "Plans with active memberships cannot be deactivated.");
                return View(model);
            }

            plan.Name = model.Name;
            plan.Description = model.Description;
            plan.DurationDays = model.DurationDays;
            plan.Price = model.Price;
            plan.IsActive = model.IsActive;
            plan.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Plan updated successfully.";
            return RedirectToAction(nameof(Details), new { id = plan.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var plan = await _context.Plans.FindAsync(id);

            if (plan is null)
            {
                return NotFound();
            }

            var hasActiveMemberships = await _context.Memberships
                .AnyAsync(membership => membership.PlanId == id && membership.EndDate > DateTime.Now);

            if (plan.IsActive && hasActiveMemberships)
            {
                TempData["Error"] = "Plans with active memberships cannot be deactivated.";
                return RedirectToAction(nameof(Index));
            }

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = plan.IsActive ? "Plan activated successfully." : "Plan deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
