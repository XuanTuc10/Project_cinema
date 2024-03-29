using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataBillFood;
using Project_cinema.Payloads.DataResponses.DataBillTicket;

namespace Project_cinema.Payloads.DataResponses.DataOrder
{
    public class DataResponseOrder
    {
        public double TotalMoney { get; set; }
        public string TradingCode { get; set; }
        public DateTime CreateTime { get; set; }
        public string CustomerName { get; set; }
        public string Name { get; set; }
        public DateTime UpdateTime { get; set; }
        public string PromotionName { get; set; }
        public string BillStatusName { get; set; }
        public bool? IsActive { get; set; }
        public IQueryable<DataResponseBillFood>? dataResponseBillFoods { get; set; }
        public IQueryable<DataResponseBillTicket>? dataResponseBillTickets { get; set; }
    }
}
