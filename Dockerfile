# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (cached unless a .csproj changes)
COPY src/PlaylistApi.API/PlaylistApi.API.csproj src/PlaylistApi.API/
COPY src/PlaylistApi.Core/PlaylistApi.Core.csproj src/PlaylistApi.Core/
COPY src/PlaylistApi.EF/PlaylistApi.EF.csproj src/PlaylistApi.EF/
RUN dotnet restore src/PlaylistApi.API/PlaylistApi.API.csproj

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish src/PlaylistApi.API/PlaylistApi.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Default port; the platform may override via the PORT env var (handled in Program.cs).
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PlaylistApi.API.dll"]
