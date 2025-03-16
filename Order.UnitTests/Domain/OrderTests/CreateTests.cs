using FluentAssertions;
using Order.Domain.Constants;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Xunit;

namespace Order.UnitTests.Domain.OrderTests;

public class CreateTests
{
    [Fact]
    public void Create_WhenQuantityIsInvalid_ShouldThrowException()
    {
        //arrange
        
        //act
        var action = ()=> Order.Domain.Order.Create(Guid.NewGuid(), 0, 100);
        
        //assert
        action.Should().Throw<OrderDomainException>()
            .WithMessage(ErrorMessages.OrderQuantityInvalid);
    }
    
    [Fact]
    public void Create_WhenUnitPriceIsInvalid_ShouldThrowException()
    {
        //arrange
        
        //act
        var action = ()=> Order.Domain.Order.Create(Guid.NewGuid(), 1, -1);
        
        //assert
        action.Should().Throw<OrderDomainException>()
            .WithMessage(ErrorMessages.OrderUnitPriceIsInvalid);

    }
    
    [Fact]
    public void Create_WhenOrderInputsAreValid_ShouldCreateOrder()
    {
        //arrange
        var bookId = Guid.NewGuid();
        
        //act
        var order = Order.Domain.Order.Create(bookId, 2, 10);
        
        //assert
        order.BookId.Should().Be(bookId);
        order.Quantity.Should().Be(2);
        order.TotalPrice.Should().Be(20);
        order.Status.Should().Be(OrderStatus.Pending);

    }
}