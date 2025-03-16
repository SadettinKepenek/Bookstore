using Book.Application.Inputs;
using Book.Application.Repositories;
using Book.Application.Repositories.Book;
using Book.Application.Repositories.Models;
using Book.Application.Services.Book;
using Book.Domain.Models;
using Book.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Book.UnitTests.Application.Services.Book;

public class SearchTests
{
    private readonly IBookService _bookService;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IBookUnitOfWork> _bookUnitOfWorkMock;
    
    public SearchTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookUnitOfWorkMock = new Mock<IBookUnitOfWork>();
        _bookService = new BookService(_bookRepositoryMock.Object, _bookUnitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task SearchAsync_ValidInput_ShouldReturnMappedBooks()
    {
        // Arrange
        var searchBooksInput = new SearchBooksInput
        {
            Title = "test",
            Limit = 10,
            Offset = 0
        };

        var book = global::Book.Domain.Book.Create(new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 100,
            Stock = 4,
            Category = "test",
            PublicationYear = DateTime.Now
        });

        var books = new List<global::Book.Domain.Book> { book };
        var totalCount = 1;

        _bookRepositoryMock.Setup(x => x.SearchAsync(It.IsAny<SearchBooksFilterModel>(), CancellationToken.None))
            .ReturnsAsync((books, totalCount));

        // Act
        var searchBooksOutput = await _bookService.SearchAsync(searchBooksInput, CancellationToken.None);

        // Assert
        searchBooksOutput.TotalCount.Should().Be(totalCount);
        searchBooksOutput.Books.Should().NotBeEmpty();

        var firstBook = searchBooksOutput.Books.First();
        firstBook.Title.Should().Be(book.Title);
        firstBook.Author.Should().Be(book.Author);
        firstBook.Price.Should().Be(book.Price);
        firstBook.Category.Should().Be(book.Category);

        _bookRepositoryMock.Verify(x => x.SearchAsync(It.IsAny<SearchBooksFilterModel>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_NoMatchingBooks_ShouldReturnEmptyResult()
    {
        // Arrange
        var searchBooksInput = new SearchBooksInput
        {
            Title = "nonexistent",
            Limit = 10,
            Offset = 0
        };

        var books = new List<global::Book.Domain.Book>();
        var totalCount = 0;

        _bookRepositoryMock.Setup(x => x.SearchAsync(It.IsAny<SearchBooksFilterModel>(), CancellationToken.None))
            .ReturnsAsync((books, totalCount));

        // Act
        var searchBooksOutput = await _bookService.SearchAsync(searchBooksInput, CancellationToken.None);

        // Assert
        searchBooksOutput.TotalCount.Should().Be(totalCount);
        searchBooksOutput.Books.Should().BeEmpty();

        _bookRepositoryMock.Verify(x => x.SearchAsync(It.IsAny<SearchBooksFilterModel>(), CancellationToken.None), Times.Once);
    }
}
