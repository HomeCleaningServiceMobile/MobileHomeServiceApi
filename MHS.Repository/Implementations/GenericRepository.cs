using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MHS.Common;
using MHS.Common.DTOs;
using MHS.Repository.Data;
using MHS.Repository.Interfaces;
using MHS.Repository.Models;

namespace MHS.Repository.Implementations;

public class GenericRepository<T> : IGenericRepository<T>
where T : BaseEntity
{
    private readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _context.Set<T>().UpdateRange(entities);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public async Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> predicate)
    {
        var entities = await _context.Set<T>().Where(predicate).ToListAsync();
        if (!entities.Any())
            return false;

        _context.Set<T>().RemoveRange(entities);
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _context.Set<T>().CountAsync();
    }

    public T? Delete(T entityToDelete)
    {
        if (_context.Set<T>().Entry(entityToDelete).State == EntityState.Detached)
        {
            _context.Set<T>().Attach(entityToDelete);
        }
        var deletedEntity = _context.Set<T>().Remove(entityToDelete).Entity;
        return deletedEntity;
    }

    public T? Delete(object id)
    {
        var entityToDelete = _context.Set<T>().Find(id);
        return entityToDelete == null ? null : Delete(entityToDelete);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AnyAsync(predicate);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> match)
    {
        return await _context.Set<T>().SingleOrDefaultAsync(match);
    }

    public async Task<T?> GetEntityByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    //public async Task<IReadOnlyList<T>> ListAsync(
    //    Expression<Func<T, bool>>? filter = null,
    //    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
    //)
    //{
    //    IQueryable<T> query = _context.Set<T>();

    //    if (filter != null)
    //    {
    //        query = query.Where(filter);
    //    }

    //    if (orderBy != null)
    //    {
    //        query = orderBy(query);
    //    }

    //    return await query.ToListAsync();
    //}

    public async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProperties = null
    )
    {
        IQueryable<T> query = _context.Set<T>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            query = includeProperties(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.ToListAsync();
    }

    public async Task<IPaginatedList<T>> ListAsyncWithPaginated(
        Expression<Func<T, bool>>? filter = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProperties = null, 
        PaginationRequest? pagination = null, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _context.Set<T>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            query = includeProperties(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        if (pagination != null)
        {
            query = query.Skip(pagination.Skip).Take(pagination.PageSize);
        }

        var items = await query.ToListAsync(cancellationToken);

        return PaginatedList<T>.Create(
            items,
            pagination?.PageNumber ?? 1,
            pagination?.PageSize ?? totalCount,
            totalCount);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }
} 