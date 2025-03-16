using Order.Application.Enums;
using Order.Application.Outputs;

namespace Bookstore.Api.Models.Responses;

public class GetOrderDetailsResponse
{
    public Guid Id { get; init; }
    public Guid BookId { get; init; }
    public int Quantity { get;  init; }
    public decimal TotalPrice { get;  init; }
    public OrderStatusDto Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
