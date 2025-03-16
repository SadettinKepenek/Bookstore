using Book.Application.Constants;
using Book.Application.Exceptions;
using Book.Application.Repositories;
using Book.Application.Repositories.Book;
using Book.Application.Services.Book;
using Book.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace Book.UnitTests.Application.Services.Book;

public class ValidateAndGetBookForOrderTests
{
    private readonly IBookService _bookService;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IBookUnitOfWork> _bookUnitOfWorkMock;
    
    public ValidateAndGetBookForOrderTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookUnitOfWorkMock = new Mock<IBookUnitOfWork>();
        _bookService = new BookService(_bookRepositoryMock.Object, _bookUnitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task ValidateAndGetBookForOrderAsync_ValidBook_ShouldReturnBookForOrder()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var orderQuantity = 3;
        
        var bookCreateModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 100,
            Stock = 5,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        var book = global::Book.Domain.Book.Create(bookCreateModel);

        _bookRepositoryMock.Setup(x => x.GetByIdAsync(bookId, CancellationToken.None))
            .ReturnsAsync(book);

        // Act
        var result = await _bookService.ValidateAndGetBookForOrderAsync(bookId, orderQuantity, CancellationToken.None);

        // Assert
        result.Id.Should().Be(book.Id);
        result.UnitPrice.Should().Be(book.Price);
        _bookRepositoryMock.Verify(x => x.GetByIdAsync(bookId, CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task ValidateAndGetBookForOrderAsync_BookNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var orderQuantity = 3;

        _bookRepositoryMock.Setup(x => x.GetByIdAsync(bookId, CancellationToken.None))
            .ReturnsAsync((global::Book.Domain.Book)null);

        // Act
        var action = async () => await _bookService.ValidateAndGetBookForOrderAsync(bookId, orderQuantity, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.BookNotFound);

        _bookRepositoryMock.Verify(x => x.GetByIdAsync(bookId, CancellationToken.None), Times.Once);
    }
}
