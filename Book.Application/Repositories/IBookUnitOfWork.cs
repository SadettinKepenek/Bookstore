namespace Book.Application.Repositories;

public interface IBookUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}