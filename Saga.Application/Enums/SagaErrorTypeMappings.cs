using Book.Application.Exceptions;
using Book.Domain.Exceptions;
using Order.Application.Exceptions;
using Order.Domain.Exceptions;
using OrderNotFoundException = Order.Application.Exceptions.NotFoundException;
using OrderValidationException = Order.Application.Exceptions.ValidationException;

using BookNotFoundException = Book.Application.Exceptions.NotFoundException;
using BookValidationException = Book.Application.Exceptions.ValidationException;

namespace Saga.Application.Enums;

public static class SagaErrorTypeMappings
{
    public static readonly Dictionary<Type, SagaErrorType> ErrorTypeMapping = new()
    {
        { typeof(OrderNotFoundException), SagaErrorType.NotFound },
        { typeof(OrderValidationException), SagaErrorType.BadRequest },
        { typeof(OrderApplicationException), SagaErrorType.BadRequest },
        { typeof(OrderDomainException), SagaErrorType.DomainError },
        
        { typeof(BookNotFoundException), SagaErrorType.NotFound },
        { typeof(BookValidationException), SagaErrorType.BadRequest },
        { typeof(BookApplicationException), SagaErrorType.BadRequest },
        { typeof(BookDomainException), SagaErrorType.DomainError }
    };
}