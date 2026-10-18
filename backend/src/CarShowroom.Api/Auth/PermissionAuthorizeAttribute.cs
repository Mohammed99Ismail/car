using Microsoft.AspNetCore.Authorization;

namespace CarShowroom.Api.Auth;

/// <summary>
/// Authorizes actions by specific permission keys (adapted from Shared/Auth/PermissionAuthorizeAttribute).
/// Example: [PermissionAuthorize(PermissionKeysConstant.Cars.VIEW_KEY)]
/// </summary>
public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "PERMISSION_";
    private const string Separator = "__";

    public PermissionAuthorizeAttribute(string permission)
    {
        Policy = $"{PolicyPrefix}{(int)PermissionOperatorEnum.And}{Separator}{permission}";
    }

    public PermissionAuthorizeAttribute(PermissionOperatorEnum permissionOperator, params string[] permissions)
    {
        Policy = $"{PolicyPrefix}{(int)permissionOperator}{Separator}{string.Join(Separator, permissions)}";
    }

    public static PermissionOperatorEnum GetOperatorFromPolicy(string policyName)
    {
        var opChar = policyName.Substring(PolicyPrefix.Length, 1);
        return (PermissionOperatorEnum)int.Parse(opChar);
    }

    public static string[] GetPermissionsFromPolicy(string policyName)
    {
        return policyName.Substring(PolicyPrefix.Length + 1 + Separator.Length)
            .Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
    }
}
