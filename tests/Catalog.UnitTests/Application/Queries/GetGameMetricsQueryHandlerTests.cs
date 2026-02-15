using Catalog.API.Application.Queries.GetGameMetrics;

namespace Catalog.UnitTests.Application.Queries;

public sealed class GetGameMetricsQueryHandlerTests
{
    private readonly IGameSearchRepository _searchRepository;
    private readonly GetGameMetricsQueryHandler _handler;

    public GetGameMetricsQueryHandlerTests()
    {
        _searchRepository = Substitute.For<IGameSearchRepository>();
        _handler = new GetGameMetricsQueryHandler(_searchRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnMetricsFromRepository()
    {
        // Arrange
        var expectedMetrics = new GameMetrics
        {
            TopGenres = [new MetricBucket("Action", 45), new MetricBucket("RPG", 32)],
            TopPlatforms = [new MetricBucket("PC", 120), new MetricBucket("PlayStation5", 80)],
            AveragePrice = 39.99m,
            PriceRanges =
            [
                new PriceRangeBucket("*-20.0", null, 20.0, 15),
                new PriceRangeBucket("20.0-50.0", 20.0, 50.0, 45),
                new PriceRangeBucket("50.0-100.0", 50.0, 100.0, 30),
                new PriceRangeBucket("100.0-*", 100.0, null, 10)
            ],
            MostReviewed = [new MetricBucket("Elden Ring", 500)]
        };

        _searchRepository.GetGameMetricsAsync(Arg.Any<CancellationToken>())
            .Returns(expectedMetrics);

        var query = new GetGameMetricsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TopGenres.Should().HaveCount(2);
        result.TopGenres[0].Key.Should().Be("Action");
        result.TopGenres[0].DocCount.Should().Be(45);
        result.TopPlatforms.Should().HaveCount(2);
        result.AveragePrice.Should().Be(39.99m);
        result.PriceRanges.Should().HaveCount(4);
        result.MostReviewed.Should().HaveCount(1);
        result.MostReviewed[0].Key.Should().Be("Elden Ring");
    }

    [Fact]
    public async Task Handle_EmptyMetrics_ShouldReturnDefaultMetrics()
    {
        // Arrange
        var emptyMetrics = new GameMetrics();

        _searchRepository.GetGameMetricsAsync(Arg.Any<CancellationToken>())
            .Returns(emptyMetrics);

        var query = new GetGameMetricsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TopGenres.Should().BeEmpty();
        result.TopPlatforms.Should().BeEmpty();
        result.AveragePrice.Should().Be(0);
        result.PriceRanges.Should().BeEmpty();
        result.MostReviewed.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryOnce()
    {
        // Arrange
        _searchRepository.GetGameMetricsAsync(Arg.Any<CancellationToken>())
            .Returns(new GameMetrics());

        var query = new GetGameMetricsQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _searchRepository.Received(1).GetGameMetricsAsync(Arg.Any<CancellationToken>());
    }
}
