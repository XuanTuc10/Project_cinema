namespace Project_cinema.Entities
{
    public class Schedule : BaseEntity
    {
        public double Price { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Code { get; set; }    
        public int MovieID { get; set; }
        public string Name { get; set; }
        public int RoomID { get; set; }
        public bool? IsActive { get; set; }
        public virtual Movie? Movie { get; set; }
        public virtual Room? Room { get; set; }
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
}
