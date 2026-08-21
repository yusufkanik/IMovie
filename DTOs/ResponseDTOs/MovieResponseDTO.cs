namespace MovieAPI.DTOs.ResponseDTOs
{
    // class we use to send the movie info to frontend
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
