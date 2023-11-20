using Zaraye.Core;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data;
using Zaraye.Services.Customers;
using Zaraye.Services.Localization;
using Zaraye.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Catalog
{
    public partial class IndustryService : IIndustryService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Industry> _industryRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IRepository<IndustryIndustryTagMapping> _industryTagMappingRepository;
        private readonly IRepository<IndustryTag> _industryTagRepository;

        #endregion

        #region Ctor

        public IndustryService(
            IAclService aclService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IRepository<Industry> industryRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IWorkContext workContext,
            IRepository<IndustryIndustryTagMapping> industryTagMappingRepository,
            IRepository<IndustryTag> industryTagRepository)
        {
            _aclService = aclService;
            _customerService = customerService;
            _localizationService = localizationService;
            _industryRepository = industryRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _workContext = workContext;
            _industryTagMappingRepository = industryTagMappingRepository;
            _industryTagRepository = industryTagRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent industry identifier</param>
        /// <param name="ignoreIndustriesWithoutExistingParent">A value indicating whether categories without parent industry in provided industry list (source) should be ignored</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sorted categories
        /// </returns>
        protected virtual async Task<IList<Industry>> SortIndustriesForTreeAsync(IList<Industry> source, int parentId = 0,
            bool ignoreIndustriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<Industry>();

            foreach (var cat in source/*.Where(c => c.ParentIndustryId == parentId)*/.ToList())
            {
                result.Add(cat);
                result.AddRange(await SortIndustriesForTreeAsync(source, cat.Id, true));
            }

            if (ignoreIndustriesWithoutExistingParent || result.Count == source.Count)
                return result;

            //find categories without parent in provided industry source and insert them into result
            foreach (var cat in source)
                if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                    result.Add(cat);

            return result;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Delete industry
        /// </summary>
        /// <param name="industry">Industry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteIndustryAsync(Industry industry)
        {
            await _industryRepository.DeleteAsync(industry);
        }

        /// <summary>
        /// Delete Industries
        /// </summary>
        /// <param name="categories">Industries</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteIndustriesAsync(IList<Industry> categories)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));

            foreach (var industry in categories)
                await DeleteIndustryAsync(industry);
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        public virtual async Task<IList<Industry>> GetAllIndustriesAsync(int storeId = 0, bool showHidden = false, bool? isAppPublished = null)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.IndustriesAllCacheKey,
                storeId,
                await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                showHidden);

            var industries = await _staticCacheManager
                .GetAsync(key, async () => (await GetAllIndustriesAsync(string.Empty, storeId, showHidden: showHidden)).ToList());

            return industries;
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Industry name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        public virtual async Task<IPagedList<Industry>> GetAllIndustriesAsync(string categoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null, bool? isAppPublished = null)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.IndustriesAllCacheKey);
            var Industries = await _staticCacheManager.GetAsync(key, async () =>
            {

                var unsortedIndustries = await _industryRepository.GetAllAsync(async query =>
                {
                    if (isAppPublished.HasValue && isAppPublished.Value)
                        query = query.Where(m => m.AppPublished);
                    else
                    {
                        if (!showHidden)
                            query = query.Where(c => c.Published);
                        else if (overridePublished.HasValue)
                            query = query.Where(c => c.Published == overridePublished.Value);
                        if (!showHidden)
                        {
                            //apply ACL constraints
                            var customer = await _workContext.GetCurrentCustomerAsync();
                            query = await _aclService.ApplyAcl(query, customer);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(categoryName))
                        query = query.Where(c => c.Name.Contains(categoryName));

                    query = query.Where(c => !c.Deleted);

                    return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                });

                //sort categories
                //var sortedIndustries = await SortIndustriesForTreeAsync(unsortedIndustries);

                //paging
                return new PagedList<Industry>(unsortedIndustries, pageIndex, pageSize);
            });
            return Industries;
        }

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        public virtual async Task<IList<Industry>> GetAllIndustriesDisplayedOnHomepageAsync(bool showHidden = false)
        {
            var categories = await _industryRepository.GetAllAsync(query =>
            {
                return from c in query
                       orderby c.DisplayOrder, c.Id
                       where c.Published &&
                             !c.Deleted &&
                             c.ShowOnHomepage
                       select c;
            }, cache => cache.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.IndustriesHomepageCacheKey));

            if (showHidden)
                return categories;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.IndustriesHomepageWithoutHiddenCacheKey,
                await _storeContext.GetCurrentStoreAsync(), await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()));

            var result = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                return await categories
                    .WhereAwait(async c => await _aclService.AuthorizeAsync(c))
                    .ToListAsync();
            });

            return result;
        }

        /// <summary>
        /// Gets a industry
        /// </summary>
        /// <param name="categoryId">Industry identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the industry
        /// </returns>
        public virtual async Task<Industry> GetIndustryByIdAsync(int categoryId)
        {
            return await _industryRepository.GetByIdAsync(categoryId, cache => default);
        }

        /// <summary>
        /// Inserts industry
        /// </summary>
        /// <param name="industry">Industry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertIndustryAsync(Industry industry)
        {
            await _industryRepository.InsertAsync(industry);
        }

        /// <summary>
        /// Updates the industry
        /// </summary>
        /// <param name="industry">Industry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateIndustryAsync(Industry industry)
        {
            if (industry == null)
                throw new ArgumentNullException(nameof(industry));

            await _industryRepository.UpdateAsync(industry);
        }

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing categories
        /// </returns>
        public virtual async Task<string[]> GetNotExistingIndustriesAsync(string[] categoryIdsNames)
        {
            if (categoryIdsNames == null)
                throw new ArgumentNullException(nameof(categoryIdsNames));

            var query = _industryRepository.Table;
            var queryFilter = categoryIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = await query.Select(c => c.Name)
                .Where(c => queryFilter.Contains(c))
                .ToListAsync();

            queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (!queryFilter.Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = await query.Select(c => c.Id.ToString())
                .Where(c => queryFilter.Contains(c))
                .ToListAsync();

            return queryFilter.Except(filter).ToArray();
        }

        /// <summary>
        /// Gets categories by identifier
        /// </summary>
        /// <param name="categoryIds">Industry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        public virtual async Task<IList<Industry>> GetIndustriesByIdsAsync(int[] categoryIds)
        {
            return await _industryRepository.GetByIdsAsync(categoryIds, includeDeleted: false);
        }

        /// <summary>
        /// Get formatted industry breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="industry">Industry</param>
        /// <param name="allIndustries">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted breadcrumb
        /// </returns>
        public virtual async Task<string> GetFormattedBreadCrumbAsync(Industry industry, IList<Industry> allIndustries = null,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = await GetIndustryBreadCrumbAsync(industry, allIndustries, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = await _localizationService.GetLocalizedAsync(breadcrumb[i], x => x.Name, languageId);
                result = string.IsNullOrEmpty(result) ? categoryName : $"{result} {separator} {categoryName}";
            }

            return result;
        }

        /// <summary>
        /// Get industry breadcrumb 
        /// </summary>
        /// <param name="industry">Industry</param>
        /// <param name="allIndustries">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the industry breadcrumb 
        /// </returns>
        public virtual async Task<IList<Industry>> GetIndustryBreadCrumbAsync(Industry industry, IList<Industry> allIndustries = null, bool showHidden = false)
        {
            if (industry == null)
                throw new ArgumentNullException(nameof(industry));

            var breadcrumbCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.IndustryBreadcrumbCacheKey,
                industry,
                await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                await _storeContext.GetCurrentStoreAsync(),
                await _workContext.GetWorkingLanguageAsync());

            return await _staticCacheManager.GetAsync(breadcrumbCacheKey, async () =>
            {
                var result = new List<Industry>();

                //used to prevent circular references
                var alreadyProcessedIndustryIds = new List<int>();

                while (industry != null && //not null
                       !industry.Deleted && //not deleted
                       (showHidden || industry.Published) && //published
                       (showHidden || await _aclService.AuthorizeAsync(industry)) && //ACL
                       !alreadyProcessedIndustryIds.Contains(industry.Id)) //prevent circular references
                {
                    result.Add(industry);

                    alreadyProcessedIndustryIds.Add(industry.Id);
                }

                result.Reverse();

                return result;
            });
        }

        #endregion

        #region CustomMehtod
        public virtual async Task<IList<Industry>> SearchIndustriesAsync(string keywords = null, bool searchIndustryTags = false, bool showHidden = false, bool? overridePublished = null, bool? isAppPublished = null)
        {
            var query = _industryRepository.Table;

            query = query.Where(c => !c.Deleted);

            if (!showHidden)
                query = query.Where(c => c.Published);
            else if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            if (!string.IsNullOrEmpty(keywords))
            {
                IQueryable<int> IndustriesByKeywords;

                IndustriesByKeywords =
                        from c in _industryRepository.Table
                        where c.Name.Contains(keywords)
                        select c.Id;

                if (searchIndustryTags)
                {
                    IndustriesByKeywords = IndustriesByKeywords.Union(
                        from cctm in _industryTagMappingRepository.Table
                        join ct in _industryTagRepository.Table on cctm.IndustryTagId equals ct.Id
                        where ct.Name == keywords
                        select cctm.IndustryId
                    );

                }

                query =
                    from p in query
                    from pbk in LinqToDB.LinqExtensions.InnerJoin(IndustriesByKeywords, pbk => pbk == p.Id)
                    select p;
            }

            return query.ToList();
        }
        #endregion
    }
}