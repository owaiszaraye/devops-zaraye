using Zaraye.Core.Infrastructure;
using Zaraye.Services.Buyer;
using Zaraye.Services.Common;
using Zaraye.Services.MarketPlace;
using Zaraye.Services.Notification;
using Zaraye.Services.OrderManagement;
using Zaraye.Services.Requests;
using Zaraye.Services.Security;
using Zaraye.Services.ShoppingCart;
using Zaraye.Services.Utility;
using Zaraye.Services.Logging;
using Zaraye.Services.Tax;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Zaraye.Core;
using Zaraye.Core.Caching;
using Zaraye.Core.Configuration;
using Zaraye.Core.Events;
using Zaraye.Data;
using Zaraye.Framework.Infrastructure.Extensions;
using Zaraye.Framework.Mvc.Routing;
using Zaraye.Framework.Websocket;
using Zaraye.Services.Affiliates;
using Zaraye.Services.Authentication;
using Zaraye.Services.Blogs;
using Zaraye.Services.Caching;
using Zaraye.Services.Catalog;
using Zaraye.Services.Configuration;
using Zaraye.Services.CustomerLedgers;
using Zaraye.Services.Customers;
using Zaraye.Services.CustomerTestimonial;
using Zaraye.Services.Directory;
using Zaraye.Services.Discounts;
using Zaraye.Services.Events;
using Zaraye.Services.ExportImport;
using Zaraye.Services.Forums;
using Zaraye.Services.Gdpr;
using Zaraye.Services.Helpers;
using Zaraye.Services.Html;
using Zaraye.Services.Inventory;
using Zaraye.Services.Localization;
using Zaraye.Services.MarketPlaceExchangerate;
using Zaraye.Services.Media;
using Zaraye.Services.Media.RoxyFileman;
using Zaraye.Services.Messages;
using Zaraye.Services.News;
using Zaraye.Services.Orders;
using Zaraye.Services.Payments;
using Zaraye.Services.PriceDiscovery;
using Zaraye.Services.ScheduleTasks;
using Zaraye.Services.Seo;
using Zaraye.Services.Shipping;
using Zaraye.Services.Shipping.Date;
using Zaraye.Services.Stores;
using Zaraye.Services.Topics;
using Zaraye.Framework;
using ILogger = Zaraye.Services.Logging.ILogger;

namespace Zaraye.Infrastructure
{
    /// <summary>
    /// Represents the registering services on application startup
    /// </summary>
    public partial class ZarayeStartup : IZarayeStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //add options feature
            services.AddOptions();

            //add distributed cache
            services.AddDistributedCache();

            //add HTTP sesion state feature
            services.AddHttpSession();

            //add default HTTP clients
            services.AddZarayeHttpClients();

            //add anti-forgery
            services.AddAntiForgery();

            //add data protection
            services.AddZarayeDataProtection();

            //add authentication
            services.AddZarayeAuthentication();
            //add and configure MVC feature
            services.AddZarayeMvc();
            services.AddZarayeMiniProfiler();
            services.AddWebEncoders();
            //file provider
            services.AddScoped<IZarayeFileProvider, ZarayeFileProvider>();

            //web helper
            services.AddScoped<IWebHelper, WebHelper>();

            //user agent helper
            services.AddScoped<IUserAgentHelper, UserAgentHelper>();


            //static cache manager
            var appSettings = Singleton<AppSettings>.Instance;
            var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();
            if (distributedCacheConfig.Enabled)
            {
                switch (distributedCacheConfig.DistributedCacheType)
                {
                    case DistributedCacheType.Memory:
                        services.AddScoped<ILocker, MemoryDistributedCacheManager>();
                        services.AddScoped<IStaticCacheManager, MemoryDistributedCacheManager>();
                        break;
                    case DistributedCacheType.SqlServer:
                        services.AddScoped<ILocker, MsSqlServerCacheManager>();
                        services.AddScoped<IStaticCacheManager, MsSqlServerCacheManager>();
                        break;
                    case DistributedCacheType.Redis:
                        services.AddScoped<ILocker, RedisCacheManager>();
                        services.AddScoped<IStaticCacheManager, RedisCacheManager>();
                        break;
                }
            }
            else
            {
                services.AddSingleton<ILocker, MemoryCacheManager>();
                services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            }


