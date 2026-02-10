using Catalog.API.Application.Commands.ActivateGame;

namespace Catalog.API.Application.Validations;

public class ActivateGameCommandValidator : AbstractValidator<ActivateGameCommand>
{
    public ActivateGameCommandValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0).WithMessage("Game ID must be greater than zero");
    }
}
