using Catalog.API.Application.Queries.SearchGames;

namespace Catalog.API.Application.Validations;

public class SearchGamesQueryValidator : AbstractValidator<SearchGamesQuery>
{
    public SearchGamesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than zero");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum price must be greater than or equal to zero")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum price must be greater than or equal to zero")
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice ?? 0)
            .WithMessage("Maximum price must be greater than or equal to minimum price")
            .When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue);

        RuleFor(x => x.MinRating)
            .InclusiveBetween(0, 5).WithMessage("Minimum rating must be between 0 and 5")
            .When(x => x.MinRating.HasValue);
    }
}
