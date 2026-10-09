namespace SecurityApplicationLogic.Settings
{
    public class JwtSettings
    {
        public string SigningKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int RefreshTokenDays { get; set; }
        public int AccessTokenMinutes { get; set; } 
    }
}
