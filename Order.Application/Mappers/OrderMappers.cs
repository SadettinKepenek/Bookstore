using Order.Application.Enums;
using Order.Application.Outputs;

namespace Order.Application.Mappers;

public static class OrderMappers
{
    public static GetOrderDetailsOutput MapToGetOrderDetailsOutput(Domain.Order order)
    {
        return new GetOrderDetailsOutput
        {
            Id = order.Id,
            BookId = order.BookId,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice,
            Status = (OrderStatusDto)order.Status,
            CreatedAt = order.CreatedAt
        };
    }
}