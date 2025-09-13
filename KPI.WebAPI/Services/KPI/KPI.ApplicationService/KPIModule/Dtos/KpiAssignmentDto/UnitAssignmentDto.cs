using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class UnitAssignmentDetailsDto
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; } = null!;
        public int UserId { get; set; }
        public int Year { get; set; }

        public List<string> Statuses { get; set; } = new(); // chứa nhiều status
        public List<AssignmentItemDto> KpiItems { get; set; } = new();
    }

}
