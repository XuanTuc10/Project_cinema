using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataSchedule;
using Project_cinema.Payloads.DataResponses.DataSeat;

namespace Project_cinema.Payloads.DataResponses.DataRoom
{
    public class DataResponseRoom
    {
        public int Capacity { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public string NameCinema { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public IQueryable<DataResponseSchedule> dataResponseSchedules { get; set; }
        public IQueryable<DataResponseSeat> dataResponseSeats { get; set; }
    }
}
