namespace Book.Application.Outputs;

public class GetBookForOrderOutput
{
    public Guid Id { get; init; }
    public decimal UnitPrice { get; init; }
}