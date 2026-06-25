using Microsoft.AspNetCore.Mvc;
using PlaylistApi.Core.Interfaces;
using PlaylistApi.Core.Entities;
using PlaylistApi.Core.DTOs.Playlists;
using PlaylistApi.Core.Mappers;

namespace PlaylistApi.API.Controllers;

[ApiController]
[Route("api/playlists")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistRepository _playlistRepository;

    public PlaylistsController(IPlaylistRepository playlistRepository, ISongRepository songRepository)
    {

        _playlistRepository = playlistRepository;

    }


    [HttpGet]
    public async Task<IActionResult> GetAllPlaylists()
    {
        var playlists = await _playlistRepository.GetAllPlaylistsAsync();
        return Ok(playlists.Select(p => p.playlistToDto()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlaylistById(Guid id)
    {
        var playlist = await _playlistRepository.GetPlaylistByIdAsync(id);
        if (playlist == null) return NotFound();
        return Ok(playlist.playlistToDto());
    }

    [HttpPost]
    public async Task<IActionResult> AddPlaylist([FromBody] PlaylistDto playlist) 
    {
        var addedPlaylist = await _playlistRepository.AddPlaylistAsync(playlist.dtoToPlaylist());
        return CreatedAtAction(nameof(GetPlaylistById), new { id = addedPlaylist?.Id }, addedPlaylist);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlaylist(Guid id, [FromBody] PlaylistDto playlist) 
    {
        var updatedPlaylist = await _playlistRepository.UpdatePlaylistAsync(id, playlist.dtoToPlaylist());
        if (updatedPlaylist == null) return NotFound();
        return Ok(updatedPlaylist.playlistToDto());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlaylist(Guid id)
    {
        var deletedPlaylist = await _playlistRepository.DeletePlaylistAsync(id);
        if (deletedPlaylist == null) return NotFound();
        return NoContent();
    }



    [HttpPost("{playlistId}/songs")]
    public async Task<IActionResult> AddSongToPlaylist(Guid playlistId, [FromBody] Song song)
    {
        var updatedPlaylist = await _playlistRepository.AddSongToPlaylistAsync(playlistId, song);
        if (updatedPlaylist == null) return NotFound();
        return Ok(updatedPlaylist.playlistToDto());
    }

    [HttpDelete("{playlistId}/songs/{songId}")]
    public async Task<IActionResult> RemoveSongFromPlaylist(Guid playlistId, Guid songId)
    {
        var updatedPlaylist = await _playlistRepository.RemoveSongFromPlaylistAsync(playlistId, songId);
        if (updatedPlaylist == null) return NotFound();
        return Ok(updatedPlaylist.playlistToDto());
    }
}