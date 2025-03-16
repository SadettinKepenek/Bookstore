using Order.Application.Constants;
using Order.Application.Enums;
using Order.Application.Exceptions;
using Order.Application.Inputs;
using Order.Application.Mappers;
using Order.Application.Outputs;
using Order.Application.Repositories;
using Order.Domain.Enums;

namespace Order.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderUnitOfWork _orderUnitOfWork;

    public OrderService(IOrderRepository orderRepository, IOrderUnitOfWork orderUnitOfWork)
    {
        _orderRepository = orderRepository;
        _orderUnitOfWork = orderUnitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateOrderInput input, CancellationToken cancellationToken)
    {
        var order = Domain.Order.Create(input.BookId, input.Quantity, input.UnitPrice);
        
        _orderRepository.Add(order);
        await _orderUnitOfWork.SaveChangesAsync(cancellationToken);
        
        return order.Id;
    }

    public async Task CancelAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await GetOrderById(orderId, cancellationToken);
        
        order.Cancel();
        await _orderUnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteOrderAsync(Guid orderId,CancellationToken cancellationToken)
    {
        var order = await GetOrderById(orderId, cancellationToken);
        
        order.Complete();
        await _orderUnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<GetOrderDetailsOutput> GetDetailsAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await GetOrderById(orderId, cancellationToken);
        return OrderMappers.MapToGetOrderDetailsOutput(order);
    }

    public async Task UpdateStatusAsync(Guid orderId, OrderStatusDto status,CancellationToken cancellationToken)
    {
        var order = await GetOrderById(orderId, cancellationToken);

        order.UpdateStatus((OrderStatus)status);
        await _orderUnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Domain.Order> GetOrderById(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new NotFoundException(ErrorMessages.OrderNotFound);
        return order;
    }
}