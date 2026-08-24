namespace MovieAPI.DTOs.ReviewDTOs
{
    public record ReviewResponseDto(
    int Id,
    int Rating,
    string Comment,
    string UserEmail,
    DateTime CreatedAt
    );
}
