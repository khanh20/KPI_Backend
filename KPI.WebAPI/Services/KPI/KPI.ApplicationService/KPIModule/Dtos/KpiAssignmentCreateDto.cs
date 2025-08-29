using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos
{
    public class KpiAssignmentCreateDto
    {
        public int KpiItemId { get; set; }   // KPI nào
        public int AssignedToUserId { get; set; } // Giao cho ai
        public int? AssignedToUnitId { get; set; } // Hoặc giao cho đơn vị
        public DateTime Deadline { get; set; }
    }
}
