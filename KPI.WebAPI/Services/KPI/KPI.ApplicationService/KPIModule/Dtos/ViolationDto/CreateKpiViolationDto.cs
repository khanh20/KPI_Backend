using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ViolationDto
{
    public class CreateKpiViolationDto
    {
        public int UserId { get; set; }
        public string ViolationType { get; set; } = null!;
        public int ViolationCount { get; set; }
        public float DeductionScore { get; set; }
        public DateTime ViolationDate { get; set; }
    }
}
