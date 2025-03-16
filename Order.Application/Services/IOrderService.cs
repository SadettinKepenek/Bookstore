using Order.Application.Enums;
using Order.Application.Inputs;
using Order.Application.Outputs;

namespace Order.Application.Services;

public interface IOrderService
{
    public Task<Guid> CreateAsync(CreateOrderInput input, CancellationToken cancellationToken);

    public Task CancelAsync(Guid orderId, CancellationToken cancellationToken);
    public Task CompleteOrderAsync(Guid orderId,CancellationToken cancellationToken);
    public Task<GetOrderDetailsOutput> GetDetailsAsync(Guid orderId,CancellationToken cancellationToken);
    public Task UpdateStatusAsync(Guid orderId,OrderStatusDto status,CancellationToken cancellationToken);

}