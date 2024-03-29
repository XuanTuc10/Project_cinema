namespace Project_cinema.Entities
{
    public class Seat : BaseEntity
    {
        public int Number { get; set; }
        public int SeatStatusID { get; set; }
        public string Line { get; set; }
        public int RoomID { get; set; }
        public bool? IsActive { get; set; }
        public int SeatTypeID { get; set; }
        public virtual SeatStatus? SeatStatus { get; set; }
        public virtual SeatType? SeatType { get; set; }  
        public virtual Room? Room { get; set; }
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
}
