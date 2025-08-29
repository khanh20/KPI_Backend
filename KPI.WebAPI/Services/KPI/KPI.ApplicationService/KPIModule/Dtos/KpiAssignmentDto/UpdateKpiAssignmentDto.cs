using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class UpdateKpiAssignmentDto
    {
        public float TargetValue { get; set; }
        public float ContributionWeight { get; set; }
        public string Status { get; set; } = null!;
    }
}
