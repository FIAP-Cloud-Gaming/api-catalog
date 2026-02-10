using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Infrastructure.Repositories;

public sealed class GameRepositoryTests : IDisposable
{
    private readonly CatalogContext _context;
    private readonly GameRepository _repository;

    public GameRepositoryTests()
    {
        _context = CatalogContextFactory.Create();
        _repository = new GameRepository(_context);
    }

    [Fact]
    public async Task Add_ShouldAddGameToContext()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame();

        // Act
        _repository.Add(game);
        await _context.SaveChangesAsync();

        // Assert
        var addedGame = await _context.Games.FirstOrDefaultAsync(g => g.Id == game.Id);
        addedGame.Should().NotBeNull();
        addedGame.Should().Be(game);
    }

    [Fact]
    public async Task GetByIdAsync_GameExists_ShouldReturnGame()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game");
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Value.Should().Be("Test Game");
    }

    [Fact]
    public async Task GetByIdAsync_GameNotExists_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGames()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGameWithId(1, "Game 1");
        var game2 = GameTestFactory.CreateValidGameWithId(2, "Game 2");
        var game3 = GameTestFactory.CreateValidGameWithId(3, "Game 3");

        _context.Games.AddRange(game1, game2, game3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderByCreatedAtDescending()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGameWithId(1, "Game 1");
        await Task.Delay(10);
        var game2 = GameTestFactory.CreateValidGameWithId(2, "Game 2");
        await Task.Delay(10);
        var game3 = GameTestFactory.CreateValidGameWithId(3, "Game 3");

        _context.Games.AddRange(game1, game2, game3);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(3);
        result[1].Id.Should().Be(2);
        result[2].Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnFilteredGames()
    {
        // Arrange
        var draftGame = GameTestFactory.CreateValidGameWithId(1, "Draft Game");
        var publishedGame = GameTestFactory.CreateValidGameWithId(2, "Published Game");
        publishedGame.Publish();

        _context.Games.AddRange(draftGame, publishedGame);
        await _context.SaveChangesAsync();

        // Act
        var draftGames = await _repository.GetByStatusAsync(GameStatus.Draft);
        var publishedGames = await _repository.GetByStatusAsync(GameStatus.Published);

        // Assert
        draftGames.Should().ContainSingle();
        draftGames.First().Id.Should().Be(1);

        publishedGames.Should().ContainSingle();
        publishedGames.First().Id.Should().Be(2);
    }

    [Fact]
    public void Update_ShouldMarkGameAsModified()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game");
        _context.Games.Add(game);
        _context.SaveChanges();

        // Act
        _repository.Update(game);

        // Assert
        var entry = _context.Entry(game);
        entry.State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public async Task Remove_ShouldRemoveGameFromContext()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game");
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(game);
        await _context.SaveChangesAsync();

        // Assert
        var removedGame = await _context.Games.FirstOrDefaultAsync(g => g.Id == 1);
        removedGame.Should().BeNull();
    }

    [Fact]
    public void UnitOfWork_ShouldReturnContext()
    {
        // Act
        var unitOfWork = _repository.UnitOfWork;

        // Assert
        unitOfWork.Should().NotBeNull();
        unitOfWork.Should().Be(_context);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
