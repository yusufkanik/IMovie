using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Extensions;
using MovieAPI.Models;
using MovieAPI.Services;
using System.Security.Claims;
using MovieAPI.DTOs.ResponseDTOs;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserMovieService _service;

        public UsersController(UserMovieService service)
        {
            _service = service;
        }

        [HttpGet("me/movies/{movieId}/status")]
        public async Task<IActionResult> GetWatchStatus(int movieId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var status = await _service.GetWatchStatusAsync(userId, movieId);

            return Ok(new { status });
        }

        [HttpGet("me/stats")]
        public async Task<IActionResult> GetMyStats()
        {
            int userId = User.GetUserId();
            var result = await _service.GetUserStatsAsync(userId);

            return Ok(result);
        }

        [HttpPut("me/movies/{movieId}")]
        public async Task<IActionResult> SetWatchStatus(int movieId, [FromBody] UpdateWatchStatusDto dto)
        {
            int userId = User.GetUserId();
            await _service.SetWatchStatusAsync(userId, movieId, dto.status);

            return Ok(new { message = "Film izleme durumu başarıyla güncellendi." });
        }

        [HttpGet("me/movies/{status}")]
        public async Task<IActionResult> GetMyMoviesByStatus(WatchStatus status)
        {
            int userId = User.GetUserId();
            var result = await _service.GetUserMoviesByStatusAsync(userId, status);

            return Ok(result);
        }
    }


}
