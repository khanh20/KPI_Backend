using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.ApprovalDto
{
    // Tạo một DTO mới để phê duyệt/từ chối hàng loạt
    public class ApproveKpiAssignmentBulkDto
    {
        public List<int> AssignmentIds { get; set; }
        public string Action { get; set; } // "Approve" hoặc "Reject"
        public string Comment { get; set; }
    }
}
