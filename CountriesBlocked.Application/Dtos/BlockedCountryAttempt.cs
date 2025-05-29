
namespace CountriesBlocked.Application.Dtos
{
    public class BlockedCountryAttempt
    {
        public bool BlockedStatus { get; set; }
        public DateTime Timestamp { get; set; }
        public string CountryCode { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }
    }
}
