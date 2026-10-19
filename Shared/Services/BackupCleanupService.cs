using Microsoft.Extensions.Options;
using ClinicAPI.Shared.Model;

namespace ClinicAPI.Shared.Services;

public interface IBackupCleanupService
{
    Task CleanupOldBackupsAsync(CancellationToken cancellationToken = default);
    Task<int> GetBackupFileCountAsync();
    Task<long> GetTotalBackupSizeAsync();
}

public class BackupCleanupService : IBackupCleanupService
{
    private readonly BackupConfiguration _backupConfig;
    private readonly ILogger<BackupCleanupService> _logger;

    public BackupCleanupService(IOptions<BackupConfiguration> backupConfig, ILogger<BackupCleanupService> logger)
    {
        _backupConfig = backupConfig.Value;
        _logger = logger;
    }

    public async Task CleanupOldBackupsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting cleanup of old backup files. Retention period: {RetentionDays} days", 
                _backupConfig.RetentionDays);

            if (!Directory.Exists(_backupConfig.BackupPath))
            {
                _logger.LogWarning("Backup directory does not exist: {BackupPath}", _backupConfig.BackupPath);
                return;
            }

            var cutoffDate = DateTime.Now.AddDays(-_backupConfig.RetentionDays);
            var deletedCount = 0;
            var totalSizeDeleted = 0L;

            // Get all backup files matching the prefix pattern (.sql and .dump files)
            var backupFiles = Directory.GetFiles(_backupConfig.BackupPath, $"{_backupConfig.BackupPrefix}_*")
                .Where(file => file.EndsWith(".sql") || file.EndsWith(".dump"))
                .Select(file => new FileInfo(file))
                .Where(file => file.CreationTime < cutoffDate)
                .OrderBy(file => file.CreationTime)
                .ToList();

            _logger.LogInformation("Found {FileCount} backup files older than {CutoffDate}", 
                backupFiles.Count, cutoffDate.ToString("yyyy-MM-dd HH:mm:ss"));

            foreach (var file in backupFiles)
            {
                try
                {
                    var fileSize = file.Length;
                    file.Delete();
                    deletedCount++;
                    totalSizeDeleted += fileSize;

                    _logger.LogDebug("Deleted old backup file: {FileName} (Size: {Size} bytes, Created: {CreatedDate})", 
                        file.Name, fileSize, file.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete backup file: {FileName}", file.Name);
                }
            }

            _logger.LogInformation("Cleanup completed. Deleted {DeletedCount} files, freed {TotalSize} bytes", 
                deletedCount, totalSizeDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during backup cleanup");
        }
    }

    public async Task<int> GetBackupFileCountAsync()
    {
        try
        {
            if (!Directory.Exists(_backupConfig.BackupPath))
                return 0;

            var backupFiles = Directory.GetFiles(_backupConfig.BackupPath, $"{_backupConfig.BackupPrefix}_*")
                .Where(file => file.EndsWith(".sql") || file.EndsWith(".dump"));
            return backupFiles.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while counting backup files");
            return 0;
        }
    }

    public async Task<long> GetTotalBackupSizeAsync()
    {
        try
        {
            if (!Directory.Exists(_backupConfig.BackupPath))
                return 0;

            var backupFiles = Directory.GetFiles(_backupConfig.BackupPath, $"{_backupConfig.BackupPrefix}_*")
                .Where(file => file.EndsWith(".sql") || file.EndsWith(".dump"));
            return backupFiles.Sum(file => new FileInfo(file).Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while calculating total backup size");
            return 0;
        }
    }
}
