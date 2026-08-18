namespace MovieAPI.DTOs
{
    public class MovieResponseDTO
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public double Rating { get; set; }
        public int VoteCount { get; set; }

    }
}
