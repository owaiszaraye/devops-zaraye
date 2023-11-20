using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Directory;
using Zaraye.Data;
using Zaraye.Services.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Directory
{
    /// <summary>
    /// State province service
    /// </summary>
    public partial class StateProvinceService : IStateProvinceService
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        protected readonly IZarayeDataProvider _zarayeDataProvider;

        #endregion

        #region Ctor

        public StateProvinceService(IStaticCacheManager staticCacheManager,
            ILocalizationService localizationService,
            IRepository<StateProvince> stateProvinceRepository,
            IZarayeDataProvider zarayeDataProvider)
        {
            _staticCacheManager = staticCacheManager;
            _localizationService = localizationService;
            _stateProvinceRepository = stateProvinceRepository;
            _zarayeDataProvider = zarayeDataProvider;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteStateProvinceAsync(StateProvince stateProvince)
        {
            await _stateProvinceRepository.DeleteAsync(stateProvince);
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province
        /// </returns>
        public virtual async Task<StateProvince> GetStateProvinceByIdAsync(int stateProvinceId)
        {
            return await _stateProvinceRepository.GetByIdAsync(stateProvinceId, cache => default);
        }

        /// <summary>
        /// Gets a state/province by abbreviation
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <param name="countryId">Country identifier; pass null to load the state regardless of a country</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province
        /// </returns>
        public virtual async Task<StateProvince> GetStateProvinceByAbbreviationAsync(string abbreviation, int? countryId = null)
        {
            if (string.IsNullOrEmpty(abbreviation))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeDirectoryDefaults.StateProvincesByAbbreviationCacheKey
                , abbreviation, countryId ?? 0);

            var query = _stateProvinceRepository.Table.Where(state => state.Abbreviation == abbreviation);

            //filter by country
            if (countryId.HasValue)
                query = query.Where(state => state.CountryId == countryId);

            return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Gets a state/province by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<StateProvince> GetStateProvinceByAddressAsync(Address address)
        {
            return await GetStateProvinceByIdAsync(address?.StateProvinceId ?? 0);
        }

        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        public virtual async Task<IList<StateProvince>> GetStateProvincesByCountryIdAsync(int countryId, int languageId = 0, bool showHidden = false)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeDirectoryDefaults.StateProvincesByCountryCacheKey, countryId, languageId, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = from sp in _stateProvinceRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.CountryId == countryId &&
                            (showHidden || sp.Published) && sp.ParentId == 0
                            && !sp.Deleted
                            select sp;
                var stateProvinces = await query.ToListAsync();

                if (languageId > 0)
                    //we should sort states by localized names when they have the same display order
                    stateProvinces = await stateProvinces.ToAsyncEnumerable()
                        .OrderBy(c => c.DisplayOrder)
                        .ThenByAwait(async c => await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId))
                        .ToListAsync();

                return stateProvinces;
            });
        }

        public virtual async Task<IList<StateProvince>> GetPriceDiscoveryStateProvincesByCountryIdAsync(int countryId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeDirectoryDefaults.PriceDiscoveryStateProvincesByCountryCacheKey, countryId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = from sp in _stateProvinceRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.CountryId == countryId &&
                            sp.PublishedOnPriceDiscovery && sp.ParentId == 0 && !sp.Deleted
                            select sp;
                var stateProvinces = await query.ToListAsync();
                return stateProvinces;
            });
        }

        public virtual async Task<IList<StateProvince>> GetAllStateProvincesByShowPriceDiscoveryAsync(bool showHidden = false, bool showPriceDiscovery = false)
        {
            var query = from sp in _stateProvinceRepository.Table
                        orderby sp.DisplayOrder, sp.Name
                        where
                        (showHidden || sp.Published) &&
                        (showPriceDiscovery || sp.PublishedOnPriceDiscovery)
                        && sp.ParentId == 0
                        && !sp.Deleted
                        select sp;
            var stateProvinces = await query.ToListAsync();

            return stateProvinces;
        }

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        public virtual async Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false)
        {
            var query = from sp in _stateProvinceRepository.Table
                        orderby sp.CountryId, sp.DisplayOrder, sp.Name
                        where showHidden || sp.Published
                        select sp;


            var stateProvinces = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(ZarayeDirectoryDefaults.StateProvincesAllCacheKey, showHidden), async () => await query.ToListAsync());

            return stateProvinces;
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertStateProvinceAsync(StateProvince stateProvince)
        {
            await _stateProvinceRepository.InsertAsync(stateProvince);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateStateProvinceAsync(StateProvince stateProvince)
        {
            await _stateProvinceRepository.UpdateAsync(stateProvince);
        }


        public virtual async Task<IList<StateProvince>> GetAllAreasByCityIdAsync(int cityId = 0, bool showHidden = false)
        {
            var query = from sp in _stateProvinceRepository.Table
                        orderby sp.DisplayOrder, sp.Name
                        where (sp.ParentId == cityId) && !sp.Deleted
                        select sp;

            if (showHidden)
                query = query.Where(x => x.Published);

            var areas = await query.ToListAsync();

            return areas;
        }

        public virtual async Task<StateProvince> GetStateProvinceByNameAsync(string name, int countryId)
        {
            if (string.IsNullOrEmpty(name) || countryId == 0)
                return null;

            var query = _stateProvinceRepository.Table.Where(state => state.CountryId == countryId && state.Name == name);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<StateProvince> GetAreaByNameAsync(string name, int cityId, int countryId)
        {
            if (string.IsNullOrEmpty(name) || cityId == 0 || countryId == 0)
                return null;

            var query = _stateProvinceRepository.Table.Where(state => state.CountryId == countryId && state.Name == name && state.ParentId == cityId);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<IList<CityCombinationList>> GetAllCityCombinationsListAsync(int industryId = 0, int categoryId = 0, int userId = 0)
        {
            var allCitiesCombinations = await _zarayeDataProvider.QueryProcAsync<CityCombinationList>("Generate_Cities_Combinations");
            return await allCitiesCombinations.AsQueryable().ToListAsync();
        }

        #endregion
    }
}