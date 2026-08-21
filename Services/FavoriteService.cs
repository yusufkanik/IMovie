using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Exceptions;
using MovieAPI.Models;
using MovieAPI.DTOs.ResponseDTOs;
using MovieAPI.Extensions;

namespace MovieAPI.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _context;

        public FavoriteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToFavoritesAsync(int userId, int movieId)
        {
            var movieExists = await _context.Movies.AnyAsync(m => m.Id == movieId);

            if (!movieExists) { throw new NotFoundException("Eklenecek film bulunamadı."); }

            var alreadyFavorite = await _context.UserFavoriteMovies.AnyAsync(f => f.UserId == userId && f.MovieId == movieId);

            if (alreadyFavorite) { throw new BadRequestException("Bu film zaten favorilerde ekli."); }

            var UserFavorite = new UserFavoriteMovie
            {
                MovieId = movieId,
                UserId = userId,
            };

            _context.UserFavoriteMovies.Add(UserFavorite);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromFavoritesAsync(int userId, int movieId)
        {

            var favorite = await _context.UserFavoriteMovies.FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == movieId);

            if (favorite is null) { throw new NotFoundException("Favori kaydı bulunamadı."); }

            _context.UserFavoriteMovies.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MovieResponseDTO>> GetUserFavoritesAsync(int userId)
        {
            return await _context.UserFavoriteMovies
                .Where(f => f.UserId == userId)
                .Select(f => f.Movie.ToResponseDto())
                .ToListAsync();
        }
    }
}
