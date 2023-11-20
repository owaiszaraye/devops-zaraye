using Zaraye.Core.Domain.Buyer;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Media;
using Zaraye.Core;
using Zaraye.Services.Blogs;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.CustomerTestimonial;
using Zaraye.Services.MarketPlaceExchangerate;
using Zaraye.Services.Media;
using Zaraye.Services.Seo;
using Zaraye.Services.Topics;
using Zaraye.Models.Api.V4.MarketPlace;
using Zaraye.Core.Caching;

namespace Zaraye.Services.MarketPlace
{
    public class MarketPlaceService : IMarketPlaceService
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly IBlogService _blogService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;
        private readonly IIndustryService _industryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITopicService _topicService;
        private readonly IStoreContext _storeContext;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkContext _workContext;
        private readonly IOnlineLeadService _onlineLeadService;
        private readonly ICustomerTestimonialService _customerTestimonialService;
        private readonly IEmployeeInsightsService _employeeInsightsService;
        private readonly IMarketPlaceExchangerateService _marketPlaceExchangerateService;
        private readonly IAwsS3FilesService _awsS3FilesService;
        protected readonly IStaticCacheManager _staticCacheManager;


        #endregion

        #region Ctor
        public MarketPlaceService(
           IProductService productService,
            IBlogService blogService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            ICategoryService categoryService,
            IIndustryService industryService,
            IManufacturerService manufacturerService,
            ITopicService topicService,
            IStoreContext storeContext,
            MediaSettings mediaSettings, IWorkContext workContext,
            IOnlineLeadService onlineLeadService, ICustomerTestimonialService customerTestimonialService,
            IEmployeeInsightsService employeeInsightsService,
            IMarketPlaceExchangerateService marketPlaceExchangerateService,
            IAwsS3FilesService awsS3FilesService, IStaticCacheManager staticCacheManager)
        {
            _productService = productService;
            _blogService = blogService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _categoryService = categoryService;
            _industryService = industryService;
            _manufacturerService = manufacturerService;
            _topicService = topicService;
            _storeContext = storeContext;
            _mediaSettings = mediaSettings;
            _workContext = workContext;
            _onlineLeadService = onlineLeadService;
            _customerTestimonialService = customerTestimonialService;
            _employeeInsightsService = employeeInsightsService;
            _marketPlaceExchangerateService = marketPlaceExchangerateService;
            _awsS3FilesService = awsS3FilesService;
            _staticCacheManager = staticCacheManager;
        }
        #endregion

