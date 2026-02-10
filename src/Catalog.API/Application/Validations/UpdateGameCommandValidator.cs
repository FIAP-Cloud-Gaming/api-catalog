using Catalog.API.Application.Commands.UpdateGame;

namespace Catalog.API.Application.Validations;

public class UpdateGameCommandValidator : AbstractValidator<UpdateGameCommand>
{
    public UpdateGameCommandValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0).WithMessage("Game ID must be greater than zero");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => x.Description is not null);

        RuleFor(x => x.Publisher)
            .MaximumLength(200).WithMessage("Publisher must not exceed 200 characters")
            .When(x => x.Publisher is not null);

        RuleFor(x => x.Developer)
            .MaximumLength(200).WithMessage("Developer must not exceed 200 characters")
            .When(x => x.Developer is not null);

        RuleFor(x => x.CoverImageUrl)
            .MaximumLength(500).WithMessage("Cover image URL must not exceed 500 characters")
            .When(x => x.CoverImageUrl is not null);

        RuleFor(x => x)
            .Must(HasAtLeastOnePropertyToUpdate)
            .WithMessage("At least one property must be provided for update");
    }

    private static bool HasAtLeastOnePropertyToUpdate(UpdateGameCommand command)
    {
        return command.Description is not null
            || command.ReleaseDate is not null
            || command.Publisher is not null
            || command.Developer is not null
            || command.CoverImageUrl is not null
            || (command.Genres is not null && command.Genres.Count > 0)
            || (command.Platforms is not null && command.Platforms.Count > 0);
    }
}
