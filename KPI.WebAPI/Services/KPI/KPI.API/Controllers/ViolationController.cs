using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos.ViolationDto;
using Microsoft.AspNetCore.Mvc;

namespace KPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViolationController : ControllerBase
    {
        private readonly IKpiService _violationService;

        public ViolationController(IKpiService violationService)
        {
            _violationService = violationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateViolation([FromBody] CreateKpiViolationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _violationService.CreateViolationAsync(dto);

            return Ok(result);
        }
    }

}
