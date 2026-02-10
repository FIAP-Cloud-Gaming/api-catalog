namespace Catalog.UnitTests.Domain.ValueObjects;

public sealed class PriceTests
{
    [Fact]
    public void Constructor_WithValidAmount_ShouldCreate()
    {
        // Arrange & Act
        var price = new Price(59.99m, "BRL");

        // Assert
        price.Amount.Should().Be(59.99m);
        price.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowInvalidPriceException()
    {
        // Arrange
        var negativeAmount = -10.50m;

        // Act
        var act = () => new Price(negativeAmount, "BRL");

        // Assert
        act.Should().Throw<InvalidPriceException>()
            .WithMessage($"Price '{negativeAmount}' is invalid. Price must be greater than or equal to zero.");
    }

    [Fact]
    public void Constructor_WithZeroAmount_ShouldCreate()
    {
        // Arrange & Act
        var price = new Price(0m, "BRL");

        // Assert
        price.Amount.Should().Be(0m);
        price.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Constructor_DefaultCurrency_ShouldBeBRL()
    {
        // Arrange & Act
        var price = new Price(59.99m);

        // Assert
        price.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Free_ShouldReturnZeroPriceBRL()
    {
        // Arrange & Act
        var freePrice = Price.Free();

        // Assert
        freePrice.Amount.Should().Be(0m);
        freePrice.Currency.Should().Be("BRL");
    }

    [Fact]
    public void ApplyDiscount_ValidPercentage_ShouldApplyCorrectly()
    {
        // Arrange
        var price = new Price(100m, "BRL");

        // Act
        var discountedPrice = price.ApplyDiscount(20m);

        // Assert
        discountedPrice.Amount.Should().Be(80m);
        discountedPrice.Currency.Should().Be("BRL");
    }

    [Fact]
    public void ApplyDiscount_NegativePercentage_ShouldThrow()
    {
        // Arrange
        var price = new Price(100m, "BRL");

        // Act
        var act = () => price.ApplyDiscount(-10m);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Discount percentage must be between 0 and 100.");
    }

    [Fact]
    public void ApplyDiscount_OverHundredPercentage_ShouldThrow()
    {
        // Arrange
        var price = new Price(100m, "BRL");

        // Act
        var act = () => price.ApplyDiscount(101m);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Discount percentage must be between 0 and 100.");
    }

    [Fact]
    public void ApplyDiscount_ShouldRoundToTwoDecimals()
    {
        // Arrange
        var price = new Price(99.99m, "BRL");

        // Act
        var discountedPrice = price.ApplyDiscount(33.33m);

        // Assert
        discountedPrice.Amount.Should().Be(66.66m);
    }

    [Fact]
    public void Equality_SameAmountAndCurrency_ShouldBeEqual()
    {
        // Arrange
        var price1 = new Price(59.99m, "BRL");
        var price2 = new Price(59.99m, "BRL");

        // Act & Assert
        price1.Should().Be(price2);
        (price1 == price2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var price = new Price(59.99m, "USD");

        // Act
        var result = price.ToString();

        // Assert
        result.Should().Be("59.99 USD");
    }

    [Fact]
    public void Constructor_WithNullCurrency_ShouldDefaultToBRL()
    {
        // Arrange & Act
        var price = new Price(59.99m, null!);

        // Assert
        price.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Equality_DifferentAmounts_ShouldNotBeEqual()
    {
        // Arrange
        var price1 = new Price(59.99m, "BRL");
        var price2 = new Price(49.99m, "BRL");

        // Act & Assert
        price1.Should().NotBe(price2);
        (price1 != price2).Should().BeTrue();
        price1.Equals(price2).Should().BeFalse();
    }

    [Fact]
    public void Equality_DifferentCurrencies_ShouldNotBeEqual()
    {
        // Arrange
        var price1 = new Price(59.99m, "BRL");
        var price2 = new Price(59.99m, "USD");

        // Act & Assert
        price1.Should().NotBe(price2);
        (price1 != price2).Should().BeTrue();
    }

    [Fact]
    public void ApplyDiscount_ZeroPercent_ShouldReturnSameAmount()
    {
        // Arrange
        var price = new Price(100m, "BRL");

        // Act
        var discountedPrice = price.ApplyDiscount(0m);

        // Assert
        discountedPrice.Amount.Should().Be(100m);
        discountedPrice.Currency.Should().Be("BRL");
    }

    [Fact]
    public void ApplyDiscount_HundredPercent_ShouldReturnZero()
    {
        // Arrange
        var price = new Price(100m, "BRL");

        // Act
        var discountedPrice = price.ApplyDiscount(100m);

        // Assert
        discountedPrice.Amount.Should().Be(0m);
        discountedPrice.Currency.Should().Be("BRL");
    }

    [Fact]
    public void GetHashCode_SameAmountAndCurrency_ShouldBeEqual()
    {
        // Arrange
        var price1 = new Price(59.99m, "BRL");
        var price2 = new Price(59.99m, "BRL");

        // Act & Assert
        price1.GetHashCode().Should().Be(price2.GetHashCode());
    }
}
