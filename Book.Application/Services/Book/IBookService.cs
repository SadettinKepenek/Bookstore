using Book.Application.Inputs;
using Book.Application.Outputs;

namespace Book.Application.Services.Book;

public interface IBookService
{
    public Task CreateAsync(CreateBookInput createBookInput,CancellationToken cancellationToken);
    public Task<ListBooksOutput> ListAsync(int offset,int limit,CancellationToken cancellationToken);
    public Task<GetBookDetailOutput> GetDetailAsync(Guid id,CancellationToken cancellationToken);
    public Task<SearchBooksOutput> SearchAsync(SearchBooksInput input,CancellationToken cancellationToken);
    public Task<bool> CheckStockAsync(Guid bookId, int quantity, CancellationToken cancellationToken);

    public Task ReduceStockAsync(Guid bookId,int quantity,CancellationToken cancellationToken);
    public Task RestoreStockAsync(Guid bookId,int quantity,CancellationToken cancellationToken);
    public Task<GetBookForOrderOutput> ValidateAndGetBookForOrderAsync(Guid bookId, int orderQuantity, CancellationToken cancellationToken);
}