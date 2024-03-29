using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Handler.HandleEmail;
using Project_cinema.Handler.HandleVnPay;
using Project_cinema.Payloads.DataRequests.VnPayRequests;
using Project_cinema.Payloads.DataResponses.DataVnPay;
using Project_cinema.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace Project_cinema.Services.Implements
{
    public class VnPayService : IVnPayService
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreatePaymentUrl(HttpContext context, int customerId, Request_VnPayModel model)
        {
            var customer = _context.users.SingleOrDefault(u => u.Id == customerId);
            if (customer == null)
            {
                // Xử lý trường hợp không tìm thấy khách hàng
                return null;
            }
            var checkBill = _context.bills.SingleOrDefault(x => x.CustomerID ==  customerId && x.Id == model.BillId);
            if (checkBill != null)
            {
                var tick = DateTime.Now.Ticks.ToString();
                var vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
                vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
                vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
                vnpay.AddRequestData("vnp_Amount", (checkBill.TotalMoney * 100).ToString());
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
                vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng:" + model.BillId.ToString());
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:PaymentBackReturnUrl"]);
                vnpay.AddRequestData("vnp_BuyerEmail", customer.Email);
                vnpay.AddRequestData("vnp_TxnRef", tick);

                var paymentUrl = vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"], _configuration["VnPay:HashSecret"]);
                return paymentUrl;
            }
            return null;
        }

            public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
            {
                var vnpay = new VnPayLibrary();
                foreach (var (key, value) in collections)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }

                var vnp_OrderId = vnpay.GetResponseData("vnp_TxnRef");
                var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
                var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
                var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]);
                if (!checkSignature)
                {
                    return new VnPaymentResponseModel()
                    {
                        Success = false
                    };
                }
                string message = SendEmail(new EmailTo
                {
                    To = collections["vnp_BuyerEmail"],
                    Subject = "VnPay Thông báo:",
                    Content = $"Bạn đã thanh toán đơn hàng thành công!"
                });
                return new VnPaymentResponseModel()
                {
                    Success = true,
                    PaymentMethod = "VnPay",
                    OrderDescription = vnp_OrderInfo,
                    OrderId = vnp_OrderId.ToString(),
                    TransactionId = vnp_TransactionId.ToString(),
                    Token = vnp_SecureHash,
                    VnPayResponseCode = vnp_ResponseCode

                };

            }
            public string SendEmail(EmailTo emailTo)
            {
                if (!Validate.IsValidEmail(emailTo.To))
                {
                    return "Định dạng email không hợp lệ";
                }
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("nguyenxuantuc1011@gmail.com", "nniq ejxg rvqs ckch"),
                    EnableSsl = true
                };
                try
                {
                    var message = new MailMessage();
                    message.From = new MailAddress("nguyenxuantuc1011@gmail.com");
                    message.To.Add(emailTo.To);
                    message.Subject = emailTo.Subject;
                    message.Body = emailTo.Content;
                    message.IsBodyHtml = true;
                    smtpClient.Send(message);

                    return "Xác nhận gửi email thành công";
                }
                catch (Exception ex)
                {
                    return "Lỗi khi gửi email: " + ex.Message;
                }
            }
    }
}
