namespace Catalog.API.Application.Commands.ReindexGames;

public record ReindexGamesCommand : ICommand<ReindexGamesResult>;

public record ReindexGamesResult(int TotalGames, int IndexedGames, int FailedGames);

public class ReindexGamesCommandHandler(
    IGameRepository gameRepository,
    IGameSearchRepository searchRepository,
    ILogger<ReindexGamesCommandHandler> logger)
    : ICommandHandler<ReindexGamesCommand, ReindexGamesResult>
{
    public async Task<ReindexGamesResult> Handle(ReindexGamesCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting game reindexing process");

        var games = await gameRepository.GetAllAsync(cancellationToken);
        var gamesList = games.ToList();

        var totalGames = gamesList.Count;
        var indexedGames = 0;
        var failedGames = 0;

        foreach (var game in gamesList)
        {
            try
            {
                var searchDoc = GameSearchDocument.FromGame(game);
                await searchRepository.IndexGameAsync(searchDoc, cancellationToken);
                indexedGames++;

                logger.LogInformation("Game {GameId} - {Title} indexed successfully", game.Id, game.Title.Value);
            }
            catch (Exception ex)
            {
                failedGames++;
                logger.LogError(ex, "Failed to index game {GameId} - {Title}", game.Id, game.Title.Value);
            }
        }

        logger.LogInformation(
            "Reindexing completed. Total: {Total}, Indexed: {Indexed}, Failed: {Failed}",
            totalGames, indexedGames, failedGames);

        return new ReindexGamesResult(totalGames, indexedGames, failedGames);
    }
}
