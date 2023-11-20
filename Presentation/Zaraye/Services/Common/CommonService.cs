using Azure.Storage.Blobs.Models;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUglify;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Zaraye.Core;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.PriceDiscovery;
using Zaraye.Data;
using Zaraye.Models.Api.V4.Common;
using Zaraye.Models.Api.V4.OrderManagement;
using Zaraye.Services.Blogs;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.Common.Pdf;
using Zaraye.Services.Configuration;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.Helpers;
using Zaraye.Services.Html;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Media;
using Zaraye.Services.Messages;
using Zaraye.Services.Orders;
using Zaraye.Services.PriceDiscovery;
using Zaraye.Services.Seo;
using Zaraye.Services.Shipping;
using Zaraye.Services.Topics;
using Zaraye.Services.Utility;
using static Zaraye.Models.Api.V4.Common.CommonApiModel;

namespace Zaraye.Services.Common
{
    public class CommonService : ICommonService
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICategoryService _categoryService;
        private readonly IIndustryService _industryService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IAppVersionService _appVersionService;
        private readonly IPictureService _pictureService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ILanguageService _languageService;
        private readonly ICustomerService _customerService;
        private readonly IMeasureService _measureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IJaizaService _jaizaService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderService _orderService;
        private readonly IFaqService _faqService;
        private readonly IAppFeedBackService _appFeedBackService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IRequestService _requestService;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceDiscoveryService _priceDiscoveryService;
        private readonly ISettingService _settingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ITopicService _topicService;
        private readonly IBlogService _blogService;
        private readonly MediaSettings _mediaSettings;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IWebSliderService _webSliderService;
        private readonly IAwsS3FilesService _awsS3FilesService;
        private readonly ICommodityDataService _commodityDataService;
        private readonly IUtilityService _utilityService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IProductModelService _productModelService;
        private readonly IAmazonS3BuketService _amazonS3BuketService;
        private readonly Core.Domain.Common.CommonSettings _commonSettings;
        private readonly IHtmlFormatter _htmlFormatter;
        #endregion

        #region Ctor

        public CommonService(
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICategoryService categoryService,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IAppVersionService appVersionService,
            IPictureService pictureService,
            IManufacturerService manufacturerService,
            ILanguageService languageService,
            ICustomerService customerService,
            IMeasureService measureService,
            IProductAttributeParser productAttributeParser,
            IDownloadService downloadService,
            IJaizaService jaizaService,
            IPriceFormatter priceFormatter,
            IOrderService orderService,
            IFaqService faqService,
            IAppFeedBackService appFeedBackService,
            IWorkflowMessageService workflowMessageService,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            CatalogSettings catalogSettings,
            IIndustryService industryService,
            IRequestService requestService,
            IShippingService shippingService,
            IShipmentService shipmentService,
            IShoppingCartService shoppingCartService,
            IDateTimeHelper dateTimeHelper,
            IPriceDiscoveryService priceDiscoveryService,
            ISettingService settingService,
            IUrlRecordService urlRecordService,
            ITopicService topicService,
            IBlogService blogService,
            MediaSettings mediaSettings,
            IStaticCacheManager staticCacheManager,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<Product> productRepository,
            IWebSliderService webSliderService,
            IAwsS3FilesService awsS3FilesService,
            ICommodityDataService commodityDataService, IUtilityService utilityService, IRecentlyViewedProductsService recentlyViewedProductsService,
            IProductModelService productModelService, IAmazonS3BuketService amazonS3BuketService, Core.Domain.Common.CommonSettings commonSettings, IHtmlFormatter htmlFormatter
            )
        {
            _storeContext = storeContext;
            _localizationService = localizationService;
            _workContext = workContext;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _categoryService = categoryService;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _appVersionService = appVersionService;
            _pictureService = pictureService;
            _manufacturerService = manufacturerService;
            _languageService = languageService;
            _customerService = customerService;
            _measureService = measureService;
            _productAttributeParser = productAttributeParser;
            _downloadService = downloadService;
            _jaizaService = jaizaService;
            _priceFormatter = priceFormatter;
            _orderService = orderService;
            _faqService = faqService;
            _appFeedBackService = appFeedBackService;
            _workflowMessageService = workflowMessageService;
            _customerActivityService = customerActivityService;
            _genericAttributeService = genericAttributeService;
            _catalogSettings = catalogSettings;
            _industryService = industryService;
            _requestService = requestService;
            _shippingService = shippingService;
            _shipmentService = shipmentService;
            _shoppingCartService = shoppingCartService;
            _dateTimeHelper = dateTimeHelper;
            _priceDiscoveryService = priceDiscoveryService;
            _settingService = settingService;
            _urlRecordService = urlRecordService;
            _topicService = topicService;
            _blogService = blogService;
            _mediaSettings = mediaSettings;
            _staticCacheManager = staticCacheManager;
            _productCategoryRepository = productCategoryRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _productService = productService;
            _webSliderService = webSliderService;
            _awsS3FilesService = awsS3FilesService;
            _commodityDataService = commodityDataService;
            _utilityService = utilityService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _productModelService = productModelService;
            _amazonS3BuketService = amazonS3BuketService;
            _commonSettings = commonSettings;
            _htmlFormatter = htmlFormatter;
        }
        #endregion

        #region Country / Cities / Areas

        public async Task<object> AllCountries()
        {
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();
            var countries = await _countryService.GetAllCountriesAsync(workingLanguage.Id, isAppPublished: true);

            if (countries == null || countries.Count == 0)
                throw new ApplicationException("Record not found");

            var data = countries.Select(c => new
            {
                Text = _localizationService.GetLocalizedAsync(c, x => x.Name).Result,
                Value = c.Id.ToString(),
            }).ToList();
            return data;
        }

        public async Task<object> AllStates(int countryId)
        {
            if (countryId <= 0)
                throw new ApplicationException("Country id is required");

            var workingLanguage = await _workContext.GetWorkingLanguageAsync();
            var stateProvinces = await _stateProvinceService.GetStateProvincesByCountryIdAsync(countryId, workingLanguage.Id);

            if (stateProvinces == null || stateProvinces.Count == 0)
                throw new ApplicationException("Record not found");
            var localizedStateProvinces = stateProvinces.Select(async c => new
            {
                Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                Value = c.Id.ToString(),
            });

            var data = await Task.WhenAll(localizedStateProvinces);
            return data;
        }


        public async Task<object> AllAreas(int cityId)
        {
            if (cityId <= 0)
                throw new ApplicationException("City id is required");

            var areas = await _stateProvinceService.GetAllAreasByCityIdAsync(cityId);

            if (areas == null || areas.Count == 0)
                throw new ApplicationException("Record not found");

            var localizedAreas = areas.Select(async c => new
            {
                Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                Value = c.Id.ToString(),
            });

            var data = await Task.WhenAll(localizedAreas);
            return data;
        }


        #endregion

        #region Brand / Industry / category / product  / attributes

        public async Task<object> GetUnitByProductId(int productId)
        {
            if (productId <= 0)
                throw new ApplicationException("Product Id is required");
            var product = await _productService.GetProductByIdAsync(productId);
            if (product is null)
                throw new ApplicationException("Product not found");

            var unit = await _measureService.GetMeasureWeightByIdAsync(product.UnitId);
            if (unit is null)
                throw new ApplicationException("Unit not found");

            var data = new { unitId = unit.Id, unitName = unit.Name };
            return data;
        }

        public async Task<object> AllBrands(int productId)
        {
            if (productId <= 0)
                throw new ApplicationException("Product Id is required");
            var brands = await _manufacturerService.GetAllBrandsByProductIdAsync(productId, isAppPublished: true);

            if (brands == null || brands.Count == 0)
                throw new ApplicationException("Record not found");
            var customer = await _workContext.GetCurrentCustomerAsync();

            var localizedBrands = brands.Select(async i => new
            {
                Text = await _localizationService.GetLocalizedAsync(i, x => x.Name, languageId: customer.LanguageId),
                Value = i.Id.ToString()
            });

            var data = await Task.WhenAll(localizedBrands);

            return data;
        }

        public async Task<object> GetAllProductFilters()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var industryDataList = (await _industryService.GetAllIndustriesAsync("", (await _storeContext.GetCurrentStoreAsync()).Id, overridePublished: true))
                .Select(async item =>
                {
                    return new
                    {
                        item.Id,
                        ParentId = 0,
                        Name = await _localizationService.GetLocalizedAsync(item, x => x.Name, languageId: customer.LanguageId),
                        Picture = await _pictureService.GetPictureUrlAsync(item.PictureId > 0 ? item.PictureId : 0, 0, showDefaultPicture: true),
                    };
                }).Select(t => t.Result).ToList();

            var categoryDataList = (await _categoryService.GetAllCategoriesAsync("", (await _storeContext.GetCurrentStoreAsync()).Id, overridePublished: true))
                  .Select(async item =>
                  {
                      return new
                      {
                          item.Id,
                          ParentId = item.IndustryId,
                          Name = await _localizationService.GetLocalizedAsync(item, x => x.Name, languageId: customer.LanguageId),
                          Picture = await _pictureService.GetPictureUrlAsync(item.PictureId > 0 ? item.PictureId : 0, 0, showDefaultPicture: true),
                      };
                  }).Select(t => t.Result).ToList();

