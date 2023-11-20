using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zaraye.Models.Api.V4.Common;
using Zaraye.Models.Api.V4.OrderManagement;

namespace Zaraye.Services.Common
{
    public interface ICommonService
    {
        Task<object> AllCountries();
        Task<object> AllStates(int countryId);
        Task<object> AllAreas(int cityId);
        Task<object> GetUnitByProductId(int productId);
        Task<object> AllBrands(int productId);
        Task<object> GetAllProductFilters();
        Task<object> GetAllFilteredProduct(ProductFilterModel model);
        Task<object> GetAllSeNames();
        Task<object> GetAllBlogs(int pageIndex = 0, int pagesize = 5);
        Task<object> GetLatestBlogs(int numberofblog = 4);
        Task<object> GetBlogDetailById(int blogId = 0);
        Task<object> GetTopicDetailById(int topicId = 0);
        Task<object> GetAllBrands();
        Task<object> AllIndustries(string type);
        Task<object> AllCategories(int industryId, bool showNumberOfProducts = true);
        Task<object> GetAllTopCategories();
        Task<object> CategoriesAndProductsByIndustryId(int industryId);
        Task<object> GetCategorydetails(int categoryId);
        Task<object> GetIndustriesAndCategories();
        Task<object> GetIndustryDetail(int Id);
        Task<object> AllProducts(int categoryId = 0, List<int> categoryIds = null);
        Task<object> AllProducts(int categoryId = 0);
        Task<List<CommonApiModel.ProductAttributesApiModel>> AllProductAttributes(int productId, string attributesXml = "");
        Task<object> ProductAttributeChange(int productId, List<CommonApiModel.CombinationAttributeApiModel> attributesData);
        Task<IEnumerable<object>> AllAppVersionsAsync(string platform = "", string type = "Consumer App");
        Task<IEnumerable<object>> AllBuyerTypesAsync();
        Task<IEnumerable<object>> AllSupplierTypesAsync();
        Task<object> GetProductDetailsById(int productId = 0, bool recentlyViewflag = false);
        Task<object> GetBrandDetailsById(int brandId = 0);
        Task<Dictionary<string, string>> AllLocalizations(string resourceName, int languageId);
        Task<object> AllLanguages();
        Task<string> SetLanguage(int languageId);
        List<SelectListItem> AllRejectedReasons();
        Task<object> AllBusinessModules(int industryId);
        Task<object> AllModeOfPayment();
        Task<object> OrderManagement_GetAllRates();
        Task<object> OrderManagement_GetAllRatesByIndustryId(int IndustryId, int CategoryId = 0);
        Task<object> OrderManagement_GetAllFavouriteRates();
        Task<string> OrderManagement_AddRate(OrderManagementApiModel.RateModel model);
        Task<string> OrderManagement_AddRateByGroupId(int groupId, decimal price);
        Task<string> OrderManagement_FavouriteRateGroup(int productId, int attributeValueId, int brandId, bool isWishlist);
        Task<object> AllWebSliders();
        Task<object> AllBuyers();
        Task<object> AllBookers();
        List<SelectListItem> AllRoles();
        Task<object> AllJaizas();
        Task<object> AllFaqs();
        Task<object> AllFaqsbyType(bool isApp = true);
        Task<string> ClearCache();
        Task<object> GetAllCommodityData();
        Task<object> GetAllBestSellingProductsAsync(int pageIndex = 0, int pageSize = int.MaxValue);
        Task<object> AllDeliveryCostReasons();
        Task<object> AllDelayedReasons();
        Task<object> AllWarehouse();
        Task<object> GetCurrentStore();
        Task<string> AddAppFeedBack(AppFeedBackApiModel model);
        Task<object> GetViewedProducts();
        Task<object> GetRelatedProductsByProductId(int productId);
        Task<object> UploadFile(UploadeFileModel model);
        Task<object> ContactUs(ContactUsModel model);
        Task<object> exploreAllCategories(CategoryFilterModel model);
    }
}
