using FluentValidation;
using SympliSeoTracker.Domain.Models;

namespace SympliSeoTracker.Application.Validators
{
    public class SearchRequestValidator : AbstractValidator<SearchRequest>
    {
        public SearchRequestValidator()
        {
            RuleFor(x => x.Keywords)
                .NotEmpty().WithMessage("Keywords are required")
                .MaximumLength(200).WithMessage("Keywords cannot exceed 200 characters");

            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("URL is required")
                .MaximumLength(2000).WithMessage("URL cannot exceed 2000 characters");               

            RuleFor(x => x.Provider)
                .IsInEnum().WithMessage("Invalid search provider");
        }

       
    }
}