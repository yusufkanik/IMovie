namespace MovieAPI.Models
{
    public class MovieCast
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public string CharacterName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
