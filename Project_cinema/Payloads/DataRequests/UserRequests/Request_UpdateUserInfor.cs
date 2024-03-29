using System.ComponentModel.DataAnnotations;

namespace Project_cinema.Payloads.DataRequests.UserRequests
{
    public class Request_UpdateUserInfor
    {
        public string Name { get; set; }
        public string Email { get; set; }
        /*[DataType(DataType.Upload)]
        public IFormFile Avatar { get; set; }*/
        public string PhoneNumber { get; set; }
    }
}
