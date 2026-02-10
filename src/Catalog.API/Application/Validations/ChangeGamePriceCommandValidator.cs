using Catalog.API.Application.Commands.ChangeGamePrice;

namespace Catalog.API.Application.Validations;

public class ChangeGamePriceCommandValidator : AbstractValidator<ChangeGamePriceCommand>
{
    public ChangeGamePriceCommandValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0).WithMessage("Game ID must be greater than zero");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Price amount must be greater than or equal to zero");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter ISO currency code (e.g., USD, EUR)");
    }
}
