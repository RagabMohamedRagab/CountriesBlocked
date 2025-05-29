using System.Collections.Generic;
using CountriesBlocked.Application.Dtos;
using CountriesBlocked.Application.Responses;

namespace CountriesBlocked.Infrastructure.IManger
{
    public interface IBlockedAttemptsStore
    {
        Task Add(BlockedCountryAttempt attempt);

        Task<BlockResponse<List<BlockedCountryAttempt>>> GetAll();
    }
}

