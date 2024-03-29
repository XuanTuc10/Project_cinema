using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.DataRequests.FoodRequests;
using Project_cinema.Payloads.DataRequests.ScheduleRequests;
using Project_cinema.Payloads.DataResponses.DataFood;
using Project_cinema.Payloads.DataResponses.DataSchedule;
using Project_cinema.Payloads.Responses;

namespace Project_cinema.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<ResponseObject<DataResponseSchedule>> CreateSchedule(int userId, Request_CreateSchedule schedule);
        Task<ResponseObject<DataResponseSchedule>> UpdateSchedule(int userId, Request_UpdateSchedule schedule);
        Task<string> DeleteSchedule(int userId, int ScheduleId);
        Task<PageResult<DataResponseSchedule>> GetAllSchedule(int pageSize, int pageNumber);
    }
}
