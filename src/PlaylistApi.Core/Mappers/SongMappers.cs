using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaylistApi.Core.DTOs.Songs;
using PlaylistApi.Core.Entities;

namespace PlaylistApi.Core.Mappers
{
    public static class SongMappers
    {
        public static SongDto songToDto(this Song song)
        {
            return new SongDto
            {
                Id = song.Id,
                Title = song.Title,
                Artist = song.Artist,
                Album = song.Album,
                Genre = song.Genre,
                Duration = song.Duration
            };
        }

        public static Song dtoToSong(this SongDto songDto)
        {
            return new Song
            {
                Id = songDto.Id,
                Title = songDto.Title,
                Artist = songDto.Artist,
                Album = songDto.Album,
                Genre = songDto.Genre,
                Duration = songDto.Duration
            };
        }
    }
}