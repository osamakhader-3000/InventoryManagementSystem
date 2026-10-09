namespace UsersApplicationPresentation.DataTransferObjects
{
    public class LoginAttempt
    {
        public Int64 ID { get; set; } = 0;
        public Guid? UserID { get; set; }
        public DateTime? LoginTime { get; set; }
        public Int16 LoginStatus { get; set; }
    }
}
