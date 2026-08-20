using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using MovieAPI.Data;
using MovieAPI.DTOs;
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
                    throw new BadRequestException($"TMDB Servis Hatası (Sayfa {page}): {response.Message}");
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

        [HttpGet]  // Get endpoint using query parameters
        public async Task<ActionResult<PagedResponseDTO<MovieResponseDTO>>> GetMovies([FromQuery] GetMoviesQueryDTO request)

        {
            var query = _context.Movies.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(m => m.Title.ToLower().Contains(request.Search.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "vote" => query.OrderByDescending(m => m.VoteAverage),
                    "votecount" => query.OrderByDescending(m => m.VoteCount),
                    "date" => query.OrderByDescending(m => string.IsNullOrEmpty(m.ReleaseDate) ? "0000-00-00": m.ReleaseDate), 
                    _ => query.OrderByDescending(m => m.Id)
                };
            }

            var totalRecords = await query.CountAsync();

            var movies = await query.Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).ToListAsync();

            var dtos = movies.ToResponseDTOList();

            return Ok(new PagedResponseDTO<MovieResponseDTO>(dtos, request.Page, request.PageSize, totalRecords));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMovieById(int id) 
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                throw new NotFoundException($"{id} ID'li film veritabanında bulunamadı.");
            }

            return Ok(movie.ToResponseDto());
        }
    }
}
