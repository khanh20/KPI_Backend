using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table ("KpiViolationCore")]
    public class KpiViolationCore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public float TotalDeduction { get;set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
