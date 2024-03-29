using Project_cinema.Payloads.DataResponses.DataUser;

namespace Project_cinema.Payloads.DataResponses.DataToken
{
    public class DataResponseToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DataResponseUser DataResponseUser { get; set; }
    }
}
