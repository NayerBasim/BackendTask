using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaylistApi.Core.DTOs.Songs;
namespace PlaylistApi.Core.DTOs.Playlists
{
    public class EditPlaylistDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<SongDto> Songs { get; set; } = new List<SongDto>();
    }
}