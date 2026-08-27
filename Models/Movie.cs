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
        public int Runtime {  get; set; }
        public long Budget { get; set; }
        public long Revenue { get; set; }
        public string? TrailerUrl { get; set; }

        public double LocalVoteAverage { get; set; } = 0.0;
        public int LocalVoteCount { get; set; } = 0;

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
        public ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
        public ICollection<MovieDirector> MovieDirectors { get; set; } = new List<MovieDirector>();
    }
}

