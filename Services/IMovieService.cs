using MovieAPI.DTOs;
using MovieAPI.DTOs.ResponseDTOs;

namespace MovieAPI.Services
{
    public interface IMovieService
    {
        Task<SyncResultDTO> SyncPopularMoviesAsync(int pageCount);   // add or update the movies at database 
        Task<PagedResponseDTO<MovieResponseDTO>> GetMoviesAsync(GetMoviesQueryDTO request);  // get movies from our database
        Task<MovieResponseDTO> GetMovieByIdAsync(int id);  // get movie by id from database
        Task<PagedResponseDTO<MovieResponseDTO>> SearchTmdbMoviesAsync(string query, int page = 1); // get movies from TMDB service and list them to user
                                                                                                    // but this does not add the movies to our database because of possible bloat
                                                                                                    // when user queries, this just shows the results from TMDB nothin else.
        Task<MovieResponseDTO> SyncSingleMovieAsync(int tmdbId); // this one adds or update the movie that frontend wants details about
                                                                 // after user sees the query result from above function, they click on one of the movies
                                                                 // that movie is passed to this function to be updated or added to database
    }
}
