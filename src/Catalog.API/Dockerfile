FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GITHUB_TOKEN
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Catalog.API/Catalog.API.csproj", "src/Catalog.API/"]
COPY ["src/Catalog.Domain/Catalog.Domain.csproj", "src/Catalog.Domain/"]
COPY ["src/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "src/Catalog.Infrastructure/"]

RUN if [ -n "$GITHUB_TOKEN" ]; then \
      dotnet nuget remove source github-fcg --configfile nuget.config || true && \
      dotnet nuget add source "https://nuget.pkg.github.com/FIAP-Cloud-Gaming/index.json" \
        --name github-fcg \
        --username FIAP-Cloud-Gaming \
        --password "$GITHUB_TOKEN" \
        --store-password-in-clear-text \
        --configfile nuget.config; \
    fi

RUN dotnet restore "src/Catalog.API/Catalog.API.csproj"
COPY . .
WORKDIR "/src/src/Catalog.API"
RUN dotnet build "Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Catalog.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
