using Book.Domain.Constants;
using Book.Domain.Exceptions;
using Book.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Book.UnitTests.Domain.BookTests;

public class ReduceStockTests
{
    [Fact]
    public void ReduceStock_WhenStockIsNotAvailable_ShouldReturnException()
    {
        //arrange
        var createModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 10,
            Stock = 12,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        var book =  Book.Domain.Book.Create(createModel);

        //act
        var action = () =>book.ReduceStock(15);
        
        //assert
        action.Should().Throw<BookDomainException>()
            .WithMessage(ErrorMessages.StockIsNotAvailable);
    }
    
    [Fact]
    public void ReduceStock_WhenStockIsAvailable_ShouldReduceStock()
    {
        //arrange
        var createModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 10,
            Stock = 12,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        var book =  Book.Domain.Book.Create(createModel);

        //act
        book.ReduceStock(4);
        
        //assert
        book.Stock.Should().Be(8);
    }
    
    [Fact]
    public void IsStockAvailable_WhenStockIsAvailable_ShouldReturnTrue()
    {
        //arrange
        var createModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 10,
            Stock = 12,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        var book =  Book.Domain.Book.Create(createModel);

        //act
        var isStockAvailable = book.IsStockAvailable(2);
        
        //assert
        isStockAvailable.Should().BeTrue();
    }
}