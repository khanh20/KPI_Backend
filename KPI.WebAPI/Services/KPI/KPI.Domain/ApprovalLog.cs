using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table("ApprovalLogs")]
    public class ApprovalLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int KpiAssignmentId { get; set; }

        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Action { get; set; } = null!;

        [MaxLength(1024)]
        public string? Comment { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
