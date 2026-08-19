using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Services;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TmdbService _tmdbService;

        public MoviesController(AppDbContext context, TmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        // Syncronize the movies from TMDB if exists in the database update, else just add to db
        [HttpPost("sync-popular")]
        public async Task<IActionResult> SyncPopularMovies([FromQuery] int pageCount = 5)
        {
            int totalAdded = 0;
            int totalUpdated = 0;

            for (int page = 1; page <= pageCount; page++)
            {
                var wrapper = await _tmdbService.GetPopularMoviesAsync(page);

            }
        }
    }
}
