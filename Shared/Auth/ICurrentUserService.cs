namespace ClinicAPI.Shared.Auth;

public interface ICurrentUserService
{
  string UserId { get; }
  string FirstName { get; }
  string LastName { get; }
  string Email { get; }
  string MobileNumber { get; }
  List<string> RolesIds { get; }
  List<KeyValuePair<string, string>> Claims { get; }
  
  /// <summary>
  /// Gets the first role ID from the user's roles. Returns null if no roles are found.
  /// </summary>
  /// <returns>The first role ID as an integer, or null if no roles exist</returns>
  int? GetFirstRoleId();
  
  /// <summary>
  /// Gets all role IDs as integers. Returns empty list if no roles are found or if parsing fails.
  /// </summary>
  /// <returns>List of role IDs as integers</returns>
  List<int> GetRoleIds();
}

