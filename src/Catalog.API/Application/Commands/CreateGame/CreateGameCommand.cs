namespace Catalog.API.Application.Commands.CreateGame;

public record CreateGameCommand(
    string Title,
    string Description,
    decimal PriceAmount,
    string PriceCurrency,
    IReadOnlyCollection<Genre> Genres,
    IReadOnlyCollection<Platform> Platforms,
    DateTime? ReleaseDate,
    string? Publisher,
    string? Developer,
    string? CoverImageUrl
) : ICommand<GameViewModel>
{
    public static CreateGameCommand FromRequest(CreateGameRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CreateGameCommand(
            request.Title,
            request.Description,
            request.PriceAmount,
            request.PriceCurrency,
            request.Genres,
            request.Platforms,
            request.ReleaseDate,
            request.Publisher,
            request.Developer,
            request.CoverImageUrl
        );
    }
}

public class CreateGameCommandHandler(IGameRepository gameRepository)
    : ICommandHandler<CreateGameCommand, GameViewModel>
{
    public async Task<GameViewModel> Handle(CreateGameCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var title = new GameTitle(command.Title);
        var price = new Price(command.PriceAmount, command.PriceCurrency);

        var game = Game.Create(
            title,
            command.Description,
            price,
            command.Genres,
            command.Platforms,
            command.ReleaseDate,
            command.Publisher,
            command.Developer,
            command.CoverImageUrl
        );

        gameRepository.Add(game);
        await gameRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return GameViewModel.FromGame(game);
    }
}
