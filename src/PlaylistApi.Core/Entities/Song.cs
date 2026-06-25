using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistApi.Core.Entities
{
    public class Song
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Artist { get; set; } = string.Empty;

        [StringLength(200)]
        public string Album { get; set; } = string.Empty;

        [StringLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Duration { get; set; } // Duration in seconds
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}