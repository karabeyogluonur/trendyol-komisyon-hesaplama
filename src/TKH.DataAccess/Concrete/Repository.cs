using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using TKH.Core.DataAccess;
using TKH.Core.Entities.Abstract;
using TKH.DataAccess.Contexts;

namespace TKH.DataAccess.Concrete
{
    public class Repository<T> : IRepository<T> where T : class, IEntity, new()
    {
        protected readonly TKHDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(TKHDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddAsync(IEnumerable<T> entities)
        {
            bool autoDetectChangesEnabled = _context.ChangeTracker.AutoDetectChangesEnabled;
            try
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = false;
                await _dbSet.AddRangeAsync(entities);
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;
            }
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Update(IEnumerable<T> entities)
        {
            bool autoDetectChangesEnabled = _context.ChangeTracker.AutoDetectChangesEnabled;
            try
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = false;
                _dbSet.UpdateRange(entities);
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;
            }
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            bool autoDetectChangesEnabled = _context.ChangeTracker.AutoDetectChangesEnabled;
            try
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = false;
                _dbSet.RemoveRange(entities);
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;
            }
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                foreach (Expression<Func<T, object>> include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                foreach (Expression<Func<T, object>> include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }
    }
}
