using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<CreateGameCommand>>();
        var behavior = new ValidationBehavior<CreateGameCommand, GameViewModel>(validators);

        var command = new CreateGameCommand(
            Title: "Test Game",
            Description: "Test Description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        var expectedResult = new GameViewModel
        {
            Id = 1,
            Title = "Test Game",
            Description = "Test Description",
            PriceAmount = 59.99m,
            PriceCurrency = "BRL",
            Status = GameStatus.Draft,
            Genres = [Genre.Action],
            Platforms = [Platform.PC],
            CreatedAt = DateTime.UtcNow
        };

        var nextCalled = false;
        RequestHandlerDelegate<GameViewModel> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResult);
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCallNext()
    {
        // Arrange
        var validator = Substitute.For<IValidator<CreateGameCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var validators = new[] { validator };
        var behavior = new ValidationBehavior<CreateGameCommand, GameViewModel>(validators);

        var command = new CreateGameCommand(
            Title: "Test Game",
            Description: "Test Description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        var expectedResult = new GameViewModel
        {
            Id = 1,
            Title = "Test Game",
            Description = "Test Description",
            PriceAmount = 59.99m,
            PriceCurrency = "BRL",
            Status = GameStatus.Draft,
            Genres = [Genre.Action],
            Platforms = [Platform.PC],
            CreatedAt = DateTime.UtcNow
        };

        var nextCalled = false;
        RequestHandlerDelegate<GameViewModel> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResult);
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be(expectedResult);
        await validator.Received(1).ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var validator = Substitute.For<IValidator<CreateGameCommand>>();
        var validationFailure = new ValidationFailure("Title", "Title is required");
        validator.ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { validationFailure }));

        var validators = new[] { validator };
        var behavior = new ValidationBehavior<CreateGameCommand, GameViewModel>(validators);

        var command = new CreateGameCommand(
            Title: string.Empty,
            Description: "Test Description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        RequestHandlerDelegate<GameViewModel> next = (ct) => Task.FromResult(new GameViewModel
        {
            Id = 1,
            Title = "Test",
            Description = "Test",
            PriceAmount = 10,
            PriceCurrency = "BRL",
            Status = GameStatus.Draft,
            Genres = [Genre.Action],
            Platforms = [Platform.PC],
            CreatedAt = DateTime.UtcNow
        });

        // Act
        var act = async () => await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*validation*");
    }

    [Fact]
    public async Task Handle_MultipleValidators_AllValid_ShouldCallNext()
    {
        // Arrange
        var validator1 = Substitute.For<IValidator<CreateGameCommand>>();
        validator1.ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var validator2 = Substitute.For<IValidator<CreateGameCommand>>();
        validator2.ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var validators = new[] { validator1, validator2 };
        var behavior = new ValidationBehavior<CreateGameCommand, GameViewModel>(validators);

        var command = new CreateGameCommand(
            Title: "Test Game",
            Description: "Test Description",
            PriceAmount: 59.99m,
            PriceCurrency: "BRL",
            Genres: [Genre.Action],
            Platforms: [Platform.PC],
            ReleaseDate: DateTime.UtcNow,
            Publisher: null,
            Developer: null,
            CoverImageUrl: null
        );

        var expectedResult = new GameViewModel
        {
            Id = 1,
            Title = "Test Game",
            Description = "Test Description",
            PriceAmount = 59.99m,
            PriceCurrency = "BRL",
            Status = GameStatus.Draft,
            Genres = [Genre.Action],
            Platforms = [Platform.PC],
            CreatedAt = DateTime.UtcNow
        };

        var nextCalled = false;
        RequestHandlerDelegate<GameViewModel> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResult);
        };

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be(expectedResult);
        await validator1.Received(1).ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>());
        await validator2.Received(1).ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MultipleFailures_ShouldIncludeAllErrors()
    {
        // Arrange
        var validator1 = Substitute.For<IValidator<CreateGameCommand>>();
        var failure1 = new ValidationFailure("Title", "Title is required");
        validator1.ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { failure1 }));

        var validator2 = Substitute.For<IValidator<CreateGameCommand>>();
        var failure2 = new ValidationFailure("Description", "Description is required");
        validator2.ValidateAsync(Arg.Any<ValidationContext<CreateGameCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { failure2 }));

        var validators = new[] { validator1, validator2 };
        var behavior = new ValidationBehavior<CreateGameCommand, GameViewModel>(validators);

        var command = new CreateGameCommand(
            Title: string.Empty,
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

        RequestHandlerDelegate<GameViewModel> next = (ct) => Task.FromResult(new GameViewModel
        {
            Id = 1,
            Title = "Test",
            Description = "Test",
            PriceAmount = 10,
            PriceCurrency = "BRL",
            Status = GameStatus.Draft,
            Genres = [Genre.Action],
            Platforms = [Platform.PC],
            CreatedAt = DateTime.UtcNow
        });

        // Act
        var act = async () => await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_SingleParam_NoValidators_CallsNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<DeleteGameCommand>>();
        var behavior = new ValidationBehavior<DeleteGameCommand>(validators);

        var command = new DeleteGameCommand(1);

        var nextCalled = false;
        RequestHandlerDelegate next = (ct) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SingleParam_ValidInput_CallsNext()
    {
        // Arrange
        var validator = Substitute.For<IValidator<DeleteGameCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<DeleteGameCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var validators = new[] { validator };
        var behavior = new ValidationBehavior<DeleteGameCommand>(validators);

        var command = new DeleteGameCommand(1);

        var nextCalled = false;
        RequestHandlerDelegate next = (ct) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        await validator.Received(1).ValidateAsync(Arg.Any<ValidationContext<DeleteGameCommand>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SingleParam_InvalidInput_ThrowsValidationException()
    {
        // Arrange
        var validator = Substitute.For<IValidator<DeleteGameCommand>>();
        var validationFailure = new ValidationFailure("GameId", "GameId must be greater than 0");
        validator.ValidateAsync(Arg.Any<ValidationContext<DeleteGameCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { validationFailure }));

        var validators = new[] { validator };
        var behavior = new ValidationBehavior<DeleteGameCommand>(validators);

        var command = new DeleteGameCommand(0);

        RequestHandlerDelegate next = (ct) => Task.CompletedTask;

        // Act
        var act = async () => await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*validation*");
    }
}
