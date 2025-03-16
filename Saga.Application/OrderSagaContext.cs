namespace Saga.Application;

public class OrderSagaContext
{
    public Guid BookId { get; init; }
    public Guid OrderId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; init; }
    public bool StockReserved { get; set; }
}
