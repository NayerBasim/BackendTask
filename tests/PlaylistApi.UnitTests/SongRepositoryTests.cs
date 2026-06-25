// tests/PlaylistApi.UnitTests/Repositories/SongRepositoryTests.cs
using Microsoft.EntityFrameworkCore;
using PlaylistApi.Core.Entities;
using PlaylistApi.EF;
using PlaylistApi.EF.Repositories;

namespace PlaylistApi.UnitTests.Repositories;

public class SongRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SongRepository _repository;

    public SongRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
            .Options;

        _context = new AppDbContext(options);
        _repository = new SongRepository(_context);
    }

    // ── GetAllSongsAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetAllSongsAsync_ReturnsAllSongs()
    {
        _context.Songs.AddRange(
            new Song { Title = "Song A", Artist = "Artist A" },
            new Song { Title = "Song B", Artist = "Artist B" }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllSongsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
    }

    [Fact]
    public async Task GetAllSongsAsync_EmptyDb_ReturnsEmptyList()
    {
        var result = await _repository.GetAllSongsAsync();

        Assert.NotNull(result);
        Assert.Empty(result!);
    }

    public void Dispose() => _context.Dispose();
}
