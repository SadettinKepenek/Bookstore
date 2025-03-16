namespace Book.Application.Exceptions;

public class NotFoundException : BookApplicationException
{
    public NotFoundException(string message) : base(message) { }
}