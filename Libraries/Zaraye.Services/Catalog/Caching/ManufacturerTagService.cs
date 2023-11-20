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
    public partial class ManufacturerTagService : IManufacturerTagService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ManufacturerManufacturerTagMapping> _manufacturerManufacturerTagMappingRepository;
        private readonly IRepository<ManufacturerTag> _manufacturerTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ManufacturerTagService(
            IAclService aclService,
            ICustomerService customerService,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<ManufacturerManufacturerTagMapping> manufacturerManufacturerTagMappingRepository,
            IRepository<ManufacturerTag> manufacturerTagRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _customerService = customerService;
            _manufacturerRepository = manufacturerRepository;
            _manufacturerManufacturerTagMappingRepository = manufacturerManufacturerTagMappingRepository;
            _manufacturerTagRepository = manufacturerTagRepository;
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
        protected virtual async Task DeleteManufacturerManufacturerTagMappingAsync(int manufacturerId, int manufacturerTagId)
        {
            var mappingRecord = await _manufacturerManufacturerTagMappingRepository.Table
                .FirstOrDefaultAsync(pptm => pptm.ManufacturerId == manufacturerId && pptm.ManufacturerTagId == manufacturerTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await _manufacturerManufacturerTagMappingRepository.DeleteAsync(mappingRecord);
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
        protected virtual async Task<bool> ManufacturerTagExistsAsync(Manufacturer manufacturer, int manufacturerTagId)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            return await _manufacturerManufacturerTagMappingRepository.Table
                .AnyAsync(pptm => pptm.ManufacturerId == manufacturer.Id && pptm.ManufacturerTagId == manufacturerTagId);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag
        /// </returns>
        protected virtual async Task<ManufacturerTag> GetManufacturerTagByNameAsync(string name)
        {
            var query = from pt in _manufacturerTagRepository.Table
                        where pt.Name == name
                        select pt;

            var manufacturerTag = await query.FirstOrDefaultAsync();
            return manufacturerTag;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertManufacturerTagAsync(ManufacturerTag manufacturerTag)
        {
            await _manufacturerTagRepository.InsertAsync(manufacturerTag);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteManufacturerTagAsync(ManufacturerTag manufacturerTag)
        {
            await _manufacturerTagRepository.DeleteAsync(manufacturerTag);
        }

        /// <summary>
        /// Delete product tags
        /// </summary>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteManufacturerTagsAsync(IList<ManufacturerTag> manufacturerTags)
        {
            if (manufacturerTags == null)
                throw new ArgumentNullException(nameof(manufacturerTags));

            foreach (var manufacturerTag in manufacturerTags)
                await DeleteManufacturerTagAsync(manufacturerTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<ManufacturerTag>> GetAllManufacturerTagsAsync(string tagName = null)
        {
            var allManufacturerTags = await _manufacturerTagRepository.GetAllAsync(query => query, getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName))
                allManufacturerTags = allManufacturerTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allManufacturerTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<ManufacturerTag>> GetAllManufacturerTagsByManufacturerIdAsync(int manufacturerId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.ManufacturerTagsByManufacturerCacheKey, manufacturerId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var tagMapping = from ptm in _manufacturerManufacturerTagMappingRepository.Table
                                 join pt in _manufacturerTagRepository.Table on ptm.ManufacturerTagId equals pt.Id
                                 where ptm.ManufacturerId == manufacturerId
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
        public virtual async Task<ManufacturerTag> GetManufacturerTagByIdAsync(int manufacturerTagId)
        {
            return await _manufacturerTagRepository.GetByIdAsync(manufacturerTagId, cache => default);
        }

        /// <summary>
        /// Gets product tags
        /// </summary>
        /// <param name="productTagIds">Product tags identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<ManufacturerTag>> GetManufacturerTagsByIdsAsync(int[] manufacturerTagIds)
        {
            return await _manufacturerTagRepository.GetByIdsAsync(manufacturerTagIds);
        }

        /// <summary>
        /// Inserts a product-product tag mapping
        /// </summary>
        /// <param name="tagMapping">Product-product tag mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertManufacturerManufacturerTagMappingAsync(ManufacturerManufacturerTagMapping tagMapping)
        {
            await _manufacturerManufacturerTagMappingRepository.InsertAsync(tagMapping);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateManufacturerTagAsync(ManufacturerTag manufacturerTag)
        {
            if (manufacturerTag == null)
                throw new ArgumentNullException(nameof(manufacturerTag));

            await _manufacturerTagRepository.UpdateAsync(manufacturerTag);

            var seName = await _urlRecordService.ValidateSeNameAsync(manufacturerTag, string.Empty, manufacturerTag.Name, true);
            await _urlRecordService.SaveSlugAsync(manufacturerTag, seName, 0);
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
        public virtual async Task<int> GetManufacturerCountByManufacturerTagIdAsync(int manufacturerTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetManufacturerCountAsync(storeId, showHidden);
            if (dictionary.ContainsKey(manufacturerTagId))
                return dictionary[manufacturerTagId];

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
        public virtual async Task<Dictionary<int, int>> GetManufacturerCountAsync(int storeId, bool showHidden = false)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.ManufacturerTagCountCacheKey, storeId, customerRoleIds, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = _manufacturerManufacturerTagMappingRepository.Table;

                if (!showHidden)
                {
                    var manufacturersQuery = _manufacturerRepository.Table.Where(p => p.Published);


                    query = query.Where(pc => manufacturersQuery.Any(p => !p.Deleted && pc.ManufacturerId == p.Id));
                }

                var pTagCount = from pt in _manufacturerTagRepository.Table
                                join ptm in query on pt.Id equals ptm.ManufacturerTagId
                                group ptm by ptm.ManufacturerTagId into ptmGrouped
                                select new
                                {
                                    ManufacturerTagId = ptmGrouped.Key,
                                    ManufacturerCount = ptmGrouped.Count()
                                };

                return pTagCount.ToDictionary(item => item.ManufacturerTagId, item => item.ManufacturerCount);
            });
        }

        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateManufacturerTagsAsync(Manufacturer manufacturer, string[] manufacturerTags)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            //manufacturer tags
            var existingManufacturerTags = await GetAllManufacturerTagsByManufacturerIdAsync(manufacturer.Id);
            var manufacturerTagsToRemove = new List<ManufacturerTag>();
            foreach (var existingManufacturerTag in existingManufacturerTags)
            {
                var found = false;
                foreach (var newManufacturerTag in manufacturerTags)
                {
                    if (!existingManufacturerTag.Name.Equals(newManufacturerTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                    manufacturerTagsToRemove.Add(existingManufacturerTag);
            }

            foreach (var manufacturerTag in manufacturerTagsToRemove)
                await DeleteManufacturerManufacturerTagMappingAsync(manufacturer.Id, manufacturerTag.Id);

            foreach (var manufacturerTagName in manufacturerTags)
            {
                ManufacturerTag manufacturerTag;
                var manufacturerTag2 = await GetManufacturerTagByNameAsync(manufacturerTagName);
                if (manufacturerTag2 == null)
                {
                    //add new manufacturer tag
                    manufacturerTag = new ManufacturerTag
                    {
                        Name = manufacturerTagName
                    };
                    await InsertManufacturerTagAsync(manufacturerTag);
                }
                else
                    manufacturerTag = manufacturerTag2;

                if (!await ManufacturerTagExistsAsync(manufacturer, manufacturerTag.Id))
                    await InsertManufacturerManufacturerTagMappingAsync(new ManufacturerManufacturerTagMapping { ManufacturerTagId = manufacturerTag.Id, ManufacturerId = manufacturer.Id });

                var seName = await _urlRecordService.ValidateSeNameAsync(manufacturerTag, string.Empty, manufacturerTag.Name, true);
                await _urlRecordService.SaveSlugAsync(manufacturerTag, seName, 0);
            }

            //cache
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeEntityCacheDefaults<ManufacturerTag>.Prefix);
        }

        #endregion
    }
}
