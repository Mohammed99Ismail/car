using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CarShowroom.Api.Auth;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private readonly ConcurrentDictionary<string, Lazy<AuthorizationPolicy>> _policies = new();

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PermissionAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return await base.GetPolicyAsync(policyName);
        }

        var lazyPolicy = _policies.GetOrAdd(policyName, static name => new Lazy<AuthorizationPolicy>(() =>
        {
            var op = PermissionAuthorizeAttribute.GetOperatorFromPolicy(name);
            var permissions = PermissionAuthorizeAttribute.GetPermissionsFromPolicy(name);
            var requirement = new PermissionRequirement(op, permissions);

            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(requirement)
                .Build();
        }, LazyThreadSafetyMode.ExecutionAndPublication));

        return lazyPolicy.Value;
    }
}
