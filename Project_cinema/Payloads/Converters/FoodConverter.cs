using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataFood;

namespace Project_cinema.Payloads.Converters
{
    public class FoodConverter
    {
        public DataResponseFood EntityToDTO(Food food)
        {
            if (food == null || food.Id == null)
            {
                throw new ArgumentNullException("Food is null or Food.Id is null");
            }
            return new DataResponseFood
            {
                Price = food.Price,
                Description = food.Description,
                Image = food.Image,
                NameOfFood = food.NameOfFood,
                IsActive = food.IsActive
            };
        }
    }
}
