namespace Project_cinema.Entities
{
    public class BillFood : BaseEntity
    {
        public int Quantity { get; set; }
        public int BillID { get; set; }
        public int FoodID { get; set; }
        public virtual Bill? Bill { get; set; }
        public virtual Food? Food { get; set; }
    }
}
