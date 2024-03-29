using Project_cinema.Entities;

namespace Project_cinema.Payloads.DataRequests.ScheduleRequests
{
    public class Request_CreateSchedule
    {
        public double Price { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int MovieID { get; set; }
        public string Name { get; set; }
        public int RoomID { get; set; }
    }
}
