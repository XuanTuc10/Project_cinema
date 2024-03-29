using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataUser;

namespace Project_cinema.Payloads.Converters
{
    public class UserConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        public DataResponseUser EntityToDTO(User user)
        {
            if (user == null || user.Id == null)
            {
                throw new ArgumentNullException("User is null or User.Id is null");
            }

            var userItem = _context.users
                .Include(x => x.Role)
                .Include(x => x.UserStatus)
                .Include(x=>x.RankCustomer)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == user.Id);

            if (userItem == null)
            {
                return null;
            }

            return new DataResponseUser
            {
                Id = user.Id,
                Point = user.Point,
                UserName = user.Username,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                RoleName = userItem.Role?.Name,
                RankCustomerName = userItem.RankCustomer?.Name,
                UserStatusName = userItem.UserStatus?.Name
            };
        }
    }
}
