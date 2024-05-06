using Core.Entities;
using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<RefreshToken>? RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetRelationsAndIndexes(modelBuilder);
            ApplyGlobalIsDeletedFilter(modelBuilder);
        }


        private static void SetRelationsAndIndexes(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasData(new List<User>
            //{
            //    new User
            //    {
            //        Id = Guid.NewGuid(),
            //        Email = "test@test.com",
            //        PasswordHash =
            //            "AQAAAAEAACcQAAAAEK3lyWfqNrzQ0P109198vqCfSb4EdhlF2EprWMW9aXYsQnEp4tx3vnYQLZGuebYClg==", // 1qaz@WSX
            //        Confirmed = true,
            //        Name = "Test Developer",
            //        CreatedDateUtc = DateTime.UtcNow,
            //        ModifiedDateUtc = DateTime.UtcNow,
            //        CreatedBy = "DbContextInitializer",
            //        ModifiedBy = "DbContextInitializer"
            //    }
            //});

        }

        private static void ApplyGlobalIsDeletedFilter(ModelBuilder modelBuilder)
        {
            Expression<Func<AuditableEntity, bool>> expression = entity => !entity.IsDeleted;
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.BaseType != typeof(AuditableEntity)) continue;

                var newParam = Expression.Parameter(entityType.ClrType);
                var newBody =
                    ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(newBody, newParam));
            }
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditProperties();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditProperties();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditProperties();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void UpdateAuditProperties()
        {
            var now = DateTime.UtcNow;

            ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity as AuditableEntity).Where(e => e != null)
                .ToList()
                .ForEach(e => e.CreatedDateUtc = now);

            ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity as AuditableEntity).Where(e => e != null)
                .ToList()
                .ForEach(e => e.ModifiedDateUtc = now);
        }
    }
}
