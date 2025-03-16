using Book.Domain.Constants;
using Book.Domain.Exceptions;
using Book.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Book.UnitTests.Domain.BookTests;

public class CreateTests
{

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WhenPriceIsInvalid_ShouldThrowException(int price)
    {
        //arrange
        var createModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = price,
            Stock = 12,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        //act
        var action = () => Book.Domain.Book.Create(createModel);
        
        //assert
        action.Should().Throw<BookDomainException>()
            .WithMessage(ErrorMessages.BookPriceLowerThanOrEqualsZero);
    }
    
    [Fact]
    public void Create_WhenStockIsInvalid_ShouldThrowException()
    {
        //arrange
        var createModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 100,
            Stock = -1,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        //act
        var action = () => Book.Domain.Book.Create(createModel);
        
        //assert
        action.Should().Throw<BookDomainException>()
            .WithMessage(ErrorMessages.BookStockCannotBeLowerThanZero);
    }
    
    [Fact]
    public void Create_WhenCreateModelIsValid_ShouldCreateBook()
    {
        //arrange
        var createModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 100,
            Stock = 2,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        //act
        var book = Book.Domain.Book.Create(createModel);
        
        //assert
        book.Title.Should().Be(createModel.Title);
        book.Author.Should().Be(createModel.Author);
        book.ISBN.Should().Be(createModel.ISBN);
        book.Category.Should().Be(createModel.Category);
        book.Stock.Should().Be(createModel.Stock);
        book.Price.Should().Be(createModel.Price);
        book.PublicationYear.Should().Be(createModel.PublicationYear);
    }
}