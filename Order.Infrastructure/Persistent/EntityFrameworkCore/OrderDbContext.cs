using Microsoft.EntityFrameworkCore;
using Order.Application.Repositories;
using Order.Domain.Base;

namespace Order.Infrastructure.Persistent.EntityFrameworkCore;

public class OrderDbContext : DbContext,IOrderUnitOfWork
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
       
    }

    public DbSet<Domain.Order> Orders { get; set; }
    
    async Task<int> IOrderUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        AddTimestamps();
        var result = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow;

            if (entity.State == EntityState.Added) ((BaseEntity)entity.Entity).CreatedAt = now;
            ((BaseEntity)entity.Entity).UpdatedAt = now;
        }
    }
}