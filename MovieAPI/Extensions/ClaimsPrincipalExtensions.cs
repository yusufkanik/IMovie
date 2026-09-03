using System.Security.Claims;

namespace MovieAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            // ClaimTypes.NameIdentifier veya JWT standardı olan "sub" claim'ini arar
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Geçersiz veya bulunamayan kullanıcı kimliği.");
            }

            return userId;
        }
    }
}
