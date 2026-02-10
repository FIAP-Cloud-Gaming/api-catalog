using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Infrastructure;

public sealed class CatalogContextTests : IDisposable
{
    private readonly CatalogContext _context;

    public CatalogContextTests()
    {
        _context = CatalogContextFactory.Create();
    }

    [Fact]
    public async Task SaveEntitiesAsync_ShouldReturnTrue()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();
        _context.Games.Add(game);

        // Act
        var result = await _context.SaveEntitiesAsync();

        // Assert
        result.Should().BeTrue();
        var savedGame = await _context.Games.FirstOrDefaultAsync(g => g.Id == game.Id);
        savedGame.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveEntitiesAsync_ShouldPersistGameData()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(
            title: "The Witcher 3",
            description: "A masterpiece RPG",
            priceAmount: 99.90m,
            priceCurrency: "BRL",
            genres: [Genre.RPG, Genre.Action],
            platforms: [Platform.PC, Platform.PlayStation5]);

        _context.Games.Add(game);
        await _context.SaveEntitiesAsync();

        // Act
        var retrievedGame = await _context.Games.FirstOrDefaultAsync(g => g.Title.Value == "The Witcher 3");

        // Assert
        retrievedGame.Should().NotBeNull();
        retrievedGame!.Title.Value.Should().Be("The Witcher 3");
        retrievedGame.Description.Should().Be("A masterpiece RPG");
        retrievedGame.Price.Amount.Should().Be(99.90m);
        retrievedGame.Price.Currency.Should().Be("BRL");
        retrievedGame.Genres.Should().HaveCount(2);
        retrievedGame.Platforms.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetEntitiesWithNotifications_WithNotifications_ShouldReturnEntities()
    {
        // Arrange - usar Game.Create que adiciona notificação
        var title = new GameTitle("Test Game");
        var price = new Price(59.99m, "BRL");
        var game = Game.Create(
            title,
            "Description",
            price,
            [Genre.Action],
            [Platform.PC]);

        _context.Games.Add(game);

        // Act
        var entitiesWithNotifications = _context.GetEntitiesWithNotifications();

        // Assert
        entitiesWithNotifications.Should().NotBeEmpty();
        entitiesWithNotifications.Should().Contain(game);
        game.Notifications.Should().NotBeEmpty();
    }

    [Fact]
    public void GetEntitiesWithNotifications_WithoutNotifications_ShouldReturnEmpty()
    {
        // Arrange - usar GameTestFactory que limpa notificações
        var game = GameTestFactory.CreateValidGame();
        _context.Games.Add(game);

        // Act
        var entitiesWithNotifications = _context.GetEntitiesWithNotifications();

        // Assert
        entitiesWithNotifications.Should().BeEmpty();
        game.Notifications.Should().BeEmpty();
    }

    [Fact]
    public void Games_DbSet_ShouldNotBeNull()
    {
        // Act
        var gamesDbSet = _context.Games;

        // Assert
        gamesDbSet.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
