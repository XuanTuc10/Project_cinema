using Project_cinema.Entities;

namespace Project_cinema.Payloads.DataRequests.BillFoodRequest
{
    public class Request_BillFood
    {
        public int Quantity { get; set; }
        public int FoodID { get; set; }
    }
}
