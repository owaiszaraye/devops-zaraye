using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.Catalog
{
    /// <summary>
    /// Industry service interface
    /// </summary>
    public partial interface IIndustryService
    {
        /// <summary>
        /// Delete industry
        /// </summary>
        /// <param name="industry">Industry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteIndustryAsync(Industry industry);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IList<Industry>> GetAllIndustriesAsync(int storeId = 0, bool showHidden = false, bool? isAppPublished = null);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="industryName">Industry name</param>
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
        Task<IPagedList<Industry>> GetAllIndustriesAsync(string industryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null, bool? isAppPublished = null);

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IList<Industry>> GetAllIndustriesDisplayedOnHomepageAsync(bool showHidden = false);

        /// <summary>
        /// Gets a industry
        /// </summary>
        /// <param name="industryId">Industry identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the industry
        /// </returns>
        Task<Industry> GetIndustryByIdAsync(int industryId);

        /// <summary>
        /// Inserts industry
        /// </summary>
        /// <param name="industry">Industry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertIndustryAsync(Industry industry);

        /// <summary>
        /// Updates the industry
        /// </summary>
        /// <param name="industry">Industry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateIndustryAsync(Industry industry);

        /// <summary>
        /// Delete a list of categories
        /// </summary>
        /// <param name="categories">Industries</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteIndustriesAsync(IList<Industry> categories);

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="industryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing categories
        /// </returns>
        Task<string[]> GetNotExistingIndustriesAsync(string[] industryIdsNames);

        /// <summary>
        /// Gets categories by identifier
        /// </summary>
        /// <param name="industryIds">Industry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IList<Industry>> GetIndustriesByIdsAsync(int[] industryIds);

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
        Task<string> GetFormattedBreadCrumbAsync(Industry industry, IList<Industry> allIndustries = null,
            string separator = ">>", int languageId = 0);

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
        Task<IList<Industry>> GetIndustryBreadCrumbAsync(Industry industry, IList<Industry> allIndustries = null, bool showHidden = false);

        Task<IList<Industry>> SearchIndustriesAsync(string keywords = null, bool searchIndustryTags = false, bool showHidden = false, bool? overridePublished = null, bool? isAppPublished = null);
    }
}