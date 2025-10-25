using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;

namespace Infrastructure.Repo
{
    public class Repo<T> : IRepo<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repo(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            _context.SaveChanges();

        }

        public virtual void Remove(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            _context.SaveChanges();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {

            return await _dbSet.AnyAsync(predicate);

        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {

            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
