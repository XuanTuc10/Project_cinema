using Project_cinema.Payloads.DataRequests.OrderRequests;
using Project_cinema.Payloads.DataResponses.DataOrder;
using Project_cinema.Payloads.Responses;

namespace Project_cinema.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseObject<DataResponseOrder>> CreateOrder(int customerId, Request_CreateOrder request);
    }
}
