using FluentValidation;
using Bookstore.Api.Models.Requests;

public class SearchBooksRequestValidator : AbstractValidator<SearchBooksRequest>
{
    public SearchBooksRequestValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than zero.") 
            .LessThanOrEqualTo(100).WithMessage("Limit cannot exceed 100.");
        
        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset cannot be negative."); 

        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage("Title cannot be longer than 100 characters.");
    }
}