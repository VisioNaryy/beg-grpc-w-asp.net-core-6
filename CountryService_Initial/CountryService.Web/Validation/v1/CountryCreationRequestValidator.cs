using CountryServices.v1;
using FluentValidation;

namespace CountryService.Web.Validation.v1;

public class CountryCreationRequestValidator : AbstractValidator<CountryCreationRequest>
{
    public CountryCreationRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Name is required.");
        
        RuleFor(request => request.Description)
            .MinimumLength(5)
            .WithMessage("Description is required and should be longer than 5 characters");
    }
}