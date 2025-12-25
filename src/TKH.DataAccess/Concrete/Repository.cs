using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using TKH.Core.DataAccess;
using TKH.Core.Entities.Abstract;
using TKH.Core.Utilities.Paging;
using TKH.DataAccess.Contexts;
using TKH.DataAccess.Extensions;

namespace TKH.DataAccess.Concrete
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly TKHDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(TKHDbContext dbContext)
        {
            _dbContext = dbContext ??
              throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet;
        }

        public IQueryable<TEntity> GetAll(
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public IQueryable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector);
            }
            else
            {
                return query.Select(selector);
            }
        }

        public virtual IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageIndex = 0,
          int pageSize = 20,
          bool disableTracking = true,
          bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedList(pageIndex, pageSize);
            }
            else
            {
                return query.ToPagedList(pageIndex, pageSize);
            }
        }

        public virtual Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageIndex = 0,
          int pageSize = 20,
          bool disableTracking = true,
          CancellationToken cancellationToken =
          default(CancellationToken),
          bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
            else
            {
                return query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
        }

        public virtual IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageIndex = 0,
          int pageSize = 20,
          bool disableTracking = true,
          bool ignoreQueryFilters = false)
        where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize);
            }
            else
            {
                return query.Select(selector).ToPagedList(pageIndex, pageSize);
            }
        }

        public virtual Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          int pageIndex = 0,
          int pageSize = 20,
          bool disableTracking = true,
          CancellationToken cancellationToken =
          default(CancellationToken),
          bool ignoreQueryFilters = false)
        where TResult : class
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
            else
            {
                return query.Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
        }

        public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).FirstOrDefault();
            }
            else
            {
                return query.FirstOrDefault();
            }
        }

        public virtual async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).FirstOrDefaultAsync();
            }
            else
            {
                return await query.FirstOrDefaultAsync();
            }
        }

        public virtual TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true,
          bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).FirstOrDefault();
            }
            else
            {
                return query.Select(selector).FirstOrDefault();
            }
        }

        public virtual async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).FirstOrDefaultAsync();
            }
            else
            {
                return await query.Select(selector).FirstOrDefaultAsync();
            }
        }

        public virtual IQueryable<TEntity> FromSql(string sql, params object[] parameters) => _dbSet.FromSqlRaw(sql, parameters);

        public virtual TEntity Find(params object[] keyValues) => _dbSet.Find(keyValues);

        public virtual ValueTask<TEntity> FindAsync(params object[] keyValues) => _dbSet.FindAsync(keyValues);

        public virtual ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken) => _dbSet.FindAsync(keyValues, cancellationToken);

        public virtual int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _dbSet.Count();
            }
            else
            {
                return _dbSet.Count(predicate);
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.CountAsync();
            }
            else
            {
                return await _dbSet.CountAsync(predicate);
            }
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _dbSet.LongCount();
            }
            else
            {
                return _dbSet.LongCount(predicate);
            }
        }

        public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.LongCountAsync();
            }
            else
            {
                return await _dbSet.LongCountAsync(predicate);
            }
        }

        public virtual T Max<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
        {
            if (predicate == null)
                return _dbSet.Max(selector);
            else
                return _dbSet.Where(predicate).Max(selector);
        }

        public virtual async Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
        {
            if (predicate == null)
                return await _dbSet.MaxAsync(selector);
            else
                return await _dbSet.Where(predicate).MaxAsync(selector);
        }

        public virtual T Min<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
        {
            if (predicate == null)
                return _dbSet.Min(selector);
            else
                return _dbSet.Where(predicate).Min(selector);
        }

        public virtual async Task<T> MinAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
        {
            if (predicate == null)
                return await _dbSet.MinAsync(selector);
            else
                return await _dbSet.Where(predicate).MinAsync(selector);
        }

        public virtual decimal Average(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        {
            if (predicate == null)
                return _dbSet.Average(selector);
            else
                return _dbSet.Where(predicate).Average(selector);
        }

        public virtual async Task<decimal> AverageAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        {
            if (predicate == null)
                return await _dbSet.AverageAsync(selector);
            else
                return await _dbSet.Where(predicate).AverageAsync(selector);
        }

        public virtual decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        {
            if (predicate == null)
                return _dbSet.Sum(selector);
            else
                return _dbSet.Where(predicate).Sum(selector);
        }

        public virtual async Task<decimal> SumAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
        {
            if (predicate == null)
                return await _dbSet.SumAsync(selector);
            else
                return await _dbSet.Where(predicate).SumAsync(selector);
        }

        public bool Exists(Expression<Func<TEntity, bool>> selector = null)
        {
            if (selector == null)
            {
                return _dbSet.Any();
            }
            else
            {
                return _dbSet.Any(selector);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null)
        {
            if (selector == null)
            {
                return await _dbSet.AnyAsync();
            }
            else
            {
                return await _dbSet.AnyAsync(selector);
            }
        }

        public virtual TEntity Insert(TEntity entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        public virtual void Insert(params TEntity[] entities) => _dbSet.AddRange(entities);

        public virtual void Insert(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

        public virtual ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken =
          default(CancellationToken))
        {
            return _dbSet.AddAsync(entity, cancellationToken);

        }

        public virtual Task InsertAsync(params TEntity[] entities) => _dbSet.AddRangeAsync(entities);

        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken =
          default(CancellationToken)) => _dbSet.AddRangeAsync(entities, cancellationToken);

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);

        }

        public virtual void Update(params TEntity[] entities) => _dbSet.UpdateRange(entities);

        public virtual void Update(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

        public virtual void Delete(TEntity entity) => _dbSet.Remove(entity);

        public virtual void Delete(object id)
        {

            var typeInfo = typeof(TEntity).GetTypeInfo();
            var key = _dbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeInfo.GetProperty(key?.Name);
            if (property != null)
            {
                var entity = Activator.CreateInstance<TEntity>();
                property.SetValue(entity, id);
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    Delete(entity);
                }
            }
        }

        public virtual void Delete(params TEntity[] entities) => _dbSet.RemoveRange(entities);

        public virtual void Delete(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        public async Task<IList<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>> predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).ToListAsync();
            }
            else
            {
                return await query.Select(selector).ToListAsync();
            }
        }

        public void ChangeEntityState(TEntity entity, EntityState state)
        {
            _dbContext.Entry(entity).State = state;
        }
    }
}
