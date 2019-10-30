using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using StockManagementSystem.Core;
using StockManagementSystem.Data.Paging;

namespace StockManagementSystem.Data
{
    public interface IRepositoryAsync<T> : IDisposable where T : BaseEntity
    {
        Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task AddAsync(params T[] entities);
        Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAsync(IEnumerable<T> entities);
        Task DeleteAsync(object id);
        Task DeleteAsync(params T[] entities);
        Task DeleteAsync(T entity);
        Task<IPaginate<TResult>> GetPagedListAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, int index = 0, int size = int.MaxValue, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken)) where TResult : class;
        Task<IPaginate<T>> GetPagedListAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, int index = 0, int size = int.MaxValue, bool disableTracking = true, CancellationToken cancellationToken = default(CancellationToken));
        Task<IQueryable<T>> GetQueryableAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IQueryable<T>> searchFunc = null, Func<IQueryable<T>, IQueryable<T>> queryExp = null, bool disableTracking = true);
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true);
        Task UpdateAsync(IEnumerable<T> entities);
        Task UpdateAsync(params T[] entities);
        Task UpdateAsync(T entity);
    }
}