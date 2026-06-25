using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistApi.Core.Entities
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Song> Songs { get; set; } = new List<Song>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}