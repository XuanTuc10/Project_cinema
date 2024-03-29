namespace Project_cinema.Entities
{
    public class MovieType : BaseEntity
    {
        public string MovieTypeName { get; set; }
        public bool? IsActive { get; set; }
        public virtual ICollection<Movie>? Movies { get; set; }
    }
}
