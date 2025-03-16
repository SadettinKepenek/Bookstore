namespace Order.Application.Exceptions;

public class NotFoundException : OrderApplicationException
{
    public NotFoundException(string message) : base(message) { }
}