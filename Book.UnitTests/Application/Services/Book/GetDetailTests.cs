using System;
using System.Threading;
using System.Threading.Tasks;
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

public class GetDetailTests
{
    private readonly IBookService _bookService;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IBookUnitOfWork> _bookUnitOfWorkMock;

    public GetDetailTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookUnitOfWorkMock = new Mock<IBookUnitOfWork>();
        _bookService = new BookService(_bookRepositoryMock.Object, _bookUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetDetail_ExistingBook_ShouldReturnBookDetail()
    {
        // Arrange
        var bookCreateModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 100,
            Stock = 4,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        var book = global::Book.Domain.Book.Create(bookCreateModel);

        _bookRepositoryMock
            .Setup(repo => repo.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _bookService.GetDetailAsync(book.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(book.Id);
        result.Title.Should().Be(book.Title);
        result.Author.Should().Be(book.Author);
        result.ISBN.Should().Be(book.ISBN);
        result.Price.Should().Be(book.Price);
        result.Stock.Should().Be(book.Stock);
        result.Category.Should().Be(book.Category);
        result.PublicationYear.Should().Be(book.PublicationYear);

        _bookRepositoryMock.Verify(repo => repo.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDetail_WhenBookDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _bookRepositoryMock
            .Setup(repo => repo.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::Book.Domain.Book)null);

        // Act
        var action = async () => await _bookService.GetDetailAsync(bookId, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage(ErrorMessages.BookNotFound);
    
        _bookRepositoryMock.Verify(repo => repo.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
