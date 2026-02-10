namespace Catalog.API.Application.Commands.ActivateGame;

public record ActivateGameCommand(int GameId) : ICommand<GameViewModel>;

public class ActivateGameCommandHandler(IGameRepository gameRepository)
    : ICommandHandler<ActivateGameCommand, GameViewModel>
{
    public async Task<GameViewModel> Handle(ActivateGameCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var game = await gameRepository.GetByIdAsync(command.GameId, cancellationToken)
            ?? throw new KeyNotFoundException($"Game with ID {command.GameId} not found.");

        game.Publish();

        gameRepository.Update(game);
        await gameRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return GameViewModel.FromGame(game);
    }
}
