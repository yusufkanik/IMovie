using MovieAPI.DTOs;

namespace MovieAPI.Services
{
    public interface IMovieService
    {
        Task<SyncResultDTO> SyncPopularMoviesAsync(int pageCount);
        Task<PagedResponseDTO<MovieResponseDTO>> GetMoviesAsync(GetMoviesQueryDTO request);
        Task<MovieResponseDTO> GetMovieByIdAsync(int id);
    }
}
