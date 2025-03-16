using System.Net;
using Book.Application.Exceptions;
using Book.Domain.Exceptions;
using System.Text.Json;
using Order.Application.Exceptions;
using Order.Domain.Exceptions;
using BookNotFoundException = Book.Application.Exceptions.NotFoundException;
using BookValidationException = Book.Application.Exceptions.ValidationException;
using OrderNotFoundException = Order.Application.Exceptions.NotFoundException;
using OrderValidationException = Order.Application.Exceptions.ValidationException;

namespace Bookstore.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Dictionary<Type, HttpStatusCode> _exceptionStatusCodes = new()
    {
        { typeof(BookNotFoundException), HttpStatusCode.NotFound },
        { typeof(BookValidationException), HttpStatusCode.BadRequest },
        { typeof(OrderNotFoundException), HttpStatusCode.NotFound },
        { typeof(OrderValidationException), HttpStatusCode.BadRequest },
        { typeof(BookDomainException), HttpStatusCode.BadRequest },
        { typeof(OrderDomainException), HttpStatusCode.BadRequest },
        { typeof(BookApplicationException), HttpStatusCode.InternalServerError },
        { typeof(OrderApplicationException), HttpStatusCode.InternalServerError }
    };

    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = _exceptionStatusCodes
                .FirstOrDefault(x => x.Key.IsInstanceOfType(ex))
                .Value;

            response.StatusCode = (int)(statusCode != 0 ? statusCode : HttpStatusCode.InternalServerError);

            var result = JsonSerializer.Serialize(new { Success = false, Message = ex.Message });
            await response.WriteAsync(result);
        }
    }
}