using FluentAssertions;
using Moq;
using Order.Application.Inputs;
using Order.Application.Repositories;
using Order.Application.Services;
using Order.Domain.Enums;
using Xunit;

namespace Order.UnitTests.Application.Order;

public class CreateTests
{
    private readonly IOrderService _orderService;
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IOrderUnitOfWork> _orderUnitOfWork;

    public CreateTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _orderUnitOfWork = new Mock<IOrderUnitOfWork>();
        _orderService = new OrderService(_orderRepository.Object, _orderUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidOrder_ShouldCreateOrderAndReturnOrderId()
    {
        // Arrange
        var input = new CreateOrderInput
        {
            BookId = Guid.NewGuid(),
            Quantity = 5,
            UnitPrice = 100
        };

        var order = global::Order.Domain.Order.Create(input.BookId, input.Quantity, input.UnitPrice);
        _orderRepository.Setup(x => x.Add(order)).Verifiable();
        _orderUnitOfWork.Setup(x => x.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        // Act
         var id = await _orderService.CreateAsync(input, CancellationToken.None);

        // Assert
        _orderRepository.Verify(x => x.Add(It.Is<global::Order.Domain.Order>(y
            => y.Quantity == order.Quantity &&
               y.BookId == order.BookId &&
               y.Status == OrderStatus.Pending &&
               y.TotalPrice == order.TotalPrice && 
               y.Id == id)), Times.Once);
        _orderUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
    
}
