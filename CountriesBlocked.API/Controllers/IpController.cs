using System.Net;
using CountriesBlocked.Application.Dtos;
using CountriesBlocked.Application.ThirdPartyClients;
using CountriesBlocked.Domain.Entities;
using CountriesBlocked.Infrastructure.IManger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CountriesBlocked.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpController(IBlockedAttemptsStore blockedAttemptsStore,ILocationService locationService,IBlockedCountriesStore blockedCountriesStore):ControllerBase
    {
        private readonly ILocationService _locationService = locationService;
        private readonly IBlockedCountriesStore _blockedCountriesStore = blockedCountriesStore;
        private readonly IBlockedAttemptsStore _blockedAttemptsStore = blockedAttemptsStore;


        [HttpGet("Lookup/{ipAddress}")]
        public async Task<IActionResult> Lookup(string? ipAddress)
        {
            if(string.IsNullOrEmpty(ipAddress)) {
                ipAddress=HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }
            return Ok(await _locationService.GetLocationByIpAsync(ipAddress));
        }
        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();  //??"8.8.8.8";
            var userAgent = Request.Headers["User-Agent"].ToString();
            var response = await _locationService.GetLocationByIpAsync(ip); // "8.8.8.8"
            if(response.Entity is null) {
                return Ok(response);
            }

            var isExist = await _blockedCountriesStore.CheckBlocked(response.Entity.Country_Code);
           
            await _blockedAttemptsStore.Add(new BlockedCountryAttempt() {
                IP=response.Entity.Ip!,
                Timestamp=DateTime.UtcNow,
                CountryCode=response.Entity.Country_Code,
                BlockedStatus=isExist,
                UserAgent=userAgent
            });

            return Ok(response);
        }
    }
}
