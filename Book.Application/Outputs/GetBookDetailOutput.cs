namespace Book.Application.Outputs;

public class GetBookDetailOutput
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Author { get; init; }
    public string ISBN { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string Category { get; init; }
    public DateTime PublicationYear { get; init; }
}