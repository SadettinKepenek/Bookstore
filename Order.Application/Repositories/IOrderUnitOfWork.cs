namespace Order.Application.Repositories;

public interface IOrderUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}