using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class CreateKpiAssignmentDto
    {
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int KpiItemId { get; set; }
        public float? ContributionWeight { get; set; }
        public int Year { get; set; }
    }
}
