namespace Catalog.UnitTests.Domain.Exceptions;

public sealed class DomainExceptionsTests
{
    [Fact]
    public void CatalogDomainException_ShouldSetMessage()
    {
        // Arrange
        var expectedMessage = "This is a domain exception message";

        // Act
        var exception = new CatalogDomainException(expectedMessage);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(expectedMessage);
        exception.Should().BeOfType<CatalogDomainException>();
    }

    [Fact]
    public void InvalidGameTitleException_ShouldContainTitle()
    {
        // Arrange
        var invalidTitle = "InvalidTitle";

        // Act
        var exception = new InvalidGameTitleException(invalidTitle);

        // Assert
        exception.Should().BeOfType<InvalidGameTitleException>();
        exception.Message.Should().Contain(invalidTitle);
        exception.Message.Should().Contain("is invalid");
        exception.Message.Should().Contain("Title cannot be empty and must be less than 200 characters");
    }

    [Fact]
    public void InvalidPriceException_ShouldContainAmount()
    {
        // Arrange
        var invalidPrice = -10.50m;

        // Act
        var exception = new InvalidPriceException(invalidPrice);

        // Assert
        exception.Should().BeOfType<InvalidPriceException>();
        exception.Message.Should().Contain(invalidPrice.ToString());
        exception.Message.Should().Contain("is invalid");
        exception.Message.Should().Contain("Price must be greater than or equal to zero");
    }

    [Fact]
    public void InvalidRatingException_ShouldContainValue()
    {
        // Arrange
        var invalidRating = 10;

        // Act
        var exception = new InvalidRatingException(invalidRating);

        // Assert
        exception.Should().BeOfType<InvalidRatingException>();
        exception.Message.Should().Contain(invalidRating.ToString());
        exception.Message.Should().Contain("is invalid");
        exception.Message.Should().Contain("Rating must be between 1 and 5");
    }

    [Fact]
    public void CatalogDomainException_ParameterlessConstructor_ShouldCreateException()
    {
        // Arrange & Act
        var exception = new CatalogDomainException();

        // Assert
        exception.Should().NotBeNull();
        exception.Should().BeOfType<CatalogDomainException>();
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CatalogDomainException_WithInnerException_ShouldSetInnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner exception message");
        var message = "Outer exception message";

        // Act
        var exception = new CatalogDomainException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException.Message.Should().Be("Inner exception message");
    }
}
