using CountriesBlocked.Infrastructure.IManger;
using Microsoft.AspNetCore.Mvc;

namespace CountriesBlocked.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController(IBlockedAttemptsStore blockedAttemptsStore):ControllerBase
    {
        private readonly IBlockedAttemptsStore _blockedAttemptsStore = blockedAttemptsStore;

        [HttpGet("blocked-attempts")]
        public async Task<IActionResult> BlockedAttempts()
        {
            return Ok(await _blockedAttemptsStore.GetAll());
        }
    }
}
