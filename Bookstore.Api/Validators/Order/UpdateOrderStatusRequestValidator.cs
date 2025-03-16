using FluentValidation;
using Bookstore.Api.Models.Requests;

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid order status.");
    }
}