using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.DTOs.AuthDTOs;
using MovieAPI.Models;
using MovieAPI.Exceptions;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace MovieAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }


        public async Task<AuthResponseDto> RegisterAsync(RegisterDto request)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new BadRequestException("Bu e-posta adresi zaten kullanılıyor.");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return await GenerateAuthResponseAsync(user);
        }


        public async Task<AuthResponseDto> LoginAsync(LoginDto request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user != null)
            {

                if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    throw new BadRequestException("Kullanıcı adı veya şifre yanlış.");
                }

                return await GenerateAuthResponseAsync(user);
            }

            throw new BadRequestException("Kullanıcı adı veya şifre yanlış.");
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }


        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var secretKey = _configuration["JwtSettings:Secret"] 
                ?? throw new InvalidOperationException("JWT Secret Key tanımlanmamış!");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpireInMinutes"] ?? "60")),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException("Refresh Token bulunamadı.");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Geçersiz Refresh Token");
            }

            if (user.TokenExpires < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh Token süresi doldu. Tekrar giriş yapın.");
            }

            return await GenerateAuthResponseAsync(user);

        }

        public async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
        {
            var accessToken = CreateToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.TokenCreated = DateTime.UtcNow;
            user.TokenExpires = DateTime.UtcNow.AddDays(7);

            await _dbContext.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Username = user.UserName,
                Email = user.Email,
                Role = user.Role,
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task LogoutAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);

            if (user != null)
            {
                // Token bilgilerini temizliyoruz
                user.RefreshToken = string.Empty;
                user.TokenExpires = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
