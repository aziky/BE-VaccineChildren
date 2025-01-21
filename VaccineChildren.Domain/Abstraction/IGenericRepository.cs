using System.Linq.Expressions;
using VaccineChildren.Core.Base;

namespace VaccineChildren.Domain.Abstraction;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> Entities { get; }
    Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate);
    Task<IList<T>> GetAllAsync(Expression<Func<IQueryable<T>, IQueryable<T>>>? include);
    Task<IList<T>> GetAllAsync();
    Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);
    Task<T?> GetByIdNoTracking(object id);
    Task<T?> GetByIdAsync(object id);
    Task InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object id);
    Task SaveAsync();
}