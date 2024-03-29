using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_cinema.Payloads.DataRequests.SeatRequests;
using Project_cinema.Services.Interfaces;

namespace Project_cinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;
        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }
        [HttpPost("CreateSeat")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> CreateSeat([FromForm] Request_CreateSeat request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _seatService.CreateSeat(id, request);
            switch (result.Status)
            {
                case 200:
                    return Ok(result);
                case 404:
                    return NotFound(result);
                case 400:
                    return BadRequest(result);
                case 403:
                    return Unauthorized(result);
                default:
                    return StatusCode(500, result);
            }
        }
        [HttpPut("DeleteSeat/{SeatId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSeat([FromRoute] int SeatId)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _seatService.DeleteSeat(id, SeatId);
            switch (result)
            {
                case "Đồ ăn không tồn tại": return NotFound(result);
                case "Xóa thành công!!!!!!": return Ok(result);
                default: return StatusCode(500, result);
            }
        }
        [HttpGet("GetAllSeat")]
        public async Task<IActionResult> GetAllSeat(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _seatService.GetAllSeat(pageSize, pageNumber));
        }
        [HttpPut("UpdateSeat")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> UpdatePost([FromForm] Request_UpdateSeat request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _seatService.UpdateSeat(id, request);
            switch (result.Status)
            {
                case 200:
                    return Ok(result);
                case 404:
                    return NotFound(result);
                case 400:
                    return BadRequest(result);
                case 403:
                    return Unauthorized(result);
                default:
                    return StatusCode(500, result);
            }
        }
    }
}
