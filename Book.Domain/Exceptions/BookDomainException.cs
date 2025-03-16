namespace Book.Domain.Exceptions;

public class BookDomainException: Exception
{
    public BookDomainException(string message) : base(message)
    {
    }

    public BookDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
