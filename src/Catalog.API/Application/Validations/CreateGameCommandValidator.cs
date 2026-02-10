using Catalog.API.Application.Commands.CreateGame;

namespace Catalog.API.Application.Validations;

public class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.PriceAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to zero");

        RuleFor(x => x.PriceCurrency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter ISO currency code (e.g., USD, EUR)");

        RuleFor(x => x.Genres)
            .NotEmpty().WithMessage("At least one genre is required")
            .Must(genres => genres != null && genres.Count > 0)
            .WithMessage("At least one genre must be specified");

        RuleFor(x => x.Platforms)
            .NotEmpty().WithMessage("At least one platform is required")
            .Must(platforms => platforms != null && platforms.Count > 0)
            .WithMessage("At least one platform must be specified");

        RuleFor(x => x.Publisher)
            .MaximumLength(200).WithMessage("Publisher must not exceed 200 characters")
            .When(x => x.Publisher is not null);

        RuleFor(x => x.Developer)
            .MaximumLength(200).WithMessage("Developer must not exceed 200 characters")
            .When(x => x.Developer is not null);

        RuleFor(x => x.CoverImageUrl)
            .MaximumLength(500).WithMessage("Cover image URL must not exceed 500 characters")
            .When(x => x.CoverImageUrl is not null);
    }
}
