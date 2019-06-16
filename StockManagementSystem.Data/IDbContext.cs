using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;

namespace StockManagementSystem.Data
{
    /// <summary>
    /// Represents DB context
    /// </summary>
    public partial interface IDbContext
    {
        /// <summary>
        /// Creates a DbSet that can be used to query and save instances of entity
        /// </summary>
        DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Generate a script to create all tables for the current model
        /// </summary>
        string GenerateCreateScript();

        /// <summary>
        /// Creates a LINQ query for the query type based on a raw SQL query
        /// </summary>
        IQueryable<TQuery> QueryFromSql<TQuery>(string sql) where TQuery : class;

        /// <summary>
        /// Creates a LINQ query for the entity based on a raw SQL query
        /// </summary>
        IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity;

        /// <summary>
        /// Executes the given SQL against the database
        /// </summary>
        int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);

        /// <summary>
        /// Detach an entity from the context
        /// </summary>
        void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        int SaveChanges();

        /// <summary>
        /// Saves all changes (async) made in this context to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}