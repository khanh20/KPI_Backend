using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ViolationDto
{
    public class UserViolationSummaryDto
    {
        public int UnitId { get; set; }
        public int TotalViolation { get; set; }
        public float TotalDeduction { get; set; }
    }
}

