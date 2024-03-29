namespace Project_cinema.Payloads.DataRequests.FoodRequests
{
    public class Request_UpdateFood
    {
        public int Id { get; set; } 
        public double Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string NameOfFood { get; set; }
    }
}
