using Book.Application.Exceptions;
using Book.Application.Services.Book;
using Book.Domain.Exceptions;
using Saga.Application.Enums;

namespace Saga.Application.Steps.BookSteps;

public class ProcessStockStep : ISagaStep
{
    private readonly IBookService _bookService;

    public ProcessStockStep(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<SagaStepResult> ExecuteAsync(OrderSagaContext sagaContext, CancellationToken cancellationToken)
    {
        try
        {
            var stockAvailable = await _bookService.CheckStockAsync(sagaContext.BookId, sagaContext.Quantity, cancellationToken);
            if (!stockAvailable)
                return SagaStepResult.Failed("Stock is not available", SagaErrorType.BadRequest);

            await _bookService.ReduceStockAsync(sagaContext.BookId, sagaContext.Quantity, cancellationToken);
            sagaContext.StockReserved = true;

            return SagaStepResult.SuccessResult();
        }
        catch (Exception ex)
        {
            var errorType = SagaErrorTypeMappings.ErrorTypeMapping.GetValueOrDefault(ex.GetType(), SagaErrorType.BadRequest);
            return SagaStepResult.Failed(errorType == SagaErrorType.BadRequest ? "Stock is not available" : ex.Message, errorType);
        }
    }

    public async Task RollbackAsync(OrderSagaContext sagaContext, CancellationToken cancellationToken)
    {
        if (sagaContext.StockReserved)
        {
            await _bookService.RestoreStockAsync(sagaContext.BookId, sagaContext.Quantity, cancellationToken);
        }
    }
}