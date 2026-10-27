namespace CarShowroom.Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "CarShowroomApi";
    public string Audience { get; set; } = "CarShowroomClient";
    /// <summary>
    /// JWT signing key. Must be provided via configuration or the JWT__SecretKey
    /// environment variable — never hardcoded. Validated at startup in Program.cs.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenLifetimeMinutes { get; set; } = 60;
    public int RefreshTokenLifetimeDays { get; set; } = 14;
}

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromEmail { get; set; } = "noreply@carshowroom.com";
    public string FromName { get; set; } = "Car Showroom System";
    public bool EnableSsl { get; set; } = true;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(FromEmail);
}
