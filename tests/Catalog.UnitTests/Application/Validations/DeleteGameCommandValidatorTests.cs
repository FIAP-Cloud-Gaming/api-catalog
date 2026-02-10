using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Validations;

public sealed class DeleteGameCommandValidatorTests
{
    private readonly DeleteGameCommandValidator _validator;

    public DeleteGameCommandValidatorTests()
    {
        _validator = new DeleteGameCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new DeleteGameCommand(GameId: 1);

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_GameIdZero_ShouldFail()
    {
        // Arrange
        var command = new DeleteGameCommand(GameId: 0);

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteGameCommand.GameId));
    }

    [Fact]
    public async Task Validate_GameIdNegative_ShouldFail()
    {
        // Arrange
        var command = new DeleteGameCommand(GameId: -1);

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteGameCommand.GameId));
    }
}
