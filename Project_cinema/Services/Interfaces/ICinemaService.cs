using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.DataRequests.CinemaRequests;
using Project_cinema.Payloads.DataResponses.DataCinema;
using Project_cinema.Payloads.Responses;

namespace Project_cinema.Services.Interfaces
{
    public interface ICinemaService
    {
        Task<ResponseObject<DataResponseCinema>> CreateCinema(int userId, Request_CreateCinema cinema);
        Task<ResponseObject<DataResponseCinema>> UpdateCinema(int userId, Request_UpdateCinema cinema);
        Task<string> DeleteCinema(int userId, int cinemaId);
        Task<PageResult<DataResponseCinema>> GetAllCinema(int pageSize, int pageNumber);
        Task<PageResult<DataResponseCinemaStatistics>> GetCinemaStatisticsById(int cinemaId, DateTime startDate, DateTime endDate, int pageSize, int pageNumber);
    }
}
