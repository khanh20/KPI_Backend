using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ViolationDto
{
    public class CreateKpiViolationLevelDto
    {
        public int CategoryId { get; set; }
        public float MaxDeduction { get; set; }
        public int ViolationCount { get; set; }
        [MaxLength(400)]
        public string? Description { get; set; }
    }
}
