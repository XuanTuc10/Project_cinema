using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.DataRequests.MovieRequests;
using Project_cinema.Payloads.DataResponses.DataMovie;
using Project_cinema.Payloads.Responses;

namespace Project_cinema.Services.Interfaces
{
    public interface IMovieService
    {
        Task<ResponseObject<DataResponseMovie>> CreateMovie(int userId, Request_CreateMovie Movie);
        Task<ResponseObject<DataResponseMovie>> UpdateMovie(int userId, Request_UpdateMovie Movie);
        Task<string> DeleteMovie(int userId, int MovieId);
        Task<PageResult<DataResponseMovie>> GetAllMovie(int pageSize, int pageNumber);
        Task<PageResult<DataResponseMovie>> GetHotMovie(int pageSize, int pageNumber);
        Task<PageResult<DataResponseMovie>> GetAllMovieOfCinema(int pageSize, int pageNumber);
        Task<PageResult<DataResponseMovie>> GetAllMovieOfRoom(int pageSize, int pageNumber);
    }
}
