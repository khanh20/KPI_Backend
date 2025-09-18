using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ViolationDto
{
    public class ViolationSummaryResultDto
    {
        public int UserId { get; set; } 
        public List<SumViolationDto> Details { get; set; }
        public int TotalDeduction { get; set; }
    }
}
