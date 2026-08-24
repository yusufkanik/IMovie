using MovieAPI.Data;
using MovieAPI.DTOs;
using MovieAPI.Models;
using MovieAPI.Exceptions;

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
                throw new NotFoundException($"{listId}'li liste bulunamadı.");
            }
        }
    }
}
