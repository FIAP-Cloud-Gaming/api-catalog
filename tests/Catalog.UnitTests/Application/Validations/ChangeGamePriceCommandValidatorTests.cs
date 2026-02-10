using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Validations;

public sealed class ChangeGamePriceCommandValidatorTests
{
    private readonly ChangeGamePriceCommandValidator _validator;

    public ChangeGamePriceCommandValidatorTests()
    {
        _validator = new ChangeGamePriceCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new ChangeGamePriceCommand(
            GameId: 1,
            Amount: 49.99m,
            Currency: "BRL"
        );

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
        var command = new ChangeGamePriceCommand(
            GameId: 0,
            Amount: 49.99m,
            Currency: "BRL"
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ChangeGamePriceCommand.GameId));
    }

    [Fact]
    public async Task Validate_NegativeAmount_ShouldFail()
    {
        // Arrange
        var command = new ChangeGamePriceCommand(
            GameId: 1,
            Amount: -10.00m,
            Currency: "BRL"
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ChangeGamePriceCommand.Amount));
    }

    [Fact]
    public async Task Validate_EmptyCurrency_ShouldFail()
    {
        // Arrange
        var command = new ChangeGamePriceCommand(
            GameId: 1,
            Amount: 49.99m,
            Currency: string.Empty
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ChangeGamePriceCommand.Currency));
    }

    [Fact]
    public async Task Validate_InvalidCurrencyLength_ShouldFail()
    {
        // Arrange
        var command = new ChangeGamePriceCommand(
            GameId: 1,
            Amount: 49.99m,
            Currency: "USDX"
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ChangeGamePriceCommand.Currency));
    }
}
