using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.MarketplaceExchangeRate;
using Zaraye.Core;

namespace Zaraye.Services.MarketPlaceExchangerate
{
    public interface IMarketPlaceExchangerateService
    {
        Task<List<MarketplaceExchangeRate>> GetAllMarketPlaceExchangeRateByDateAsync(DateTime date);
        Task<MarketplaceExchangeRate> GetAllMarketPlaceExchangeRateByNameAsync(string currencyName, DateTime date);
    }
}
