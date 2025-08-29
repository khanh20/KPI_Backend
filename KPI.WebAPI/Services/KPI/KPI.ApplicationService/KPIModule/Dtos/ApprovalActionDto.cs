using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos
{
    public class ApprovalActionDto
    {
        public int AssignmentId { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
    }
}
