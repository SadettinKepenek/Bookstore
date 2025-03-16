using FluentAssertions;
using Moq;
using Order.Application.Constants;
using Order.Application.Enums;
using Order.Application.Exceptions;
using Order.Application.Repositories;
using Order.Application.Services;
using Order.Domain.Enums;
using Order.UnitTests.Helpers;
using Xunit;

namespace Order.UnitTests.Application.Order;

public class UpdateStatusTests
{
    private readonly IOrderService _orderService;
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IOrderUnitOfWork> _orderUnitOfWork;

    public UpdateStatusTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _orderUnitOfWork = new Mock<IOrderUnitOfWork>();
        _orderService = new OrderService(_orderRepository.Object, _orderUnitOfWork.Object);
    }

    [Fact]
    public async Task UpdateStatusAsync_ValidOrder_ShouldUpdateOrderStatus()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = global::Order.Domain.Order.Create(Guid.NewGuid(), 5, 100);
        order.SetPrivate(x => x.Status, OrderStatus.Pending);
        
        var newStatus = OrderStatus.Completed;

        _orderRepository.Setup(x => x.GetByIdAsync(orderId, CancellationToken.None))
            .ReturnsAsync(order);
        _orderUnitOfWork.Setup(x => x.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        await _orderService.UpdateStatusAsync(orderId, (OrderStatusDto)newStatus, CancellationToken.None);

        // Assert
        order.Status.Should().Be(newStatus);
        _orderRepository.Verify(x => x.GetByIdAsync(orderId, CancellationToken.None), Times.Once);
        _orderUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_OrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _orderRepository.Setup(x => x.GetByIdAsync(orderId, CancellationToken.None))
            .ReturnsAsync((global::Order.Domain.Order)null);

        var newStatus = OrderStatus.Completed;

        // Act
        var action = async () => await _orderService.UpdateStatusAsync(orderId, (OrderStatusDto)newStatus, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.OrderNotFound);

        _orderUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
    }
}