        #region Methods
        public virtual async Task<IList<object>> SearchMarketPlace(string keyword)
        {
            List<object> data = new List<object>();
            var pictureSize = _mediaSettings.ProductThumbPictureSize;
            if (!string.IsNullOrEmpty(keyword))
            {
                var product = (await _productService.SearchProductsAsync(keywords: keyword, searchProductTags: true))
                    .Select(async item =>
                    {
                        var picture = (await _productService.GetProductPicturesByProductIdAsync(item.Id)).FirstOrDefault();
                        data.Add(new
                        {
                            Id = item.Id,
                            Title = item.Name,
                            SeName = await _urlRecordService.GetSeNameAsync(item, 0, true, false),
                            Picture = await _pictureService.GetPictureUrlAsync(picture == null ? 0 : picture.PictureId, targetSize: pictureSize),
                            description = item.ShortDescription,
                            BodyOverView = item.ShortDescription,
                            Type = "Product"
                        });
                        return data;
                    }).Select(t => t.Result).ToList();

                var category = (await _categoryService.SearchCategoriesAsync(keywords: keyword, searchCategoryTags: true))
                    .Select(async item =>
                    {
                        var picture = (await _productService.GetProductPicturesByProductIdAsync(item.Id)).FirstOrDefault();
                        data.Add(new
                        {
                            Id = item.Id,
                            Title = item.Name,
                            SeName = await _urlRecordService.GetSeNameAsync(item, 0, true, false),
                            Picture = await _pictureService.GetPictureUrlAsync(item.PictureId, targetSize: pictureSize),
                            BodyOverView = item.ShortDescription,
                            Type = "Category"
                        });
                        return data;
                    }).Select(t => t.Result).ToList();

                var industry = (await _industryService.SearchIndustriesAsync(keywords: keyword, searchIndustryTags: true))
                   .Select(async item =>
                   {
                       data.Add(new
                       {
                           Id = item.Id,
                           Title = item.Name,
                           SeName = await _urlRecordService.GetSeNameAsync(item, 0, true, false),
                           Picture = await _pictureService.GetPictureUrlAsync(item.PictureId, targetSize: pictureSize),
                           BodyOverView = item.Description,
                           Type = "Industry"
                       });
                       return data;
                   }).Select(t => t.Result).ToList();

                var manufacture = (await _manufacturerService.SearchManufacturersAsync(keywords: keyword, searchManufactureTags: true))
                   .Select(async item =>
                   {
                       data.Add(new
                       {
                           Id = item.Id,
                           Title = item.Name,
                           SeName = await _urlRecordService.GetSeNameAsync(item, 0, true, false),
                           Picture = await _pictureService.GetPictureUrlAsync(item.PictureId, targetSize: pictureSize),
                           BodyOverView = item.ShortDescription,
                           Type = "Brand"
                       });
                       return data;
                   }).Select(t => t.Result).ToList();

                var blog = (await _blogService.GetAllBlogPostsByTagAsync(tag: keyword, showHidden: true))
                   .Select(async item =>
                   {
                       data.Add(new
                       {
                           Id = item.Id,
                           Title = item.Title,
                           SeName = await _urlRecordService.GetSeNameAsync(item, 0, true, false),
                           Picture = await _pictureService.GetPictureUrlAsync(item.PictureId, targetSize: pictureSize),
                           BodyOverView = item.BodyOverview,
                           Type = "Blog"
                       });
                       return data;
                   }).Select(t => t.Result).ToList();

                var topic = (await _topicService.SearchTopicsAsync(keywords: keyword, searchTopicTags: true))
                   .Select(async item =>
                   {
                       data.Add(new
                       {
                           Id = item.Id,
                           Title = item.Title,
                           SeName = await _urlRecordService.GetSeNameAsync(item, 0, true, false),
                           Picture = "",
                           BodyOverView = item.MetaDescription,
                           Type = "Topic"
                       });
                       return data;
                   }).Select(t => t.Result).ToList();

                return data;
            }
            return data;
        }
        public virtual async Task<object> GetAllCustomerTestimonials(int pageIndex, int pageSize)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.CustomerTestimonialsCacheKey, $"PageSizePrefix{pageSize}", $"PageIndexPrefix{pageIndex}");

            var data = await _staticCacheManager.GetAsync(key, async () =>
            {
                var customerTestimonials = await _customerTestimonialService.GetAllCustomerTestimonialsAsync(pageIndex: pageIndex, pageSize: pageSize);
                var tasks = customerTestimonials.Select(async x =>
                {
                    return new
                    {
                        Designation = x.Designation,
                        Title = x.Title,
                        Description = x.Description,
                        Id = x.Id,
                        Picture = (await _awsS3FilesService.GetAwsS3FilesByIdAsync(x.PictureId)).FileUrl
                    };
                });
                var customerTestimonialsData = await Task.WhenAll(tasks);
                return customerTestimonialsData;
            });
            return data;
        }
        public virtual async Task<object> GetAllEmployeeInsights(int pageIndex, int pageSize)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.EmployeeInsightsCacheKey, $"PageSizePrefix{pageSize}", $"PageIndexPrefix{pageIndex}");

            var data = await _staticCacheManager.GetAsync(key, async () =>
            {
                var employeeInsights = await _employeeInsightsService.GetAllEmployeeInsightsAsync(pageIndex: pageIndex, pageSize: pageSize);
                var tasks = employeeInsights.Select(async x =>
                {
                    return new
                    {
                        Designation = x.Designation,
                        Title = x.Title,
                        Description = x.Description,
                        Id = x.Id,
                        Picture = (await _awsS3FilesService.GetAwsS3FilesByIdAsync(x.PictureId)).FileUrl
                    };
                });
                var employeeInsightsData = await Task.WhenAll(tasks);
                return employeeInsightsData;
            });
            return data;
        }
        public virtual async Task<object> GetMarketPlaceExchangerate()
        {
            var currentDate = DateTime.UtcNow.Date;
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCatalogDefaults.MarketPlaceExchangeRateCacheKey, $"DateTimePrefix{currentDate}");
            var data = await _staticCacheManager.GetAsync(key, async () =>
            {
                return (await _marketPlaceExchangerateService.GetAllMarketPlaceExchangeRateByDateAsync(currentDate))
                      .Select(async item =>
                      {
                          var prevRate = await _marketPlaceExchangerateService.GetAllMarketPlaceExchangeRateByNameAsync(item.Currency, currentDate.AddDays(-1));
                          decimal percentChange = 0;

                          if (prevRate != null)
                              percentChange = (((item.Rate - prevRate.Rate) / prevRate.Rate) * 100);
                          return new
                          {
                              Currency = item.Currency,
                              Symbol = item.Symbol,
                              Rate = item.Rate,
                              Id = item.Id,
                              Percentage = Math.Round(percentChange, 2),
                          };
                      }).Select(t => t.Result).ToList();
            });

            if (data.Count <= 0)
                throw new ApplicationException("no data found");

            return data;
        }
        public virtual async Task<string> AddOnlineLeadAsync(OnlineLeadRequestModel model)
        {
            var checkEmail = model.Email;

            if (string.IsNullOrEmpty(checkEmail))
            {
                if (!string.IsNullOrEmpty(model.ContactNumber))
                {
                    checkEmail = model.ContactNumber.Trim() + "@zaraye.co";
                }
                else
                {
                    throw new ApplicationException("Contact number is required");
                }
            }

            var onlineLead = new OnlineLead()
            {
                OnlineLeadStatus = OnlineLeadStatus.Pending,
                CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id,
                Name = model.Name,
                Service = model.Service,
                Quantity = model.Quantity,
                Unit = model.Unit,
                CountryId = model.CountryId,
                CityName = model.CityName,
                CityId = model.CityId,
                ContactNumber = model.ContactNumber,
                Email = checkEmail,
                Source = model.Source,
                CreatedOnUtc = DateTime.UtcNow,
                Deleted = false,
                
            };

            await _onlineLeadService.InsertOnlineLeadAsync(onlineLead);
            return "Lead added successfully";
        }
        #endregion
    }
}
