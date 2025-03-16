using Order.Application.Inputs;
using Order.Application.Services;
using Saga.Application.Enums;

namespace Saga.Application.Steps.OrderSteps;

public class ProcessInitializeOrderStep : ISagaStep
{
    private readonly IOrderService _orderService;

    public ProcessInitializeOrderStep(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<SagaStepResult> ExecuteAsync(OrderSagaContext sagaContext,CancellationToken cancellationToken)
    {
        try
        {
           var orderId =  await _orderService.CreateAsync(new CreateOrderInput
            {
                BookId = sagaContext.BookId,
                Quantity = sagaContext.Quantity,
                UnitPrice = sagaContext.UnitPrice
            }, cancellationToken);
           
            sagaContext.OrderId = orderId;
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
        
    }
}
