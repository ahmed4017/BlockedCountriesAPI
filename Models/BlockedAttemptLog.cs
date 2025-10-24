namespace BlockedCountriesAPI.Models
{
    public class BlockedAttemptLog
    {
        public string IpAddress { get; set; } = "";
        public string CountryCode { get; set; } = "";
        public bool IsBlocked { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UserAgent { get; set; } = "";
        public string CountryName { get; set; } = "";
        public string Isp { get; set; } = "";

    }

}
