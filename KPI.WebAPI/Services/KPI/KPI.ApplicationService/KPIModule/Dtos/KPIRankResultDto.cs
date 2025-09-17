using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos
{
    public class KpiRankResultDto
    {
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int Year { get; set; }
        public float FinalScore { get; set; }

        public string Rank { get; set; } = null!;
        public string Description { get; set; } = null!;
        public float Coefficient { get; set; }
    }
}
