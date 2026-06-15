using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(GymDbContext context)
        {
            await SeedMembersAsync(context);
            await SeedTrainersAsync(context);
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
                    DateOfBirth = new DateOnly(1998, 2, 12),
                    Gender = Gender.Male,
                    BuildingNumber = 12,
                    Street = "Nile Street",
                    City = "Cairo",
                    JoinDate = DateTime.Now.AddDays(-45),
                    HealthRecord = new HealthRecord
                    {
                        Height = 178,
                        Weight = 82,
                        BloodType = "O+",
                        Note = "Ready for general training.",
                        LastUpdate = DateTime.Now
                    }
                },
                new Member
                {
                    Name = "Ahmed Adel",
                    Email = "ahmed.adel@powerfitness.local",
                    Phone = "01000000002",
                    DateOfBirth = new DateOnly(1997, 8, 21),
                    Gender = Gender.Male,
                    BuildingNumber = 14,
                    Street = "Tahrir Street",
                    City = "Cairo",
                    JoinDate = DateTime.Now.AddDays(-30),
                    HealthRecord = new HealthRecord
                    {
                        Height = 181,
                        Weight = 86,
                        BloodType = "B+",
                        Note = "Prefers strength training.",
                        LastUpdate = DateTime.Now
                    }
                },
                new Member
                {
                    Name = "Salma Osama",
                    Email = "salma.osama@powerfitness.local",
                    Phone = "01100000003",
                    DateOfBirth = new DateOnly(2000, 6, 5),
                    Gender = Gender.Female,
                    BuildingNumber = 8,
                    Street = "Garden Road",
                    City = "Giza",
                    JoinDate = DateTime.Now.AddDays(-20),
                    HealthRecord = new HealthRecord
                    {
                        Height = 165,
                        Weight = 61,
                        BloodType = "A+",
                        Note = "Prefers cardio sessions.",
                        LastUpdate = DateTime.Now
                    }
                },
                new Member
                {
                    Name = "Maha Ahmed",
                    Email = "maha.ahmed@powerfitness.local",
                    Phone = "01200000004",
                    DateOfBirth = new DateOnly(1999, 9, 16),
                    Gender = Gender.Female,
                    BuildingNumber = 18,
                    Street = "October Road",
                    City = "Giza",
                    JoinDate = DateTime.Now.AddDays(-10),
                    HealthRecord = new HealthRecord
                    {
                        Height = 168,
                        Weight = 64,
                        BloodType = "AB+",
                        Note = "New member assessment completed.",
                        LastUpdate = DateTime.Now
                    }
                });

            await context.SaveChangesAsync();
        }

        private static async Task SeedTrainersAsync(GymDbContext context)
        {
            if (await context.Trainers.AnyAsync())
            {
                return;
            }

            context.Trainers.AddRange(
                new Trainer
                {
                    Name = "Maryam Ali",
                    Email = "maryam.ali@powerfitness.local",
                    Phone = "01500000005",
                    DateOfBirth = new DateOnly(1994, 3, 18),
                    Gender = Gender.Female,
                    BuildingNumber = 22,
                    Street = "Fitness Avenue",
                    City = "Cairo",
                    Specialty = Specialty.Yoga,
                    HireDate = DateTime.Now.AddMonths(-4)
                },
                new Trainer
                {
                    Name = "Khaled Hassan",
                    Email = "khaled.hassan@powerfitness.local",
                    Phone = "01500000006",
                    DateOfBirth = new DateOnly(1991, 11, 20),
                    Gender = Gender.Male,
                    BuildingNumber = 35,
                    Street = "Champion Street",
                    City = "Alexandria",
                    Specialty = Specialty.Boxing,
                    HireDate = DateTime.Now.AddMonths(-8)
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
                    TrainerName = "Maryam Ali",
                    Description = "Yoga session focused on flexibility, breathing, and steady strength.",
                    Capacity = 12,
                    StartDate = now.AddDays(1).Date.AddHours(18),
                    EndDate = now.AddDays(1).Date.AddHours(20)
                },
                new GymSession
                {
                    CategoryName = "Boxing",
                    TrainerName = "Khaled Hassan",
                    Description = "Boxing session for cardio, footwork, and core conditioning.",
                    Capacity = 12,
                    StartDate = now.AddMinutes(-30),
                    EndDate = now.AddMinutes(90)
                },
                new GymSession
                {
                    CategoryName = "Strength",
                    TrainerName = "Khaled Hassan",
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