            //work context
            services.AddScoped<IWorkContext, WebWorkContext>();

            //store context
            services.AddScoped<IStoreContext, WebStoreContext>();

            //services
            services.AddScoped<IBackInStockSubscriptionService, BackInStockSubscriptionService>();
            services.AddScoped<IIndustryService, IndustryService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IBidRequestTrackerService, BidRequestTrackerService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IRecentlyViewedProductsService, RecentlyViewedProductsService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IPriceFormatter, PriceFormatter>();
            services.AddScoped<IQuantityFormatter, QuantityFormatter>();
            services.AddScoped<IProductAttributeFormatter, ProductAttributeFormatter>();
            services.AddScoped<IProductAttributeParser, ProductAttributeParser>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICopyProductService, CopyProductService>();
            services.AddScoped<ISpecificationAttributeService, SpecificationAttributeService>();
            services.AddScoped<IProductTemplateService, ProductTemplateService>();
            services.AddScoped<ICategoryTemplateService, CategoryTemplateService>();
            services.AddScoped<IManufacturerTemplateService, ManufacturerTemplateService>();
            services.AddScoped<ITopicTemplateService, TopicTemplateService>();
            services.AddScoped<IProductTagService, ProductTagService>();
            services.AddScoped<IAddressAttributeFormatter, AddressAttributeFormatter>();
            services.AddScoped<IAddressAttributeParser, AddressAttributeParser>();
            services.AddScoped<IAddressAttributeService, AddressAttributeService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAffiliateService, AffiliateService>();
            services.AddScoped<ISearchTermService, SearchTermService>();
            services.AddScoped<IGenericAttributeService, GenericAttributeService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<ICustomerAttributeFormatter, CustomerAttributeFormatter>();
            services.AddScoped<ICustomerAttributeParser, CustomerAttributeParser>();
            services.AddScoped<ICustomerAttributeService, CustomerAttributeService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();
            services.AddScoped<ICustomerReportService, CustomerReportService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAclService, AclService>();
            services.AddScoped<IPriceCalculationService, PriceCalculationService>();
            services.AddScoped<IGeoLookupService, GeoLookupService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IMeasureService, MeasureService>();
            services.AddScoped<IStateProvinceService, StateProvinceService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStoreMappingService, StoreMappingService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IDownloadService, DownloadService>();
            services.AddScoped<IMessageTemplateService, MessageTemplateService>();
            services.AddScoped<IQueuedEmailService, QueuedEmailService>();
            services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
            services.AddScoped<IMessageTokenProvider, MessageTokenProvider>();
            services.AddScoped<ITokenizer, Tokenizer>();
            services.AddScoped<ISmtpBuilder, SmtpBuilder>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderReportService, OrderReportService>();
            services.AddScoped<IOrderProcessingService, OrderProcessingService>();
            services.AddScoped<IOrderTotalCalculationService, OrderTotalCalculationService>();
            services.AddScoped<IReturnRequestService, ReturnRequestService>();
            services.AddScoped<IRewardPointService, RewardPointService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<ICustomNumberFormatter, CustomNumberFormatter>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAuthenticationService, CookieAuthenticationService>();
            services.AddScoped<IUrlRecordService, UrlRecordService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IShippingService, ShippingService>();
            services.AddScoped<IDateRangeService, DateRangeService>();
            services.AddScoped<ILogger, DefaultLogger>();
            services.AddScoped<ICustomerActivityService, CustomerActivityService>();
            services.AddScoped<IForumService, ForumService>();
            services.AddScoped<IGdprService, GdprService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IDateTimeHelper, DateTimeHelper>();
            //services.AddScoped<INopHtmlHelper, NopHtmlHelper>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            services.AddScoped<IExportManager, ExportManager>();
            services.AddScoped<IImportManager, ImportManager>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddSingleton<IRoutePublisher, RoutePublisher>();
            services.AddScoped<IReviewTypeService, ReviewTypeService>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IBBCodeHelper, BBCodeHelper>();
            services.AddScoped<IHtmlFormatter, HtmlFormatter>();
            services.AddScoped<IVideoService, VideoService>();
            // services.AddScoped<IZarayeUrlHelper, ZarayeUrlHelper>();
            services.AddScoped<IAppVersionService, AppVersionService>();
            services.AddScoped<IFaqService, FaqService>();
            services.AddScoped<IJaizaService, JaizaService>();
            services.AddScoped<IAppFeedBackService, AppFeedBackService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<IOnlineLeadService, OnlineLeadService>();
            services.AddScoped<IPriceDiscoveryService, PriceDiscoveryService>();
            services.AddScoped<IQuotationService, QuotationService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<ICustomerLedgerService, CustomerLedgerService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAmazonS3BuketService, AmazonS3BuketService>();
            services.AddScoped<IAwsS3FilesService, AwsS3FilesService>();
            services.AddScoped<IWebSliderService, WebSliderService>();
            services.AddScoped<ICustomerTestimonialService, CustomerTestimonialService>();
            services.AddScoped<IEmployeeInsightsService, EmployeeInsightsService>();
            services.AddScoped<IMarketPlaceExchangerateService, MarketPlaceExchangerateService>();
            services.AddScoped<ICommodityDataService, CommodityDataService>();
            services.AddScoped<ITaxService, TaxService>();

