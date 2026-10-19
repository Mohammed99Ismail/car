using System.Security.Claims;
using ClinicAPI.Shared.Constants;

namespace ClinicAPI.Shared.Auth;

public class CurrentUserService : ICurrentUserService
{
  public CurrentUserService(IHttpContextAccessor httpContextAccessor)
  {
    var user = httpContextAccessor?.HttpContext?.User;

    // Filling user claims with null-coalescing operators to avoid null values
    UserId = user?.FindFirstValue(CustomClaimTypes.Id) ?? string.Empty;
    FirstName = user?.FindFirstValue(CustomClaimTypes.Name) ?? string.Empty;
    LastName = user?.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;
    Email = user?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    MobileNumber = user?.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;

    // Filling role claims safely using CustomClaimTypes.RoleId
    RolesIds = user?.Claims
        .Where(c => c.Type == CustomClaimTypes.RoleId)
        .Select(c => c.Value)
        .ToList() ?? new List<string>();

    // Storing all claims safely
    Claims = user?.Claims
        .Select(item => new KeyValuePair<string, string>(item.Type, item.Value))
        .ToList() ?? new List<KeyValuePair<string, string>>();
  }

  public string UserId { get; }
  public string FirstName { get; }
  public string LastName { get; }
  public string Email { get; }
  public string MobileNumber { get; }
  public List<string> RolesIds { get; }
  public List<KeyValuePair<string, string>> Claims { get; set; }

  /// <summary>
  /// Gets the first role ID from the user's roles. Returns null if no roles are found.
  /// </summary>
  /// <returns>The first role ID as an integer, or null if no roles exist</returns>
  public int? GetFirstRoleId()
  {
    if (RolesIds == null || !RolesIds.Any())
      return null;

    if (int.TryParse(RolesIds.First(), out int roleId))
      return roleId;

    return null;
  }

  /// <summary>
  /// Gets all role IDs as integers. Returns empty list if no roles are found or if parsing fails.
  /// </summary>
  /// <returns>List of role IDs as integers</returns>
  public List<int> GetRoleIds()
  {
    if (RolesIds == null || !RolesIds.Any())
      return new List<int>();

    var roleIds = new List<int>();
    foreach (var roleIdString in RolesIds)
    {
      if (int.TryParse(roleIdString, out int roleId))
        roleIds.Add(roleId);
    }

    return roleIds;
  }
}
