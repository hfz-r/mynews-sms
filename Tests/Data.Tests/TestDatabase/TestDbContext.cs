using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Data;

namespace Data.Tests.TestDatabase
{
    public class TestDbContext : DbContext, IDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public virtual DbSet<TestCategory> TestCategories { get; set; }
        public virtual DbSet<TestProduct> TestProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestCategory>(entity =>
            {
                entity.ToTable("TestCategory");
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("Name")
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<TestProduct>(entity =>
            {
                entity.ToTable("TestProduct");

                entity.HasIndex(e => e.CategoryId).HasName("testCategory_testCategory_id_foreign");
                entity.Property(e => e.CategoryId).HasColumnName("CategoryId");

                entity.Property(e => e.Name)
                    .HasColumnName("Name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Stock)
                    .HasColumnName("Stock")
                    .HasColumnType("int");

                entity.Property(e => e.InStock)
                    .HasColumnName("inStock")
                    .HasColumnType("bit")
                    .HasDefaultValue(true);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("testCategory_testCategory_id_foreign");
            });
        }

        #region IDbContext Members

        protected virtual string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            //add parameters to sql
            for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
            {
                if (!(parameters[i] is DbParameter parameter))
                    continue;

                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

                if (parameter.Direction == ParameterDirection.InputOutput ||
                    parameter.Direction == ParameterDirection.Output)
                    sql = $"{sql} output";
            }

            return sql;
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public string GenerateCreateScript()
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<TQuery> QueryFromSql<TQuery>(string sql) where TQuery : class
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            throw new System.NotImplementedException();
        }

        public int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}