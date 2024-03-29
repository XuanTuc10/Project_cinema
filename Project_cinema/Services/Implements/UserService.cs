using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.UserRequests;
using Project_cinema.Payloads.DataResponses.DataUser;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;

namespace Project_cinema.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly ResponseObject<DataResponseUser> _responseObject;
        private readonly UserConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(ResponseObject<DataResponseUser> responseObject, UserConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }

        /*public async Task<string> ChangeDecentralization(Request_ChangeDecentralization request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == request.UserId && x.isActive == true);
            if (user is null)
            {
                return "Không tìm thấy id người dùng";
            }
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return "Người dùng không được xác thực hoặc không được xác định";
            }

            if (!currentUser.IsInRole("admin"))
            {
                return "Người dùng không có quyền sử dụng chức năng này";
            }
            user.RoleID = request.RoleId;
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return "Thay đổi quyền người dùng thành công";
        }*/

        /*public async Task<string> DeleteUser(int userId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.isActive == true);
            if(user is null)
            {
                return "Người dùng không tồn tại";
            }
            user.isActive = false;
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return "Xóa thông tin người dùng thành công";
        }*/

        public async Task<PageResult<DataResponseUser>> GetAllUsers(int pageSize, int pageNumber)
        {
            var query = _context.users.Where(x => x.IsActive == true).Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseUser>> GetUserById(int userId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            if(user is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy người dùng", null);
            }
            return _responseObject.ResponseSuccess("Lấy thông tin thành công", _converter.EntityToDTO(user));
        }

        public async Task<PageResult<DataResponseUser>> GetUserByName(string? name, int pageSize, int pageNumber)
        {
            var query = _context.users.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).Where(x => x.IsActive == true).Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<PageResult<DataResponseUser>> GetUserByRole(int roleId, int pageSize, int pageNumber)
        {
            var query = _context.users.Where(x => x.IsActive == true && x.RoleID == roleId).Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        /*public async Task<DataResponseUserInformation> GetUserInformation(int userId)
        {
            var user = await _context.users.Include(x => x.Posts).SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true && x.IsLocked == false);
            if(user == null)
            {
                throw new ArgumentNullException("Không tìm thấy thông tin người dùng");
            }
            int listReports = _context.reports.Include(x => x.UserReport).Include(x => x.Post).AsNoTracking().Count(x => x.UserReportId == userId);
            int numberPost = _context.posts.Include(x => x.User).AsNoTracking().Count(x => x.UserId == userId);
            int numberOfFollower = _context.relationships.Include(x => x.Follower).Include(x => x.Follower).AsNoTracking().Count(x => x.Following.Id == userId);
            int numberOfFollowing = _context.relationships.Include(x => x.Follower).Include(x => x.Following).AsNoTracking().Count(x => x.Follower.Id == userId);
            DataResponseUserInformation data = new DataResponseUserInformation();
            data.PostNumber = numberPost;
            data.NumberOfFollower = numberOfFollower;
            data.NumberOfFollowing = numberOfFollowing;
            data.Posts = user.Posts.Select(x => _postConverter.EntityToDTO(x)).AsQueryable();
            return data;
        }*/

        /*public async Task<string> LockAccount(int adminId, int userLockedId)
        {
            var admin = await _context.users.SingleOrDefaultAsync(x => x.Id == adminId && x.isActive == true && x.IsLocked == false);
            var userLocked = await _context.users.SingleOrDefaultAsync(x => x.Id == userLockedId && x.isActive == true && x.IsLocked == false);
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return "Người dùng không được xác thực hoặc không được xác định";
            }

            if (!currentUser.IsInRole("Admin"))
            {
                return "Người dùng không có quyền sử dụng chức năng này";
            }
            if (userLocked == null)
            {
                return "Người dùng không tồn tại";
            }
            if (userLocked.IsLocked == true)
            {
                return "Tài khoản này đã bị khóa";
            }
            userLocked.IsLocked = true;
            _context.users.Update(userLocked);
            await _context.SaveChangesAsync();
            return "Khóa tài khoản người dùng thành công";
        }*/

        /*public async Task<string> UnLockAccount(int adminId, int userUnLockedId)
        {
            var admin = await _context.users.SingleOrDefaultAsync(x => x.Id == adminId && x.IsActive == true && x.IsLocked == false);
            var userLocked = await _context.users.SingleOrDefaultAsync(x => x.Id == userUnLockedId);
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return "Người dùng không được xác thực hoặc không được xác định";
            }

            if (!currentUser.IsInRole("Admin"))
            {
                return "Người dùng không có quyền sử dụng chức năng này";
            }
            if (userLocked == null)
            {
                return "Người dùng không tồn tại";
            }
            if (userLocked.IsLocked == false)
            {
                return "Tài khoản này chưa bị khóa";
            }
            userLocked.IsLocked = false;
            _context.users.Update(userLocked);
            await _context.SaveChangesAsync();
            return "Mở khóa tài khoản người dùng thành công";
        }*/

        /*public async Task<ResponseObject<DataResponseUser>> UpdateUser(int userId, Request_UpdateUserInfor request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.isActive == true);
            if(string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || request.PhoneNumber == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            if(user == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Không tìm thấy thông tin người dùng", null);
            }
            user.Name = request.Name;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật thông tin người dùng thành công", _converter.EntityToDTO(user));
        }*/
    }
}
