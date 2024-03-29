using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.CinemaRequests;
using Project_cinema.Payloads.DataRequests.RoomRequests;
using Project_cinema.Payloads.DataResponses.DataCinema;
using Project_cinema.Payloads.DataResponses.DataRoom;
using Project_cinema.Payloads.DataResponses.DataSeat;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;
using Project_Room.Payloads.Converters;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project_cinema.Services.Implements
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseRoom> _responseObject;
        private readonly RoomConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoomService(ResponseObject<DataResponseRoom> responseObject, RoomConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = new AppDbContext();
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataResponseRoom>> CreateRoom(int userId, Request_CreateRoom request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var cinema = await _context.cinemas.SingleOrDefaultAsync(x => x.Id == request.CinemaID);
            if (cinema == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy rạp phim", null);
            }
            Room room = new Room
            {
                Capacity = request.Capacity,
                Type = request.Type,
                Description = request.Description,
                CinemaID = request.CinemaID,
                Code = request.Code,
                Name = request.Name,
                IsActive = false
            };
            await _context.rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            cinema.IsActive = true;
            _context.cinemas.Update(cinema);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thêm phòng thành công", _converter.EntityToDTO(room));
        }

        public async Task<string> DeleteRoom(int userId, int roomId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var room = await _context.rooms.SingleOrDefaultAsync(x => x.Id == roomId);
            var schedules = _context.schedules.Where(x => x.Id == room.Id).ToList();
            var seats = _context.seats.Where(x => x.Id == room.Id).ToList();
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
            if (room == null)
            {
                return "Phòng không tồn tại";
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
            _context.rooms.Remove(room);
            await _context.SaveChangesAsync();
            return "Xóa rạp thành công";
        }

        public async Task<PageResult<DataResponseRoom>> GetAllRoom(int pageSize, int pageNumber)
        {
            var query = _context.rooms.Include(x => x.Seats).Include(x => x.Schedules).AsNoTracking().Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<PageResult<DataResponseSeat>> GetSeatStatusOfRoom(int roomId, int pageSize, int pageNumber)
        {
            var room = await _context.rooms
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
            {
                return null;
            }

            var seatStatusList = room.Seats.Select(seat => new DataResponseSeat
            {
                Number = seat.Number,
                NameStatus = seat.SeatStatus?.NameStatus,
                IsActive = seat.IsActive
            });
            var result = Pagination.GetPagedData(seatStatusList, pageSize, pageNumber);
            return result;
        }
        public async Task<ResponseObject<DataResponseRoom>> UpdateRoom(int userId, Request_UpdateRoom request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var room = await _context.rooms.SingleOrDefaultAsync(x => x.Id == request.Id);
            if (room == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy rạp phim", null);
            }
            room.Capacity = request.Capacity;
            room.Type = request.Type;
            room.Description = request.Description;
            room.CinemaID = request.CinemaID;
            room.Code = request.Code;
            room.Name = request.Name;
            _context.rooms.Update(room);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật thông tin thành công", _converter.EntityToDTO(room));
        }
    }
}
