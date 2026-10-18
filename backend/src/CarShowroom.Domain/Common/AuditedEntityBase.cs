namespace CarShowroom.Domain.Common;

/// <summary>
/// Entities implementing this interface get their audit fields filled automatically by the AuditInterceptor.
/// </summary>
public interface IAuditedEntity
{
    int? CreatedByUserId { get; set; }
    DateTime CreatedDate { get; set; }
    int? LastModifiedByUserId { get; set; }
    DateTime LastModifiedDate { get; set; }
}

public abstract class AuditedEntityBase : BaseEntity, IAuditedEntity
{
    public int? CreatedByUserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? LastModifiedByUserId { get; set; }
    public DateTime LastModifiedDate { get; set; }
}
