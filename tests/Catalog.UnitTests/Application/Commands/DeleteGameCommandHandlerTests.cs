using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Commands;

public sealed class DeleteGameCommandHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteGameCommandHandler _handler;

    public DeleteGameCommandHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _gameRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);

        _handler = new DeleteGameCommandHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_GameExists_ShouldReturnTrue()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game");
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new DeleteGameCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_GameExists_ShouldCallRemoveAndSave()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game");
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new DeleteGameCommand(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _gameRepository.Received(1).Remove(game);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_GameNotExists_ShouldReturnFalse()
    {
        // Arrange
        _gameRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Game?)null);

        var command = new DeleteGameCommand(999);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _gameRepository.DidNotReceive().Remove(Arg.Any<Game>());
        await _unitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
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
