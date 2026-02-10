namespace Catalog.API.Application.Commands.DeleteGame;

public record DeleteGameCommand(int GameId) : ICommand<bool>;

public class DeleteGameCommandHandler(IGameRepository gameRepository)
    : ICommandHandler<DeleteGameCommand, bool>
{
    public async Task<bool> Handle(DeleteGameCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var game = await gameRepository.GetByIdAsync(command.GameId, cancellationToken);

        if (game is null) return false;

        gameRepository.Remove(game);
        await gameRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}
