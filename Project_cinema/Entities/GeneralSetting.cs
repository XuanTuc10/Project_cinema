namespace Project_cinema.Entities
{
    public class GeneralSetting : BaseEntity
    {
        public DateTime BreakTime { get; set; }
        public int BusinessHours { get; set; }
        public DateTime CloseTime { get; set; }
        public double FixedTicketPride { get; set; }
        public int PercentDay { get; set; }
        public int PercentWeekend { get; set; }
        public DateTime TimeBeginToChange { get; set; }
    }
}
