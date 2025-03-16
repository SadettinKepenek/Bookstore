using Book.Application.Exceptions;
using Book.Application.Outputs;
using Book.Application.Services.Book;
using FluentAssertions;
using Moq;
using Saga.Application;
using Saga.Application.Enums;
using Saga.Application.Steps.BookSteps;
using Xunit;

namespace Saga.UnitTests.Steps.Book;

public class ProcessValidateAndGetBookStepTests
{
    private readonly Mock<IBookService> _bookServiceMock;
    private readonly ProcessValidateAndGetBookStep _processValidateAndGetBookStep;

    public ProcessValidateAndGetBookStepTests()
    {
        _bookServiceMock = new Mock<IBookService>();
        _processValidateAndGetBookStep = new ProcessValidateAndGetBookStep(_bookServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidBook_ShouldSetUnitPrice()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 2
        };

        var book = new GetBookForOrderOutput
        {
            UnitPrice = 50
        };

        _bookServiceMock.Setup(x => x.ValidateAndGetBookForOrderAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _processValidateAndGetBookStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        sagaContext.UnitPrice.Should().Be(book.UnitPrice);
        _bookServiceMock.Verify(x => x.ValidateAndGetBookForOrderAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_BookNotFound_ShouldFailWithBadRequest()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 2
        };

        _bookServiceMock.Setup(x => x.ValidateAndGetBookForOrderAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Book not found"));

        // Act
        var result = await _processValidateAndGetBookStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Book not found");
        result.ErrorType.Should().Be(SagaErrorType.NotFound);
        _bookServiceMock.Verify(x => x.ValidateAndGetBookForOrderAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()), Times.Once);
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

        _bookServiceMock.Setup(x => x.ValidateAndGetBookForOrderAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _processValidateAndGetBookStep.ExecuteAsync(sagaContext, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Unexpected error");
        result.ErrorType.Should().Be(SagaErrorType.BadRequest); 
        _bookServiceMock.Verify(x => x.ValidateAndGetBookForOrderAsync(sagaContext.BookId, sagaContext.Quantity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RollbackAsync_ShouldNotDoAnything()
    {
        // Arrange
        var sagaContext = new OrderSagaContext
        {
            BookId = Guid.NewGuid(),
            Quantity = 2
        };

        // Act
        await _processValidateAndGetBookStep.RollbackAsync(sagaContext, CancellationToken.None);

        // Assert
        // Nothing should happen since Rollback is empty
        _bookServiceMock.Verify(x => x.ValidateAndGetBookForOrderAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
