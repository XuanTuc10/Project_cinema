namespace Project_cinema.Payloads.DataRequests.MovieRequests
{
    public class Request_UpdateMovie
    {
        public int Id { get; set; }
        public int MovieDuration { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime PremiereDate { get; set; }
        public string Director { get; set; }
        public string Image { get; set; }
        public string HeroImage { get; set; }
        public string Language { get; set; }
        public int MovieTypeID { get; set; }
        public string Name { get; set; }
        public int RateID { get; set; }
        public string Trailer { get; set; }
    }
}
