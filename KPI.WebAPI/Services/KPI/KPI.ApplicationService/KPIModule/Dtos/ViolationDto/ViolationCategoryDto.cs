using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ViolationDto
{
    public class ViolationCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float TargetValue { get; set; }
        public string? CalculationFormula { get; set; }
        public List<ViolationLevelDto> Levels { get; set; }

    }
}
