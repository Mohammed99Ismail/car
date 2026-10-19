using ClinicAPI.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ClinicAPI.Shared.Auth;

public class PermissionRequirement : AuthorizationHandler<PermissionRequirement>, IAuthorizationRequirement
{
  // 1 - The operator
  public PermissionOperatorEnum PermissionOperator { get; }
  // 2 - The list of permissions passed
  public string[] Permissions { get; }

  public static string ClaimType => "permissions";

  public PermissionRequirement(PermissionOperatorEnum permissionOperator, string[] permissions)
  {
    if (permissions.Length == 0)
      throw new ArgumentException("At least one permission is required.", nameof(permissions));

    PermissionOperator = permissionOperator;
    Permissions = permissions;
  }

  /// <summary>
  /// Permisson Handler Check if User Has The required Permission in his Claim
  /// </summary>
  /// <param name="context"></param>
  /// <param name="requirement"></param>
  /// <returns></returns>
  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
  PermissionRequirement requirement)
  {

    //-- If User Has Role SuperAdmin Skip Check Permission 
    if (context.User.HasSuperAdminRole())
    {
      // identity has all required permissions
      context.Succeed(requirement);
      return Task.CompletedTask;
    }

    if (requirement.PermissionOperator == PermissionOperatorEnum.And)
    {
      foreach (var permission in requirement.Permissions)
      {
        if (!context.User.HasClaim(ClaimType, permission))
        {
          context.Fail(new AuthorizationFailureReason(this, $"Access denied - User does not have required permission: {permission}"));
          return Task.CompletedTask;
        }
      }

      // identity has all required permissions
      context.Succeed(requirement);
      return Task.CompletedTask;
    }

    foreach (var permission in requirement.Permissions)
    {
      if (context.User.HasClaim(ClaimType, permission))
      {
        context.Succeed(requirement);

        return Task.CompletedTask;
      }
    }

    // identity does not have any of the required permissions
    context.Fail(new AuthorizationFailureReason(this, "Access denied - User does not have any of the required permissions"));
    return Task.CompletedTask;
  }
}

