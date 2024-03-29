using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataSchedule;
using Project_cinema.Payloads.DataResponses.DataTicket;
using System.Xml.Linq;

namespace Project_cinema.Payloads.Converters
{
    public class ScheduleConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly TicketConverter _converter;
        public ScheduleConverter(TicketConverter converter)
        {
            _converter = converter;
        }
        public DataResponseSchedule EntityToDTO(Schedule schedule)
        {
            if (schedule == null || schedule.Id == null)
            {
                throw new ArgumentNullException("Schedule is null or Schedule.Id is null");
            }
            var scheduleItem = _context.schedules
                .Include(x => x.Room)
                .Include(x => x.Movie)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == schedule.Id);
            var ticketItem = _context.tickets.Where(x => x.ScheduleID == schedule.Id).Select(x => _converter.EntityToDTO(x));
            return new DataResponseSchedule
            {
                Price = schedule.Price,
                StartAt = schedule.StartAt,
                EndAt = schedule.EndAt,
                Code = schedule.Name,
                NameMovie = scheduleItem.Movie?.Name,
                Name = schedule.Name,
                NameRoom = scheduleItem.Room?.Name,
                IsActive = schedule.IsActive,
                dataResponseTickets = ticketItem

            };
        }
    }
}
