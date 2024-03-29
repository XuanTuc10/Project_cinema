namespace Project_cinema.Entities
{
    public class Ticket : BaseEntity
    {
        public string Code { get; set; }    
        public int ScheduleID { get; set; } 
        public int? SeatID { get; set; }
        public virtual Seat? Seat { get; set; }
        public double PriceTicket { get; set; } 
        public bool? IsActive { get; set; }
        public virtual Schedule? Schedule { get; set; }
        public virtual ICollection<BillTicket>? BillTickets { get; set; }
    }
}
