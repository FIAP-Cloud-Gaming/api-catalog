using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Domain.Aggregates;

public sealed class GameTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateGame()
    {
        // Arrange
        var title = new GameTitle("Test Game");
        var description = "A test game description";
        var price = new Price(59.99m, "BRL");
        var genres = new[] { Genre.Action };
        var platforms = new[] { Platform.PC };

        // Act
        var game = Game.Create(title, description, price, genres, platforms);

        // Assert
        game.Should().NotBeNull();
        game.Title.Value.Should().Be("Test Game");
        game.Description.Should().Be(description);
        game.Price.Should().Be(price);
        game.Status.Should().Be(GameStatus.Draft);
        game.Genres.Should().ContainSingle().Which.Should().Be(Genre.Action);
        game.Platforms.Should().ContainSingle().Which.Should().Be(Platform.PC);
        game.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldRaiseGameCreatedDomainEvent()
    {
        // Arrange
        var title = new GameTitle("Test Game");
        var price = new Price(59.99m);
        var genres = new[] { Genre.Action };
        var platforms = new[] { Platform.PC };

        // Act
        var game = Game.Create(title, "Description", price, genres, platforms);

        // Assert
        var domainEvents = game.Notifications;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<GameCreatedDomainEvent>();
        var createdEvent = (GameCreatedDomainEvent)domainEvents.First();
        createdEvent.GameId.Should().Be(game.Id);
        createdEvent.Title.Should().Be("Test Game");
    }

    [Fact]
    public void Create_WithNoGenres_ShouldThrowCatalogDomainException()
    {
        // Arrange
        var title = new GameTitle("Test Game");
        var price = new Price(59.99m);
        var emptyGenres = Array.Empty<Genre>();
        var platforms = new[] { Platform.PC };

        // Act
        var act = () => Game.Create(title, "Description", price, emptyGenres, platforms);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Game must have at least one genre.");
    }

    [Fact]
    public void Create_WithNoPlatforms_ShouldThrowCatalogDomainException()
    {
        // Arrange
        var title = new GameTitle("Test Game");
        var price = new Price(59.99m);
        var genres = new[] { Genre.Action };
        var emptyPlatforms = Array.Empty<Platform>();

        // Act
        var act = () => Game.Create(title, "Description", price, genres, emptyPlatforms);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Game must have at least one platform.");
    }

    [Fact]
    public void Create_WithNullTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var price = new Price(59.99m);
        var genres = new[] { Genre.Action };
        var platforms = new[] { Platform.PC };

        // Act
        var act = () => Game.Create(null!, "Description", price, genres, platforms);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Create_WithNullPrice_ShouldThrowArgumentNullException()
    {
        // Arrange
        var title = new GameTitle("Test Game");
        var genres = new[] { Genre.Action };
        var platforms = new[] { Platform.PC };

        // Act
        var act = () => Game.Create(title, "Description", null!, genres, platforms);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("price");
    }

    [Fact]
    public void Create_WithNullDescription_ShouldDefaultToEmptyString()
    {
        // Arrange
        var title = new GameTitle("Test Game");
        var price = new Price(59.99m);
        var genres = new[] { Genre.Action };
        var platforms = new[] { Platform.PC };

        // Act
        var game = Game.Create(title, null!, price, genres, platforms);

        // Assert
        game.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void Create_WithDuplicateGenres_ShouldStorDistinct()
    {
        // Arrange
        var title = new GameTitle("Test Game");
        var price = new Price(59.99m);
        var genres = new[] { Genre.Action, Genre.Action, Genre.Adventure, Genre.Action };
        var platforms = new[] { Platform.PC };

        // Act
        var game = Game.Create(title, "Description", price, genres, platforms);

        // Assert
        game.Genres.Should().HaveCount(2);
        game.Genres.Should().Contain(Genre.Action);
        game.Genres.Should().Contain(Genre.Adventure);
    }

    [Fact]
    public void ChangePrice_ShouldUpdatePrice()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(priceAmount: 59.99m);
        var newPrice = new Price(49.99m);

        // Act
        game.ChangePrice(newPrice);

        // Assert
        game.Price.Should().Be(newPrice);
        game.Price.Amount.Should().Be(49.99m);
    }

    [Fact]
    public void ChangePrice_ShouldRaiseGamePriceChangedDomainEvent()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(priceAmount: 59.99m);
        var oldPrice = game.Price;
        var newPrice = new Price(49.99m);

        // Act
        game.ChangePrice(newPrice);

        // Assert
        var domainEvents = game.Notifications;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<GamePriceChangedDomainEvent>();
        var priceChangedEvent = (GamePriceChangedDomainEvent)domainEvents.First();
        priceChangedEvent.GameId.Should().Be(game.Id);
        priceChangedEvent.OldPrice.Should().Be(oldPrice);
        priceChangedEvent.NewPrice.Should().Be(newPrice);
    }

    [Fact]
    public void ChangePrice_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();

        // Act
        var act = () => game.ChangePrice(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateDescription()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(description: "Old description");

        // Act
        game.UpdateDetails(description: "New description");

        // Assert
        game.Description.Should().Be("New description");
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateMultipleFields()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();
        var newReleaseDate = new DateTime(2024, 12, 25);

        // Act
        game.UpdateDetails(
            description: "Updated description",
            releaseDate: newReleaseDate,
            publisher: "New Publisher",
            developer: "New Developer",
            coverImageUrl: "https://new-url.com/cover.jpg");

        // Assert
        game.Description.Should().Be("Updated description");
        game.ReleaseDate.Should().Be(newReleaseDate);
        game.Publisher.Should().Be("New Publisher");
        game.Developer.Should().Be("New Developer");
        game.CoverImageUrl.Should().Be("https://new-url.com/cover.jpg");
    }

    [Fact]
    public void UpdateDetails_ShouldRaiseGameUpdatedDomainEvent()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(title: "My Game");

        // Act
        game.UpdateDetails(description: "Updated");

        // Assert
        var domainEvents = game.Notifications;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<GameUpdatedDomainEvent>();
        var updatedEvent = (GameUpdatedDomainEvent)domainEvents.First();
        updatedEvent.GameId.Should().Be(game.Id);
        updatedEvent.Title.Should().Be("My Game");
    }

    [Fact]
    public void UpdateDetails_ShouldSetUpdatedAt()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        game.UpdateDetails(description: "Updated");

        // Assert
        game.UpdatedAt.Should().NotBeNull();
        game.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public void AddGenre_ShouldAddNewGenre()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(genres: new[] { Genre.Action });

        // Act
        game.AddGenre(Genre.Adventure);

        // Assert
        game.Genres.Should().HaveCount(2);
        game.Genres.Should().Contain(Genre.Action);
        game.Genres.Should().Contain(Genre.Adventure);
    }

    [Fact]
    public void AddGenre_DuplicateGenre_ShouldNotAdd()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(genres: new[] { Genre.Action });

        // Act
        game.AddGenre(Genre.Action);

        // Assert
        game.Genres.Should().ContainSingle();
        game.Genres.Should().Contain(Genre.Action);
    }

    [Fact]
    public void RemoveGenre_ShouldRemoveGenre()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(genres: new[] { Genre.Action, Genre.Adventure });

        // Act
        game.RemoveGenre(Genre.Action);

        // Assert
        game.Genres.Should().ContainSingle();
        game.Genres.Should().Contain(Genre.Adventure);
        game.Genres.Should().NotContain(Genre.Action);
    }

    [Fact]
    public void RemoveGenre_LastGenre_ShouldThrowCatalogDomainException()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(genres: new[] { Genre.Action });

        // Act
        var act = () => game.RemoveGenre(Genre.Action);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Game must have at least one genre.");
    }

    [Fact]
    public void AddPlatform_ShouldAddNewPlatform()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(platforms: new[] { Platform.PC });

        // Act
        game.AddPlatform(Platform.PlayStation5);

        // Assert
        game.Platforms.Should().HaveCount(2);
        game.Platforms.Should().Contain(Platform.PC);
        game.Platforms.Should().Contain(Platform.PlayStation5);
    }

    [Fact]
    public void AddPlatform_DuplicatePlatform_ShouldNotAdd()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(platforms: new[] { Platform.PC });

        // Act
        game.AddPlatform(Platform.PC);

        // Assert
        game.Platforms.Should().ContainSingle();
        game.Platforms.Should().Contain(Platform.PC);
    }

    [Fact]
    public void RemovePlatform_ShouldRemovePlatform()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(platforms: new[] { Platform.PC, Platform.PlayStation5 });

        // Act
        game.RemovePlatform(Platform.PC);

        // Assert
        game.Platforms.Should().ContainSingle();
        game.Platforms.Should().Contain(Platform.PlayStation5);
        game.Platforms.Should().NotContain(Platform.PC);
    }

    [Fact]
    public void RemovePlatform_LastPlatform_ShouldThrowCatalogDomainException()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(platforms: new[] { Platform.PC });

        // Act
        var act = () => game.RemovePlatform(Platform.PC);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Game must have at least one platform.");
    }

    [Fact]
    public void Publish_ShouldSetStatusToPublished()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();
        game.Status.Should().Be(GameStatus.Draft);

        // Act
        game.Publish();

        // Assert
        game.Status.Should().Be(GameStatus.Published);
    }

    [Fact]
    public void Publish_ShouldRaiseGamePublishedDomainEvent()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(title: "My Game");

        // Act
        game.Publish();

        // Assert
        var domainEvents = game.Notifications;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<GamePublishedDomainEvent>();
        var publishedEvent = (GamePublishedDomainEvent)domainEvents.First();
        publishedEvent.GameId.Should().Be(game.Id);
        publishedEvent.Title.Should().Be("My Game");
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ShouldThrow()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();
        game.Publish();
        game.ClearNotification();

        // Act
        var act = () => game.Publish();

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Game is already published.");
    }

    [Fact]
    public void Unpublish_WhenPublished_ShouldSetStatusToUnpublished()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();
        game.Publish();
        game.ClearNotification();

        // Act
        game.Unpublish();

        // Assert
        game.Status.Should().Be(GameStatus.Unpublished);
    }

    [Fact]
    public void Unpublish_WhenNotPublished_ShouldThrow()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();
        game.Status.Should().Be(GameStatus.Draft);

        // Act
        var act = () => game.Unpublish();

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Only published games can be unpublished.");
    }

    [Fact]
    public void Archive_ShouldSetStatusToArchived()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();

        // Act
        game.Archive();

        // Assert
        game.Status.Should().Be(GameStatus.Archived);
    }

    [Fact]
    public void UpdateRatingStats_ShouldUpdateValues()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();
        game.AverageRating.Should().Be(0);
        game.TotalReviews.Should().Be(0);

        // Act
        game.UpdateRatingStats(averageRating: 4.5m, totalReviews: 10);

        // Assert
        game.AverageRating.Should().Be(4.5m);
        game.TotalReviews.Should().Be(10);
        game.UpdatedAt.Should().NotBeNull();
    }
}
