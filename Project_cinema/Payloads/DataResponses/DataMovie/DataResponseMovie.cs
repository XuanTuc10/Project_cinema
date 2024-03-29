using Project_cinema.Entities;
using Project_cinema.Payloads.DataResponses.DataSchedule;

namespace Project_cinema.Payloads.DataResponses.DataMovie
{
    public class DataResponseMovie
    {
        public int MovieDuration { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime PremiereDate { get; set; }
        public string Director { get; set; }
        public string Image { get; set; }
        public string HeroImage { get; set; }
        public string Language { get; set; }
        public string MovieTypeName { get; set; }
        public string Name { get; set; }
        public string CodeRate { get; set; }
        public string Trailer { get; set; }
        public bool? IsActive { get; set; }
        public IQueryable<DataResponseSchedule>? dataResponseSchedules { get; set; }
    }
}
