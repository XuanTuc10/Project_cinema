namespace Project_cinema.Entities
{
    public class RankCustomer : BaseEntity
    {
        public int Poit { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool? isActive { get; set; }
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<Promotion>? Promotions { get; set; }
    }
}
