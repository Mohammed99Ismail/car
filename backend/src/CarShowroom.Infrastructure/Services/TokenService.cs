using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Security;
using CarShowroom.Domain.Entities;
using CarShowroom.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CarShowroom.Infrastructure.Services;

public class TokenService(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider) : ITokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_options.RefreshTokenLifetimeDays);

    public AccessToken CreateAccessToken(User user, IReadOnlyCollection<int> roleIds, IReadOnlyCollection<string> permissions)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expires = now.AddMinutes(_options.AccessTokenLifetimeMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(CustomClaimTypes.Id, user.Id.ToString()),
            new(CustomClaimTypes.Name, user.FullName)
        };

        foreach (var roleId in roleIds.Distinct())
        {
            claims.Add(new Claim(CustomClaimTypes.RoleId, roleId.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, roleId.ToString()));
        }

        foreach (var perm in permissions.Distinct())
        {
            claims.Add(new Claim(CustomClaimTypes.Permissions, perm));
        }

        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = creds
        };

        var token = handler.CreateToken(tokenDescriptor);
        return new AccessToken(token, expires);
    }

    public string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
