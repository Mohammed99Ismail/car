using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Infrastructure.Options;
using CarShowroom.Infrastructure.Persistence;
using CarShowroom.Infrastructure.Persistence.Interceptors;
using CarShowroom.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarShowroom.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        // System TimeProvider
        services.AddSingleton(TimeProvider.System);

        // Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IFileStorage, FileStorage>();

        // Interceptors
        services.AddScoped<AuditInterceptor>();

        // DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=car_showroom_db;Username=postgres;Password=123456;";

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            })
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(auditInterceptor)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
