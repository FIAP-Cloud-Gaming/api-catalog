using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Validations;

public sealed class DeactivateGameCommandValidatorTests
{
    private readonly DeactivateGameCommandValidator _validator;

    public DeactivateGameCommandValidatorTests()
    {
        _validator = new DeactivateGameCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new DeactivateGameCommand(GameId: 1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_GameIdZero_ShouldFail()
    {
        // Arrange
        var command = new DeactivateGameCommand(GameId: 0);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeactivateGameCommand.GameId));
    }

    [Fact]
    public async Task Validate_GameIdNegative_ShouldFail()
    {
        // Arrange
        var command = new DeactivateGameCommand(GameId: -1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeactivateGameCommand.GameId));
    }
}
