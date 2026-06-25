using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlaylistApi.Core.Entities;

namespace PlaylistApi.EF
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Core.Entities.Playlist> Playlists { get; set; }
        public DbSet<Core.Entities.Song> Songs { get; set; }
        public DbSet<Core.Entities.User> Users { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditTimestamps();
            return base.SaveChanges();
        }

        // Stamps CreatedAt on insert and UpdatedAt on insert/update for auditable entities.
        private void ApplyAuditTimestamps()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }
        }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Song>().HasData(
                new Song { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Title = "3 Daqat",        Artist = "Abu", Album = "3 Daqat", Genre = "Pop", Duration = 210, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Song { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Title = "Tamally Maak",   Artist = "Amr Diab", Album = "Tamally Maak", Genre = "Pop", Duration = 240, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Song { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Title = "Khaysara",       Artist = "Saad Lamjarred", Album = "Khaysara", Genre = "Pop", Duration = 270, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Song { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Title = "Law Bas Tearaf", Artist = "Najwa Karam", Album = "Law Bas Tearaf", Genre = "Pop", Duration = 250, CreatedAt = seedDate, UpdatedAt = seedDate }
            );
        }
    }
}