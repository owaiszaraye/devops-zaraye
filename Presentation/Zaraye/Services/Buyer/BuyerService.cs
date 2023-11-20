using Microsoft.AspNetCore.Mvc;
using Zaraye.Controllers;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Models.Api.V4.Buyer;
using Zaraye.Services.Authentication;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.CustomerLedgers;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.Helpers;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Media;
using Zaraye.Services.News;
using Zaraye.Services.Orders;
using Zaraye.Services.Payments;
using Zaraye.Services.Shipping;
using Zaraye.Services.Utility;

namespace Zaraye.Services.Buyer
{
    public class BuyerService : BaseApiController, IBuyerService
    {
        #region Fields

        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IIndustryService _industryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly IMeasureService _measureService;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IManufacturerService _brandService;
        private readonly INewsService _newsService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly HttpClient _httpClient;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IRequestService _requestService;
        private readonly IQuotationService _quotationService;
        private readonly IPaymentService _paymentService;
        private readonly IAddressService _addressService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerLedgerService _customerLedgerService;
        private readonly ICustomerLedgerService _ledgerService;
        private readonly IAmazonS3BuketService _amazonS3BuketService;
        private readonly CommonSettings _commonSettings;
        private readonly ICountryService _countryService;
        private readonly IUtilityService _utilityService;

        #endregion

        #region Ctor

        public BuyerService(ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICustomerActivityService customerActivityService,
            IAuthenticationService authenticationService,
            IProductService productService,
            ICategoryService categoryService,
            IIndustryService industryService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IShoppingCartService shoppingCartService,
            IProductAttributeService productAttributeService,
            IDownloadService downloadService,
            IMeasureService measureService,
            IOrderService orderService,
            IShipmentService shipmentService,
            ICustomNumberFormatter customNumberFormatter,
            IManufacturerService brandService,
            INewsService newsService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            HttpClient httpClient,
            IPriceCalculationService priceCalculationService,
            IRequestService requestService,
            IQuotationService quotationService,
            IPaymentService paymentService,
            IAddressService addressService,
            IOrderProcessingService orderProcessingService,
            ICustomerLedgerService customerLedgerService,
            ICustomerLedgerService ledgerService,
            IAmazonS3BuketService amazonS3BuketService,
            CommonSettings commonSettings,
            ICountryService countryService, IUtilityService utilityService
            )
        {
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _workContext = workContext;
            _customerActivityService = customerActivityService;
            _authenticationService = authenticationService;
            _productService = productService;
            _categoryService = categoryService;
            _industryService = industryService;
            _dateTimeHelper = dateTimeHelper;
            _priceFormatter = priceFormatter;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
            _productAttributeService = productAttributeService;
            _downloadService = downloadService;
            _measureService = measureService;
            _orderService = orderService;
            _shipmentService = shipmentService;
            _customNumberFormatter = customNumberFormatter;
            _brandService = brandService;
            _newsService = newsService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _httpClient = httpClient;
            _priceCalculationService = priceCalculationService;
            _requestService = requestService;
            _quotationService = quotationService;
            _paymentService = paymentService;
            _addressService = addressService;
            _orderProcessingService = orderProcessingService;
            _customerLedgerService = customerLedgerService;
            _ledgerService = ledgerService;
            _amazonS3BuketService = amazonS3BuketService;
            _commonSettings = commonSettings;
            _countryService = countryService;
            _utilityService = utilityService;
        }

        #endregion

        #region Methods

        #region Dashboard
        public virtual async Task<object> GetDashboardData()
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");


            var getLastRequest = (await _requestService.GetAllRequestAsync(buyerId: buyer.Id)).FirstOrDefault();
            var last = new
            {
                product = getLastRequest is not null ? await _localizationService.GetLocalizedAsync((await _productService.GetProductByIdAsync(getLastRequest.ProductId)), x => x.Name) : null,
                deliverydate = getLastRequest is not null ? (await _dateTimeHelper.ConvertToUserTimeAsync(getLastRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM yy") : null,
                quantity = getLastRequest is not null ? getLastRequest.Quantity : 0,
                totalquotations = 0 //getLastRequest is not null ? await _quotationService.GetAllQuotationAsync(sbIds: new List<int> { (int)QuotationStatus.Verified }, buyerRequestId: getLastRequest.Id) : 0,
            };

            var data = new
            {
                NumberOfOrders = (await _orderService.SearchOrdersAsync(getOnlyTotalCount: true)).TotalCount,
                NumberOfPartners = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] {
                    (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id,
                    (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id }, getOnlyTotalCount: true)).TotalCount,
                BagsDelivered = _orderService.GetTotalOrderItemsCountAsync(),
                sliders = (await _pictureService.GetAllAppSlidersAsync()).Select(async s =>
                {
                    return new
                    {
                        ImageUrl = (await _pictureService.GetPictureUrlAsync((await _pictureService.GetPictureByIdAsync(s.PictureId)))).Url ?? "",
                        Title = s.Title,
                        Description = s.Description,
                        DisplayOrder = s.DisplayOrder
                    };
                }).Select(t => t.Result).ToList(),
                News = (await _newsService.GetAllNewsAsync(showAppNews: true)).Select(async s =>
                {
                    return new
                    {
                        Title = s.Title,
                        Description = s.Short,
                        CreatedOnUtc = s.CreatedOnUtc,
                        CreatedOnUtcFormatted = s.CreatedOnUtc.ToString("dd MMM yy"),
                        PictureUrl = await _pictureService.GetPictureUrlAsync(s.PictureId, 300)
                    };
                }).Select(t => t.Result).ToList(),
                lastRequest = getLastRequest is not null ? last : null

            };
            return data;
        }
        #endregion

