using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistApi.Core.Entities
{
    public class Playlist
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public List<Song> Songs { get; set; } = new List<Song>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}