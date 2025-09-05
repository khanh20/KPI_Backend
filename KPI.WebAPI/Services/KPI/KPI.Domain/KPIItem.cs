using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table("KpiItem")]
    public class KPIItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(256)]
        [Required]
        public string KpiName { get; set; }
        /// <summary>
        /// Functional / Objective / Compliance
        /// </summary>
        [Required, MaxLength(50)]
        public string KpiType { get; set; } = null!;
        [MaxLength(512)]
        public string CalculationFormula { get; set; }
        public int  KpiTemplateId { get; set; }
        public float Weight { get; set; } 
        public DateTime DeadLine { get; set; }

        // Audit
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }

    }
}
