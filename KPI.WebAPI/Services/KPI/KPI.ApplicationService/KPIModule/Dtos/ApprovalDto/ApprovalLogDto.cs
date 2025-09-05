using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ApprovalDto
{
    public class ApprovalLogDto
    {
        public int Id { get; set; }
        public int KpiAssignmentId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = null!;
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
