using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.MovieRequests;
using Project_cinema.Payloads.DataResponses.DataMovie;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Xml.Linq;

namespace Project_cinema.Services.Implements
{
    public class MovieService : IMovieService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseMovie> _responseObject;
        private readonly MovieConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MovieService(ResponseObject<DataResponseMovie> responseObject, MovieConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = new AppDbContext();
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataResponseMovie>> CreateMovie(int userId, Request_CreateMovie request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var rate = await _context.rates.SingleOrDefaultAsync(x => x.Id == request.RateID);
            var movieType = await _context.movieTypes.SingleOrDefaultAsync(x => x.Id == request.MovieTypeID);
            if (rate == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy tỷ lệ", null);
            }
            if (movieType == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy loại phim", null);
            }
            Movie movie = new Movie
            {
                MovieDuration = request.MovieDuration,
                EndTime = request.EndTime,
                PremiereDate = request.PremiereDate,
                Director = request.Director,
                Image = request.Image,
                HeroImage = request.HeroImage,
                Language = request.Language,
                Name = request.Name,
                RateID = request.RateID,
                Trailer = request.Trailer,
                IsActive = false,
                MovieTypeID = request.MovieTypeID
            };
            await _context.movies.AddAsync(movie);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thêm ghế thành công", _converter.EntityToDTO(movie));
        }

        public async Task<string> DeleteMovie(int userId, int movieId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var movie = await _context.movies.SingleOrDefaultAsync(x => x.Id == movieId);
            var schedules = _context.schedules.Where(x => x.Id == movie.Id).ToList();
            List<Ticket> tickets = new List<Ticket>();
            foreach (var schedule in schedules)
            {
                var ticket = await _context.tickets.SingleOrDefaultAsync(x => x.ScheduleID == schedule.Id);
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
            if (movie == null)
            {
                return "Film không tồn tại";
            }
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("001"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            _context.tickets.RemoveRange(tickets);
            _context.billTickets.RemoveRange(billTickets);
            _context.movies.Remove(movie);
            await _context.SaveChangesAsync();
            return "Xóa Film thành công";
        }

        public async Task<PageResult<DataResponseMovie>> GetAllMovie(int pageSize, int pageNumber)
        {
            var query = _context.movies.AsNoTracking().Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<PageResult<DataResponseMovie>> GetAllMovieOfCinema(int pageSize, int pageNumber)
        {
            var query = _context.cinemas
                .Include(cinema => cinema.Rooms)
                    .ThenInclude(room => room.Schedules)
                        .ThenInclude(schedule => schedule.Movie)
                        .AsEnumerable()
                .SelectMany(cinema => cinema.Rooms.SelectMany(room => room.Schedules.Select(schedule => schedule.Movie)))
                .Distinct()
                .Select(movie => _converter.EntityToDTO(movie));

            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }
        public async Task<PageResult<DataResponseMovie>> GetAllMovieOfRoom(int pageSize, int pageNumber)
        {
            var query = _context.rooms
                .Include(room => room.Schedules)
                    .ThenInclude(schedule => schedule.Movie)
                .AsEnumerable()
                .SelectMany(room => room.Schedules.Select(schedule => schedule.Movie))
                .Distinct()
                .Select(movie => _converter.EntityToDTO(movie));

            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }
        public async Task<PageResult<DataResponseMovie>> GetHotMovie(int pageSize, int pageNumber)
        {
            var query = _context.billTickets
                .Include(billTicket => billTicket.Ticket.Schedule.Movie)
                .AsEnumerable()
                .GroupBy(billTicket => billTicket.Ticket.Schedule.Movie)
                .Select(group => new
                {
                    Movie = group.Key,
                    TicketCount = group.Sum(billTicket => billTicket.Quantity)
                })
                .OrderByDescending(item => item.TicketCount)
                .Select(x => _converter.EntityToDTO(x.Movie));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseMovie>> UpdateMovie(int userId, Request_UpdateMovie request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var movie = await _context.movies.SingleOrDefaultAsync(x => x.Id == request.Id);
            if (movie == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy film", null);
            }
            movie.MovieDuration = request.MovieDuration;
            movie.EndTime = request.EndTime;
            movie.PremiereDate = request.PremiereDate;
            movie.Director = request.Director;
            movie.Image = request.Image;
            movie.HeroImage = request.HeroImage;
            movie.Language = request.Language;
            movie.Name = request.Name;
            movie.RateID = request.RateID;
            movie.Trailer = request.Trailer;
            movie.IsActive = false;
            movie.MovieTypeID = request.MovieTypeID;
            _context.movies.Update(movie);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật thông tin thành công", _converter.EntityToDTO(movie));
        }
    }
}
