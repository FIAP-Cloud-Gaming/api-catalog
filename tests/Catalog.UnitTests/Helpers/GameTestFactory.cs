namespace Catalog.UnitTests.Helpers;

internal static class GameTestFactory
{
    public static Game CreateValidGame(
        string title = "Test Game",
        string description = "A test game description",
        decimal priceAmount = 59.99m,
        string priceCurrency = "BRL",
        Genre[]? genres = null,
        Platform[]? platforms = null,
        DateTime? releaseDate = null,
        string? publisher = "Test Publisher",
        string? developer = "Test Developer",
        string? coverImageUrl = "https://example.com/cover.jpg")
    {
        var gameTitle = new GameTitle(title);
        var price = new Price(priceAmount, priceCurrency);
        var gameGenres = genres ?? [Genre.Action];
        var gamePlatforms = platforms ?? [Platform.PC];

        var game = Game.Create(
            gameTitle,
            description,
            price,
            gameGenres,
            gamePlatforms,
            releaseDate,
            publisher,
            developer,
            coverImageUrl);

        game.ClearNotification();
        return game;
    }

    public static Game CreateValidGameWithId(
        int id,
        string title = "Test Game",
        decimal priceAmount = 59.99m)
    {
        var game = CreateValidGame(title: title, priceAmount: priceAmount);
        SetGameId(game, id);
        return game;
    }

    public static void SetGameId(Game game, int id)
    {
        var idProperty = typeof(Game).BaseType!.BaseType!
            .GetField("_id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        idProperty?.SetValue(game, id);
    }

    public static Review CreateValidReview(
        int gameId = 1,
        string userId = "user-123",
        int rating = 4,
        string comment = "Great game!",
        Platform platform = Platform.PC,
        bool verifiedPurchase = true,
        int? hoursPlayed = 10)
    {
        var ratingVo = new Rating(rating);
        var review = Review.Create(gameId, userId, ratingVo, comment, platform, verifiedPurchase, hoursPlayed);
        review.ClearNotification();
        return review;
    }
}
