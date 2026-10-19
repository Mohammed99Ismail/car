using ClinicAPI.Features.HR.Services;
using Quartz;

namespace ClinicAPI.Shared.Quartz;

[DisallowConcurrentExecution]
public class MonthlyVacationAccrualJob : IJob
{
    private readonly ILogger<MonthlyVacationAccrualJob> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    const string TaskName = nameof(MonthlyVacationAccrualJob);

    public MonthlyVacationAccrualJob(
        ILogger<MonthlyVacationAccrualJob> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var isEnabled = _configuration.GetValue<bool>("MonthlyVacationAccrualJob:IsEnabled");
        if (!isEnabled)
        {
            _logger.LogInformation("{Task} is disabled via configuration. Skipping.", TaskName);
            return;
        }

        _logger.LogInformation("{Task} starting.", TaskName);

        // Always process the previous month to ensure complete attendance data
        var now = DateTime.UtcNow;
        var targetDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
        int year = targetDate.Year;
        int month = targetDate.Month;

        try
        {
            // IMonthlyVacationAccrualService is scoped — resolve per execution
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMonthlyVacationAccrualService>();

            int processed = await service.CalculateForAllUsersAsync(year, month);
            _logger.LogInformation("{Task} completed. Processed {Count} users for {Year}-{Month:D2}.",
                TaskName, processed, year, month);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Task} failed for {Year}-{Month:D2}.", TaskName, year, month);
        }
    }
}
