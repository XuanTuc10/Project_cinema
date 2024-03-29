using Project_cinema.Entities;

namespace Project_cinema.Payloads.DataResponses.DataBillTicket
{
    public class DataResponseBillTicket
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string TicketName { get; set; }
    }
}
