namespace Project_cinema.Entities
{
    public class ConfirmEmail : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime ExpiredTime { get; set; }
        public string ConfirmCode { get; set; }
        public bool IsConfirm { get; set; } = false;
    }
}
