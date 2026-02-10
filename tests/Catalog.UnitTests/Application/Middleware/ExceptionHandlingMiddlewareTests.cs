using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Middleware;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_NoException_ShouldCallNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = NullLogger<ExceptionHandlingMiddleware>.Instance;

        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionHandlingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_ShouldReturn400()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = NullLogger<ExceptionHandlingMiddleware>.Instance;

        var validationFailure = new ValidationFailure("Title", "Title is required");
        var validationException = new ValidationException(new[] { validationFailure });

        RequestDelegate next = (ctx) => throw validationException;

        var middleware = new ExceptionHandlingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_CatalogDomainException_ShouldReturn400()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = NullLogger<ExceptionHandlingMiddleware>.Instance;

        var domainException = new CatalogDomainException("Cannot publish game in current state");

        RequestDelegate next = (ctx) => throw domainException;

        var middleware = new ExceptionHandlingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().NotBeNullOrEmpty();

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.Should().Contain("Domain Error");
        responseBody.Should().Contain("Cannot publish game in current state");
    }

    [Fact]
    public async Task InvokeAsync_KeyNotFoundException_ShouldReturn404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = NullLogger<ExceptionHandlingMiddleware>.Instance;

        var notFoundException = new KeyNotFoundException("Game not found");

        RequestDelegate next = (ctx) => throw notFoundException;

        var middleware = new ExceptionHandlingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(404);
        context.Response.ContentType.Should().NotBeNullOrEmpty();

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.Should().Contain("Not Found");
    }

    [Fact]
    public async Task InvokeAsync_UnauthorizedAccessException_ShouldReturn401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = NullLogger<ExceptionHandlingMiddleware>.Instance;

        var unauthorizedException = new UnauthorizedAccessException("User not authorized");

        RequestDelegate next = (ctx) => throw unauthorizedException;

        var middleware = new ExceptionHandlingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(401);
        context.Response.ContentType.Should().NotBeNullOrEmpty();

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.Should().Contain("Unauthorized");
    }

    [Fact]
    public async Task InvokeAsync_GenericException_ShouldReturn500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = NullLogger<ExceptionHandlingMiddleware>.Instance;

        var genericException = new Exception("Unexpected error occurred");

        RequestDelegate next = (ctx) => throw genericException;

        var middleware = new ExceptionHandlingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().NotBeNullOrEmpty();

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.Should().Contain("Server Error");
        responseBody.Should().Contain("An unexpected error occurred");
    }
}
