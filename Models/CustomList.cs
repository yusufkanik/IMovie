namespace MovieAPI.Models
{
    public class CustomList
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User = null!;

        public ICollection<CustomListMovie> CustomListMovies { get; set; } = new List<CustomListMovie>(); 

    }
}
