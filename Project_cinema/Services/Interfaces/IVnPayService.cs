using Project_cinema.Handler.HandleEmail;
using Project_cinema.Payloads.DataRequests.VnPayRequests;
using Project_cinema.Payloads.DataResponses.DataVnPay;

namespace Project_cinema.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context,int customerId, Request_VnPayModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
