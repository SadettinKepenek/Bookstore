using Order.Application.Services;
using Saga.Application.Enums;

namespace Saga.Application.Steps.OrderSteps;

public class ProcessCompleteOrderStep : ISagaStep
{
    private readonly IOrderService _orderService;

    public ProcessCompleteOrderStep(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<SagaStepResult> ExecuteAsync(OrderSagaContext sagaContext, CancellationToken cancellationToken)
    {
        try
        {
            if (!sagaContext.StockReserved)
                return SagaStepResult.Failed("Stock is not available", SagaErrorType.BadRequest);

            await _orderService.CompleteOrderAsync(sagaContext.OrderId, cancellationToken);
            return SagaStepResult.SuccessResult();
        }
        catch (Exception ex)
        {
            var errorType = SagaErrorTypeMappings.ErrorTypeMapping.GetValueOrDefault(ex.GetType(), SagaErrorType.BadRequest);
            return SagaStepResult.Failed(ex.Message, errorType);
        }
    }

    public async Task RollbackAsync(OrderSagaContext sagaContext,CancellationToken cancellationToken)
    {
        await _orderService.CancelAsync(sagaContext.OrderId,cancellationToken);
    }
}
