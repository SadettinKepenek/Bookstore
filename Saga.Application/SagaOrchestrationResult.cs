using Saga.Application.Enums;

namespace Saga.Application;

public class SagaOrchestrationResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }
    public SagaErrorType ErrorType { get; }

    private SagaOrchestrationResult(bool success, string errorMessage = "", SagaErrorType errorType = SagaErrorType.None)
    {
        Success = success;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static SagaOrchestrationResult SuccessResult() => new(true);
    
    public static SagaOrchestrationResult Failed(string errorMessage, SagaErrorType errorType) 
        => new(false, errorMessage, errorType);
}
