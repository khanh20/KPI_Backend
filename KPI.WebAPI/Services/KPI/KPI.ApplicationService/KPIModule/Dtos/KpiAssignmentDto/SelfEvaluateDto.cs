using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class SelfEvaluateDto
    {
        public int AssignmentId { get; set; }
        public float ActualResults { get; set; }
    }
}