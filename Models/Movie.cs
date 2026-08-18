namespace MovieAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public string? ReleaseDate { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    }
}
