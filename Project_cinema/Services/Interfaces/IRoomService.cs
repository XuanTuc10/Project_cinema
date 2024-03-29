using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.DataRequests.CinemaRequests;
using Project_cinema.Payloads.DataRequests.RoomRequests;
using Project_cinema.Payloads.DataResponses.DataCinema;
using Project_cinema.Payloads.DataResponses.DataRoom;
using Project_cinema.Payloads.DataResponses.DataSeat;
using Project_cinema.Payloads.Responses;

namespace Project_cinema.Services.Interfaces
{
    public interface IRoomService
    {
        Task<ResponseObject<DataResponseRoom>> CreateRoom(int userId, Request_CreateRoom Room);
        Task<ResponseObject<DataResponseRoom>> UpdateRoom(int userId, Request_UpdateRoom Room);
        Task<string> DeleteRoom(int userId, int RoomId);
        Task<PageResult<DataResponseRoom>> GetAllRoom(int pageSize, int pageNumber);
        Task<PageResult<DataResponseSeat>> GetSeatStatusOfRoom(int roomId, int pageSize, int pageNumber);
    }
}
