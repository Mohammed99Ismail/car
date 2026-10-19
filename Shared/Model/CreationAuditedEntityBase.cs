using System.ComponentModel.DataAnnotations.Schema;
using ClinicAPI.Models;

namespace ClinicAPI.Shared.Model
{
    public class CreationAuditedEntityBase
    {
        [ForeignKey(nameof(CreatedByUser))]
        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
