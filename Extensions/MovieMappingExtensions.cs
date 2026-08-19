using MovieAPI.DTOs;
using MovieAPI.Models;
using System.Collections;

namespace MovieAPI.Extensions
{
    public static class MovieMappingExtensions
    {
        // convert the DTO object to Movie object
        public static Movie ToModel(this TmdbMovieDto dto)
        {
            return new Movie
            {
                TmdbId = dto.Id,
                Title = dto.Title,
                Overview = dto.Overview,
                PosterPath = dto.PosterPath,
                ReleaseDate = dto.ReleaseDate,
                VoteAverage = dto.VoteAverage,
                VoteCount = dto.VoteCount,
            };
        }

        // convert a list of DTOs to a list of Movie objects
        public static List<Movie> ToModelList(this IEnumerable<TmdbMovieDto> dtos)
        {
            return dtos.Select(dto => dto.ToModel()).ToList();
        }

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
                VoteCount = movie.VoteCount
            };
        }
    }
}
