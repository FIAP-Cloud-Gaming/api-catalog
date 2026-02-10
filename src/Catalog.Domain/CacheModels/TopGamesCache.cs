namespace Catalog.Domain.CacheModels;

public class TopGamesCache
{
    public List<TopGameEntry> Games { get; set; } = [];
    public DateTime CachedAt { get; set; }
    public int TotalGames { get; set; }

    public static string GetCacheKey() => "catalog:top-games";
}

public class TopGameEntry
{
    public int GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal Price { get; set; }
    public decimal AverageRating { get; set; }
}
