using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling.Internal;
using System.ComponentModel.DataAnnotations;
using Zaraye.Core.Domain.Common;
using Zaraye.Models.Api.V4.Common;
using Zaraye.Models.Api.V4.OrderManagement;
using Zaraye.Services.Common;
using Zaraye.Services.Logging;

namespace Zaraye.Controllers.V4.Security
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("4")]
    [Route("v{version:apiVersion}/common")]
    public class CommonController : BaseApiController
    {
        #region Fields
        private readonly ICommonService _commonService;
        private readonly IAppLoggerService _appLoggerService;

        #endregion

        #region Ctor
        public CommonController(ICommonService commonService, IAppLoggerService appLoggerService)
        {
            _commonService = commonService;
            _appLoggerService = appLoggerService;
        }

        #endregion

        #region Country / Cities / Areas

        [HttpGet("all-countries")]
        public async Task<IActionResult> AllCountries()
        {
            try
            {
                var data = await _commonService.AllCountries();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-states/{countryId}")]
        public async Task<IActionResult> AllStates(int countryId)
        {
            try
            {
                var data = await _commonService.AllStates(countryId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-areas/{cityId}")]
        public async Task<IActionResult> AllAreas(int cityId)
        {
            try
            {
                var data = await _commonService.AllAreas(cityId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Brand / Industry / category / product  / attributes

        [HttpGet("get-unit/{productId}")]
        public async Task<IActionResult> GetUnitByProductId(int productId)
        {
            try
            {
                var data = await _commonService.GetUnitByProductId(productId);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-brands/{productId}")]
        public async Task<IActionResult> AllBrands(int productId)
        {
            try
            {
                var data = await _commonService.AllBrands(productId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-product-filters")]
        public async Task<IActionResult> GetAllProductFilters()
        {
            try
            {
                var data = await _commonService.GetAllProductFilters();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("get-all-filtered-product")]
        public async Task<IActionResult> GetAllFilteredProduct(ProductFilterModel model)
        {
            try
            {
                var data = await _commonService.GetAllFilteredProduct(model);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-se-name")]
        public async Task<IActionResult> GetAllSeNames()
        {
            try
            {
                var data = await _commonService.GetAllSeNames();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        //------------get all blogs
        [HttpGet("get-all-blogs")]
        public async Task<IActionResult> GetAllBlogs(int pageIndex = 0, int pagesize = 5)
        {
            try
            {
                var data = await _commonService.GetAllBlogs(pageIndex, pagesize);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-latest-blogs/{numberofblog}")]
        public async Task<IActionResult> GetLatestBlogs(int numberofblog = 4)
        {
            try
            {
                var data = await _commonService.GetLatestBlogs(numberofblog);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-blog-detail/{blogId}")]
        public async Task<IActionResult> GetBlogDetailById(int blogId = 0)
        {
            try
            {
                var data = await _commonService.GetBlogDetailById(blogId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-topic-detail/{topicId}")]
        public async Task<IActionResult> GetTopicDetailById(int topicId = 0)
        {
            try
            {
                var data = await _commonService.GetTopicDetailById(topicId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-brands")]
        public async Task<IActionResult> GetAllBrands()
        {
            try
            {
                var data = await _commonService.GetAllBrands();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-industries")]
        public async Task<IActionResult> AllIndustries(string type = null)
        {
            try
            {
                var data = await _commonService.AllIndustries(type);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-categories/{industryId}")]
        public async Task<IActionResult> AllCategories(int industryId, bool showNumberOfProducts = true)
        {
            try
            {
                var data = await _commonService.AllCategories(industryId, showNumberOfProducts);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-top-categories")]
        public async Task<IActionResult> GetAllTopCategories()
        {
            try
            {
                var data = await _commonService.GetAllTopCategories();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("categories-and-products/{industryId}")]
        public async Task<IActionResult> CategoriesAndProductsByIndustryId(int industryId)
        {
            try
            {
                var data = await _commonService.CategoriesAndProductsByIndustryId(industryId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
        //-----category detail by ID
        [HttpGet("get-category-details/{categoryId}")]
        public async Task<IActionResult> GetCategorydetails(int categoryId)
        {
            try
            {
                var data = await _commonService.GetCategorydetails(categoryId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-industries-and-categories")]
        public async Task<IActionResult> GetIndustriesAndCategories()
        {
            try
            {
                var data = await _commonService.GetIndustriesAndCategories();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-industry-detail/{Id}")]
        public async Task<IActionResult> GetIndustryDetail(int Id)
        {
            try
            {
                var data = await _commonService.GetIndustryDetail(Id);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-products")]
        public async Task<IActionResult> AllProducts(int categoryId = 0, List<int> categoryIds = null)
        {
            try
            {
                var data = await _commonService.AllProducts(categoryId, categoryIds);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-products/{categoryId}")]
        public async Task<IActionResult> AllProducts(int categoryId = 0)
        {
            try
            {
                var data = await _commonService.AllProducts(categoryId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("product-attributes/{productId}")]
        public async Task<IActionResult> AllProductAttributes(int productId, string attributesXml = "")
        {
            try
            {
                var data = await _commonService.AllProductAttributes(productId, attributesXml);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("product-attribute-change/{productId}")]
        public async Task<IActionResult> ProductAttributeChange(int productId, [FromBody] List<CommonApiModel.CombinationAttributeApiModel> attributesData)
        {
            try
            {
                var data = await _commonService.ProductAttributeChange(productId, attributesData);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-app-versions")]
        public async Task<IActionResult> AllAppVersions(string platform = "", string type = "Consumer App")
        {
            try
            {
                var data = await _commonService.AllAppVersionsAsync(platform, type);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-buyer-types")]
        public async Task<IActionResult> AllBuyerTypes()
        {
            try
            {
                var data = await _commonService.AllBuyerTypesAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-supplier-types")]
        public async Task<IActionResult> AllSupplierTypes()
        {
            try
            {
                var data = await _commonService.AllSupplierTypesAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-product-details/{productId}")]
        public async Task<IActionResult> GetProductDetailsById(int productId = 0)
        {
            try
            {
                var data = await _commonService.GetProductDetailsById(productId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-brand-details/{brandId}")]
        public async Task<IActionResult> GetBrandDetailsById(int brandId = 0)
        {
            try
            {
                var data = await _commonService.GetBrandDetailsById(brandId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Language Methods

        [HttpGet("all-localizations/{resourceName}/{languageId}")]
        public async Task<IActionResult> AllLocalizations(string resourceName, int languageId)
        {
            try
            {
                var data = await _commonService.AllLocalizations(resourceName, languageId);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-languages")]
        public async Task<IActionResult> AllLanguages()
        {
            try
            {
                var data = await _commonService.AllLanguages();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("set-language/{languageId}")]
        public async Task<IActionResult> SetLanguage(int languageId)
        {
            try
            {
                var data = await _commonService.SetLanguage(languageId);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Request Reject

        [HttpGet("rejected-reasons")]
        public IActionResult AllRejectedReasons()
        {
            try
            {
                var data = _commonService.AllRejectedReasons();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Business Model

        [HttpGet("business-module/{industryId}")]
        public async Task<IActionResult> AllBusinessModules(int industryId)
        {
            try
            {
                var data = await _commonService.AllBusinessModules(industryId);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-modofpayment")]
        public async Task<IActionResult> AllModeOfPayment()
        {
            try
            {
                var data = await _commonService.AllModeOfPayment();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Price Discovery

        [HttpGet("get-all-rates")]
        public async Task<IActionResult> OrderManagement_GetAllRates()
        {
            try
            {
                var data = await _commonService.OrderManagement_GetAllRates();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-rates-by-industryId/{IndustryId}")]
        public async Task<IActionResult> OrderManagement_GetAllRatesByIndustryId(int IndustryId, int CategoryId = 0)
        {
            try
            {
                var data = await _commonService.OrderManagement_GetAllRatesByIndustryId(IndustryId, CategoryId);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-favourite-rates")]
        public async Task<IActionResult> OrderManagement_GetAllFavouriteRates()
        {
            try
            {
                var data = await _commonService.OrderManagement_GetAllFavouriteRates();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("rate-add")]
        public async Task<IActionResult> OrderManagement_AddRate([FromBody] OrderManagementApiModel.RateModel model)
        {
            try
            {
                var data = await _commonService.OrderManagement_AddRate(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("rate-add-by-groupId/{groupId}")]
        public async Task<IActionResult> OrderManagement_AddRateByGroupId(int groupId, decimal price)
        {
            try
            {
                var data = await _commonService.OrderManagement_AddRateByGroupId(groupId, price);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("favourite-rate-group")]
        public async Task<IActionResult> OrderManagement_FavouriteRateGroup(int productId, int attributeValueId, int brandId, bool isWishlist)
        {
            try
            {
                var data = await _commonService.OrderManagement_FavouriteRateGroup(productId, attributeValueId, brandId, isWishlist);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region WebSlider

        [HttpGet("get-all-web-sliders")]
        public async Task<IActionResult> AllWebSliders()
        {
            try
            {
                var data = await _commonService.AllWebSliders();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        [HttpGet("all-buyers")]
        public async Task<IActionResult> AllBuyers()
        {
            try
            {
                var data = await _commonService.AllBuyers();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-bookers")]
        public async Task<IActionResult> AllBookers()
        {
            try
            {
                var data = await _commonService.AllBookers();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-roles")]
        public IActionResult AllRoles()
        {
            try
            {
                var data = _commonService.AllRoles();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-jaizas")]
        public async Task<IActionResult> AllJaizas()
        {
            try
            {
                var data = await _commonService.AllJaizas();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-faqs")]
        public async Task<IActionResult> AllFaqs()
        {
            try
            {
                var data = await _commonService.AllFaqs();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
        //get faq by type
        [HttpGet("get-faqs-by-type")]
        public async Task<IActionResult> AllFaqsbyType(bool isApp = true)
        {
            try
            {
                var faqs = await _commonService.AllFaqsbyType(isApp);
                return Ok(new { success = true, data = faqs });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add-feedBack")]
        public async Task<IActionResult> AddAppFeedBack([FromBody] AppFeedBackApiModel model)
        {
            try
            {
                var data = await _commonService.AddAppFeedBack(model);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-current-store")]
        public async Task<IActionResult> GetCurrentStore()
        {
            try
            {
                var data = await _commonService.GetCurrentStore();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-warehouse")]
        public async Task<IActionResult> AllWarehouse()
        {
            try
            {
                var data = await _commonService.AllWarehouse();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-delayed-reason")]
        public async Task<IActionResult> AllDelayedReasons()
        {
            try
            {
                var data = await _commonService.AllDelayedReasons();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-delivery-cost-reasons")]
        public async Task<IActionResult> AllDeliveryCostReasons()
        {
            try
            {
                var data = await _commonService.AllDeliveryCostReasons();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("clear-cache")]
        public async Task<IActionResult> ClearCache()
        {
            var data = await _commonService.ClearCache();
            return Ok(new { success = true, message = data });
        }

        [HttpGet("best-selling-product")]
        public async Task<IActionResult> GetAllBestSellingProductsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var data = await _commonService.GetAllBestSellingProductsAsync(pageIndex, pageSize);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-commodity-data")]
        public async Task<IActionResult> GetAllCommodityData()
        {
            try
            {
                var data = await _commonService.GetAllCommodityData();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-recently-viewed-products")]
        public async Task<IActionResult> GetViewedProducts()
        {
            try
            {
                var data = await _commonService.GetViewedProducts();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = true, message = ex.Message });
            }
        }
        [HttpGet("get-related-products")]
        public async Task<IActionResult> GetRelatedProductsByProductId([Required] int productId)
        {
            try
            {
                var data = await _commonService.GetRelatedProductsByProductId(productId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = true, message = ex.Message });
            }
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile([FromBody] UploadeFileModel model)
        {
            try
            {
                var saveAwsS3File = await _commonService.UploadFile(model);
                return Ok(new { success = true, data = saveAwsS3File });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("explore-all-category")]

        public async Task<IActionResult> ExploreAllCategories(CategoryFilterModel model)
        {
            try
            {
                var data = await _commonService.exploreAllCategories(model);
                if (data == null)
                    return Ok(new { success = true, message = "no Record found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #region Contact us
        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs([FromBody] ContactUsModel model)
        {
            try
            {
                var contactus = await _commonService.ContactUs(model);
                return Ok(new { success = true, data = contactus });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


        #endregion
    }
}