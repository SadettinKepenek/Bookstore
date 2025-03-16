namespace Bookstore.Api.Models.Requests;

public class SearchBooksRequest
{
    public int Limit { get; init; }
    public int Offset { get; init; }
    public string Title { get; init; }
}