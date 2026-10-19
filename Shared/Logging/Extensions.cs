using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Serilog.Sinks.SpectreConsole;

namespace ClinicAPI.Shared.Logging;

public static class Extensions
{
  public static IHostBuilder AddCustomSerilog(
      this IHostBuilder builder,
      Action<LoggerConfiguration>? extraConfigure = null)
  {
    return builder.UseSerilog((context, serviceProvider, loggerConfiguration) =>
    {
      extraConfigure?.Invoke(loggerConfiguration);

      // Add enrichers
      loggerConfiguration
              .Enrich.WithSpan()
              .Enrich.WithBaggage()
              .Enrich.WithExceptionDetails()
              .Enrich.WithCorrelationIdHeader()
              .Enrich.FromLogContext();

      // Read configuration from appsettings.json
      loggerConfiguration.ReadFrom.Configuration(context.Configuration);

      // Add console sink for development
      if (context.HostingEnvironment.IsDevelopment())
      {
        loggerConfiguration.WriteTo.SpectreConsole();
      }
    }, writeToProviders: true);
  }
}
