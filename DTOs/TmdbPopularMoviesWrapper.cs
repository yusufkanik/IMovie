using System.Text.Json.Serialization;

namespace MovieAPI.DTOs;
// wrapper class to handle the popular movies request to TMDB API (separate the page number and Movie results)
public class TmdbPopularMoviesWrapper
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<TmdbMovieDto> Results { get; set; } = new();
}
