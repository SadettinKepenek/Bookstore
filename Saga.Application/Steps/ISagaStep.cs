namespace Saga.Application.Steps;

public interface ISagaStep
{
    Task<SagaStepResult> ExecuteAsync(OrderSagaContext sagaContext,CancellationToken cancellationToken);
    Task RollbackAsync(OrderSagaContext sagaContext,CancellationToken cancellationToken);
}