// tests/PlaylistApi.IntegrationTests/Controllers/PlaylistsControllerTests.cs
using System.Net;
using System.Net.Http.Json;
using PlaylistApi.Core.Entities;

namespace PlaylistApi.IntegrationTests.Controllers;

public class PlaylistsControllerTests : IClassFixture<PlaylistApiFactory>
{
    private readonly HttpClient _client;

    public PlaylistsControllerTests(PlaylistApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── POST /api/playlists ───────────────────────────────────────────

    [Fact]
    public async Task CreatePlaylist_ValidBody_Returns201()
    {
        var payload = new { Name = "Chill Vibes", Description = "A chill playlist" };

        var response = await _client.PostAsJsonAsync("/api/playlists", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreatePlaylist_ReturnsCreatedPlaylistWithId()
    {
        var payload = new { Name = "Chill Vibes", Description = "A chill playlist" };

        var response = await _client.PostAsJsonAsync("/api/playlists", payload);
        var created = await response.Content.ReadFromJsonAsync<Playlist>();

        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);
        Assert.Equal("Chill Vibes", created.Name);
    }

    [Fact]
    public async Task CreatePlaylist_EmptyName_Returns400()
    {
        var payload = new { Name = "", Description = "A chill playlist" };

        var response = await _client.PostAsJsonAsync("/api/playlists", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── GET /api/playlists ────────────────────────────────────────────

    [Fact]
    public async Task GetAllPlaylists_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/playlists");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllPlaylists_AfterCreating_ReturnsCorrectCount()
    {
        await _client.PostAsJsonAsync("/api/playlists", new { Name = "A", Description = "A chill playlist" });
        await _client.PostAsJsonAsync("/api/playlists", new { Name = "B", Description = "A hype playlist" });

        var response = await _client.GetAsync("/api/playlists");
        var playlists = await response.Content.ReadFromJsonAsync<List<Playlist>>();

        Assert.True(playlists!.Count >= 2);
    }

    // ── GET /api/playlists/{id} ───────────────────────────────────────

    [Fact]
    public async Task GetPlaylistById_ExistingId_ReturnsOk()
    {
        var created = await CreatePlaylistAsync("Chill");

        var response = await _client.GetAsync($"/api/playlists/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPlaylistById_NonExistingId_Returns404()
    {
        var response = await _client.GetAsync($"/api/playlists/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPlaylistById_ReturnsSongsIncluded()
    {
        var created = await CreatePlaylistAsync("Chill");
        await _client.PostAsJsonAsync(
            $"/api/playlists/{created!.Id}/songs",
            new { Title = "Song A", Artist = "Artist A" });

        var response = await _client.GetAsync($"/api/playlists/{created.Id}");
        var playlist = await response.Content.ReadFromJsonAsync<Playlist>();

        Assert.Single(playlist!.Songs);
    }

    // ── PUT /api/playlists/{id} ───────────────────────────────────────

    [Fact]
    public async Task UpdatePlaylist_ExistingId_ReturnsOk()
    {
        var created = await CreatePlaylistAsync("Old Name");

        var response = await _client.PutAsJsonAsync(
            $"/api/playlists/{created!.Id}",
            new { Name = "New Name", Description = "A chill playlist" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlaylist_NonExistingId_Returns404()
    {
        var response = await _client.PutAsJsonAsync(
            $"/api/playlists/{Guid.NewGuid()}",
            new { Name = "New Name", Description = "A chill playlist" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlaylist_ChangesArePersisted()
    {
        var created = await CreatePlaylistAsync("Old Name");

        await _client.PutAsJsonAsync(
            $"/api/playlists/{created!.Id}",
            new { Name = "New Name", Description = "A chill playlist" });

        var response = await _client.GetAsync($"/api/playlists/{created.Id}");
        var updated = await response.Content.ReadFromJsonAsync<Playlist>();

        Assert.Equal("New Name", updated!.Name);
    }

    // ── DELETE /api/playlists/{id} ────────────────────────────────────

    [Fact]
    public async Task DeletePlaylist_ExistingId_Returns204()
    {
        var created = await CreatePlaylistAsync("Chill");

        var response = await _client.DeleteAsync($"/api/playlists/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeletePlaylist_NonExistingId_Returns404()
    {
        var response = await _client.DeleteAsync($"/api/playlists/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePlaylist_CannotBeRetrievedAfterDeletion()
    {
        var created = await CreatePlaylistAsync("Chill");
        await _client.DeleteAsync($"/api/playlists/{created!.Id}");

        var response = await _client.GetAsync($"/api/playlists/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── Helper ────────────────────────────────────────────────────────

    private async Task<Playlist?> CreatePlaylistAsync(string name, string description = "A chill playlist")
    {
        var response = await _client.PostAsJsonAsync(
            "/api/playlists", new { Name = name, Description = description });
        return await response.Content.ReadFromJsonAsync<Playlist>();
    }
}