using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Exceptions;
using MovieAPI.Models;
using MovieAPI.DTOs.ResponseDTOs;
using MovieAPI.Extensions;

namespace MovieAPI.Services
{
    public class UserMovieService
    {
        private readonly AppDbContext _context;

        public UserMovieService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SetWatchStatusAsync(int userId, int movieId, WatchStatus watchStatus)
        {
            var movieExists = await _context.Movies.AnyAsync(m => m.Id == movieId);
            if (!movieExists) throw new NotFoundException("Film bulunamadı.");

            var userStatus = await _context.UserMovieStatuses.FirstOrDefaultAsync(ums => ums.UserId == userId && ums.MovieId == movieId);

            if (userStatus == null)
            {
                userStatus = new UserMovieStatus
                {
                    UserId = userId,
                    MovieId = movieId,
                    status = watchStatus,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.UserMovieStatuses.Add(userStatus);
            }
            else
            {
                userStatus.status = watchStatus;
                userStatus.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UserStatsDto> GetUserStatsAsync(int userId)
        {

            var userStatuses = await _context.UserMovieStatuses
                .Where(usm => usm.UserId == userId)
                .Select(usm => usm.status)
                .ToListAsync();

            var totalMoviesWatched = userStatuses.Count(s => s == WatchStatus.Watched);
            var totalPlanToWatch = userStatuses.Count(s => s == WatchStatus.PlanToWatch);
            var totalDropped = userStatuses.Count(s => s == WatchStatus.Dropped);

            var averageRatingGiven = await _context.Reviews
                .Where(r => r.UserId == userId)
                .Select(r => (double?)r.Rating)
                .AverageAsync() ?? 0.0;

            return new UserStatsDto
            (
                totalMoviesWatched,
                totalPlanToWatch,
                totalDropped,
                Math.Round(averageRatingGiven, 1) 
            );
        }

        public async Task<List<MovieResponseDTO>> GetUserMoviesByStatusAsync(int userId, WatchStatus status)
        {
           return await _context.UserMovieStatuses
                            .Include(ums => ums.Movie)
                            .ThenInclude(m => m.MovieGenres)
                            .ThenInclude(mg => mg.Genre)
                            .Where(ums => ums.UserId == userId && ums.status == status)
                            .AsNoTracking()
                            .Select(ums => ums.Movie.ToResponseDto())
                            .ToListAsync();
        }
    }
}
