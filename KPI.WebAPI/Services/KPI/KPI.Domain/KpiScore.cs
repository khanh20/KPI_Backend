using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table("KpiScore")]
    public class KpiScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int Year { get; set; }
        public float TotalFunctionalScore {  get; set; }
        public float TotalObjectiveScore { get; set; }
        public float TotalComplianceScore { get; set; }
        public float FinalScore { get; set; }

        public int Status { get; set; }
    }
}
