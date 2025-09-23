using KPI.ApplicationService.KpiModule.Implements;
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
            try
            {
                var result = await _assignmentService.AssignItemAsync(dto, userId, role);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // lỗi nghiệp vụ, FE sẽ nhận được status 400 + message
                return BadRequest(new { message = ex.Message });
            }
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


        #region Export Excel
        [HttpGet("assignments/export")]
        public async Task<IActionResult> ExportAssignment([FromQuery] int? unitId, [FromQuery] int? userId, [FromQuery] int year)
        {
            var fileBytes = await _assignmentService.ExportAssignmentToExcelAsync(unitId, userId, year);

            string fileName = userId.HasValue
                ? $"KPI_User_{userId}_{year}.xlsx"
                : $"KPI_Unit_{unitId}_{year}.xlsx";

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        #endregion

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
        [HttpGet("GetAllTotalScore")]
        public async Task<IActionResult> GetAllKpiScores()
        {
            var result = await _assignmentService.GetAllKpiScores();

            if (result == null || result.Count == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //tất cả Assignment trong một Unit
        [HttpGet("unit/{unitId}/{year}")]
        public async Task<IActionResult> GetAssignmentsByUnit(int unitId, int year)
        {
            var assignments = await _assignmentService.GetAssignmentsByUnitAsync(unitId, year);
            return Ok(assignments);
        }

        // Get  tất cả Assignment của member thuộc quyền tôi
        [HttpGet("unit-members/{year}")]
        public async Task<IActionResult> GetAssignmentsByUnitMembers(int year)
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var assignments = await _assignmentService.GetAssignmentsByUnitMembersAsync(currentUserId, year);
            return Ok(assignments);
        }

        // Get  tất cả Assignment của trưởng đơn vị trong một Unit
        [HttpGet("units/{year}")]
        public async Task<IActionResult> GetUnitAssignments(int year)
        {
            var result = await _assignmentService.GetUnitAssignmentsAsync(year);
            return Ok(result);
        }

        /// <summary>
        /// Tính toán điểm KPI final của trưởng đơn vị theo headUserId
        /// </summary>
        [HttpGet("final-score/{headUserId}")]
        public async Task<ActionResult<KpiTypeScoreResultDto>> GetHeadOfUnitFinalScore(int headUserId)
        {
            var result = await _assignmentService.GetHeadOfUnitFinalScore(headUserId);
            if (result == null) return NotFound(new { Message = "Không tìm thấy đơn vị hoặc dữ liệu KPI." });

            return Ok(result);
        }

        [HttpGet("unit-final-score/{unitId}")]
        public async Task<ActionResult<KpiTypeScoreResultDto>> GetUnitFinalScore(int unitId)
        {
            var result = await _assignmentService.GetUnitFinalScore(unitId);
            if (result == null) return NotFound(new { Message = "Không tìm thấy đơn vị hoặc dữ liệu KPI." });

            return Ok(result);
        }

        /// <summary>
        /// Lưu hoặc cập nhật điểm KPI của trưởng đơn vị vào DB
        /// </summary>
        [HttpPost("save/{headUserId}")]
        public async Task<IActionResult> SaveHeadOfUnitFinalScore(int headUserId)
        {
            await _assignmentService.SaveHeadOfUnitFinalScore(headUserId);
            return Ok(new { Message = "Đã lưu KPI score của trưởng đơn vị." });
        }
    }
}
