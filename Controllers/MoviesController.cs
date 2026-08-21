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
using System.Text;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
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
    }
}
