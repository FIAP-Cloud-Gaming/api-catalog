using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Commands;

public sealed class ActivateGameCommandHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ActivateGameCommandHandler _handler;

    public ActivateGameCommandHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _gameRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);

        _handler = new ActivateGameCommandHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldPublishGame()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game");
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new ActivateGameCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        game.Status.Should().Be(GameStatus.Published);
        result.Should().NotBeNull();
        result.Status.Should().Be(GameStatus.Published);
    }

    [Fact]
    public async Task Handle_GameNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _gameRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Game?)null);

        var command = new ActivateGameCommand(999);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Game with ID 999 not found.");
    }

    [Fact]
    public async Task Handle_ShouldCallUpdateAndSave()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game");
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new ActivateGameCommand(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _gameRepository.Received(1).Update(game);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullCommand_ShouldThrowArgumentNullException()
    {
        // Act
        var act = async () => await _handler.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
