using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zaraye.Core;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Topics;
using Zaraye.Data;
using Zaraye.Services.Catalog;
using Zaraye.Services.Customers;
using Zaraye.Services.Security;
using Zaraye.Services.Stores;

namespace Zaraye.Services.Topics
{
    /// <summary>
    /// Topic service
    /// </summary>
    public partial class TopicService : ITopicService
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<TopicTopicTagMapping> _topicTagMappingRepository;
        private readonly IRepository<TopicTag> _topicTagRepository;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public TopicService(
            IAclService aclService,
            ICustomerService customerService,
            IRepository<Topic> topicRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            IRepository<TopicTopicTagMapping> topicTagMappingRepository,
            IRepository<TopicTag> topicTagRepository,
            IStoreContext storeContext
            )
        {
            _aclService = aclService;
            _customerService = customerService;
            _topicRepository = topicRepository;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
            _topicTagMappingRepository = topicTagMappingRepository;
            _topicTagRepository = topicTagRepository;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTopicAsync(Topic topic)
        {
            await _topicRepository.DeleteAsync(topic);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic
        /// </returns>
        public virtual async Task<Topic> GetTopicByIdAsync(int topicId)
        {
            return await _topicRepository.GetByIdAsync(topicId, cache => default);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="storeId">Store identifier; pass 0 to ignore filtering by store and load the first one</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic
        /// </returns>
        public virtual async Task<Topic> GetTopicBySystemNameAsync(string systemName, int storeId = 0)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeTopicDefaults.TopicBySystemNameCacheKey, systemName, storeId, customerRoleIds);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = _topicRepository.Table
                    .Where(t => t.Published);

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, customerRoleIds);

                return query.Where(t => t.SystemName == systemName)
                    .OrderBy(t => t.Id)
                    .FirstOrDefault();
            });
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topics
        /// </returns>
        /// 
        public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId,
            bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeTopicDefaults.TopicsAllCacheKey);

            var Topics = await _staticCacheManager
               .GetAsync(key, async () =>
               {
                   return await _topicRepository.GetAllAsync(async query =>
                   {
                       if (!showHidden)
                       {
                           query = query.Where(t => t.Published);

                           //apply store mapping constraints
                           query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                           //apply ACL constraints
                           if (!ignoreAcl)
                               query = await _aclService.ApplyAcl(query, customerRoleIds);
                       }

                       if (onlyIncludedInTopMenu)
                           query = query.Where(t => t.IncludeInTopMenu);

                       return query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
                   }, cache =>
                   {
                       return ignoreAcl
                           ? cache.PrepareKeyForDefaultCache(ZarayeTopicDefaults.TopicsAllCacheKey, storeId, showHidden, onlyIncludedInTopMenu)
                           : cache.PrepareKeyForDefaultCache(ZarayeTopicDefaults.TopicsAllWithACLCacheKey, storeId, showHidden, onlyIncludedInTopMenu, customerRoleIds);
                   });
               });

            return Topics;
        }
        //public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId,
        //    bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        //{
        //    var customer = await _workContext.GetCurrentCustomerAsync();
        //    var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

        //    return await _topicRepository.GetAllAsync(async query =>
        //    {
        //        if (!showHidden)
        //        {
        //            query = query.Where(t => t.Published);

        //            //apply store mapping constraints
        //            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

        //            //apply ACL constraints
        //            if (!ignoreAcl)
        //                query = await _aclService.ApplyAcl(query, customerRoleIds);
        //        }

        //        if (onlyIncludedInTopMenu)
        //            query = query.Where(t => t.IncludeInTopMenu);

        //        return query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);
        //    }, cache =>
        //    {
        //        return ignoreAcl
        //            ? cache.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllCacheKey, storeId, showHidden, onlyIncludedInTopMenu)
        //            : cache.PrepareKeyForDefaultCache(NopTopicDefaults.TopicsAllWithACLCacheKey, storeId, showHidden, onlyIncludedInTopMenu, customerRoleIds);
        //    });
        //}

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="keywords">Keywords to search into body or title</param>
        /// <param name="ignoreAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <param name="onlyIncludedInTopMenu">A value indicating whether to show only topics which include on the top menu</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opics
        /// </returns>
        public virtual async Task<IList<Topic>> GetAllTopicsAsync(int storeId, string keywords,
            bool ignoreAcl = false, bool showHidden = false, bool onlyIncludedInTopMenu = false)
        {
            var topics = await GetAllTopicsAsync(storeId,
                ignoreAcl: ignoreAcl,
                showHidden: showHidden,
                onlyIncludedInTopMenu: onlyIncludedInTopMenu);

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                return topics
                    .Where(topic => (topic.Title?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                        (topic.Body?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false))
                    .ToList();
            }

            return topics;
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTopicAsync(Topic topic)
        {
            await _topicRepository.InsertAsync(topic);
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTopicAsync(Topic topic)
        {
            await _topicRepository.UpdateAsync(topic);
        }

        #endregion

        #region CustomMehtod
        public virtual async Task<IList<Topic>> SearchTopicsAsync(string keywords = null, bool searchTopicTags = false, bool showHidden = false, bool? overridePublished = null)
        {
            var query = _topicRepository.Table;

            if (!showHidden)
            {
                query = query.Where(t => t.Published);
            }

            if (!string.IsNullOrEmpty(keywords))
            {
                IQueryable<int> TopicByKeywords;

                TopicByKeywords =
                        from c in _topicRepository.Table
                        where c.Title.Contains(keywords)
                        select c.Id;


                if (searchTopicTags)
                {
                    TopicByKeywords = TopicByKeywords.Union(
                        from cctm in _topicTagMappingRepository.Table
                        join ct in _topicTagRepository.Table on cctm.TopicTagId equals ct.Id
                        where ct.Name == keywords
                        select cctm.TopicId
                    );

                }

                query =
                    from p in query
                    from pbk in LinqToDB.LinqExtensions.InnerJoin(TopicByKeywords, pbk => pbk == p.Id)
                    select p;
            }

            return query.OrderBy(t => t.DisplayOrder).ToList();
        }
        #endregion
    }
}