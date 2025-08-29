using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos
{
    public class KpiAssignmentDto
    {
        public int Id { get; set; }
        public int KpiItemId { get; set; }
        public int AssignedByUserId { get; set; }
        public int AssignedToUserId { get; set; }
        public int? AssignedToUnitId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
    }
}
