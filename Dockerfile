# 1. Base Image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy projects
COPY ["HealthApp.API/HealthApp.API.csproj", "HealthApp.API/"]
COPY ["HealthApp.Business/HealthApp.Business.csproj", "HealthApp.Business/"]
COPY ["HealthApp.DataAccess/HealthApp.DataAccess.csproj", "HealthApp.DataAccess/"]
COPY ["HealthApp.Domain/HealthApp.Domain.csproj", "HealthApp.Domain/"]

# Restore packages
RUN dotnet restore "HealthApp.API/HealthApp.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/HealthApp.API"
RUN dotnet build "HealthApp.API.csproj" -c Release -o /app/build

# 3. Publish
FROM build AS publish
RUN dotnet publish "HealthApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthApp.API.dll"]
