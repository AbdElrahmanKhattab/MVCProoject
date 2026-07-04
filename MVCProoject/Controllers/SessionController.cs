using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly GymDbContext _context;
        public SessionController(GymDbContext context) => _context = context;
        public async Task<IActionResult> Index() => View(await _context.GymSessions.Include(x => x.Bookings).OrderBy(x => x.StartDate).ToListAsync());
        public IActionResult Create() => View(new GymSession { StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(1).AddHours(1), Capacity = 10 });
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GymSession session) { if(session.EndDate <= session.StartDate) ModelState.AddModelError(nameof(session.EndDate), "End date must be after start date."); if(!ModelState.IsValid) return View(session); _context.GymSessions.Add(session); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Details(int id) { var s=await _context.GymSessions.Include(x=>x.Bookings).FirstOrDefaultAsync(x=>x.Id==id); if(s is null) return RedirectToAction(nameof(Index)); return View(s); }
        public async Task<IActionResult> Edit(int id) { var s=await _context.GymSessions.FindAsync(id); if(s is null) return RedirectToAction(nameof(Index)); return View(s); }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GymSession session) { if(session.EndDate <= session.StartDate) ModelState.AddModelError(nameof(session.EndDate), "End date must be after start date."); if(!ModelState.IsValid) return View(session); _context.GymSessions.Update(session); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Delete(int id) { var s=await _context.GymSessions.FindAsync(id); if(s is null) return RedirectToAction(nameof(Index)); return View(s); }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var s=await _context.GymSessions.FindAsync(id); if(s is not null){_context.GymSessions.Remove(s); await _context.SaveChangesAsync();} return RedirectToAction(nameof(Index)); }
    }
}
