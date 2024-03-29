using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.DataRequests.FoodRequests;
using Project_cinema.Payloads.DataResponses.DataFood;
using Project_cinema.Payloads.Responses;

namespace Project_cinema.Services.Interfaces
{
    public interface IFoodService
    {
        Task<ResponseObject<DataResponseFood>> CreateFood(int userId, Request_CreateFood Food);
        Task<ResponseObject<DataResponseFood>> UpdateFood(int userId, Request_UpdateFood Food);
        Task<string> DeleteFood(int userId, int FoodId);
        Task<PageResult<DataResponseFood>> GetAllFood(int pageSize, int pageNumber);
        Task<PageResult<DataResponseFood>> GetFoodSevenDaysAgo(int pageSize, int pageNumber);
    }
}
