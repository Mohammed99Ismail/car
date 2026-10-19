using Quartz;

namespace ClinicAPI.Shared.Quartz;

/// <summary>
/// Extension methods for configuring Quartz jobs and triggers
/// </summary>
public static class ServiceCollectionQuartzConfiguratorExtensions
{
  /// <summary>
  /// Configures Quartz with settings from configuration
  /// </summary>
  /// <param name="quartz">The Quartz configurator</param>
  /// <param name="config">The configuration</param>
  public static void ConfigureFromSettings(
      this IServiceCollectionQuartzConfigurator quartz,
      IConfiguration config)
  {
    var quartzSection = config.GetSection("Quartz");

    // Configure scheduler settings
    var schedulerId = quartzSection.GetValue<string>("SchedulerId");
    var schedulerName = quartzSection.GetValue<string>("SchedulerName");
    var misfireThreshold = quartzSection.GetValue<int?>("MisfireThresholdInMinutes");

    if (!string.IsNullOrEmpty(schedulerId) && schedulerId != "AUTO")
    {
      quartz.UseInMemoryStore();
      quartz.UseDefaultThreadPool(tp =>
      {
        tp.MaxConcurrency = 10;
      });
    }

    if (!string.IsNullOrEmpty(schedulerName))
    {
      quartz.UseDefaultThreadPool(tp =>
      {
        tp.MaxConcurrency = 10;
      });
    }

    if (misfireThreshold.HasValue)
    {
      quartz.UseDefaultThreadPool(tp =>
      {
        tp.MaxConcurrency = 10;
      });
    }
  }

  /// <summary>
  /// Adds a job and trigger configuration for the specified job type
  /// </summary>
  /// <typeparam name="T">The job type implementing IJob</typeparam>
  /// <param name="quartz">The Quartz configurator</param>
  /// <param name="config">The configuration</param>
  /// <param name="immediateExecution">Whether to execute the job immediately</param>
  public static void AddJobAndTrigger<T>(
      this IServiceCollectionQuartzConfigurator quartz,
      IConfiguration config,
      bool immediateExecution = false)
      where T : IJob
  {
    // Use the name of the IJob as the appsettings.json key
    string jobName = typeof(T).Name;
    var configKey = $"Quartz:Jobs:{jobName}";

    // Register the job as before
    var jobKey = new JobKey(jobName);
    quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

    // Check if StartNow is configured in appsettings
    var startNow = config.GetValue<bool>($"{configKey}:StartNow");

    if (immediateExecution || startNow)
    {
      quartz.AddTrigger(opts => opts
          .ForJob(jobKey)
          .WithIdentity(jobName + "-trigger")
          .StartNow());
    }
    else
    {
      var cronSchedule = config[$"{configKey}:CronSchedule"];
      var intervalInMinutes = config.GetValue<int?>($"{configKey}:IntervalInMinutes");

      // Validation for cron or interval schedule
      if (!string.IsNullOrEmpty(cronSchedule))
      {
        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithCronSchedule(cronSchedule)); // Use cron-based schedule
      }
      else if (intervalInMinutes.HasValue)
      {
        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(intervalInMinutes.Value)
                .RepeatForever())); // Use simple schedule
      }
      else
      {
        throw new Exception($"No valid schedule found for job in configuration at {configKey}");
      }
    }
  }

}
