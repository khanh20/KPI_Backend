using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> AssignKpi([FromBody] CreateKpiAssignmentDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var created = await _assignmentService.AssignAsync(dto, userId);
            return CreatedAtAction(nameof(GetAssignmentById), new { id = created.Id }, created);
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
    }
}
