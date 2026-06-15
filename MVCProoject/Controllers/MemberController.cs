using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.ViewModels;

namespace MVC.Controllers
{
    public class MemberController : Controller
    {
        private readonly GymDbContext _context;

        public MemberController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _context.Members
                .Include(member => member.HealthRecord)
                .OrderBy(member => member.Name)
                .ToListAsync();

            return View(members);
        }

        public async Task<IActionResult> Details(int id)
        {
            var member = await _context.Members
                .Include(member => member.HealthRecord)
                .FirstOrDefaultAsync(member => member.Id == id);

            if (member is null)
            {
                return NotFound();
            }

            return View(member);
        }

        public async Task<IActionResult> HealthRecordDetails(int id)
        {
            var member = await _context.Members
                .Include(member => member.HealthRecord)
                .FirstOrDefaultAsync(member => member.Id == id);

            if (member?.HealthRecord is null)
            {
                return NotFound();
            }

            return View(member);
        }

        public IActionResult Create()
        {
            return View(new CreateMemberViewModel
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-18))
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberViewModel model)
        {
            await ValidateMemberUniquenessAsync(model.Email, model.Phone);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var now = DateTime.Now;
            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                BuildingNumber = model.BuildingNumber,
                Street = model.Street,
                City = model.City,
                JoinDate = now,
                HealthRecord = new HealthRecord
                {
                    Height = model.HealthRecord.Height,
                    Weight = model.HealthRecord.Weight,
                    BloodType = model.HealthRecord.BloodType,
                    Note = model.HealthRecord.Note,
                    LastUpdate = now
                }
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Member created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member is null)
            {
                return NotFound();
            }

            return View(new MemberToUpdateViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.BuildingNumber,
                Street = member.Street,
                City = member.City
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MemberToUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            await ValidateMemberUniquenessAsync(model.Email, model.Phone, model.Id);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var member = await _context.Members.FindAsync(id);

            if (member is null)
            {
                return NotFound();
            }

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.BuildingNumber = model.BuildingNumber;
            member.Street = model.Street;
            member.City = model.City;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Member updated successfully.";
            return RedirectToAction(nameof(Details), new { id = member.Id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.Members
                .Include(member => member.HealthRecord)
                .FirstOrDefaultAsync(member => member.Id == id);

            if (member is null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member is null)
            {
                return NotFound();
            }

            var hasActiveBookings = await _context.Bookings
                .Include(booking => booking.GymSession)
                .AnyAsync(booking => booking.MemberId == id && booking.GymSession.EndDate > DateTime.Now);

            if (hasActiveBookings)
            {
                TempData["Error"] = "Members with active bookings cannot be deleted.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Member deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateMemberUniquenessAsync(string email, string phone, int? currentMemberId = null)
        {
            var emailExists = await _context.Members
                .AnyAsync(member => member.Email == email && member.Id != currentMemberId);

            if (emailExists)
            {
                ModelState.AddModelError(nameof(CreateMemberViewModel.Email), "Email is already used by another member.");
            }

            var phoneExists = await _context.Members
                .AnyAsync(member => member.Phone == phone && member.Id != currentMemberId);

            if (phoneExists)
            {
                ModelState.AddModelError(nameof(CreateMemberViewModel.Phone), "Phone is already used by another member.");
            }
        }
    }
}