        public virtual async Task<object> GetInfo()
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            var buyerTypeAttributeId = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.BuyerTypeIdAttribute);
            var userType = await _customerService.GetUserTypeByIdAsync(buyer.UserTypeId > 0 ? Convert.ToInt32(buyer.UserTypeId) : 0);
            var agent = await _customerService.GetCustomerByIdAsync(buyer.SupportAgentId);
            var store = _storeContext.GetCurrentStore();
            var pinLocationAttribute = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, storeId: store.Id);
            var pinLocation = !string.IsNullOrWhiteSpace(pinLocationAttribute) ? pinLocationAttribute.Split(",") : new string[] { };
            var checkIndustryAllowCredit = buyer.IndustryId > 0 ? (await _industryService.GetIndustryByIdAsync(buyer.IndustryId)).IsAllowCredit : false;
            var country = await _countryService.GetCountryByIdAsync(buyer.CountryId);
            var city = await _stateProvinceService.GetStateProvinceByIdAsync(buyer.StateProvinceId);

            var data = new
            {
                id = buyer.Id,
                isAppActive = buyer.IsAppActive,
                email = buyer.Email,
                firstName = buyer.FirstName,
                lastName = buyer.LastName,
                companyName = buyer.Company,
                address = buyer.StreetAddress,
                address2 = buyer.StreetAddress2,
                countryId = buyer.CountryId,
                countryName = country != null ? country.Name : "",
                stateId = buyer.StateProvinceId,
                areaId = buyer.AreaId,
                city = buyer.City,
                cityName = city != null ? city.Name : "",
                phone = buyer.Username,
                industryId = buyer.IndustryId,
                buyerType = userType != null ? userType.Name : null,
                buyerTypeId = buyer.UserTypeId,
                buyerPinLocation = pinLocation.Any() ?
                  new BuyerInfoApiModel.BuyerPinLocationApiModel()
                  {
                      Latitude = pinLocation[0],
                      Longitude = pinLocation[1],
                      Location = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute, storeId: store.Id),
                  } :
                  new BuyerInfoApiModel.BuyerPinLocationApiModel(),
                AgentInfo = agent is not null ? new
                {
                    id = agent.Id,
                    email = agent.Email,
                    phonenumber = agent.Username,
                    fullname = agent.FullName
                } : null,
                Progress = await _utilityService.GetBuyerProgress(buyer),
                isCompany = false,
                isEmployee = false,
                isAllowCredit = checkIndustryAllowCredit,


            };
            return data;
        }
        public virtual async Task<object> UpdateInfo(BuyerInfoApiModel model)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            if (!CommonHelper.IsValidEmail(model.Email))
                throw new ApplicationException(await _localizationService.GetResourceAsync("Common.WrongEmail"));

            //email
            var email = model.Email.Trim();
            if (!buyer.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
            {
                //change email
                var requireValidation = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                await _customerRegistrationService.SetEmailAsync(buyer, email, requireValidation);

                //do not authenticate users in impersonation mode
                if (_workContext.OriginalCustomerIfImpersonated == null)
                {
                    //re-authenticate (if usernames are disabled)
                    if (!requireValidation)
                        await _authenticationService.SignInAsync(buyer, true);
                }
            }
            //form fields
            buyer.FirstName = model.FirstName;
            buyer.LastName = model.LastName;
            buyer.Company = model.Company;
            //buyer.StreetAddress = model.Address;
            //buyer.StreetAddress2 = model.Address2;
            buyer.IndustryId = model.IndustryId;
            //buyer.CountryId = model.CountryId;
            //buyer.StateProvinceId = model.StateId;
            //buyer.AreaId = model.AreaId;
            buyer.Phone = model.Phone;
            buyer.UserTypeId = model.BuyerTypeId;

            await _customerService.UpdateCustomerAsync(buyer);

            return await _localizationService.GetResourceAsync("Buyer.Info.Edit.Success");
        }
        public virtual async Task<string> UpdateAddress(BuyerInfoAddressApiModel model)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            var store = _storeContext.GetCurrentStore();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            //form fields
            buyer.CountryId = model.CountryId;
            buyer.StateProvinceId = model.StateId;
            buyer.AreaId = model.AreaId;
            buyer.StreetAddress = model.Address;
            buyer.StreetAddress2 = model.Address2;


            if (model.BuyerPinLocation != null)
            {
                await _genericAttributeService.SaveAttributeAsync(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, $"{model.BuyerPinLocation.Latitude},{model.BuyerPinLocation.Longitude}", storeId: store.Id);
                await _genericAttributeService.SaveAttributeAsync(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute, model.BuyerPinLocation.Location, storeId: store.Id);
            }

            await _customerService.UpdateCustomerAsync(buyer);
            return await _localizationService.GetResourceAsync("Buyer.Info.Edit.Success");
        }

        #region Buyer Request
        public virtual async Task<IList<object>> GetBuyerRequestHistory(bool active = false)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            List<object> list = new List<object>();

            //if (statusIds is null)
            //    statusIds = Enum.GetValues(typeof(BuyerRequestStatus)).Cast<int>().Where(s => s != (int)BuyerRequestStatus.Expired).ToList();

            var buyerRequests = await _requestService.GetAllRequestAsync(/*bsIds: statusIds,*/ buyerId: buyer.Id, getOnlyActiveRequestsForApi: active);
            foreach (var buyerRequest in buyerRequests)
            {
                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null)
                    continue;

                //var order = await _orderService.GetOrderByRequestIdAsync(buyerRequest.Id);
                //if (order is null)
                //    continue;

                //var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(buyerRequest.ProductId, isAppPublished: true)).FirstOrDefault();
                //if (productCategory is null)
                //    continue;

                var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                if (category is null)
                    continue;

                var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
                if (industry is null)
                    continue;

                var pictureId = (await _pictureService.GetPicturesByProductIdAsync(product.Id)).FirstOrDefault()?.Id;

                var totalDays = 0;
                var totalHours = 0;
                var minutes = 0;
                var percentage = 0;

                if (buyerRequest.ExpiryDate.HasValue)
                {
                    var (_totalDays, _totalHours, _minutes, _percentage) = await _utilityService.GetPercentageAndTimeRemaining(buyerRequest.ExpiryDate.Value, buyerRequest.CreatedOnUtc);
                    totalDays = _totalDays;
                    totalHours = _totalHours;
                    minutes = _minutes;
                    percentage = _percentage;
                }

                list.Add(new
                {
                    Id = buyerRequest.Id,
                    CustomRequestNumber = buyerRequest.CustomRequestNumber,
                    BuyerId = buyerRequest.BuyerId,
                    BuyerName = await _customerService.GetCustomerFullNameAsync(buyerRequest.BuyerId),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    IndustryId = industry.Id,
                    IndustryName = industry.Name,
                    PictureUrl = await _pictureService.GetPictureUrlAsync(pictureId.HasValue ? pictureId.Value : 0, 140, showDefaultPicture: true),
                    Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "",
                    BrandId = buyerRequest.BrandId,
                    Qty = buyerRequest.Quantity,
                    QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                    DeliveryAddress = buyerRequest.DeliveryAddress,
                    DeliveryDate = await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc),
                    DeliveryDateFormatted = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM yy"),
                    CreatedOn = buyerRequest.CreatedOnUtc,
                    CreatedOnFormatted = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm - dd MMM yy"),
                    TotalQuotations = await _requestService.GetQuotationCountByRequestIdAsync(buyerRequest.Id),
                    StatusId = buyerRequest.RequestStatusId,
                    Status = await _localizationService.GetLocalizedEnumAsync(buyerRequest.RequestStatus),
                    PaymentDuration = buyerRequest.PaymentDuration,
                    //QuotationId = buyerRequest.BuyerRequestStatus == BuyerRequestStatus.QuotationChosen ? buyerRequest.SellerBidId : 0,
                    //OrderId = order.Id,
                    //CustomOrderNumber = order.CustomOrderNumber,
                    TimeRemaining = totalHours >= 25 ? (totalDays) + " days remaining" : totalHours.ToString("00") + ":" + minutes.ToString("00") + " minutes remaining",
                    Percentage = percentage,
                    //ProductAttributes = await _buyerModelFactory.PrepareProductAttributeModelsAsync(product, buyerRequest)
                    ProductAttributes = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                });
            }

            return list;
        }
        public virtual async Task<object> GetBuyerRequest(int requestId)
        {
            if (requestId <= 0)
                throw new ApplicationException("Buyer request id is required");

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) /*|| !await _customerService.IsBuyerAsync(buyer)*/)
                throw new ApplicationException("Buyer not found");

            var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
            if (buyerRequest is null /*|| buyerRequest.BuyerId != buyer.Id*/)
                throw new ApplicationException("Buyer request not found");

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null || product.Id != buyerRequest.ProductId)
                throw new ApplicationException("Buyer request not found");

            var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(buyerRequest.ProductId, isAppPublished: true)).FirstOrDefault();
            if (productCategory is null || product.Id != productCategory.ProductId)
                throw new ApplicationException("Buyer request not found");

            var category = await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId);
            if (category is null)
                throw new ApplicationException("Buyer request not found");

            var industry = await _categoryService.GetCategoryByIdAsync(category.ParentCategoryId);
            if (industry is null)
                throw new ApplicationException("Buyer request not found");
            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

            var data = new
            {
                Id = buyerRequest.Id,
                CustomRequestNumber = buyerRequest.CustomRequestNumber,
                BuyerId = buyerRequest.BuyerId,
                ProductId = product.Id,
                ProductName = product.Name,
                CategoryId = category.Id,
                CategoryName = category.Name,
                IndustryId = industry.Id,
                IndustryName = industry.Name,
                Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : await _localizationService.GetResourceAsync("Admin.Brand.Option.Other") + " " + "-",
                Qty = buyerRequest.Quantity,
                QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                DeliveryAddress = buyerRequest.DeliveryAddress,
                DeliveryAddress2 = buyerRequest.DeliveryAddress2,
                CountryId = buyerRequest.CountryId,
                CityId = buyerRequest.CityId,
                AreaId = buyerRequest.AreaId,
                DeliveryDate = buyerRequest.DeliveryDate,
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.CreatedOnUtc, DateTimeKind.Utc),
                //ProductAttributes = await _requestModelFactory.PrepareProductAttributeModelsAsync(product, buyerRequest),
                StatusId = buyerRequest.RequestStatusId,
                Status = await _localizationService.GetLocalizedEnumAsync(buyerRequest.RequestStatus),
                PaymentDuration = buyerRequest.PaymentDuration
            };
            return data;
        }
        public virtual async Task<object> AddBuyerRequest(BuyerRequestApiModel model)
        {
            try
            {
                var buyer = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                    throw new ApplicationException("Buyer not found");

                if (model.IndustryId <= 0)
                    throw new ApplicationException("Industry Id is required");

                if (model.CategoryId <= 0)
                    throw new ApplicationException("Category Id is required");

                var category = await _categoryService.GetCategoryByIdAsync(model.CategoryId);
                if (category is null)
                    throw new ApplicationException("Category not found");

                if (model.ProductId <= 0)
                    throw new ApplicationException("Product Id is required");

                var product = await _productService.GetProductByIdAsync(model.ProductId);
                if (product is null)
                    throw new ApplicationException("Product not found");

                if (model.BrandId <= 0 && string.IsNullOrWhiteSpace(model.OtherBrand))
                    throw new ApplicationException("Brand Id is required");

                var warnings = new List<string>();
                var attributesXml = await _utilityService.ConvertToXmlAsync(model.AttributesData, product.Id);
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, product, 1, attributesXml));
                if (warnings.Any())
                    return warnings.ToArray();

                if (model.Quantity <= 0)
                    throw new ApplicationException("Quantity is required");
                int brandId = 0;
                var brand = await _brandService.GetManufacturerByNameAsync(model.OtherBrand);
                if (brand is null)
                {
                    var newBrand = new Manufacturer();
                    newBrand.Name = model.OtherBrand;
                    newBrand.IndustryId = model.IndustryId;
                    newBrand.AppPublished = true;
                    newBrand.Published = true;
                    await _brandService.InsertManufacturerAsync(newBrand);
                    brandId = newBrand.Id;
                }
                else
                    brandId = brand.Id;

                var buyerRequest = new Request
                {
                    BuyerId = buyer.Id,
                    IndustryId = model.IndustryId,
                    CategoryId = model.CategoryId,
                    ProductId = model.ProductId,
                    ProductAttributeXml = attributesXml,
                    RequestStatus = RequestStatus.Pending,
                    //DeliveryDate = (DateTime)_dateTimeHelper.ConvertToUtcTime(model.DeliveryDate, await _dateTimeHelper.GetCurrentTimeZoneAsync()),
                    Quantity = model.Quantity,
                    BrandId = brandId,
                    //OtherBrand = model.OtherBrand,
                    //CityId = model.CityId,
                    //AreaId = model.AreaId,
                    //PaymentDuration = model.PaymentDuration,
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(category.ExpiryDays > 0 ? category.ExpiryDays : 2),
                    IdealBuyingPrice = model.IdealBuyingPrice,
                    RequestTypeId = (int)RequestTypeEnum.External,
                    Source = "Consumer",
                    BusinessModelId = (int)BusinessModelEnum.Standard,
                };
                await _requestService.InsertRequestAsync(buyerRequest);

                //generate and set custom request number
                buyerRequest.CustomRequestNumber = _customNumberFormatter.GenerateRequestCustomNumber(buyerRequest);
                await _requestService.UpdateRequestAsync(buyerRequest);

                return await _localizationService.GetResourceAsync("Buyer.Request.Created.Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public virtual async Task<object> EditBuyerRequest(int requestId, [FromBody] BuyerRequestApiModel model)
        {
            try
            {
                if (requestId <= 0)
                    throw new ApplicationException("Buyer request id is required");

                var buyer = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                    throw new ApplicationException("Buyer not found");

                var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
                if (buyerRequest is null /*|| buyerRequest.BuyerId != buyer.Id*/)
                    throw new ApplicationException("Buyer request not found");

                if (buyerRequest.RequestStatus == RequestStatus.Approved)
                    throw new ApplicationException("Buyer request not found");

                if (model.IndustryId <= 0)
                    throw new ApplicationException("Industry is required");

                if (model.CategoryId <= 0)
                    throw new ApplicationException("Category is required");

                if (model.ProductId <= 0)
                    throw new ApplicationException("Product is required");

                var product = await _productService.GetProductByIdAsync(model.ProductId);
                if (product is null)
                    throw new ApplicationException("Product not found");

                var warnings = new List<string>();
                var attributesXml = await _utilityService.ConvertToXmlAsync(model.AttributesData, product.Id);
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, product, 1, attributesXml));
                if (warnings.Any())
                    return warnings.ToArray();

                if (model.Quantity <= 0)
                    throw new ApplicationException("Quantity is required");

                buyerRequest.BuyerId = (await _workContext.GetCurrentCustomerAsync()).Id;
                buyerRequest.CategoryId = model.CategoryId;
                buyerRequest.ProductId = model.ProductId;
                buyerRequest.ProductAttributeXml = attributesXml;
                buyerRequest.RequestStatus = RequestStatus.Pending;
                buyerRequest.BrandId = model.BrandId;
                buyerRequest.Quantity = model.Quantity;
                buyerRequest.DeliveryDate = (DateTime)_dateTimeHelper.ConvertToUtcTime(model.DeliveryDate, await _dateTimeHelper.GetCurrentTimeZoneAsync());
                buyerRequest.PaymentDuration = model.PaymentDuration;
                buyerRequest.CityId = model.CityId;
                buyerRequest.AreaId = model.AreaId;

                await _requestService.UpdateRequestAsync(buyerRequest);

                await _customerActivityService.InsertActivityAsync("EditBuyerRequest",
                   await _localizationService.GetResourceAsync("ActivityLog.EditBuyerRequestFromConsumerApp"), buyerRequest);

                return await _localizationService.GetResourceAsync("Buyer.Request.Updated.Success");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region Buyer Request Quotation History
        public virtual async Task<IList<object>> BuyerRequestQuotationHistory(int requestId, List<int> statusIds = null)
        {
            if (requestId <= 0)
                throw new ApplicationException("Buyer request id is required");

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
            if (buyerRequest is null /*|| buyerRequest.BuyerId != buyer.Id*/)
                throw new ApplicationException("Buyer request not found");

            List<object> list = new List<object>();

            if (statusIds is null)
                statusIds = Enum.GetValues(typeof(QuotationStatus)).Cast<int>().ToList();

            var olderBidPrice = 0m;

            var quotations = (await _requestService.GetQuotationsByRequestIdAsync(buyerRequest.Id)).ToList();
            if (quotations.Any())
                olderBidPrice = quotations.FirstOrDefault().QuotationPrice;

            foreach (var quotation in quotations)
            {
                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null)
                    continue;

                var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                if (category is null)
                    continue;

                (int totalDays, int totalHours, int minutes, int percentage) = await _utilityService.GetPercentageAndTimeRemaining(quotation.PriceValidity, quotation.CreatedOnUtc);
                list.Add(new
                {
                    Id = quotation.Id,
                    CustomQuotationNumber = quotation.CustomQuotationNumber,
                    CustomRequestNumber = buyerRequest.CustomRequestNumber,
                    ProductName = product.Name,
                    CategoryName = category.Name,
                    Brand = quotation.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(quotation.BrandId)), x => x.Name) : "",
                    BrandId = quotation.BrandId,
                    Quantity = quotation.Quantity,
                    QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                    DeliveryDate = buyerRequest.DeliveryDate.ToString("dd MMM yy"),
                    //UnitPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice / quotation.Quantity),
                    QuotationPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice),
                    DateValidity = (await _dateTimeHelper.ConvertToUserTimeAsync(quotation.PriceValidity, DateTimeKind.Utc)).ToString("HH:mm - dd MMM yy") /*quotation.PriceValidity.ToString("HH:mm - dd MMM yy")*/,
                    TimeRemaining = totalHours >= 25 ? (totalDays) + " days remaining" : totalHours.ToString("00") + ":" + minutes.ToString("00") + " minutes remaining",
                    Percentage = percentage,
                    IsLowestQuotation = olderBidPrice == quotation.QuotationPrice
                });
            }
            return list;

        }
        public virtual async Task<object> BuyerRequestQuotationApproved(BuyerRequestBidApproveApiModel model)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            if (!model.QuotationId.Any())
                throw new ApplicationException("At least one quotation is required");

            var buyerRequest = await _requestService.GetRequestByIdAsync(model.BuyerRequestId);
            if (buyerRequest is null || buyerRequest.RequestStatus == RequestStatus.Pending)
                throw new ApplicationException("Invalid buyer request");

            //try to get a quotation with the specified id
            var quotations = await _quotationService.GetQuotationByIdsAsync(model.QuotationId.ToArray());
            if (!quotations.Any())
                throw new ApplicationException("No quotation found with the specified ids");

            if (quotations.Sum(x => x.Quantity) != buyerRequest.Quantity)
                return "Quotations quantity does not match the request quantity";

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null)
                throw new ApplicationException("Product not found");

            foreach (var quotation in quotations)
            {
                var supplier = await _customerService.GetCustomerByIdAsync(quotation.SupplierId);
                if (supplier is null)
                    throw new ApplicationException("Supplier not found");

                var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(quotation.RfqId);
                if (requestForQuotation is null)
                    throw new ApplicationException("Request for quotation not found");

                //Clear Cart
                var cartItems = await _shoppingCartService.GetShoppingCartAsync(supplier, ShoppingCartType.ShoppingCart, storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
                foreach (var cartItem in cartItems)
                    await _shoppingCartService.DeleteShoppingCartItemAsync(cartItem);

                //now let's try adding product to the cart (now including product attribute validation, etc)
                var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: supplier,
                    product: product,
                    shoppingCartType: ShoppingCartType.ShoppingCart,
                    storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                    attributesXml: buyerRequest.ProductAttributeXml,
                    quantity: quotation.Quantity, overridePrice: quotation.QuotationPrice, brandId: buyerRequest.BrandId);
                if (addToCartWarnings.Any())
                {
                    return addToCartWarnings;
                }

                //save address for supplier 
                await _utilityService.SavePurchaseOrderAddressAsync(supplier);

                //place order
                var processPaymentRequest = new ProcessPaymentRequest
                {
                    StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                };

                _paymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.CustomerId = quotation.SupplierId;
                processPaymentRequest.RequestId = buyerRequest.Id;
                processPaymentRequest.RFQId = requestForQuotation.Id;
                processPaymentRequest.QuotationId = quotation.Id;
                processPaymentRequest.OrderTypeId = (int)OrderType.PurchaseOrder;
                processPaymentRequest.OrderTotal = quotation.QuotationPrice * quotation.Quantity;
                processPaymentRequest.SalePrice = quotation.QuotationPrice;
                processPaymentRequest.Quotation = quotation;
                processPaymentRequest.Source = "Consumer";

                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    //await SavePurchaseOrderCalculationAsync(placeOrderResult.PlacedOrder, directOrderCalculation);

                    //Update Quotation Status
                    quotation.QuotationStatus = QuotationStatus.QuotationSelected;
                    quotation.IsApproved = true;
                    await _quotationService.UpdateQuotationAsync(quotation);

                    //Update Rfq status
                    requestForQuotation.RequestForQuotationStatus = RequestForQuotationStatus.Processing;
                    await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

                    var quotationsUnselected = (await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id)).Where(x => !x.IsApproved).ToList();
                    foreach (var unselectQuotation in quotationsUnselected)
                    {
                        unselectQuotation.QuotationStatus = QuotationStatus.QuotationUnSelected;
                        await _quotationService.UpdateQuotationAsync(unselectQuotation);
                    }
                }
                else
                    return await _localizationService.GetResourceAsync("Order.Does'nt.Placed.Success");
            }

            return await _localizationService.GetResourceAsync("Request.PurchaseOrder.Genearted.Successfully");
        }

        #endregion

        #region Buyer order
        public virtual async Task<IList<object>> BuyerOrderList(bool active)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            var orders = (await _orderService.SearchOrdersAsync(customerId: buyer.Id, getOnlyBuyersActiveOrdersForApi: active)).ToList();
            if (!orders.Any())
                throw new ApplicationException("Orders not found");

            //  var data = new List<object>();

            var data = orders.Select(async order =>
            {
                var paymentDueDate = "";
                var industry = "";
                var industryId = 0;

                if (order.RequestId > 0)
                {
                    var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
                    if (buyerRequest is not null)
                    {
                        industry = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";
                        industryId = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Id ?? 0;

                        var orderShipments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id));
                        if (!orderShipments.Any())
                        {
                            paymentDueDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                        }
                        else
                        {
                            var orderShipment = orderShipments.FirstOrDefault();
                            paymentDueDate = orderShipment.DeliveryDateUtc.HasValue ?
                                (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.DeliveryDateUtc.Value.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy") :
                                (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                        }
                    }
                }

                return new
                {
                    OrderId = order.Id,
                    OrderCustomNumber = order.CustomOrderNumber,
                    OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                    OrderStatusId = order.OrderStatusId,
                    PaymentDueDate = paymentDueDate,
                    Industry = industry,
                    IndustryId = industryId,
                };

            });

            var result = await Task.WhenAll(data);
            return result;
        }
        public virtual async Task<object> BuyerOrderDetail(int orderId)
        {
            if (orderId <= 0)
                throw new ApplicationException("Buyer order id is required");

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) /*|| !await _customerService.IsBuyerAsync(buyer)*/)
                throw new ApplicationException("Buyer not found");

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                throw new ApplicationException("Order not found");


            var industry = "";
            var productName = "";
            var brand = "";
            var qtyType = "";
            var deliveryDate = "";
            var deliveryAddress = "";
            var quantity = 0m;
            var orderShipped = "";
            var orderDelivered = "";

            var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
            if (buyerRequest is not null)
            {
                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";
                quantity = buyerRequest.Quantity;

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is not null)
                {
                    productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
                    qtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty;
                }

                brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "";
                deliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM yy");

                var city = (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.CityId));
                if (city != null)
                    deliveryAddress += city.Name + ", ";

                var area = (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.AreaId));
                if (area != null)
                    deliveryAddress += area.Name + ", ";

                if (!string.IsNullOrWhiteSpace(buyerRequest.DeliveryAddress))
                    deliveryAddress += buyerRequest.DeliveryAddress + ", " + buyerRequest.DeliveryAddress2;
            }

            if (order.ShippingStatusId == (int)ShippingStatus.Delivered || order.ShippingStatusId == (int)ShippingStatus.Shipped)
            {
                var orderShipment = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id)).FirstOrDefault();
                if (orderShipment is not null)
                {
                    orderShipped = orderShipment.ShippedDateUtc.HasValue ?
                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "";

                    orderDelivered = orderShipment.DeliveryDateUtc.HasValue && order.ShippingStatusId == (int)ShippingStatus.Delivered ?
                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "";
                }
            }

            //balance quantity working
            decimal shipmentBalanceQuantity = 0;
            foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true)) //we can ship only shippable products
            {
                var totalNumberOfItemsCanBeAddedToShipment = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem/*, true*/);
                if (totalNumberOfItemsCanBeAddedToShipment <= 0)
                    continue;

                //yes, we have at least one item to create a new shipment
                shipmentBalanceQuantity += await _priceCalculationService.RoundPriceAsync(totalNumberOfItemsCanBeAddedToShipment);
            }

            var buyerContract = (await _orderService.GetAllContractAsync(orderId: order.Id, buyerId: buyerRequest.BuyerId)).FirstOrDefault();
            if (buyerContract is not null)
                await _httpClient.GetStringAsync($"{_storeContext.GetCurrentStore().Url}download/get-contract/{buyerContract.ContractGuid}");

            var data = new
            {
                OrderInfo = new
                {
                    OrderId = order.Id,
                    CustomOrderNumber = order.CustomOrderNumber,
                    OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                    ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                    Industry = industry,
                    Product = productName,
                    Brand = brand,
                    Quantity = quantity,
                    QtyType = qtyType,
                    DeliveryDate = deliveryDate,
                    OrderCreatedDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
                    BuyerDeliveryAddress = deliveryAddress,
                    ProductAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    BuyerContract = new
                    {
                        contractId = buyerContract != null ? buyerContract.Id : 0,
                        downloadUrl = buyerContract != null ? $"{_storeContext.GetCurrentStore().Url}download/get-contract/{buyerContract.ContractGuid}" : "",
                        previewUrl = buyerContract != null ? $"{_storeContext.GetCurrentStore().Url}files/contracts/{buyerContract.ContractGuid}.pdf" : "",
                        contractSignature = buyerContract != null && buyerContract.SignaturePictureId > 0 ? true : false
                    }
                },
                OrderStatus = new
                {
                    OrderPending = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                    OrderProcessed = order.ProcessingDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(order.ProcessingDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "",
                    OrderShipped = orderShipped,
                    OrderDelivered = orderDelivered
                },
                ShippmentDetail = new
                {
                    DeliverAddress = buyerRequest.DeliveryAddress,
                    ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                    //NoOfShipmentNumber = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id, (int)ShipmentType.SaleOrder)).Count(),
                    QuantityLeft = shipmentBalanceQuantity,
                    Shippments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id)).Select(s =>
                    {
                        return new
                        {
                            ShippmentId = s.Id
                        };
                    }).ToList()
                }
            };
            return data;
        }
        public virtual async Task<object> BuyerOrderSummary(int orderId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new ApplicationException("user not found");

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                throw new ApplicationException("Order not found");

            var orderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(orderId);
            if (orderCalculation is null)
                throw new ApplicationException("Order calculation not found");

            var totalReceived = await _shipmentService.GetOrderPaidAmount(order);
            var totalAmountBalance = orderCalculation.OrderTotal - totalReceived;


            var data = new
            {
                TotalReceivables = await _priceFormatter.FormatPriceAsync(orderCalculation.OrderTotal),
                TotalReceived = await _priceFormatter.FormatPriceAsync(totalReceived),
                TotalBalance = await _priceFormatter.FormatPriceAsync(totalAmountBalance),
                PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus)
            };
            return data;
        }

        #endregion

        #region Buyer Reports
        public virtual async Task<object> BuyerRequestStatusSummary()
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");
            try
            {
                var lastRequestUpdated = await _requestService.GetLastUpdatedRecordByStatusAsync(buyerId: buyer.Id);

                var data = new
                {
                    Pending = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Pending }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = _utilityService.TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Pending, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Pending ? true : false
                    },
                    Verified = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Verified }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = _utilityService.TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Verified, buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Verified ? true : false
                    },
                    //QuotationChosen = new
                    //{
                    //    Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.QuotationChosen }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                    //    LastUpdated = TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.QuotationChosen, buyerId: buyer.Id)),
                    //    IsNew = lastRequestUpdated != null && lastRequestUpdated.StatusId == (int)BuyerRequestStatus.QuotationChosen ? true : false
                    // },
                    Approved = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Approved }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = _utilityService.TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Approved, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Approved ? true : false
                    },
                    Complete = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Complete }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = _utilityService.TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Complete, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Complete ? true : false
                    },
                    Cancelled = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Cancelled }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = _utilityService.TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Cancelled, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Cancelled ? true : false
                    },
                    Expired = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Expired }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = _utilityService.TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Expired, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Expired ? true : false
                    }
                };
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Support Agent
        public virtual async Task<object> SupportAgentInfo()
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            if (buyer.SupportAgentId == 0)
                throw new ApplicationException("Support agent not found");

            var agent = await _customerService.GetCustomerByIdAsync(buyer.SupportAgentId);
            if (agent is null)
                throw new ApplicationException("Support agent not found");

            var data = new
            {
                id = agent.Id,
                email = agent.Email,
                phonenumber = agent.Username,
                fullname = agent.FullName
            };
            return data;
        }
        #endregion

        #region Buyer Ledger
        public virtual async Task<object> BuyerLedgerDetail(DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");
            try
            {
                var startDateValue = !startDate.HasValue ? null
                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(startDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
                var endDateValue = !endDate.HasValue ? null
                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(endDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

                var detailModel = new List<BuyerLedgerDetailApiModel>();

                var customerLedgerDetails = (await _customerLedgerService.GetCustomerLedgerDetailByIdAsync(buyerId: buyer.Id, startDateValue, endDateValue, pageIndex: pageIndex - 1, pageSize: pageSize)).ToList();
                var totalBlanaceFormate = "";
                if (customerLedgerDetails.Any())
                {
                    var ledgerDeatils = (customerLedgerDetails.GroupBy(x => x.Date.HasValue ? x.Date.Value.ToString("MM/dd/yyyy") : "").ToList());
                    foreach (var ledgerDetail in ledgerDeatils)
                    {
                        if (string.IsNullOrWhiteSpace(ledgerDetail.Key))
                            continue;

                        var detailLedgerModel = new BuyerLedgerDetailApiModel();
                        detailLedgerModel.date = (await _dateTimeHelper.ConvertToUserTimeAsync(Convert.ToDateTime(ledgerDetail.Key), DateTimeKind.Utc)).ToString();

                        foreach (var ledger in ledgerDetail)
                        {
                            var buyerLedgerModel = new BuyerLedgerDeatailListApiModel();
                            buyerLedgerModel.description = ledger.Description == "Delivery" ? "Goods Delivered" : ledger.Description;
                            buyerLedgerModel.debitFormatted = ledger.Debit > 0 || ledger.Debit < 0 ? await _priceFormatter.FormatPriceAsync(ledger.Debit, true, false) : "";
                            buyerLedgerModel.debit = ledger.Debit;
                            buyerLedgerModel.sku = ledger.ProductName;
                            buyerLedgerModel.creditFormatted = ledger.Credit > 0 ? await _priceFormatter.FormatPriceAsync(ledger.Credit, true, false) : "";
                            buyerLedgerModel.credit = ledger.Credit;
                            buyerLedgerModel.outstandingBalance = ledger.Balance < 0 ? await _priceFormatter.FormatPriceAsync(ledger.Balance < 0 ? Math.Abs(ledger.Balance) : ledger.Balance, true, false) + "CR" : await _priceFormatter.FormatPriceAsync(ledger.Balance, true, false) + "DR";
                            buyerLedgerModel.orderId = ledger.OrderId.HasValue ? ledger.OrderId.Value : 0;
                            buyerLedgerModel.customOrderNumber = ledger.OrderId.HasValue ? (await _orderService.GetOrderByIdAsync(ledger.OrderId.Value)).CustomOrderNumber : "";
                            buyerLedgerModel.quantity = ledger.Quantity.HasValue ? Math.Round(ledger.Quantity.Value, 2) : 0;
                            buyerLedgerModel.quantityType = ledger.QuantityType;
                            buyerLedgerModel.shipmentId = ledger.ShipmentId.HasValue ? ledger.ShipmentId.Value : 0;
                            buyerLedgerModel.customShipmentNumber = ledger.CustomShipmentNumber;
                            buyerLedgerModel.brandId = ledger.BrandId.HasValue ? ledger.BrandId.Value : 0;
                            buyerLedgerModel.brand = ledger.BrandName;
                            buyerLedgerModel.paymentId = ledger.PaymentId.HasValue ? ledger.PaymentId.Value : 0;

                            if (buyerLedgerModel.paymentId > 0)
                            {
                                var allocatePayments = (await _ledgerService.GetAllShipmentPaymentMappingsAsync(paymentId: ledger.PaymentId.Value)).ToList();
                                foreach (var payment in allocatePayments)
                                {
                                    var shipment = await _shipmentService.GetShipmentByIdAsync(payment.ShipmentId);
                                    if (shipment is null)
                                        continue;

                                    var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
                                    if (order is null)
                                        continue;

                                    buyerLedgerModel.PaymentDetails.Add(new BuyerLedgerDeatailListApiModel.LedgerPaymentDetailModel
                                    {
                                        ShipmentId = shipment.Id,
                                        CustomShipmentNumber = shipment.CustomShipmentNumber,
                                        OrderId = order.Id,
                                        CustomOrderNumber = order.CustomOrderNumber,
                                        AmountFormatted = await _priceFormatter.FormatPriceAsync(payment.Amount, true, false),
                                        DateFormatted = (await _dateTimeHelper.ConvertToUserTimeAsync(payment.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MMMM/yy h:mm:ss tt")
                                    });
                                }
                            }

                            detailLedgerModel.buyerLedgerlistModel.Add(buyerLedgerModel);
                        }
                        detailModel.Add(detailLedgerModel);
                    }

                    var totalBalanace = detailModel.Sum(x => x.buyerLedgerlistModel.Sum(m => m.debit) - x.buyerLedgerlistModel.Sum(m => m.credit));
                    totalBlanaceFormate = await _priceFormatter.FormatPriceAsync(totalBalanace < 0 ? Math.Abs(totalBalanace) : totalBalanace, true, false);

                    if (totalBalanace > 0)
                        totalBlanaceFormate = $"{totalBlanaceFormate} DR";
                    if (totalBalanace < 0)
                        totalBlanaceFormate = $"{totalBlanaceFormate} CR";
                }

                var data = new
                {
                    totalBalance = totalBlanaceFormate,
                    downloadUrl = $"{_storeContext.GetCurrentStore().Url}admin/customerledger/CustomerLedgerPdfDownload/{buyer.Id}",
                    detailModel
                };

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Buyer Contract
        public virtual async Task<object> Buyer_BuyerContactUploadSignature(BuyerContractUploadSignatureModel model)
        {
            if (model.imgBytes == null)
                throw new ApplicationException("Image is required");

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new ApplicationException("user not found");

            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                throw new ApplicationException("Order not found.");

            var buyerContract = await _orderService.GetContractByIdAsync(model.ContactId);
            if (buyerContract == null)
                throw new ApplicationException("Buyer contract not found.");

            buyerContract.SignaturePictureId = await _utilityService.Buyer_UploadPicture(model.imgBytes, $"{buyerContract.ContractGuid}");
            await _orderService.UpdateContractAsync(buyerContract);

            await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}");

            var BuyerContract = new
            {
                contractId = buyerContract.Id,
                downloadUrl = $"{_storeContext.GetCurrentStore().Url}download/get-contract/{buyerContract.ContractGuid}",
                previewUrl = $"{_storeContext.GetCurrentStore().Url}files/contracts/{buyerContract.ContractGuid}.pdf",
                contractSignature = buyerContract.SignaturePictureId > 0 ? true : false
            };
            return buyerContract;
        }
        #endregion

        #region Applied Credit Customer
        public virtual async Task ApplyForCreditCustomer(ApplyCreditCustomerModel model)
        {
            //if (string.IsNullOrEmpty(model.FullName))
            //    throw new ApplicationException("FullName is required");
            //if (string.IsNullOrEmpty(model.RegisteredphoneNumber))
            //    throw new ApplicationException("RegisteredphoneNumber is required");
            //if (string.IsNullOrEmpty(model.BusinessAddress))
            //    throw new ApplicationException("BusinessAddress is required");
            //if (string.IsNullOrEmpty(model.CnicFrontImageByte))
            //    throw new ApplicationException("Cnic Front is required");
            //if (string.IsNullOrEmpty(model.CnicBackImageByte))
            //    throw new ApplicationException("Cnic Back is required");
            //if (string.IsNullOrEmpty(model.Cnic))
            //    throw new ApplicationException("Cnic number is required");

            //var user = await _workContext.GetCurrentCustomerAsync();
            //if (!await _customerService.IsRegisteredAsync(user))
            //    throw new ApplicationException("user not found");

            //Guid objfront = Guid.NewGuid();
            //Guid objback = Guid.NewGuid();

            //var cincfront = await _amazonS3BuketService.UploadBase64FileAsync(model.CnicFrontImageByte, _commonSettings.BucketName, objfront.ToString() + ".jpg");
            //var cincback = await _amazonS3BuketService.UploadBase64FileAsync(model.CnicBackImageByte, _commonSettings.BucketName, objback.ToString() + ".jpg");
            //var applyCreditCustomer = new AppliedCreditCustomer
            //{
            //    CustomerId = user.Id,
            //    FullName = model.FullName,
            //    RegisteredPhoneNumber = model.RegisteredphoneNumber,
            //    BusinessAddress = model.BusinessAddress,
            //    CnicFront = cincfront,
            //    CnicBack = cincback,
            //    Cnic = model.Cnic,
            //    StatusId = (int)AppliedCreditCustomerStatusEnum.Pending,
            //    CreatedOnUtc = DateTime.UtcNow,
            //    CreatedById = user.Id
            //};

            //await _customerService.InsertAppliedCreditCustomerAsync(applyCreditCustomer);
        }
        public virtual async Task<object> GetApplyForCreditCustomer()
        {
            //var user = await _workContext.GetCurrentCustomerAsync();
            //if (!(await _customerService.IsRegisteredAsync(user) && await _customerService.IsBuyerAsync(user)))
            //{
            //    throw new ApplicationException("User not found");
            //}

            //var getcustomer = await _customerService.GetAppliedCreditCustomerByCustomerIdAsync(user.Id);
            //if (getcustomer == null)
            //{
            //    throw new ApplicationException("Customer not found");
            //}

            //var status = await _localizationService.GetLocalizedEnumAsync<AppliedCreditCustomerStatusEnum>((AppliedCreditCustomerStatusEnum)getcustomer.StatusId);

            //var data = new
            //{
            //    CustomerId = getcustomer.CustomerId,
            //    FullName = getcustomer.FullName,
            //    RegisteredPhoneNumber = getcustomer.RegisteredPhoneNumber,
            //    BusinessAddress = getcustomer.BusinessAddress,
            //    CnicFront = getcustomer.CnicFront,
            //    CnicBack = getcustomer.CnicBack,
            //    Cnic = getcustomer.Cnic,
            //    Status = status,
            //    CreatedOnUtc = getcustomer.CreatedOnUtc,
            //    CreatedById = getcustomer.CreatedById
            //};
            var data = new
            {
                test=true
            };
            return data;

        }
        #endregion

        public async Task<string> CreditApplication(CreditApplicationModel model)
        {

            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new ApplicationException("Phone Number is required");

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new ApplicationException("user not found");

            var applyCreditCustomer = new AppliedCreditCustomer
            {
                CustomerId = user.Id,
                AnnualBusinessRevenue = model.AnnualBusinessRev,
                CurrentAssetValuation = model.CurrentAssetValuation,
                DatedChecque = model.DatedChecque,
                FinancialStatementFileId = model.FinancialStatementFileId,
                FinancingAmountRequired = model.FinancingAmountRequired,
                FixedAssetValuation = model.FixedAssetValuation,
                HistoricInvoicesFileIds = string.Join(",", model.HistoricInvoicesFileIds),
                NoOfUniqueClients = model.NoOfUniqueClients,
                TaxationCertificateFileId = model.TaxationCertificateFileId,
                TenancyContractFileId = model.TenancyContractFileId,
                BusinessName = model.BusinessName,
                CnicFrontAndBackFileIds = string.Join(",", model.CnicFrontAndBackFileIds),
                EmailAddress = model.EmailAddress,
                IncorporationCertificateFileId = model.IncorporationCertificateFileId,
                PhoneNumber = model.PhoneNumber,
                BusinessCityId = model.BusinessCityId,
                UtilityBillFileIds = string.Join(",", model.UtilityBillFileIds),
                StatusId = (int)AppliedCreditCustomerStatusEnum.Pending,
                CreatedOnUtc = DateTime.UtcNow,
                CreatedById = user.Id
            };

            await _customerService.InsertAppliedCreditCustomerAsync(applyCreditCustomer);

            foreach (var item in model.BankStatements)
            {
                var bankStatement = new BankStatement
                {
                    BankName = item.BankName,
                    FileId = item.FileId,
                    AppliedCreditCustomerId = applyCreditCustomer.Id,
                    CreatedOnUtc = DateTime.UtcNow

                };
                await _customerService.InsertBankStatementAsync(bankStatement);

            }
            return "Record has been added.";
        }
        #endregion
    }
}
