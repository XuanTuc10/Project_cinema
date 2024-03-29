using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses;
using Project_cinema.Payloads.DataResponses.DataRoom;

namespace Project_cinema.Payloads.DataRequests.CinemaRequests
{
    public class Request_CreateCinema
    {
        public string Address { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string NameOfCinema { get; set; }
    }
}
