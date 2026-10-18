using System.Security.Claims;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Security;

namespace CarShowroom.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var val = User?.FindFirstValue(CustomClaimTypes.Id)
                ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(val, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(CustomClaimTypes.Email)
        ?? User?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<int> RoleIds
    {
        get
        {
            if (User is null) return [];

            return User.FindAll(c => c.Type == CustomClaimTypes.RoleId || c.Type == ClaimTypes.Role)
                .Select(c => int.TryParse(c.Value, out var id) ? (int?)id : null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();
        }
    }

    public IReadOnlyList<string> Permissions
    {
        get
        {
            if (User is null) return [];
            return User.FindAll(CustomClaimTypes.Permissions).Select(c => c.Value).Distinct().ToList();
        }
    }
}
