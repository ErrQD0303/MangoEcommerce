using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Extensions.Claims;
using API.Interfaces.Services;
using API.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services.Tokens
{
    public class TokenService<TUser> : IUserTwoFactorTokenProvider<TUser>
    where TUser : IdentityUser
    {
        private IConfiguration _config;
        private SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"] ?? throw new NullReferenceException("No signing key found!")));
        }

        public async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return (await GenerateAsync("login", manager, user)) != null;
        }

        public async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            // Create Claims to attach to response every time a new user is created
            var claims = new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            };

            var roles = await manager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create the encryptor which will be used to encrypt the claims with the Signing Key (Private key) before sending back to the user with the specified algorithm
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Create a new Token based on new signing credential above in X509 format
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMonths(1),
                NotBefore = DateTime.Now,
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
            };

            // Create a new JwtSecurityTokenHandler for creating user's new token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create a new Security Token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            if (string.IsNullOrEmpty(token)) return false;

            if (token.StartsWith("Bearer"))
                token = token.Substring(7);

            SecurityToken? securityToken;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    IssuerSigningKey = _key,
                    ValidAudience = _config["JWT:Audience"],
                    ValidIssuer = _config["JWT:Issuer"]
                    // ClockSkew = TimeSpan.Zero,
                };

                // Validate the token with token validation parameters, and output the security token if success
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                if (securityToken == null) return false;

                var tokenObject = tokenHandler.ReadJwtToken(token);

                var claimUsername = claimsPrincipal.GetUsername();
                var claimEmail = claimsPrincipal.GetEmail();
                var claimRoles = claimsPrincipal.GetRoles();

                var userRoles = await manager.GetRolesAsync(user);

                return !string.Equals(claimUsername, user.UserName, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(claimEmail, user.Email, StringComparison.OrdinalIgnoreCase) ||
                    claimRoles.Any(r => !userRoles.Any(ur => ur.Equals(r, StringComparison.OrdinalIgnoreCase)));

                /* // Create a new JwtSecurityToken based on the string token
                var tokenObject = tokenHandler.ReadJwtToken(token);
                return tokenObject.Claims ?? new List<Claim>(); */
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch
            {
                throw;
            }
        }
    }
}