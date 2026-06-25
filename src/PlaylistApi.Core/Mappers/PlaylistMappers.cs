using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaylistApi.Core.DTOs.Playlists;
using PlaylistApi.Core.Entities;

namespace PlaylistApi.Core.Mappers
{
    public static class PlaylistMappers
    {
        public static PlaylistDto playlistToDto(this Playlist playlist)
        {
            return new PlaylistDto
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Description = playlist.Description,
                Songs = playlist.Songs.Select(s => s.songToDto()).ToList()
            };
        }

        public static Playlist toPlaylist(this CreatePlaylistDto dto)
        {
            return new Playlist
            {
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public static Playlist toPlaylist(this EditPlaylistDto dto)
        {
            return new Playlist
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };
        }
    }
}