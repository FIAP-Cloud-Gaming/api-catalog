using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Validations;

public sealed class SearchGamesQueryValidatorTests
{
    private readonly SearchGamesQueryValidator _validator;

    public SearchGamesQueryValidatorTests()
    {
        _validator = new SearchGamesQueryValidator();
    }

    [Fact]
    public async Task Validate_ValidQuery_ShouldPass()
    {
        // Arrange
        var query = new SearchGamesQuery(
            SearchTerm: "Action",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            MinPrice: 10.00m,
            MaxPrice: 100.00m,
            MinRating: 4.0m,
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_PageNumberZero_ShouldFail()
    {
        // Arrange
        var query = new SearchGamesQuery(
            SearchTerm: null,
            Genres: null,
            Platforms: null,
            MinPrice: null,
            MaxPrice: null,
            MinRating: null,
            PageNumber: 0,
            PageSize: 20
        );

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchGamesQuery.PageNumber));
    }

    [Fact]
    public async Task Validate_PageSizeZero_ShouldFail()
    {
        // Arrange
        var query = new SearchGamesQuery(
            SearchTerm: null,
            Genres: null,
            Platforms: null,
            MinPrice: null,
            MaxPrice: null,
            MinRating: null,
            PageNumber: 1,
            PageSize: 0
        );

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchGamesQuery.PageSize));
    }

    [Fact]
    public async Task Validate_PageSizeOver100_ShouldFail()
    {
        // Arrange
        var query = new SearchGamesQuery(
            SearchTerm: null,
            Genres: null,
            Platforms: null,
            MinPrice: null,
            MaxPrice: null,
            MinRating: null,
            PageNumber: 1,
            PageSize: 101
        );

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchGamesQuery.PageSize));
    }

    [Fact]
    public async Task Validate_NegativeMinPrice_ShouldFail()
    {
        // Arrange
        var query = new SearchGamesQuery(
            SearchTerm: null,
            Genres: null,
            Platforms: null,
            MinPrice: -10.00m,
            MaxPrice: null,
            MinRating: null,
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchGamesQuery.MinPrice));
    }

    [Fact]
    public async Task Validate_NegativeMaxPrice_ShouldFail()
    {
        // Arrange
        var query = new SearchGamesQuery(
            SearchTerm: null,
            Genres: null,
            Platforms: null,
            MinPrice: null,
            MaxPrice: -10.00m,
            MinRating: null,
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchGamesQuery.MaxPrice));
    }

    [Fact]
    public async Task Validate_MaxPriceLessThanMinPrice_ShouldFail()
    {
        // Arrange
        var query = new SearchGamesQuery(
            SearchTerm: null,
            Genres: null,
            Platforms: null,
            MinPrice: 100.00m,
            MaxPrice: 50.00m,
            MinRating: null,
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchGamesQuery.MaxPrice));
    }

    [Fact]
    public async Task Validate_MinRatingOutOfRange_ShouldFail()
    {
        // Arrange
        var query = new SearchGamesQuery(
            SearchTerm: null,
            Genres: null,
            Platforms: null,
            MinPrice: null,
            MaxPrice: null,
            MinRating: 6.0m,
            PageNumber: 1,
            PageSize: 20
        );

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SearchGamesQuery.MinRating));
    }
}
