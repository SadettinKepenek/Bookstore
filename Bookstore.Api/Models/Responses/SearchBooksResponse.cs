namespace Bookstore.Api.Models.Responses;

public class SearchBooksResponse
{
    public List<BookSearchItemModel> Books { get; init; }
    public int TotalCount { get; init; }
}

public class BookSearchItemModel
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Author { get; init; }
    public decimal Price { get; init; }
    public string Category { get; init; }

}