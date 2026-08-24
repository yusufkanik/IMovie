using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;
namespace MovieAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<User> Users { get; set; }
        public DbSet<UserFavoriteMovie> UserFavoriteMovies { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserFavoriteMovie Composite Primary Key
            modelBuilder.Entity<UserFavoriteMovie>()
                .HasKey(ufm => new { ufm.UserId, ufm.MovieId });

            // User -> UserFavoriteMovie
            modelBuilder.Entity<UserFavoriteMovie>()
                .HasOne(ufm => ufm.User)
                .WithMany(u => u.FavoriteMovies)
                .HasForeignKey(ufm => ufm.UserId);

            // Movie -> UserFavoriteMovie
            modelBuilder.Entity<UserFavoriteMovie>()
                .HasOne(ufm => ufm.Movie)
                .WithMany()
                .HasForeignKey(ufm => ufm.MovieId);

            modelBuilder.Entity<Review>()
            .ToTable("Reviews");

            // Bir kullanıcı aynı filme sadece 1 yorum yapabilir
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.MovieId })
                .IsUnique();
        }
    }

}
