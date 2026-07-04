using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers
{
    [Authorize]
    public class TrainerController : Controller
    {
        private readonly GymDbContext _context;
        public TrainerController(GymDbContext context) => _context = context;

        public async Task<IActionResult> Index() => View(await _context.Trainers.OrderBy(x => x.Name).ToListAsync());
        public IActionResult Create() => View(new Trainer());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer)
        {
            if (await _context.Trainers.AnyAsync(x => x.Email == trainer.Email)) ModelState.AddModelError(nameof(trainer.Email), "Trainer email already exists.");
            if (!ModelState.IsValid) return View(trainer);
            _context.Trainers.Add(trainer); await _context.SaveChangesAsync(); TempData["SuccessMessage"] = "Trainer created successfully."; return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id) { var trainer = await _context.Trainers.FindAsync(id); if (trainer is null) return RedirectToAction(nameof(Index)); return View(trainer); }
        public async Task<IActionResult> Edit(int id) { var trainer = await _context.Trainers.FindAsync(id); if (trainer is null) return RedirectToAction(nameof(Index)); return View(trainer); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Trainer trainer)
        {
            if (await _context.Trainers.AnyAsync(x => x.Email == trainer.Email && x.Id != trainer.Id)) ModelState.AddModelError(nameof(trainer.Email), "Trainer email already exists.");
            if (!ModelState.IsValid) return View(trainer);
            _context.Trainers.Update(trainer); await _context.SaveChangesAsync(); TempData["SuccessMessage"] = "Trainer updated successfully."; return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id) { var trainer = await _context.Trainers.FindAsync(id); if (trainer is null) return RedirectToAction(nameof(Index)); return View(trainer); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id); if (trainer is not null) { _context.Trainers.Remove(trainer); await _context.SaveChangesAsync(); TempData["SuccessMessage"] = "Trainer deleted successfully."; }
            return RedirectToAction(nameof(Index));
        }
    }
}
