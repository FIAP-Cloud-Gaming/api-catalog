namespace Catalog.UnitTests.Domain.ValueObjects;

public sealed class RatingTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Constructor_WithValidRating_ShouldCreate(int validRating)
    {
        // Arrange & Act
        var rating = new Rating(validRating);

        // Assert
        rating.Value.Should().Be(validRating);
    }

    [Fact]
    public void Constructor_WithZero_ShouldThrow()
    {
        // Arrange
        var invalidRating = 0;

        // Act
        var act = () => new Rating(invalidRating);

        // Assert
        act.Should().Throw<InvalidRatingException>()
            .WithMessage($"Rating '{invalidRating}' is invalid. Rating must be between 1 and 5.");
    }

    [Fact]
    public void Constructor_WithSix_ShouldThrow()
    {
        // Arrange
        var invalidRating = 6;

        // Act
        var act = () => new Rating(invalidRating);

        // Assert
        act.Should().Throw<InvalidRatingException>()
            .WithMessage($"Rating '{invalidRating}' is invalid. Rating must be between 1 and 5.");
    }

    [Fact]
    public void Constructor_WithNegative_ShouldThrow()
    {
        // Arrange
        var invalidRating = -1;

        // Act
        var act = () => new Rating(invalidRating);

        // Assert
        act.Should().Throw<InvalidRatingException>()
            .WithMessage($"Rating '{invalidRating}' is invalid. Rating must be between 1 and 5.");
    }

    [Fact]
    public void ToString_ShouldFormat()
    {
        // Arrange
        var rating = new Rating(4);

        // Act
        var result = rating.ToString();

        // Assert
        result.Should().Be("4/5");
    }

    [Fact]
    public void ImplicitIntConversion_ShouldWork()
    {
        // Arrange
        var rating = new Rating(5);

        // Act
        int ratingValue = rating;

        // Assert
        ratingValue.Should().Be(5);
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        // Arrange
        var rating1 = new Rating(4);
        var rating2 = new Rating(4);

        // Act & Assert
        rating1.Should().Be(rating2);
        (rating1 == rating2).Should().BeTrue();
        rating1.Equals(rating2).Should().BeTrue();
        rating1.GetHashCode().Should().Be(rating2.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var rating1 = new Rating(4);
        var rating2 = new Rating(5);

        // Act & Assert
        rating1.Should().NotBe(rating2);
        (rating1 != rating2).Should().BeTrue();
        rating1.Equals(rating2).Should().BeFalse();
    }
}
