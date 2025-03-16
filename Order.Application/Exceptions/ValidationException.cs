namespace Order.Application.Exceptions;

public class ValidationException : OrderApplicationException
{
    public ValidationException(string message) : base(message) { }
}