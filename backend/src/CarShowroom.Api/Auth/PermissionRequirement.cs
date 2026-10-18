using CarShowroom.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;

namespace CarShowroom.Api.Auth;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionOperatorEnum PermissionOperator { get; }
    public string[] Permissions { get; }

    public PermissionRequirement(PermissionOperatorEnum permissionOperator, string[] permissions)
    {
        if (permissions.Length == 0)
            throw new ArgumentException("At least one permission is required.", nameof(permissions));

        PermissionOperator = permissionOperator;
        Permissions = permissions;
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = context.User;

        // 1. SuperAdmin role or SuperAdmin permission bypasses all specific checks
        var isSuperAdmin = user.HasClaim(c =>
            (c.Type == CustomClaimTypes.RoleId && c.Value == SystemRoles.SuperAdminId.ToString()) ||
            (c.Type == CustomClaimTypes.Permissions && c.Value == PermissionKeysConstant.SuperAdmin.SUPER_ADMIN_KEY));

        if (isSuperAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 2. Evaluate permissions
        var userPermissions = user.FindAll(CustomClaimTypes.Permissions).Select(c => c.Value).ToHashSet();

        if (requirement.PermissionOperator == PermissionOperatorEnum.And)
        {
            var hasAll = requirement.Permissions.All(p => userPermissions.Contains(p));
            if (hasAll)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, "Missing required permissions."));
            }
            return Task.CompletedTask;
        }

        // Or operator
        var hasAny = requirement.Permissions.Any(p => userPermissions.Contains(p));
        if (hasAny)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "Missing required permissions."));
        }

        return Task.CompletedTask;
    }
}
