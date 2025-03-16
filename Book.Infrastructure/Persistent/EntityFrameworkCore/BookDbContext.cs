using Book.Application.Repositories;
using Book.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Book.Infrastructure.Persistent.EntityFrameworkCore;

public class BookDbContext : DbContext,IBookUnitOfWork
{
    public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
    {
       
    }

    public DbSet<Domain.Book> Books { get; set; }
    
    async Task<int> IBookUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        AddTimestamps();
        var result = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookDbContext).Assembly);
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