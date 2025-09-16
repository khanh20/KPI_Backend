using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table ("ViolationCategory")]
    public class KpiViolationCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string? CalculationFormula { get; set; }
        public float TargetValue { get; set; }
    }
}
