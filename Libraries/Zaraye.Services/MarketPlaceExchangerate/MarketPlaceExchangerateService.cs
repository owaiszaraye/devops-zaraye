using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.EmployeeInsights;
using Zaraye.Core;
using Zaraye.Core.Domain.MarketplaceExchangeRate;
using Zaraye.Core.Infrastructure;
using Zaraye.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Stores;
using Irony.Parsing;

namespace Zaraye.Services.MarketPlaceExchangerate
{
    public class MarketPlaceExchangerateService : IMarketPlaceExchangerateService
    {
        public virtual async Task<List<MarketplaceExchangeRate>> GetAllMarketPlaceExchangeRateByDateAsync(DateTime date)
        {
            var _marketplaceExchangeRateRepository = EngineContext.Current.Resolve<IRepository<MarketplaceExchangeRate>>();

            var query = _marketplaceExchangeRateRepository.Table;
            query = query.Where(item => !item.Deleted);
            query = query.Where(item => item.CreatedOnUtc.Date == date);
            query = query.OrderBy(item => item.Id);

            if (query.Count() <= 0)
            {
                DateTime dateTime = await GetLastDateTime();
                if (dateTime != DateTime.MinValue)
                    return await GetAllMarketPlaceExchangeRateByDateAsync(dateTime.Date);
            }
            return await query.ToListAsync();
        }
        public virtual async Task<DateTime> GetLastDateTime()
        {
            var _marketplaceExchangeRateRepository = EngineContext.Current.Resolve<IRepository<MarketplaceExchangeRate>>();

            var query = from pp in _marketplaceExchangeRateRepository.Table
                        where !pp.Deleted
                        orderby pp.CreatedOnUtc descending
                        select pp.CreatedOnUtc;
            var dateTime = query.FirstOrDefault();
            return dateTime.Date;
        }

        public async Task<MarketplaceExchangeRate> GetAllMarketPlaceExchangeRateByNameAsync(string currencyName, DateTime date)
        {
            var _marketplaceExchangeRateRepository = EngineContext.Current.Resolve<IRepository<MarketplaceExchangeRate>>();
            var query = _marketplaceExchangeRateRepository.Table;
            query = query.Where(item => !item.Deleted);
            query = query.Where(item => item.CreatedOnUtc.Date == date && item.Currency == currencyName);
            query = query.OrderBy(item => item.Id);
            return await query.FirstOrDefaultAsync();
        }
    }
}
