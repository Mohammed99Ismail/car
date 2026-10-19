namespace ClinicAPI.Shared.Model;

public class BackupConfiguration
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string DatabaseName { get; set; } = "Clinic.API";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BackupPath { get; set; } = "/var/www/oliva/backups/";
    public int RetentionDays { get; set; } = 30;
    public bool CompressBackup { get; set; } = true;
    public string BackupPrefix { get; set; } = "ClinicAPI_Backup";
}
