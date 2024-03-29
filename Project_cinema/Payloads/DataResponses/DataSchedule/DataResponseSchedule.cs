using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataTicket;

namespace Project_cinema.Payloads.DataResponses.DataSchedule
{
    public class DataResponseSchedule
    {
        public double Price { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Code { get; set; }
        public string NameMovie { get; set; }
        public string Name { get; set; }
        public string NameRoom { get; set; }
        public bool? IsActive { get; set; }
        public IQueryable<DataResponseTicket>? dataResponseTickets { get; set; }
    }
}
