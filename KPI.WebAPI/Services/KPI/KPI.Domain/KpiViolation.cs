using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table("KpiViolation")]
    public class KpiViolation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required, MaxLength(100)]
        public string ViolationType { get; set; } = null!;
        public int ViolationCount { get; set; }
        public float DeductionScore { get; set; }
        public DateTime ViolationDate { get; set; }
    }
}
