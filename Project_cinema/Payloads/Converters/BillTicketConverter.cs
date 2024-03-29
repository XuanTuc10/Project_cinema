using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataBillTicket;

namespace Project_cinema.Payloads.Converters
{
    public class BillTicketConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        public DataResponseBillTicket EntityToDTO(BillTicket billTicket)
        {
            if (billTicket == null || billTicket.Id == null)
            {
                throw new ArgumentNullException("billTicket is null or billTicket.Id is null");
            }
            var ticketItem = _context.billTickets
                .Include(x => x.Ticket)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == billTicket.Id);

            return new DataResponseBillTicket
            {
                TicketName = ticketItem.Ticket?.Code,
                Price = ticketItem.Ticket.PriceTicket,
                Quantity = billTicket.Quantity
            };
        }
    }
}
