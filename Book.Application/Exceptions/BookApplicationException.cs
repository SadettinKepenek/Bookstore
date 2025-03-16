namespace Book.Application.Exceptions;

public class BookApplicationException: Exception
{
    public BookApplicationException(string message) : base(message)
    {
    }

    public BookApplicationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
