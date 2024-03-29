using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataBillFood;
using Project_cinema.Payloads.DataResponses.DataOrder;

namespace Project_cinema.Payloads.Converters
{
    public class BillFoodConverter
    {
        private readonly AppDbContext _context;
        public BillFoodConverter()
        {
            _context = new AppDbContext();
        }
        public DataResponseBillFood EntityToDTO(BillFood billFood)
        {
            if (billFood == null || billFood.Id == null)
            {
                throw new ArgumentNullException("billFood is null or billFood.Id is null");
            }
            var foodItem = _context.billFoods
                .Include(x => x.Food)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == billFood.Id);

            return new DataResponseBillFood
            {
                FoodName = foodItem.Food?.NameOfFood,
                Price = foodItem.Food.Price,
                Quantity = billFood.Quantity
            };
        }
    }
}
