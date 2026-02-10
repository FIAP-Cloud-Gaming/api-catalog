using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Commands;

public sealed class CreateGameCommandHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateGameCommandHandler _handler;

    public CreateGameCommandHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _gameRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);

        _handler = new CreateGameCommandHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnGameViewModel()
    {
        // Arrange
        var command = new CreateGameCommand(
            "Elden Ring",
            "An epic action RPG",
            199.99m,
            "BRL",
            [Genre.RPG, Genre.Action],
            [Platform.PC, Platform.PlayStation5],
            new DateTime(2022, 2, 25),
            "Bandai Namco",
            "FromSoftware",
            "https://example.com/elden-ring.jpg");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Elden Ring");
        result.Description.Should().Be("An epic action RPG");
        result.PriceAmount.Should().Be(199.99m);
        result.PriceCurrency.Should().Be("BRL");
        result.Genres.Should().HaveCount(2);
        result.Platforms.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallAddAndSave()
    {
        // Arrange
        var command = new CreateGameCommand(
            "Test Game",
            "Description",
            59.99m,
            "BRL",
            [Genre.Action],
            [Platform.PC],
            null,
            null,
            null,
            null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _gameRepository.Received(1).Add(Arg.Any<Game>());
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectGameProperties()
    {
        // Arrange
        var command = new CreateGameCommand(
            "God of War Ragnarök",
            "The sequel to the acclaimed God of War",
            249.90m,
            "BRL",
            [Genre.Action, Genre.Adventure],
            [Platform.PlayStation5],
            new DateTime(2022, 11, 9),
            "Sony Interactive Entertainment",
            "Santa Monica Studio",
            "https://example.com/gow-ragnarok.jpg");

        Game? capturedGame = null;
        _gameRepository.Add(Arg.Do<Game>(game => capturedGame = game));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedGame.Should().NotBeNull();
        capturedGame!.Title.Value.Should().Be("God of War Ragnarök");
        capturedGame.Description.Should().Be("The sequel to the acclaimed God of War");
        capturedGame.Price.Amount.Should().Be(249.90m);
        capturedGame.Price.Currency.Should().Be("BRL");
        capturedGame.Status.Should().Be(GameStatus.Draft);
        capturedGame.Publisher.Should().Be("Sony Interactive Entertainment");
        capturedGame.Developer.Should().Be("Santa Monica Studio");
        capturedGame.CoverImageUrl.Should().Be("https://example.com/gow-ragnarok.jpg");
        capturedGame.ReleaseDate.Should().Be(new DateTime(2022, 11, 9));
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
    public async Task Handle_WithOptionalFields_ShouldCreateGame()
    {
        // Arrange
        var releaseDate = new DateTime(2023, 6, 15);
        var command = new CreateGameCommand(
            "Cyberpunk 2077",
            "A futuristic RPG",
            149.90m,
            "BRL",
            [Genre.RPG],
            [Platform.PC],
            releaseDate,
            "CD Projekt",
            "CD Projekt Red",
            "https://example.com/cyberpunk.jpg");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ReleaseDate.Should().Be(releaseDate);
        result.Publisher.Should().Be("CD Projekt");
        result.Developer.Should().Be("CD Projekt Red");
        result.CoverImageUrl.Should().Be("https://example.com/cyberpunk.jpg");
    }

    [Fact]
    public void FromRequest_ValidRequest_ReturnsCommand()
    {
        // Arrange
        var request = new CreateGameRequest
        {
            Title = "Elden Ring",
            Description = "An epic action RPG",
            PriceAmount = 199.99m,
            PriceCurrency = "BRL",
            Genres = [Genre.RPG, Genre.Action],
            Platforms = [Platform.PC, Platform.PlayStation5],
            ReleaseDate = new DateTime(2022, 2, 25),
            Publisher = "Bandai Namco",
            Developer = "FromSoftware",
            CoverImageUrl = "https://example.com/elden-ring.jpg"
        };

        // Act
        var command = CreateGameCommand.FromRequest(request);

        // Assert
        command.Should().NotBeNull();
        command.Title.Should().Be("Elden Ring");
        command.Description.Should().Be("An epic action RPG");
        command.PriceAmount.Should().Be(199.99m);
        command.PriceCurrency.Should().Be("BRL");
        command.Genres.Should().HaveCount(2);
        command.Genres.Should().Contain(Genre.RPG);
        command.Genres.Should().Contain(Genre.Action);
        command.Platforms.Should().HaveCount(2);
        command.Platforms.Should().Contain(Platform.PC);
        command.Platforms.Should().Contain(Platform.PlayStation5);
        command.ReleaseDate.Should().Be(new DateTime(2022, 2, 25));
        command.Publisher.Should().Be("Bandai Namco");
        command.Developer.Should().Be("FromSoftware");
        command.CoverImageUrl.Should().Be("https://example.com/elden-ring.jpg");
    }

    [Fact]
    public void FromRequest_NullRequest_ThrowsArgumentNullException()
    {
        // Act
        var act = () => CreateGameCommand.FromRequest(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
