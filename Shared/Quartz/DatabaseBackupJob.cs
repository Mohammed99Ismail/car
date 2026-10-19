using Quartz;
using ClinicAPI.Shared.Services;

namespace ClinicAPI.Shared.Quartz;

[DisallowConcurrentExecution]
public class DatabaseBackupJob : IJob
{
    private readonly IDatabaseBackupService _backupService;
    private readonly IBackupCleanupService _cleanupService;
    private readonly ILogger<DatabaseBackupJob> _logger;

    const string TaskName = nameof(DatabaseBackupJob);

    public DatabaseBackupJob(
        IDatabaseBackupService backupService,
        IBackupCleanupService cleanupService,
        ILogger<DatabaseBackupJob> logger)
    {
        _backupService = backupService;
        _cleanupService = cleanupService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("{Task} Service is starting.", TaskName);
        var cancellationToken = context.CancellationToken;

        try
        {
            _logger.LogError("{Task} starting database backup process...", TaskName);
            _logger.LogInformation("{Task} starting database backup process...", TaskName);

            // Create the backup
            var backupSuccess = await _backupService.CreateBackupAsync(cancellationToken);

            if (backupSuccess)
            {
                _logger.LogInformation("{Task} database backup completed successfully.", TaskName);

                // Clean up old backups after successful backup
                _logger.LogInformation("{Task} starting cleanup of old backup files...", TaskName);
                await _cleanupService.CleanupOldBackupsAsync(cancellationToken);

                // Log backup statistics
                var fileCount = await _cleanupService.GetBackupFileCountAsync();
                var totalSize = await _cleanupService.GetTotalBackupSizeAsync();
                
                _logger.LogInformation("{Task} backup statistics - Files: {FileCount}, Total Size: {TotalSize} bytes", 
                    TaskName, fileCount, totalSize);
            }
            else
            {
                _logger.LogError("{Task} database backup failed.", TaskName);
            }

            _logger.LogInformation("{Task} completed.", TaskName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Task} has exception: {Exception}", TaskName, ex.Message);
        }
    }
}
