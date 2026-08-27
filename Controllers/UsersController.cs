using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Extensions;
using MovieAPI.Models;
using MovieAPI.Services;
using System.Security.Claims;

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

        [HttpGet("me/stats")]
        public async Task<IActionResult> GetMyStats()
        {
            int userId = User.GetUserId();
            var result = await _service.GetUserStatsAsync(userId);

            return Ok(result);
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
