using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore

using MovieAPI.Data;
using MovieAPI.Extensions;
using MovieAPI.Services;
using System.Text;

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
                var response = await _tmdbService.GetPopularMoviesAsync(page);

                if (!response.IsSuccess)
                {
                    return StatusCode(response.StatusCode, new
                    {
                        Success = false,
                        Error = response.Message,
                        FailedPage = page
                    });
                }

                var movies = response.Data?.Results?.ToModelList();

                if (movies is null) { continue; }

                foreach (var movie in movies)
                {
                    var existing = await _context.Movies.FirstOrDefaultAsync(m => m.TmdbId == movie.TmdbId);

                    if (existing is null)
                    {
                        _context.Movies.Add(movie);
                        totalAdded++;
                    }
                    else
                    {
                        existing.Title = movie.Title;
                        existing.Overview = movie.Overview;
                        existing.PosterPath = movie.PosterPath;
                        existing.ReleaseDate = movie.ReleaseDate;
                        existing.VoteAverage = movie.VoteAverage;
                        existing.VoteCount = movie.VoteCount;
                        totalUpdated++;
                    }
                }


            }
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"{pageCount} sayfa işlendi. {totalAdded} yeni film eklendi. {totalUpdated} film güncellendi." });
        }

    }
}
