using Catalog.Infrastructure.IntegrationEvents;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(configuration);

        // PostgreSQL
        services.AddDbContext<CatalogContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("CatalogDB"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(CatalogContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                })
            .ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();

        // MongoDB
        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("MongoDB")
                ?? throw new InvalidOperationException("MongoDB connection string not found");
            return new MongoClient(connectionString);
        });

        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseName = configuration["MongoDB:DatabaseName"] ?? "CatalogDB";
            return client.GetDatabase(databaseName);
        });

        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();

        // Elasticsearch
        services.AddSingleton(sp =>
        {
            var uri = configuration["Elasticsearch:Uri"]
                ?? throw new InvalidOperationException("Elasticsearch URI not found");

            var settings = new ElasticsearchClientSettings(new Uri(uri))
                .DefaultIndex("games")
                .DisableDirectStreaming()
                .RequestTimeout(TimeSpan.FromMinutes(2));

            var username = configuration["Elasticsearch:Username"];
            var password = configuration["Elasticsearch:Password"];
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                settings.Authentication(new Elastic.Transport.BasicAuthentication(username, password));
            }

            return new ElasticsearchClient(settings);
        });

        services.AddScoped<IGameSearchRepository, GameSearchRepository>();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var redisConnection = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string not found");

            return ConnectionMultiplexer.Connect(redisConnection);
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JwtSettings:Secret not found in configuration");

        var jwtIssuer = configuration["JwtSettings:Issuer"]
            ?? throw new InvalidOperationException("JwtSettings:Issuer not found in configuration");

        var jwtAudience = configuration["JwtSettings:Audience"]
            ?? throw new InvalidOperationException("JwtSettings:Audience not found in configuration");

        if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
            throw new InvalidOperationException("JwtSettings:Secret must be at least 32 characters long");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }
}