            var brandDataList = (await _manufacturerService.GetAllManufacturersAsync("", (await _storeContext.GetCurrentStoreAsync()).Id, overridePublished: true))
                  .Select(async item =>
                  {
                      return new
                      {
                          item.Id,
                          ParentId = item.IndustryId,
                          Name = await _localizationService.GetLocalizedAsync(item, x => x.Name, languageId: customer.LanguageId),
                          Picture = await _pictureService.GetPictureUrlAsync(item.PictureId > 0 ? item.PictureId : 0, targetSize: _mediaSettings.ManufacturerThumbPictureSize, showDefaultPicture: true),
                      };
                  }).Select(t => t.Result).ToList();

            var data = new
            {
                Industries = industryDataList,
                Categories = categoryDataList,
                Brands = brandDataList
            };
            if (data is null)
                throw new ApplicationException("Record not found");
            return data;
        }

        public async Task<object> GetAllFilteredProduct(ProductFilterModel model)
        {
            List<int> industriesIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.IndustryIds))
                industriesIds = model.IndustryIds.Split(',').Select(int.Parse).ToList();


            List<int> categoriesIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.CategoriesIds))
                categoriesIds = model.CategoriesIds.Split(',').Select(int.Parse).ToList();

            List<int> brandsids = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.BrandsIds))
                brandsids = model.BrandsIds.Split(',').Select(int.Parse).ToList();

            //todo
            var customer = await _workContext.GetCurrentCustomerAsync();

            var products = await _productService.GetProductsByIndustryCategoryAndBrandIdsAsync(industryIds: industriesIds, categoryIds: categoriesIds, manufacturerIds: brandsids, pageIndex: model.PageIndex, pageSize: model.PageSize);
            var pictureSize = _mediaSettings.ProductThumbPictureSize;

            var productList = products.Select(async productItem =>
                {
                    var productSeName = await _urlRecordService.GetSeNameAsync(productItem);
                    var picture = (await _productService.GetProductPicturesByProductIdAsync(productItem.Id)).FirstOrDefault();
                    return new
                    {
                        Name = await _localizationService.GetLocalizedAsync(productItem, x => x.Name, languageId: customer.LanguageId),
                        SeName = productSeName,
                        productItem.Id,
                        productItem.MetaTitle,
                        MetaDescription = await _localizationService.GetLocalizedAsync(productItem, x => x.MetaDescription, languageId: customer.LanguageId),
                        productItem.MetaKeywords,
                        Picture = await _pictureService.GetPictureUrlAsync(picture == null ? 0 : picture.PictureId, targetSize: pictureSize)
                    };
                }).Select(t => t.Result).ToList();

            var data = new
            {
                products.PageIndex,
                products.PageSize,
                products.TotalCount,
                products.TotalPages,
                products.HasPreviousPage,
                products.HasNextPage,
                Products = productList
            };
            if (data is null)
                throw new ApplicationException("Record not found");
            return data;
        }

        public async Task<object> GetAllSeNames()
        {
            var list = new List<AllSeNames>();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var industryData = await _industryService.GetAllIndustriesAsync("", (await _storeContext.GetCurrentStoreAsync()).Id, overridePublished: true);
            foreach (var industryItem in industryData)
            {
                var industrySeName = await _urlRecordService.GetSeNameAsync(industryItem);
                list.Add(new AllSeNames()
                {
                    Slug = industrySeName,
                    Type = "Industry",
                    Id = industryItem.Id,
                    Title = await _localizationService.GetLocalizedAsync(industryItem, x => x.Name, languageId: customer.LanguageId),
                    MetaTitle = industryItem.MetaTitle,
                    MetaDescription = await _localizationService.GetLocalizedAsync(industryItem, x => x.MetaDescription, languageId: customer.LanguageId),
                    MetaKeywords = industryItem.MetaKeywords
                });
                var categoriesData = await _categoryService.GetAllCategoriesByIndustryIdAsync(industryItem.Id, isAppPublished: true);
                foreach (var categoriesItem in categoriesData)
                {
                    var categoriesSeName = await _urlRecordService.GetSeNameAsync(categoriesItem);
                    list.Add(new AllSeNames()
                    {
                        Slug = categoriesSeName,
                        Type = "Category",
                        Title = await _localizationService.GetLocalizedAsync(categoriesItem, x => x.Name, languageId: customer.LanguageId),
                        Id = categoriesItem.Id,
                        MetaTitle = categoriesItem.MetaTitle,
                        MetaDescription = await _localizationService.GetLocalizedAsync(categoriesItem, x => x.MetaDescription, languageId: customer.LanguageId),
                        MetaKeywords = categoriesItem.MetaKeywords
                    });

                }
            }
            var productData = await _productService.GetAllProductsAsync();
            foreach (var productItem in productData)
            {
                var productSeName = await _urlRecordService.GetSeNameAsync(productItem);
                list.Add(new AllSeNames()
                {
                    Slug = productSeName,
                    Type = "Product",
                    Title = await _localizationService.GetLocalizedAsync(productItem, x => x.Name, languageId: customer.LanguageId),
                    Id = productItem.Id,
                    MetaTitle = productItem.MetaTitle,
                    MetaDescription = await _localizationService.GetLocalizedAsync(productItem, x => x.MetaDescription, languageId: customer.LanguageId),
                    MetaKeywords = productItem.MetaKeywords
                });
                var brandData = await _manufacturerService.GetAllBrandsByProductIdAsync(productItem.Id, isAppPublished: true);
                foreach (var brandItem in brandData)
                {
                    var brandSeName = await _urlRecordService.GetSeNameAsync(brandItem);
                    list.Add(new AllSeNames()
                    {
                        Slug = brandSeName,
                        Type = "Brand",
                        Title = await _localizationService.GetLocalizedAsync(brandItem, x => x.Name, languageId: customer.LanguageId),
                        Id = brandItem.Id,
                        MetaTitle = brandItem.MetaTitle,
                        MetaDescription = await _localizationService.GetLocalizedAsync(brandItem, x => x.MetaDescription, languageId: customer.LanguageId),
                        MetaKeywords = brandItem.MetaKeywords
                    });
                }
            }
            //get all topics
            var topicData = await _topicService.GetAllTopicsAsync((await _storeContext.GetCurrentStoreAsync()).Id, showHidden: true);
            foreach (var topicItem in topicData)
            {
                var topicsSeName = await _urlRecordService.GetSeNameAsync(topicItem);
                list.Add(new AllSeNames()
                {
                    Slug = topicsSeName,
                    Type = "Topic",
                    Id = topicItem.Id,
                    Title = await _localizationService.GetLocalizedAsync(topicItem, x => x.Title, languageId: customer.LanguageId),
                    MetaTitle = topicItem.MetaTitle,
                    MetaDescription = await _localizationService.GetLocalizedAsync(topicItem, x => x.MetaDescription, languageId: customer.LanguageId),
                    MetaKeywords = topicItem.MetaKeywords
                });
            }
            //get all blogs
            var blogData = await _blogService.GetAllBlogPostsAsync((await _storeContext.GetCurrentStoreAsync()).Id, showHidden: true);
            foreach (var blogItem in blogData)
            {
                var blogsSeName = await _urlRecordService.GetSeNameAsync(blogItem);
                list.Add(new AllSeNames()
                {
                    Slug = blogsSeName,
                    Type = "Blog",
                    Id = blogItem.Id,
                    Title = blogItem.Title,
                    MetaTitle = blogItem.MetaTitle,
                    MetaDescription = blogItem.MetaDescription,
                    MetaKeywords = blogItem.MetaKeywords
                });
            }
            return list;
        }

        //------------get all blogs
        public async Task<object> GetAllBlogs(int pageIndex = 0, int pagesize = 5)
        {
            var blogData = await _blogService.GetAllBlogPostsAsync(showHidden: false, pageIndex: pageIndex, pageSize: pagesize);

            if (blogData.Count() == 0)
            {
                throw new ApplicationException("Blogs not found");
            }

            var blogList = blogData.SelectAwait(async blog => new
            {
                blog.Id,
                SeName = _urlRecordService.GetSeNameAsync(blog).Result,
                PublishedOn = _dateTimeHelper.ConvertToUserTimeAsync(blog.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("MMMM dd yyyy"),
                CreatedByName = _customerService.GetCustomerByIdAsync(blog.CreatedById).Result?.FullName,
                Title = await _localizationService.GetLocalizedAsync(blog, x => x.Title),
                Body = await _localizationService.GetLocalizedAsync(blog, x => x.Body),
                BodyOverview = await _localizationService.GetLocalizedAsync(blog, x => x.BodyOverview),
                AutorName = blog.AuthorName,
                Picture = _pictureService.GetPictureUrlAsync(blog.PictureId).Result,
                blog.MetaTitle,
                MetaKeyword = blog.MetaKeywords,
                blog.MetaDescription
            }).ToListAsync();

            var data = new
            {
                blogData.PageIndex,
                blogData.PageSize,
                blogData.TotalCount,
                blogData.TotalPages,
                blogData.HasPreviousPage,
                blogData.HasNextPage,
                Blogs = blogList.Result,
            };

            if (data == null)
                throw new ApplicationException("Record not found");
            return data;
        }


        public async Task<object> GetLatestBlogs(int numberofblog = 4)
        {
            var storeId = (await _storeContext.GetCurrentStoreAsync())?.Id;
            var data = (await _blogService.GetAllBlogPostsAsync(showHidden: false))
                .Where(blog => numberofblog == 4 || blog.StoreId == storeId)
                .OrderByDescending(blog => blog.Id)
                .Take(numberofblog)
                .Select(async blog => new
                {
                    blog.Id,
                    SeName = await _urlRecordService.GetSeNameAsync(blog),
                    PublishedOn = (await _dateTimeHelper.ConvertToUserTimeAsync(blog.CreatedOnUtc, DateTimeKind.Utc)).ToString("MMMM dd yyyy"),
                    CreatedByName = (await _customerService.GetCustomerByIdAsync(blog.CreatedById))?.FullName,
                    Title = await _localizationService.GetLocalizedAsync(blog, x => x.Title),
                    Body = await _localizationService.GetLocalizedAsync(blog, x => x.Body),
                    BodyOverview = await _localizationService.GetLocalizedAsync(blog, x => x.BodyOverview),
                    AutorName = blog.AuthorName,
                    Picture = await _pictureService.GetPictureUrlAsync(blog.PictureId),
                    blog.MetaTitle,
                    MetaKeyword = blog.MetaKeywords,
                    blog.MetaDescription
                })
                .Select(t => t.Result)
                .ToList();

            if (data == null || data.Count == 0)
                throw new ApplicationException("Record not found.");
            return data;
        }
        public async Task<object> GetBlogDetailById(int blogId = 0)
        {
            if (blogId <= 0)
                throw new ApplicationException("BlogId is required");
            var blog = await _blogService.GetBlogPostByIdAsync(blogId);
            if (blog is null)
                throw new ApplicationException("Record not found.");
            var publishedDate = await _dateTimeHelper.ConvertToUserTimeAsync(blog.CreatedOnUtc, DateTimeKind.Utc);
            var data = new
            {
                id = blog.Id,
                seName = await _urlRecordService.GetSeNameAsync(blog),
                Title = await _localizationService.GetLocalizedAsync(blog, x => x.Title),
                Body = await _localizationService.GetLocalizedAsync(blog, x => x.Body),
                BodyOverview = await _localizationService.GetLocalizedAsync(blog, x => x.BodyOverview),
                authorName = blog.AuthorName,
                publishedOn = publishedDate.ToString("MMMM dd yyyy"),
                picture = await _pictureService.GetPictureUrlAsync(blog.PictureId),
                metaTitle = blog.MetaTitle,
                metaKeyword = blog.MetaKeywords,
                metaDescription = blog.MetaDescription
            };
            return data;
        }
        public async Task<object> GetTopicDetailById(int topicId = 0)
        {
            if (topicId <= 0)
                throw new ApplicationException("Topic Id is required");
            var topic = await _topicService.GetTopicByIdAsync(topicId);

            if (topic == null)
                throw new ApplicationException("Record not found.");
            var customer = await _workContext.GetCurrentCustomerAsync();

            var data = new
            {
                topic.Id,
                Title = await _localizationService.GetLocalizedAsync(topic, x => x.Title, languageId: customer.LanguageId),
                Body = await _localizationService.GetLocalizedAsync(topic, x => x.Body, languageId: customer.LanguageId),
                SeName = await _urlRecordService.GetSeNameAsync(topic),
                topic.MetaTitle,
                MetaKeyword = topic.MetaKeywords,
                MetaDescription = await _localizationService.GetLocalizedAsync(topic, x => x.MetaDescription, languageId: customer.LanguageId),
            };

            return data;
        }

        public async Task<object> GetAllBrands()
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var brands = await _manufacturerService.GetAllManufacturersAsync(storeId: currentStore.Id, overridePublished: true);
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (brands == null || brands.Count <= 0)
            {
                throw new ApplicationException("Record not found.");
            }

            var pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
            var data = brands.Select(async item =>
            {
                return new
                {
                    item.Id,
                    BrandName = await _localizationService.GetLocalizedAsync(item, x => x.Name, languageId: customer.LanguageId),
                    SeName = await _urlRecordService.GetSeNameAsync(item),
                    Picture = await _pictureService.GetPictureUrlAsync(item.PictureId, targetSize: pictureSize),
                    item.IndustryId
                };
            });

            return (await Task.WhenAll(data)).ToList();
        }


        public async Task<object> AllIndustries(string type)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var industries = await _industryService.GetAllIndustriesAsync("", currentStore.Id, isAppPublished: true);

            var data = await Task.WhenAll(industries.Select(async i =>
            {
                var categoryIds = new List<int>();

                if (type != "Request")
                {
                    var categories = await _categoryService.GetAllCategoriesByIndustryIdAsync(i.Id, isAppPublished: true);
                    categoryIds.AddRange(categories.Select(x => x.Id));
                }

                var pictureId = i.PictureId > 0 ? i.PictureId : 0;
                var numberOfProducts = type == "Request" ?
                    (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Verified }, industryId: i.Id, getOnlyTotalCount: true)).TotalCount :
                    await _productService.GetNumberOfProductsInCategoryAsync(categoryIds, currentStore.Id);

                var localizedName = await _localizationService.GetLocalizedAsync(i, x => x.Name, languageId: customer.LanguageId);

                return new
                {
                    Text = localizedName,
                    Value = i.Id.ToString(),
                    Picture = await _pictureService.GetPictureUrlAsync(pictureId, 0, showDefaultPicture: true),
                    SeName = await _urlRecordService.GetSeNameAsync(i),
                    NumberOfProducts = numberOfProducts
                };
            }));

            return data.ToList();
        }


        public async Task<object> AllCategories(int industryId, bool showNumberOfProducts = true)
        {
            if (industryId <= 0)
            {
                throw new ApplicationException("Industry id is required");
            }

            var categories = await _categoryService.GetAllCategoriesByIndustryIdAsync(industryId, isAppPublished: true);
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();

            var data = await Task.WhenAll(categories.Select(async c =>
            {
                var categoryIds = new List<int> { c.Id };
                categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(c.Id, currentStore.Id));

                var pictureId = c.PictureId > 0 ? c.PictureId : 0;
                var numberOfProducts = showNumberOfProducts ? await _productService.GetNumberOfProductsInCategoryAsync(categoryIds, currentStore.Id) : 0;

                return new
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId: customer.LanguageId),
                    Value = c.Id.ToString(),
                    Picture = await _pictureService.GetPictureUrlAsync(pictureId, showDefaultPicture: true),
                    NumberOfProducts = numberOfProducts
                };
            }));

            return data.ToList();
        }

        public async Task<object> GetAllTopCategories()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var data = (await _categoryService.GetAllCategoriesAsync())
                .Where(x => x.IncludeInTopCategory == true)
                .Select(async c =>
                {
                    var pictureId = c.PictureId > 0 ? c.PictureId : 0;
                    return new
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId: customer.LanguageId),
                        Value = c.Id.ToString(),
                        Picture = await _pictureService.GetPictureUrlAsync(pictureId, showDefaultPicture: true),
                        SeName = await _urlRecordService.GetSeNameAsync(c),
                    };
                })
                .Select(t => t.Result)
                .ToList();

            return data;
        }

        public async Task<object> CategoriesAndProductsByIndustryId(int industryId)
        {
            if (industryId <= 0)
                throw new ApplicationException("Industry id is required");
            var categories = await _categoryService.GetAllCategoriesByIndustryIdAsync(industryId, isAppPublished: true);
            var customer = await _workContext.GetCurrentCustomerAsync();

            var data = categories.Select(async category =>
            {
                var products = (await _productService.GetAllProductsByCategoryIdAsync(category.Id, isAppPublished: true)).Select(async p =>
                {
                    return new
                    {
                        ProductId = p.Id.ToString(),
                        ProductName = await _localizationService.GetLocalizedAsync(p, x => x.Name, languageId: customer.LanguageId)
                    };
                }).Select(t => t.Result).ToList();

                return new
                {
                    CategoryId = category.Id,
                    CategoryName = await _localizationService.GetLocalizedAsync(category, x => x.Name, languageId: customer.LanguageId),
                    Products = products
                };
            }).Select(t => t.Result).ToList();

            return data;
        }

        //-----category detail by ID
        public async Task<object> GetCategorydetails(int categoryId)
        {
            if (categoryId <= 0)
                throw new Exception("Category id is required");

            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var categoryItem = await _categoryService.GetCategoryByIdAsync(categoryId);
                if (categoryItem == null)
                    throw new Exception("Category not found against the input id");

                var productList = (await GetProductListForCategoryAsync(categoryId)).ToList();

                var data = new
                {
                    categoryItem.Id,
                    Name = await _localizationService.GetLocalizedAsync(categoryItem, x => x.Name, languageId: customer.LanguageId),
                    SeName = await _urlRecordService.GetSeNameAsync(categoryItem),
                    Picture = await GetPictureUrlAsync(categoryItem.PictureId, _mediaSettings.CategoryThumbPictureSize),
                    categoryItem.MetaKeywords,
                    MetaDescription = await _localizationService.GetLocalizedAsync(categoryItem, x => x.MetaDescription, languageId: customer.LanguageId),
                    Metatitle = categoryItem.MetaTitle,
                    Description = await _localizationService.GetLocalizedAsync(categoryItem, x => x.Description, languageId: customer.LanguageId),
                    ShortDescription = await _localizationService.GetLocalizedAsync(categoryItem, x => x.ShortDescription, languageId: customer.LanguageId),
                    Products = productList,
                };

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<IEnumerable<object>> GetProductListForCategoryAsync(int categoryId)
        {
            var productData = await _productService.SearchProductsAsync(categoryIds: new List<int> { categoryId });
            var customer = await _workContext.GetCurrentCustomerAsync();

            var productList = productData.Select(async product =>
            {
                var picture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault();
                return new
                {
                    Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId: customer.LanguageId),
                    Id = product.Id.ToString(),
                    Picture = await _pictureService.GetPictureUrlAsync(picture?.PictureId ?? 0, _mediaSettings.ProductThumbPictureSize),
                    SeName = await _urlRecordService.GetSeNameAsync(product),
                    product.MetaKeywords,
                    MetaDescription = await _localizationService.GetLocalizedAsync(product, x => x.MetaDescription, languageId: customer.LanguageId),
                    product.MetaTitle,
                };
            }).Select(t => t.Result);

            return productList;
        }
        public async Task<object> GetIndustriesAndCategories()
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var industriesData = await _industryService.GetAllIndustriesAsync("", currentStore.Id);
            var customer = await _workContext.GetCurrentCustomerAsync();

            var data = industriesData.Select(async industryItem =>
            {
                var categoriesData = await _categoryService.GetAllCategoriesByIndustryIdAsync(industryItem.Id);
                var categoryList = categoriesData.Select(async categoryItem =>
                {
                    return new
                    {
                        categoryItem.Id,
                        Name = await _localizationService.GetLocalizedAsync(categoryItem, x => x.Name, languageId: customer.LanguageId),
                        SeName = await _urlRecordService.GetSeNameAsync(categoryItem),
                        Picture = await GetPictureUrlAsync(categoryItem.PictureId, _mediaSettings.CategoryThumbPictureSize),
                    };
                }).Select(t => t.Result).ToList();

                return new
                {
                    industryItem.Id,
                    Name = await _localizationService.GetLocalizedAsync(industryItem, x => x.Name, languageId: customer.LanguageId),
                    SeName = await _urlRecordService.GetSeNameAsync(industryItem),
                    Picture = await GetPictureUrlAsync(industryItem.PictureId, 0),
                    Categories = categoryList,
                };
            }).Select(t => t.Result).ToList();
            return data;
        }

        private async Task<string> GetPictureUrlAsync(int pictureId, int targetSize)
        {
            return await _pictureService.GetPictureUrlAsync(pictureId, targetSize, showDefaultPicture: true);
        }


        public async Task<object> GetIndustryDetail(int Id)
        {

            if (Id <= 0)
                throw new ApplicationException("industry id is required");

            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var industry = await _industryService.GetIndustryByIdAsync(Id);

                if (industry == null)
                    throw new Exception("industry not found against the input id");

                //all-products-agaist-industry
                List<object> productList = new List<object>();
                var products = await _productService.GetAllProductsByIndustryIdAsync(industryId: industry.Id);
                foreach (var product in products)
                {
                    var picture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault();
                    productList.Add(new
                    {
                        Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId: customer.LanguageId),
                        Id = product.Id.ToString(),
                        Picture = await _pictureService.GetPictureUrlAsync(picture != null ? picture.PictureId : 0, 0, showDefaultPicture: true),
                        SeName = await _urlRecordService.GetSeNameAsync(product),
                        product.MetaKeywords,
                        MetaDescription = await _localizationService.GetLocalizedAsync(product, x => x.MetaDescription, languageId: customer.LanguageId),
                        product.MetaTitle,

                    });
                }

                //all-best-selling-products-agaist-industry
                var bestProducts = await _productService.GetAllBestSellingProductsByIndustryIdAsync(industryId: industry.Id);
                var bestProductList = new List<object>();
                foreach (var product in bestProducts)
                {
                    var productSeName = await _urlRecordService.GetSeNameAsync(product);
                    var picture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault();
                    bestProductList.Add(new
                    {
                        Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId: customer.LanguageId),
                        SeName = productSeName,
                        product.Id,
                        Picture = await _pictureService.GetPictureUrlAsync(picture == null ? 0 : picture.PictureId, targetSize: _mediaSettings.ProductThumbPictureSize),
                        product.MetaTitle,
                        MetaDescription = await _localizationService.GetLocalizedAsync(product, x => x.MetaDescription, languageId: customer.LanguageId),
                        product.MetaKeywords,
                    });
                }


                //all-categories-agaist-industry
                List<object> categoriesList = new List<object>();
                var categories = await _categoryService.GetAllCategoriesByIndustryIdAsync(industryId: industry.Id);
                foreach (var item in categories)
                {
                    categoriesList.Add(new
                    {
                        Name = await _localizationService.GetLocalizedAsync(item, x => x.Name, languageId: customer.LanguageId),
                        Id = item.Id.ToString(),
                        Picture = await _pictureService.GetPictureUrlAsync(item.PictureId, targetSize: _mediaSettings.CategoryThumbPictureSize),
                        SeName = await _urlRecordService.GetSeNameAsync(item),
                        item.MetaKeywords,
                        MetaDescription = await _localizationService.GetLocalizedAsync(item, x => x.MetaDescription, languageId: customer.LanguageId),
                        item.MetaTitle

                    });
                }

                //all-brands-agaist-industry
                List<object> brandList = new List<object>();
                var brands = await _manufacturerService.GetAllBrandsByIndustryIdAsync(industryId: industry.Id);
                foreach (var item in brands)
                {
                    brandList.Add(new
                    {
                        Name = await _localizationService.GetLocalizedAsync(item, x => x.Name, languageId: customer.LanguageId),
                        Id = item.Id.ToString(),
                        Picture = await _pictureService.GetPictureUrlAsync(item.PictureId, targetSize: _mediaSettings.ManufacturerThumbPictureSize),
                        SeName = await _urlRecordService.GetSeNameAsync(item),
                        item.MetaKeywords,
                        MetaDescription = await _localizationService.GetLocalizedAsync(item, x => x.MetaDescription, languageId: customer.LanguageId),
                        item.MetaTitle

                    });
                }


                var data = new
                {
                    industry.Id,
                    Name = await _localizationService.GetLocalizedAsync(industry, x => x.Name, languageId: customer.LanguageId),
                    SeName = await _urlRecordService.GetSeNameAsync(industry),
                    Picture = await _pictureService.GetPictureUrlAsync(industry.PictureId > 0 ? industry.PictureId : 0, 0, showDefaultPicture: true),
                    industry.MetaKeywords,
                    MetaDescription = await _localizationService.GetLocalizedAsync(industry, x => x.MetaDescription, languageId: customer.LanguageId),
                    Metatitle = industry.MetaTitle,
                    Description = await _localizationService.GetLocalizedAsync(industry, x => x.Description, languageId: customer.LanguageId),
                    Categories = categoriesList,
                    Products = productList,
                    BestSellingProducts = bestProductList,
                    Brands = brandList,
                };

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> AllProducts(int categoryId = 0, List<int> categoryIds = null)
        {
            if (categoryId <= 0 && (categoryIds == null || !categoryIds.Any()))
            {
                throw new ApplicationException("Category id or category ids are required");
            }

            var data = categoryId > 0
                ? await GetProductsByCategoryId(categoryId)
                : categoryIds != null && categoryIds.Any()
                ? await GetProductsByCategoryIds(categoryIds)
                : new List<object>();

            return data;
        }

        private async Task<List<object>> GetProductsByCategoryId(int categoryId)
        {
            var products = await _productService.GetAllProductsByCategoryIdAsync(categoryId, isAppPublished: true);
            return await ProcessProducts(products);
        }

        private async Task<List<object>> GetProductsByCategoryIds(List<int> categoryIds)
        {
            var products = await _productService.GetAllProductsByCategoryIdsAsync(categoryIds.ToArray());
            return await ProcessProducts(products);
        }

        private async Task<List<object>> ProcessProducts(IEnumerable<Product> products)
        {
            var result = new List<object>();
            var customer = await _workContext.GetCurrentCustomerAsync();

            foreach (var product in products)
            {
                var picture = await _productService.GetProductPictureByIdAsync(product.Id);
                var localizedName = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId: customer.LanguageId);
                var pictureUrl = await _pictureService.GetPictureUrlAsync(picture?.PictureId ?? 0, showDefaultPicture: true, targetSize: _mediaSettings.ProductThumbPictureSize);

                result.Add(new
                {
                    Text = localizedName,
                    Value = product.Id.ToString(),
                    Picture = pictureUrl,
                });
            }
            return result;
        }

        public async Task<object> AllProducts(int categoryId = 0)
        {
            if (categoryId <= 0)
                throw new Exception("Category id is required");

            var products = await _productService.GetAllProductsByCategoryIdAsync(categoryId, isAppPublished: true);
            var customer = await _workContext.GetCurrentCustomerAsync();

            var data = products.Select(async c => new
            {
                Text = _localizationService.GetLocalizedAsync(c, x => x.Name, languageId: customer.LanguageId).Result,
                Value = c.Id.ToString(),
            }).ToList();

            return data;
        }
        public async Task<List<ProductAttributesApiModel>> AllProductAttributes(int productId, string attributesXml = "")
        {
            if (productId <= 0)
                throw new Exception("product id is required");

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                throw new Exception("product not found");

            var data = new List<ProductAttributesApiModel>();
            var customer = await _workContext.GetCurrentCustomerAsync();

            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            foreach (var attribute in productAttributeMapping)
            {
                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);

                var attributeModel = new ProductAttributesApiModel
                {
                    Id = attribute.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = string.IsNullOrWhiteSpace(attribute.TextPrompt) ? await _localizationService.GetLocalizedAsync(productAttribute, x => x.Name, languageId: customer.LanguageId) : await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt, languageId: customer.LanguageId),
                    Description = await _localizationService.GetLocalizedAsync(productAttribute, x => x.Description, languageId: customer.LanguageId),
                    TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt, languageId: customer.LanguageId),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = (int)attribute.AttributeControlType,
                    DefaultValue = attributesXml != null ? null : await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue),
                    ValidationMinLength = attribute.ValidationMinLength,
                    ValidationMaxLength = attribute.ValidationMaxLength,
                    HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml),
                    Values = new List<ProductAttributesApiModel.ProductAttributeValueApiModel>(),
                };

                if (attribute.ShouldHaveValues())
                {
                    var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductAttributesApiModel.ProductAttributeValueApiModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name, languageId: customer.LanguageId),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity,
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                if (!string.IsNullOrWhiteSpace(attributesXml))
                {
                    await ProcessAttributesXml(attributeModel, attributesXml, attribute);
                }

                data.Add(attributeModel);
            }

            return data;
        }
        private async Task ProcessAttributesXml(ProductAttributesApiModel attributeModel, string attributesXml, ProductAttributeMapping attribute)
        {
            switch (attribute.AttributeControlType)
            {
                // Handle different AttributeControlTypes
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                    // Handle selection and quantity for certain attribute types
                    await ProcessAttributeSelectionAndQuantity(attributeModel, attributesXml, attribute);
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.NumericTextBox:
                case AttributeControlType.MultilineTextbox:
                    // Handle text input attributes
                    ProcessAttributeText(attributeModel, attributesXml, attribute);
                    break;
                case AttributeControlType.Datepicker:
                    // Handle date attributes
                    ProcessAttributeDate(attributeModel, attributesXml, attribute);
                    break;
                case AttributeControlType.FileUpload:
                    // Handle file upload attributes
                    ProcessAttributeFileUpload(attributeModel, attributesXml, attribute);
                    break;
                default:
                    break;
            }
        }

        private async Task ProcessAttributeSelectionAndQuantity(ProductAttributesApiModel attributeModel, string attributesXml, ProductAttributeMapping attribute)
        {
            if (!string.IsNullOrEmpty(attributesXml))
            {
                var selectedValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
                foreach (var attributeValue in selectedValues)
                {
                    foreach (var item in attributeModel.Values)
                    {
                        if (attributeValue.Id == item.Id)
                        {
                            item.IsPreSelected = true;
                            if (attributeValue.CustomerEntersQty)
                            {
                                item.Quantity = attributeValue.Quantity;
                            }
                        }
                    }
                }
            }
        }
        private void ProcessAttributeText(ProductAttributesApiModel attributeModel, string attributesXml, ProductAttributeMapping attribute)
        {
            if (!string.IsNullOrEmpty(attributesXml))
            {
                var enteredText = _productAttributeParser.ParseValues(attributesXml, attribute.Id);
                if (enteredText.Any())
                {
                    attributeModel.DefaultValue = enteredText[0];
                }
            }
        }

        private void ProcessAttributeDate(ProductAttributesApiModel attributeModel, string attributesXml, ProductAttributeMapping attribute)
        {
            var selectedDateStr = _productAttributeParser.ParseValues(attributesXml, attribute.Id);
            if (selectedDateStr.Any() && DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture, DateTimeStyles.None, out var selectedDate))
            {
                attributeModel.SelectedDay = selectedDate.Day;
                attributeModel.SelectedMonth = selectedDate.Month;
                attributeModel.SelectedYear = selectedDate.Year;
            }
        }
        private async void ProcessAttributeFileUpload(ProductAttributesApiModel attributeModel, string attributesXml, ProductAttributeMapping attribute)
        {
            if (!string.IsNullOrEmpty(attributesXml))
            {
                var downloadGuidStr = _productAttributeParser.ParseValues(attributesXml, attribute.Id).FirstOrDefault();
                if (Guid.TryParse(downloadGuidStr, out var downloadGuid))
                {
                    var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                    if (download != null)
                    {
                        attributeModel.DefaultValue = download.DownloadGuid.ToString();
                    }
                }
            }
        }

        public async Task<object> ProductAttributeChange(int productId, List<CombinationAttributeApiModel> attributesData)
        {
            if (productId <= 0)
                throw new Exception("product id is required");

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                throw new Exception("product not found");

            var attributeXml = await _utilityService.Common_ConvertToXmlAsync(attributesData, product.Id);

            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();

            var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            foreach (var attribute in attributes)
            {
                var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeMappingIds.Add(attribute.Id);
                    else
                        disabledAttributeMappingIds.Add(attribute.Id);
                }
            }

            var data = new
            {
                productId,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
            };

            return data;
        }

        public async Task<IEnumerable<object>> AllAppVersionsAsync(string platform = "", string type = "Consumer App")
        {
            var appVersions = await _appVersionService.GetAllAppVersionsAsync(type: type, platform: platform, onlyForceUpdate: true, pageIndex: 0, pageSize: 1);

            return appVersions.Select(i => new { i.Version });
        }
        public async Task<IEnumerable<object>> AllBuyerTypesAsync()
        {
            var buyerTypes = await _customerService.GetAllUserTypesAsync(type: "Buyer");

            var data = buyerTypes.Select(type => new
            {
                Text = type.Name,
                Value = type.Id.ToString()
            });

            return data;
        }
        public async Task<IEnumerable<object>> AllSupplierTypesAsync()
        {
            var supplierTypes = await _customerService.GetAllUserTypesAsync(type: "Supplier");

            var data = supplierTypes.Select(type => new
            {
                Text = type.Name,
                Value = type.Id.ToString()
            });

            return data;
        }

        public async Task<object> GetProductDetailsById(int productId, bool recentlyViewFlag = false)
        {
            if (productId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be a positive value.");
            var customer = await _workContext.GetCurrentCustomerAsync();
            var product = await _productService.GetProductByIdAsync(productId) ?? throw new InvalidOperationException("Product not found.");

            var unit = await _measureService.GetMeasureWeightByIdAsync(product.UnitId);
            var categories = (await _categoryService.GetProductCategoriesByProductIdAsync(productId))
                .Select(async item =>
                {
                    var categoryName = (await _categoryService.GetCategoryByIdAsync(item.CategoryId))?.Name;
                    var localizedName = await _localizationService.GetLocalizedAsync((await _categoryService.GetCategoryByIdAsync(item.CategoryId)), x => x.Name, languageId: customer.LanguageId);
                    return new { CategoryName = localizedName, Id = item.CategoryId };
                })
                .Select(t => t.Result)
                .ToList();

            var productReviews = await _productService.GetAllProductReviewsAsync(approved: true, productId: product.Id);
            var pictureSize = _mediaSettings.ProductThumbPictureSize;
            var picture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault();

            var data = new
            {
                Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId: customer.LanguageId),
                ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription, languageId: customer.LanguageId),
                LongDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId: customer.LanguageId),
                UnitName = unit?.Name ?? "",
                Categories = categories,
                Picture = await _pictureService.GetPictureUrlAsync(picture?.PictureId ?? 0, targetSize: pictureSize),
                ProductReviews = productReviews,
                SeName = await _urlRecordService.GetSeNameAsync(product),

            };

            if (!recentlyViewFlag)
                await _recentlyViewedProductsService.AddProductToRecentlyViewedListAsync(productId);
            return data;
        }
        public async Task<object> GetBrandDetailsById(int brandId = 0)
        {
            if (brandId <= 0)
                throw new ArgumentOutOfRangeException(nameof(brandId), "Brand ID must be a positive value.");
            var brand = await _manufacturerService.GetManufacturerByIdAsync(brandId);

            if (brand is null)
                throw new InvalidOperationException("Brand not found.");
            var products = await _manufacturerService.GetAllProductsByBrandIdAsync(brand.Id);
            var customer = await _workContext.GetCurrentCustomerAsync();

            var productData = products.Select(async item =>
            {
                var productSeName = await _urlRecordService.GetSeNameAsync(item);
                var picture = (await _productService.GetProductPicturesByProductIdAsync(item.Id)).FirstOrDefault();

                return new
                {
                    item.Id,
                    Name = await _localizationService.GetLocalizedAsync(item, x => x.Name, languageId: customer.LanguageId),
                    SeName = productSeName,
                    item.MetaTitle,
                    MetaDescription = await _localizationService.GetLocalizedAsync(item, x => x.MetaDescription, languageId: customer.LanguageId),
                    item.MetaKeywords,
                    Picture = await _pictureService.GetPictureUrlAsync(picture?.PictureId ?? 0, targetSize: _mediaSettings.ProductThumbPictureSize)
                };
            }).Select(t => t.Result).ToList();

            var brandDetails = new
            {
                Name = await _localizationService.GetLocalizedAsync(brand, x => x.Name, languageId: customer.LanguageId),
                ShortDescription = await _localizationService.GetLocalizedAsync(brand, x => x.ShortDescription, languageId: customer.LanguageId),
                LongDescription = await _localizationService.GetLocalizedAsync(brand, x => x.Description, languageId: customer.LanguageId),
                Picture = await _pictureService.GetPictureUrlAsync(brand.PictureId, targetSize: _mediaSettings.ManufacturerThumbPictureSize),
                Products = productData
            };

            return brandDetails;
        }

        #endregion

        #region Language Methods

        public async Task<Dictionary<string, string>> AllLocalizations(string resourceName, int languageId)
        {
            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(nameof(resourceName), "Resource name is required");
            if (languageId <= 0)
                throw new ArgumentOutOfRangeException(nameof(languageId), "Language ID must be a positive value");
            var localizationData = new Dictionary<string, string>();

            // Get locale resources
            var localeResources = await _localizationService.GetAllResourceValuesAsync(languageId, resourceName, loadPublicLocales: null);
            var localeResourceList = await localeResources.ToListAsync();

            foreach (var resourceEntry in localeResourceList)
            {
                localizationData.Add(resourceEntry.Key.ToString(), resourceEntry.Value.Value.Replace("\"", "").Replace(@"\", "-").ToString());
            }
            return localizationData;
        }


        public async Task<object> AllLanguages()
        {
            var languageData = (await _languageService.GetAllLanguagesAsync()).Select(language =>
            {
                return new
                {
                    language.Id,
                    language.Name,
                    language.LanguageCulture,
                    language.UniqueSeoCode,
                    language.FlagImageFileName,
                    language.Rtl
                };
            }).ToList();

            return languageData;
        }


        public async Task<string> SetLanguage(int languageId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);
            if (language == null || !language.Published)
                throw new ApplicationException("Language not found");
            await _workContext.SetWorkingLanguageAsync(language);
            return "Working language set successfully";
        }

        #endregion

        #region Buyer Request Reject

        public List<SelectListItem> AllRejectedReasons()
        {
            var rejectedReasons = new List<SelectListItem>
             {
                new SelectListItem { Value = "Sales Return", Text = "Sales Return" },
                new SelectListItem { Value = "Tax Compliance Issue", Text = "Tax Compliance Issue" },
                new SelectListItem { Value = "Availability Issue", Text = "Availability Issue" },
                new SelectListItem { Value = "Price Issue (Cheaper prices available in Market)", Text = "Price Issue (Cheaper prices available in Market)" },
                new SelectListItem { Value = "Credit Issue (20+)", Text = "Credit Issue (20+)" },
                new SelectListItem { Value = "Delivery Issue", Text = "Delivery Issue" },
                new SelectListItem { Value = "Customer side issue", Text = "Customer side issue" },
                new SelectListItem { Value = "No response", Text = "No response" },
                new SelectListItem { Value = "Late Response", Text = "Late Response" },
                new SelectListItem { Value = "Financial Constraints", Text = "Financial Constraints" },
                new SelectListItem { Value = "Customer Old Payment Due", Text = "Customer Old Payment Due" },
                new SelectListItem { Value = "Market Volatility", Text = "Market Volatility" },
                new SelectListItem { Value = "Wrong Quantity Entered", Text = "Wrong Quantity Entered" },
                new SelectListItem { Value = "Wrong Product Selected", Text = "Wrong Product Selected" },
                new SelectListItem { Value = "Duplicate Entry", Text = "Duplicate Entry" },
                new SelectListItem { Value = "Dummy Entry", Text = "Dummy Entry" },
                new SelectListItem { Value = "Incorrect Information", Text = "Incorrect Information" },
                new SelectListItem { Value = "Incorrect Information & Duplicate order", Text = "Incorrect Information & Duplicate order" },
                new SelectListItem { Value = "Low demand", Text = "Low demand" },
                new SelectListItem { Value = "Financial Constraint (Non GST)", Text = "Financial Constraint (Non GST)" },
                new SelectListItem { Value = "Other", Text = "Other" }
             };
            return rejectedReasons;
        }

        #endregion

        #region Business Model

        public async Task<object> AllBusinessModules(int industryId)
        {
            var modules = await BusinessModelEnum.Standard.ToSelectListAsync(false);
            return modules;
        }

        public async Task<object> AllModeOfPayment()
        {
            var paymentModes = await ModeOfPayment.Bank.ToSelectListAsync(false);
            var modeOfPaymentList = paymentModes.Select(item => new SelectListItem
            {
                Value = item.Value,
                Text = item.Text
            }).ToList();

            return modeOfPaymentList;
        }
        #endregion

        #region Price Discovery

        public async Task<object> OrderManagement_GetAllRates()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            var pictureSize = _mediaSettings.ProductThumbPictureSize;
            var rates = await _priceDiscoveryService.GetGetTodayRatesListAsync(statusId: (int)DailyRateEnum.Approve, published: true);
            var language = await _languageService.GetLanguageByIdAsync(user.LanguageId ?? 0);

            var rateDataTasks = await rates.Select(async r =>
            {
                var picture = (await _productService.GetProductPicturesByProductIdAsync(r.ProductId))?.FirstOrDefault();
                var finalRate = await _utilityService.GetFinalRate(r.Id, user, r.SupplierId, r.Rate, r.CategoryId, r.IncludeFirstMile);

                var formatPrice = await _priceFormatter.FormatPriceAsync(finalRate, true, false);
                if (r.IncludeGst)
                    formatPrice = $"{formatPrice}+";

                List<decimal> previousRates = !string.IsNullOrWhiteSpace(r.PreviousRates)
                    ? r.PreviousRates.Split(',').Select(decimal.Parse).ToList()
                    : new List<decimal>();

                string UpdatedOn = language.Rtl == true ? _utilityService.getRelativeDateTime(r.CreatedOnUtc, true) : _utilityService.getRelativeDateTime(r.CreatedOnUtc);
                var product = await _productService.GetProductByIdAsync(r.ProductId);
                var brand = await _manufacturerService.GetManufacturerByIdAsync(r.BrandId);
                var industry = await _industryService.GetIndustryByIdAsync(r.IndustryId);

                return new
                {
                    CategoryId = r.CategoryId,
                    IndustryId = r.IndustryId,
                    industry = await _localizationService.GetLocalizedAsync(industry, x => x.Name, languageId: user.LanguageId),
                    BrandId = r.BrandId,
                    brand = await _localizationService.GetLocalizedAsync(brand, x => x.Name, languageId: user.LanguageId),
                    ProductId = r.ProductId,
                    product = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId: user.LanguageId),
                    sku = r.ProductSku,
                    picture = await _pictureService.GetPictureUrlAsync(picture != null ? picture.PictureId : 0, targetSize: pictureSize),
                    formatPrice = formatPrice,
                    price = finalRate,
                    createdOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(r.CreatedOnUtc, DateTimeKind.Utc)).ToString(),
                    updateOn = UpdatedOn,
                    isFavourite = user != null && await _priceDiscoveryService.GetFavouriteRateGroupAsync(user.Id, r.ProductId, r.AttributeValueId, r.BrandId) != null,
                    ProductAttributes = await _utilityService.ParseAttributeXml(r.AttributeXml),
                    productattributesxml = r.AttributeXml,
                    attributeValueId = r.AttributeValueId,
                    attributeValue = r.AttributeValue,
                    RateMA = previousRates.ToArray(),
                    SeName = await _urlRecordService.GetSeNameAsync(product)
                };
            }).ToListAsync();
            var rateData = await Task.WhenAll(rateDataTasks);
            return rateData;
        }

        public async Task<object> OrderManagement_GetAllRatesByIndustryId(int IndustryId, int CategoryId = 0)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            var pictureSize = _mediaSettings.ProductThumbPictureSize;
            var rates = await _priceDiscoveryService.GetGetTodayRatesListAsync(statusId: (int)DailyRateEnum.Approve, published: true, industryId: IndustryId, categoryId: CategoryId);

            var rateData = await rates.SelectAwait(async rate =>
            {
                var picture = (await _productService.GetProductPicturesByProductIdAsync(rate.ProductId))?.FirstOrDefault();
                var finalRate = await _utilityService.GetFinalRate(rate.Id, user, rate.SupplierId, rate.Rate, rate.CategoryId, rate.IncludeFirstMile);

                var formatPrice = await _priceFormatter.FormatPriceAsync(finalRate, true, false);
                if (rate.IncludeGst)
                    formatPrice = $"{formatPrice}+";

                List<decimal> previousRates = !string.IsNullOrWhiteSpace(rate.PreviousRates)
                    ? rate.PreviousRates.Split(',').Select(decimal.Parse).ToList()
                    : new List<decimal>();
                var product = await _productService.GetProductByIdAsync(rate.ProductId);

                return new
                {
                    CategoryId = rate.CategoryId,
                    IndustryId = rate.IndustryId,
                    industry = rate.IndustryName,
                    BrandId = rate.BrandId,
                    brand = rate.BrandName,
                    ProductId = rate.ProductId,
                    product = rate.ProductName,
                    sku = rate.ProductSku,
                    picture = await _pictureService.GetPictureUrlAsync(picture != null ? picture.PictureId : 0, targetSize: pictureSize),
                    formatPrice = formatPrice,
                    price = finalRate,
                    createdOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(rate.CreatedOnUtc, DateTimeKind.Utc)).ToString(),
                    isFavourite = user != null && await _priceDiscoveryService.GetFavouriteRateGroupAsync(user.Id, rate.ProductId, rate.AttributeValueId, rate.BrandId) != null,
                    ProductAttributes = await _utilityService.ParseAttributeXml(rate.AttributeXml),
                    productattributesxml = rate.AttributeXml,
                    attributeValueId = rate.AttributeValueId,
                    attributeValue = rate.AttributeValue,
                    RateMA = previousRates.ToArray()
                };
            }).ToListAsync();

            return rateData;
        }
        public async Task<object> OrderManagement_GetAllFavouriteRates()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            var pictureSize = _mediaSettings.ProductThumbPictureSize;
            var rates = await _priceDiscoveryService.GetGetTodayRatesListAsync(statusId: (int)DailyRateEnum.Approve, published: true, userId: user.Id);

            var rateData = await rates.SelectAwait(async rate =>
            {
                var picture = (await _productService.GetProductPicturesByProductIdAsync(rate.ProductId))?.FirstOrDefault();
                var finalRate = await _utilityService.GetFinalRate(rate.Id, user, rate.SupplierId, rate.Rate, rate.CategoryId, rate.IncludeFirstMile);

                var formatPrice = await _priceFormatter.FormatPriceAsync(finalRate, true, false);
                if (rate.IncludeGst)
                    formatPrice = $"{formatPrice}+";

                List<decimal> previousRates = !string.IsNullOrWhiteSpace(rate.PreviousRates)
                    ? rate.PreviousRates.Split(',').Select(decimal.Parse).ToList()
                    : new List<decimal>();

                return new
                {
                    CategoryId = rate.CategoryId,
                    IndustryId = rate.IndustryId,
                    industry = rate.IndustryName,
                    BrandId = rate.BrandId,
                    brand = rate.BrandName,
                    ProductId = rate.ProductId,
                    product = rate.ProductName,
                    sku = rate.ProductSku,
                    picture = await _pictureService.GetPictureUrlAsync(picture != null ? picture.PictureId : 0, targetSize: pictureSize),
                    formatPrice = formatPrice,
                    price = finalRate,
                    createdOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(rate.CreatedOnUtc, DateTimeKind.Utc)).ToString(),
                    isFavourite = user != null && await _priceDiscoveryService.GetFavouriteRateGroupAsync(user.Id, rate.ProductId, rate.AttributeValueId, rate.BrandId) != null,
                    ProductAttributes = await _utilityService.ParseAttributeXml(rate.AttributeXml),
                    productattributesxml = rate.AttributeXml,
                    attributeValueId = rate.AttributeValueId,
                    attributeValue = rate.AttributeValue,
                    RateMA = previousRates.ToArray()
                };
            }).ToListAsync();
            return rateData;
        }
        public async Task<string> OrderManagement_AddRate(OrderManagementApiModel.RateModel model)
        {
            return await _localizationService.GetResourceAsync("Rate.Add.Successfully");
        }
        public async Task<string> OrderManagement_AddRateByGroupId(int groupId, decimal price)
        {
            return await _localizationService.GetResourceAsync("Rate.Add.Successfully");
        }
        public async Task<string> OrderManagement_FavouriteRateGroup(int productId, int attributeValueId, int brandId, bool isWishlist)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            var favouriteRateGroup = await _priceDiscoveryService.GetFavouriteRateGroupAsync(user.Id, productId, attributeValueId, brandId);

            if (isWishlist)
            {
                if (favouriteRateGroup is null)
                {
                    await _priceDiscoveryService.InsertFavouriteRateGroupAsync(new FavouriteRateGroup
                    {
                        ProductId = productId,
                        AttributeValueId = attributeValueId,
                        BrandId = brandId,
                        CustomerId = user.Id,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    return "Add to Favourite";
                }
                return "Already in Favourites";
            }
            else
            {
                if (favouriteRateGroup is not null)
                {
                    await _priceDiscoveryService.DeleteFavouriteRateGroupAsync(favouriteRateGroup);
                    return "Removed from Favourites";
                }
                return "Not in Favourites";
            }
        }

        #endregion

        #region WebSlider

        public async Task<object> AllWebSliders()
        {
            var webSliders = await _webSliderService.GetAllWebSliderAsync();

            var data = webSliders.Select(async webSlider =>
            {
                var title = await _localizationService.GetLocalizedAsync(webSlider, x => x.Title);
                var description = await _localizationService.GetLocalizedAsync(webSlider, x => x.Description);
                var picture = (await _awsS3FilesService.GetAwsS3FilesByIdAsync(webSlider.PictureId)).FileUrl;
                return new
                {
                    Title = title,
                    Description = description,
                    Link = webSlider.link,
                    Picture = picture
                };
            });

            var result = await Task.WhenAll(data);
            return result.ToList();
        }


        #endregion

        public async Task<object> AllBuyers()
        {
            var buyerRole = await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName);
            var buyers = (await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { buyerRole.Id })).ToList();

            var buyerData = await buyers.SelectAwait(async buyer =>
            {
                var buyerTypeAttributeId = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.BuyerTypeIdAttribute);

                var buyerType = (await _customerService.GetUserTypeByIdAsync(buyerTypeAttributeId != null ? Convert.ToInt32(buyerTypeAttributeId) : 0))?.Name;

                return new
                {
                    Id = buyer.Id,
                    FullName = buyer.FullName,
                    Email = buyer.Email,
                    Phone = buyer.Username,
                    BuyerType = buyerType
                };
            }).ToListAsync();

            return buyerData;
        }
        public async Task<object> AllBookers()
        {
            var bookerRole = await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BookerRoleName);
            var bookers = await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { bookerRole.Id });

            var bookerData = bookers.Select(booker => new
            {
                Id = booker.Id,
                FullName = booker.FullName,
                Email = booker.Email,
                Phone = booker.Username
            }).ToList();

            return bookerData;
        }
        public List<SelectListItem> AllRoles()
        {
            var roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Demand", Text = "Demand" },
                new SelectListItem { Value = "Supply", Text = "Supply" },
                new SelectListItem { Value = "Operations", Text = "Operations" },
                new SelectListItem { Value = "Finance", Text = "Finance" },
                new SelectListItem { Value = "BusinessHead", Text = "Business Head" },
                new SelectListItem { Value = "FinanceHead", Text = "Finance Head" },
                new SelectListItem { Value = "OpsHead", Text = "Ops Head" }
            };
            return roles;
        }
        public async Task<object> AllJaizas()
        {
            var jaizaData = await _jaizaService.GetAllJaizaAsync();

            var previouseRow = jaizaData.OrderByDescending(x => x.Id).Skip(1).Select(x => x.Id).FirstOrDefault();
            var currentDollarRate = await _orderService.GetDollarRatePKR();

            var data = jaizaData.Select(async jaiza =>
            {
                var prediction = await _localizationService.GetLocalizedAsync(jaiza, x => x.Prediction);
                var recommendation = await _localizationService.GetLocalizedAsync(jaiza, x => x.Recommendation);
                var predictionPicture = await _pictureService.GetPictureUrlAsync(jaiza.predictionPictureId);
                var recommendationPicture = await _pictureService.GetPictureUrlAsync(jaiza.RecommendationPictureId);
                var unitName = (await _measureService.GetMeasureWeightByIdAsync(jaiza.UnitId))?.Name;

                var isUp = previouseRow > 0 ? jaiza.Rate > (await _jaizaService.GetJaizaByIdAsync(previouseRow)).Rate : false;

                var dollarRateData = new
                {
                    Rate = currentDollarRate,
                    IsUp = currentDollarRate > jaiza.DollarRate
                };

                return new
                {
                    jaiza.Id,
                    Prediction = prediction,
                    PredictionPublished = jaiza.predictionPublished,
                    Recommendation = recommendation,
                    jaiza.RecommendationPublished,
                    PredictionPicture = predictionPicture,
                    RecommendationPicture = recommendationPicture,
                    Rate = await _priceFormatter.FormatPriceAsync(jaiza.Rate, true, false),
                    UnitName = unitName,
                    jaiza.RatePublished,
                    jaiza.Type,
                    IsUp = isUp,
                    DollarRate = dollarRateData
                };
            });

            var result = await Task.WhenAll(data);
            return result.OrderByDescending(a => a.Id).FirstOrDefault();
        }
        public async Task<object> AllFaqs()
        {
            var data = (await _faqService.GetAllFaqAsync()).Select(async c =>
            {
                var faq = new
                {
                    Question = await _localizationService.GetLocalizedAsync(c, x => x.Question),
                    Answer = await _localizationService.GetLocalizedAsync(c, x => x.Answer),
                };

                return faq;
            });

            var faqList = await Task.WhenAll(data);

            return faqList;
        }

        //get faq by type
        public async Task<object> AllFaqsbyType(bool isApp = true)
        {
            var data = await _faqService.GetAllFaqbyTypeAsync(isApp);
            var faqList = new List<object>();

            foreach (var faq in data)
            {
                var question = await _localizationService.GetLocalizedAsync(faq, x => x.Question);
                var answer = await _localizationService.GetLocalizedAsync(faq, x => x.Answer);

                var faqData = new
                {
                    Question = question,
                    Answer = answer
                };

                faqList.Add(faqData);
            }
            return faqList;
        }

        public async Task<string> AddAppFeedBack(AppFeedBackApiModel model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    throw new ApplicationException("Invalid user");


                if (string.IsNullOrWhiteSpace(model.Feedback))
                    throw new ApplicationException("Feedback is required");

                if (model.Rating <= 0)
                    throw new ApplicationException("Rating is required");

                var feedBack = new AppFeedBack
                {
                    FeedBack = model.Feedback,
                    Rating = model.Rating,
                    UserId = user.Id,
                    EmailSentToUser = false,
                    OwnerEmail = "",
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _appFeedBackService.InsertAppFeedBackAsync(feedBack);

                await _workflowMessageService.SendFeedbackNotificationToAdminAsync(feedBack, (await _workContext.GetWorkingLanguageAsync()).Id);
                await _workflowMessageService.SendFeedbackNotificationToCustomerAsync(feedBack, (await _workContext.GetWorkingLanguageAsync()).Id);

                await _customerActivityService.InsertActivityAsync("AddedNewfeedback",
                await _localizationService.GetResourceAsync("ActivityLog.AddFeedback"), feedBack);

                return await _localizationService.GetResourceAsync("Common.AppFeedBack.Created.Success");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<object> GetCurrentStore()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var currentStoreData = new
            {
                storeId = store.Id,
                storeName = store.Name,
                storeUrl = store.Url
            };
            return currentStoreData;
        }

        public async Task<object> AllWarehouse()
        {
            var data = new List<object>();

            var warehouses = await _shippingService.GetAllWarehousesAsync();

            foreach (var warehouse in warehouses)
            {
                var warehouseData = new
                {
                    Text = warehouse.Name,
                    Value = warehouse.Id.ToString()
                };

                data.Add(warehouseData);
            }
            return data;
        }

        public async Task<object> AllDelayedReasons()
        {
            var data = new List<object>();
            var availableDeliveryTimeReasons = await _shipmentService.GetAllDeliveryTimeReasonAsync();
            foreach (var timeReason in availableDeliveryTimeReasons)
            {
                var delayedReasonData = new
                {
                    Value = timeReason.Id.ToString(),
                    Text = timeReason.Name
                };
                data.Add(delayedReasonData);
            }
            return data;
        }

        public async Task<object> AllDeliveryCostReasons()
        {
            var data = new List<object>();
            var availableDeliveryCostReasons = await _shipmentService.GetAllDeliveryCostReasonAsync();
            foreach (var deliveryCostReason in availableDeliveryCostReasons)
            {
                var deliveryCostData = new
                {
                    Value = deliveryCostReason.Id.ToString(),
                    Text = deliveryCostReason.Name
                };

                data.Add(deliveryCostData);
            }
            return data;
        }

        public async Task<string> ClearCache()
        {
            await _staticCacheManager.ClearAsync();
            return "";
        }

        public async Task<object> GetAllBestSellingProductsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var products = await _productService.GetAllBestSellingProductsAsync(pageIndex, pageSize);
                var productList = new List<object>();
                foreach (var product in products)
                {
                    var productSeName = await _urlRecordService.GetSeNameAsync(product);
                    var picture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault();
                    productList.Add(new
                    {
                        product.Name,
                        SeName = productSeName,
                        product.Id,
                        Picture = await _pictureService.GetPictureUrlAsync(picture == null ? 0 : picture.PictureId, targetSize: _mediaSettings.ProductThumbPictureSize),
                        product.MetaTitle,
                        product.MetaDescription,
                        product.MetaKeywords,
                    });
                }
                return productList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<object> GetAllCommodityData()
        {
            var currentDate = DateTime.UtcNow.Date;
            var currentRate = await _commodityDataService.GetAllCommodityDataByDateAsync(currentDate);

            if (currentRate.Count <= 0)
            {
                return new List<object>();
            }

            var commodityData = currentRate.Select(item => new
            {
                item.Id,
                item.Name,
                item.Rate,
                item.Percentage,
            }).ToList();

            return commodityData;
        }
        public async Task<object> GetViewedProducts()
        {
            try
            {
                if (!_catalogSettings.RecentlyViewedProductsEnabled)
                    return "";

                var products = await _recentlyViewedProductsService.GetRecentlyViewedProductsAsync(_catalogSettings.RecentlyViewedProductsNumber);

                var model = new List<Object>();
                model.AddRange(await _productModelService.PrepareProductModelsAsync(products));

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<object> GetRelatedProductsByProductId(int productId)
        {

            var relatedProducts = await _productService.GetRelatedProductsByProductId1Async(productId);
            if (relatedProducts == null)
                throw new ApplicationException($"{productId} related product does not exist");

            var tasks = relatedProducts.Select(async relatedProduct =>
            {
                var productDetail = await GetProductDetailsById(relatedProduct.ProductId2, true);
                return productDetail;
            });

            var relatedProductsData = await Task.WhenAll(tasks);
            return relatedProductsData;
        }
        public async Task<object> UploadFile(UploadeFileModel model)
        {

            if (model is not null)
            {
                if (string.IsNullOrEmpty(model.FileName))
                    throw new ApplicationException("FileName is empty.");

                if (string.IsNullOrEmpty(model.FileType))
                    throw new ApplicationException("FileType is empty.");

                if (string.IsNullOrEmpty(model.FileBytes))
                    throw new ApplicationException("FileBytes is empty.");

                if (string.IsNullOrEmpty(model.EntityName))
                    throw new ApplicationException("EntityName is empty.");

                //var extention= GetFileExtension(model.FileBytes);  
                var extention = model.FileName.Split('.').Last();

                var uploadFile = await _amazonS3BuketService.UploadBase64FileAsync(model.FileBytes, _commonSettings.BucketName, $"{model.EntityName}/{(Guid.NewGuid()).ToString()}.{extention}");

                var saveAwsS3File = new AwsS3Files
                {
                    FileName = model.FileName.Trim().Replace(" ", "-"),
                    FileType = model.FileType,
                    EntityName = model.EntityName,
                    FileUrl = uploadFile,
                    CreatedOnUtc = DateTime.UtcNow,
                    IsActive = true,
                    Deleted = false
                };

                await _awsS3FilesService.InsertAwsS3FilesAsync(saveAwsS3File);

                return saveAwsS3File;
            }
            else
            {
                throw new ApplicationException("model is empty.");
            }
        }

        public virtual async Task<object> exploreAllCategories(CategoryFilterModel model)
        {
            List<int> industriesIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.IndustryIds))
                industriesIds = model.IndustryIds.Split(',').Select(int.Parse).ToList();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var categories = await _categoryService.GetCategoriesByIndustryIdsAsync(industryIds: industriesIds, isPublished: true, pageIndex: model.PageIndex, pageSize: model.PageSize);
            if (categories.Count <= 0)
                return null;

            var categorydata = Task.WhenAll(categories.Select(async item =>
            {
                return new
                {
                    Name = await _localizationService.GetLocalizedAsync(item, x => x.Name, languageId: customer.LanguageId),
                    Id = item.Id.ToString(),
                    Picture = await _pictureService.GetPictureUrlAsync(item.PictureId, targetSize: _mediaSettings.CategoryThumbPictureSize),
                    SeName = await _urlRecordService.GetSeNameAsync(item),
                    item.MetaKeywords,
                    MetaDescription = await _localizationService.GetLocalizedAsync(item, x => x.MetaDescription, languageId: customer.LanguageId),
                    item.MetaTitle
                };
            }));

            return new
            {
                categories.PageIndex,
                categories.PageSize,
                categories.TotalCount,
                categories.TotalPages,
                categories.HasPreviousPage,
                categories.HasNextPage,
                Categories = categorydata.Result
            };
        }

        #region ContactUs

        public async Task<object> ContactUs(ContactUsModel model)
        {

            if (model is not null)
            {
                if (string.IsNullOrEmpty(model.FullName))
                    throw new ApplicationException("FullName is empty.");

                if (string.IsNullOrEmpty(model.ContactNo))
                    throw new ApplicationException("ContactNo is empty.");

                if (string.IsNullOrEmpty(model.Email))
                    throw new ApplicationException("Email is empty.");

                if (string.IsNullOrEmpty(model.CompanyDomain))
                    throw new ApplicationException("CompanyDomain is empty.");

                var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);

                await _workflowMessageService.SendContactUsMessageAsync((await _workContext.GetWorkingLanguageAsync()).Id,
                    model.Email.Trim(), model.FullName, subject, body, model.ContactNo, model.CompanyDomain, model.Country);

                model.SuccessfullySent = true;
                model.Result = await _localizationService.GetResourceAsync("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.ContactUs",
                    await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ContactUs"));

                return model.Result;

            }
            else
            {
                throw new ApplicationException("model is empty.");
            }
        }

        #endregion
    }
}

