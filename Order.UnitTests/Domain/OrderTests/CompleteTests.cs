using FluentAssertions;
using Order.Domain.Constants;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Order.UnitTests.Helpers;
using Xunit;

namespace Order.UnitTests.Domain.OrderTests;

public class CompleteTests
{
    [Fact]
    public void Complete_WhenOrderStatusIsCancel_ShouldThrowException()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Cancelled);
        
        //act
        var action = () => order.Complete();
        
        //assert
        action.Should().Throw<OrderDomainException>()
            .WithMessage(ErrorMessages.CancelOrderStatusCannotBeCompleted);
    }
    
    [Fact]
    public void Complete_WhenOrderStatusIsCompleted_ShouldThrowException()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Completed);
        
        //act
        var action = () => order.Complete();
        
        //assert
        action.Should().Throw<OrderDomainException>()
            .WithMessage(ErrorMessages.OrderIsAlreadyCompleted);
    }
    
    [Fact]
    public void Complete_WhenOrderStatusIsPending_ShouldCompleteOrder()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Pending);
        
        //act
        var action = () => order.Complete();
        
        //assert
        action.Should().NotThrow<OrderDomainException>();
        order.Status.Should().Be(OrderStatus.Completed);
    }
}