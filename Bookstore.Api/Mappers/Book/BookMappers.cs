using Book.Application.Inputs;
using Book.Application.Outputs;
using Bookstore.Api.Models.Requests;
using Bookstore.Api.Models.Responses;

namespace Bookstore.Api.Mappers.Book;

public static class BookMappers
{
    public static CreateBookInput MapToInput(CreateBookRequest request)
    {
        return new CreateBookInput
        {
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            Price = request.Price,
            Stock = request.Stock,
            Category = request.Category,
            PublicationYear = request.PublicationYear
        };
    }

    public static ListBooksResponse MapToListBooksResponse(ListBooksOutput output)
    {
        return new ListBooksResponse
        {
            CategoryBooks = output.CategoryBooks.Select(bwc => new CategoryBooksModel
            {
                Category =bwc.Category ,
                Books = bwc.Books.Select(b => new BookListItemModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Price = b.Price
                }).ToList() 
            }).ToList(),
            TotalCount = output.TotalCount
        };
    }

    public static GetBookDetailResponse MapToGetBookDetailResponse(GetBookDetailOutput output)
    {
        return new GetBookDetailResponse
        {
            Id = output.Id,
            Title = output.Title,
            Author = output.Author,
            ISBN = output.ISBN,
            Price = output.Price,
            Stock = output.Stock,
            Category = output.Category,
            PublicationYear = output.PublicationYear
        };
    }
    
    public static SearchBooksResponse MapToSearchBooksResponse(SearchBooksOutput output)
    {
        return new SearchBooksResponse
        {
            Books = output.Books.Select(b => new BookSearchItemModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Price = b.Price,
                Category = b.Category,
            }).ToList(),
            TotalCount = output.TotalCount
        };
    }

}