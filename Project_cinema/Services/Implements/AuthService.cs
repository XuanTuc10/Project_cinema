using Project_cinema.DataContexts;
using Project_cinema.Handler.HandleEmail;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.UserRequests;
using Project_cinema.Payloads.DataResponses.DataToken;
using Project_cinema.Payloads.DataResponses.DataUser;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using Project_cinema.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Project_cinema.Constants;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using BcryptNet = BCrypt.Net.BCrypt;
using System.Text;
using Project_cinema.Payloads.DataRequests.TokenRequests;

namespace Project_cinema.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly ResponseObject<DataResponseUser> _responseObject;
        private readonly UserConverter _converter;
        private readonly IConfiguration _configuration;
        private readonly ResponseObject<DataResponseToken> _responseTokenObject;
        public AuthService(ResponseObject<DataResponseUser> responseObject, UserConverter converter, IConfiguration configuration, ResponseObject<DataResponseToken> responseTokenObject)
        {
            _responseObject = responseObject;
            _converter = converter;
            _configuration = configuration;
            _responseTokenObject = responseTokenObject;
        }
        #region Đăng nhập
        public async Task<ResponseObject<DataResponseToken>> Login(Request_Login request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            var user = await _context.users.FirstOrDefaultAsync(x => x.Username.Equals(request.UserName));
            if (user is null)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status404NotFound, "Tên tài khoản không tồn tại trên hệ thống", null);
            }
            if (user.UserStatusID == 1 || user.IsActive == false)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Tài khoản chưa được kích hoạt hoặc đã bị xóa, vui lòng kích hoạt tài khoản", null);
            }
            /*if(user.IsLocked == true)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Tài khoản đã bị khóa", null);
            }*/
            bool checkPass = BcryptNet.Verify(request.Password, user.Password);
            if (!checkPass)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Mật khẩu không chính xác", null);
            }
            else
            {
                return _responseTokenObject.ResponseSuccess("Đăng nhập thành công", GenerateAccessToken(user));
            }
        }
        #endregion
        #region Renew Access Token
        public ResponseObject<DataResponseToken> RenewAccessToken(Request_Token request)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration.GetSection("AppSettings:SecretKey").Value;

                var tokenValidation = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection(AppSettingsKeys.AUTH_SECRET).Value!))
                };

                var tokenAuthentication = jwtTokenHandler.ValidateToken(request.AccessToken, tokenValidation, out var validatedToken);

                if (!(validatedToken is JwtSecurityToken jwtSecurityToken) || jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Token không hợp lệ", null);
                }

                var refreshToken = _context.refreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);

                if (refreshToken == null)
                {
                    return _responseTokenObject.ResponseError(StatusCodes.Status404NotFound, "RefreshToken không tồn tại trong database", null);
                }

                if (refreshToken.ExpiredTime < DateTime.Now)
                {
                    return _responseTokenObject.ResponseError(StatusCodes.Status401Unauthorized, "Token đã hết hạn", null);
                }

                var user = _context.users.FirstOrDefault(x => x.Id == refreshToken.UserId);

                if (user == null)
                {
                    return _responseTokenObject.ResponseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
                }

                var newToken = GenerateAccessToken(user);

                return _responseTokenObject.ResponseSuccess("Làm mới token thành công", newToken);
            }
            catch (SecurityTokenValidationException ex)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status400BadRequest, "Lỗi xác thực token: " + ex.Message, null);
            }
            catch (Exception ex)
            {
                return _responseTokenObject.ResponseError(StatusCodes.Status500InternalServerError, "Lỗi không xác định: " + ex.Message, null);
            }
        }
        #endregion
        #region Đăng ký tài khoản
        public async Task<ResponseObject<DataResponseUser>> Register(Request_Register request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName)
             || string.IsNullOrWhiteSpace(request.Password)
             || string.IsNullOrWhiteSpace(request.Email)
             || string.IsNullOrWhiteSpace(request.Name)
                )
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            if (!Validate.IsValidEmail(request.Email))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng email không hợp lệ", null);
            }
            if (!Validate.IsValidPhoneNumber(request.PhoneNumber))
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng phonenumber không hợp lệ", null);
            }
            if (await _context.users.FirstOrDefaultAsync(x => x.Username.Equals(request.UserName)) != null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Tên tài khoản đã tồn tại trên hệ thống", null);
            }
            if (await _context.users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email)) != null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Email đã tồn tại trên hệ thống", null);
            }
            try
            {
                User user = new User();
                user.Username = request.UserName;
                user.Password = BcryptNet.HashPassword(request.Password);
                user.Name = request.Name;
                user.Email = request.Email;
                user.Point = 0;
                user.PhoneNumber = request.PhoneNumber;
                user.IsActive = true;
                user.RoleID = 1;
                user.UserStatusID = 1;
                user.RankCustomerID = 1;
                /*user.IsLocked = false;*/
                await _context.users.AddAsync(user);
                await _context.SaveChangesAsync();
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    UserId = user.Id,
                    ConfirmCode = GenerateCodeActive().ToString(),
                    ExpiredTime = DateTime.Now.AddMinutes(30),
                    IsConfirm = false
                };
                await _context.confirmEmails.AddAsync(confirmEmail);
                await _context.SaveChangesAsync();
                string message = SendEmail(new EmailTo
                {
                    To = request.Email,
                    Subject = "Nhận mã xác nhận để xác nhận đăng ký tài khoản của bạn tại đây: ",
                    Content = $"Mã kích hoạt của bạn là: {confirmEmail.ConfirmCode}, mã này có hiệu lực trong 30 phút"
                });
                return _responseObject.ResponseSuccess("Bạn đã đăng ký tài khoản, nhận mã xác nhận xong email để kích hoạt tài khoản", _converter.EntityToDTO(user));
            }
            catch (Exception ex)
            {
                return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
        #endregion
        #region Tạo access token
        public DataResponseToken GenerateAccessToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection(AppSettingsKeys.AUTH_SECRET).Value!);

            var decentralization = _context.roles.FirstOrDefault(x => x.Id == user.RoleID);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserName", user.Username),
                    new Claim("RoleID", user.RoleID.ToString()),
                    new Claim(ClaimTypes.Role, decentralization?.Code ?? "")
                }),
                Expires = DateTime.Now.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            RefreshToken rf = new RefreshToken
            {
                Token = refreshToken,
                ExpiredTime = DateTime.Now.AddHours(4),
                UserId = user.Id
            };

            _context.refreshTokens.Add(rf);
            _context.SaveChanges();

            DataResponseToken tokenDTO = new DataResponseToken
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                DataResponseUser = _converter.EntityToDTO(user)
            };
            return tokenDTO;
        }
        #endregion
        #region Tạo refresh token
        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        #endregion
        #region Hỗ trợ gửi email
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

                return "Xác nhận gửi email thành công, lấy mã để xác thực";
            }
            catch (Exception ex)
            {
                return "Lỗi khi gửi email: " + ex.Message;
            }
        }
        #endregion
        #region Code để active tài khoản
        private int GenerateCodeActive()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }
        #endregion
        public async Task<ResponseObject<DataResponseUser>> ConfirmCreateNewPassword(Request_ConfirmNewPassword request)
        {
            ConfirmEmail confirmEmail = await _context.confirmEmails.Where(x => x.ConfirmCode.Equals(request.ConfirmCode)).FirstOrDefaultAsync();
            if (confirmEmail is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận không chính xác", null);
            }
            if (confirmEmail.ExpiredTime < DateTime.Now)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận đã hết hạn", null);
            }
            User user = _context.users.FirstOrDefault(x => x.Id == confirmEmail.UserId);
            user.Password = BcryptNet.HashPassword(request.NewPassword);
            _context.confirmEmails.Remove(confirmEmail);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Tạo mật khẩu mới thành công", _converter.EntityToDTO(user));
        }



        public async Task<ResponseObject<DataResponseUser>> ForgotPassword(Request_ForgotPassword request)
        {
            User user = await _context.users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email));
            if (user is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Email không tồn tại trong hệ thống", null);
            }
            else
            {
                var confirms = _context.confirmEmails.Where(x => x.UserId == user.Id).ToList();
                _context.confirmEmails.RemoveRange(confirms);
                await _context.SaveChangesAsync();
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    UserId = user.Id,
                    IsConfirm = false,
                    ExpiredTime = DateTime.Now.AddMinutes(30),
                    ConfirmCode = "Pin" + "_" + GenerateCodeActive().ToString()
                };
                await _context.confirmEmails.AddAsync(confirmEmail);
                await _context.SaveChangesAsync();
                string message = SendEmail(new EmailTo
                {
                    To = request.Email,
                    Subject = "Nhận mã xác nhận để tạo mật khẩu mới từ đây: ",
                    Content = $"Mã kích hoạt của bạn là: {confirmEmail.ConfirmCode}, mã này sẽ hết hạn sau 4 tiếng"
                });
                return _responseObject.ResponseSuccess("Gửi mã xác nhận về email thành công, vui lòng kiểm tra email", _converter.EntityToDTO(user));
            }
        }

        public async Task<ResponseObject<DataResponseUser>> ConfirmCreateNewAccount(Request_ConfirmCreateNewAccount request)
        {
            ConfirmEmail confirmEmail = await _context.confirmEmails.Where(x => x.ConfirmCode.Equals(request.ConfirmCode)).FirstOrDefaultAsync();
            if (confirmEmail is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Mã xác nhận không chính xác", null);
            }
            if (confirmEmail.ExpiredTime < DateTime.Now)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận đã hết hạn", null);
            }
            User user = _context.users.FirstOrDefault(x => x.Id == confirmEmail.UserId);
            user.UserStatusID = 2;
            _context.confirmEmails.Remove(confirmEmail);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Xác nhận đăng ký tài khoản thành công, vui lòng đăng nhập tài khoản của bạn", _converter.EntityToDTO(user));
        }

        public async Task<ResponseObject<DataResponseUser>> ChangePassword(int userId, Request_ChangePassword request)
        {
            var user = await _context.users.FirstOrDefaultAsync(x => x.Id == userId);
            bool checkPass = BcryptNet.Verify(request.OldPassword, user.Password);
            if (!checkPass)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mật khẩu cũ không chính xác", null);
            }
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mật khẩu không trùng nhau! Vui lòng thử lại", null);
            }
            user.Password = BcryptNet.HashPassword(request.NewPassword);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thay đổi mật khẩu thành công", _converter.EntityToDTO(user));
        }
    }
}
