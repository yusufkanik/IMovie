using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DTOs.ReviewDTOs
{
    public record CreateReviewDto(
    [Range(1, 10, ErrorMessage = "Puan 1 ile 10 arasında olmalıdır.")] int Rating,
    [Required, MaxLength(1000)] string Comment
    );
}
