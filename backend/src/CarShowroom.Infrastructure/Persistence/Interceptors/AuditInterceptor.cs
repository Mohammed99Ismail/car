using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CarShowroom.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Fills audit fields and normalizes DateTime values to UTC (adapted from Shared/Extensions/AuditInterceptor).
/// Uses the typed <see cref="IAuditedEntity"/> interface instead of reflection for performance.
/// </summary>
public class AuditInterceptor(ICurrentUserService currentUser, TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetAuditFields(DbContext? context)
    {
        if (context is null) return;

        NormalizeDateTimesToUtc(context);

        var userId = currentUser.UserId;
        var now = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var entry in context.ChangeTracker.Entries<IAuditedEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.CreatedByUserId ??= userId;
                entry.Entity.LastModifiedDate = now;
                entry.Entity.LastModifiedByUserId ??= userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.CreatedDate).IsModified = false;
                entry.Property(e => e.CreatedByUserId).IsModified = false;
                entry.Entity.LastModifiedDate = now;
                entry.Entity.LastModifiedByUserId = userId;
            }
        }
    }

    private static void NormalizeDateTimesToUtc(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is DateTime dt && dt.Kind != DateTimeKind.Utc)
                    property.CurrentValue = ConvertToUtc(dt);
            }
        }
    }

    private static DateTime ConvertToUtc(DateTime dateTime) => dateTime.Kind switch
    {
        DateTimeKind.Local => dateTime.ToUniversalTime(),
        DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
        _ => dateTime
    };
}
