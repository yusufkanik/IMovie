namespace MovieAPI.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfilePath { get; set; }

        public ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
        public ICollection<MovieDirector> MovieDirectors { get; set; } = new List<MovieDirector>();
    }
}
