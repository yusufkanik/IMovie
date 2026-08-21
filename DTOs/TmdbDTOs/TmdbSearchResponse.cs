using System.Text.Json.Serialization;

namespace MovieAPI.DTOs.TmdbDTOs
{
    // class to receive the API response for search query reequests from Tmdb
    public class TmdbSearchResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<TmdbMovieDto> Results { get; set; } = new();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }
}
