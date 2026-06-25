// tests/PlaylistApi.UnitTests/Repositories/PlaylistRepositoryTests.cs
using Microsoft.EntityFrameworkCore;

using PlaylistApi.Core.Entities;
using PlaylistApi.EF;
using PlaylistApi.EF.Repositories;

namespace PlaylistApi.UnitTests.Repositories;

public class PlaylistRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly PlaylistRepository _repository;

    public PlaylistRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
            .Options;

        _context = new AppDbContext(options);
        _repository = new PlaylistRepository(_context);
    }

    // ── GetPlaylistByIdAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetPlaylistByIdAsync_ExistingId_ReturnsPlaylist()
    {
        var playlist = new Playlist { Name = "Chill", Description = "A chill playlist" };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        var result = await _repository.GetPlaylistByIdAsync(playlist.Id);

        Assert.NotNull(result);
        Assert.Equal("Chill", result.Name);
    }

    [Fact]
    public async Task GetPlaylistByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _repository.GetPlaylistByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPlaylistByIdAsync_IncludesSongs()
    {
        var playlist = new Playlist
        {
            Name = "Chill",
            Description = "A chill playlist",
            Songs = new List<Song> { new Song { Title = "Song A", Artist = "Artist A" } }
        };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        var result = await _repository.GetPlaylistByIdAsync(playlist.Id);

        Assert.Single(result!.Songs);
    }

    // ── GetAllPlaylistsAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetAllPlaylistsAsync_ReturnsAllPlaylists()
    {
        _context.Playlists.AddRange(
            new Playlist { Name = "Chill", Description = "A chill playlist" },
            new Playlist { Name = "Hype", Description = "A hype playlist" }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllPlaylistsAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllPlaylistsAsync_EmptyDb_ReturnsEmptyList()
    {
        var result = await _repository.GetAllPlaylistsAsync();
        Assert.Empty(result);
    }

    // ── AddPlaylistAsync ──────────────────────────────────────────────

    [Fact]
    public async Task AddPlaylistAsync_ValidPlaylist_SavesAndReturns()
    {
        var playlist = new Playlist { Name = "Chill", Description = "A chill playlist" };

        var result = await _repository.AddPlaylistAsync(playlist);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(1, await _context.Playlists.CountAsync());
    }

    [Fact]
    public async Task AddPlaylistAsync_AssignsId()
    {
        var playlist = new Playlist { Name = "Chill", Description = "A chill playlist" };
        var result = await _repository.AddPlaylistAsync(playlist);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    // ── UpdatePlaylistAsync ───────────────────────────────────────────

    [Fact]
    public async Task UpdatePlaylistAsync_ExistingId_UpdatesAndReturns()
    {
        var playlist = new Playlist { Name = "Old Name", Description = "A chill playlist" };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        var result = await _repository.UpdatePlaylistAsync(
            playlist.Id, new Playlist { Name = "New Name", Description = "A chill playlist" });

        Assert.NotNull(result);
        Assert.Equal("New Name", result!.Name);
    }

    [Fact]
    public async Task UpdatePlaylistAsync_NonExistingId_ReturnsNull()
    {
        var result = await _repository.UpdatePlaylistAsync(
            Guid.NewGuid(), new Playlist { Name = "X", Description = "A chill playlist" });

        Assert.Null(result);
    }

    // ── DeletePlaylistAsync ───────────────────────────────────────────

    [Fact]
    public async Task DeletePlaylistAsync_ExistingId_DeletesAndReturns()
    {
        var playlist = new Playlist { Name = "Chill", Description = "A chill playlist" };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        var result = await _repository.DeletePlaylistAsync(playlist.Id);

        Assert.NotNull(result);
        Assert.Equal(0, await _context.Playlists.CountAsync());
    }

    [Fact]
    public async Task DeletePlaylistAsync_NonExistingId_ReturnsNull()
    {
        var result = await _repository.DeletePlaylistAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task DeletePlaylistAsync_KeepsSongsButOrphansThem()
    {
        // Songs are a shared catalog: the Song.PlaylistId FK is optional, so deleting
        // a playlist sets its songs' PlaylistId to null rather than deleting them.
        var song = new Song { Title = "Song A", Artist = "Artist A" };
        var playlist = new Playlist
        {
            Name = "Chill",
            Description = "A chill playlist",
            Songs = new List<Song> { song }
        };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        await _repository.DeletePlaylistAsync(playlist.Id);

        // The song survives the playlist deletion ...
        Assert.Equal(1, await _context.Songs.CountAsync());
        // ... and is no longer linked to any playlist.
        var survivingSong = await _context.Songs.FindAsync(song.Id);
        Assert.NotNull(survivingSong);
        Assert.Null(_context.Entry(survivingSong!).Property<Guid?>("PlaylistId").CurrentValue);
    }

    // ── AddSongToPlaylistAsync ────────────────────────────────────────

    [Fact]
    public async Task AddSongToPlaylistAsync_ExistingPlaylist_AddsSong()
    {
        var playlist = new Playlist { Name = "Chill", Description = "A chill playlist" };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        var result = await _repository.AddSongToPlaylistAsync(
            playlist.Id, new Song { Title = "Song A", Artist = "Artist A" });

        Assert.NotNull(result);
        Assert.Single(result!.Songs);
        Assert.Equal("Song A", result.Songs[0].Title);
        Assert.Equal(1, await _context.Songs.CountAsync());
    }

    [Fact]
    public async Task AddSongToPlaylistAsync_NonExistingPlaylist_ReturnsNull()
    {
        var result = await _repository.AddSongToPlaylistAsync(
            Guid.NewGuid(), new Song { Title = "Song A", Artist = "Artist A" });

        Assert.Null(result);
    }

    // ── RemoveSongFromPlaylistAsync ───────────────────────────────────

    [Fact]
    public async Task RemoveSongFromPlaylistAsync_ExistingSong_RemovesSong()
    {
        var song = new Song { Title = "Song A", Artist = "Artist A" };
        var playlist = new Playlist
        {
            Name = "Chill",
            Description = "A chill playlist",
            Songs = new List<Song> { song }
        };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        var result = await _repository.RemoveSongFromPlaylistAsync(playlist.Id, song.Id);

        Assert.NotNull(result);
        Assert.Empty(result!.Songs);
    }

    [Fact]
    public async Task RemoveSongFromPlaylistAsync_NonExistingPlaylist_ReturnsNull()
    {
        var result = await _repository.RemoveSongFromPlaylistAsync(
            Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveSongFromPlaylistAsync_NonExistingSong_ReturnsNull()
    {
        var playlist = new Playlist { Name = "Chill", Description = "A chill playlist" };
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();

        var result = await _repository.RemoveSongFromPlaylistAsync(playlist.Id, Guid.NewGuid());

        Assert.Null(result);
    }

    public void Dispose() => _context.Dispose();
}