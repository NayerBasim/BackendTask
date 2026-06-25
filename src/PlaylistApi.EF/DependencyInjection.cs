// src/PlaylistApi.EF/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlaylistApi.EF.Repositories;
using PlaylistApi.Core.Interfaces;
using PlaylistApi.EF;


namespace PlaylistApi.EF;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString, b => b.MigrationsAssembly("PlaylistApi.EF")));   // UseSqlite lives here, not in API

        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<ISongRepository, SongRepository>();
        return services;
    }
}