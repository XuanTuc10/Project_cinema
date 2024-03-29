using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.Converters;
using Project_cinema.Payloads.DataResponses.DataRoom;

namespace Project_Room.Payloads.Converters
{
    public class RoomConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly SeatConverter _seatConverter;
        private readonly ScheduleConverter _scheduleConverter;
        public RoomConverter(SeatConverter seatConverter, ScheduleConverter scheduleConverter)
        {
            _seatConverter = seatConverter;
            _scheduleConverter = scheduleConverter;
        }
        public DataResponseRoom EntityToDTO(Room room)
        {
            if (room == null || room.Id == null)
            {
                throw new ArgumentNullException("Room is null or Room.Id is null");
            }
            var roomItem = _context.rooms
                .Include(x => x.Cinema)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == room.Id);
            var seatItem = _context.seats.Where(x => x.RoomID == room.Id).Select(x => _seatConverter.EntityToDTO(x));
            var scheduleItem = _context.schedules.Where(x => x.RoomID == room.Id).Select(x => _scheduleConverter.EntityToDTO(x));

            return new DataResponseRoom
            {
                Capacity = room.Capacity,
                Type = room.Type,
                Description = room.Description,
                Code = room.Code,
                NameCinema = roomItem.Cinema?.NameOfCinema,
                IsActive = room.IsActive,
                dataResponseSchedules = scheduleItem,
                dataResponseSeats = seatItem
            };
        }
    }
}
