using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PlaylistApi.src.PlaylistApi.EF
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Core.Entities.Playlist> Playlists { get; set; }
        public DbSet<Core.Entities.Song> Songs { get; set; }
        public DbSet<Core.Entities.User> Users { get; set; }
        
        
    }
}