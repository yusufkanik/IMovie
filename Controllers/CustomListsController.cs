using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DTOs;
using MovieAPI.Extensions;
using MovieAPI.Services;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace MovieAPI.Controllers
{
    [Route("api/lists")]
    [ApiController]
    public class CustomListsController : ControllerBase
    {
        private readonly CustomListService _listService;

        public CustomListsController(CustomListService listService)
        {
            _listService = listService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateList([FromBody] CreateCustomListDto dto)
        {
            var userId = User.GetUserId();

            int listId = await _listService.CreateListAsync(userId, dto);

            return CreatedAtAction(nameof(GetListById), new { listId }, new { id = listId, message = "Liste başarıyla oluşturuldu." });
        }

        [HttpPost("{listId}/movies")]
        [Authorize]

        public async Task<IActionResult> AddMovieToMyList([FromBody] AddMovieToListDto dto, int listId)
        {
            var userId = User.GetUserId();

            await _listService.AddMovieToListAsync(userId, listId, dto.movieId);

            return Ok(new { message = "Film listeye eklendi." });
        }

        [HttpDelete("{listId}/movies/{movieId}")]
        [Authorize]
        public async Task<IActionResult> RemoveMovieFromList(int listId, int movieId)
        {
            var userId = User.GetUserId();
            await _listService.RemoveMovieFromListAsync(userId, listId, movieId);
            return Ok(new { message = "Film listeden çıkarıldı." });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyLists()
        {
            var userId = User.GetUserId();
            var lists = await _listService.GetUserListsAsync(userId);
            return Ok(lists);
        }

        [HttpGet("{listId}")]
        public async Task<IActionResult> GetListById(int listId)
        {
            int? currentUserId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                currentUserId = int.Parse(userIdClaim.Value);
            }

            var list = await _listService.GetListByIdAsync(listId, (int)currentUserId);
            return Ok(list);
        }
    }
}
