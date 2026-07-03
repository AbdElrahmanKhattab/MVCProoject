using System.Linq.Expressions;

namespace MVC.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Query();
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
