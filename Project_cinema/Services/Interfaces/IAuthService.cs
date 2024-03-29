using Project_cinema.Payloads.DataRequests.TokenRequests;
using Project_cinema.Payloads.DataRequests.UserRequests;
using Project_cinema.Payloads.DataResponses.DataToken;
using Project_cinema.Payloads.DataResponses.DataUser;
using Project_cinema.Payloads.Responses;

namespace Project_cinema.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseObject<DataResponseUser>> Register(Request_Register request);
        Task<ResponseObject<DataResponseToken>> Login(Request_Login request);
        ResponseObject<DataResponseToken> RenewAccessToken(Request_Token request);
        Task<ResponseObject<DataResponseUser>> ConfirmCreateNewPassword(Request_ConfirmNewPassword request);
        Task<ResponseObject<DataResponseUser>> ForgotPassword(Request_ForgotPassword request);
        Task<ResponseObject<DataResponseUser>> ConfirmCreateNewAccount(Request_ConfirmCreateNewAccount request);
        Task<ResponseObject<DataResponseUser>> ChangePassword(int userId, Request_ChangePassword request);
    }
}
