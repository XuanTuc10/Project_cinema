using Project_cinema.Entities;
using Project_cinema.Payloads.DataRequests.BillFoodRequest;
using Project_cinema.Payloads.DataRequests.BillTicketRequest;
using Project_cinema.Payloads.DataRequests.ScheduleRequests;

namespace Project_cinema.Payloads.DataRequests.OrderRequests
{
    public class Request_CreateOrder
    {
        public string Name { get; set; }
        public  List<Request_BillFood> billFoods { get; set; }
        public  Request_BillTicket billTickets { get; set; }
    }
}
