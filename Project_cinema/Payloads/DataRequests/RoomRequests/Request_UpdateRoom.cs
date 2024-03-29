namespace Project_cinema.Payloads.DataRequests.RoomRequests
{
    public class Request_UpdateRoom
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public int CinemaID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
