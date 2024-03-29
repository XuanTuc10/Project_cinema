using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_cinema.DataContexts;
using Project_cinema.Handler.HandleEmail;
using Project_cinema.Payloads.DataRequests.OrderRequests;
using Project_cinema.Payloads.DataRequests.VnPayRequests;
using Project_cinema.Services.Implements;
using Project_cinema.Services.Interfaces;

namespace Project_cinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly IOrderService _orderService;
        private readonly IVnPayService _vnPayService;
        public OrderController(IOrderService orderService, IVnPayService vnPayService)
        {
            _orderService = orderService;
            _vnPayService = vnPayService;
        }

        [HttpPost("CreateOrder")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateOrder([FromBody] Request_CreateOrder request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var result = await _orderService.CreateOrder(id, request);
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

        [HttpPost("CreatePayment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CreatePayment([FromBody] Request_VnPayModel request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            var paymentResult = _vnPayService.CreatePaymentUrl(HttpContext, id, request);
            if (paymentResult == null)
            {
                return BadRequest("Không tìm thấy hóa đơn khách hàng");
            }
            return Ok(paymentResult);
        }

        [HttpGet("PaymenCallBack")]
        public IActionResult PaymenCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
