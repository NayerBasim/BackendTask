using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlaylistApi.Core.Entities;
using PlaylistApi.Core.Interfaces;
using PlaylistApi.EF;


namespace PlaylistApi.EF.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly AppDbContext _context;
        public PlaylistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Playlist?> AddPlaylistAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<List<Playlist>?> GetAllPlaylistsAsync()
        {
            return await _context.Playlists.ToListAsync();
        }

        public async Task<Playlist?> GetPlaylistByIdAsync(Guid id)
        {
            return await _context.Playlists
                .Include(p => p.Songs)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<Playlist?> DeletePlaylistAsync(Guid id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Songs)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (playlist == null) return null;

            // Songs are a shared catalog — orphan them (set PlaylistId = null) instead of
            // deleting. Clearing the navigation makes EF null the FK before the delete,
            // which satisfies SQLite's foreign-key constraint.
            playlist.Songs.Clear();

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<Playlist?> UpdatePlaylistAsync(Guid id, Playlist playlist)
        {


            var existingPlaylist = await _context.Playlists.FindAsync(id);
            if (existingPlaylist == null) return null;

            existingPlaylist.Name = playlist.Name;
            existingPlaylist.Description = playlist.Description;
            // Songs are managed through the add/remove-song endpoints, not on update.

            await _context.SaveChangesAsync();
            return existingPlaylist;
        }

        public async Task<Playlist?> AddSongToPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Songs)
                .FirstOrDefaultAsync(p => p.Id == playlistId);
            if (playlist == null) return null;

            var song = await _context.Songs.FindAsync(songId);
            if (song == null) return null;

            // Link the existing catalog song to this playlist (no-op if already linked).
            if (!playlist.Songs.Any(s => s.Id == songId))
            {
                playlist.Songs.Add(song);
                await _context.SaveChangesAsync();
            }

            return playlist;
        }

        public async Task<Playlist?> RemoveSongFromPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Songs)
                .FirstOrDefaultAsync(p => p.Id == playlistId);
            if (playlist == null) return null;

            var songToRemove = playlist.Songs.FirstOrDefault(s => s.Id == songId);
            if (songToRemove == null) return null;

            playlist.Songs.Remove(songToRemove);
            await _context.SaveChangesAsync();
            return playlist;
        }

    }
}
