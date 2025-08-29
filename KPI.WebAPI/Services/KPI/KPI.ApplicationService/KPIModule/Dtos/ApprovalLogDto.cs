using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos
{
    public class ApprovalLogDto
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int ActionByUserId { get; set; }
        public string Action { get; set; } // Approved/Rejected
        public string Comment { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
