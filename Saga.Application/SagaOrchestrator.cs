using Saga.Application.Enums;
using Saga.Application.Steps;

namespace Saga.Application;

public class SagaOrchestrator
{
    private readonly IEnumerable<ISagaStep> _steps;
    private readonly Stack<ISagaStep> _executedSteps = new();

    public SagaOrchestrator(IEnumerable<ISagaStep> steps)
    {
        _steps = steps;
    }

    public async Task<SagaOrchestrationResult> ExecuteAsync(OrderSagaContext context, CancellationToken cancellationToken)
    {
        foreach (var step in _steps)
        {
            var result = await step.ExecuteAsync(context, cancellationToken);
            if (!result.Success)
            {
                await RollbackAsync(step, context, cancellationToken); 
                return SagaOrchestrationResult.Failed(result.ErrorMessage, result.ErrorType ?? SagaErrorType.BadRequest);
            }

            _executedSteps.Push(step); 
        }
        return SagaOrchestrationResult.SuccessResult();
    }

    private async Task RollbackAsync(ISagaStep failedStep, OrderSagaContext context, CancellationToken cancellationToken)
    {
        await failedStep.RollbackAsync(context, cancellationToken);

    }
}