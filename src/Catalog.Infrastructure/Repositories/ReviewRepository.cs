using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ReviewRepository(IMongoDatabase database)
    : IReviewRepository
{
    private readonly IMongoCollection<Review> _reviews = database.GetCollection<Review>("reviews");

    public async Task<Review?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _reviews.Find(r => r.Id == id)
            .FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<Review>> GetByGameIdAsync(int gameId, int pageIndex, int pageSize, CancellationToken ct = default)
        => await _reviews.Find(r => r.GameId == gameId)
            .SortByDescending(r => r.CreatedAt)
            .Skip(pageIndex * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

    public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId, int pageIndex, int pageSize, CancellationToken ct = default)
        => await _reviews.Find(r => r.UserId == userId)
            .SortByDescending(r => r.CreatedAt)
            .Skip(pageIndex * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

    public async Task<bool> UserHasReviewedGameAsync(int gameId, string userId, CancellationToken ct = default)
        => await _reviews.CountDocumentsAsync(
            r => r.GameId == gameId && r.UserId == userId,
            cancellationToken: ct) > 0;

    public async Task<(decimal AverageRating, int TotalReviews)> GetGameRatingStatsAsync(int gameId, CancellationToken ct = default)
    {
        var reviews = await _reviews.Find(r => r.GameId == gameId)
            .ToListAsync(ct);

        if (reviews.Count == 0)
            return (0, 0);

        var avgRating = reviews.Average(r => (int)r.Rating);
        var total = reviews.Count;

        return ((decimal)avgRating, total);
    }

    public async Task AddAsync(Review review, CancellationToken ct = default) 
        => await _reviews.InsertOneAsync(review, cancellationToken: ct);

    public async Task UpdateAsync(Review review, CancellationToken ct = default) 
        => await _reviews.ReplaceOneAsync(
            r => r.Id == review.Id,
            review,
            cancellationToken: ct);

    public async Task DeleteAsync(int id, CancellationToken ct = default) 
        => await _reviews.DeleteOneAsync(r => r.Id == id, ct);
}
