namespace Catalog.Domain.SearchModels;

public record GameMetrics
{
    public IReadOnlyList<MetricBucket> TopGenres { get; init; } = [];
    public IReadOnlyList<MetricBucket> TopPlatforms { get; init; } = [];
    public decimal AveragePrice { get; init; }
    public IReadOnlyList<PriceRangeBucket> PriceRanges { get; init; } = [];
    public IReadOnlyList<MetricBucket> MostReviewed { get; init; } = [];
}

public record MetricBucket(string Key, long DocCount);

public record PriceRangeBucket(string Key, double? From, double? To, long DocCount);
