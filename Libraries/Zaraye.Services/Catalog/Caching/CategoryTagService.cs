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
    public partial class CategoryTagService : ICategoryTagService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategoryCategoryTagMapping> _categoryCategoryTagMappingRepository;
        private readonly IRepository<CategoryTag> _categoryTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CategoryTagService(
            IAclService aclService,
            ICustomerService customerService,
            IRepository<Category> categoryRepository,
            IRepository<CategoryCategoryTagMapping> categoryCategoryTagMappingRepository,
            IRepository<CategoryTag> categoryTagRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _customerService = customerService;
            _categoryRepository = categoryRepository;
            _categoryCategoryTagMappingRepository = categoryCategoryTagMappingRepository;
            _categoryTagRepository = categoryTagRepository;
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
        protected virtual async Task DeleteCategoryCategoryTagMappingAsync(int categoryId, int categoryTagId)
        {
            var mappingRecord = await _categoryCategoryTagMappingRepository.Table
                .FirstOrDefaultAsync(pptm => pptm.CategoryId == categoryId && pptm.CategoryTagId == categoryTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await _categoryCategoryTagMappingRepository.DeleteAsync(mappingRecord);
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
        protected virtual async Task<bool> CategoryTagExistsAsync(Category category, int categoryTagId)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            return await _categoryCategoryTagMappingRepository.Table
                .AnyAsync(pptm => pptm.CategoryId == category.Id && pptm.CategoryTagId == categoryTagId);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag
        /// </returns>
        protected virtual async Task<CategoryTag> GetCategoryTagByNameAsync(string name)
        {
            var query = from pt in _categoryTagRepository.Table
                        where pt.Name == name
                        select pt;

            var categoryTag = await query.FirstOrDefaultAsync();
            return categoryTag;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertCategoryTagAsync(CategoryTag categoryTag)
        {
            await _categoryTagRepository.InsertAsync(categoryTag);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCategoryTagAsync(CategoryTag categoryTag)
        {
            await _categoryTagRepository.DeleteAsync(categoryTag);
        }

        /// <summary>
        /// Delete product tags
        /// </summary>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCategoryTagsAsync(IList<CategoryTag> categoryTags)
        {
            if (categoryTags == null)
                throw new ArgumentNullException(nameof(categoryTags));

            foreach (var categoryTag in categoryTags)
                await DeleteCategoryTagAsync(categoryTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<CategoryTag>> GetAllCategoryTagsAsync(string tagName = null)
        {
            var allCategoryTags = await _categoryTagRepository.GetAllAsync(query => query, getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName))
                allCategoryTags = allCategoryTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allCategoryTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<CategoryTag>> GetAllCategoryTagsByCategoryIdAsync(int categoryId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.CategoryTagsByCategoryCacheKey, categoryId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var tagMapping = from ptm in _categoryCategoryTagMappingRepository.Table
                                 join pt in _categoryTagRepository.Table on ptm.CategoryTagId equals pt.Id
                                 where ptm.CategoryId == categoryId
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
        public virtual async Task<CategoryTag> GetCategoryTagByIdAsync(int categoryTagId)
        {
            return await _categoryTagRepository.GetByIdAsync(categoryTagId, cache => default);
        }

        /// <summary>
        /// Gets product tags
        /// </summary>
        /// <param name="productTagIds">Product tags identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<CategoryTag>> GetCategoryTagsByIdsAsync(int[] categoryTagIds)
        {
            return await _categoryTagRepository.GetByIdsAsync(categoryTagIds);
        }

        /// <summary>
        /// Inserts a product-product tag mapping
        /// </summary>
        /// <param name="tagMapping">Product-product tag mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCategoryCategoryTagMappingAsync(CategoryCategoryTagMapping tagMapping)
        {
            await _categoryCategoryTagMappingRepository.InsertAsync(tagMapping);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCategoryTagAsync(CategoryTag categoryTag)
        {
            if (categoryTag == null)
                throw new ArgumentNullException(nameof(categoryTag));

            await _categoryTagRepository.UpdateAsync(categoryTag);

            var seName = await _urlRecordService.ValidateSeNameAsync(categoryTag, string.Empty, categoryTag.Name, true);
            await _urlRecordService.SaveSlugAsync(categoryTag, seName, 0);
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
        public virtual async Task<int> GetCategoryCountByCategoryTagIdAsync(int categoryTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetCategoryCountAsync(storeId, showHidden);
            if (dictionary.ContainsKey(categoryTagId))
                return dictionary[categoryTagId];

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
        public virtual async Task<Dictionary<int, int>> GetCategoryCountAsync(int storeId, bool showHidden = false)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.CategoryTagCountCacheKey, storeId, customerRoleIds, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = _categoryCategoryTagMappingRepository.Table;

                if (!showHidden)
                {
                    var categorysQuery = _categoryRepository.Table.Where(p => p.Published);

                    //apply store mapping constraints
                    categorysQuery = await _storeMappingService.ApplyStoreMapping(categorysQuery, storeId);

                    //apply ACL constraints
                    categorysQuery = await _aclService.ApplyAcl(categorysQuery, customerRoleIds);

                    query = query.Where(pc => categorysQuery.Any(p => !p.Deleted && pc.CategoryId == p.Id));
                }

                var pTagCount = from pt in _categoryTagRepository.Table
                                join ptm in query on pt.Id equals ptm.CategoryTagId
                                group ptm by ptm.CategoryTagId into ptmGrouped
                                select new
                                {
                                    CategoryTagId = ptmGrouped.Key,
                                    CategoryCount = ptmGrouped.Count()
                                };

                return pTagCount.ToDictionary(item => item.CategoryTagId, item => item.CategoryCount);
            });
        }

        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCategoryTagsAsync(Category category, string[] categoryTags)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            //category tags
            var existingCategoryTags = await GetAllCategoryTagsByCategoryIdAsync(category.Id);
            var categoryTagsToRemove = new List<CategoryTag>();
            foreach (var existingCategoryTag in existingCategoryTags)
            {
                var found = false;
                foreach (var newCategoryTag in categoryTags)
                {
                    if (!existingCategoryTag.Name.Equals(newCategoryTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                    categoryTagsToRemove.Add(existingCategoryTag);
            }

            foreach (var categoryTag in categoryTagsToRemove)
                await DeleteCategoryCategoryTagMappingAsync(category.Id, categoryTag.Id);

            foreach (var categoryTagName in categoryTags)
            {
                CategoryTag categoryTag;
                var categoryTag2 = await GetCategoryTagByNameAsync(categoryTagName);
                if (categoryTag2 == null)
                {
                    //add new category tag
                    categoryTag = new CategoryTag
                    {
                        Name = categoryTagName
                    };
                    await InsertCategoryTagAsync(categoryTag);
                }
                else
                    categoryTag = categoryTag2;

                if (!await CategoryTagExistsAsync(category, categoryTag.Id))
                    await InsertCategoryCategoryTagMappingAsync(new CategoryCategoryTagMapping { CategoryTagId = categoryTag.Id, CategoryId = category.Id });

                var seName = await _urlRecordService.ValidateSeNameAsync(categoryTag, string.Empty, categoryTag.Name, true);
                await _urlRecordService.SaveSlugAsync(categoryTag, seName, 0);
            }

            //cache
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeEntityCacheDefaults<CategoryTag>.Prefix);
        }

        #endregion
    }
}
