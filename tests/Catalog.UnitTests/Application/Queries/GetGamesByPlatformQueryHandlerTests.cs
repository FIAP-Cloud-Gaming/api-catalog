using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Queries;

public sealed class GetGamesByPlatformQueryHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly GetGamesByPlatformQueryHandler _handler;

    public GetGamesByPlatformQueryHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _handler = new GetGamesByPlatformQueryHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_GamesExist_ShouldReturnViewModels()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGame(title: "Halo Infinite", platforms: [Platform.XboxSeriesX]);
        var game2 = GameTestFactory.CreateValidGame(title: "Forza Horizon 5", platforms: [Platform.XboxSeriesX]);
        GameTestFactory.SetGameId(game1, 1);
        GameTestFactory.SetGameId(game2, 2);

        _gameRepository.GetByPlatformAsync(Platform.XboxSeriesX, Arg.Any<CancellationToken>())
            .Returns([game1, game2]);

        var query = new GetGamesByPlatformQuery(Platform.XboxSeriesX);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Halo Infinite");
        result[1].Title.Should().Be("Forza Horizon 5");
    }

    [Fact]
    public async Task Handle_NoGames_ShouldReturnEmpty()
    {
        // Arrange
        _gameRepository.GetByPlatformAsync(Platform.NintendoSwitch, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetGamesByPlatformQuery(Platform.NintendoSwitch);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallGetByPlatformAsync()
    {
        // Arrange
        _gameRepository.GetByPlatformAsync(Platform.PC, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetGamesByPlatformQuery(Platform.PC);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByPlatformAsync(Platform.PC, Arg.Any<CancellationToken>());
    }
}
