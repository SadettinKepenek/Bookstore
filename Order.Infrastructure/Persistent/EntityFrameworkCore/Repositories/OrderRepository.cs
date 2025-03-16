using Microsoft.EntityFrameworkCore;
using Order.Application.Repositories;

namespace Order.Infrastructure.Persistent.EntityFrameworkCore.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _orderDbContext;

    public OrderRepository(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public void Add(Domain.Order order)
    {
        _orderDbContext.Orders.Add(order);
    }

    public async Task<Domain.Order> GetByIdAsync(Guid id,CancellationToken cancellationToken)
    {
        return await _orderDbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}