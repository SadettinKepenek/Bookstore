namespace Bookstore.Api.Validators.Book;

using FluentValidation;
using Models.Requests;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Author).NotEmpty().WithMessage("Author is required.");
        RuleFor(x => x.ISBN).NotEmpty().WithMessage("ISBN is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        RuleFor(x => x.PublicationYear).LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Publication year must be in the past.");
    }
}
