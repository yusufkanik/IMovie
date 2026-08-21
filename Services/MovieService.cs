using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.DTOs;
using MovieAPI.DTOs.ResponseDTOs;
using MovieAPI.Exceptions;
using MovieAPI.Extensions;

namespace MovieAPI.Services
{
    public class MovieService : IMovieService
    {
        private readonly AppDbContext _context;
        private readonly TmdbService _tmdbService;

        public MovieService(AppDbContext context, TmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        public async Task<SyncResultDTO> SyncPopularMoviesAsync(int PageCount)
        {
            int totalAdded = 0;
            int totalUpdated = 0;

            for (int page = 1; page <= PageCount; page++)
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
                        movie.LastUpdated = DateTime.UtcNow;
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
                        existing.LastUpdated = DateTime.UtcNow;
                        totalUpdated++;
                    }
                }
            }
            await _context.SaveChangesAsync();

            return new SyncResultDTO(
                PageCount,
                totalAdded,
                totalUpdated,
                $"{PageCount} sayfa işlendi. {totalAdded} yeni film eklendi. {totalUpdated} film güncellendi."
                );
        }

        public async Task<PagedResponseDTO<MovieResponseDTO>> GetMoviesAsync(GetMoviesQueryDTO request)
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
                    "date" => query.OrderByDescending(m => string.IsNullOrEmpty(m.ReleaseDate) ? "0000-00-00" : m.ReleaseDate),
                    "votecount" => query.OrderByDescending(m => m.VoteCount),
                    _ => query.OrderByDescending(m => m.Id)
                };
            }

            var totalRecords = await query.CountAsync();

            var movies = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = movies.ToResponseDTOList();

            return new PagedResponseDTO<MovieResponseDTO>(dtos, request.Page, request.PageSize, totalRecords);

        }

        public async Task<MovieResponseDTO> GetMovieByIdAsync (int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie is null)
            {
                throw new NotFoundException($"{id} ID'li film veritabanında bulunamadı.");
            }
            return movie.ToResponseDto();
        }

        public async Task<PagedResponseDTO<MovieResponseDTO>> SearchTmdbMoviesAsync(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new BadRequestException("Arama kelimesi boş olamaz.");
            }

            var response = await _tmdbService.SearchMoviesAsync(query, page);

            if (!response.IsSuccess || response.Data?.Results is null)
            {
                throw new InvalidOperationException(response.Message ?? "TMDB servisinden yanıt alınamadı.");
            }

            var dtos = response.Data.Results.ToResponseDTOList();
            const int tmdbPageSize = 20;

            return new PagedResponseDTO<MovieResponseDTO>(
                dtos,
                response.Data.Page,
                tmdbPageSize,
                response.Data.TotalResults
            );

        }

        public async Task<MovieResponseDTO> SyncSingleMovieAsync(int tmdbId)
        {
            var existingMovie = await _context.Movies.FirstOrDefaultAsync(m => m.TmdbId == tmdbId);
            var tmdbResponse = await _tmdbService.GetMovieByTmdbIdAsync(tmdbId);

            if (!tmdbResponse.IsSuccess)
            {
                throw new InvalidOperationException($"TMDB Bağlantı Hatası: {tmdbResponse.Message}");
            }

            
            if (tmdbResponse.Data is null)
            {
                throw new NotFoundException($"TMDB üzerinde {tmdbId} ID'li film bulunamadı.");
            }

            if (existingMovie != null)
            {
                if (tmdbResponse.IsSuccess && tmdbResponse.Data != null)
                {
                    var updatedData = tmdbResponse.Data.ToModel();

                    existingMovie.Title = updatedData.Title;
                    existingMovie.Overview = updatedData.Overview;
                    existingMovie.PosterPath = updatedData.PosterPath;
                    existingMovie.ReleaseDate = updatedData.ReleaseDate;
                    existingMovie.VoteAverage = updatedData.VoteAverage;
                    existingMovie.VoteCount = updatedData.VoteCount;
                    existingMovie.LastUpdated = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }
                return existingMovie.ToResponseDto();
            }

            

            var dto = tmdbResponse.Data;

            var movie = dto.ToModel();
            movie.LastUpdated = DateTime.UtcNow;

            _context.Movies.Add( movie );

            await _context.SaveChangesAsync();

            return movie.ToResponseDto();
        }

    }
}
