using MVC.Models;

namespace MVC.Repositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<Member> Members { get; }
        IGenericRepository<Membership> Memberships { get; }
        IGenericRepository<GymSession> Sessions { get; }
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Trainer> Trainers { get; }
        IGenericRepository<Plan> Plans { get; }
        Task<int> SaveChangesAsync();
    }
}
