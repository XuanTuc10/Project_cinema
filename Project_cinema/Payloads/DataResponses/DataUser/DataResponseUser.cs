namespace Project_cinema.Payloads.DataResponses.DataUser
{
    public class DataResponseUser : DataResponseBase
    {
        public string UserName { get; set; }
        public int Point { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public string UserStatusName { get; set; }
        public string RankCustomerName { get; set; }
    }
}
