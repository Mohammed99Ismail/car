using System.ComponentModel.DataAnnotations.Schema;
using ClinicAPI.Models;
using ClinicAPI.Shared.Model.Interfaces;

namespace ClinicAPI.Shared.Model
{
    public abstract class AuditedEntityBase : IAuditedEntityBase
    {
        [ForeignKey(nameof(CreatedByUser))]
        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public DateTime CreatedDate { get; set; }
        
        [ForeignKey(nameof(LastModifiedByUser))]
        public int? LastModifiedByUserId { get; set; }
        public User? LastModifiedByUser { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
