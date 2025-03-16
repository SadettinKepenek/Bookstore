using Book.Domain.Constants;
using Book.Domain.Exceptions;
using Book.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Book.UnitTests.Domain.BookTests;

public class EnsureValidForOrderTests
{
    [Fact]
    public void EnsureValidForOrder_WhenStockIsNotAvailable_ShouldThrowExeption()
    {
        //arrange
        var createModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 10,
            Stock = 0,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        var book = Book.Domain.Book.Create(createModel);

        //act
        var action = () => book.EnsureValidForOrder(2);

        //assert
        action.Should().Throw<BookDomainException>()
            .WithMessage(ErrorMessages.StockIsNotAvailable);
    }
    
    [Fact]
    public void EnsureValidForOrder_WhenStockIsAvailable_ShouldEnsureValidationForOrder()
    {
        //arrange
        var createModel = new BookCreateModel
        {
            Title = "test",
            Author = "test",
            ISBN = "test",
            Price = 10,
            Stock = 2,
            Category = "test",
            PublicationYear = DateTime.Now
        };
        var book = Book.Domain.Book.Create(createModel);

        //act
        var action = () => book.EnsureValidForOrder(2);

        //assert
        action.Should().NotThrow<BookDomainException>();
    }
}