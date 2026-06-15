using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.ViewModels;

namespace MVC.Controllers
{
    public class TrainerController : Controller
    {
        private readonly GymDbContext _context;

        public TrainerController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers
                .OrderBy(trainer => trainer.Name)
                .ToListAsync();

            return View(trainers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer is null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        public IActionResult Create()
        {
            return View(new TrainerFormViewModel
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25))
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerFormViewModel model)
        {
            await ValidateTrainerUniquenessAsync(model.Email, model.Phone);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var trainer = new Trainer
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                BuildingNumber = model.BuildingNumber,
                Street = model.Street,
                City = model.City,
                Specialty = model.Specialty,
                HireDate = DateTime.Now
            };

            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Trainer created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer is null)
            {
                return NotFound();
            }

            return View(new TrainerFormViewModel
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth,
                Gender = trainer.Gender,
                BuildingNumber = trainer.BuildingNumber,
                Street = trainer.Street,
                City = trainer.City,
                Specialty = trainer.Specialty,
                IsEditMode = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            model.IsEditMode = true;
            await ValidateTrainerUniquenessAsync(model.Email, model.Phone, model.Id);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer is null)
            {
                return NotFound();
            }

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.BuildingNumber = model.BuildingNumber;
            trainer.Street = model.Street;
            trainer.City = model.City;
            trainer.Specialty = model.Specialty;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Trainer updated successfully.";
            return RedirectToAction(nameof(Details), new { id = trainer.Id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer is null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer is null)
            {
                return NotFound();
            }

            var hasScheduledSessions = await _context.GymSessions
                .AnyAsync(session => session.TrainerName == trainer.Name && session.EndDate > DateTime.Now);

            if (hasScheduledSessions)
            {
                TempData["Error"] = "Trainers with scheduled sessions cannot be deleted.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Trainer deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateTrainerUniquenessAsync(string email, string phone, int? currentTrainerId = null)
        {
            var emailExists = await _context.Trainers
                .AnyAsync(trainer => trainer.Email == email && trainer.Id != currentTrainerId);

            if (emailExists)
            {
                ModelState.AddModelError(nameof(TrainerFormViewModel.Email), "Email is already used by another trainer.");
            }

            var phoneExists = await _context.Trainers
                .AnyAsync(trainer => trainer.Phone == phone && trainer.Id != currentTrainerId);

            if (phoneExists)
            {
                ModelState.AddModelError(nameof(TrainerFormViewModel.Phone), "Phone is already used by another trainer.");
            }
        }
    }
}
