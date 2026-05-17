using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(GymDbContext context)
        {
            await SeedMembersAsync(context);
            await SeedSessionsAsync(context);
            await SeedMembershipsAsync(context);
            await SeedBookingsAsync(context);
        }

        private static async Task SeedMembersAsync(GymDbContext context)
        {
            if (await context.Members.AnyAsync())
            {
                return;
            }

            context.Members.AddRange(
                new Member
                {
                    Name = "Omar Ahmed",
                    Email = "omar.ahmed@powerfitness.local",
                    Phone = "01000000001",
                    JoinDate = DateTime.Now.AddDays(-45)
                },
                new Member
                {
                    Name = "Ahmed Adel",
                    Email = "ahmed.adel@powerfitness.local",
                    Phone = "01000000002",
                    JoinDate = DateTime.Now.AddDays(-30)
                },
                new Member
                {
                    Name = "Salma Osama",
                    Email = "salma.osama@powerfitness.local",
                    Phone = "01000000003",
                    JoinDate = DateTime.Now.AddDays(-20)
                },
                new Member
                {
                    Name = "Maha Ahmed",
                    Email = "maha.ahmed@powerfitness.local",
                    Phone = "01000000004",
                    JoinDate = DateTime.Now.AddDays(-10)
                });

            await context.SaveChangesAsync();
        }

        private static async Task SeedSessionsAsync(GymDbContext context)
        {
            var now = DateTime.Now;

            if (await context.GymSessions.AnyAsync(x => x.EndDate > now))
            {
                return;
            }

            context.GymSessions.AddRange(
                new GymSession
                {
                    CategoryName = "Yoga",
                    TrainerName = "Omar",
                    Description = "Yoga session focused on flexibility, breathing, and steady strength.",
                    Capacity = 12,
                    StartDate = now.AddDays(1).Date.AddHours(18),
                    EndDate = now.AddDays(1).Date.AddHours(20)
                },
                new GymSession
                {
                    CategoryName = "Boxing",
                    TrainerName = "Maryam",
                    Description = "Boxing session for cardio, footwork, and core conditioning.",
                    Capacity = 12,
                    StartDate = now.AddMinutes(-30),
                    EndDate = now.AddMinutes(90)
                },
                new GymSession
                {
                    CategoryName = "Strength",
                    TrainerName = "Khaled",
                    Description = "Strength class for full-body resistance training.",
                    Capacity = 15,
                    StartDate = now.AddDays(2).Date.AddHours(19),
                    EndDate = now.AddDays(2).Date.AddHours(20).AddMinutes(30)
                });

            await context.SaveChangesAsync();
        }

        private static async Task SeedMembershipsAsync(GymDbContext context)
        {
            if (await context.Memberships.AnyAsync())
            {
                return;
            }

            var plan = await context.Plans
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

            if (plan is null)
            {
                return;
            }

            var members = await context.Members
                .OrderBy(x => x.Id)
                .Take(2)
                .ToListAsync();

            var now = DateTime.Now;

            foreach (var member in members)
            {
                context.Memberships.Add(new Membership
                {
                    MemberId = member.Id,
                    PlanId = plan.Id,
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(plan.DurationDays - 5)
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedBookingsAsync(GymDbContext context)
        {
            if (await context.Bookings.AnyAsync())
            {
                return;
            }

            var now = DateTime.Now;
            var activeMemberIds = await context.Memberships
                .Where(x => x.EndDate > now)
                .OrderBy(x => x.MemberId)
                .Select(x => x.MemberId)
                .Take(2)
                .ToListAsync();

            if (!activeMemberIds.Any())
            {
                return;
            }

            var upcomingSession = await context.GymSessions
                .Where(x => x.StartDate > now)
                .OrderBy(x => x.StartDate)
                .FirstOrDefaultAsync();

            var ongoingSession = await context.GymSessions
                .Where(x => x.StartDate <= now && x.EndDate > now)
                .OrderBy(x => x.StartDate)
                .FirstOrDefaultAsync();

            if (upcomingSession is not null)
            {
                context.Bookings.Add(new Booking
                {
                    MemberId = activeMemberIds[0],
                    GymSessionId = upcomingSession.Id,
                    BookingDate = now.AddDays(-1),
                    IsAttended = false
                });
            }

            if (ongoingSession is not null)
            {
                context.Bookings.Add(new Booking
                {
                    MemberId = activeMemberIds.Count > 1 ? activeMemberIds[1] : activeMemberIds[0],
                    GymSessionId = ongoingSession.Id,
                    BookingDate = now.AddHours(-2),
                    IsAttended = false
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
