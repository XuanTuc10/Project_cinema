using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_cinema.Payloads.DataRequests.FoodRequests;
using Project_cinema.Services.Interfaces;

namespace Project_cinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;
        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }
        [HttpPost("CreateFood")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "001")]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> CreateFood([FromForm] Request_CreateFood request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _foodService.CreateFood(id, request);
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
        [HttpPut("DeleteFood/{foodId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "001")]
        public async Task<IActionResult> DeleteFood([FromRoute] int foodId)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _foodService.DeleteFood(id, foodId);
            switch (result)
            {
                case "Đồ ăn không tồn tại": return NotFound(result);
                case "Xóa thành công!!!!!!": return Ok(result);
                default: return StatusCode(500, result);
            }
        }
        [HttpGet("GetAllFood")]
        public async Task<IActionResult> GetAllFood(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _foodService.GetAllFood(pageSize, pageNumber));
        }

        [HttpGet("GetFoodSevenDaysAgo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "001")]
        public async Task<IActionResult> GetFoodSevenDaysAgo(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _foodService.GetFoodSevenDaysAgo(pageSize, pageNumber));
        }
        [HttpPut("UpdateFood")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> UpdatePost([FromForm] Request_UpdateFood request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _foodService.UpdateFood(id, request);
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
