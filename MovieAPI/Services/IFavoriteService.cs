using MovieAPI.DTOs.ResponseDTOs;
namespace MovieAPI.Services
{
    public interface IFavoriteService
    {
        Task AddToFavoritesAsync(int userId, int movieId);
        Task RemoveFromFavoritesAsync(int userId, int movieId);
        Task<List<MovieResponseDTO>> GetUserFavoritesAsync(int userId);
    }
}
