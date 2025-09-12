using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssignmentController : ControllerBase
    {
        private readonly IKpiService _assignmentService;

        public AssignmentController(IKpiService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost("assign-item")]
        public async Task<IActionResult> AssignItem([FromBody] CreateKpiAssignmentDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            var result = await _assignmentService.AssignItemAsync(dto, userId, role);
            return Ok(result);
        }

        [HttpPost("assign-template")]
        public async Task<IActionResult> AssignTemplate([FromBody] AssignTemplateDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            var result = await _assignmentService.AssignTemplateAsync(dto, userId, role);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
            if (assignment == null) return NotFound();
            return Ok(assignment);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAssignmentByUser(int userId)
        {
            var assignments = await _assignmentService.GetAssignmentByUserAsync(userId);
            return Ok(assignments);
        }

        [HttpGet("unit/{unitId}")]
        public async Task<IActionResult> GetAssignmentByUnit(int unitId)
        {
            var assignments = await _assignmentService.GetAssignmentByUnitAsync(unitId);
            return Ok(assignments);
        }
        [HttpGet("getAllAssigement")]
        public async Task<IActionResult> GetAllAssigement()
        {
            try
            {
                var assignments = await _assignmentService.GetAllAssigment();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("self-evaluate")]
        public async Task<IActionResult> SelfEvaluate([FromBody] SelfEvaluateDto dto)
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim);

            try
            {
                var assignment = await _assignmentService.SelfEvaluate(currentUserId, dto);
                return Ok(new { message = "Đánh giá thành công", assignment });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("componentScores/{userId}")]
        public async Task<IActionResult> GetTotalComponentScores(int userId)
        {
            var result = await _assignmentService.GetTotalComponentScoreByUser(userId);

            if (result == null || result.ScoresByType == null || result.ScoresByType.Count == 0)
            {
                return NotFound(new { message = "Không tìm thấy dữ liệu cho user này hoặc user chưa đánh giá." });
            }

            return Ok(result);
        }
        [HttpPost("GetAllTotalScore")]
        public async Task<IActionResult> GetAllKpiScores()
        {
            var result = await _assignmentService.GetAllKpiScores();

            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }


    }
}
