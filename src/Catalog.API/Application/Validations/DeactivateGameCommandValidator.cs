using Catalog.API.Application.Commands.DeactivateGame;

namespace Catalog.API.Application.Validations;

public class DeactivateGameCommandValidator : AbstractValidator<DeactivateGameCommand>
{
    public DeactivateGameCommandValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0).WithMessage("Game ID must be greater than zero");
    }
}
