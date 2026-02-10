namespace Catalog.API.Application.Queries.GetGamesByPlatform;

public record GetGamesByPlatformQuery(Platform Platform) : IQuery<IEnumerable<GameViewModel>>;

public class GetGamesByPlatformQueryHandler(IGameRepository gameRepository) : IQueryHandler<GetGamesByPlatformQuery, IEnumerable<GameViewModel>>
{
    public async Task<IEnumerable<GameViewModel>> Handle(GetGamesByPlatformQuery query, CancellationToken cancellationToken)
    {
        var games = await gameRepository.GetByPlatformAsync(query.Platform, cancellationToken);
        return games.Select(GameViewModel.FromGame);
    }
}
