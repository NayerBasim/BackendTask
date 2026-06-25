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
            return await _context.Playlists.FindAsync(id);
        }


        public async Task<Playlist?> DeletePlaylistAsync(Guid id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null) return null;

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
            existingPlaylist.Songs = playlist.Songs;

            await _context.SaveChangesAsync();
            return existingPlaylist;
        }
        }
    }
