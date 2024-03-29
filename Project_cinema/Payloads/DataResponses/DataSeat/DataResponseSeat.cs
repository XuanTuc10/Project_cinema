using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataTicket;

namespace Project_cinema.Payloads.DataResponses.DataSeat
{
    public class DataResponseSeat
    {
        public int Number { get; set; }
        public string NameStatus { get; set; }
        public string Line { get; set; }
        public string NameRoom { get; set; }
        public bool? IsActive { get; set; }
        public string NameType { get; set; }
        public IQueryable<DataResponseTicket>? dataResponseTickets { get; set; }
    }
}
