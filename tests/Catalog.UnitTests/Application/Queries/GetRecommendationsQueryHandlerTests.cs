using Catalog.API.Application.Queries.GetRecommendations;

namespace Catalog.UnitTests.Application.Queries;

public sealed class GetRecommendationsQueryHandlerTests
{
    private readonly IGameSearchRepository _searchRepository;
    private readonly GetRecommendationsQueryHandler _handler;

    public GetRecommendationsQueryHandlerTests()
    {
        _searchRepository = Substitute.For<IGameSearchRepository>();
        _handler = new GetRecommendationsQueryHandler(_searchRepository);
    }

    [Fact]
    public async Task Handle_WithResults_ShouldReturnMappedViewModels()
    {
        // Arrange
        var doc1 = new GameSearchDocument
        {
            Id = 10,
            Title = "Elden Ring",
            Description = "Epic RPG",
            Price = 199.99m,
            Currency = "BRL",
            Genres = ["RPG", "Action"],
            Platforms = ["PC", "PlayStation5"],
            Status = "Published",
            AverageRating = 4.8m,
            TotalReviews = 1500,
            ReleaseDate = new DateTime(2022, 2, 25),
            Publisher = "Bandai Namco",
            Developer = "FromSoftware",
            CoverImageUrl = "https://example.com/elden.jpg",
            IndexedAt = DateTime.UtcNow
        };

        var doc2 = new GameSearchDocument
        {
            Id = 20,
            Title = "Dark Souls III",
            Description = "Challenging RPG",
            Price = 149.99m,
            Currency = "BRL",
            Genres = ["RPG"],
            Platforms = ["PC"],
            Status = "Published",
            AverageRating = 4.5m,
            TotalReviews = 2000,
            IndexedAt = DateTime.UtcNow
        };

        _searchRepository.GetRecommendationsAsync(
            Arg.Any<IEnumerable<int>>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns([doc1, doc2]);

        var query = new GetRecommendationsQuery([1, 2, 3], 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(10);
        result[0].Title.Should().Be("Elden Ring");
        result[0].PriceAmount.Should().Be(199.99m);
        result[0].Status.Should().Be(GameStatus.Published);
        result[0].Genres.Should().HaveCount(2);
        result[1].Id.Should().Be(20);
        result[1].Title.Should().Be("Dark Souls III");
    }

    [Fact]
    public async Task Handle_EmptyResults_ShouldReturnEmptyList()
    {
        // Arrange
        _searchRepository.GetRecommendationsAsync(
            Arg.Any<IEnumerable<int>>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetRecommendationsQuery([1], 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectParametersToRepository()
    {
        // Arrange
        _searchRepository.GetRecommendationsAsync(
            Arg.Any<IEnumerable<int>>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns([]);

        var gameIds = new List<int> { 5, 10, 15 };
        var query = new GetRecommendationsQuery(gameIds, 5);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _searchRepository.Received(1).GetRecommendationsAsync(
            Arg.Is<IReadOnlyCollection<int>>(ids =>
                ids.Count == 3 &&
                ids.Contains(5) &&
                ids.Contains(10) &&
                ids.Contains(15)),
            5,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var doc = new GameSearchDocument
        {
            Id = 1,
            Title = "The Witcher 3",
            Description = "Open world RPG",
            Price = 99.90m,
            Currency = "BRL",
            Genres = ["RPG", "Adventure"],
            Platforms = ["PC", "PlayStation5", "XboxSeriesX"],
            Status = "Published",
            AverageRating = 4.9m,
            TotalReviews = 5000,
            ReleaseDate = new DateTime(2015, 5, 19),
            Publisher = "CD Projekt",
            Developer = "CD Projekt Red",
            CoverImageUrl = "https://example.com/witcher3.jpg",
            IndexedAt = DateTime.UtcNow
        };

        _searchRepository.GetRecommendationsAsync(
            Arg.Any<IEnumerable<int>>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns([doc]);

        var query = new GetRecommendationsQuery([1], 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var viewModel = result[0];
        viewModel.Id.Should().Be(1);
        viewModel.Title.Should().Be("The Witcher 3");
        viewModel.Description.Should().Be("Open world RPG");
        viewModel.PriceAmount.Should().Be(99.90m);
        viewModel.PriceCurrency.Should().Be("BRL");
        viewModel.Status.Should().Be(GameStatus.Published);
        viewModel.AverageRating.Should().Be(4.9m);
        viewModel.TotalReviews.Should().Be(5000);
        viewModel.ReleaseDate.Should().Be(new DateTime(2015, 5, 19));
        viewModel.Publisher.Should().Be("CD Projekt");
        viewModel.Developer.Should().Be("CD Projekt Red");
        viewModel.CoverImageUrl.Should().Be("https://example.com/witcher3.jpg");
        viewModel.Genres.Should().Contain(Genre.RPG);
        viewModel.Genres.Should().Contain(Genre.Adventure);
        viewModel.Platforms.Should().HaveCount(3);
    }
}
