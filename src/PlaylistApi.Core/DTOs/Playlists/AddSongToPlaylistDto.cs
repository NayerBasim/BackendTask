using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistApi.Core.DTOs.Playlists
{
    public class AddSongToPlaylistDto
    {
        // The playlist is identified by the route; only the song to link is needed in the body.
        public Guid SongId { get; set; }
    }
}