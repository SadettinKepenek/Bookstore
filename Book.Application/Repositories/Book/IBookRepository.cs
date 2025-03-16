using Book.Application.Repositories.Models;

namespace Book.Application.Repositories.Book;

public interface IBookRepository
{
    public void Add(Domain.Book book);
    public Task<(List<Domain.Book> Books, int TotalCount)> ListAsync(int offset, int limit, CancellationToken cancellationToken);
    public Task<Domain.Book> GetByIdAsync(Guid id,CancellationToken cancellationToken);
    public Task<(List<Domain.Book> Books, int TotalCount)> SearchAsync(SearchBooksFilterModel filter, CancellationToken cancellationToken);
}