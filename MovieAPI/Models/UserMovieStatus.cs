namespace MovieAPI.Models
{
    public class UserMovieStatus
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public WatchStatus status { get; set; }
        public DateTime UpdatedAt = DateTime.UtcNow;
    }
}
