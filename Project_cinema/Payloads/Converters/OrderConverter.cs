using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataOrder;

namespace Project_cinema.Payloads.Converters
{
    public class OrderConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly BillFoodConverter _billFoodConverter;
        private readonly BillTicketConverter _billTicketConverter;
        public OrderConverter(BillFoodConverter billFoodConverter, BillTicketConverter billTicketConverter)
        {
            _billFoodConverter = billFoodConverter;
            _billTicketConverter = billTicketConverter;
        }
        public DataResponseOrder EntityToDTO(Bill bill)
        {
            if (bill == null || bill.Id == null)
            {
                throw new ArgumentNullException("Bill is null or Bill.Id is null");
            }
            var billItem = _context.bills
                .Include(x => x.BillStatus)
                .Include(x => x.Customer)
                .Include(x => x.Promotion)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == bill.Id);
            var billFoodItem = _context.billFoods.Where(x => x.BillID == bill.Id).Select(x => _billFoodConverter.EntityToDTO(x));
            var billTicketItem = _context.billTickets.Where(x => x.BillID == bill.Id).Select(x => _billTicketConverter.EntityToDTO(x));

            return new DataResponseOrder
            {
                TotalMoney = bill.TotalMoney,
                TradingCode = bill.TradingCode,
                CreateTime = bill.CreateTime,
                CustomerName = billItem.Customer?.Name,
                PromotionName = billItem.Promotion?.Name,
                BillStatusName = billItem.BillStatus?.Name,
                Name = bill.Name,
                UpdateTime = bill.UpdateTime,
                IsActive = bill.IsActive,
                dataResponseBillFoods = billFoodItem,
                dataResponseBillTickets = billTicketItem

            };
        }
    }
}
