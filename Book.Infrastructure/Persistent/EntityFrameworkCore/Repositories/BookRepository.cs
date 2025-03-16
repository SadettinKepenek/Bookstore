using Book.Application.Repositories.Book;
using Book.Application.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.Infrastructure.Persistent.EntityFrameworkCore.Repositories;

public class BookRepository : IBookRepository
{
    private readonly BookDbContext _bookDbContext;

    public BookRepository(BookDbContext bookDbContext)
    {
        _bookDbContext = bookDbContext;
    }

    public void Add(Domain.Book book)
    {
        _bookDbContext.Books.Add(book);
    }

    public async Task<(List<Domain.Book> Books, int TotalCount)> ListAsync(int offset, int limit,CancellationToken cancellationToken)
    {
        var query = _bookDbContext.Books.AsQueryable();
        
        var totalCount = await query.CountAsync(cancellationToken);
        var books = await query.Skip(offset).Take(limit).ToListAsync(cancellationToken);

        return (books, totalCount);
    }

    public async Task<Domain.Book> GetByIdAsync(Guid id,CancellationToken cancellationToken)
    {
        return await _bookDbContext.Books.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(List<Domain.Book> Books, int TotalCount)> SearchAsync(SearchBooksFilterModel filter, CancellationToken cancellationToken)
    {
        var query = _bookDbContext.Books
            .Where(x => filter.Title == null || x.Title.ToLower().Contains(filter.Title.ToLower()));
        
        var totalCount = await query.CountAsync(cancellationToken);
        var books = await query.Skip(filter.Offset).Take(filter.Limit).ToListAsync(cancellationToken);

        return (books, totalCount);
    }
}