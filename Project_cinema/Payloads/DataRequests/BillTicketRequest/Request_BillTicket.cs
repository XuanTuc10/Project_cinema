using Project_cinema.Entities;
using Project_cinema.Payloads.DataRequests.BillFoodRequest;
using Project_cinema.Payloads.DataRequests.ScheduleRequests;

namespace Project_cinema.Payloads.DataRequests.BillTicketRequest
{
    public class Request_BillTicket
    {
        public int Quantity { get; set; }
        public int TicketID { get; set; }
    }
}
