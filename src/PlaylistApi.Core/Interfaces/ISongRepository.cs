using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaylistApi.Core.Entities;

namespace PlaylistApi.Core.Interfaces
{
    public interface ISongRepository
    {
        Task<List<Song>?> GetAllSongsAsync();

        
    }
}