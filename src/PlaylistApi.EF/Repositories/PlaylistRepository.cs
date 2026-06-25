using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlaylistApi.src.PlaylistApi.Core.Entities;
using PlaylistApi.src.PlaylistApi.Core.Interfaces;
using PlaylistApi.src.PlaylistApi.EF;

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

        public async Task<Playlist?> UpdatePlaylistAsync(Playlist playlist)
        {
            _context.Playlists.Update(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }
        }
    }
