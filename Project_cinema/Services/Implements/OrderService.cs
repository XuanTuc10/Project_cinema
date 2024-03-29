using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.BillFoodRequest;
using Project_cinema.Payloads.DataRequests.BillTicketRequest;
using Project_cinema.Payloads.DataRequests.FoodRequests;
using Project_cinema.Payloads.DataRequests.OrderRequests;
using Project_cinema.Payloads.DataRequests.ScheduleRequests;
using Project_cinema.Payloads.DataResponses.DataOrder;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Project_cinema.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseOrder> _responseObject;
        private readonly OrderConverter _orderConverter;

        public OrderService(ResponseObject<DataResponseOrder> responseObject, OrderConverter OrderConverter)
        {
            _context = new AppDbContext();
            _responseObject = responseObject;
            _orderConverter = OrderConverter;
        }
        public async Task<ResponseObject<DataResponseOrder>> CreateOrder(int customerId, Request_CreateOrder request)
        {
            var promotionId = _context.users
                .Where(user => user.Id == customerId)
                .Include(user => user.RankCustomer)
                    .ThenInclude(rankCustomer => rankCustomer.Promotions)
                .AsEnumerable()
                .SelectMany(user => user.RankCustomer.Promotions.Select(rankCustomer => rankCustomer.Id))
                .FirstOrDefault();
            var soOrder = _context.bills.Count();
            Bill bill = new Bill();
            bill.CustomerID = customerId;
            bill.TradingCode = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}_00{soOrder}";
            bill.CreateTime = DateTime.Now;
            bill.UpdateTime = DateTime.Now;
            bill.Name = request.Name;
            bill.TotalMoney = 0;
            bill.BillStatusID = 1;
            bill.PromotionID = promotionId;
            bill.IsActive = false;
            bill.BillFoods = null;
            bill.BillTicket = null;
            _context.bills.Add(bill);
            _context.SaveChanges();
            bill.BillTicket = AddTicket(bill.Id, request.billTickets);
            if (bill.BillTicket is null)
            {
                _context.bills.Remove(bill);
                _context.SaveChanges();
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Ticket chua ton tai. Vui long kiem tra lai!", null);
            }
            bill.BillFoods = AddListFood(bill.Id, request.billFoods);
            if (bill.BillFoods is null)
            {
                _context.bills.Remove(bill);
                _context.SaveChanges();
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Food chua ton tai. Vui long kiem tra lai!", null);
            }

            _context.bills.Update(bill);
            _context.SaveChanges();
            bill.TotalMoney = CalculateTotalBill(bill.Id);
            _context.bills.Update(bill);
            _context.SaveChanges();
            return _responseObject.ResponseSuccess("Them bill thanh cong", _orderConverter.EntityToDTO(bill));
        }

        private double CalculateTotalBill(int billId)
        {
            var bill = _context.bills
                              .Include(b => b.BillFoods)
                              .Include(b => b.BillTicket)
                              .FirstOrDefault(b => b.Id == billId);

            if (bill == null)
            {
                return 0;
            }

            double total = 0;
            foreach (var billFood in bill.BillFoods)
            {
                total += billFood.Quantity * billFood.Food.Price;
            }

            total += bill.BillTicket.Quantity * bill.BillTicket.Ticket.PriceTicket;

            int customerRank = bill.Customer?.RankCustomer?.Poit ?? 0;

            double discountPercent = 0;
            if (bill.Promotion != null && bill.Promotion.Quantity != 0)
            {
                discountPercent = bill.Promotion.Percent;
                bill.Promotion.Quantity--;
                _context.promotions.Update(bill.Promotion);
                _context.SaveChanges();
            }

            double discount = total * (discountPercent / 100);
            double totalAfterDiscount = total - discount;

            return totalAfterDiscount;
        }

        private List<BillFood> AddListFood(int billId, List<Request_BillFood> requests)
        {
            var bill = _context.bills.SingleOrDefault(x => x.Id == billId);
            if (bill is null)
            {
                return null;
            }
            List<BillFood> billFoods = new List<BillFood>();
            foreach (var request in requests)
            {
                BillFood billFood = new BillFood();
                var food = _context.foods.SingleOrDefault(x => x.Id == request.FoodID);
                if (food is null)
                {
                    return null;
                }
                billFood.BillID = billId;
                billFood.Quantity = request.Quantity;
                billFood.FoodID = request.FoodID;
                billFoods.Add(billFood);
            }
            _context.billFoods.AddRange(billFoods);
            _context.SaveChanges();
            return billFoods;
        }
        private BillTicket AddTicket(int billId, Request_BillTicket request)
        {
            var bill = _context.bills.SingleOrDefault(x => x.Id == billId);
            if (bill is null)
            {
                return null;
            }
            BillTicket billTicket = new BillTicket();
            var Ticket = _context.tickets.SingleOrDefault(x => x.Id == request.TicketID);
            if (Ticket is null)
            {
                return null;
            }
            billTicket.BillID = billId;
            billTicket.Quantity = request.Quantity;
            billTicket.TicketID = request.TicketID;
            _context.billTickets.AddRange(billTicket);
            _context.SaveChanges();
            return billTicket;
        }
    }
}
