using MHS.Common;
using MHS.Repository.Models;

namespace MHS.Repository.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    int Complete();
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    bool HasChanges();
    void RejectChanges();
    void DetachAllEntities();
} 