using MovieAPI.DTOs.AuthDTOs;

namespace MovieAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto request);
        Task<AuthResponseDto> LoginAsync(LoginDto request);

        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);
    }
}
