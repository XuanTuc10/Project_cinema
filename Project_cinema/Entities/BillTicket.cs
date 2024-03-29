namespace Project_cinema.Entities
{
    public class BillTicket : BaseEntity
    {
        public int Quantity { get; set; }
        public int BillID { get; set; } 
        public int TicketID { get; set; }
        public virtual Bill? Bill { get; set; }
        public virtual Ticket? Ticket { get; set; }  
    }
}
