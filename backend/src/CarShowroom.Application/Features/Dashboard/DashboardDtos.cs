namespace CarShowroom.Application.Features.Dashboard;

/// <summary>
/// Aggregated counts shown on the dashboard home page.
/// Returned by a single endpoint instead of four list queries so the client
/// makes one lightweight request and the backend only runs COUNT(*) queries.
/// </summary>
public sealed record DashboardStatsDto(
    int CarsCount,
    int CustomersCount,
    int BranchesCount,
    int ContractTypesCount);
