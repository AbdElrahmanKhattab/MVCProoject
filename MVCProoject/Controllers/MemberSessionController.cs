using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.ViewModels;

namespace MVC.Controllers
{
    [Authorize]
    public class MemberSessionController : Controller
    {
        private readonly GymDbContext _context;

        public MemberSessionController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var sessions = await _context.GymSessions
                .Include(x => x.Bookings)
                .Where(x => x.EndDate > now)
                .OrderBy(x => x.StartDate)
                .ToListAsync();

            return View(new MemberSessionIndexViewModel
            {
                UpcomingSessions = sessions
                    .Where(x => x.StartDate > now)
                    .Select(x => ToSessionCard(x, "Upcoming")),
                OngoingSessions = sessions
                    .Where(x => x.StartDate <= now && x.EndDate > now)
                    .Select(x => ToSessionCard(x, "Ongoing"))
            });
        }

        public async Task<IActionResult> GetMembersForUpcomingSession(int sessionId)
        {
            var session = await _context.GymSessions
                .Include(x => x.Bookings)
                .ThenInclude(x => x.Member)
                .FirstOrDefaultAsync(x => x.Id == sessionId);

            if (session is null)
            {
                TempData["ErrorMessage"] = "Session does not exist.";
                return RedirectToAction(nameof(Index));
            }

            if (session.StartDate <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Only upcoming sessions can be managed from this page.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SessionBookingsViewModel
            {
                Session = session,
                Bookings = session.Bookings.OrderBy(x => x.BookingDate)
            });
        }

        public async Task<IActionResult> GetMembersForOngoingSessions(int sessionId)
        {
            var now = DateTime.Now;
            var session = await _context.GymSessions
                .Include(x => x.Bookings)
                .ThenInclude(x => x.Member)
                .FirstOrDefaultAsync(x => x.Id == sessionId);

            if (session is null)
            {
                TempData["ErrorMessage"] = "Session does not exist.";
                return RedirectToAction(nameof(Index));
            }

            if (session.StartDate > now || session.EndDate <= now)
            {
                TempData["ErrorMessage"] = "Attendance can only be managed for ongoing sessions.";
                return RedirectToAction(nameof(Index));
            }

            return View(new SessionBookingsViewModel
            {
                Session = session,
                Bookings = session.Bookings.OrderBy(x => x.Member.Name)
            });
        }

        public async Task<IActionResult> Create(int sessionId)
        {
            var session = await _context.GymSessions.FindAsync(sessionId);

            if (session is null)
            {
                TempData["ErrorMessage"] = "Session does not exist.";
                return RedirectToAction(nameof(Index));
            }

            if (session.StartDate <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Bookings can only be created for future sessions.";
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildBookingCreateViewModelAsync(session));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel viewModel)
        {
            var now = DateTime.Now;
            var session = await _context.GymSessions
                .Include(x => x.Bookings)
                .FirstOrDefaultAsync(x => x.Id == viewModel.SessionId);
            var member = await _context.Members.FindAsync(viewModel.MemberId);

            if (session is null)
            {
                TempData["ErrorMessage"] = "Session does not exist.";
                return RedirectToAction(nameof(Index));
            }

            if (member is null)
            {
                ModelState.AddModelError(nameof(viewModel.MemberId), "Selected member does not exist.");
            }

            if (session.StartDate <= now)
            {
                ModelState.AddModelError(string.Empty, "A booking can only be created for a future session.");
            }

            if (session.Bookings.Count >= session.Capacity)
            {
                ModelState.AddModelError(string.Empty, "This session is already full.");
            }

            var hasActiveMembership = await _context.Memberships
                .AnyAsync(x => x.MemberId == viewModel.MemberId && x.EndDate > now);

            if (!hasActiveMembership)
            {
                ModelState.AddModelError(nameof(viewModel.MemberId), "The member must have an active membership to book sessions.");
            }

            var duplicateBooking = await _context.Bookings
                .AnyAsync(x => x.MemberId == viewModel.MemberId && x.GymSessionId == viewModel.SessionId);

            if (duplicateBooking)
            {
                ModelState.AddModelError(nameof(viewModel.MemberId), "This member already booked the selected session.");
            }

            if (!ModelState.IsValid || member is null)
            {
                return View(await BuildBookingCreateViewModelAsync(session, viewModel.MemberId));
            }

            _context.Bookings.Add(new Booking
            {
                MemberId = member.Id,
                GymSessionId = session.Id,
                BookingDate = now,
                IsAttended = false
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking created successfully.";
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { sessionId = session.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings
                .Include(x => x.GymSession)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (booking is null)
            {
                TempData["ErrorMessage"] = "Booking does not exist.";
                return RedirectToAction(nameof(Index));
            }

            if (booking.GymSession.StartDate <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Only future bookings can be cancelled.";
                return RedirectToAction(nameof(Index));
            }

            var sessionId = booking.GymSessionId;
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking cancelled successfully.";
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { sessionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(int id)
        {
            var now = DateTime.Now;
            var booking = await _context.Bookings
                .Include(x => x.GymSession)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (booking is null)
            {
                TempData["ErrorMessage"] = "Booking does not exist.";
                return RedirectToAction(nameof(Index));
            }

            if (booking.GymSession.StartDate > now || booking.GymSession.EndDate <= now)
            {
                TempData["ErrorMessage"] = "Attendance can only be marked for ongoing sessions.";
                return RedirectToAction(nameof(Index));
            }

            booking.IsAttended = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Attendance marked successfully.";
            return RedirectToAction(nameof(GetMembersForOngoingSessions), new { sessionId = booking.GymSessionId });
        }

        private static SessionCardViewModel ToSessionCard(GymSession session, string status)
        {
            return new SessionCardViewModel
            {
                Session = session,
                Status = status,
                BookedCount = session.Bookings.Count
            };
        }

        private async Task<BookingCreateViewModel> BuildBookingCreateViewModelAsync(GymSession session, int selectedMemberId = 0)
        {
            var now = DateTime.Now;
            var activeMemberIds = await _context.Memberships
                .Where(x => x.EndDate > now)
                .Select(x => x.MemberId)
                .Distinct()
                .ToListAsync();

            var members = await _context.Members
                .Where(x => activeMemberIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Id == selectedMemberId))
                .ToListAsync();

            return new BookingCreateViewModel
            {
                SessionId = session.Id,
                Session = session,
                MemberId = selectedMemberId,
                Members = members
            };
        }
    }
}


