using MovieAPI.DTOs.ResponseDTOs;
using MovieAPI.DTOs.TmdbDTOs;
using MovieAPI.Models;
using System.Collections;

namespace MovieAPI.Extensions
{
    public static class MovieMappingExtensions
    {
        // convert the DTO object to Movie object
        public static Movie ToModel(this TmdbMovieDto dto)
        {
            var targetGenreIds = dto.GenreIds.Count > 0
                    ? dto.GenreIds
                    : dto.Genres?.Select(g => g.Id).ToList() ?? new List<int>();

            return new Movie
            {
                TmdbId = dto.Id,
                Title = dto.Title,
                Overview = dto.Overview,
                PosterPath = dto.PosterPath,
                ReleaseDate = dto.ReleaseDate,
                VoteAverage = dto.VoteAverage,
                VoteCount = dto.VoteCount,
                MovieGenres = targetGenreIds.Select(genreId => new MovieGenre
                {
                    GenreId = genreId
                }).ToList()
            };
        }

        // convert a list of DTOs to a list of Movie objects
        public static List<Movie> ToModelList(this IEnumerable<TmdbMovieDto> dtos)
        {
            return dtos.Select(dto => dto.ToModel()).ToList();
        }

        // DTOs for sending the response to the frontend

        public static MovieResponseDTO ToResponseDto(this Movie movie)
        {
            return new MovieResponseDTO
            {
                Id = movie.Id,
                TmdbId = movie.TmdbId,
                Title = movie.Title,
                Overview = movie.Overview,
                PosterUrl = string.IsNullOrEmpty(movie.PosterPath) ?
                    null : $"https://image.tmdb.org/t/p/w500{movie.PosterPath}",
                Rating = movie.VoteAverage,
                VoteCount = movie.VoteCount,
                Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                LocalVoteAverage = movie.LocalVoteAverage,
                LocalVoteCount = movie.LocalVoteCount,
            };
        }

        public static IEnumerable<MovieResponseDTO> ToResponseDTOList(this IEnumerable<Movie> movies)
        {
            return movies.Select(m => ToResponseDto(m)).ToList();
        }

        public static MovieResponseDTO ToResponseDto(
                this TmdbMovieDto dto,
                Dictionary<int, Movie>? existingMoviesLookup = null,
                Dictionary<int, string>? genreLookup = null)
        {

            Movie? localMovie = null;
            if (existingMoviesLookup != null && existingMoviesLookup.TryGetValue(dto.Id, out var foundMovie))
            {
                localMovie = foundMovie;
            }

            return new MovieResponseDTO
            {

                Id = localMovie?.Id ?? 0,
                TmdbId = dto.Id,
                Title = dto.Title ?? string.Empty,
                Overview = dto.Overview ?? string.Empty,
                PosterUrl = string.IsNullOrEmpty(dto.PosterPath)
                    ? null
                    : $"https://image.tmdb.org/t/p/w500{dto.PosterPath}",
                Rating = dto.VoteAverage,
                VoteCount = dto.VoteCount,


                LocalVoteAverage = localMovie?.LocalVoteAverage ?? 0.0,
                LocalVoteCount = localMovie?.LocalVoteCount ?? 0,


                Genres = dto.GenreIds != null && genreLookup != null
                    ? dto.GenreIds
                        .Where(id => genreLookup.ContainsKey(id))
                        .Select(id => genreLookup[id])
                        .ToList()
                    : new List<string>()
            };
        }

        public static IEnumerable<MovieResponseDTO> ToResponseDTOList(this IEnumerable<TmdbMovieDto> dtos)
        {
            return dtos.Select(dto => dto.ToResponseDto());
        }

        public static MovieDetailsDto ToDetailsDto(this Movie movie)
        {
            return new MovieDetailsDto
            {
                Id = movie.Id,
                TmdbId = movie.TmdbId,
                Title = movie.Title,
                Overview = movie.Overview,
                PosterUrl = string.IsNullOrEmpty(movie.PosterPath)
                    ? null
                    : $"https://image.tmdb.org/t/p/w500{movie.PosterPath}",
                Rating = movie.VoteAverage,
                VoteCount = movie.VoteCount,
                LocalVoteAverage = movie.LocalVoteAverage,
                LocalVoteCount = movie.LocalVoteCount,
                Runtime = movie.Runtime,
                Budget = movie.Budget,
                Revenue = movie.Revenue,
                TrailerUrl = movie.TrailerUrl,
                Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),

                Directors = movie.MovieDirectors
                    .Where(md => md.Person != null)
                    .Select(md => new PersonDto(
                        md.Person.Id,
                        md.Person.Name,
                        string.IsNullOrEmpty(md.Person.ProfilePath)
                            ? null
                            : $"https://image.tmdb.org/t/p/w500{md.Person.ProfilePath}"
                    ))
                    .ToList(),

                Cast = movie.MovieCasts
                    .Where(mc => mc.Person != null)
                    .OrderBy(mc => mc.DisplayOrder)
                    .Select(mc => new CastDto(
                        mc.Person.Id,
                        mc.Person.Name,
                        mc.CharacterName,
                        string.IsNullOrEmpty(mc.Person.ProfilePath)
                            ? null
                            : $"https://image.tmdb.org/t/p/w500{mc.Person.ProfilePath}"
                    ))
                    .ToList()
            };
        }
    }
}
