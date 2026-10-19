using ClinicAPI.Shared.Services;
using Quartz;

namespace ClinicAPI.Shared.Quartz;

[DisallowConcurrentExecution]
public class DailyJob : IJob
{
  private readonly ILogger<DailyJob> _logger;
  private readonly DailyService _dailyService;

  const string TaskName = nameof(DailyJob);

    public DailyJob(ILogger<DailyJob> logger, DailyService dailyService)
    {
        _logger = logger;
        _dailyService = dailyService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
      _logger.LogInformation("{Task} Service is starting.", TaskName);
      try
      {

        _logger.LogInformation("{Task} processing scheduled emails...", TaskName);

        // Simulate email processing
        await _dailyService.RunDailyService();

        _logger.LogInformation("{Task} completed successfully.", TaskName);
      }
      catch (Exception ex)
      {
        _logger.LogError("{Task} has exception {Exception}", TaskName, ex.Message);
      }
    }
}
