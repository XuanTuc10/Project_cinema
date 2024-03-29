using Project_cinema.DataContexts;
using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataMovie;
using Project_cinema.Payloads.DataResponses.DataSchedule;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Project_Room.Payloads.Converters;

namespace Project_cinema.Payloads.Converters
{
    public class MovieConverter
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly ScheduleConverter _scheduleConverter;
        public MovieConverter(ScheduleConverter converter)
        {
            _scheduleConverter = converter;
        }
        public DataResponseMovie EntityToDTO(Movie movie)
        {
            if (movie == null || movie.Id == null)
            {
                throw new ArgumentNullException("Movie is null or Movie.Id is null");
            }
            var movieItem = _context.movies
                .Include(x => x.MovieType)
                .Include(x => x.Rate)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == movie.Id);
            var scheduleItem = _context.schedules.Where(x => x.MovieID == movie.Id).Select(x => _scheduleConverter.EntityToDTO(x));

            return new DataResponseMovie
            {
                MovieDuration = movie.MovieDuration,
                EndTime = movie.EndTime,
                PremiereDate = movie.PremiereDate,
                Director = movie.Director,
                Image = movie.Image,
                HeroImage = movie.HeroImage,
                Language = movie.Language,
                MovieTypeName = movieItem.MovieType?.MovieTypeName,
                Name = movie.Name,
                CodeRate = movieItem.Rate?.Code,
                Trailer = movie.Trailer,
                IsActive = movie.IsActive,
                dataResponseSchedules = scheduleItem,

            };
        }
    }
}
