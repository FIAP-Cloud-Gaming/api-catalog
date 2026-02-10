namespace Catalog.Infrastructure.Repositories;

public class GameRepository(CatalogContext context) : IGameRepository
{
    private readonly CatalogContext _context = context 
        ?? throw new ArgumentNullException(nameof(context));

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Game?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Games.FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default)
        => await _context.Games
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Game>> GetByStatusAsync(GameStatus status, CancellationToken ct = default) 
        => await _context.Games
            .Where(g => g.Status == status)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Game>> GetByGenreAsync(Genre genre, CancellationToken ct = default)
    {
        var genreJson = $"[\"{genre}\"]";
        return await _context.Games
            .Where(g => EF.Functions.JsonContains(EF.Property<string>(g, "_genres"), genreJson))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Game>> GetByPlatformAsync(Platform platform, CancellationToken ct = default)
    {
        var platformJson = $"[\"{platform}\"]";
        return await _context.Games
            .Where(g => EF.Functions.JsonContains(EF.Property<string>(g, "_platforms"), platformJson))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);
    }

    public void Add(Game game) 
        => _context.Games.Add(game);

    public void Update(Game game) 
        => _context.Entry(game).State = EntityState.Modified;

    public void Remove(Game game) 
        => _context.Games.Remove(game);
}
