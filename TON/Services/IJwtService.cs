using Domain.Entities;
using System.Security.Claims;

namespace TON.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserModel user);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
    }
}
