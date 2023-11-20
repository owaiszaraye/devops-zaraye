using LinqToDB.Data;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.PriceDiscovery;
using Zaraye.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.PriceDiscovery
{
    public partial class PriceDiscoveryService : IPriceDiscoveryService
    {
        #region Fields

        private readonly IRepository<DailyRate> _dailyRateRepository;
        private readonly IRepository<DailyRateMargin> _dailyRateMarginRepository;
        protected readonly IZarayeDataProvider _zarayeDataProvider;
        protected readonly IRepository<FavouriteRateGroup> _favouriteRateGroupRepository;

        #endregion

        #region Ctor

        public PriceDiscoveryService(
            IRepository<DailyRate> dailyRateRepository,
            IRepository<DailyRateMargin> dailyRateMarginRepository,
            IZarayeDataProvider zarayeDataProvider,
            IRepository<FavouriteRateGroup> favouriteRateGroupRepository
        )
        {
            _dailyRateRepository = dailyRateRepository;
            _dailyRateMarginRepository = dailyRateMarginRepository;
            _zarayeDataProvider = zarayeDataProvider;
            _favouriteRateGroupRepository = favouriteRateGroupRepository;
        }

        #endregion

        #region Methods

        #region Daily Rate

        public virtual async Task DeleteDailyRateAsync(DailyRate dailyRate)
        {
            await _dailyRateRepository.DeleteAsync(dailyRate);
        }

        public virtual async Task InsertDailyRateAsync(DailyRate dailyRate)
        {
            await _dailyRateRepository.InsertAsync(dailyRate);
        }

        public virtual async Task UpdateDailyRateAsync(DailyRate dailyRate)
        {
            await _dailyRateRepository.UpdateAsync(dailyRate);
        }

        public virtual async Task<DailyRate> GetDailyRateByIdAsync(int dailyRateId)
        {
            return await _dailyRateRepository.GetByIdAsync(dailyRateId);
        }

        public virtual async Task<IPagedList<DailyRate>> GetAllDailyRatesAsync(int supplierId = 0, int industryId = 0, int categoryId = 0,
         int productId = 0, int attributeValueId = 0,
         int brandId = 0, DateTime? dateFrom = null, DateTime? dateTo = null,
         int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {

            var query = from dr in _dailyRateRepository.Table
                        where !dr.Deleted
                        select dr;

            if (dateFrom.HasValue)
                query = query.Where(b => dateFrom.Value <= b.CreatedOnUtc);

            if (dateTo.HasValue)
                query = query.Where(b => dateTo.Value >= b.CreatedOnUtc);

            if (supplierId > 0)
                query = query.Where(b => supplierId == b.SupplierId);

            if (industryId > 0)
                query = query.Where(b => industryId == b.IndustryId);

            if (categoryId > 0)
                query = query.Where(b => categoryId == b.CategoryId);

            if (productId > 0)
                query = query.Where(b => productId == b.ProductId);

            if (attributeValueId > 0)
                query = query.Where(b => attributeValueId == b.AttributeValueId);

            if (brandId > 0)
                query = query.Where(b => brandId == b.BrandId);


            query = query.OrderByDescending(b => b.CreatedOnUtc);

            var dailyRates = await query.ToPagedListAsync(pageIndex, pageSize);
            return dailyRates;
        }

        public virtual async Task<IList<ProductCombinationList>> GetAllProductCombinationsListAsync(int categoryId = 0)
        {
            var allProductsCombinations = await _zarayeDataProvider.QueryProcAsync<ProductCombinationList>("Generate_Products_Combinations",
                 new DataParameter("@P_CategoryId", categoryId, LinqToDB.DataType.Int32));
            return await allProductsCombinations.AsQueryable().ToListAsync();
        }

        public virtual async Task<IPagedList<TodayRateList>> GetGetTodayRatesListAsync(int statusId, int industryId = 0, int categoryId = 0,
            int userId = 0, bool? published = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = await _zarayeDataProvider.QueryProcAsync<TodayRateList>("get_today_rates",
                new DataParameter("@p_IndustryId", industryId, LinqToDB.DataType.Int32),
                new DataParameter("@p_CategoryId", categoryId, LinqToDB.DataType.Int32),
                new DataParameter("@p_UserId", userId, LinqToDB.DataType.Int32),
                new DataParameter("@p_StatusId", statusId, LinqToDB.DataType.Int32),
                new DataParameter("@p_ShowPublished", published, LinqToDB.DataType.Boolean));

            return await query.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #region Daily Rate Margin


        public virtual async Task DeleteDailyRateMarginAsync(DailyRateMargin dailyRateMargin)
        {
            await _dailyRateMarginRepository.DeleteAsync(dailyRateMargin);
        }
        public virtual async Task InsertDailyRateMarginAsync(DailyRateMargin dailyRateMargin)
        {
            await _dailyRateMarginRepository.InsertAsync(dailyRateMargin);
        }

        public virtual async Task UpdateDailyRateMarginAsync(DailyRateMargin dailyRateMargin)
        {
            await _dailyRateMarginRepository.UpdateAsync(dailyRateMargin);
        }

        public virtual async Task<DailyRateMargin> GetDailyRateMarginByIdAsync(int dailyRateMarginId)
        {
            return await _dailyRateMarginRepository.GetByIdAsync(dailyRateMarginId);
        }

        public virtual async Task<IPagedList<DailyRateMargin>> GetAllDailyRateMarginsAsync(int dailyRateId = 0, int cityId = 0,
         int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {

            var query = from drm in _dailyRateMarginRepository.Table
                        join dr in _dailyRateRepository.Table on drm.DailyRateId equals dr.Id
                        where !dr.Deleted
                        select drm;

            if (cityId > 0)
                query = query.Where(b => cityId == b.CityId);

            if (dailyRateId > 0)
                query = query.Where(b => dailyRateId == b.DailyRateId);

            var dailyRateMargins = await query.ToPagedListAsync(pageIndex, pageSize);
            return dailyRateMargins;
        }

        public virtual async Task<decimal?> GetMarginRateAsync(int dailyRateId, int cityId)
        {
            if (dailyRateId == 0 || cityId == 0)
                return 0M;

            var margin = (await _dailyRateMarginRepository.Table.FirstOrDefaultAsync(o => o.DailyRateId == dailyRateId && o.CityId == cityId))?.MarginRate;
            return margin;
        }

        #endregion

        #region Favourite Rate Group

        public virtual async Task<FavouriteRateGroup> GetFavouriteRateGroupByIdAsync(int id)
        {
            return await _favouriteRateGroupRepository.GetByIdAsync(id, cache => default);
        }

        public virtual async Task<FavouriteRateGroup> GetFavouriteRateGroupAsync(int customerId, int productId, int attributeValueId, int brandId)
        {
            return await (from r in _favouriteRateGroupRepository.Table
                          where r.CustomerId == customerId && r.ProductId == productId && r.AttributeValueId == attributeValueId && r.BrandId == brandId
                          select r).FirstOrDefaultAsync();
        }

        public virtual async Task<IList<FavouriteRateGroup>> GetAllFavouriteRateGroupListAsync()
        {
            var query = from r in _favouriteRateGroupRepository.Table
                        orderby r.CreatedOnUtc descending
                        select r;

            return await query.ToListAsync();
        }

        public virtual async Task InsertFavouriteRateGroupAsync(FavouriteRateGroup rateGroup)
        {
            await _favouriteRateGroupRepository.InsertAsync(rateGroup);
        }

        public virtual async Task UpdateFavouriteRateGroupAsync(FavouriteRateGroup rateGroup)
        {
            await _favouriteRateGroupRepository.UpdateAsync(rateGroup);
        }

        public virtual async Task DeleteFavouriteRateGroupAsync(FavouriteRateGroup rateGroup)
        {
            await _favouriteRateGroupRepository.DeleteAsync(rateGroup);
        }

        #endregion

        #endregion
    }
}
