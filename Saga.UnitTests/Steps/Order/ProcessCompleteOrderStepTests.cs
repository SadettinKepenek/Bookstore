using FluentAssertions;
using Moq;
using Order.Application.Services;
using Saga.Application;
using Saga.Application.Enums;
using Saga.Application.Steps.OrderSteps;
using Xunit;

namespace Saga.UnitTests.Steps.Order;

public class ProcessCompleteOrderStepTests
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly ProcessCompleteOrderStep _processCompleteOrderStep;

    public ProcessCompleteOrderStepTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _processCompleteOrderStep = new ProcessCompleteOrderStep(_orderServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_StockReserved_ShouldCompleteOrder()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            OrderId = Guid.NewGuid(),
            StockReserved = true
        };

        _orderServiceMock.Setup(x => x.CompleteOrderAsync(sagaContext.OrderId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _processCompleteOrderStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        _orderServiceMock.Verify(x => x.CompleteOrderAsync(sagaContext.OrderId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NoStockReserved_ShouldFailWithBadRequest()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            OrderId = Guid.NewGuid(),
            StockReserved = false
        };

        // Act
        var result = await _processCompleteOrderStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Stock is not available");
        result.ErrorType.Should().Be(SagaErrorType.BadRequest);
        _orderServiceMock.Verify(x => x.CompleteOrderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ExceptionThrown_ShouldFailWithMappedErrorType()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            OrderId = Guid.NewGuid(),
            StockReserved = true
        };

        _orderServiceMock.Setup(x => x.CompleteOrderAsync(sagaContext.OrderId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _processCompleteOrderStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Unexpected error");
        result.ErrorType.Should().Be(SagaErrorType.BadRequest); 
        _orderServiceMock.Verify(x => x.CompleteOrderAsync(sagaContext.OrderId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RollbackAsync_ShouldCancelOrder()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            OrderId = Guid.NewGuid()
        };

        _orderServiceMock.Setup(x => x.CancelAsync(sagaContext.OrderId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _processCompleteOrderStep.RollbackAsync(sagaContext, CancellationToken.None);

        // Assert
        _orderServiceMock.Verify(x => x.CancelAsync(sagaContext.OrderId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
