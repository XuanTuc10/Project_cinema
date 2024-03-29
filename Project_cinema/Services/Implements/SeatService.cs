using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.SeatRequests;
using Project_cinema.Payloads.DataResponses.DataSeat;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;

namespace Project_cinema.Services.Implements
{
    public class SeatService : ISeatService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseSeat> _responseObject;
        private readonly SeatConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeatService(ResponseObject<DataResponseSeat> responseObject, SeatConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = new AppDbContext();
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataResponseSeat>> CreateSeat(int userId, Request_CreateSeat request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var room = await _context.rooms.SingleOrDefaultAsync(x => x.Id == request.RoomID);
            if (room == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy phòng", null);
            }
            Seat seat = new Seat
            {
                Number = request.Number,
                SeatStatusID = request.SeatStatusID,
                Line = request.Line,
                RoomID = request.RoomID,
                IsActive = false,
                SeatTypeID = request.SeatTypeID
            };
            await _context.seats.AddAsync(seat);
            await _context.SaveChangesAsync();
            room.IsActive = true;
            _context.rooms.Update(room);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thêm ghế thành công", _converter.EntityToDTO(seat));
        }

        public async Task<string> DeleteSeat(int userId, int seatId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var seat = await _context.seats.SingleOrDefaultAsync(x => x.Id == seatId);
            var tickets = _context.tickets.Where(x => x.Id == seat.Id).ToList();
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
            if (seat == null)
            {
                return "Ghế không tồn tại";
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
            _context.billTickets.RemoveRange(billTickets);
            _context.seats.Remove(seat);
            await _context.SaveChangesAsync();
            return "Xóa ghế thành công";
        }

        public async Task<PageResult<DataResponseSeat>> GetAllSeat(int pageSize, int pageNumber)
        {
            var query = _context.seats.Include(x => x.Tickets).AsNoTracking().Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseSeat>> UpdateSeat(int userId, Request_UpdateSeat request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var seat = await _context.seats.SingleOrDefaultAsync(x => x.Id == request.Id);
            if (seat == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy ghế", null);
            }
            seat.Number = request.Number;
            seat.SeatStatusID = request.SeatStatusID;
            seat.Line = request.Line;
            seat.RoomID = request.RoomID;
            seat.SeatTypeID = request.SeatTypeID;
            _context.seats.Update(seat);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật thông tin thành công", _converter.EntityToDTO(seat));
        }
    }
}
