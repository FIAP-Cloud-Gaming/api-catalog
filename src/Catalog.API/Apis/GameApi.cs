using Catalog.API.Application.Commands.ActivateGame;
using Catalog.API.Application.Commands.ChangeGamePrice;
using Catalog.API.Application.Commands.CreateGame;
using Catalog.API.Application.Commands.DeactivateGame;
using Catalog.API.Application.Commands.DeleteGame;
using Catalog.API.Application.Commands.ReindexGames;
using Catalog.API.Application.Commands.UpdateGame;
using Catalog.API.Application.Queries.GetGameById;
using Catalog.API.Application.Queries.GetGamesByGenre;
using Catalog.API.Application.Queries.GetGamesByPlatform;
using Catalog.API.Application.Queries.GetGamesByStatus;
using Catalog.API.Application.Queries.SearchGames;
using Catalog.API.Application.Queries.GetGameMetrics;
using Catalog.API.Application.Queries.GetRecommendations;

namespace Catalog.API.Apis;

public static class GameApi
{
    public static RouteGroupBuilder MapGamesApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/games").WithTags("Games");

        api.MapPost("/", CreateGame)
            .WithName("CreateGame")
            .WithDescription("Create a new game in the catalog")
            .Produces<GameViewModel>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        api.MapGet("/{id:int}", GetGameById)
            .WithName("GetGameById")
            .WithDescription("Get a game by its ID")
            .Produces<GameViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        api.MapPut("/{id:int}", UpdateGame)
            .WithName("UpdateGame")
            .WithDescription("Update an existing game")
            .Produces<GameViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        api.MapDelete("/{id:int}", DeleteGame)
            .WithName("DeleteGame")
            .WithDescription("Delete a game from the catalog")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        api.MapPut("/{id:int}/price", ChangeGamePrice)
            .WithName("ChangeGamePrice")
            .WithDescription("Change the price of a game")
            .Produces<GameViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        api.MapPut("/{id:int}/activate", ActivateGame)
            .WithName("ActivateGame")
            .WithDescription("Activate (publish) a game")
            .Produces<GameViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        api.MapPut("/{id:int}/deactivate", DeactivateGame)
            .WithName("DeactivateGame")
            .WithDescription("Deactivate (unpublish) a game")
            .Produces<GameViewModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        api.MapGet("/search", SearchGames)
            .WithName("SearchGames")
            .WithDescription("Search for games with filters using Elasticsearch")
            .Produces<PagedResponse<GameViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        api.MapGet("/genre/{genre}", GetGamesByGenre)
            .WithName("GetGamesByGenre")
            .WithDescription("Get all games by genre")
            .Produces<IEnumerable<GameViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        api.MapGet("/platform/{platform}", GetGamesByPlatform)
            .WithName("GetGamesByPlatform")
            .WithDescription("Get all games by platform")
            .Produces<IEnumerable<GameViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        api.MapGet("/status/{status}", GetGamesByStatus)
            .WithName("GetGamesByStatus")
            .WithDescription("Get all games by status (Draft, Published, Unpublished, Archived)")
            .Produces<IEnumerable<GameViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        api.MapPost("/reindex", ReindexGames)
            .WithName("ReindexGames")
            .WithDescription("Re-index all games from PostgreSQL to Elasticsearch")
            .Produces<ReindexGamesResult>(StatusCodes.Status200OK);

        api.MapGet("/metrics", GetGameMetrics)
            .WithName("GetGameMetrics")
            .WithDescription("Get aggregated game metrics (genres, platforms, prices, reviews)")
            .Produces<GameMetrics>(StatusCodes.Status200OK);

        api.MapGet("/recommendations", GetRecommendations)
            .WithName("GetRecommendations")
            .WithDescription("Get game recommendations based on similar games (More Like This)")
            .Produces<IReadOnlyList<GameViewModel>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        return api;
    }

    private static async Task<IResult> CreateGame(
        [FromBody] CreateGameRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = CreateGameCommand.FromRequest(request);
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Created($"/api/games/{result.Id}", result);
    }

    private static async Task<IResult> GetGameById(
        [FromRoute] int id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetGameByIdQuery(id);
        var result = await mediator.Receive(query, cancellationToken);

        return result is not null
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = $"Game with ID {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
    }

    private static async Task<IResult> UpdateGame(
        [FromRoute] int id,
        [FromBody] UpdateGameRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = UpdateGameCommand.FromRequest(id, request);
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeleteGame(
        [FromRoute] int id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteGameCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result
            ? TypedResults.NoContent()
            : TypedResults.NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = $"Game with ID {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
    }

    private static async Task<IResult> ChangeGamePrice(
        [FromRoute] int id,
        [FromBody] ChangePriceRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = ChangeGamePriceCommand.FromRequest(id, request);
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> ActivateGame(
        [FromRoute] int id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ActivateGameCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeactivateGame(
        [FromRoute] int id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateGameCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> SearchGames(
        [AsParameters] SearchGamesRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = SearchGamesQuery.FromRequest(request);
        var result = await mediator.Receive(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetGamesByGenre(
        [FromRoute] Genre genre,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetGamesByGenreQuery(genre);
        var result = await mediator.Receive(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetGamesByPlatform(
        [FromRoute] Platform platform,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetGamesByPlatformQuery(platform);
        var result = await mediator.Receive(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetGamesByStatus(
        [FromRoute] GameStatus status,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetGamesByStatusQuery(status);
        var result = await mediator.Receive(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> ReindexGames(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ReindexGamesCommand();
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetGameMetrics(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetGameMetricsQuery();
        var result = await mediator.Receive(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetRecommendations(
        [FromQuery] string gameIds,
        [FromQuery] int size,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var ids = gameIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToList();

        var query = new GetRecommendationsQuery(ids, size > 0 ? size : 10);
        var result = await mediator.Receive(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
