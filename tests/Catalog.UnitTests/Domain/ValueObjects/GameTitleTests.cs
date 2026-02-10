namespace Catalog.UnitTests.Domain.ValueObjects;

public sealed class GameTitleTests
{
    [Fact]
    public void Constructor_WithValidTitle_ShouldCreate()
    {
        // Arrange
        var titleValue = "The Last of Us Part II";

        // Act
        var title = new GameTitle(titleValue);

        // Assert
        title.Value.Should().Be(titleValue);
    }

    [Fact]
    public void Constructor_WithNullTitle_ShouldThrowInvalidGameTitleException()
    {
        // Arrange
        string? nullTitle = null;

        // Act
        var act = () => new GameTitle(nullTitle!);

        // Assert
        act.Should().Throw<InvalidGameTitleException>()
            .WithMessage("Game title '*' is invalid. Title cannot be empty and must be less than 200 characters.");
    }

    [Fact]
    public void Constructor_WithEmptyTitle_ShouldThrow()
    {
        // Arrange
        var emptyTitle = "";

        // Act
        var act = () => new GameTitle(emptyTitle);

        // Assert
        act.Should().Throw<InvalidGameTitleException>();
    }

    [Fact]
    public void Constructor_WithWhitespaceTitle_ShouldThrow()
    {
        // Arrange
        var whitespaceTitle = "   ";

        // Act
        var act = () => new GameTitle(whitespaceTitle);

        // Assert
        act.Should().Throw<InvalidGameTitleException>();
    }

    [Fact]
    public void Constructor_TitleTooLong_ShouldThrow()
    {
        // Arrange
        var longTitle = new string('a', 201);

        // Act
        var act = () => new GameTitle(longTitle);

        // Assert
        act.Should().Throw<InvalidGameTitleException>()
            .WithMessage("Game title '*' is invalid. Title cannot be empty and must be less than 200 characters.");
    }

    [Fact]
    public void Constructor_ShouldTrimValue()
    {
        // Arrange
        var titleWithSpaces = "  Game Title  ";

        // Act
        var title = new GameTitle(titleWithSpaces);

        // Assert
        title.Value.Should().Be("Game Title");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var titleValue = "Cyberpunk 2077";
        var title = new GameTitle(titleValue);

        // Act
        var result = title.ToString();

        // Assert
        result.Should().Be(titleValue);
    }

    [Fact]
    public void ImplicitStringConversion_ShouldWork()
    {
        // Arrange
        var title = new GameTitle("God of War");

        // Act
        string titleString = title;

        // Assert
        titleString.Should().Be("God of War");
    }

    [Fact]
    public void Equality_SameTitles_ShouldBeEqual()
    {
        // Arrange
        var title1 = new GameTitle("The Witcher 3");
        var title2 = new GameTitle("The Witcher 3");

        // Act & Assert
        title1.Should().Be(title2);
        (title1 == title2).Should().BeTrue();
        title1.Equals(title2).Should().BeTrue();
        title1.GetHashCode().Should().Be(title2.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentTitles_ShouldNotBeEqual()
    {
        // Arrange
        var title1 = new GameTitle("The Witcher 3");
        var title2 = new GameTitle("The Witcher 2");

        // Act & Assert
        title1.Should().NotBe(title2);
        (title1 != title2).Should().BeTrue();
        title1.Equals(title2).Should().BeFalse();
    }
}
