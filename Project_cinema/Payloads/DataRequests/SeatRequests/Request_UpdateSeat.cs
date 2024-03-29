namespace Project_cinema.Payloads.DataRequests.SeatRequests
{
    public class Request_UpdateSeat
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int SeatStatusID { get; set; }
        public string Line { get; set; }
        public int RoomID { get; set; }
        public int SeatTypeID { get; set; }
    }
}
