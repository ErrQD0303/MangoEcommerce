using System.Security.Claims;
using API.Models.Accounts;

namespace API.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
        IEnumerable<Claim>? VerifyToken(string token);
    }
}