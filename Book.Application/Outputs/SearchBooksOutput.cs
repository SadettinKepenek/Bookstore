namespace Book.Application.Outputs;

public class SearchBooksOutput
{
    public List<BookSearchItemDto> Books { get; init; }
    public int TotalCount { get; init; }
}

public class BookSearchItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Author { get; init; }
    public decimal Price { get; init; }
    public string Category { get; init; }

}