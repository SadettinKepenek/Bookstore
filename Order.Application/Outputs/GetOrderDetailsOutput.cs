using Order.Application.Enums;

namespace Order.Application.Outputs;

public class GetOrderDetailsOutput
{
    public Guid Id { get; init; }
    public Guid BookId { get; init; }
    public int Quantity { get;  init; }
    public decimal TotalPrice { get;  init; }
    public OrderStatusDto Status { get; init; }
    public DateTime CreatedAt { get; init; }
}