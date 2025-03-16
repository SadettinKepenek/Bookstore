using FluentAssertions;
using Moq;
using Order.Application.Constants;
using Order.Application.Exceptions;
using Order.Application.Repositories;
using Order.Application.Services;
using Order.Domain.Enums;
using Order.UnitTests.Helpers;
using Xunit;

namespace Order.UnitTests.Application.Order;

public class CancelTests
{
    private readonly IOrderService _orderService;
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IOrderUnitOfWork> _orderUnitOfWork;

    public CancelTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _orderUnitOfWork = new Mock<IOrderUnitOfWork>();
        _orderService = new OrderService(_orderRepository.Object, _orderUnitOfWork.Object);
    }

    [Fact]
    public async Task CancelAsync_ValidOrder_ShouldCancelOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = global::Order.Domain.Order.Create(Guid.NewGuid(), 5, 100);
        order.SetPrivate(x => x.Status,OrderStatus.Completed);

        _orderRepository.Setup(x => x.GetByIdAsync(orderId, CancellationToken.None))
            .ReturnsAsync(order);
        _orderUnitOfWork.Setup(x => x.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
        await _orderService.CancelAsync(orderId, CancellationToken.None);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        _orderRepository.Verify(x => x.GetByIdAsync(orderId, CancellationToken.None), Times.Once);
        _orderUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_OrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _orderRepository.Setup(x => x.GetByIdAsync(orderId, CancellationToken.None))
            .ReturnsAsync((global::Order.Domain.Order)null); 

        // Act
        var action = async () => await _orderService.CancelAsync(orderId, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.OrderNotFound);
        
        _orderUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
    }
}
