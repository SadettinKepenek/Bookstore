namespace Book.Application.Inputs;

public class ListBooksInput
{
    public int Limit { get; init; }
    public int Offset { get; init; }
    public string Category { get; init; }
}