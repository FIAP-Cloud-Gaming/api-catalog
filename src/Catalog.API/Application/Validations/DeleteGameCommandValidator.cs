using Catalog.API.Application.Commands.DeleteGame;

namespace Catalog.API.Application.Validations;

public class DeleteGameCommandValidator : AbstractValidator<DeleteGameCommand>
{
    public DeleteGameCommandValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0).WithMessage("Game ID must be greater than zero");
    }
}
