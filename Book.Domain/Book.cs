using System.ComponentModel.DataAnnotations;
using Book.Domain.Base;
using Book.Domain.Constants;
using Book.Domain.Exceptions;
using Book.Domain.Models;

namespace Book.Domain;

public class Book : BaseEntity , IAggregateRoot
{
    public string Title { get; private init; }
    public string Author { get; private init; }
    public string ISBN { get; private init; }
    public decimal Price { get; private init; }
    public int Stock { get; private set; }
    public string Category { get; private init; }
    public DateTime PublicationYear { get; private init; }

    private Book() { }

    public static Book Create(BookCreateModel createModel)
    {
        if (createModel.Price <= 0)
            throw new BookDomainException(ErrorMessages.BookPriceLowerThanOrEqualsZero);
        
        if (createModel.Stock < 0)
            throw new BookDomainException(ErrorMessages.BookStockCannotBeLowerThanZero);
        //TODO: Other domain validations...
        
        return new Book
        {
            Id = Guid.NewGuid(),
            Title = createModel.Title,
            Author = createModel.Author,
            ISBN = createModel.ISBN,
            Price = createModel.Price,
            Stock = createModel.Stock,
            Category = createModel.Category,
            PublicationYear = createModel.PublicationYear
        };
    }

    public bool IsStockAvailable(int quantity)
    {
        return Stock >= quantity;
    }
    
    public void ReduceStock(int quantity)
    {
        if (Stock < quantity)
            throw new BookDomainException(ErrorMessages.StockIsNotAvailable);
        
        Stock -= quantity;
    }
    public void RestoreStock(int quantity)
    {
        Stock += quantity;
    }

    public void EnsureValidForOrder(int quantity)
    {
        if (!IsStockAvailable(quantity))
            throw new BookDomainException(ErrorMessages.StockIsNotAvailable);
    }
}