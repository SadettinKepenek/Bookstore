namespace Order.Application.Repositories;

public interface IOrderRepository
{
    public void Add(Domain.Order order);
    Task<Domain.Order> GetByIdAsync(Guid id,CancellationToken cancellationToken);
}