using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaylistApi.Core.Entities;

namespace PlaylistApi.Core.Interfaces
{
    public interface IPlaylistRepository
    {
        Task<Playlist?> GetPlaylistByIdAsync(Guid id);
        Task<List<Playlist>?> GetAllPlaylistsAsync();
        Task<Playlist?> AddPlaylistAsync(Playlist playlist);
        Task<Playlist?> UpdatePlaylistAsync(Guid id, Playlist playlist);
        Task<Playlist?> DeletePlaylistAsync(Guid id);
        Task<Playlist?> AddSongToPlaylistAsync(Guid playlistId, Guid songId);
        Task<Playlist?> RemoveSongFromPlaylistAsync(Guid playlistId, Guid songId);
    }
}