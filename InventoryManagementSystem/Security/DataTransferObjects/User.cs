

namespace SecurityApplicationPresentation.DataTransferObjects
{
    public class User
    {
        public Guid? ID { get; set; } = Guid.Empty;
        public string? Username { get; set; }
        public string Email { get; set; }=  string.Empty;

        public byte AccountStatus { get; set; } = 0;

        public byte EmailVerificationStatus { get; set; } = 0;

        public Guid? UserRole { get; set; }

        public bool LocalAccount { get; set; } = true;

    }
}
