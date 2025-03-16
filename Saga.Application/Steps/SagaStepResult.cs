using Saga.Application.Enums;

public class SagaStepResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }
    public SagaErrorType? ErrorType { get; }

    private SagaStepResult(bool success, string errorMessage = "", SagaErrorType? errorType = null)
    {
        Success = success;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static SagaStepResult SuccessResult() => new(true);

    public static SagaStepResult Failed(string errorMessage, SagaErrorType errorType) =>
        new(false, errorMessage, errorType);
}