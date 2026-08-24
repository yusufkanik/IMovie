using MovieAPI.Models;

namespace MovieAPI.DTOs.ResponseDTOs
{
    public record UpdateWatchStatusDto (WatchStatus status);
    public record UserStatsDto (
        int TotalWatchedMovies,
        int TotalPlanToWatch,
        int TotalDropped,
        double AverageRatingGiven
     );
}
