namespace Project_cinema.Entities
{
    public class BillStatus : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Bill>? Bills { get; set; }
    }
}
