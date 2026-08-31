using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.DTOs;
using MovieAPI.DTOs.ResponseDTOs;
using MovieAPI.Exceptions;
using MovieAPI.Extensions;
using MovieAPI.Models;

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
                    var existing = await _context.Movies
                                                .Include(m => m.MovieGenres)
                                                .FirstOrDefaultAsync(m => m.TmdbId == movie.TmdbId);

                    if (existing is null)
                    {
                        var isAlreadyTracked = _context.Movies.Local.Any(m => m.TmdbId == movie.TmdbId);
                        if (isAlreadyTracked)
                        {
                            continue; // Mükerrer kaydı atla
                        }

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

                        _context.MovieGenres.RemoveRange(existing.MovieGenres);
                        existing.MovieGenres.Clear();

                        
                        foreach (var mg in movie.MovieGenres)
                        {
                            existing.MovieGenres.Add(new MovieGenre
                            {
                                MovieId = existing.Id,
                                GenreId = mg.GenreId
                            });
                        }
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
            var query = _context.Movies
                                .Include(m => m.MovieGenres)
                                .ThenInclude(mg => mg.Genre)
                                .AsNoTracking()
                                .AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(m => m.Title.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = request.SortBy?.ToLower() switch
                {
                    "vote" => request.SortOrder == "asc" ? query.OrderBy(m => m.VoteAverage) : query.OrderByDescending(m => m.VoteAverage),
                    "votecount" => request.SortOrder == "asc" ? query.OrderBy(m => m.VoteCount) : query.OrderByDescending(m => m.VoteCount),
                    "date" => request.SortOrder == "asc" ? query.OrderBy(m => m.ReleaseDate) : query.OrderByDescending(m => m.ReleaseDate),
                    _ => query.OrderByDescending(m => m.Id)
                };
            }

            if (request.MinYear.HasValue)
            {
                var minYearStr = request.MinYear.Value.ToString();
                query = query.Where(m => m.ReleaseDate != null && string.Compare(m.ReleaseDate.Substring(0, 4), minYearStr) >= 0);
            }

            if (request.MaxYear.HasValue)
            {
                var maxYearStr = request.MaxYear.Value.ToString();
                query = query.Where(m => m.ReleaseDate != null && string.Compare(m.ReleaseDate.Substring(0, 4), maxYearStr) <= 0);
            }


            if (request.MinRating.HasValue)
            {
                query = query.Where(m => m.VoteAverage >= request.MinRating.Value);
            }
            if (request.MaxRating.HasValue)
            {
                query = query.Where(m => m.VoteAverage <= request.MaxRating.Value);
            }
            if (request.MinVoteCount.HasValue)
            {
                query = query.Where(m => m.VoteCount >= request.MinVoteCount.Value);
            }

            if (request.GenreIds != null && request.GenreIds.Count > 0)
            {
                foreach (var genreId in request.GenreIds)
                {
                    var id = genreId;
                    query = query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == id));
                }
            }

            var totalRecords = await query.CountAsync();

            var movies = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = movies.ToResponseDTOList();

            return new PagedResponseDTO<MovieResponseDTO>(dtos, request.Page, request.PageSize, totalRecords);

        }

        public async Task<MovieDetailsDto> GetMovieByIdAsync (int id)
        {
            var movie = await _context.Movies
                                        .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                                        .Include(m => m.MovieDirectors).ThenInclude(md => md.Person)
                                        .Include(m => m.MovieCasts).ThenInclude(mc => mc.Person)
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (movie is null)
            {
                throw new NotFoundException($"{id} ID'li film veritabanında bulunamadı.");
            }
            return movie.ToDetailsDto();
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

            var tmdbResults = response.Data.Results;
            var tmdbIds = tmdbResults.Select(r => r.Id).ToList();

            
            var existingMoviesLookup = await _context.Movies
                .Where(m => tmdbIds.Contains(m.TmdbId))
                .ToDictionaryAsync(m => m.TmdbId);

           
            var genreLookup = await _context.Genres
                .ToDictionaryAsync(g => g.Id, g => g.Name);

            var dtos = tmdbResults
                .Select(dto => dto.ToResponseDto(existingMoviesLookup, genreLookup))
                .ToList();

            const int tmdbPageSize = 20;

            return new PagedResponseDTO<MovieResponseDTO>(
                dtos,
                response.Data.Page,
                tmdbPageSize,
                response.Data.TotalResults
            );
        }

        public async Task<MovieDetailsDto> SyncSingleMovieAsync(int tmdbId)
        {
            var tmdbResponse = await _tmdbService.GetMovieByTmdbIdAsync(tmdbId);
            if (!tmdbResponse.IsSuccess || tmdbResponse.Data is null)
                throw new InvalidOperationException($"TMDB Hatası: {tmdbResponse.Message}");

            var data = tmdbResponse.Data;
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MovieDirectors)
                .Include(m => m.MovieCasts)
                .FirstOrDefaultAsync(m => m.TmdbId == tmdbId) ?? new Movie { TmdbId = tmdbId };

            // 1. Temel + Detay Verileri (Poster & TMDB Oy Değerleri Dahil)
            movie.Title = data.Title;
            movie.Overview = data.Overview;
            movie.PosterPath = data.PosterPath;
            movie.ReleaseDate = data.ReleaseDate;
            movie.VoteAverage = data.VoteAverage;
            movie.VoteCount = data.VoteCount;
            movie.Runtime = data.Runtime;
            movie.Budget = data.Budget;
            movie.Revenue = data.Revenue;
            movie.LastUpdated = DateTime.UtcNow;

            // LocalVoteAverage ve LocalVoteCount alanlarına dokunmuyoruz (mevcut DB değerleri korunur)

            // 2. Fragman (YouTube)
            var trailer = data.Videos?.Results?.FirstOrDefault(v => v.Site == "YouTube" && v.Type == "Trailer");
            movie.TrailerUrl = trailer != null ? $"https://www.youtube.com/watch?v={trailer.Key}" : null;

            // 3. Türler (Genres) Güncellemesi
            _context.MovieGenres.RemoveRange(movie.MovieGenres);
            movie.MovieGenres.Clear();

            if (data.Genres != null && data.Genres.Any())
            {
                var tmdbGenreIds = data.Genres.Select(g => g.Id).ToList();

                // Veritabanımızdaki Genre kayıtlarını TMDB Genre ID'sine (veya Id'ye) göre buluyoruz
                var localGenres = await _context.Genres
                    .Where(g => tmdbGenreIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genre in localGenres)
                {
                    movie.MovieGenres.Add(new MovieGenre
                    {
                        Movie = movie,
                        GenreId = genre.Id
                    });
                }
            }

            // 4. Yönetmenler
            _context.MovieDirectors.RemoveRange(movie.MovieDirectors);
            movie.MovieDirectors.Clear();
            var directors = data.Credits?.Crew?.Where(c => c.Job == "Director").ToList() ?? new();
            foreach (var dir in directors)
            {
                var person = await GetOrCreatePersonAsync(dir.Id, dir.Name, dir.ProfilePath);
                movie.MovieDirectors.Add(new MovieDirector { Person = person });
            }

            // 5. Oyuncular (İlk 10)
            _context.MovieCasts.RemoveRange(movie.MovieCasts);
            movie.MovieCasts.Clear();
            var castList = data.Credits?.Cast?.Take(10).ToList() ?? new();
            for (int i = 0; i < castList.Count; i++)
            {
                var actor = castList[i];
                var person = await GetOrCreatePersonAsync(actor.Id, actor.Name, actor.ProfilePath);
                movie.MovieCasts.Add(new MovieCast { Person = person, CharacterName = actor.Character, DisplayOrder = i + 1 });
            }

            if (movie.Id == 0) _context.Movies.Add(movie);

            await _context.SaveChangesAsync();
            return await GetMovieByIdAsync(movie.Id);
        }

        private async Task<Person> GetOrCreatePersonAsync(int tmdbPersonId, string name, string? profilePath)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id == tmdbPersonId)
                         ?? _context.People.Local.FirstOrDefault(p => p.Id == tmdbPersonId);

            if (person is null)
            {
                person = new Person { Id = tmdbPersonId, Name = name, ProfilePath = profilePath };
                _context.People.Add(person);
            }
            else if (!string.IsNullOrEmpty(profilePath) && string.IsNullOrEmpty(person.ProfilePath))
            {
                person.ProfilePath = profilePath; // Daha önce profilePath null kaydedildiyse güncelle
            }

            return person;
        }

        public async Task<List<MovieResponseDTO>> GetSimilarMoviesAsync(int movieId)
        {
            var targetGenreIds = await _context.MovieGenres
                .Where(mg => mg.MovieId == movieId)
                .Select(mg => mg.GenreId)
                .ToListAsync();

            if (!targetGenreIds.Any()) return new List<MovieResponseDTO>();

            var similarMovies = await _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .AsNoTracking()
                .Where(m => m.Id != movieId)
                .Where(m => m.MovieGenres.Any(mg => targetGenreIds.Contains(mg.GenreId)))
                .OrderByDescending(m => m.MovieGenres.Count(mg => targetGenreIds.Contains(mg.GenreId)))
                .ThenByDescending(m => m.VoteAverage)
                .Take(10)
                .ToListAsync();

            return similarMovies.Select(m => m.ToResponseDto()).ToList();
        }

        public async Task<List<MovieResponseDTO>> GetPersonalizedRecommendationsAsync(int userId)
        {
            var favoriteMovieIds = await _context.UserFavoriteMovies
                .Where(usm => usm.UserId == userId)
                .Select(usm => usm.MovieId)
                .ToListAsync();

            if (!favoriteMovieIds.Any())
            {
                return await _context.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .AsNoTracking()
                    .Where(m => m.VoteCount > 1000)
                    .OrderByDescending(m => m.VoteAverage)
                    .Take(10)
                    .Select(m => m.ToResponseDto())
                    .ToListAsync();

            }

            var favoriteGenreIds = await _context.MovieGenres
                .Where(mg => favoriteMovieIds.Contains(mg.MovieId))
                .GroupBy(mg => mg.GenreId)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToListAsync();

            var recommendations = await _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .AsNoTracking()
                .Where(m => !favoriteMovieIds.Contains(m.Id))
                .Where(m => m.MovieGenres.Any(mg => favoriteGenreIds.Contains(mg.GenreId)))
                .OrderByDescending(m => m.VoteAverage)
                .Take(10)
                .ToListAsync();

            return recommendations.Select(m => m.ToResponseDto()).ToList();
        }
    }
}
