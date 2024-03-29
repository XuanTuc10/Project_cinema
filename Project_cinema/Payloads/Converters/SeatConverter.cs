using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataSeat;
using Project_cinema.Payloads.DataResponses.DataTicket;

namespace Project_cinema.Payloads.Converters
{
    public class SeatConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly TicketConverter _converter;
        public SeatConverter(TicketConverter converter)
        {
            _converter = converter;
        }
        public DataResponseSeat EntityToDTO(Seat seat)
        {
            if (seat == null || seat.Id == null)
            {
                throw new ArgumentNullException("Seat is null or Seat.Id is null");
            }
            var seatItem = _context.seats
                .Include(x => x.Room)
                .Include(x => x.SeatStatus)
                .Include(x => x.SeatType)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == seat.Id);
            var ticketItem = _context.tickets.Where(x => x.SeatID == seat.Id).Select(x => _converter.EntityToDTO(x));
            return new DataResponseSeat
            {
                Number = seat.Number,
                NameStatus = seatItem.SeatStatus?.NameStatus,
                Line = seat.Line,
                NameRoom = seatItem.Room?.Name,
                IsActive = seat.IsActive,
                NameType = seatItem.SeatType?.NameType,
                dataResponseTickets = ticketItem
            };
        }
    }
}
