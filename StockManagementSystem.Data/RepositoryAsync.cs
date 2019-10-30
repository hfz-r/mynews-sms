using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using StockManagementSystem.Core;
using StockManagementSystem.Data.Paging;

namespace StockManagementSystem.Data
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : BaseEntity
    {
        private readonly IDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public RepositoryAsync(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T> SingleAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null ? await orderBy(query).FirstOrDefaultAsync() : await query.FirstOrDefaultAsync();
        }

        // TODO: merge with IPagedList<T>
        public async Task<IPaginate<T>> GetPagedListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int index = 0,
            int size = int.MaxValue,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null
                ? await orderBy(query).ToPaginateAsync(index, size, 0, cancellationToken)
                : await query.ToPaginateAsync(index, size, 0, cancellationToken);
        }

        // TODO: merge with IPagedList<T>
        public async Task<IPaginate<TResult>> GetPagedListAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int index = 0,
            int size = int.MaxValue,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken)) where TResult : class
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            return orderBy != null
                ? await orderBy(query).Select(selector).ToPaginateAsync(index, size, 0, cancellationToken)
                : await query.Select(selector).ToPaginateAsync(index, size, 0, cancellationToken);
        }

        public async Task<IQueryable<T>> GetQueryableAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IQueryable<T>> searchFunc = null,
            Func<IQueryable<T>, IQueryable<T>> queryExp = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (searchFunc != null) query = searchFunc(query);

            if (queryExp != null) query = queryExp(query);

            if (predicate != null) query = query.Where(predicate);

            return await Task.FromResult(query);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task AddAsync(params T[] entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task AddAsync(IEnumerable<T> entities,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public async Task DeleteAsync(T entity)
        {
            var existing = await _dbSet.FindAsync(entity);
            if (existing != null) _dbSet.Remove(existing);
        }

        public async Task DeleteAsync(object id)
        {
            if (!(_dbContext is DbContext dbContext))
                throw new InvalidOperationException("Context does not support operation");

            var typeInfo = typeof(T).GetTypeInfo();
            var key = dbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeInfo.GetProperty(key?.Name ?? throw new InvalidOperationException());
            if (property != null)
            {
                var entity = Activator.CreateInstance<T>();
                property.SetValue(entity, id);
                dbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null) await DeleteAsync(entity);
            }
        }

        public Task DeleteAsync(params T[] entities)
        {
            return Task.Run(() => _dbSet.RemoveRange(entities));
        }

        public Task DeleteAsync(IEnumerable<T> entities)
        {
            return Task.Run(() => _dbSet.RemoveRange(entities));
        }

        public Task UpdateAsync(T entity)
        {
            return Task.Run(() => _dbSet.Update(entity));
        }

        public Task UpdateAsync(params T[] entities)
        {
            return Task.Run(() => _dbSet.UpdateRange(entities));
        }

        public Task UpdateAsync(IEnumerable<T> entities)
        {
            return Task.Run(() => _dbSet.UpdateRange(entities));
        }

        public void Dispose()
        {
            if (!(_dbContext is DbContext dbContext))
                throw new InvalidOperationException("Context does not support operation");

            dbContext.Dispose();
        }
    }
}