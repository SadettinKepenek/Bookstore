using Book.Application.Inputs;
using Book.Application.Repositories;
using Book.Application.Repositories.Book;
using Book.Application.Services.Book;
using Moq;
using Xunit;

namespace Book.UnitTests.Application.Services.Book;

public class CreateTests
{
    private readonly IBookService _bookService;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IBookUnitOfWork> _bookUnitOfWorkMock;
    public CreateTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookUnitOfWorkMock = new Mock<IBookUnitOfWork>();
        _bookService = new BookService(_bookRepositoryMock.Object,_bookUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task Create_Always_ShouldCreateBook()
    {
        //arrange
        var createBookInput = new CreateBookInput
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 100,
            Stock = 4,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        _bookRepositoryMock.Setup(x => x.Add(It.IsAny<global::Book.Domain.Book>()));
        
        //act
        await _bookService.CreateAsync(createBookInput, CancellationToken.None);
        
        //assert
        _bookRepositoryMock.Verify(x => x.Add(It.Is<global::Book.Domain.Book>(y => 
            y.Title == createBookInput.Title &&
            y.Stock == createBookInput.Stock &&
            y.Author == createBookInput.Author &&
            y.Category == createBookInput.Category &&
            y.Price == createBookInput.Price &&
            y.PublicationYear == createBookInput.PublicationYear)),Times.Once);
        _bookUnitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None),Times.Once);

    }
}