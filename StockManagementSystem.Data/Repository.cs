using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Data
{
    public partial class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IDbContext _context;

        private DbSet<TEntity> _entities;

        public Repository(IDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Rollback of entity changes and return full error message
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Error message</returns>
        protected string GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {
            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch (InvalidOperationException)
                    {
                        // ignored
                    }
                });
            }

            _context.SaveChanges();
            return exception.ToString();
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual TEntity GetById(object id)
        {
            return Entities.Find(id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Add(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.AddRange(entities);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Update(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.UpdateRange(entities);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Remove(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.RemoveRange(entities);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table => Entities;

        /// <summary>
        /// Gets a table with "no tracking" enabled 
        /// </summary>
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        /// <summary>
        /// Gets an entity set
        /// </summary>
        protected virtual DbSet<TEntity> Entities => _entities ?? (_entities = _context.Set<TEntity>());

        #endregion

        #region Async Method

        protected async Task<string> GetFullErrorTextAndRollbackEntityChangesAsync(DbUpdateException exception)
        {
            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch (InvalidOperationException)
                    {
                        // ignored
                    }
                });
            }

            await _context.SaveChangesAsync();
            return exception.ToString();
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await Entities.FindAsync(id);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                await Entities.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(await GetFullErrorTextAndRollbackEntityChangesAsync(exception), exception);
            }
        }

        public virtual async Task InsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                await Entities.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(await GetFullErrorTextAndRollbackEntityChangesAsync(exception), exception);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(await GetFullErrorTextAndRollbackEntityChangesAsync(exception), exception);
            }
        }

        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.UpdateRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(await GetFullErrorTextAndRollbackEntityChangesAsync(exception), exception);
            }
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(await GetFullErrorTextAndRollbackEntityChangesAsync(exception), exception);
            }
        }

        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                Entities.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(await GetFullErrorTextAndRollbackEntityChangesAsync(exception), exception);
            }
        }

        #endregion
    }
}