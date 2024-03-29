namespace Project_cinema.Entities
{
    public class Cinema : BaseEntity
    {
        public string Address { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string NameOfCinema { get; set; }
        public bool? IsActive { get; set; }
        public virtual ICollection<Room>? Rooms { get; set; }
    }
}
