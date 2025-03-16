namespace Bookstore.Api.Models.Responses;

public class ListBooksResponse
{
    public List<CategoryBooksModel> CategoryBooks { get; init; }
    public int TotalCount { get; init; }
}

public class CategoryBooksModel
{
    public string Category { get; init; }
    public List<BookListItemModel> Books { get; init; }
}
public class BookListItemModel
{
    public Guid Id { get; set; }
    public string Title { get; init; }
    public string Author { get; init; }
    public decimal Price { get; init; }
}