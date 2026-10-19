using System;

namespace ClinicAPI.Shared.Model.Interfaces
{
    public interface IAuditedEntityBase
    {
        int? CreatedByUserId { get; set; }
        DateTime CreatedDate { get; set; }
        int? LastModifiedByUserId { get; set; }
        DateTime LastModifiedDate { get; set; }
    }
}
