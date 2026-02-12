global using System.Reflection;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.OpenApi;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.OpenApi;

global using FluentValidation;

global using Mediator;
global using Mediator.Commands;
global using Mediator.Queries;
global using Mediator.Notifications;
global using Mediator.Behaviors;
global using Mediator.Extensions;

global using Catalog.Domain.AggregatesModel.GameAggregate;
global using Catalog.Domain.AggregatesModel.PriceHistoryAggregate;
global using Catalog.Domain.ValueObjects;
global using Catalog.Domain.SearchModels;
global using Catalog.Domain.Events;
global using Catalog.Domain.Exceptions;

global using Catalog.API.Application.Models;
global using Catalog.API.Application.Behaviors;

global using Catalog.Infrastructure.Extensions;
global using Catalog.Infrastructure;

global using Elastic.Serilog.Sinks;
global using Elastic.Clients.Elasticsearch;

global using MongoDB.Driver;

global using Serilog;

global using Scalar.AspNetCore;

global using Core.SeedWork;

global using Catalog.API.Application.IntegrationEvents.Events;

global using EventBus.Abstractions;
global using EventBus.Events;
global using EventBus.Extensions;
