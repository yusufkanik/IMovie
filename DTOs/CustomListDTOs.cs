using MovieAPI.DTOs.ResponseDTOs;
using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DTOs
{
    public record CreateCustomListDto(
                [Required, MaxLength(100)] string Title,
                string? Description,
                bool IsPublic = true
       );

    public record AddMovieToListDto(int movieId);

    public record CustomListSummaryDto(
        int Id,
        string Title,
        string? Description,
        bool IsPublic,
        int movieCount,
        DateTime CreatedAt
      );

    public record CustomListDetailDto(
        int Id,
        string Title,
        string? Description,
        bool IsPublic,
        string OwnerEmail,
        List<MovieResponseDTO> Movies,
        DateTime CreatedAt,
        int movieCount
    );
}
