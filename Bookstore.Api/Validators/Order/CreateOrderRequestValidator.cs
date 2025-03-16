using FluentValidation;
using Bookstore.Api.Models.Requests;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("BookId is required.");  

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}