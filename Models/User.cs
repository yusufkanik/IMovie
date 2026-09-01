namespace MovieAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set;} = Array.Empty<byte>();
        public string Role { get; set; } = "User";

        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Navigation property
        public ICollection<UserFavoriteMovie> FavoriteMovies { get; set; } = new List<UserFavoriteMovie>();
    }
}
