using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DTOs.ReviewDTOs;
using MovieAPI.Services;
using System.Security.Claims;

namespace MovieAPI.Controllers
{
    [Route("api/movies/{movieId}/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(int movieId)
        {
            var result = await _reviewService.GetMovieReviewsAsync(movieId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int movieId, [FromBody] CreateReviewDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _reviewService.AddReviewAsync(userId, movieId, dto);

            return Ok(new {Message = "Yorum başarıyla eklendi."});
        }

        [HttpPut]
        [Authorize]

        public async Task<IActionResult> UpdateReview(int movieId, [FromBody] CreateReviewDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _reviewService.UpdateMovieReviewAsync(userId, movieId, dto);

            return Ok(new { Message = "Yorum başarıyla güncellendi." });
        }

        [HttpDelete]
        [Authorize]

        public async Task<IActionResult> DeleteReview (int movieId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _reviewService.DeleteMovieReviewAsync(userId, movieId);

            return Ok(new { Message = "Yorum başarıyla silindi." });
        }
    }
}
