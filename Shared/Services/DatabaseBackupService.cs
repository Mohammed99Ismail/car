using Microsoft.Extensions.Options;
using ClinicAPI.Shared.Model;
using System.Diagnostics;

namespace ClinicAPI.Shared.Services;

public interface IDatabaseBackupService
{
    Task<bool> CreateBackupAsync(CancellationToken cancellationToken = default);
    string GetBackupFileName();
}

public class DatabaseBackupService : IDatabaseBackupService
{
    private readonly BackupConfiguration _backupConfig;
    private readonly ILogger<DatabaseBackupService> _logger;

    public DatabaseBackupService(IOptions<BackupConfiguration> backupConfig, ILogger<DatabaseBackupService> logger)
    {
        _backupConfig = backupConfig.Value;
        _logger = logger;
    }

    public string GetBackupFileName()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var extension = _backupConfig.CompressBackup ? ".dump" : ".sql";
        return $"{_backupConfig.BackupPrefix}_{timestamp}{extension}";
    }

    public async Task<bool> CreateBackupAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting database backup process for {DatabaseName}", _backupConfig.DatabaseName);

            // Ensure backup directory exists
            if (!Directory.Exists(_backupConfig.BackupPath))
            {
                Directory.CreateDirectory(_backupConfig.BackupPath);
                _logger.LogInformation("Created backup directory: {BackupPath}", _backupConfig.BackupPath);
            }

            var backupFileName = GetBackupFileName();
            var fullBackupPath = Path.Combine(_backupConfig.BackupPath, backupFileName);

            // Build pg_dump command arguments
            var pgDumpArgs = BuildPgDumpArguments(fullBackupPath);

            _logger.LogInformation("Executing pg_dump for {DatabaseName} to {BackupPath}", 
                _backupConfig.DatabaseName, fullBackupPath);

            // Execute pg_dump process
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "pg_dump",
                Arguments = pgDumpArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Set environment variable for password if provided
            if (!string.IsNullOrEmpty(_backupConfig.Password))
            {
                processStartInfo.Environment["PGPASSWORD"] = _backupConfig.Password;
            }

            using var process = new Process { StartInfo = processStartInfo };
            
            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    outputBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for process to complete with timeout
            var completed = await Task.Run(() => process.WaitForExit(300000), cancellationToken); // 5 minutes timeout

            if (!completed)
            {
                process.Kill();
                _logger.LogError("pg_dump process timed out after 5 minutes");
                return false;
            }

            if (process.ExitCode != 0)
            {
                var errorOutput = errorBuilder.ToString();
                _logger.LogError("pg_dump failed with exit code {ExitCode}. Error: {Error}", 
                    process.ExitCode, errorOutput);
                return false;
            }

            // Verify backup file was created
            if (File.Exists(fullBackupPath))
            {
                var fileInfo = new FileInfo(fullBackupPath);
                _logger.LogInformation("Backup completed successfully. File: {BackupPath}, Size: {Size} bytes", 
                    fullBackupPath, fileInfo.Length);
                return true;
            }
            else
            {
                _logger.LogError("Backup file was not created: {BackupPath}", fullBackupPath);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database backup for {DatabaseName}", _backupConfig.DatabaseName);
            return false;
        }
    }

    private string BuildPgDumpArguments(string fullBackupPath)
    {
        var args = new System.Text.StringBuilder();

        // Host
        args.Append($"--host={_backupConfig.Host}");

        // Port
        args.Append($" --port={_backupConfig.Port}");

        // Username
        if (!string.IsNullOrEmpty(_backupConfig.Username))
        {
            args.Append($" --username={_backupConfig.Username}");
        }

        // Database name
        args.Append($" --dbname={_backupConfig.DatabaseName}");

        // Format: custom format (binary) for compressed backups, plain SQL for uncompressed
        if (_backupConfig.CompressBackup)
        {
            args.Append(" --format=custom");
        }
        else
        {
            args.Append(" --format=plain");
        }

        // Verbose output
        args.Append(" --verbose");

        // Output file
        args.Append($" --file=\"{fullBackupPath}\"");

        // Additional options for better backup
        args.Append(" --no-owner --no-acl");

        return args.ToString();
    }
}
