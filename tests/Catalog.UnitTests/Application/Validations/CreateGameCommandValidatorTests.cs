using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Validations;

public sealed class CreateGameCommandValidatorTests
{
    private readonly CreateGameCommandValidator _validator;

    public CreateGameCommandValidatorTests()
    {
        _validator = new CreateGameCommandValidator();
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPassValidation()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: "Valid Game Title",
            Description: "Valid game description with enough content",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action, Genre.Adventure],
            Platforms: [Platform.PC, Platform.PlayStation5],
            ReleaseDate: DateTime.UtcNow.AddMonths(3),
            Publisher: "Valid Publisher",
            Developer: "Valid Developer",
            CoverImageUrl: "https://example.com/cover.jpg"
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_EmptyTitle_ShouldFail()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: string.Empty,
            Description: "Valid description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.Title));
    }

    [Fact]
    public async Task Validate_TitleTooLong_ShouldFail()
    {
        // Arrange
        var longTitle = new string('A', 201);
        var command = new CreateGameCommand(
            Title: longTitle,
            Description: "Valid description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.Title));
    }

    [Fact]
    public async Task Validate_EmptyDescription_ShouldFail()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: string.Empty,
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.Description));
    }

    [Fact]
    public async Task Validate_DescriptionTooLong_ShouldFail()
    {
        // Arrange
        var longDescription = new string('A', 2001);
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: longDescription,
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.Description));
    }

    [Fact]
    public async Task Validate_NegativePrice_ShouldFail()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: "Valid description",
            PriceAmount: -10.00m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.PriceAmount));
    }

    [Fact]
    public async Task Validate_EmptyCurrency_ShouldFail()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: "Valid description",
            PriceAmount: 59.99m,
            PriceCurrency: string.Empty,
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.PriceCurrency));
    }

    [Fact]
    public async Task Validate_InvalidCurrencyLength_ShouldFail()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: "Valid description",
            PriceAmount: 59.99m,
            PriceCurrency: "US",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.PriceCurrency));
    }

    [Fact]
    public async Task Validate_EmptyGenres_ShouldFail()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: "Valid description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.Genres));
    }

    [Fact]
    public async Task Validate_EmptyPlatforms_ShouldFail()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: "Valid description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.Platforms));
    }

    [Fact]
    public async Task Validate_PublisherTooLong_ShouldFail()
    {
        // Arrange
        var longPublisher = new string('A', 201);
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: "Valid description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: longPublisher,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateGameCommand.Publisher));
    }

    [Fact]
    public async Task Validate_ValidCommand_WithOptionalFieldsNull_ShouldPass()
    {
        // Arrange
        var command = new CreateGameCommand(
            Title: "Valid Title",
            Description: "Valid description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        // Act
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
