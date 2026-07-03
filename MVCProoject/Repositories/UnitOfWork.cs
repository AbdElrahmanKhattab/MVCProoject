using MVC.Data;
using MVC.Models;

namespace MVC.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _context;

        public UnitOfWork(GymDbContext context)
        {
            _context = context;
            Members = new GenericRepository<Member>(context);
            Memberships = new GenericRepository<Membership>(context);
            Sessions = new GenericRepository<GymSession>(context);
            Bookings = new GenericRepository<Booking>(context);
            Categories = new GenericRepository<Category>(context);
            Trainers = new GenericRepository<Trainer>(context);
            Plans = new GenericRepository<Plan>(context);
        }

        public IGenericRepository<Member> Members { get; }
        public IGenericRepository<Membership> Memberships { get; }
        public IGenericRepository<GymSession> Sessions { get; }
        public IGenericRepository<Booking> Bookings { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Trainer> Trainers { get; }
        public IGenericRepository<Plan> Plans { get; }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
