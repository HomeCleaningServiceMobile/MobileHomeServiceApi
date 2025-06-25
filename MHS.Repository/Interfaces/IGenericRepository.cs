using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using MHS.Common;
using MHS.Common.DTOs;
using MHS.Repository.Models;

namespace MHS.Repository.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void UpdateRange(IEnumerable<T> entities);
    void DeleteRange(IEnumerable<T> entities);
    Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync();
    T? Delete(T entityToDelete);
    T? Delete(object id);
    void Update(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindAsync(Expression<Func<T, bool>> match);
    Task<T?> GetEntityByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<int> SaveChangesAsync();
    Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
    );
    Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProperties = null
    );
    Task<IPaginatedList<T>> ListAsyncWithPaginated(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProperties = null,
        PaginationRequest? pagination = null,
        CancellationToken cancellationToken = default
    );
} 