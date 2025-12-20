using Microsoft.EntityFrameworkCore;
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

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
        {
            return filter == null
                ? await _dbSet.ToListAsync()
                : await _dbSet.Where(filter).ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.FirstOrDefaultAsync(filter);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
