using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class AssignTemplateDto
    {
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int TemplateId { get; set; }
        public int Year { get; set; }
        public float? DefaultContributionWeight { get; set; }
    }
}
