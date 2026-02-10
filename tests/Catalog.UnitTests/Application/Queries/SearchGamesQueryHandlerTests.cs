using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Queries;

public sealed class SearchGamesQueryHandlerTests
{
    private readonly IGameSearchRepository _searchRepository;
    private readonly SearchGamesQueryHandler _handler;

    public SearchGamesQueryHandlerTests()
    {
        _searchRepository = Substitute.For<IGameSearchRepository>();
        _handler = new SearchGamesQueryHandler(_searchRepository);
    }

    [Fact]
    public async Task Handle_WithResults_ShouldReturnPagedResponse()
    {
        // Arrange
        var searchDoc1 = new GameSearchDocument
        {
            Id = 1,
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

        var searchDoc2 = new GameSearchDocument
        {
            Id = 2,
            Title = "Dark Souls III",
            Description = "Challenging RPG",
            Price = 149.99m,
            Currency = "BRL",
            Genres = ["RPG", "Action"],
            Platforms = ["PC"],
            Status = "Published",
            AverageRating = 4.5m,
            TotalReviews = 2000,
            ReleaseDate = new DateTime(2016, 4, 12),
            Publisher = "Bandai Namco",
            Developer = "FromSoftware",
            CoverImageUrl = "https://example.com/ds3.jpg",
            IndexedAt = DateTime.UtcNow
        };

        _searchRepository.SearchAsync(
            Arg.Any<string>(),
            Arg.Any<List<Genre>>(),
            Arg.Any<List<Platform>>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns(([searchDoc1, searchDoc2], 2L));

        var query = new SearchGamesQuery(
            "souls",
            [Genre.RPG],
            [Platform.PC],
            0m,
            300m,
            4.0m,
            1,
            10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalRecords.Should().Be(2);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_EmptyResults_ShouldReturnEmptyPagedResponse()
    {
        // Arrange
        _searchRepository.SearchAsync(
            Arg.Any<string>(),
            Arg.Any<List<Genre>>(),
            Arg.Any<List<Platform>>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns((Array.Empty<GameSearchDocument>(), 0L));

        var query = new SearchGamesQuery(
            "nonexistent game",
            null,
            null,
            null,
            null,
            null,
            1,
            10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectParametersToSearch()
    {
        // Arrange
        _searchRepository.SearchAsync(
            Arg.Any<string>(),
            Arg.Any<List<Genre>>(),
            Arg.Any<List<Platform>>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns((Array.Empty<GameSearchDocument>(), 0L));

        var query = new SearchGamesQuery(
            "cyberpunk",
            [Genre.RPG],
            [Platform.PC, Platform.PlayStation5],
            50m,
            200m,
            4.5m,
            2,
            20);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _searchRepository.Received(1).SearchAsync(
            "cyberpunk",
            Arg.Is<List<Genre>>(g => g.Contains(Genre.RPG)),
            Arg.Is<List<Platform>>(p => p.Contains(Platform.PC) && p.Contains(Platform.PlayStation5)),
            50m,
            200m,
            4.5m,
            1,
            20,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldMapSearchDocumentsToViewModels()
    {
        // Arrange
        var searchDoc = new GameSearchDocument
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

        _searchRepository.SearchAsync(
            Arg.Any<string>(),
            Arg.Any<List<Genre>>(),
            Arg.Any<List<Platform>>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<decimal?>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns(([searchDoc], 1L));

        var query = new SearchGamesQuery("witcher", null, null, null, null, null, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var viewModel = result.Data.First();
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
        viewModel.Genres.Should().HaveCount(2);
        viewModel.Platforms.Should().HaveCount(3);
    }

    [Fact]
    public void FromRequest_ValidRequest_ReturnsQuery()
    {
        // Arrange
        var request = new SearchGamesRequest
        {
            SearchTerm = "cyberpunk",
            Genres = "RPG,Action",
            Platforms = "PC,PlayStation5",
            MinPrice = 50m,
            MaxPrice = 200m,
            MinRating = 4.5m,
            PageNumber = 2,
            PageSize = 20
        };

        // Act
        var query = SearchGamesQuery.FromRequest(request);

        // Assert
        query.Should().NotBeNull();
        query.SearchTerm.Should().Be("cyberpunk");
        query.Genres.Should().HaveCount(2);
        query.Genres.Should().Contain(Genre.RPG);
        query.Genres.Should().Contain(Genre.Action);
        query.Platforms.Should().HaveCount(2);
        query.Platforms.Should().Contain(Platform.PC);
        query.Platforms.Should().Contain(Platform.PlayStation5);
        query.MinPrice.Should().Be(50m);
        query.MaxPrice.Should().Be(200m);
        query.MinRating.Should().Be(4.5m);
        query.PageNumber.Should().Be(2);
        query.PageSize.Should().Be(20);
    }

    [Fact]
    public void FromRequest_NullRequest_ThrowsArgumentNullException()
    {
        // Act
        var act = () => SearchGamesQuery.FromRequest(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromRequest_WithGenres_ParsesCommaSeparated()
    {
        // Arrange
        var request = new SearchGamesRequest
        {
            Genres = "RPG, Action, Adventure",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var query = SearchGamesQuery.FromRequest(request);

        // Assert
        query.Genres.Should().HaveCount(3);
        query.Genres.Should().Contain(Genre.RPG);
        query.Genres.Should().Contain(Genre.Action);
        query.Genres.Should().Contain(Genre.Adventure);
    }

    [Fact]
    public void FromRequest_WithPlatforms_ParsesCommaSeparated()
    {
        // Arrange
        var request = new SearchGamesRequest
        {
            Platforms = "PC, PlayStation5, XboxSeriesX",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var query = SearchGamesQuery.FromRequest(request);

        // Assert
        query.Platforms.Should().HaveCount(3);
        query.Platforms.Should().Contain(Platform.PC);
        query.Platforms.Should().Contain(Platform.PlayStation5);
        query.Platforms.Should().Contain(Platform.XboxSeriesX);
    }

    [Fact]
    public void FromRequest_NullGenresAndPlatforms_SetsNull()
    {
        // Arrange
        var request = new SearchGamesRequest
        {
            Genres = null,
            Platforms = null,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var query = SearchGamesQuery.FromRequest(request);

        // Assert
        query.Genres.Should().BeNull();
        query.Platforms.Should().BeNull();
    }
}
