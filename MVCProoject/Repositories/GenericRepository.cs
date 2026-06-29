using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MVC.Data;

namespace MVC.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly GymDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(GymDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _dbSet;
        }

        public Task<List<T>> GetAllAsync()
        {
            return _dbSet.ToListAsync();
        }

        public Task<T?> GetByIdAsync(int id)
        {
            return _dbSet.FindAsync(id).AsTask();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AnyAsync(predicate);
        }

        public Task AddAsync(T entity)
        {
            return _dbSet.AddAsync(entity).AsTask();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
