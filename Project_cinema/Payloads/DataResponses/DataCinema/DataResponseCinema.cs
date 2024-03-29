using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataRoom;

namespace Project_cinema.Payloads.DataResponses.DataCinema
{
    public class DataResponseCinema : DataResponseBase
    {
        public string Address { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string NameOfCinema { get; set; }
        public bool? IsActive { get; set; }
        public IQueryable<DataResponseRoom>? dataResponseRooms { get; set; }
    }
}
