

namespace CountriesBlocked.Application.Dtos
{
    public class IpLocationResponse
    {
        public string Ip { get; set; }
        public string Country_Code { get; set; }
        public string Continent_Name { get; set; }
        public string Region_Name { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }
}
