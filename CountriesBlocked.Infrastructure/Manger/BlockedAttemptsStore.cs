using System.Collections.Concurrent;
using System.Net;
using CountriesBlocked.Application.Dtos;
using CountriesBlocked.Application.Responses;
using CountriesBlocked.Infrastructure.IManger;

namespace CountriesBlocked.Infrastructure.Manger
{
    public class BlockedAttemptsStore:IBlockedAttemptsStore
    {
        private static ConcurrentDictionary<string,BlockedCountryAttempt> _BlockedAttempts = new();

        public async Task Add(BlockedCountryAttempt attempt)
        {
            _BlockedAttempts[attempt.CountryCode]=attempt;
        }

        public async Task<BlockResponse<List<BlockedCountryAttempt>>> GetAll()
        {
            return new BlockResponse<List<BlockedCountryAttempt>>() {
                Entity=_BlockedAttempts.Values.ToList(),
                Message="Blocked country attempts retrieved successfully.",
                Status=HttpStatusCode.OK,
            };
        }
    }
}
