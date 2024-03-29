using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.CinemaRequests;
using Project_cinema.Payloads.DataResponses.DataCinema;
using Project_cinema.Payloads.DataResponses.DataFood;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;
using System;

namespace Project_cinema.Services.Implements
{
    public class CinemaService : ICinemaService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseCinema> _responseObject;
        private readonly CinemaConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CinemaService(ResponseObject<DataResponseCinema> responseObject, CinemaConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = new AppDbContext();
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataResponseCinema>> CreateCinema(int userId, Request_CreateCinema request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            Cinema cinema = new Cinema
            {
                Address = request.Address,
                Description = request.Description,
                Code = request.Code,
                NameOfCinema = request.NameOfCinema,
                IsActive = false
            };
            await _context.cinemas.AddAsync(cinema);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thêm rạp phim thành công", _converter.EntityToDTO(cinema));
        }

        public async Task<string> DeleteCinema(int userId, int cinemaId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var cinema = await _context.cinemas.SingleOrDefaultAsync(x => x.Id == cinemaId);
            var rooms = _context.rooms.Where(x => x.Id == cinema.Id).ToList();
            List<Schedule> schedules = new List<Schedule>();
            List<Seat> seats = new List<Seat>();
            foreach (var room in rooms)
            {
                var schedule = await _context.schedules.SingleOrDefaultAsync(x => x.RoomID == room.Id);
                var seat = await _context.seats.SingleOrDefaultAsync(x => x.RoomID == room.Id);
                schedules.Add(schedule);
                seats.Add(seat);
            }
            List<Ticket> tickets = new List<Ticket>();
            foreach (var seat in seats)
            {
                var ticket = await _context.tickets.SingleOrDefaultAsync(x => x.SeatID == seat.Id);
                tickets.Add(ticket);
            }
            List<BillTicket> billTickets = new List<BillTicket>();
            foreach (var ticket in tickets)
            {
                var billTicket = await _context.billTickets.SingleOrDefaultAsync(x => x.TicketID == ticket.Id);
                billTickets.Add(billTicket);
            }
            foreach (var billTicket in billTickets)
            {
                var bill = await _context.bills.SingleOrDefaultAsync(x => x.Id == billTicket.BillID);
                bill.IsActive = false;
                _context.bills.Update(bill);
                await _context.SaveChangesAsync();
            }
            if (cinema == null)
            {
                return "Bộ sưu tập không tồn tại";
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
            _context.tickets.RemoveRange(tickets);
            _context.schedules.RemoveRange(schedules);
            _context.seats.RemoveRange(seats);
            _context.billTickets.RemoveRange(billTickets);
            _context.rooms.RemoveRange(rooms);
            _context.cinemas.Remove(cinema);
            await _context.SaveChangesAsync();
            return "Xóa rạp thành công";
        }

        public async Task<PageResult<DataResponseCinema>> GetAllCinema(int pageSize, int pageNumber)
        {
            var query = _context.cinemas.Include(x => x.Rooms).AsNoTracking().Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseCinema>> UpdateCinema(int userId, Request_UpdateCinema request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var cinema = await _context.cinemas.SingleOrDefaultAsync(x => x.Id == request.Id);
            if (cinema == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy rạp phim", null);
            }
            cinema.Address = request.Address;
            cinema.Description = request.Description;
            cinema.Code = request.Code;
            cinema.NameOfCinema = request.NameOfCinema;
            _context.cinemas.Update(cinema);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật thông tin thành công", _converter.EntityToDTO(cinema));
        }

        public async Task<PageResult<DataResponseCinemaStatistics>> GetCinemaStatisticsById(int cinemaId, DateTime startDate, DateTime endDate, int pageSize, int pageNumber)
        {
            // Lấy thông tin của rạp dựa trên cinemaId
            var cinema = await _context.cinemas.Include(c => c.Rooms).FirstOrDefaultAsync(c => c.Id == cinemaId);
            if (cinema == null)
            {
                return null; // hoặc xử lý theo nhu cầu của bạn
            }

            // Thống kê doanh số của rạp
            var cinemaStatistics = cinema.Rooms
                .SelectMany(r => r.Schedules)
                .SelectMany(s => s.Tickets)
                .SelectMany(t => t.BillTickets)
                .Where(bt => bt.Bill.CreateTime >= startDate && bt.Bill.CreateTime <= endDate)
                .GroupBy(bt => bt.Bill.BillTicket.Ticket.Schedule.Room)
                .Select(g => new DataResponseCinemaStatistics
                {
                    NameCinema = cinema.NameOfCinema,
                    DoanhSo = g.Sum(bt => bt.Quantity * bt.Ticket.PriceTicket) +
                              g.Sum(bt => bt.Bill.BillFoods.Sum(bf => bf.Quantity * bf.Food.Price))
                })
                .OrderByDescending(r => r.DoanhSo)
                .ToList();

            // Phân trang kết quả
            var result = Pagination.GetPagedData(cinemaStatistics, pageSize, pageNumber);
            return result;
        }



    }
}
