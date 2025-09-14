using KPI.ApplicationService.KpiModule.Implements;
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

        [HttpPost("bulk-approve")]
        public async Task<IActionResult> ApproveBulk([FromBody] ApproveKpiAssignmentBulkDto dto)
        {
            var approverId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            await _approvalService.ApproveBulkAsync(dto.AssignmentIds, dto.Comment, approverId);
            return Ok(new { message = "Assignments approved successfully." });
        }

        // Endpoint mới để từ chối hàng loạt
        [HttpPost("bulk-reject")]
        public async Task<IActionResult> RejectBulk([FromBody] ApproveKpiAssignmentBulkDto dto)
        {
            var approverId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            await _approvalService.RejectBulkAsync(dto.AssignmentIds, dto.Comment, approverId);
            return Ok(new { message = "Assignments rejected successfully." });
        }






        [HttpGet("{assignmentId}")]
        public async Task<IActionResult> GetLogs(int assignmentId)
        {
            var logs = await _approvalService.GetLogsByAssignmentIdAsync(assignmentId);
            return Ok(logs);
        }

        [HttpDelete("template/request/{id}")]
        public async Task<IActionResult> RequestDeleteTemplate(int id, [FromQuery] string? comment)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var success = await _approvalService.RequestDeleteTemplateAsync(id, userId, comment);
            if (!success) return NotFound();
            return Ok(new { message = "Delete request submitted, waiting for approval" });
        }

        [HttpDelete("item/request/{id}")]
        public async Task<IActionResult> RequestDeleteItem(int id, [FromQuery] string? comment)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var success = await _approvalService.RequestDeleteItemAsync(id, userId, comment);
            if (!success) return NotFound();
            return Ok(new { message = "Delete request submitted, waiting for approval" });
        }

        [HttpPost("approve-delete/{logId}")]
        public async Task<IActionResult> ApproveDelete(int logId, [FromQuery] bool approve, [FromQuery] string? comment)
        {
            var approverId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var success = await _approvalService.ApproveDeleteAsync(logId, approverId, approve, comment);
            if (!success) return BadRequest();
            return Ok(new { message = approve ? "Delete approved" : "Delete rejected" });
        }

    }

}
