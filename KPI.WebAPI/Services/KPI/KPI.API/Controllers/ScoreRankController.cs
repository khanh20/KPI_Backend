using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos;
using KPI.ApplicationService.KPIModule.Dtos.ViolationDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPI.API.Controllers
{
    public class ScoreRankController : ControllerBase
    {
        private readonly IKpiService _scoreRankService;

        public ScoreRankController(IKpiService scoreRankService)
        {
            _scoreRankService = scoreRankService;
        }

        [HttpGet("my-rank/{year}")]
        [Authorize]
        public async Task<ActionResult<KpiRankResultDto>> GetMyKpiRank(int year)
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "Không lấy được thông tin người dùng." });

            int userId = int.Parse(userIdClaim);
            var result = await _scoreRankService.GetKpiRank(userId, year);

            if (result == null)
                return NotFound(new { Message = "Không tìm thấy dữ liệu KPI của bạn." });

            return Ok(result);
        }


        // <summary>
        /// Lấy xếp loại KPI của cá nhân
        /// </summary>
        [HttpGet("rank/{userId}/{year}")]
        public async Task<ActionResult<KpiRankResultDto>> GetKpiRank(int userId, int year)
        {
            var result = await _scoreRankService.GetKpiRank(userId, year);
            if (result == null) return NotFound(new { Message = "Không tìm thấy dữ liệu KPI." });

            return Ok(result);
        }

        /// <summary>
        /// Lấy xếp loại KPI của tất cả nhân viên thuộc đơn vị mà mình làm trưởng
        /// </summary>
        [HttpGet("subordinates/{year}")]
        public async Task<ActionResult<List<KpiRankResultDto>>> GetSubordinatesKpiRanks(int year)
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "Không lấy được thông tin người dùng." });

            int headUserId = int.Parse(userIdClaim);
            var results = await _scoreRankService.GetSubordinatesKpiRanks(headUserId, year);

            if (results == null || results.Count == 0)
                return NotFound(new { Message = "Không tìm thấy dữ liệu KPI cho cấp dưới." });

            return Ok(results);
        }
    }
}
