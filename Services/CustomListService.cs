using MovieAPI.Data;
using MovieAPI.DTOs;
using MovieAPI.Models;
using MovieAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using MovieAPI.Extensions;

namespace MovieAPI.Services
{
    public class CustomListService
    {
        private readonly AppDbContext _context;

        public CustomListService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateListAsync(int userId, CreateCustomListDto dto)
        {
            var customList = new CustomList
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                IsPublic = dto.IsPublic,

            };

            _context.CustomLists.Add(customList);
            await _context.SaveChangesAsync();
            return customList.Id;
        }

        public async Task AddMovieToListAsync(int userId, int listId, int movieId)
        { 
            var list = await _context.CustomLists.FindAsync(listId);

            if (list == null)
            {
                throw new NotFoundException($"{listId} idli liste bulunamadı.");
            }

            if (list.UserId != userId) throw new UnauthorizedAccessException("Bu liste üzerinde yetkiniz yok.");

            var movie = await _context.Movies.AnyAsync(m => m.Id == movieId);

            if (!movie)
            {
                throw new NotFoundException($"{movieId} idli film bulunamadı.");
            }

            var movieAlreadyExists = await _context.CustomListMovies.AnyAsync(csm => csm.MovieId == movieId && csm.CustomListId == listId);

            if (movieAlreadyExists)
            {
                throw new BadRequestException("Film zaten eklemek istediğiniz listede mevcut.");
            }

            var newMovie = new CustomListMovie
            {
                CustomListId = listId,
                MovieId = movieId,
            };

            _context.CustomListMovies.Add(newMovie);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMovieFromListAsync(int userId, int listId, int movieId)
        {
            var list = await _context.CustomLists.FindAsync(listId);

            if (list == null)
            {
                throw new NotFoundException($"{listId} idli liste bulunamadı.");
            }

            if (list.UserId != userId) throw new UnauthorizedAccessException("Bu liste üzerinde yetkiniz yok.");


            var movieInList = await _context.CustomListMovies.FirstOrDefaultAsync(csm => csm.MovieId == movieId && csm.CustomListId == listId);

            if (movieInList is null)
            {
                throw new BadRequestException("Verilen film zaten listede bulunmuyor.");
            }

            _context.CustomListMovies.Remove(movieInList);
            await _context.SaveChangesAsync();
        }


        public async Task<List<CustomListSummaryDto>> GetUserListsAsync(int userId)
        {

            return await _context.CustomLists
                    .Where(c => c.UserId == userId)
                    .Select(c => new CustomListSummaryDto
                    (
                        c.Id,
                        c.Title,
                        c.Description,
                        c.IsPublic,
                        c.CustomListMovies.Count(),
                        c.CreatedAt
                    ))
                    .ToListAsync();
        }

        public async Task<CustomListDetailDto> GetListByIdAsync(int listId, int userId)
        {
            var list = await _context.CustomLists
                                     .Include(l => l.User)
                                     .FirstOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                throw new NotFoundException($"{listId} idli liste bulunamadı.");
            }

            if (!list.IsPublic && list.UserId !=  userId)
            {
                throw new UnauthorizedAccessException("Bu özel bir listedir, görüntüleme yetkiniz yok.");
            }

            var movies = await _context.CustomListMovies
                                       .Where(csm => csm.CustomListId == listId)
                                       .Select(csm => csm.Movie.ToResponseDto())
                                       .ToListAsync();

            return new CustomListDetailDto(

                    list.Id,
                    list.Title,
                    list.Description,
                    list.IsPublic,
                    list.User.Email,
                    movies,
                    list.CreatedAt,
                    movies.Count
                );
        }
    }
}
