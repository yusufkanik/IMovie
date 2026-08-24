using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using MovieAPI.Data;
using MovieAPI.DTOs;
using MovieAPI.DTOs.ResponseDTOs;
using MovieAPI.Exceptions;
using MovieAPI.Extensions;
using MovieAPI.Services;
using System.Security.Claims;
using System.Text;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly UserMovieService _userMovieService;

        public MoviesController(IMovieService movieService, UserMovieService userMovieService)
        {
            _movieService = movieService;
            _userMovieService = userMovieService;
        }

        // Syncronize the movies from TMDB if exists in the database update, else just add to db

        [Authorize(Roles = "Admin")]
        [HttpPost("sync-popular")]
        public async Task<IActionResult> SyncPopularMovies([FromQuery] int pageCount = 5)
        {
            var result = await _movieService.SyncPopularMoviesAsync(pageCount);
            return Ok(result);
        }

        [HttpGet]  // Get endpoint using query parameters
        public async Task<ActionResult<PagedResponseDTO<MovieResponseDTO>>> GetMovies([FromQuery] GetMoviesQueryDTO request)

        {
            var result = await _movieService.GetMoviesAsync(request);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var result = await _movieService.GetMovieByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("tmdb/search")]
        public async Task<ActionResult<PagedResponseDTO<MovieResponseDTO>>> SearchTmdb([FromQuery] string query, [FromQuery] int page = 1)
        {
            var result = await _movieService.SearchTmdbMoviesAsync(query, page);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("sync/{tmdbId:int}")]
        public async Task<ActionResult<MovieResponseDTO>> SyncSingleMovie(int tmdbId)
        {
            var result = await _movieService.SyncSingleMovieAsync(tmdbId);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{movieId:int}/status")]

        public async Task<IActionResult> SetWatchStatus(int movieId, [FromBody] UpdateWatchStatusDto dto)    // a function from UserMovieService will be used here
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _userMovieService.SetWatchStatusAsync(userId, movieId, dto.status);

            return Ok(new {Message = "İzleme durumu güncellendi."});
        }
    }
}
