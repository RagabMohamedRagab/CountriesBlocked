using System.Net;
using System.Text.RegularExpressions;
using CountriesBlocked.Application.Responses;
using CountriesBlocked.Domain.Entities;
using CountriesBlocked.Infrastructure.IManger;
using Microsoft.Extensions.Caching.Memory;

namespace CountriesBlocked.Infrastructure.Manger
{
    public class BlockedCountriesStore:IBlockedCountriesStore
    {
      
        private readonly IMemoryCache _cache;
        private const string BLOCKED_COUNTRIES_KEY = "blocked_countries";

        public BlockedCountriesStore(IMemoryCache cache)
        {
            _cache=cache;
        }

        private Dictionary<string,Blocked> GetBlockedCountries()
        {
            return _cache.GetOrCreate(BLOCKED_COUNTRIES_KEY,entry => {
                return new Dictionary<string,Blocked>();
            });
        }

        public async Task<BlockResponse<Blocked>> Add(Blocked blockCountry)
        {
            var response = new BlockResponse<Blocked>();
            var countries = GetBlockedCountries();
            if(!ValidCode(blockCountry.CountryCode)) {
                response.Status=HttpStatusCode.BadRequest;
                response.Message="Country Code Not Valid Format";
                return response;
            }
            if(countries.ContainsKey(blockCountry.CountryCode)) {
                response.Status=HttpStatusCode.BadRequest;
                response.Message="Country Code Already Exists";
                return response;
            }

            var blocked = new Blocked {
                IP=blockCountry.IP,
                CountryCode=blockCountry.CountryCode,
                CountryName=blockCountry.CountryName,
                IsBlocked=blockCountry.IsBlocked,
                CreatedOn=DateTime.UtcNow
            };

            countries[blockCountry.CountryCode]=blocked;
            // Set in Memory 
            _cache.Set(BLOCKED_COUNTRIES_KEY,countries);

            response.Status=HttpStatusCode.OK;
            response.Message="Successfully Blocked";
            response.Entity=blockCountry;
            return response;
        }

        public async Task<BlockResponse<Blocked>> DeleteBlocked(string countryCode)
        {
            var response = new BlockResponse<Blocked>();
            var countries = GetBlockedCountries();
           
            if(!countries.ContainsKey(countryCode)) {
                response.Status=HttpStatusCode.NoContent;
                response.Message="Country Code Does Not Exist";
                return response;
            }

            bool isRemoved = countries.Remove(countryCode);
            // Remove from cashe
            _cache.Set(BLOCKED_COUNTRIES_KEY,countries);

            if(isRemoved) {
                response.Status=HttpStatusCode.OK;
                response.Message="Successfully Deleted";
            } else {
                response.Status=HttpStatusCode.NotFound;
                response.Message="Failed to Delete";
            }

            return response;
        }

        public async Task<bool> CheckBlocked(string countryCode)
        {
            var countries = GetBlockedCountries();

            if(countries.ContainsKey(countryCode)) {
               
                return true;
            }
            return false;
        }

        public async Task<BlockResponse<List<Blocked>>> GetAll(int pageSize,int pageNumber,string search)
        {
            var response = new BlockResponse<List<Blocked>>();

            var countries = GetBlockedCountries();
            if(countries.Count<=0) {
                response.Status=HttpStatusCode.NoContent;
                response.Message="No Countries Avaliable";
                return response;
            }
             search=search??string.Empty;
             var filtered = countries.Values
                .Where(x=>x.CountryName.Contains(search)||x.CountryCode.Contains(search))
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .Select(b => new Blocked {
                    IP=b.IP,
                    CountryCode=b.CountryCode,
                    CountryName=b.CountryName,
                    IsBlocked=b.IsBlocked,
                     CreatedOn=b.CreatedOn,
                      ExpireAt=b.ExpireAt,
                })

                .ToList();


            response.Status=HttpStatusCode.OK;
            response.Message="Successfully Get";
            response.Entity=filtered;
            return response;
        }

        public async Task<BlockResponse<Blocked>> TemporarilyBlock(string CountryCode,int Minutes)
        {
            var response = new BlockResponse<Blocked>();

            var countries = GetBlockedCountries();

            if(!ValidCode(CountryCode)) {
                response.Status=HttpStatusCode.BadRequest;
                response.Message="Country Code Not Valid Format";
                return response;
            }

            if(countries!=null&&countries.Count>0&&countries.ContainsKey(CountryCode)) {
                if(countries[CountryCode].IsBlocked.Value) {
                    response.Status=HttpStatusCode.Conflict;
                    response.Message="Country already temporarily blocked";
                    return response;
                }
                countries[CountryCode].IsBlocked = true;
                countries[CountryCode].ExpireAt=DateTime.UtcNow.AddMinutes(Minutes);

                response.Status=HttpStatusCode.OK;
                response.Message="Successfully Blocked";
                response.Entity=countries[CountryCode];
                return response;
            }
            response.Status=HttpStatusCode.NoContent;
            response.Message="Country Not Exist";
            return response;
        }

        public async Task CleanupExpiredBlocks()
        {
            var countries = GetBlockedCountries();
            if(_cache.TryGetValue(BLOCKED_COUNTRIES_KEY,out Dictionary<string,Blocked> dict)) {
                var now = DateTime.UtcNow;
                var expired = dict.Where(kvp => kvp.Value.ExpireAt<=now).Select(kvp => kvp.Key).ToList();

                foreach(var key in expired) {
                    dict.Remove(key);
                }
            }
        }
        private bool ValidCode(string code)
        {
            return Regex.IsMatch(code,"^[A-Z]{2}$");
        }
    }

}

