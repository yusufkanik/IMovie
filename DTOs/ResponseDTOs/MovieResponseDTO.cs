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

        public double LocalVoteAverage {  get; set; }
        public int LocalVoteCount { get; set; }

        public List<string> Genres { get; set; } = new();

    }
    public class MovieDetailsDto : MovieResponseDTO
    {
        public int Runtime { get; set; }
        public long Budget { get; set; }
        public long Revenue { get; set; }
        public string? TrailerUrl { get; set; }

        public List<PersonDto> Directors { get; set; } = new();
        public List<CastDto> Cast { get; set; } = new();
    }

    public record PersonDto(
        int PersonId,
        string Name,
        string? ProfilePath
    );

    public record CastDto(
        int PersonId,
        string Name,
        string Character,
        string? ProfilePath
    );
}
