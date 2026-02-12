global using Xunit;
global using FluentAssertions;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;

global using Catalog.Domain.AggregatesModel.GameAggregate;
global using Catalog.Domain.AggregatesModel.ReviewAggregate;
global using Catalog.Domain.AggregatesModel.PriceHistoryAggregate;
global using Catalog.Domain.ValueObjects;
global using Catalog.Domain.Events;
global using Catalog.Domain.Exceptions;
global using Catalog.Domain.CacheModels;
global using Catalog.Domain.SearchModels;

global using Catalog.UnitTests.Helpers;

global using Catalog.Infrastructure;
global using Catalog.Infrastructure.Repositories;

global using Catalog.API.Application.Commands.CreateGame;
global using Catalog.API.Application.Commands.UpdateGame;
global using Catalog.API.Application.Commands.DeleteGame;
global using Catalog.API.Application.Commands.ActivateGame;
global using Catalog.API.Application.Commands.DeactivateGame;
global using Catalog.API.Application.Commands.ChangeGamePrice;
global using Catalog.API.Application.Commands.ReindexGames;
global using Catalog.API.Application.Queries.GetGameById;
global using Catalog.API.Application.Queries.GetGamesByGenre;
global using Catalog.API.Application.Queries.GetGamesByPlatform;
global using Catalog.API.Application.Queries.GetGamesByStatus;
global using Catalog.API.Application.Queries.SearchGames;
global using Catalog.API.Application.Behaviors;
global using Catalog.API.Application.Validations;
global using Catalog.API.Application.Models;
global using Catalog.API.Application.DomainEventHandlers;
global using Catalog.API.Middleware;

global using Mediator;
global using Mediator.Commands;
global using Mediator.Queries;
global using Mediator.Notifications;
global using Mediator.Behaviors;

global using Core.SeedWork;

global using EventBus.Abstractions;
global using EventBus.Events;

global using Catalog.API.Application.IntegrationEvents.Events;

global using FluentValidation;
global using FluentValidation.Results;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.AspNetCore.Http;
