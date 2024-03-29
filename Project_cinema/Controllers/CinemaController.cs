using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_cinema.Payloads.DataRequests.CinemaRequests;
using Project_cinema.Services.Interfaces;

namespace Project_cinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemaController : ControllerBase
    {
        private readonly ICinemaService _CinemaService;
        public CinemaController(ICinemaService cinemaService)
        {
            _CinemaService = cinemaService;
        }
        [HttpPost("CreateCinema")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> CreateCinema([FromForm] Request_CreateCinema request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _CinemaService.CreateCinema(id, request);
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
        [HttpPut("DeleteCinema/{cinemaId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCinema([FromRoute] int cinemaId)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _CinemaService.DeleteCinema(id, cinemaId);
            switch (result)
            {
                case "Đồ ăn không tồn tại": return NotFound(result);
                case "Xóa thành công!!!!!!": return Ok(result);
                default: return StatusCode(500, result);
            }
        }
        [HttpGet("GetAllCinema")]
        public async Task<IActionResult> GetAllCinema(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _CinemaService.GetAllCinema(pageSize, pageNumber));
        }

        [HttpGet("GetCinemaStatisticsById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCinemaStatisticsById(int cinemaID, DateTime startDate, DateTime endDate, int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _CinemaService.GetCinemaStatisticsById(cinemaID, startDate, endDate,pageSize, pageNumber));
        }
        [HttpPut("UpdateCinema")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> UpdatePost([FromForm] Request_UpdateCinema request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _CinemaService.UpdateCinema(id, request);
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
