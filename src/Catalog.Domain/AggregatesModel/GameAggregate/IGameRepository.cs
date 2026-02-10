namespace Catalog.Domain.AggregatesModel.GameAggregate;

public interface IGameRepository : IRepository<Game>
{
    Task<Game?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetByStatusAsync(GameStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetByGenreAsync(Genre genre, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetByPlatformAsync(Platform platform, CancellationToken cancellationToken = default);

    void Add(Game game);
    void Update(Game game);
    void Remove(Game game);
}
