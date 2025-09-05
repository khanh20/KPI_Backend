using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos.UnitDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UnitController : ControllerBase
    {
        private readonly IKpiService _kpiService;

        public UnitController(IKpiService kpiService)
        {
            _kpiService = kpiService;
        }

        #region Unit
        [HttpGet("units")]
        public async Task<IActionResult> GetAllUnit()
        {
            var units = await _kpiService.GetAllUnitAsync();
            return Ok(units);
        }

        [HttpGet("units/{id}")]
        public async Task<IActionResult> GetUnitById(int id)
        {
            var unit = await _kpiService.GetUnitByIdAsync(id);
            if (unit == null) return NotFound();
            return Ok(unit);
        }

        [HttpPost("units")]
        public async Task<IActionResult> Create([FromBody] CreateUnitDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _kpiService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUnitById), new { id = created.Id }, created);
        }

        [HttpPut("units/{id}")]
        public async Task<IActionResult> UpdateUnit(int id, [FromBody] UpdateUnitDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _kpiService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("units/{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            var result = await _kpiService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        #endregion
    }
}
