using TKH.Core.Entities.Abstract;

namespace TKH.Core.DataAccess
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity, new();
        Task<int> SaveChangesAsync();
    }
}
