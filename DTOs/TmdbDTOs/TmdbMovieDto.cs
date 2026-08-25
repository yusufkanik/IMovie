using System.Text.Json.Serialization;
namespace MovieAPI.DTOs.TmdbDTOs

{
    public class TmdbGenreDto   // A DTO for single movie requests made to TMDB
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    // class to receive the movie info from the TMDB API
    public class TmdbMovieDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("original_title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string? PosterPath {  get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("genre_ids")]   // for the list queries from TMDB
        public List<int> GenreIds { get; set; } = new();

        [JsonPropertyName("genres")]    // for the single movie queries from TMDB
        public List<TmdbGenreDto>? Genres { get; set; }
    }
}
