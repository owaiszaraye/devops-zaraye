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
using Zaraye.Core.Domain.Topics;

namespace Zaraye.Services.Catalog.Caching
{
    public partial class TopicTagService : ITopicTagService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<TopicTopicTagMapping> _topicTopicTagMappingRepository;
        private readonly IRepository<TopicTag> _topicTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicTagService(
            IAclService aclService,
            ICustomerService customerService,
            IRepository<Topic> topicRepository,
            IRepository<TopicTopicTagMapping> topicTopicTagMappingRepository,
            IRepository<TopicTag> topicTagRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _customerService = customerService;
            _topicRepository = topicRepository;
            _topicTopicTagMappingRepository = topicTopicTagMappingRepository;
            _topicTagRepository = topicTagRepository;
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
        protected virtual async Task DeleteTopicTopicTagMappingAsync(int topicId, int topicTagId)
        {
            var mappingRecord = await _topicTopicTagMappingRepository.Table
                .FirstOrDefaultAsync(pptm => pptm.TopicId == topicId && pptm.TopicTagId == topicTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await _topicTopicTagMappingRepository.DeleteAsync(mappingRecord);
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
        protected virtual async Task<bool> TopicTagExistsAsync(Topic topic, int topicTagId)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            return await _topicTopicTagMappingRepository.Table
                .AnyAsync(pptm => pptm.TopicId == topic.Id && pptm.TopicTagId == topicTagId);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag
        /// </returns>
        protected virtual async Task<TopicTag> GetTopicTagByNameAsync(string name)
        {
            var query = from pt in _topicTagRepository.Table
                        where pt.Name == name
                        select pt;

            var topicTag = await query.FirstOrDefaultAsync();
            return topicTag;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertTopicTagAsync(TopicTag topicTag)
        {
            await _topicTagRepository.InsertAsync(topicTag);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTopicTagAsync(TopicTag topicTag)
        {
            await _topicTagRepository.DeleteAsync(topicTag);
        }

        /// <summary>
        /// Delete product tags
        /// </summary>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTopicTagsAsync(IList<TopicTag> topicTags)
        {
            if (topicTags == null)
                throw new ArgumentNullException(nameof(topicTags));

            foreach (var topicTag in topicTags)
                await DeleteTopicTagAsync(topicTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<TopicTag>> GetAllTopicTagsAsync(string tagName = null)
        {
            var allTopicTags = await _topicTagRepository.GetAllAsync(query => query, getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName))
                allTopicTags = allTopicTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allTopicTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<TopicTag>> GetAllTopicTagsByTopicIdAsync(int topicId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.TopicTagsByTopicCacheKey, topicId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var tagMapping = from ptm in _topicTopicTagMappingRepository.Table
                                 join pt in _topicTagRepository.Table on ptm.TopicTagId equals pt.Id
                                 where ptm.TopicId == topicId
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
        public virtual async Task<TopicTag> GetTopicTagByIdAsync(int topicTagId)
        {
            return await _topicTagRepository.GetByIdAsync(topicTagId, cache => default);
        }

        /// <summary>
        /// Gets product tags
        /// </summary>
        /// <param name="productTagIds">Product tags identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<TopicTag>> GetTopicTagsByIdsAsync(int[] topicTagIds)
        {
            return await _topicTagRepository.GetByIdsAsync(topicTagIds);
        }

        /// <summary>
        /// Inserts a product-product tag mapping
        /// </summary>
        /// <param name="tagMapping">Product-product tag mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTopicTopicTagMappingAsync(TopicTopicTagMapping tagMapping)
        {
            await _topicTopicTagMappingRepository.InsertAsync(tagMapping);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTopicTagAsync(TopicTag topicTag)
        {
            if (topicTag == null)
                throw new ArgumentNullException(nameof(topicTag));

            await _topicTagRepository.UpdateAsync(topicTag);

            var seName = await _urlRecordService.ValidateSeNameAsync(topicTag, string.Empty, topicTag.Name, true);
            await _urlRecordService.SaveSlugAsync(topicTag, seName, 0);
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
        public virtual async Task<int> GetTopicCountByTopicTagIdAsync(int topicTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetTopicCountAsync(storeId, showHidden);
            if (dictionary.ContainsKey(topicTagId))
                return dictionary[topicTagId];

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
        public virtual async Task<Dictionary<int, int>> GetTopicCountAsync(int storeId, bool showHidden = false)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.TopicTagCountCacheKey, storeId, customerRoleIds, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = _topicTopicTagMappingRepository.Table;

                if (!showHidden)
                {
                    var topicsQuery = _topicRepository.Table.Where(p => p.Published);


                    query = query.Where(pc => topicsQuery.Any(p => !p.Deleted && pc.TopicId == p.Id));
                }

                var pTagCount = from pt in _topicTagRepository.Table
                                join ptm in query on pt.Id equals ptm.TopicTagId
                                group ptm by ptm.TopicTagId into ptmGrouped
                                select new
                                {
                                    TopicTagId = ptmGrouped.Key,
                                    TopicCount = ptmGrouped.Count()
                                };

                return pTagCount.ToDictionary(item => item.TopicTagId, item => item.TopicCount);
            });
        }

        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTopicTagsAsync(Topic topic, string[] topicTags)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            //topic tags
            var existingTopicTags = await GetAllTopicTagsByTopicIdAsync(topic.Id);
            var topicTagsToRemove = new List<TopicTag>();
            foreach (var existingTopicTag in existingTopicTags)
            {
                var found = false;
                foreach (var newTopicTag in topicTags)
                {
                    if (!existingTopicTag.Name.Equals(newTopicTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                    topicTagsToRemove.Add(existingTopicTag);
            }

            foreach (var topicTag in topicTagsToRemove)
                await DeleteTopicTopicTagMappingAsync(topic.Id, topicTag.Id);

            foreach (var topicTagName in topicTags)
            {
                TopicTag topicTag;
                var topicTag2 = await GetTopicTagByNameAsync(topicTagName);
                if (topicTag2 == null)
                {
                    //add new topic tag
                    topicTag = new TopicTag
                    {
                        Name = topicTagName
                    };
                    await InsertTopicTagAsync(topicTag);
                }
                else
                    topicTag = topicTag2;

                if (!await TopicTagExistsAsync(topic, topicTag.Id))
                    await InsertTopicTopicTagMappingAsync(new TopicTopicTagMapping { TopicTagId = topicTag.Id, TopicId = topic.Id });

                var seName = await _urlRecordService.ValidateSeNameAsync(topicTag, string.Empty, topicTag.Name, true);
                await _urlRecordService.SaveSlugAsync(topicTag, seName, 0);
            }

            //cache
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeEntityCacheDefaults<TopicTag>.Prefix);
        }

        #endregion
    }
}
