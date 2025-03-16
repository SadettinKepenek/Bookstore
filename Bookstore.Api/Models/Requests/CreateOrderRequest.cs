namespace Bookstore.Api.Models.Requests;

public class CreateOrderRequest
{
    public Guid BookId { get; init; }
    public int Quantity { get; init; }
}