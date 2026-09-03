namespace MovieAPI.Models
{
    public class CustomListMovie
    {
        public int CustomListId { get; set; }
        public CustomList CustomList { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
