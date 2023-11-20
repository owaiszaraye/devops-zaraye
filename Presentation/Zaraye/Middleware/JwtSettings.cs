namespace Zaraye.Middleware
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public int ExpirationInMinutes { get; set; }
    }
}
