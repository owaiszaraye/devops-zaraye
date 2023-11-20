using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core;
using Zaraye.Data;
using Zaraye.Services.Customers;
using Zaraye.Services.Security;
using Zaraye.Services.Seo;
using Zaraye.Services.Stores;

namespace Zaraye.Services.Catalog.Caching
{
    public partial class IndustryTagService : IIndustryTagService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<Industry> _industryRepository;
        private readonly IRepository<IndustryIndustryTagMapping> _industryIndustryTagMappingRepository;
        private readonly IRepository<IndustryTag> _industryTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public IndustryTagService(
            IAclService aclService,
            ICustomerService customerService,
            IRepository<Industry> industryRepository,
            IRepository<IndustryIndustryTagMapping> industryIndustryTagMappingRepository,
            IRepository<IndustryTag> industryTagRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _customerService = customerService;
            _industryRepository = industryRepository;
            _industryIndustryTagMappingRepository = industryIndustryTagMappingRepository;
            _industryTagRepository = industryTagRepository;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Delete a category-category tag mapping
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="categoryTagId">Category tag identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task DeleteIndustryIndustryTagMappingAsync(int industryId, int industryTagId)
        {
            var mappingRecord = await _industryIndustryTagMappingRepository.Table
                .FirstOrDefaultAsync(pptm => pptm.IndustryId == industryId && pptm.IndustryTagId == industryTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await _industryIndustryTagMappingRepository.DeleteAsync(mappingRecord);
        }

        /// <summary>
        /// Indicates whether a product tag exists
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<bool> IndustryTagExistsAsync(Industry industry, int industryTagId)
        {
            if (industry == null)
                throw new ArgumentNullException(nameof(industry));

            return await _industryIndustryTagMappingRepository.Table
                .AnyAsync(pptm => pptm.IndustryId == industry.Id && pptm.IndustryTagId == industryTagId);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag
        /// </returns>
        protected virtual async Task<IndustryTag> GetIndustryTagByNameAsync(string name)
        {
            var query = from pt in _industryTagRepository.Table
                        where pt.Name == name
                        select pt;

            var industryTag = await query.FirstOrDefaultAsync();
            return industryTag;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertIndustryTagAsync(IndustryTag industryTag)
        {
            await _industryTagRepository.InsertAsync(industryTag);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteIndustryTagAsync(IndustryTag industryTag)
        {
            await _industryTagRepository.DeleteAsync(industryTag);
        }

        /// <summary>
        /// Delete product tags
        /// </summary>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteIndustryTagsAsync(IList<IndustryTag> industryTags)
        {
            if (industryTags == null)
                throw new ArgumentNullException(nameof(industryTags));

            foreach (var industryTag in industryTags)
                await DeleteIndustryTagAsync(industryTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<IndustryTag>> GetAllIndustryTagsAsync(string tagName = null)
        {
            var allIndustryTags = await _industryTagRepository.GetAllAsync(query => query, getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName))
                allIndustryTags = allIndustryTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allIndustryTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<IndustryTag>> GetAllIndustryTagsByIndustryIdAsync(int industryId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.IndustryTagsByIndustryCacheKey, industryId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var tagMapping = from ptm in _industryIndustryTagMappingRepository.Table
                                 join pt in _industryTagRepository.Table on ptm.IndustryTagId equals pt.Id
                                 where ptm.IndustryId == industryId
                                 orderby pt.Id
                                 select pt;

                return await tagMapping.ToListAsync();
            });
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag
        /// </returns>
        public virtual async Task<IndustryTag> GetIndustryTagByIdAsync(int industryTagId)
        {
            return await _industryTagRepository.GetByIdAsync(industryTagId, cache => default);
        }

        /// <summary>
        /// Gets product tags
        /// </summary>
        /// <param name="productTagIds">Product tags identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<IndustryTag>> GetIndustryTagsByIdsAsync(int[] industryTagIds)
        {
            return await _industryTagRepository.GetByIdsAsync(industryTagIds);
        }

        /// <summary>
        /// Inserts a product-product tag mapping
        /// </summary>
        /// <param name="tagMapping">Product-product tag mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertIndustryIndustryTagMappingAsync(IndustryIndustryTagMapping tagMapping)
        {
            await _industryIndustryTagMappingRepository.InsertAsync(tagMapping);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateIndustryTagAsync(IndustryTag industryTag)
        {
            if (industryTag == null)
                throw new ArgumentNullException(nameof(industryTag));

            await _industryTagRepository.UpdateAsync(industryTag);

            var seName = await _urlRecordService.ValidateSeNameAsync(industryTag, string.Empty, industryTag.Name, true);
            await _urlRecordService.SaveSlugAsync(industryTag, seName, 0);
        }

        /// <summary>
        /// Get products quantity linked to a passed tag identifier
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of products
        /// </returns>
        public virtual async Task<int> GetIndustryCountByIndustryTagIdAsync(int industryTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetIndustryCountAsync(storeId, showHidden);
            if (dictionary.ContainsKey(industryTagId))
                return dictionary[industryTagId];

            return 0;
        }

        /// <summary>
        /// Get product count for every linked tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dictionary of "product tag ID : product count"
        /// </returns>
        public virtual async Task<Dictionary<int, int>> GetIndustryCountAsync(int storeId, bool showHidden = false)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.IndustryTagCountCacheKey, storeId, customerRoleIds, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = _industryIndustryTagMappingRepository.Table;

                if (!showHidden)
                {
                    var industriesQuery = _industryRepository.Table.Where(p => p.Published);


                    query = query.Where(pc => industriesQuery.Any(p => !p.Deleted && pc.IndustryId == p.Id));
                }

                var pTagCount = from pt in _industryTagRepository.Table
                                join ptm in query on pt.Id equals ptm.IndustryTagId
                                group ptm by ptm.IndustryTagId into ptmGrouped
                                select new
                                {
                                    IndustryTagId = ptmGrouped.Key,
                                    IndustryCount = ptmGrouped.Count()
                                };

                return pTagCount.ToDictionary(item => item.IndustryTagId, item => item.IndustryCount);
            });
        }

        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateIndustryTagsAsync(Industry industry, string[] industryTags)
        {
            if (industry == null)
                throw new ArgumentNullException(nameof(industry));

            //industry tags
            var existingIndustryTags = await GetAllIndustryTagsByIndustryIdAsync(industry.Id);
            var industryTagsToRemove = new List<IndustryTag>();
            foreach (var existingIndustryTag in existingIndustryTags)
            {
                var found = false;
                foreach (var newIndustryTag in industryTags)
                {
                    if (!existingIndustryTag.Name.Equals(newIndustryTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                    industryTagsToRemove.Add(existingIndustryTag);
            }

            foreach (var industryTag in industryTagsToRemove)
                await DeleteIndustryIndustryTagMappingAsync(industry.Id, industryTag.Id);

            foreach (var industryTagName in industryTags)
            {
                IndustryTag industryTag;
                var industryTag2 = await GetIndustryTagByNameAsync(industryTagName);
                if (industryTag2 == null)
                {
                    //add new industry tag
                    industryTag = new IndustryTag
                    {
                        Name = industryTagName
                    };
                    await InsertIndustryTagAsync(industryTag);
                }
                else
                    industryTag = industryTag2;

                if (!await IndustryTagExistsAsync(industry, industryTag.Id))
                    await InsertIndustryIndustryTagMappingAsync(new IndustryIndustryTagMapping { IndustryTagId = industryTag.Id, IndustryId = industry.Id });

                var seName = await _urlRecordService.ValidateSeNameAsync(industryTag, string.Empty, industryTag.Name, true);
                await _urlRecordService.SaveSlugAsync(industryTag, seName, 0);
            }

            //cache
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeEntityCacheDefaults<IndustryTag>.Prefix);
        }

        #endregion
    }
}
