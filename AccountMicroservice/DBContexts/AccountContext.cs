using AccountMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.DBContexts
{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserProfileView> UserProfileView { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Use the entity name instead of the Context.DbSet<T> name
                // refs https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api#table-name
                builder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
            }

            base.OnModelCreating(builder);

            builder
                .Entity<User>()
                .HasData(
                new User { UserId = "3", UserName = "User3", CreatedBy = "Admin", CreatedDate = DateTime.Now, PwdHash = "Password12345" },
                new User { UserId = "4", UserName = "User4", CreatedBy = "Admin", CreatedDate = DateTime.Now, PwdHash = "Password12345" }
                );
        }
    }
}
