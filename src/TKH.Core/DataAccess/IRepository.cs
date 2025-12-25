using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using TKH.Core.Entities.Abstract;
using TKH.Core.Utilities.Paging;

namespace TKH.Core.DataAccess
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {

        IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageIndex = 0,
          int pageSize = 20,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageIndex = 0,
          int pageSize = 20,
          bool disableTracking = true,
          CancellationToken cancellationToken =
          default(CancellationToken),
          bool ignoreQueryFilters = false);

        IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageIndex = 0,
          int pageSize = 20,
          bool disableTracking = true,
          bool ignoreQueryFilters = false) where TResult : class;

        Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageIndex = 0,
          int pageSize = 20,
          bool disableTracking = true,
          CancellationToken cancellationToken =
          default(CancellationToken),
          bool ignoreQueryFilters = false) where TResult : class;

        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        TEntity Find(params object[] keyValues);

        ValueTask<TEntity> FindAsync(params object[] keyValues);

        ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken);

        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        IQueryable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        Task<IList<TEntity>> GetAllAsync();

        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false);

        int Count(Expression<Func<TEntity, bool>> predicate = null);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);

        long LongCount(Expression<Func<TEntity, bool>> predicate = null);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null);

        T Max<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        T Min<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        Task<T> MinAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        decimal Average(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        Task<decimal> AverageAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        Task<decimal> SumAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        bool Exists(Expression<Func<TEntity, bool>> selector = null);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null);

        TEntity Insert(TEntity entity);

        void Insert(params TEntity[] entities);

        void Insert(IEnumerable<TEntity> entities);

        ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken =
          default(CancellationToken));

        Task InsertAsync(params TEntity[] entities);

        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken =
          default(CancellationToken));

        void Update(TEntity entity);

        void Update(params TEntity[] entities);

        void Update(IEnumerable<TEntity> entities);

        void Delete(object id);

        void Delete(TEntity entity);

        void Delete(params TEntity[] entities);

        void Delete(IEnumerable<TEntity> entities);

        void ChangeEntityState(TEntity entity, EntityState state);
    }
}
