using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_cinema.Payloads.DataRequests.MovieRequests;
using Project_cinema.Services.Interfaces;

namespace Project_cinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService MovieService)
        {
            _movieService = MovieService;
        }
        [HttpPost("CreateMovie")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> CreateMovie([FromForm] Request_CreateMovie request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _movieService.CreateMovie(id, request);
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
        [HttpPut("DeleteMovie/{MovieId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMovie([FromRoute] int movieId)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _movieService.DeleteMovie(id, movieId);
            switch (result)
            {
                case "Đồ ăn không tồn tại": return NotFound(result);
                case "Xóa thành công!!!!!!": return Ok(result);
                default: return StatusCode(500, result);
            }
        }
        [HttpGet("GetAllMovie")]
        public async Task<IActionResult> GetAllMovie(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _movieService.GetAllMovie(pageSize, pageNumber));
        }
        [HttpGet("GetHotMovie")]
        public async Task<IActionResult> GetHotMovie(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _movieService.GetHotMovie(pageSize, pageNumber));
        }
        [HttpGet("GetAllMovieOfCinema")]
        public async Task<IActionResult> GetAllMovieOfCinema(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _movieService.GetAllMovieOfCinema(pageSize, pageNumber));
        }
        [HttpGet("GetAllMovieOfRoom")]
        public async Task<IActionResult> GetAllMovieOfRoom(int pageSize = 10, int pageNumber = 1)
        {
            return Ok(await _movieService.GetAllMovieOfRoom(pageSize, pageNumber));
        }
        [HttpPut("UpdateMovie")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<IActionResult> UpdatePost([FromForm] Request_UpdateMovie request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _movieService.UpdateMovie(id, request);
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
