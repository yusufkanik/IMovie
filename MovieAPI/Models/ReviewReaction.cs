namespace MovieAPI.Models
{
    public class ReviewReaction
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ReviewId { get; set; }
        public Review Review { get; set; } = null!;

        public bool IsLike { get; set; } // true: Like, false: Dislike
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
