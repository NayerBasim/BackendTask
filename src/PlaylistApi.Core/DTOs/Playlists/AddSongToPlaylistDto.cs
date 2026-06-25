using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistApi.Core.DTOs.Playlists
{
    public class AddSongToPlaylistDto
    {
        public Guid PlaylistId { get; set; }
        public Guid SongId { get; set; }
        
    }
}