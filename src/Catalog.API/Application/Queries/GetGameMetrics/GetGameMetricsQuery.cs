namespace Catalog.API.Application.Queries.GetGameMetrics;

public record GetGameMetricsQuery() : IQuery<GameMetrics>;

public class GetGameMetricsQueryHandler(IGameSearchRepository gameSearchRepository)
    : IQueryHandler<GetGameMetricsQuery, GameMetrics>
{
    public async Task<GameMetrics> Handle(GetGameMetricsQuery query, CancellationToken cancellationToken)
    {
        return await gameSearchRepository.GetGameMetricsAsync(cancellationToken);
    }
}
