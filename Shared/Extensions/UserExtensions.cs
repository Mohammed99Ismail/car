using System.Security.Claims;
using ClinicAPI.Shared.Constants;

namespace ClinicAPI.Shared.Extensions
{
    public static class UserExtensions
    {
        public static int? GetId(this ClaimsPrincipal identity)
        {
            Claim? claim = identity?.Claims?.Where(c => c.Type == CustomClaimTypes.Id).SingleOrDefault();

            if (claim == null)
                return null;

            return int.Parse(claim.Value);
        }
        
        public static string GetUserName(this ClaimsPrincipal identity)
        {
            Claim? claim = identity?.Claims?.Where(c => c.Type == CustomClaimTypes.Name).SingleOrDefault();

            if (claim == null)
                return string.Empty;

            return claim.Value;
        }

        public static List<int>? GetRolesId(this ClaimsPrincipal identity)
        {
            Claim? claim = identity?.Claims?.Where(c => c.Type == CustomClaimTypes.RoleId).SingleOrDefault();

            if (claim == null)
                return null;

            var roleIds = claim.Value.Split(',')
                            .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                            .Where(id => id.HasValue)
                            .Select(id => id.Value)
                            .ToList();

            return roleIds.Any() ? roleIds : null;
        }

        public static int? GetBranchId(this ClaimsPrincipal identity)
        {
            Claim? claim = identity?.Claims?.Where(c => c.Type == CustomClaimTypes.BranchId).SingleOrDefault();

            if (claim == null)
                return null;

            return int.Parse(claim.Value);
        }
    }

}
