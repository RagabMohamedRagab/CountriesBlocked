using CountriesBlocked.Application.Responses;
using CountriesBlocked.Domain.Entities;

namespace CountriesBlocked.Infrastructure.IManger
{
    public interface IBlockedCountriesStore
    {
       Task<BlockResponse<Blocked>> Add(Blocked blockCountry);

        Task<BlockResponse<Blocked>> DeleteBlocked(string CountryCode);

        Task<BlockResponse<List<Blocked>>> GetAll(int pageSize,int PageNumber,string? search);

        Task<BlockResponse<Blocked>> TemporarilyBlock(string CountryCode,int Minutes);

        Task<bool> CheckBlocked(string countryCode);
        Task CleanupExpiredBlocks();

    }
}
