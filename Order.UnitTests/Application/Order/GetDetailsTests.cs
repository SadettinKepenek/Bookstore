using FluentAssertions;
using Moq;
using Order.Application.Constants;
using Order.Application.Exceptions;
using Order.Application.Repositories;
using Order.Application.Services;
using Order.Domain.Enums;
using Order.Application.Enums;
using Order.Application.Outputs;
using Order.UnitTests.Helpers;
using Xunit;

namespace Order.UnitTests.Application.Order;

public class GetDetailsTests
{
    private readonly IOrderService _orderService;
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IOrderUnitOfWork> _orderUnitOfWork;

    public GetDetailsTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _orderUnitOfWork = new Mock<IOrderUnitOfWork>();
        _orderService = new OrderService(_orderRepository.Object, _orderUnitOfWork.Object);
    }

    [Fact]
    public async Task GetDetailsAsync_ValidOrder_ShouldReturnOrderDetails()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = global::Order.Domain.Order.Create(Guid.NewGuid(), 5, 100);
        order.SetPrivate(x => x.Status, OrderStatus.Completed);
        order.SetPrivate(x => x.CreatedAt, DateTime.Now);
        
        var expectedOrderDetails = new GetOrderDetailsOutput
        {
            Id = order.Id,
            BookId = order.BookId,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice,
            Status = (OrderStatusDto)order.Status,
            CreatedAt = order.CreatedAt
        };

        _orderRepository.Setup(x => x.GetByIdAsync(orderId, CancellationToken.None))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetDetailsAsync(orderId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedOrderDetails);
        _orderRepository.Verify(x => x.GetByIdAsync(orderId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetDetailsAsync_OrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _orderRepository.Setup(x => x.GetByIdAsync(orderId, CancellationToken.None))
            .ReturnsAsync((global::Order.Domain.Order)null);

        // Act
        var action = async () => await _orderService.GetDetailsAsync(orderId, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.OrderNotFound);
    }
}
