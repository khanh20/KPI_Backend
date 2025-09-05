using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class CreateKpiAssignmentDto
    {
        public int UserId { get; set; }      // giao cho ai
        public int UnitId { get; set; }      // thuộc đơn vị nào
        public int KpiItemId { get; set; }   // KPI nào
        public float TargetValue { get; set; }
        public float ContributionWeight { get; set; }
        public int Year { get; set; }
    }
}
