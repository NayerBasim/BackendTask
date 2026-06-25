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

        public static Playlist dtoToPlaylist(this PlaylistDto playlistDto)
        {
            return new Playlist
            {
                Id = playlistDto.Id,
                Name = playlistDto.Name,
                Description = playlistDto.Description,
                Songs = playlistDto.Songs.Select(s => s.dtoToSong()).ToList()
            };
        }
    }
}