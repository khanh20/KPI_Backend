using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ViolationDto
{
    public class ViolationLevelDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public float MaxDeduction { get; set; }
        public int ViolationCount { get; set; }
        public string? Description { get; set; }
    }
}
