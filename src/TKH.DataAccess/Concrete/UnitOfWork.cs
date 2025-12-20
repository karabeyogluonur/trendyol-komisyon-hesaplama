using TKH.Core.DataAccess;
using TKH.Core.Entities.Abstract;
using TKH.DataAccess.Contexts;

namespace TKH.DataAccess.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TKHDbContext _context;

        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TKHDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> GetRepository<T>() where T : class, IEntity, new()
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IRepository<T>)_repositories[typeof(T)];
            }

            var repository = new Repository<T>(_context);

            _repositories.Add(typeof(T), repository);

            return repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
