using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

//Provides common CRUD operations for all entities
namespace Ticket.Domain.Interface
{
    public interface IGenericRepo<T> where T : class
    {
        // Basic CRUD 
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // Additional query methods
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // For complex queries
        IQueryable<T> GetQueryable();
    }
}
