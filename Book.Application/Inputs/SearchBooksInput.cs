namespace Book.Application.Inputs;

public class SearchBooksInput
{
    public int Limit { get; init; }
    public int Offset { get; init; }
    public string Title { get; init; }
}