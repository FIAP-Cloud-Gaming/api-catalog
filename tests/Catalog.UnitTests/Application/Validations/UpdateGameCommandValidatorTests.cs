using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Validations;

public sealed class UpdateGameCommandValidatorTests
{
    private readonly UpdateGameCommandValidator _validator;

    public UpdateGameCommandValidatorTests()
    {
        _validator = new UpdateGameCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new UpdateGameCommand(
            GameId: 1,
            Description: "Updated description",
            ReleaseDate: DateTime.UtcNow.AddMonths(2),
            Publisher: "Updated Publisher",
            Developer: "Updated Developer",
            CoverImageUrl: "https://example.com/new-cover.jpg",
            Genres: [Genre.Action, Genre.RPG],
            Platforms: [Platform.PC, Platform.XboxSeriesX]
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
        var command = new UpdateGameCommand(
            GameId: 0,
            Description: "Updated description",
            ReleaseDate: null,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null,
            Genres: null,
            Platforms: null
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateGameCommand.GameId));
    }

    [Fact]
    public async Task Validate_GameIdNegative_ShouldFail()
    {
        // Arrange
        var command = new UpdateGameCommand(
            GameId: -1,
            Description: "Updated description",
            ReleaseDate: null,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null,
            Genres: null,
            Platforms: null
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateGameCommand.GameId));
    }

    [Fact]
    public async Task Validate_DescriptionTooLong_ShouldFail()
    {
        // Arrange
        var longDescription = new string('A', 2001);
        var command = new UpdateGameCommand(
            GameId: 1,
            Description: longDescription,
            ReleaseDate: null,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null,
            Genres: null,
            Platforms: null
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateGameCommand.Description));
    }

    [Fact]
    public async Task Validate_PublisherTooLong_ShouldFail()
    {
        // Arrange
        var longPublisher = new string('A', 201);
        var command = new UpdateGameCommand(
            GameId: 1,
            Description: null,
            ReleaseDate: null,
            Publisher: longPublisher,
            Developer: null,
            CoverImageUrl: null,
            Genres: null,
            Platforms: null
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateGameCommand.Publisher));
    }

    [Fact]
    public async Task Validate_NoPropertiesToUpdate_ShouldFail()
    {
        // Arrange
        var command = new UpdateGameCommand(
            GameId: 1,
            Description: null,
            ReleaseDate: null,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null,
            Genres: null,
            Platforms: null
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Validate_OnlyDescription_ShouldPass()
    {
        // Arrange
        var command = new UpdateGameCommand(
            GameId: 1,
            Description: "Only description updated",
            ReleaseDate: null,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null,
            Genres: null,
            Platforms: null
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_OnlyGenres_ShouldPass()
    {
        // Arrange
        var command = new UpdateGameCommand(
            GameId: 1,
            Description: null,
            ReleaseDate: null,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null,
            Genres: [Genre.Strategy, Genre.Simulation],
            Platforms: null
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
