namespace Book.Application.Exceptions;

public class ValidationException : BookApplicationException
{
    public ValidationException(string message) : base(message) { }
}