using Book.Domain.Constants;
using Book.Domain.Exceptions;
using Book.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Book.UnitTests.Domain.BookTests;

public class IsStockAvailableTests
{
    [Fact]
    public void IsStockAvailable_WhenStockIsNotAvailable_ShouldReturnFalse()
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
        var isStockAvailable = book.IsStockAvailable(15);
        
        //assert
        isStockAvailable.Should().BeFalse();
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