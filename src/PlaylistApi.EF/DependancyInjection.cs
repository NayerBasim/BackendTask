// src/PlaylistApi.EF/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlaylistApi.src.PlaylistApi.EF;

namespace PlaylistApi.EF;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString, b => b.MigrationsAssembly("PlaylistApi.EF")));   // UseSqlite lives here, not in API

        return services;
    }
}