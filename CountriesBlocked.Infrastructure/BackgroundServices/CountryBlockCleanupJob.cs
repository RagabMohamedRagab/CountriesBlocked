using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CountriesBlocked.Infrastructure.IManger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CountriesBlocked.Infrastructure.BackgroundServices
{
    public class CountryBlockCleanupJob(IServiceProvider services):BackgroundService
    {
        private readonly IServiceProvider _services=services;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested) {
                using var scope = _services.CreateScope();
                var countryBlockService = scope.ServiceProvider.GetRequiredService<IBlockedCountriesStore>();

               await countryBlockService.CleanupExpiredBlocks();

                await Task.Delay(_interval,stoppingToken);
            }
        }
    }
}
