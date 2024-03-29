using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_cinema.Payloads.DataRequests.RoomRequests;
using Project_cinema.Services.Interfaces;

namespace Project_Room.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _RoomService;
        public RoomController(IRoomService RoomService)
        {
            _RoomService = RoomService;
        }
        [HttpPost("CreateRoom")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> CreateRoom([FromForm] Request_CreateRoom request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _RoomService.CreateRoom(id, request);
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
        [HttpPut("DeleteRoom/{RoomId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoom([FromRoute] int roomId)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _RoomService.DeleteRoom(id, roomId);
            switch (result)
            {
                case "Đồ ăn không tồn tại": return NotFound(result);
                case "Xóa thành công!!!!!!": return Ok(result);
                default: return StatusCode(500, result);
            }
        }
        [HttpGet("GetAllRoom")]
        public async Task<IActionResult> GetAllRoom(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _RoomService.GetAllRoom(pageSize, pageNumber));
        }
        [HttpPut("UpdateRoom")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> UpdatePost([FromForm] Request_UpdateRoom request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _RoomService.UpdateRoom(id, request);
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
