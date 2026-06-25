using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlaylistApi.Core.Entities;
using PlaylistApi.Core.Interfaces;

namespace PlaylistApi.EF.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly AppDbContext _context;
        public SongRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Song>?> GetAllSongsAsync()
        {
            return await _context.Songs.ToListAsync(); 
        }
    }
}