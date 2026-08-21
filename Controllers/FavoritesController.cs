using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Services;
using MovieAPI.DTOs.ResponseDTOs;

namespace MovieAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieResponseDTO>>> GetFavorites()
        {
            int id = GetCurrentUserId();
            var result = await _favoriteService.GetUserFavoritesAsync(id);

            return Ok(result);
        }

        [HttpPost("{movieId:int}")]
        public async Task<IActionResult> AddFavorite(int movieId)
        {
            int userId = GetCurrentUserId();
            await _favoriteService.AddToFavoritesAsync(userId, movieId);

            return Ok(new {message = "Film favorilere eklendi."});
        }

        [HttpDelete("{movieId:int}")]
        public async Task<IActionResult> RemoveFavorite(int movieId)
        {
            int userId = GetCurrentUserId();
            await _favoriteService.RemoveFromFavoritesAsync(userId, movieId);

            return Ok(new { message = "Film favorilerden silindi." });
        }
    }
}
