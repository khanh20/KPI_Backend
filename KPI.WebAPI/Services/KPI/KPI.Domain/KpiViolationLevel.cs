using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table ("KpiViolationLevel")]
    public class KpiViolationLevel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public float MaxDeduction { get; set; }
        public int ViolationCount { get; set; }
        [MaxLength(400)]
        public string? Description { get; set; }
        

    }
}
