using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.DataRequests.SeatRequests;
using Project_cinema.Payloads.DataResponses.DataSeat;
using Project_cinema.Payloads.Responses;

namespace Project_cinema.Services.Interfaces
{
    public interface ISeatService
    {
        Task<ResponseObject<DataResponseSeat>> CreateSeat(int userId, Request_CreateSeat Seat);
        Task<ResponseObject<DataResponseSeat>> UpdateSeat(int userId, Request_UpdateSeat Seat);
        Task<string> DeleteSeat(int userId, int SeatId);
        Task<PageResult<DataResponseSeat>> GetAllSeat(int pageSize, int pageNumber);
    }
}
