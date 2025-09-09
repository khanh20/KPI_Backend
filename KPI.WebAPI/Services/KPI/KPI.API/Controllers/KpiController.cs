using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos;
using KPI.ApplicationService.KPIModule.Dtos.UnitDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KpiController : ControllerBase
    {
        private readonly IKpiService _kpiService;

        public KpiController(IKpiService kpiService)
        {
            _kpiService = kpiService;
        }

        #region KPI Template

        [HttpGet("templates")]
        public async Task<IActionResult> GetAllTemplates()
        {
            var templates = await _kpiService.GetAllAsync();
            return Ok(templates);
        }

        [HttpGet("templates/{id}")]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            var template = await _kpiService.GetByIdAsync(id);
            if (template == null)
                return NotFound();
            return Ok(template);
        }

        [HttpPost("templates")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateKpiTemplateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var created = await _kpiService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetTemplateById), new { id = created.Id }, created);
        }

        [HttpPut("templates/{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] UpdateKpiTemplateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lấy userId từ JWT
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            // Lấy role từ JWT
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            // Update template
            var updated = await _kpiService.UpdateAsync(id, dto, userId);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpPost("templates/{id}/request-delete")]
        public async Task<IActionResult> RequestDeleteTemplate(int id, [FromBody] string? comment = null)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            // Nếu Admin/Hiệu trưởng thì xóa luôn
            if (role == "Admin" || role == "Principal")
            {
                var deleted = await _kpiService.DeleteTemplateAsync(id, userId);
                if (!deleted) return NotFound();
                return Ok(new { message = "Template deleted successfully (auto-approved)" });
            }

            // User bình thường → tạo request delete
            var requested = await _kpiService.RequestDeleteTemplateAsync(id, userId, comment);
            if (!requested) return NotFound();

            return Ok(new { message = "Delete request created, pending approval" });
        }

        #endregion

        #region KPI Item

        [HttpGet("items")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _kpiService.GetAllItemsAsync();
            return Ok(items);
        }

        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _kpiService.GetItemByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet("items/creator")]
        public async Task<IActionResult> GetMyItems()
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var items = await _kpiService.GetItemsByCreatorAsync(userId);
            return Ok(items);
        }

        [HttpPost("items")]
        public async Task<IActionResult> CreateItem([FromBody] CreateKpiItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var created = await _kpiService.CreateItemAsync(dto, userId);
            return CreatedAtAction(nameof(GetItemById), new { id = created.Id }, created);
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateKpiItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            try
            {
                var updated = await _kpiService.UpdateItemAsync(id, dto, userId);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("templates/{templateId}/items")]
        public async Task<IActionResult> GetItemsByTemplate(int templateId)
        {
            var items = await _kpiService.GetItemsByTemplateAsync(templateId);
            return Ok(items);
        }


        #endregion


        //#region Unit
        //[HttpGet("units")]
        //public async Task<IActionResult> GetAllUnit()
        //{
        //    var units = await _kpiService.GetAllUnitAsync();
        //    return Ok(units);
        //}

        //[HttpGet("units/{id}")]
        //public async Task<IActionResult> GetUnitById(int id)
        //{
        //    var unit = await _kpiService.GetUnitByIdAsync(id);
        //    if (unit == null) return NotFound();
        //    return Ok(unit);
        //}

        //[HttpPost("units")]
        //public async Task<IActionResult> Create([FromBody] CreateUnitDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var created = await _kpiService.CreateAsync(dto);
        //    return CreatedAtAction(nameof(GetUnitById), new { id = created.Id }, created);
        //}

        //[HttpPut("units/{id}")]
        //public async Task<IActionResult> UpdateUnit(int id, [FromBody] UpdateUnitDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var updated = await _kpiService.UpdateAsync(id, dto);
        //    if (updated == null) return NotFound();
        //    return Ok(updated);
        //}

        //[HttpDelete("units/{id}")]
        //public async Task<IActionResult> DeleteUnit(int id)
        //{
        //    var result = await _kpiService.DeleteAsync(id);
        //    if (!result) return NotFound();
        //    return NoContent();
        //}
        //#endregion
    }
}
