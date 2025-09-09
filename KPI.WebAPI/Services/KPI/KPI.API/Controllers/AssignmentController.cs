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
    }
}
