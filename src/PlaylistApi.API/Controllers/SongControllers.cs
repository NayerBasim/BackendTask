using Microsoft.AspNetCore.Mvc;
using PlaylistApi.Core.Interfaces;

namespace PlaylistApi.API.Controllers
{
    [ApiController]
    [Route("api/songs")]
    public class SongControllers: ControllerBase
    {
        private readonly ISongRepository _songRepository;

        public SongControllers(ISongRepository songRepository)
        {
            _songRepository = songRepository;
        }

    [HttpGet]
    public async Task<IActionResult> GetAvailableSongs()
    {
        var songs = await _songRepository.GetAllSongsAsync();
        if (songs == null) return NotFound();
       return Ok(songs);
    }


    }
}