            services.AddSingleton<WebSocketServer>();


            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //register all settings
            var typeFinder = Singleton<ITypeFinder>.Instance;

            var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
            foreach (var setting in settings)
            {
                services.AddScoped(setting, serviceProvider =>
                {
                    var storeId = DataSettingsManager.IsDatabaseInstalled()
                        ? serviceProvider.GetRequiredService<IStoreContext>().GetCurrentStore()?.Id ?? 0
                        : 0;

                    return serviceProvider.GetRequiredService<ISettingService>().LoadSettingAsync(setting, storeId).Result;
                });
            }

            //picture service
            if (appSettings.Get<AzureBlobConfig>().Enabled)
                services.AddScoped<IPictureService, AzurePictureService>();
            else
                services.AddScoped<IPictureService, PictureService>();

            //roxy file manager
            services.AddScoped<IRoxyFilemanService, RoxyFilemanService>();
            services.AddScoped<IRoxyFilemanFileProvider, RoxyFilemanFileProvider>();

            //schedule tasks
            services.AddSingleton<ITaskScheduler, Services.ScheduleTasks.TaskScheduler>();
            services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
                foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(IConsumer<>)))
                    services.AddScoped(findInterface, consumer);

            //common factories
            //services.AddScoped<IAclSupportedModelFactory, AclSupportedModelFactory>();
            //services.AddScoped<IDiscountSupportedModelFactory, DiscountSupportedModelFactory>();
            //services.AddScoped<ILocalizedModelFactory, LocalizedModelFactory>();
            //services.AddScoped<IStoreMappingSupportedModelFactory, StoreMappingSupportedModelFactory>();

            //factories
            services.AddScoped<IMarketPlaceService, MarketPlaceService>();
            services.AddScoped<IRequestsService, RequestsService>();
            services.AddScoped<IBuyerService, BuyerService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<INotificationControllerService, NotificationControllerService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IOrderManagermentService, OrderManagermentService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IAppLoggerService, AppLoggerService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<IProductModelService, ProductModelService>();
            //services.AddScoped(ServiceProvider => serviceProvider.GetService<ILoggerFactory>().CreateLogger<AppLogger>());

        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //check whether requested page is keep alive page
            application.UseKeepAlive();

            //use HTTP session
            application.UseSession();

            //use request localization
            application.UseZarayeRequestLocalization();

            application.UseWebSocketServerForZaraye();
            //exception handling
            application.UseZarayeExceptionHandler();

            //handle 400 errors (bad request)
            application.UseBadRequestResult();
            application.UseZarayeAuthentication();
            //Add the Authorization middleware
            application.UseAuthorization();
            //use MiniProfiler must come before UseNopEndpoints
            application.UseMiniProfiler();

            //Add the RoutingMiddleware
            application.UseRouting();
            application.UseZarayeEndpoints();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 2002;
    }
}
