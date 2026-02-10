using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Commands;

public sealed class ChangeGamePriceCommandHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ChangeGamePriceCommandHandler _handler;

    public ChangeGamePriceCommandHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _gameRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);

        _handler = new ChangeGamePriceCommandHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldChangePriceAndReturnViewModel()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new ChangeGamePriceCommand(1, 79.99m, "BRL");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PriceAmount.Should().Be(79.99m);
        result.PriceCurrency.Should().Be("BRL");
        game.Price.Amount.Should().Be(79.99m);
        game.Price.Currency.Should().Be("BRL");
    }

    [Fact]
    public async Task Handle_GameNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _gameRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Game?)null);

        var command = new ChangeGamePriceCommand(999, 99.99m, "BRL");

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
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var command = new ChangeGamePriceCommand(1, 49.99m, "USD");

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

    [Fact]
    public void FromRequest_ValidRequest_ReturnsCommand()
    {
        // Arrange
        var gameId = 1;
        var request = new ChangePriceRequest
        {
            Amount = 79.99m,
            Currency = "USD"
        };

        // Act
        var command = ChangeGamePriceCommand.FromRequest(gameId, request);

        // Assert
        command.Should().NotBeNull();
        command.GameId.Should().Be(1);
        command.Amount.Should().Be(79.99m);
        command.Currency.Should().Be("USD");
    }

    [Fact]
    public void FromRequest_NullRequest_ThrowsArgumentNullException()
    {
        // Act
        var act = () => ChangeGamePriceCommand.FromRequest(1, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
