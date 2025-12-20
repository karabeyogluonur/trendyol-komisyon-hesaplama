using System.Linq.Expressions;
using TKH.Core.Entities.Abstract;

namespace TKH.Core.DataAccess
{
    public interface IRepository<T> where T : class, IEntity, new()
    {
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
