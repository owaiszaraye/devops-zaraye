using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Topics;

namespace Zaraye.Services.Catalog.Caching
{
    public partial interface ITopicTagService
    {
        /// <summary>
        /// Delete a category tag
        /// </summary>
        /// <param name="categoryTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTopicTagAsync(TopicTag topicTag);

        /// <summary>
        /// Delete category tags
        /// </summary>
        /// <param name="categoryTags">category tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTopicTagsAsync(IList<TopicTag> topicTags);

        /// <summary>
        /// Gets category tags
        /// </summary>
        /// <param name="categoryTagIds">Category tags identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category tags
        /// </returns>
        Task<IList<TopicTag>> GetTopicTagsByIdsAsync(int[] topicTagIds);

        /// <summary>
        /// Gets all category tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category tags
        /// </returns>
        Task<IList<TopicTag>> GetAllTopicTagsAsync(string tagName = null);

        /// <summary>
        /// Gets all category tags by Category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category tags
        /// </returns>
        Task<IList<TopicTag>> GetAllTopicTagsByTopicIdAsync(int topicId);

        /// <summary>
        /// Gets category tag
        /// </summary>
        /// <param name="categoryTagId">Category tag identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category tag
        /// </returns>
        Task<TopicTag> GetTopicTagByIdAsync(int topicTagId);

        /// <summary>
        /// Inserts a category-category tag mapping
        /// </summary>
        /// <param name="tagMapping">Product-product tag mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTopicTopicTagMappingAsync(TopicTopicTagMapping tagMapping);

        /// <summary>
        /// Updates the category tag
        /// </summary>
        /// <param name="categoryTag">Category tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTopicTagAsync(TopicTag topicTag);

        /// <summary>
        /// Get number of categorys
        /// </summary>
        /// <param name="categoryTagId">Category tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of categorys
        /// </returns>
        Task<int> GetTopicCountByTopicTagIdAsync(int topicTagId, int storeId, bool showHidden = false);

        /// <summary>
        /// Get category count for every linked tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dictionary of "category tag ID : category count"
        /// </returns>
        Task<Dictionary<int, int>> GetTopicCountAsync(int storeId, bool showHidden = false);

        /// <summary>
        /// Update category tags
        /// </summary>
        /// <param name="category">Category for update</param>
        /// <param name="categoryTags">Category tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTopicTagsAsync(Topic topic, string[] topicTags);
    }
}
