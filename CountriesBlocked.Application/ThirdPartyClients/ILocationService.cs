
using CountriesBlocked.Application.Dtos;
using CountriesBlocked.Application.Responses;

namespace CountriesBlocked.Application.ThirdPartyClients
{
    public interface ILocationService
    {
        Task<BlockResponse<IpLocationResponse>> GetLocationByIpAsync(string ip);

    }
}
