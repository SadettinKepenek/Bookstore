using FluentAssertions;
using Order.Domain.Constants;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Order.UnitTests.Helpers;
using Xunit;

namespace Order.UnitTests.Domain.OrderTests;

public class UpdateOrderStatusTests
{
    [Fact]
    public void UpdateOrderStatusTests_WhenOrderStatusIsCancel_ShouldThrowException()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Cancelled);
        
        //act
        var action = () => order.UpdateStatus(OrderStatus.Cancelled);
        
        //assert
        action.Should().Throw<OrderDomainException>()
            .WithMessage(string.Format(ErrorMessages.CannotUpdateOrderStatus,order.Status,OrderStatus.Cancelled));
    }
    
    [Fact]
    public void UpdateStatus_WhenOrderStatusIsPendingAndUpdateStatusIsNotCompleted_ShouldThrowException()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Pending);
        
        //act
        var action = () => order.UpdateStatus(OrderStatus.Cancelled);
        
        //assert
        action.Should().Throw<OrderDomainException>()
            .WithMessage(string.Format(ErrorMessages.CannotUpdateOrderStatus,order.Status,OrderStatus.Cancelled));
    }
    [Fact]
    public void UpdateStatus_WhenOrderStatusIsPendingAndUpdateStatusIsCompleted_ShouldUpdateStatus()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Pending);
        
        //act
        var action = () => order.UpdateStatus(OrderStatus.Completed);
        
        //assert
        action.Should().NotThrow<OrderDomainException>();
        order.Status.Should().Be(OrderStatus.Completed);
    }
    
    [Fact]
    public void UpdateStatus_WhenOrderStatusIsCompletedAndUpdateStatusIsNotCancelled_ShouldThrowException()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Completed);
        
        //act
        var action = () => order.UpdateStatus(OrderStatus.Pending);
        
        //assert
        action.Should().Throw<OrderDomainException>()
            .WithMessage(string.Format(ErrorMessages.CannotUpdateOrderStatus,order.Status,OrderStatus.Pending));
    }
    [Fact]
    public void UpdateStatus_WhenOrderStatusIsCompletedAndUpdateStatusIsCancelled_ShouldUpdateStatus()
    {
        //arrange
        var order = Order.Domain.Order.Create(Guid.NewGuid(), 2, 10);
        order.SetPrivate(x => x.Status,OrderStatus.Completed);
        
        //act
        var action = () => order.UpdateStatus(OrderStatus.Cancelled);
        
        //assert
        action.Should().NotThrow<OrderDomainException>();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
   
}