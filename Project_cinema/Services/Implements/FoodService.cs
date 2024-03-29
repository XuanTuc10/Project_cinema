using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Handler.HandlePagination;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataRequests.FoodRequests;
using Project_cinema.Payloads.DataResponses.DataFood;
using Project_cinema.Payloads.Responses;
using Project_cinema.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project_cinema.Services.Implements
{
    public class FoodService : IFoodService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<DataResponseFood> _responseObject;
        private readonly FoodConverter _converter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FoodService(ResponseObject<DataResponseFood> responseObject, FoodConverter converter, IHttpContextAccessor httpContextAccessor)
        {
            _context = new AppDbContext();
            _responseObject = responseObject;
            _converter = converter;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseObject<DataResponseFood>> CreateFood(int userId, Request_CreateFood request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            Food food = new Food
            {
                Price = request.Price,
                Description = request.Description,
                Image = request.Image,
                NameOfFood = request.NameOfFood,
                IsActive = false
            };
            await _context.foods.AddAsync(food);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Thêm ghế thành công", _converter.EntityToDTO(food));
        }

        public async Task<string> DeleteFood(int userId, int foodId)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var food = await _context.foods.SingleOrDefaultAsync(x => x.Id == foodId);
            var billFoods = _context.billTickets.Where(x => x.Id == food.Id).ToList();
            foreach (var billFood in billFoods)
            {
                var bill = await _context.bills.SingleOrDefaultAsync(x => x.Id == billFood.BillID);
                bill.IsActive = false;
                _context.bills.Update(bill);
                await _context.SaveChangesAsync();
            }
            if (food == null)
            {
                return "Đồ ăn không tồn tại";
            }
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng không được xác thực hoặc không được xác định");
            }

            if (!currentUser.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException("Người dùng không có quyền sử dụng chức năng này");
            }
            _context.billTickets.RemoveRange(billFoods);
            _context.foods.Remove(food);
            await _context.SaveChangesAsync();
            return "Xóa ghế thành công";
        }

        public async Task<PageResult<DataResponseFood>> GetAllFood(int pageSize, int pageNumber)
        {
            var query = _context.foods.AsNoTracking().Select(x => _converter.EntityToDTO(x));
            var result = Pagination.GetPagedData(query, pageSize, pageNumber);
            return result;
        }

        public async Task<ResponseObject<DataResponseFood>> UpdateFood(int userId, Request_UpdateFood request)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.Id == userId && x.IsActive == true);
            var food = await _context.foods.SingleOrDefaultAsync(x => x.Id == request.Id);
            if (food == null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy đồ ăn", null);
            }
            food.Price = request.Price;
            food.Description = request.Description;
            food.Image = request.Image;
            food.NameOfFood = request.NameOfFood;
            _context.foods.Update(food);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Cập nhật thông tin thành công", _converter.EntityToDTO(food));
        }
        public async Task<PageResult<DataResponseFood>> GetFoodSevenDaysAgo(int pageSize, int pageNumber)
        {
            DateTime sevenDaysAgo = DateTime.Now.AddDays(-7);

            var foodStatistics = await _context.billFoods
                .Where(bf => _context.bills.Any(b => b.Id == bf.BillID && b.CreateTime >= sevenDaysAgo))
                .GroupBy(bf => bf.FoodID)
                .Select(g => new DataResponseFood
                {
                    Price = g.FirstOrDefault().Food.Price,
                    Description = g.FirstOrDefault().Food.Description,
                    Image = g.FirstOrDefault().Food.Image,
                    NameOfFood = g.FirstOrDefault().Food.NameOfFood,
                    IsActive = g.FirstOrDefault().Food.IsActive
                })
                .ToListAsync();

            var result = Pagination.GetPagedData(foodStatistics, pageSize, pageNumber);
            return result;
        }

    }
}
