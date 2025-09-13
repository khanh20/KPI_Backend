using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class KpiTypeScoreResultDto
    {
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int Year { get; set; }
        public List<KpiTypeScoreDto> ScoresByType { get; set; }
        public float FinishTotal { get; set; }
    }
}
