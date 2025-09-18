using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ViolationDto
{
    public class UnitSumViolationDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public float AverageDeduction { get; set; } // trung bình deduction score
        public float TotalComponent { get; set; }   // điểm sau khi áp dụng công thức
    }

    public class UnitViolationSummaryResultDto
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public List<UnitSumViolationDto> Details { get; set; }
        public float TotalDeduction { get; set; }   // tổng điểm tất cả category
    }

}
