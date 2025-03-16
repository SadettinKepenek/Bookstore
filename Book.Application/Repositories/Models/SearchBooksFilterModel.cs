namespace Book.Application.Repositories.Models;

public class SearchBooksFilterModel
{
    public int Limit { get; init; }
    public int Offset { get; init; }
    public string Title { get; init; }
}