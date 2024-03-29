using Project_cinema.Entities;

namespace Project_cinema.Payloads.DataResponses.DataFood
{
    public class DataResponseFood
    {
        public double Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string NameOfFood { get; set; }
        public bool? IsActive { get; set; }
    }
}
