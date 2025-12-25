using Microsoft.EntityFrameworkCore.Storage;
using TKH.Core.Entities.Abstract;

namespace TKH.Core.DataAccess
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity, new();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        int ExecuteSqlCommand(string sql, params object[] parameters);
        IQueryable<T> FromSql<T>(string sql, params object[] parameters) where T : class, IEntity, new();
    }
}
