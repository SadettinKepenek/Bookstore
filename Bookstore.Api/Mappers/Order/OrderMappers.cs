using Bookstore.Api.Models.Responses;
using Order.Application.Outputs;

namespace Bookstore.Api.Mappers.Order;

public static class OrderMappers
{
    public static GetOrderDetailsResponse MapToGetOrderDetailsResponse(GetOrderDetailsOutput output)
    {
        return new GetOrderDetailsResponse
        {
            Id = output.Id,
            BookId = output.BookId,
            Quantity = output.Quantity,
            TotalPrice = output.TotalPrice,
            Status = output.Status,
            CreatedAt = output.CreatedAt
        };
    }
}