using Order.Domain.Base;
using Order.Domain.Constants;
using Order.Domain.Enums;
using Order.Domain.Exceptions;

namespace Order.Domain;

public class Order : BaseEntity, IAggregateRoot
{
    public Guid BookId { get; private init; }
    public int Quantity { get; private init; }
    public decimal TotalPrice { get; private init; }
    public OrderStatus Status { get; private set; }

    private Order()
    {
    }

    public static Order Create(Guid bookId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new OrderDomainException(ErrorMessages.OrderQuantityInvalid);

        if (unitPrice <= 0)
            throw new OrderDomainException(ErrorMessages.OrderUnitPriceIsInvalid);

        return new Order
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            Quantity = quantity,
            TotalPrice = quantity * unitPrice,
            Status = OrderStatus.Pending
        };
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            throw new OrderDomainException(ErrorMessages.OrderIsAlreadyCancelled);

        Status = OrderStatus.Cancelled;
    }

    public void Complete()
    {
        Status = Status switch
        {
            OrderStatus.Cancelled => throw new OrderDomainException(ErrorMessages.CancelOrderStatusCannotBeCompleted),
            OrderStatus.Completed => throw new OrderDomainException(ErrorMessages.OrderIsAlreadyCompleted),
            _ => OrderStatus.Completed
        };
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = Status switch
        {
            OrderStatus.Cancelled => throw new OrderDomainException(string.Format(ErrorMessages.CannotUpdateOrderStatus, Status, status)),
            OrderStatus.Pending => status == OrderStatus.Completed
                ? OrderStatus.Completed
                : throw new OrderDomainException(string.Format(ErrorMessages.CannotUpdateOrderStatus, Status, status)),
            OrderStatus.Completed => status == OrderStatus.Cancelled
                ? OrderStatus.Cancelled
                : throw new OrderDomainException(string.Format(ErrorMessages.CannotUpdateOrderStatus, Status, status)),
            _ => throw new OrderDomainException(ErrorMessages.InvalidStatusData)
        };
    }
}