using FluentAssertions;
using Moq;
using Order.Application.Exceptions;
using Order.Application.Inputs;
using Order.Application.Services;
using Saga.Application.Enums;
using Saga.Application.Steps.OrderSteps;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Saga.Application;

namespace Saga.UnitTests.Steps.OrderSteps;

public class ProcessInitializeOrderStepTests
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly ProcessInitializeOrderStep _processInitializeOrderStep;

    public ProcessInitializeOrderStepTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _processInitializeOrderStep = new ProcessInitializeOrderStep(_orderServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoExceptionThrown_ShouldInitializeOrderSuccessfully()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 5,
            UnitPrice = 100
        };

        var orderId = Guid.NewGuid();

        _orderServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateOrderInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);

        // Act
        var result = await _processInitializeOrderStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        sagaContext.OrderId.Should().Be(orderId);
        _orderServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateOrderInput>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExceptionOccurs_ShouldFail()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 5,
            UnitPrice = 100
        };

        _orderServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateOrderInput>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await _processInitializeOrderStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("An error occurred");
        result.ErrorType.Should().Be(SagaErrorType.BadRequest);
        _orderServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateOrderInput>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RollbackAsync_Always_ShouldNotPerformAnyAction()
    {
        // Arrange
        var sagaContext = new OrderSagaContext();

        // Act
        await _processInitializeOrderStep.RollbackAsync(sagaContext, CancellationToken.None);

        // Assert
        _orderServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateOrderInput>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
