namespace Project_cinema.Entities
{
    public class Bill : BaseEntity
    {
        public double TotalMoney { get; set; }
        public string TradingCode { get; set; }
        public DateTime CreateTime { get; set; }
        public int? CustomerID { get; set; }
        public string Name { get; set; }
        public DateTime UpdateTime { get; set; }
        public int PromotionID { get; set; }
        public int BillStatusID { get; set; }
        public bool? IsActive { get; set; }
        public virtual User? Customer { get; set; }
        public virtual Promotion? Promotion { get; set; }
        public virtual BillStatus? BillStatus { get; set; }
        public virtual ICollection<BillFood> BillFoods { get; set; }
        public virtual BillTicket BillTicket { get; set; }
    }
}
