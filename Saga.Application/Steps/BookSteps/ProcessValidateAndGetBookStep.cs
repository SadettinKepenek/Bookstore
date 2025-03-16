using Book.Application.Exceptions;
using Book.Application.Services.Book;
using Book.Domain.Exceptions;
using Saga.Application.Enums;

namespace Saga.Application.Steps.BookSteps;

public class ProcessValidateAndGetBookStep : ISagaStep
{
    private readonly IBookService _bookService;

    public ProcessValidateAndGetBookStep(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<SagaStepResult> ExecuteAsync(OrderSagaContext sagaContext, CancellationToken cancellationToken)
    {
        try
        {
            var book = await _bookService.ValidateAndGetBookForOrderAsync(sagaContext.BookId, sagaContext.Quantity, cancellationToken);
            sagaContext.UnitPrice = book.UnitPrice;
            
            return SagaStepResult.SuccessResult();
        }
        catch (Exception ex)
        {
            var errorType = SagaErrorTypeMappings.ErrorTypeMapping.GetValueOrDefault(ex.GetType(), SagaErrorType.BadRequest);
            return SagaStepResult.Failed(ex.Message, errorType);
        }
    }

    public async Task RollbackAsync(OrderSagaContext sagaContext, CancellationToken cancellationToken)
    {
        
    }
}