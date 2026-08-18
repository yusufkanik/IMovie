using System.Text.Json.Serialization;

namespace MovieAPI.DTOs;

public class TmdbPopularMoviesWrapper
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<TmdbMovieDto> Results { get; set; } = new();
}
