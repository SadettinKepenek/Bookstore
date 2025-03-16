using FluentAssertions;
using Order.Domain.Constants;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Order.UnitTests.Helpers;
using Xunit;

namespace Order.UnitTests.Domain.OrderTests;

public class CancelTests
{
    [Fact]
    public void Cancel_WhenOrderStatusIsAlreadyCancel_ShouldThrowException()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Cancelled);
        
        //act
        var action = () => order.Cancel();
        
        //assert
        action.Should().Throw<OrderDomainException>()
            .WithMessage(ErrorMessages.OrderIsAlreadyCancelled);
    }
    
    [Fact]
    public void Cancel_WhenOrderStatusIsCompleted_ShouldCancelOrder()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Completed);
        
        //act
        var action = () => order.Cancel();
        
        //assert
        action.Should().NotThrow<OrderDomainException>();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}