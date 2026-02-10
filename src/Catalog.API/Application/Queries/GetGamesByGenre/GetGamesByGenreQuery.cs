namespace Catalog.API.Application.Queries.GetGamesByGenre;

public record GetGamesByGenreQuery(Genre Genre) : IQuery<IEnumerable<GameViewModel>>;

public class GetGamesByGenreQueryHandler(IGameRepository gameRepository) : IQueryHandler<GetGamesByGenreQuery, IEnumerable<GameViewModel>>
{
    public async Task<IEnumerable<GameViewModel>> Handle(GetGamesByGenreQuery query, CancellationToken cancellationToken)
    {
        var games = await gameRepository.GetByGenreAsync(query.Genre, cancellationToken);
        return games.Select(GameViewModel.FromGame);
    }
}
