using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataBillTicket;
using Project_cinema.Payloads.DataResponses.DataTicket;

namespace Project_cinema.Payloads.Converters
{
    public class TicketConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        public DataResponseTicket EntityToDTO(Ticket ticket)
        {
            if (ticket == null || ticket.Id == null)
            {
                throw new ArgumentNullException("Ticket is null or Ticket.Id is null");
            }
            var ticketItem = _context.tickets
                .Include(x => x.Schedule)
                .Include(x => x.Seat)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == ticket.Id);
            return new DataResponseTicket
            {
                Code = ticket.Code,
                NameSchedule = ticketItem.Schedule?.Name,
                NumberSeat = ticketItem.Seat?.Number,
                PriceTicket = ticket.PriceTicket,
                IsActive = ticket.IsActive
            };
        }
    }
}
