using ClinicAPI.Shared.Constants;
using ClinicAPI.Shared.Enums;
using System.Security.Claims;

namespace ClinicAPI.Shared.Auth;

public static class CurrentUserClaimsExtensions
{
  public static long? GetUserId(this IServiceProvider serviceProvider)
  {
    var context = serviceProvider.GetService<IHttpContextAccessor>();
    Claim? claim = context?.HttpContext?.User?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

    if (claim == null)
      return null;

    return long.TryParse(claim.Value, out var userId) ? userId : (long?)null;
  }

  public static string? GetClientId(this ClaimsPrincipal? identity)
  {
    Claim? claim = identity?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

    return claim?.Value;
  }

  public static bool HasSuperAdminRole(this ClaimsPrincipal? identity)
  {
    //if (identity?.Claims == null || !identity.Claims.Any(c => c.Type == ClaimTypes.Role))
    if (identity?.Claims == null || !identity.Claims.Any(c => c.Type == CustomClaimTypes.RoleId))
      return false;

    foreach (var claim in identity.Claims.Where(c => c.Type == CustomClaimTypes.RoleId))
    {
      if (int.TryParse(claim.Value, out int roleId) && roleId == (int)RoleEnum.SysAdmin)
      {
        return true;
      }
    }

    return false;
  }
}
