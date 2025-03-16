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

public class RestoreStockTests
{
    private readonly IBookService _bookService;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IBookUnitOfWork> _bookUnitOfWorkMock;
    
    public RestoreStockTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookUnitOfWorkMock = new Mock<IBookUnitOfWork>();
        _bookService = new BookService(_bookRepositoryMock.Object, _bookUnitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task RestoreStockAsync_ValidBook_ShouldRestoreStock()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var bookCreateModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 100,
            Stock = 2,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        var book = global::Book.Domain.Book.Create(bookCreateModel);

        _bookRepositoryMock.Setup(x => x.GetByIdAsync(bookId, CancellationToken.None))
            .ReturnsAsync(book);

        // Act
        await _bookService.RestoreStockAsync(bookId, 3, CancellationToken.None);

        // Assert
        book.Stock.Should().Be(5);
        _bookRepositoryMock.Verify(x => x.GetByIdAsync(bookId, CancellationToken.None), Times.Once);
        _bookUnitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RestoreStockAsync_BookNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var bookId = Guid.NewGuid();

        _bookRepositoryMock.Setup(x => x.GetByIdAsync(bookId, CancellationToken.None))
            .ReturnsAsync((global::Book.Domain.Book)null);

        // Act
        var action = async () => await _bookService.RestoreStockAsync(bookId, 3, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.BookNotFound);

        _bookRepositoryMock.Verify(x => x.GetByIdAsync(bookId, CancellationToken.None), Times.Once);
        _bookUnitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
    }
}
