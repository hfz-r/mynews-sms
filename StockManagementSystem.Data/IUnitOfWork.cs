using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;

namespace StockManagementSystem.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : BaseEntity;

        Task<int> SaveChangesAsync();
    }

    public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : DbContext, IDbContext
    {
        TContext Context { get; }
    }
}