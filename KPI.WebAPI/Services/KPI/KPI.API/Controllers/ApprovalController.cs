using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos.ApprovalDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "HieuTruong")] // chỉ hiệu trưởng được duyệt
    [Authorize]
    public class ApprovalController : ControllerBase
    {
        private readonly IKpiService _approvalService;

        public ApprovalController(IKpiService approvalService)
        {
            _approvalService = approvalService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Approve([FromBody] ApproveKpiAssignmentDto dto)
        {
            var approverId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var log = await _approvalService.ApproveAsync(dto, approverId);
            return Ok(log);
        }

        [HttpGet("{assignmentId}")]
        public async Task<IActionResult> GetLogs(int assignmentId)
        {
            var logs = await _approvalService.GetLogsByAssignmentIdAsync(assignmentId);
            return Ok(logs);
        }
    }

}
