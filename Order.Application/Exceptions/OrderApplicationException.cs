namespace Order.Application.Exceptions;

public class OrderApplicationException: Exception
{
    public OrderApplicationException(string message) : base(message)
    {
    }

    public OrderApplicationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
