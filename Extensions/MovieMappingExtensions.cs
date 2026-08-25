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
                Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList()
            };
        }

        public static IEnumerable<MovieResponseDTO> ToResponseDTOList(this IEnumerable<Movie> movies)
        {
            return movies.Select(m => ToResponseDto(m)).ToList();
        }

        public static MovieResponseDTO ToResponseDto(this TmdbMovieDto dto)
        {
            return new MovieResponseDTO
            {
                Id = 0, // DB kaydı henüz olmadığı için
                TmdbId = dto.Id,
                Title = dto.Title ?? string.Empty,
                Overview = dto.Overview ?? string.Empty,
                PosterUrl = dto.PosterPath,
                Rating = dto.VoteAverage,
                VoteCount = dto.VoteCount
            };
        }

        public static IEnumerable<MovieResponseDTO> ToResponseDTOList(this IEnumerable<TmdbMovieDto> dtos)
        {
            return dtos.Select(dto => dto.ToResponseDto());
        }
    }
}
