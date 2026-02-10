namespace Catalog.API.Application.Commands.ChangeGamePrice;

public record ChangeGamePriceCommand(
    int GameId,
    decimal Amount,
    string Currency
) : ICommand<GameViewModel>
{
    public static ChangeGamePriceCommand FromRequest(int gameId, ChangePriceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new ChangeGamePriceCommand(
            gameId,
            request.Amount,
            request.Currency
        );
    }
}

public class ChangeGamePriceCommandHandler(IGameRepository gameRepository)
    : ICommandHandler<ChangeGamePriceCommand, GameViewModel>
{
    public async Task<GameViewModel> Handle(ChangeGamePriceCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var game = await gameRepository.GetByIdAsync(command.GameId, cancellationToken)
            ?? throw new KeyNotFoundException($"Game with ID {command.GameId} not found.");

        var newPrice = new Price(command.Amount, command.Currency);
        game.ChangePrice(newPrice);

        gameRepository.Update(game);
        await gameRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return GameViewModel.FromGame(game);
    }
}
