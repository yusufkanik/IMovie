using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieAPI.Data;
using MovieAPI.DTOs.ReviewDTOs;
using MovieAPI.Exceptions;
using MovieAPI.Models;

namespace MovieAPI.Services
{
    public class ReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddReviewAsync(int userId, int movieId, CreateReviewDto review)
        {
            var movieExists = await _context.Movies.AnyAsync(m => m.Id == movieId);

            if (!movieExists)
            {
                throw new NotFoundException($"{movieId}'li film bulunamadı.");
            }

            var reviewExists = await _context.Reviews.AnyAsync(r => r.UserId == userId && r.MovieId == movieId);

            if (reviewExists)
            {
                throw new BadRequestException("Bu filme zaten yorum yaptınız.");
            }

            var new_review = new Review
            {
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                MovieId = movieId
            };

            _context.Reviews.Add(new_review);

            await _context.SaveChangesAsync();
        }

        public async Task<List<ReviewResponseDto>> GetMovieReviewsAsync(int movieId)
        {
            return await _context.Reviews
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponseDto
                (
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.User.Email,
                    r.CreatedAt
                )).ToListAsync();
        }

        public async Task UpdateMovieReviewAsync(int userId, int movieId, CreateReviewDto dto)
        {
            var movieExists = await _context.Movies.AnyAsync(m => m.Id == movieId);

            if (!movieExists)
            {
                throw new NotFoundException($"{movieId}'li film bulunamadı.");
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);

            if (review is null)
            {
                throw new BadRequestException($"{movieId}'li filme yorum yapmadınız.");
            }

            review.Comment = dto.Comment;
            review.Rating = dto.Rating;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMovieReviewAsync(int userId, int movieId)
        {
            var movieExists = await _context.Movies.AnyAsync(m => m.Id == movieId);

            if (!movieExists)
            {
                throw new NotFoundException($"{movieId}'li film bulunamadı.");
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);

            if (review is null)
            {
                throw new BadRequestException($"{movieId}'li filme yorum yapmadınız.");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
