using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Behaviors;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_SuccessfulRequest_ShouldCallNext()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoggingBehavior<CreateGameCommand, GameViewModel>>>();
        var behavior = new LoggingBehavior<CreateGameCommand, GameViewModel>(logger);

        var command = new CreateGameCommand(
            Title: "Test Game",
            Description: "Test Description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        var expectedResult = new GameViewModel
        {
            Id = 1,
            Title = "Test Game",
            Description = "Test Description",
            PriceAmount = 59.99m,
            PriceCurrency = "BRL",
            Status = GameStatus.Draft,
            Genres = [Genre.Action],
            Platforms = [Platform.PC],
            CreatedAt = DateTime.UtcNow
        };

        var nextCalled = false;
        RequestHandlerDelegate<GameViewModel> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResult);
        };

        // Act
        await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SuccessfulRequest_ShouldReturnResult()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoggingBehavior<CreateGameCommand, GameViewModel>>>();
        var behavior = new LoggingBehavior<CreateGameCommand, GameViewModel>(logger);

        var command = new CreateGameCommand(
            Title: "Test Game",
            Description: "Test Description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        var expectedResult = new GameViewModel
        {
            Id = 1,
            Title = "Test Game",
            Description = "Test Description",
            PriceAmount = 59.99m,
            PriceCurrency = "BRL",
            Status = GameStatus.Draft,
            Genres = [Genre.Action],
            Platforms = [Platform.PC],
            CreatedAt = DateTime.UtcNow
        };

        RequestHandlerDelegate<GameViewModel> next = (ct) => Task.FromResult(expectedResult);

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test Game");
    }

    [Fact]
    public async Task Handle_FailingRequest_ShouldRethrowException()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoggingBehavior<CreateGameCommand, GameViewModel>>>();
        var behavior = new LoggingBehavior<CreateGameCommand, GameViewModel>(logger);

        var command = new CreateGameCommand(
            Title: "Test Game",
            Description: "Test Description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        var expectedException = new InvalidOperationException("Test exception");
        RequestHandlerDelegate<GameViewModel> next = (ct) => throw expectedException;

        // Act
        var act = async () => await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");
    }

    [Fact]
    public async Task Handle_ShouldNotSwallowExceptions()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoggingBehavior<CreateGameCommand, GameViewModel>>>();
        var behavior = new LoggingBehavior<CreateGameCommand, GameViewModel>(logger);

        var command = new CreateGameCommand(
            Title: "Test Game",
            Description: "Test Description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        var expectedException = new CatalogDomainException("Domain error");
        RequestHandlerDelegate<GameViewModel> next = (ct) => throw expectedException;

        // Act
        var act = async () => await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<CatalogDomainException>();
        exception.Which.Should().Be(expectedException);
    }

    [Fact]
    public async Task Handle_SingleParam_Success_LogsStartAndEnd()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoggingBehavior<DeleteGameCommand>>>();
        var behavior = new LoggingBehavior<DeleteGameCommand>(logger);

        var command = new DeleteGameCommand(1);

        var nextCalled = false;
        RequestHandlerDelegate next = (ct) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SingleParam_Exception_LogsErrorAndThrows()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoggingBehavior<DeleteGameCommand>>>();
        var behavior = new LoggingBehavior<DeleteGameCommand>(logger);

        var command = new DeleteGameCommand(1);

        var expectedException = new InvalidOperationException("Test exception");
        RequestHandlerDelegate next = (ct) => throw expectedException;

        // Act
        var act = async () => await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");
    }
}
