using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class KpiAssignmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int KpiItemId { get; set; }
        public float TargetValue { get; set; }
        public float ContributionWeight { get; set; }
        public float ActualResults { get; set; }
        public float ComponentScore { get; set; }
        public string Status { get; set; } = null!;
        public int Year { get; set; }
    }
}
