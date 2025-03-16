namespace Order.Application.Inputs;

public class CreateOrderInput
{
    public Guid BookId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}