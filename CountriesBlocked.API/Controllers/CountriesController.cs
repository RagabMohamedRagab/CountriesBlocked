using CountriesBlocked.Application.Dtos;
using CountriesBlocked.Domain.Entities;
using CountriesBlocked.Infrastructure.IManger;
using Microsoft.AspNetCore.Mvc;

namespace CountriesBlocked.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(IBlockedCountriesStore blockedCountriesStore):ControllerBase
    {
        private readonly IBlockedCountriesStore _blockedCountriesStore=blockedCountriesStore; 

        [HttpPost("Block")]
        public async Task<ActionResult> Block([FromBody] Blocked countryDTO)
        {
            return Ok(await _blockedCountriesStore.Add(countryDTO));
        }
        [HttpDelete("{CountryCode}")]
        public async Task<ActionResult> Block(string CountryCode)
        {
            if(string.IsNullOrEmpty(CountryCode))
                return BadRequest();
            return Ok(await _blockedCountriesStore.DeleteBlocked(CountryCode));
        }

        [HttpGet("Blocked")]

        public async Task<ActionResult> Blocked(int PageSize,int PageNumber,string? Search)
        {
            return Ok(await _blockedCountriesStore.GetAll(PageSize,PageNumber,Search));
        }
        [HttpPost("temporal-block")]
        public async Task<IActionResult> TemporalBlock([FromBody] TemporalBlockRequest request)
        {
            var response = await _blockedCountriesStore.TemporarilyBlock(request.CountryCode,request.DurationMinutes);
                return Ok(response);
          

           
        }
    }
}
