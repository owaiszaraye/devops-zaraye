using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.MarketplaceExchangeRate;

namespace Zaraye.Services.Common
{
    public interface ICommodityDataService
    {
        Task<List<CommodityData>> GetAllCommodityDataByDateAsync(DateTime date);
    }
}
