using Book.Application.Inputs;
using Book.Application.Repositories;
using Book.Application.Repositories.Book;
using Book.Application.Services.Book;
using Book.Domain.Models;
using Book.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Book.UnitTests.Application.Services.Book;

public class ListTests
{
    private readonly IBookService _bookService;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IBookUnitOfWork> _bookUnitOfWorkMock;
    public ListTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookUnitOfWorkMock = new Mock<IBookUnitOfWork>();
        _bookService = new BookService(_bookRepositoryMock.Object,_bookUnitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task Create_Always_ShouldCreateBook()
    {
        //arrange
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
        var books = new List<global::Book.Domain.Book> { book };
        
        var offset = 0;
        var limit = 10;
        
        _bookRepositoryMock.Setup(x => x.ListAsync(offset, limit, CancellationToken.None))
            .ReturnsAsync((books,1));
        
        //act
        var bookListResponse = await _bookService.ListAsync(offset,limit ,CancellationToken.None);
        
        //assert
       bookListResponse.TotalCount.Should().Be(1);
       bookListResponse.CategoryBooks.Should().NotBeEmpty();
       
       var firstCategoryBook = bookListResponse.CategoryBooks.First();
       firstCategoryBook.Category.Should().Be(book.Category);
       
       var firsBook = firstCategoryBook.Books.First();
       firsBook.Author.Should().Be(book.Author);
       firsBook.Title.Should().Be(book.Title);
       firsBook.Price.Should().Be(book.Price);
       firsBook.Id.Should().Be(book.Id);
       
       _bookRepositoryMock.Verify(x => x.ListAsync(offset, limit, CancellationToken.None),Times.Once);

    }
}