namespace Project_cinema.Entities
{
    public class Room : BaseEntity
    {
        public int Capacity { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public int CinemaID { get; set; }
        public string Code { get; set; }    
        public string Name { get; set; }    
        public bool? IsActive { get; set; }
        public virtual Cinema? Cinema { get; set; }
        public virtual ICollection<Schedule>? Schedules { get; set; }
        public virtual ICollection<Seat>? Seats { get; set; }

    }
}
