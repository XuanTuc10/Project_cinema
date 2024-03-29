namespace Project_cinema.Payloads.DataRequests.UserRequests
{
    public class Request_ChangePassword
    {
        public string Token { get; set; }   
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
