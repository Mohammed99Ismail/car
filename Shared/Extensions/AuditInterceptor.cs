using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClinicAPI.Shared.Extensions;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetAuditFields(DbContext? context)
    {
        if (context == null) return;

        NormalizeDateTimesToUtc(context);

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var userId = _httpContextAccessor.HttpContext?.User.GetId();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType();

            var hasCreatedBy = entityType.GetProperty("CreatedByUserId") != null;
            var hasCreatedDate = entityType.GetProperty("CreatedDate") != null;
            var hasLastModifiedBy = entityType.GetProperty("LastModifiedByUserId") != null;
            var hasLastModifiedDate = entityType.GetProperty("LastModifiedDate") != null;

            // Handle Added entities
            if (entry.State == EntityState.Added)
            {
                if (hasCreatedDate)
                    entry.Property("CreatedDate").CurrentValue = now;

                if (hasCreatedBy && userId != null)
                    entry.Property("CreatedByUserId").CurrentValue = userId;

                if (hasLastModifiedDate)
                    entry.Property("LastModifiedDate").CurrentValue = now;

                if (hasLastModifiedBy && userId != null)
                    entry.Property("LastModifiedByUserId").CurrentValue = userId;
            }

            // Handle Modified entities
            else if (entry.State == EntityState.Modified)
            {
                if (hasLastModifiedDate)
                    entry.Property("LastModifiedDate").CurrentValue = now;

                if (hasLastModifiedBy && userId != null)
                    entry.Property("LastModifiedByUserId").CurrentValue = userId;
            }
        }
    }

    private void NormalizeDateTimesToUtc(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added && entry.State != EntityState.Modified)
                continue;

            foreach (var property in entry.Properties)
            {
                // DateTime
                if (property.Metadata.ClrType == typeof(DateTime) &&
                    property.CurrentValue is DateTime dt)
                {
                    property.CurrentValue = ConvertToUtc(dt);
                }

                // Nullable DateTime
                else if (property.Metadata.ClrType == typeof(DateTime?) &&
                        property.CurrentValue is DateTime ndt)
                {
                    property.CurrentValue = ConvertToUtc(ndt);
                }
            }
        }
    }

    private static DateTime ConvertToUtc(DateTime dateTime)
    {
        return dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            _ => dateTime
        };
    }

}
