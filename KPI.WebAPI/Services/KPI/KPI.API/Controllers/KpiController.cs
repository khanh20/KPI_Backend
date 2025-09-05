using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos;
using KPI.ApplicationService.KPIModule.Dtos.UnitDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            var created = await _kpiService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetTemplateById), new { id = created.Id }, created);
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

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var result = await _kpiService.DeleteItemAsync(id, userId);

            if (!result)
                return NotFound();

            return NoContent();
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
