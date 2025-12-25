using Microsoft.EntityFrameworkCore;
using TKH.Core.DataAccess;
using TKH.Core.Entities.Abstract;
using TKH.DataAccess.Contexts;

namespace TKH.DataAccess.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TKHDbContext _context;
        private bool _disposed = false;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TKHDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TKHDbContext DbContext => _context;

        public IRepository<T> GetRepository<T>() where T : class, IEntity, new()
        {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object>();

            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new Repository<T>(_context);
                _repositories[type] = repositoryInstance;
            }

            return (IRepository<T>)_repositories[type];
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _context.Database.ExecuteSqlRaw(sql, parameters);
        }

        public IQueryable<T> FromSql<T>(string sql, params object[] parameters) where T : class, IEntity, new()
        {
            return _context.Set<T>().FromSqlRaw(sql, parameters);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _context.DisposeAsync();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
