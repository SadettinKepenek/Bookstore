namespace Book.Application.Outputs;

public class ListBooksOutput
{
    public List<CategoryBooksDto> CategoryBooks { get; init; }
    public int TotalCount { get; init; }
}

public class CategoryBooksDto
{
    public string Category { get; init; }
    public List<BookListItemDto> Books { get; init; }
}
public class BookListItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Author { get; init; }
    public decimal Price { get; init; }
}