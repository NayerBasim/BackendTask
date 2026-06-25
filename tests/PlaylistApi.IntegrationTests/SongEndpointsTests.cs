// tests/PlaylistApi.IntegrationTests/Controllers/SongEndpointsTests.cs
using System.Net;
using System.Net.Http.Json;
using PlaylistApi.Core.Entities;

namespace PlaylistApi.IntegrationTests.Controllers;

public class SongEndpointsTests : IClassFixture<PlaylistApiFactory>
{
    private readonly HttpClient _client;

    public SongEndpointsTests(PlaylistApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── GET /api/songs ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllSongs_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/songs");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllSongs_ReturnsSeededSongs()
    {
        var response = await _client.GetAsync("/api/songs");
        var songs = await response.Content.ReadFromJsonAsync<List<Song>>();

        Assert.NotNull(songs);
        Assert.True(songs!.Count >= 4); // four songs are seeded on startup
    }

    // ── POST /api/playlists/{id}/songs ────────────────────────────────

    [Fact]
    public async Task AddSongToPlaylist_ExistingSong_LinksSong()
    {
        var playlist = await CreatePlaylistAsync("Chill");
        var songId = await GetCatalogSongIdAsync(0);

        var response = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlist!.Id}/songs",
            new { SongId = songId });
        var updated = await response.Content.ReadFromJsonAsync<Playlist>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(updated!.Songs);
        Assert.Equal(songId, updated.Songs[0].Id);
    }

    [Fact]
    public async Task AddSongToPlaylist_NonExistingPlaylist_Returns404()
    {
        var songId = await GetCatalogSongIdAsync(0);

        var response = await _client.PostAsJsonAsync(
            $"/api/playlists/{Guid.NewGuid()}/songs",
            new { SongId = songId });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddSongToPlaylist_NonExistingSong_Returns404()
    {
        var playlist = await CreatePlaylistAsync("Chill");

        var response = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlist!.Id}/songs",
            new { SongId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── DELETE /api/playlists/{id}/songs/{songId} ─────────────────────

    [Fact]
    public async Task RemoveSongFromPlaylist_ExistingSong_RemovesSong()
    {
        var playlist = await CreatePlaylistAsync("Chill");
        var songId = await GetCatalogSongIdAsync(1); // use a different catalog song
        await _client.PostAsJsonAsync(
            $"/api/playlists/{playlist!.Id}/songs",
            new { SongId = songId });

        var response = await _client.DeleteAsync(
            $"/api/playlists/{playlist.Id}/songs/{songId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // The song should no longer be part of the playlist
        var getResponse = await _client.GetAsync($"/api/playlists/{playlist.Id}");
        var afterRemoval = await getResponse.Content.ReadFromJsonAsync<Playlist>();
        Assert.Empty(afterRemoval!.Songs);
    }

    [Fact]
    public async Task RemoveSongFromPlaylist_NonExistingPlaylist_Returns404()
    {
        var response = await _client.DeleteAsync(
            $"/api/playlists/{Guid.NewGuid()}/songs/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveSongFromPlaylist_NonExistingSong_Returns404()
    {
        var playlist = await CreatePlaylistAsync("Chill");

        var response = await _client.DeleteAsync(
            $"/api/playlists/{playlist!.Id}/songs/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── Helpers ───────────────────────────────────────────────────────

    private async Task<Playlist?> CreatePlaylistAsync(string name)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/playlists", new { Name = name, Description = "A chill playlist" });
        return await response.Content.ReadFromJsonAsync<Playlist>();
    }

    private async Task<Guid> GetCatalogSongIdAsync(int index)
    {
        var songs = await _client.GetFromJsonAsync<List<Song>>("/api/songs");
        return songs![index].Id;
    }
}
