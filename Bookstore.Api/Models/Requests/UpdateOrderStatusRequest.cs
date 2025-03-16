using Order.Application.Enums;

namespace Bookstore.Api.Models.Requests;

public class UpdateOrderStatusRequest
{
    public OrderStatusDto Status { get; init; }
}