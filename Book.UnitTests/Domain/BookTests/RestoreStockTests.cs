using Book.Domain.Constants;
using Book.Domain.Exceptions;
using Book.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Book.UnitTests.Domain.BookTests;

public class RestoreStockTests
{
    [Fact]
    public void RestoreStock_Always_ShouldRestoreStock()
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
        book.RestoreStock(2);
        
        //assert
        book.Stock.Should().Be(14);
    }
}