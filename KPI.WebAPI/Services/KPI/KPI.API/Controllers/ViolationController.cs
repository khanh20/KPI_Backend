using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos.ViolationDto;
using KPI.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ViolationController : ControllerBase
    {
        private readonly IKpiService _violationService;

        public ViolationController(IKpiService violationService)
        {
            _violationService = violationService;
        }

        [HttpPost("createViolation")]
        public async Task<IActionResult> CreateViolation([FromBody] CreateKpiViolationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _violationService.CreateViolationAsync(dto);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _violationService.GetViolationCategoryById(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateVioltionCategory([FromBody] CreateKpiViolationCateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _violationService.CreateViolationCategory(dto);
            return Ok(created);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteViolationCategory(int id)
        {
            try
            {
                var success = await _violationService.DeleteViolationCategory(id);
                if (!success)
                    return NotFound(new { message = "Không tìm thấy hoặc đã xoá" });

                return Ok(new { message = "Xoá thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
        [HttpPost("violationLevel")]
        public async Task<IActionResult> Create([FromBody] CreateKpiViolationLevelDto dto)
        {
            var created = await _violationService.CreateViolationLevel(dto);
            return Ok(created);
        }
        [HttpGet("getAllViolationLevel")]
        public async Task<IActionResult> GetAllViolationLevel()
        {
            try
            {
                var levels = await _violationService.GetAllViolationLevel();
                return Ok(levels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategoryId(int categoryId)
        {
            try
            {
                var levels = await _violationService.GetViolationLevelByCategoryId(categoryId);
                return levels.Any()
                    ? Ok(levels)
                    : NotFound(new { message = "Không có mức vi phạm nào cho Category này" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("getCategoryWithLevels")]
        public IActionResult GetAllViolationCategoryWithLevels()
        {
            try
            {
                var result = _violationService.GetAllViolationCategoryWithLevels();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }
        [HttpGet("summary/{userId}")]
        public async Task<IActionResult> GetUserViolationSummaryByCategory(int userId)
        {
            try
            {
                var result = await _violationService.CalculateUserViolation(userId);

                if (result == null || result.Details == null || !result.Details.Any())
                    return NotFound(new { message = "Không tìm thấy vi phạm nào cho user này" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [HttpGet("getViolationByUser/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var violations = await _violationService.GetViolationsByUserIdAsync(userId);

            if (violations == null || violations.Count == 0)
                return NotFound($"Không tìm thấy vi phạm nào cho userId = {userId}");

            return Ok(violations);
        }
        [HttpGet("getAllTotalDeductions")]
        public async Task<IActionResult> GetAllTotalDeductions()
        {
            var result = await _violationService.GetAllTotalDeductions();
            return Ok(result);
        }
        [HttpGet("getAllUserViolations")]
        public async Task<IActionResult> GetAllUserViolation()
        {
            var result = await _violationService.GetAllUserViolations();
            return Ok(result);
        }



        /// <summary>
        /// Lấy tổng hợp KPI vi phạm của 1 đơn vị (Unit)
        /// </summary>
        [HttpGet("unit/{unitId}")]
        public async Task<ActionResult<UnitViolationSummaryResultDto>> GetUnitViolationSummary(int unitId)
        {
            var result = await _violationService.CalculateUnitViolation(unitId);
            if (result == null || !result.Details.Any())
            {
                return NotFound(new { message = $"Không tìm thấy dữ liệu vi phạm cho Unit {unitId}" });
            }

            return Ok(result);
        }

    }

}
