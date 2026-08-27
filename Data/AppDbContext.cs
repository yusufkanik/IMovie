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
        public DbSet<UserMovieStatus> UserMovieStatuses { get; set; }
        public DbSet<CustomList> CustomLists { get; set; }
        public DbSet<CustomListMovie> CustomListMovies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<ReviewReaction> ReviewReactions { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<MovieCast> MovieCasts { get; set; }
        public DbSet<MovieDirector> MovieDirectors { get; set; }

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

            modelBuilder.Entity<UserMovieStatus>()
                .HasKey(ums => new { ums.UserId, ums.MovieId });

            modelBuilder.Entity<CustomListMovie>()
                .HasKey(clm => new { clm.CustomListId, clm.MovieId });

            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId);

            // TMDB Sabit Tür Listesi (Seed Data)
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 28, Name = "Action" },
                new Genre { Id = 12, Name = "Adventure" },
                new Genre { Id = 16, Name = "Animation" },
                new Genre { Id = 35, Name = "Comedy" },
                new Genre { Id = 80, Name = "Crime" },
                new Genre { Id = 99, Name = "Documentary" },
                new Genre { Id = 18, Name = "Drama" },
                new Genre { Id = 10751, Name = "Family" },
                new Genre { Id = 14, Name = "Fantasy" },
                new Genre { Id = 36, Name = "History" },
                new Genre { Id = 27, Name = "Horror" },
                new Genre { Id = 10402, Name = "Music" },
                new Genre { Id = 9648, Name = "Mystery" },
                new Genre { Id = 10749, Name = "Romance" },
                new Genre { Id = 878, Name = "Science Fiction" },
                new Genre { Id = 10770, Name = "TV Movie" },
                new Genre { Id = 53, Name = "Thriller" },
                new Genre { Id = 10752, Name = "War" },
                new Genre { Id = 37, Name = "Western" }
            );

            modelBuilder.Entity<ReviewReaction>()
                .HasKey(r => new { r.UserId, r.ReviewId });

            modelBuilder.Entity<ReviewReaction>()
                .HasOne(r => r.Review)
                .WithMany(rev => rev.Reactions)
                .HasForeignKey(r => r.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<MovieCast>()
                .HasKey(mc => new { mc.MovieId, mc.PersonId });

            modelBuilder.Entity<MovieCast>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.MovieCasts)
                .HasForeignKey(mc => mc.MovieId);

            modelBuilder.Entity<MovieCast>()
                .HasOne(mc => mc.Person)
                .WithMany(p => p.MovieCasts)
                .HasForeignKey(mc => mc.PersonId);

            // MovieDirector - Composite Primary Key
            modelBuilder.Entity<MovieDirector>()
                .HasKey(md => new { md.MovieId, md.PersonId });

            modelBuilder.Entity<MovieDirector>()
                .HasOne(md => md.Movie)
                .WithMany(m => m.MovieDirectors)
                .HasForeignKey(md => md.MovieId);

            modelBuilder.Entity<MovieDirector>()
                .HasOne(md => md.Person)
                .WithMany(p => p.MovieDirectors)
                .HasForeignKey(md => md.PersonId);
        }
    }

}
