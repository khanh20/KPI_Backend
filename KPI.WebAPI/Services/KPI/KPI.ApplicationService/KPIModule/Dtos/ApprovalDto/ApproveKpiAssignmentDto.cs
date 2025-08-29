using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ApprovalDto
{
    public class ApproveKpiAssignmentDto
    {
        public int AssignmentId { get; set; }
        public string Action { get; set; } = null!; // Approve | Reject
        public string? Comment { get; set; }
    }
}
