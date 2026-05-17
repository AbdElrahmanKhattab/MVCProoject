using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;

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
                .Where(plan => plan.IsActive)
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
    }
}
