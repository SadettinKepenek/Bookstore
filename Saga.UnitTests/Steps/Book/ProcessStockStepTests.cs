using Book.Application.Services.Book;
using FluentAssertions;
using Moq;
using Saga.Application;
using Saga.Application.Enums;
using Saga.Application.Steps.BookSteps;
using Xunit;

namespace Saga.UnitTests.Steps.Book;

public class ProcessStockStepTests
{
    private readonly Mock<IBookService> _bookServiceMock;
    private readonly ProcessStockStep _processStockStep;

    public ProcessStockStepTests()
    {
        _bookServiceMock = new Mock<IBookService>();
        _processStockStep = new ProcessStockStep(_bookServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_StockAvailable_ShouldReserveStock()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 2
        };

        _bookServiceMock.Setup(x => x.CheckStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookServiceMock.Setup(x => x.ReduceStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _processStockStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        sagaContext.StockReserved.Should().BeTrue();
        _bookServiceMock.Verify(x => x.CheckStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()), Times.Once);
        _bookServiceMock.Verify(x => x.ReduceStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_StockNotAvailable_ShouldFailWithBadRequest()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 2
        };

        _bookServiceMock.Setup(x => x.CheckStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _processStockStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Stock is not available");
        result.ErrorType.Should().Be(SagaErrorType.BadRequest);
        _bookServiceMock.Verify(x => x.CheckStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()), Times.Once);
        _bookServiceMock.Verify(x => x.ReduceStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ExceptionThrown_ShouldFailWithMappedErrorType()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 2
        };

        _bookServiceMock.Setup(x => x.CheckStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _processStockStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Stock is not available");
        result.ErrorType.Should().Be(SagaErrorType.BadRequest); // Map error type accordingly
        _bookServiceMock.Verify(x => x.CheckStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()), Times.Once);
        _bookServiceMock.Verify(x => x.ReduceStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RollbackAsync_StockReserved_ShouldRestoreStock()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 2,
            StockReserved = true
        };

        _bookServiceMock.Setup(x => x.RestoreStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _processStockStep.RollbackAsync(sagaContext, CancellationToken.None);

        // Assert
        _bookServiceMock.Verify(x => x.RestoreStockAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RollbackAsync_NoStockReserved_ShouldNotRestoreStock()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 2,
            StockReserved = false
        };

        // Act
        await _processStockStep.RollbackAsync(sagaContext, CancellationToken.None);

        // Assert
        _bookServiceMock.Verify(x => x.RestoreStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
