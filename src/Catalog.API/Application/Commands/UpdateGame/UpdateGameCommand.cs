namespace Catalog.API.Application.Commands.UpdateGame;

public record UpdateGameCommand(
    int GameId,
    string? Description,
    DateTime? ReleaseDate,
    string? Publisher,
    string? Developer,
    string? CoverImageUrl,
    IReadOnlyCollection<Genre>? Genres,
    IReadOnlyCollection<Platform>? Platforms
) : ICommand<GameViewModel>
{
    public static UpdateGameCommand FromRequest(int gameId, UpdateGameRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new UpdateGameCommand(
            gameId,
            request.Description,
            request.ReleaseDate,
            request.Publisher,
            request.Developer,
            request.CoverImageUrl,
            request.Genres,
            request.Platforms
        );
    }
}

public class UpdateGameCommandHandler(IGameRepository gameRepository)
    : ICommandHandler<UpdateGameCommand, GameViewModel>
{
    public async Task<GameViewModel> Handle(UpdateGameCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var game = await gameRepository.GetByIdAsync(command.GameId, cancellationToken)
            ?? throw new KeyNotFoundException($"Game with ID {command.GameId} not found.");

        game.UpdateDetails(
            command.Description,
            command.ReleaseDate,
            command.Publisher,
            command.Developer,
            command.CoverImageUrl
        );

        if (command.Genres is not null && command.Genres.Any())
        {
            foreach (var newGenre in command.Genres)
            {
                game.AddGenre(newGenre);
            }

            foreach (var existingGenre in game.Genres.ToList())
            {
                if (!command.Genres.Contains(existingGenre))
                {
                    game.RemoveGenre(existingGenre);
                }
            }
        }

        if (command.Platforms is not null && command.Platforms.Count != 0)
        {
            foreach (var newPlatform in command.Platforms)
            {
                game.AddPlatform(newPlatform);
            }

            foreach (var existingPlatform in game.Platforms.ToList())
            {
                if (!command.Platforms.Contains(existingPlatform))
                {
                    game.RemovePlatform(existingPlatform);
                }
            }
        }

        gameRepository.Update(game);
        await gameRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return GameViewModel.FromGame(game);
    }
}
