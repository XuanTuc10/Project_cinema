using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataBillTicket;

namespace Project_cinema.Payloads.DataResponses.DataTicket
{
    public class DataResponseTicket
    {
        public string Code { get; set; }
        public string NameSchedule { get; set; }
        public int? NumberSeat { get; set; }
        public double PriceTicket { get; set; }
        public bool? IsActive { get; set; }
    }
}
