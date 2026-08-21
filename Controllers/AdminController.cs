using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using MovieAPI.Data;
using MovieAPI.Exceptions;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public AdminController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public class UpdateRoleDto
        {
            public int UserId { get; set; }
            public string Role { get; set; } = "User";
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UpdateRoleDto dto)
        {
            var user = await _dbContext.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }
            user.Role = dto.Role;
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = $"{user.UserName} kullanıcısının rolü {dto.Role} olarak güncellendi." });
        }
    }
}
