using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.ScheduleRequests;
using Project_cinema.Payloads.DataRequests.ScheduleRequests;
using Project_cinema.Payloads.DataResponses.DataSchedule;
using Project_cinema.Payloads.DataResponses.DataSchedule;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;
using System.Xml.Linq;

namespace Project_cinema.Services.Implements
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseSchedule> _responseObject;
        private readonly ScheduleConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScheduleService(ResponseObject<DataResponseSchedule> responseObject, ScheduleConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = new AppDbContext();
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataResponseSchedule>> CreateSchedule(int userId, Request_CreateSchedule request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            Schedule Schedule = new Schedule
            {
                Price = request.Price,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                Code = DateTime.Now.ToString() + request.MovieID.ToString() + request.RoomID.ToString(),
                MovieID = request.MovieID,
                Name = request.Name,
                RoomID = request.RoomID,
                IsActive = false
            };
            await _context.schedules.AddAsync(Schedule);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thêm lịch trình thành công", _converter.EntityToDTO(Schedule));
        }

        public async Task<string> DeleteSchedule(int userId, int ScheduleId)
        {
            var Schedule = await _context.schedules.SingleOrDefaultAsync(x => x.Id == ScheduleId);
            if (Schedule == null)
            {
                return "Schedule không tồn tại";
            }
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }

            _context.schedules.Remove(Schedule);
            await _context.SaveChangesAsync();
            return "Xóa lịch trình thành công";
        }

        public async Task<PageResult<DataResponseSchedule>> GetAllSchedule(int pageSize, int pageNumber)
        {
            var query = _context.schedules.AsNoTracking().Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseSchedule>> UpdateSchedule(int userId, Request_UpdateSchedule request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var schedule = await _context.schedules.SingleOrDefaultAsync(x => x.Id == request.Id);
            if (schedule == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy schedule", null);
            }
            schedule.Price = request.Price;
            schedule.StartAt = request.StartAt;
            schedule.EndAt = request.EndAt;
            schedule.Code = DateTime.Now.ToString() + request.MovieID.ToString() + request.RoomID.ToString();
            schedule.MovieID = request.MovieID;
            schedule.Name = request.Name;
            schedule.RoomID = request.RoomID;
            _context.schedules.Update(schedule);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật thông tin thành công", _converter.EntityToDTO(schedule));
        }
    }
}
