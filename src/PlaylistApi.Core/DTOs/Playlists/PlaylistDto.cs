using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PlaylistApi.Core.DTOs.Songs;

namespace PlaylistApi.Core.DTOs.Playlists
{
    public class PlaylistDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public List<SongDto> Songs { get; set; } = new List<SongDto>();
    }
}