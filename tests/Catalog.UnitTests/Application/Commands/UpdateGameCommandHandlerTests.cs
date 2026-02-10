using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Commands;

public sealed class UpdateGameCommandHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateGameCommandHandler _handler;

    public UpdateGameCommandHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _gameRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);

        _handler = new UpdateGameCommandHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateAndReturnViewModel()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Original Title", 99.99m);
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new UpdateGameCommand(
            1,
            "Updated description",
            new DateTime(2024, 1, 1),
            "Updated Publisher",
            "Updated Developer",
            "https://example.com/new-cover.jpg",
            null,
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be("Updated description");
        result.Publisher.Should().Be("Updated Publisher");
        result.Developer.Should().Be("Updated Developer");
        result.CoverImageUrl.Should().Be("https://example.com/new-cover.jpg");
        result.ReleaseDate.Should().Be(new DateTime(2024, 1, 1));
    }

    [Fact]
    public async Task Handle_GameNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _gameRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Game?)null);

        var command = new UpdateGameCommand(999, "New Description", null, null, null, null, null, null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Game with ID 999 not found.");
    }

    [Fact]
    public async Task Handle_UpdateDescription_ShouldUpdateGame()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game");
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new UpdateGameCommand(
            1,
            "This is the new description",
            null,
            null,
            null,
            null,
            null,
            null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        game.Description.Should().Be("This is the new description");
        _gameRepository.Received(1).Update(game);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UpdateGenres_ShouldReplaceGenres()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(
            genres: [Genre.Action, Genre.Adventure]);
        GameTestFactory.SetGameId(game, 1);

        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new UpdateGameCommand(
            1,
            null,
            null,
            null,
            null,
            null,
            [Genre.RPG, Genre.Strategy],
            null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        game.Genres.Should().HaveCount(2);
        game.Genres.Should().Contain(Genre.RPG);
        game.Genres.Should().Contain(Genre.Strategy);
        game.Genres.Should().NotContain(Genre.Action);
        game.Genres.Should().NotContain(Genre.Adventure);
    }

    [Fact]
    public async Task Handle_UpdatePlatforms_ShouldReplacePlatforms()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(
            platforms: [Platform.PC, Platform.PlayStation5]);
        GameTestFactory.SetGameId(game, 1);

        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new UpdateGameCommand(
            1,
            null,
            null,
            null,
            null,
            null,
            null,
            [Platform.XboxSeriesX, Platform.NintendoSwitch]);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        game.Platforms.Should().HaveCount(2);
        game.Platforms.Should().Contain(Platform.XboxSeriesX);
        game.Platforms.Should().Contain(Platform.NintendoSwitch);
        game.Platforms.Should().NotContain(Platform.PC);
        game.Platforms.Should().NotContain(Platform.PlayStation5);
    }

    [Fact]
    public async Task Handle_NullCommand_ShouldThrowArgumentNullException()
    {
        // Act
        var act = async () => await _handler.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void FromRequest_ValidRequest_ReturnsCommand()
    {
        // Arrange
        var gameId = 1;
        var request = new UpdateGameRequest
        {
            Description = "Updated description",
            ReleaseDate = new DateTime(2024, 1, 1),
            Publisher = "Updated Publisher",
            Developer = "Updated Developer",
            CoverImageUrl = "https://example.com/new-cover.jpg",
            Genres = [Genre.RPG, Genre.Strategy],
            Platforms = [Platform.XboxSeriesX, Platform.NintendoSwitch]
        };

        // Act
        var command = UpdateGameCommand.FromRequest(gameId, request);

        // Assert
        command.Should().NotBeNull();
        command.GameId.Should().Be(1);
        command.Description.Should().Be("Updated description");
        command.ReleaseDate.Should().Be(new DateTime(2024, 1, 1));
        command.Publisher.Should().Be("Updated Publisher");
        command.Developer.Should().Be("Updated Developer");
        command.CoverImageUrl.Should().Be("https://example.com/new-cover.jpg");
        command.Genres.Should().HaveCount(2);
        command.Genres.Should().Contain(Genre.RPG);
        command.Genres.Should().Contain(Genre.Strategy);
        command.Platforms.Should().HaveCount(2);
        command.Platforms.Should().Contain(Platform.XboxSeriesX);
        command.Platforms.Should().Contain(Platform.NintendoSwitch);
    }

    [Fact]
    public void FromRequest_NullRequest_ThrowsArgumentNullException()
    {
        // Act
        var act = () => UpdateGameCommand.FromRequest(1, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
