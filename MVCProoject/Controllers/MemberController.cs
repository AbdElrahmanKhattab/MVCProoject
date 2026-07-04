using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.ViewModels;

namespace MVC.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly GymDbContext _context;
        private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/gif"
        };

        public MemberController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _context.Members
                .OrderBy(x => x.Name)
                .Select(x => ToMemberViewModel(x))
                .ToListAsync();

            return View(members);
        }

        public IActionResult Create()
        {
            return View(new CreateMemberViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model)
        {
            await ValidatePhotoAsync(model.Photo);

            if (await _context.Members.AnyAsync(x => x.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "A member with this email already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            await using var stream = new MemoryStream();
            await model.Photo.CopyToAsync(stream);

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
                PhotoFileName = model.Photo.FileName,
                PhotoContentType = model.Photo.ContentType,
                PhotoData = stream.ToArray(),
                HealthRecord = new HealthRecord
                {
                    Height = model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Note = model.HealthRecordViewModel.Note
                }
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Member created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MemberDetails(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(ToMemberViewModel(member));
        }

        public async Task<IActionResult> MemberEdit(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(new MemberToUpdateViewModel
            {
                Id = member.Id,
                Photo = member.PhotoFileName,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.BuildingNumber,
                City = member.City,
                Street = member.Street
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MemberEdit(MemberToUpdateViewModel model)
        {
            if (await _context.Members.AnyAsync(x => x.Email == model.Email && x.Id != model.Id))
            {
                ModelState.AddModelError(nameof(model.Email), "A member with this email already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var member = await _context.Members.FindAsync(model.Id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member does not exist.";
                return RedirectToAction(nameof(Index));
            }

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.BuildingNumber = model.BuildingNumber;
            member.City = model.City;
            member.Street = model.Street;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Member updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> HealthRecordDetails(int id)
        {
            var record = await _context.HealthRecords.FirstOrDefaultAsync(x => x.MemberId == id);
            if (record is null)
            {
                TempData["ErrorMessage"] = "Health record does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(ToHealthRecordViewModel(record));
        }

        public async Task<IActionResult> HealthRecordEdit(int id)
        {
            var record = await _context.HealthRecords.FirstOrDefaultAsync(x => x.MemberId == id);
            if (record is null)
            {
                TempData["ErrorMessage"] = "Health record does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(ToHealthRecordViewModel(record));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HealthRecordEdit(HealthRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var record = await _context.HealthRecords.FirstOrDefaultAsync(x => x.MemberId == model.MemberId);
            if (record is null)
            {
                TempData["ErrorMessage"] = "Health record does not exist.";
                return RedirectToAction(nameof(Index));
            }

            record.Height = model.Height;
            record.Weight = model.Weight;
            record.BloodType = model.BloodType;
            record.Note = model.Note;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Health record updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!await _context.Members.AnyAsync(x => x.Id == id))
            {
                TempData["ErrorMessage"] = "Member does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member does not exist.";
                return RedirectToAction(nameof(Index));
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Member deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Picture(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member?.PhotoData is null || string.IsNullOrWhiteSpace(member.PhotoContentType))
            {
                return NotFound();
            }

            return File(member.PhotoData, member.PhotoContentType);
        }

        private async Task ValidatePhotoAsync(IFormFile? photo)
        {
            if (photo is null || photo.Length == 0)
            {
                ModelState.AddModelError(nameof(CreateMemberViewModel.Photo), "Profile photo is required.");
                return;
            }

            if (photo.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(CreateMemberViewModel.Photo), "Profile photo must be 5 MB or less.");
            }

            if (!AllowedImageTypes.Contains(photo.ContentType))
            {
                ModelState.AddModelError(nameof(CreateMemberViewModel.Photo), "Only JPG, PNG, and GIF images are allowed.");
            }

            await Task.CompletedTask;
        }

        private static MemberViewModel ToMemberViewModel(Member member)
        {
            return new MemberViewModel
            {
                Id = member.Id,
                Photo = member.PhotoFileName,
                Name = member.Name,
                Email = member.Email,
                Gender = member.Gender,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth,
                Address = $"{member.BuildingNumber}, {member.Street}, {member.City}"
            };
        }

        private static HealthRecordViewModel ToHealthRecordViewModel(HealthRecord record)
        {
            return new HealthRecordViewModel
            {
                Id = record.Id,
                MemberId = record.MemberId,
                Height = record.Height,
                Weight = record.Weight,
                BloodType = record.BloodType,
                Note = record.Note
            };
        }
    }
}
