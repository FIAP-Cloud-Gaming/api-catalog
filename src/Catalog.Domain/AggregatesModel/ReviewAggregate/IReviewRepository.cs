namespace Catalog.Domain.AggregatesModel.ReviewAggregate;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByGameIdAsync(int gameId, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByUserIdAsync(string userId, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> UserHasReviewedGameAsync(int gameId, string userId, CancellationToken cancellationToken = default);
    Task<(decimal AverageRating, int TotalReviews)> GetGameRatingStatsAsync(int gameId, CancellationToken cancellationToken = default);

    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
