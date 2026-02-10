namespace Catalog.API.Application.Queries.GetGameById;

public record GetGameByIdQuery(int GameId) : IQuery<GameViewModel?>;

public class GetGameByIdQueryHandler(IGameRepository gameRepository) : IQueryHandler<GetGameByIdQuery, GameViewModel?>
{
    public async Task<GameViewModel?> Handle(GetGameByIdQuery query, CancellationToken cancellationToken)
    {
        var game = await gameRepository.GetByIdAsync(query.GameId, cancellationToken);
        return game is not null ? GameViewModel.FromGame(game) : null;
    }
}
