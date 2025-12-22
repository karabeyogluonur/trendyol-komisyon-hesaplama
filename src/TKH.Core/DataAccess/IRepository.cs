using System.Linq.Expressions;
using TKH.Core.Entities.Abstract;

namespace TKH.Core.DataAccess
{
    public interface IRepository<T> where T : class, IEntity, new()
    {
        Task AddAsync(T entity);
        Task AddAsync(IEnumerable<T> entities);

        void Update(T entity);
        void Update(IEnumerable<T> entities);
        void AddOrUpdate(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);

        Task<T> GetAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);
    }
}
