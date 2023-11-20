using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.PriceDiscovery;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.PriceDiscovery
{
    public partial interface IPriceDiscoveryService
    {
        #region Methods

        #region Daily Rate

        Task DeleteDailyRateAsync(DailyRate dailyRate);

        Task InsertDailyRateAsync(DailyRate dailyRate);

        Task UpdateDailyRateAsync(DailyRate dailyRate);

        Task<DailyRate> GetDailyRateByIdAsync(int dailyRateId);

        Task<IPagedList<DailyRate>> GetAllDailyRatesAsync(int supplierId = 0, int industryId = 0, int categoryId = 0,
         int productId = 0, int attributeValueId = 0,
         int brandId = 0, DateTime? dateFrom = null, DateTime? dateTo = null,
         int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        Task<IList<ProductCombinationList>> GetAllProductCombinationsListAsync(int categoryId = 0);

        Task<IPagedList<TodayRateList>> GetGetTodayRatesListAsync(int statusId, int industryId = 0, int categoryId = 0,
            int userId = 0, bool? published = null, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion

        #region Daily Rate Margin

        Task DeleteDailyRateMarginAsync(DailyRateMargin dailyRateMargin);

        Task InsertDailyRateMarginAsync(DailyRateMargin dailyRateMargin);

        Task UpdateDailyRateMarginAsync(DailyRateMargin dailyRateMargin);

        Task<DailyRateMargin> GetDailyRateMarginByIdAsync(int dailyRateMarginId);


        Task<IPagedList<DailyRateMargin>> GetAllDailyRateMarginsAsync(int dailyRateId = 0, int cityId = 0,
         int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        Task<decimal?> GetMarginRateAsync(int dailyRateId, int cityId);

        #endregion

        #region Favourite Rate Group

        Task<FavouriteRateGroup> GetFavouriteRateGroupByIdAsync(int id);

        Task<FavouriteRateGroup> GetFavouriteRateGroupAsync(int customerId, int productId, int attributeValueId, int brandId);

        Task<IList<FavouriteRateGroup>> GetAllFavouriteRateGroupListAsync();

        Task InsertFavouriteRateGroupAsync(FavouriteRateGroup rateGroup);

        Task UpdateFavouriteRateGroupAsync(FavouriteRateGroup rateGroup);

        Task DeleteFavouriteRateGroupAsync(FavouriteRateGroup rateGroup);

        #endregion

        #endregion
    }
}
