namespace Catalog.API.Application.Commands.DeactivateGame;

public record DeactivateGameCommand(int GameId) : ICommand<GameViewModel>;

public class DeactivateGameCommandHandler(IGameRepository gameRepository)
    : ICommandHandler<DeactivateGameCommand, GameViewModel>
{
    public async Task<GameViewModel> Handle(DeactivateGameCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var game = await gameRepository.GetByIdAsync(command.GameId, cancellationToken)
            ?? throw new KeyNotFoundException($"Game with ID {command.GameId} not found.");

        game.Unpublish();

        gameRepository.Update(game);
        await gameRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return GameViewModel.FromGame(game);
    }
}
