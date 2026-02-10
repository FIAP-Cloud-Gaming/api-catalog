namespace Catalog.API.Application.Queries.GetGamesByStatus;

public record GetGamesByStatusQuery(GameStatus Status) : IQuery<IEnumerable<GameViewModel>>;

public class GetGamesByStatusQueryHandler(IGameRepository gameRepository) : IQueryHandler<GetGamesByStatusQuery, IEnumerable<GameViewModel>>
{
    public async Task<IEnumerable<GameViewModel>> Handle(GetGamesByStatusQuery query, CancellationToken cancellationToken)
    {
        var games = await gameRepository.GetByStatusAsync(query.Status, cancellationToken);
        return games.Select(GameViewModel.FromGame);
    }
}
