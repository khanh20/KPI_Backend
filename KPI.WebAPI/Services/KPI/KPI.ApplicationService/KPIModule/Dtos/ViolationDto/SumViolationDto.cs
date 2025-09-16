using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ViolationDto
{
    public class SumViolationDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }     
        public float TotalComponent { get; set; }
    }
}
