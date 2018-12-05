using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private IConfigurationRoot _configuration;

        public ApplicationDbContext(IConfigurationRoot configuration)
        {
            _configuration = (IConfigurationRoot)configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration["Data:DefaultConnection:ConnectionString"];
            optionsBuilder.UseSqlServer(connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Device>().HasKey(m => m.ID);
            builder.Entity<Device>().Property(m => m.ID).ValueGeneratedOnAdd();

        }

        public DbSet<Device> Devices { set; get; }
    }
}
