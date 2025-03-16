using Book.Application.Constants;
using Book.Application.Exceptions;
using Book.Application.Inputs;
using Book.Application.Mappers;
using Book.Application.Outputs;
using Book.Application.Repositories;
using Book.Application.Repositories.Book;
using Book.Application.Repositories.Models;

namespace Book.Application.Services.Book;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookUnitOfWork _bookUnitOfWork;
    
    public BookService(IBookRepository bookRepository, IBookUnitOfWork bookUnitOfWork)
    {
        _bookRepository = bookRepository;
        _bookUnitOfWork = bookUnitOfWork;
    }

    public async Task CreateAsync(CreateBookInput createBookInput,CancellationToken cancellationToken)
    {
        var bookCreateModel = BookMappers.MapToDomainCreateModel(createBookInput);
        var book = Domain.Book.Create(bookCreateModel);
        
        _bookRepository.Add(book);
        await _bookUnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ListBooksOutput> ListAsync(int offset, int limit, CancellationToken cancellationToken)
    {
        var (books,totalCount) = await _bookRepository.ListAsync(offset, limit, cancellationToken);
        return BookMappers.MapToListBooksOutput(books, totalCount);
    }

    public async Task<GetBookDetailOutput> GetDetailAsync(Guid id, CancellationToken cancellationToken)
    {
        var book = await GetBookById(id, cancellationToken);

        return BookMappers.MapToGetBookDetailOutput(book);
    }

    public async Task<SearchBooksOutput> SearchAsync(SearchBooksInput input, CancellationToken cancellationToken)
    {
        var filter = new SearchBooksFilterModel
        {
            Limit = input.Limit,
            Offset = input.Offset,
            Title = input.Title
        };

        var (books,totalCount) = await _bookRepository.SearchAsync(filter, cancellationToken);
        return BookMappers.MapToSearchBooksOutput(books, totalCount);
    }

    public async Task<bool> CheckStockAsync(Guid bookId,int quantity, CancellationToken cancellationToken)
    {
        var book = await GetBookById(bookId, cancellationToken);
        return book.IsStockAvailable(quantity);
    }

    public async Task ReduceStockAsync(Guid bookId,int quantity,CancellationToken cancellationToken)
    {
        var book = await GetBookById(bookId, cancellationToken);

        book.ReduceStock(quantity);
        await _bookUnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreStockAsync(Guid bookId,int quantity,CancellationToken cancellationToken)
    {
        var book = await GetBookById(bookId, cancellationToken);
        book.RestoreStock(quantity);
        await _bookUnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<GetBookForOrderOutput> ValidateAndGetBookForOrderAsync(Guid bookId, int orderQuantity,CancellationToken cancellationToken)
    {
        var book = await GetBookById(bookId, cancellationToken);
        book.EnsureValidForOrder(orderQuantity);
        
        return new GetBookForOrderOutput
        {
            Id = book.Id,
            UnitPrice = book.Price
        };
    }
    
    private async Task<Domain.Book> GetBookById(Guid bookId, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
        if (book == null)
            throw new NotFoundException(ErrorMessages.BookNotFound);

        return book;
    }
}