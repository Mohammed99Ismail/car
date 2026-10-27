namespace CarShowroom.Application.Common.Caching;

/// <summary>Cache keys and tags (pattern adapted from Shared/Constants/CacheKey).</summary>
public static class CacheKey
{
    public const string CarMakes = "lookups:car-makes";
    public const string CarModelsPrefix = "lookups:car-models:";
    public const string CarColors = "lookups:car-colors";
    public const string ContractTypes = "lookups:contract-types";
    public const string Branches = "lookups:branches";
    public const string Roles = "lookups:roles";
    public const string Permissions = "lookups:permissions";
    public const string DashboardStats = "dashboard:stats";

    public static string CarModels(int makeId) => $"{CarModelsPrefix}{makeId}";
}
