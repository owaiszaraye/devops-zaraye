using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.MarketplaceExchangeRate;
using Zaraye.Core.Infrastructure;
using Zaraye.Data;

namespace Zaraye.Services.Common
{
    public class CommodityDataService : ICommodityDataService
    {
        public virtual async Task<List<CommodityData>> GetAllCommodityDataByDateAsync(DateTime date)
        {
            var _commodityDataRepository = EngineContext.Current.Resolve<IRepository<CommodityData>>();
            var query = _commodityDataRepository.Table;
            query = query.Where(item => !item.Deleted);
            query = query.Where(item => item.CreatedOnUtc.Date == date);
            query = query.OrderBy(item => item.Id);

            if (query.Count() <= 0)
            {
                DateTime dateTime = await GetLastDateTime();
                if (dateTime != DateTime.MinValue)
                    return await GetAllCommodityDataByDateAsync(dateTime.Date);
            }
            return await query.ToListAsync();
        }
        public virtual async Task<DateTime> GetLastDateTime()
        {
            var _commodityDataRepository = EngineContext.Current.Resolve<IRepository<CommodityData>>();

            var query = from pp in _commodityDataRepository.Table
                        where !pp.Deleted
                        orderby pp.CreatedOnUtc descending
                        select pp.CreatedOnUtc;
            var dateTime = query.FirstOrDefault();
            return dateTime.Date;
        }
    }
}
