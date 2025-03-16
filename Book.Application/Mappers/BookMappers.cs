using System.ComponentModel;
using Book.Application.Inputs;
using Book.Application.Outputs;
using Book.Domain.Models;

namespace Book.Application.Mappers;

public static class BookMappers
{
    public static BookCreateModel MapToDomainCreateModel(CreateBookInput input)
    {
        return new BookCreateModel
        {
            Title = input.Title,
            Author = input.Author,
            ISBN = input.ISBN,
            Price = input.Price,
            Stock = input.Stock,
            Category = input.Category,
            PublicationYear = input.PublicationYear
        };
    }

    public static ListBooksOutput MapToListBooksOutput(List<Domain.Book> books, int totalCount)
    {
        var bookWithCategories = books.GroupBy(b => b.Category)
            .Select(x => new CategoryBooksDto
            {
                Category = x.Key,
                Books = x.Select(b => new BookListItemDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Price = b.Price,
                }).ToList()
            }).ToList();
        return new ListBooksOutput
        {
            CategoryBooks = bookWithCategories,
            TotalCount = totalCount
        };
    }

    public static GetBookDetailOutput MapToGetBookDetailOutput(Domain.Book book)
    {
        return new GetBookDetailOutput
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            Price = book.Price,
            Stock = book.Stock,
            Category = book.Category,
            PublicationYear = book.PublicationYear
        };
    }

    public static SearchBooksOutput MapToSearchBooksOutput(List<Domain.Book> books, int totalCount)
    {
        return new SearchBooksOutput
        {
            Books = books.Select(x => new BookSearchItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Author = x.Author,
                Price = x.Price,
                Category = x.Category,
            }).ToList(),
            TotalCount = totalCount
        };
    }

}