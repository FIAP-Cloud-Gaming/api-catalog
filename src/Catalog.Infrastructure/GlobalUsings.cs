global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using Catalog.Domain.AggregatesModel.GameAggregate;
global using Catalog.Domain.AggregatesModel.ReviewAggregate;
global using Catalog.Domain.AggregatesModel.PriceHistoryAggregate;
global using Catalog.Domain.SearchModels;

global using Core.SeedWork;
global using Mediator;
global using Mediator.Extensions;

global using System.Text.Json;

global using Elastic.Clients.Elasticsearch;
global using Elastic.Clients.Elasticsearch.QueryDsl;

global using StackExchange.Redis;

global using MongoDB.Driver;