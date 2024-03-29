using Microsoft.EntityFrameworkCore;
using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataCinema;
using Project_Room.Payloads.Converters;

namespace Project_cinema.Payloads.Converters
{
    public class CinemaConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly RoomConverter _converter;
        public CinemaConverter(RoomConverter converter)
        {
            _converter = converter;
        }
        public DataResponseCinema EntityToDTO(Cinema cinema)
        {
            if (cinema == null || cinema.Id == null)
            {
                throw new ArgumentNullException("Cinema is null or Cinema.Id is null");
            }
            var roomItem = _context.rooms.Where(x => x.CinemaID == cinema.Id).Select(x => _converter.EntityToDTO(x));

            return new DataResponseCinema
            {
                Id = cinema.Id,
                Address = cinema.Address,
                Description = cinema.Description,
                Code = cinema.Code,
                NameOfCinema = cinema.NameOfCinema,
                IsActive = cinema.IsActive,
                dataResponseRooms = roomItem
            };
        }
    }
}
