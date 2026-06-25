using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistApi.src.PlaylistApi.Core.Interfaces
{
    public interface IPlaylistRepository
    {
        Task<Entities.Playlist?> GetPlaylistByIdAsync(Guid id);
        Task<List<Entities.Playlist>?> GetAllPlaylistsAsync();
        Task<Entities.Playlist?> AddPlaylistAsync(Entities.Playlist playlist);
        Task<Entities.Playlist?> UpdatePlaylistAsync(Entities.Playlist playlist);
        Task<Entities.Playlist?> DeletePlaylistAsync(Guid id);
    }
}