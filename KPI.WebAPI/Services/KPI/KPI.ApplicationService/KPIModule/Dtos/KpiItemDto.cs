using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos
{
    public class KpiItemDto
    {
        public int Id { get; set; }
        public string KpiName { get; set; }
        public string KpiType { get; set; }
        public float Weight { get; set; }
        public string? CalculationFormula { get; set; }
        public int KpiTemplateId { get; set; }
        public DateTime DeadLine { get; set; }
    }
}
