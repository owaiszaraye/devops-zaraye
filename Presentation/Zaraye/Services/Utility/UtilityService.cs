using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Xml;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.Orders;
using Zaraye.Models.Api.V4.Buyer;
using Zaraye.Models.Api.V4.OrderManagement;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.Configuration;
using Zaraye.Services.CustomerLedgers;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.Helpers;
using Zaraye.Services.Inventory;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Media;
using Zaraye.Services.News;
using Zaraye.Services.Orders;
using Zaraye.Services.Payments;
using Zaraye.Services.PriceDiscovery;
using Zaraye.Services.Shipping;
using static Zaraye.Models.Api.V4.Common.CommonApiModel;
using static Zaraye.Models.Api.V4.OrderManagement.OrderManagementApiModel;

namespace Zaraye.Services.Utility
{
    public class UtilityService : IUtilityService
    {
        #region Fields

        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerActivityService _customerActivityService;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingService _settingService;
        private readonly IPriceDiscoveryService _priceDiscoveryService;
        private readonly IInventoryService _inventoryService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CalculationSettings _calculationSettings;

        #endregion

        #region Ctor

        public UtilityService(ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICustomerActivityService customerActivityService,
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
            ICountryService countryService, IHttpContextAccessor httpContextAccessor, ISettingService settingService,
            IPriceDiscoveryService priceDiscoveryService, IInventoryService inventoryService, ShoppingCartSettings shoppingCartSettings,
            CalculationSettings calculationSettings
            )
        {
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _workContext = workContext;
            _customerActivityService = customerActivityService;
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
            _httpContextAccessor = httpContextAccessor;
            _settingService = settingService;
            _priceDiscoveryService = priceDiscoveryService;
            _inventoryService = inventoryService;
            _shoppingCartSettings = shoppingCartSettings;
            _calculationSettings = calculationSettings;
        }

        #endregion
        public async Task<string> ConvertToXmlAsync(List<BuyerRequestApiModel.AttributesApiModel> attributeDtos, int productId)
        {
            var attributesXml = "";

            if (attributeDtos is not { Count: > 0 })
            {
                return attributesXml;
            }

            var productAttributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"{ZarayeCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            // there should be only one selected value for this attribute
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                int selectedAttributeValue;
                                var isInt = int.TryParse(selectedAttribute.Value, out selectedAttributeValue);
                                if (isInt && selectedAttributeValue > 0)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedAttributeValue.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            // there could be more than one selected value for this attribute
                            var selectedAttributes = attributeDtos.Where(x => x.Name == controlId);
                            foreach (var selectedAttribute in selectedAttributes)
                            {
                                int selectedAttributeValue;
                                var isInt = int.TryParse(selectedAttribute.Value, out selectedAttributeValue);
                                if (isInt && selectedAttributeValue > 0)
                                {
                                    // currently there is no support for attribute quantity
                                    var quantity = 1;
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedAttributeValue.ToString(), quantity);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only(already server - side selected) values
                            var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                                                .Where(v => v.IsPreSelected)
                                                                .Select(v => v.Id)
                                                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();

                            if (selectedAttribute != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttribute.Value);
                            }
                        }
                        break;
                    case AttributeControlType.NumericTextBox:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttribute.Value);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                DateTime selectedDate;
                                // Since nopCommerce uses this format to keep the date in the database to keep it consisten we will expect the same format to be passed
                                var validDate = DateTime.TryParseExact(selectedAttribute.Value, "D", CultureInfo.CurrentCulture,
                                                                       DateTimeStyles.None, out selectedDate);

                                if (validDate)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedDate.ToString("D"));
                                }
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                Guid downloadGuid;
                                Guid.TryParse(selectedAttribute.Value, out downloadGuid);
                                var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                                if (download != null)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, download.DownloadGuid.ToString());
                                }
                            }
                        }
                        break;
                }
            }

            return attributesXml;
        }

        public async Task<List<ProductItemAttributeApiModel>> ParseAttributeXml(string attributesXml)
        {
            var attributeDtos = new List<ProductItemAttributeApiModel>();
            if (string.IsNullOrEmpty(attributesXml))
            {
                return attributeDtos;
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(attributesXml);

            foreach (XmlNode attributeNode in xmlDoc.SelectNodes(@"//Attributes/ProductAttribute"))
            {
                if (attributeNode.Attributes?["ID"] != null)
                {
                    int attributeMappingId;
                    if (int.TryParse(attributeNode.Attributes["ID"].InnerText.Trim(), out attributeMappingId))
                    {
                        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(attributeMappingId);
                        if (productAttributeMapping is not null)
                        {
                            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId);
                            if (productAttribute is not null)
                            {
                                var attributeName = string.IsNullOrWhiteSpace(productAttributeMapping.TextPrompt) ? await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name, (await _workContext.GetWorkingLanguageAsync()).Id) : await _localizationService.GetLocalizedAsync(productAttributeMapping, a => productAttributeMapping.TextPrompt, (await _workContext.GetWorkingLanguageAsync()).Id);

                                foreach (XmlNode attributeValue in attributeNode.SelectNodes("ProductAttributeValue"))
                                {
                                    var value = "";

                                    switch (productAttributeMapping.AttributeControlType)
                                    {
                                        case AttributeControlType.DropdownList:
                                        case AttributeControlType.RadioList:
                                        case AttributeControlType.Checkboxes:
                                        case AttributeControlType.ColorSquares:
                                        case AttributeControlType.ImageSquares:
                                        case AttributeControlType.ReadonlyCheckboxes:
                                            {
                                                var productAttributeValueId = attributeValue.SelectSingleNode("Value").InnerText.Trim();
                                                var productAttributeValue = await _productAttributeService.GetProductAttributeValueByIdAsync(Convert.ToInt32(productAttributeValueId));
                                                if (productAttributeValue is not null)
                                                    value = await _localizationService.GetLocalizedAsync(productAttributeValue, a => a.Name, (await _workContext.GetWorkingLanguageAsync()).Id);

                                                attributeDtos.Add(new ProductItemAttributeApiModel
                                                {
                                                    AttributeId = productAttributeMapping.ProductAttributeId,
                                                    AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                                                    AttributeName = attributeName,
                                                    Value = value,
                                                    ValueId = productAttributeValue is not null ? productAttributeValue.Id : 0
                                                });
                                            }
                                            break;

                                        case AttributeControlType.TextBox:
                                        case AttributeControlType.NumericTextBox:
                                        case AttributeControlType.MultilineTextbox:
                                        case AttributeControlType.Datepicker:
                                        case AttributeControlType.FileUpload:
                                            {
                                                value = attributeValue.SelectSingleNode("Value").InnerText.Trim();

                                                attributeDtos.Add(new ProductItemAttributeApiModel
                                                {
                                                    AttributeId = productAttributeMapping.ProductAttributeId,
                                                    AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                                                    AttributeName = attributeName,
                                                    Value = value,
                                                    ValueId = 0
                                                });
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return attributeDtos;
        }

        public string TimeAgo(DateTime? dt)
        {
            if (!dt.HasValue)
                return "";

            TimeSpan span = DateTime.UtcNow - dt.Value;
            if (span.Days > 365)
            {
                int years = span.Days / 365;
                if (span.Days % 365 != 0)
                    years += 1;
                return string.Format("{0} {1} ago",
                years, years == 1 ? "year" : "years");
            }

            if (span.Days > 30)
            {
                int months = span.Days / 30;
                if (span.Days % 31 != 0)
                    months += 1;
                return string.Format("{0} {1} ago",
                months, months == 1 ? "month" : "months");
            }

            if (span.Days > 0)
                return string.Format("{0} {1} ago",
                span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return string.Format("{0} {1} ago",
                span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return string.Format("{0} {1} ago",
                span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            if (span.Seconds > 5)
                return string.Format("{0} seconds ago", span.Seconds);
            if (span.Seconds <= 5)
                return "just now";

            return string.Empty;
        }

        public async Task<decimal> GetBuyerProgress(Customer buyer = null)
        {
            decimal Percentage = Math.Round((decimal)((float)100 / 12f), 2);
            decimal TotalPercent = 0;

            if (buyer == null)
                return TotalPercent;

            //var buyerTypeAttributeId = await _genericAttributeService.GetAttributeAsync<int>(buyer, NopCustomerDefaults.BuyerTypeIdAttribute);
            var userType = await _customerService.GetUserTypeByIdAsync(buyer.UserTypeId);
            var agent = await _customerService.GetCustomerByIdAsync(buyer.SupportAgentId);
            var store = _storeContext.GetCurrentStore();

            var pinLocationAttribute = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, storeId: store.Id);
            var pinLocation = !string.IsNullOrWhiteSpace(pinLocationAttribute) ? pinLocationAttribute.Split(",") : new string[] { };

            //Form Fields
            var buyerType = userType != null ? userType.Name : null;
            //var countryId = await _genericAttributeService.GetAttributeAsync<int>(buyer, NopCustomerDefaults.CountryIdAttribute);
            //var stateId = await _genericAttributeService.GetAttributeAsync<int>(buyer, NopCustomerDefaults.StateProvinceIdAttribute);
            //var areaId = await _genericAttributeService.GetAttributeAsync<int>(buyer, NopCustomerDefaults.AreaIdAttribute);
            var buyerPinLocation = pinLocation.Any() ? true : false;

            if (!string.IsNullOrWhiteSpace(buyer.FirstName) && !string.IsNullOrWhiteSpace(buyer.LastName))
                TotalPercent += Percentage;
            if (!string.IsNullOrWhiteSpace(buyer.Email))
                TotalPercent += Percentage;
            if (!string.IsNullOrWhiteSpace(buyer.Username))
                TotalPercent += Percentage;
            if (buyer.IndustryId > 0)
                TotalPercent += Percentage;
            if (!string.IsNullOrWhiteSpace(buyer.Company))
                TotalPercent += Percentage;
            if (!string.IsNullOrWhiteSpace(buyerType))
                TotalPercent += Percentage;
            if (buyer.CountryId > 0)
                TotalPercent += Percentage;
            if (buyer.StateProvinceId > 0)
                TotalPercent += Percentage;
            if (buyer.AreaId > 0)
                TotalPercent += Percentage;
            if (!string.IsNullOrWhiteSpace(buyer.StreetAddress))
                TotalPercent += Percentage;
            if (!string.IsNullOrWhiteSpace(buyer.StreetAddress2))
                TotalPercent += Percentage;
            if (buyerPinLocation)
                TotalPercent += Percentage;

            return Math.Round(TotalPercent);
        }

        public async Task<(int totalDays, int totalHours, int minutes, int percentage)> GetPercentageAndTimeRemaining(DateTime priceValidity, DateTime createdOnUtc)
        {
            var bidPrice = await _priceFormatter.FormatPriceAsync(0, true, false);
            var totalDays = 0;
            var totalHours = 0;
            var minutes = 0;
            var percentage = 0;

            var timeSpan = priceValidity.Subtract(DateTime.Now);
            totalDays = Convert.ToInt32(timeSpan.TotalDays);
            totalDays = Math.Abs(totalDays);
            if (timeSpan.TotalHours > 0)
            {
                totalHours = Convert.ToInt32(timeSpan.TotalHours);
                totalHours = Math.Abs(totalHours);
                minutes = timeSpan.Minutes;
                minutes = Math.Abs(timeSpan.Minutes);
            }

            if (timeSpan.Ticks < 0)
                percentage = 100;
            else
            {
                var subtractedTotalSeconds = TimeSpan.FromTicks(timeSpan.Ticks).TotalSeconds;
                var timeElapsedTotalSecondsv = (priceValidity - createdOnUtc).TotalSeconds;
                percentage = 100 - (int)Math.Round(subtractedTotalSeconds / timeElapsedTotalSecondsv * 100);
            }

            return (totalDays, totalHours, minutes, percentage);
        }

        public string GetMimeTypeFromImageByteArray(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            using (Image image = Image.FromStream(stream))
            {
                return ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == image.RawFormat.Guid).MimeType;
            }
        }

        public async Task<int> Buyer_UploadPicture(byte[] imgBytes, string fileName)
        {
            if (imgBytes == null)
                return 0;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                var qqFileNameParameter = "qqfilename";

                if (string.IsNullOrEmpty(fileName) && _httpContextAccessor.HttpContext.Request.Form.ContainsKey(qqFileNameParameter))
                    fileName = _httpContextAccessor.HttpContext.Request.Form[qqFileNameParameter].ToString();

                var mimType = GetMimeTypeFromImageByteArray(imgBytes);

                var fileExtension = fileInfo.Extension;
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();

                var picture = await _pictureService.InsertPictureAsync(imgBytes, mimType, fileName);

                return picture.Id;
            }
            catch { }

            return 0;
        }

        public async Task SavePurchaseOrderAddressAsync(Customer supplier)
        {
            //existing address
            var address = (await _customerService.GetAddressesByCustomerIdAsync(supplier.Id)).FirstOrDefault();
            if (address == null)
            {
                //insert default address (if possible)
                address = new Address
                {
                    FirstName = supplier.FirstName,
                    LastName = supplier.LastName,
                    Email = supplier.Email,
                    Company = supplier.Company,
                    CountryId = supplier.CountryId,
                    StateProvinceId = supplier.StateProvinceId,
                    County = supplier.County,
                    City = supplier.City,
                    Address1 = supplier.StreetAddress,
                    Address2 = supplier.StreetAddress2,
                    ZipPostalCode = supplier.ZipPostalCode,
                    PhoneNumber = supplier.Phone,
                    FaxNumber = supplier.Fax,
                    CreatedOnUtc = supplier.CreatedOnUtc
                };

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.InsertAddressAsync(address);
                await _customerService.InsertCustomerAddressAsync(supplier, address);

                supplier.BillingAddressId = address.Id;
                supplier.ShippingAddressId = address.Id;

                await _customerService.UpdateCustomerAsync(supplier);
            }
            else
            {
                //update address
                address.Email = supplier.Email;
                address.FirstName = supplier.FirstName;
                address.LastName = supplier.LastName;
                address.Email = supplier.Email;
                address.Company = supplier.Company;
                address.CountryId = supplier.CountryId;
                address.StateProvinceId = supplier.StateProvinceId;
                address.County = supplier.County;
                address.City = supplier.City;
                address.Address1 = supplier.StreetAddress;
                address.Address2 = supplier.StreetAddress2;
                address.ZipPostalCode = supplier.ZipPostalCode;
                address.PhoneNumber = supplier.Phone;
                address.FaxNumber = supplier.Fax;

                await _addressService.UpdateAddressAsync(address);

                supplier.BillingAddressId = address.Id;
                supplier.ShippingAddressId = address.Id;

                await _customerService.UpdateCustomerAsync(supplier);
            }
        }

        public async Task<string> Common_ConvertToXmlAsync(List<CombinationAttributeApiModel> attributeDtos, int productId)
        {
            var attributesXml = "";

            if (attributeDtos is not { Count: > 0 })
            {
                return attributesXml;
            }

            var productAttributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"{ZarayeCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            // there should be only one selected value for this attribute
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                int selectedAttributeValue;
                                var isInt = int.TryParse(selectedAttribute.Value, out selectedAttributeValue);
                                if (isInt && selectedAttributeValue > 0)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedAttributeValue.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            // there could be more than one selected value for this attribute
                            var selectedAttributes = attributeDtos.Where(x => x.Name == controlId);
                            foreach (var selectedAttribute in selectedAttributes)
                            {
                                int selectedAttributeValue;
                                var isInt = int.TryParse(selectedAttribute.Value, out selectedAttributeValue);
                                if (isInt && selectedAttributeValue > 0)
                                {
                                    // currently there is no support for attribute quantity
                                    var quantity = 1;
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedAttributeValue.ToString(), quantity);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only(already server - side selected) values
                            var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                                                .Where(v => v.IsPreSelected)
                                                                .Select(v => v.Id)
                                                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttribute.Value);
                            }
                        }
                        break;
                    case AttributeControlType.NumericTextBox:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttribute.Value);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                DateTime selectedDate;
                                // Since nopCommerce uses this format to keep the date in the database to keep it consisten we will expect the same format to be passed
                                var validDate = DateTime.TryParseExact(selectedAttribute.Value, "D", CultureInfo.CurrentCulture,
                                                                       DateTimeStyles.None, out selectedDate);

                                if (validDate)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedDate.ToString("D"));
                                }
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                Guid downloadGuid;
                                Guid.TryParse(selectedAttribute.Value, out downloadGuid);
                                var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                                if (download != null)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, download.DownloadGuid.ToString());
                                }
                            }
                        }
                        break;
                }
            }

            return attributesXml;
        }
        public async Task<decimal> GetFinalRate(int rateId, Customer buyer, int supplierId, decimal rate, int categoryId, bool includeFM)
        {
            var firstMileCost = 0M;
            decimal? marginRate = 0M;
            decimal marginValue = 0M;

            var supplier = await _customerService.GetCustomerByIdAsync(supplierId);
            if (supplier is not null)
            {
                var supplierCity = await _stateProvinceService.GetStateProvinceByIdAsync(supplier.StateProvinceId);
                var buyerCity = await _stateProvinceService.GetStateProvinceByIdAsync(buyer.StateProvinceId);

                if (supplierCity != null && buyerCity != null && includeFM)
                    firstMileCost = await _settingService.GetSettingByKeyAsync<decimal>($"FirstMile_{categoryId}_{supplierCity.Id}_{buyerCity.Id}", 0, _storeContext.GetCurrentStore().Id);

                if (buyerCity != null)
                {
                    marginRate = await _priceDiscoveryService.GetMarginRateAsync(rateId, buyerCity.Id);
                    if (marginRate.HasValue && marginRate.Value > 0)
                        marginValue = rate * (marginRate.Value / 100m);
                }
            }

            return rate = rate + marginValue + firstMileCost;
        }
        public string getRelativeDateTime(DateTime date)
        {
            TimeSpan ts = DateTime.UtcNow - date;
            if (ts.TotalMinutes < 1)//seconds ago
                return "just now";
            if (ts.TotalHours < 1)//min ago
                return (int)ts.TotalMinutes + " Minutes ago";
            if (ts.TotalDays < 1)//hours ago
                return (int)ts.TotalHours + " Hours ago";
            if (ts.TotalDays < 7)//days ago
                return (int)ts.TotalDays + " Days ago";
            if (ts.TotalDays < 30.4368)//weeks ago
                return (int)(ts.TotalDays / 7) + " Weeks ago";
            if (ts.TotalDays < 365.242)//months ago
                return (int)(ts.TotalDays / 30.4368) == 1 ? "1 Month ago" : (int)(ts.TotalDays / 30.4368) + " Months ago";
            //years ago
            return (int)(ts.TotalDays / 365.242) + " Years ago";
        }
        public string getRelativeDateTime(DateTime date,bool isUrdu)
        {
            TimeSpan ts = DateTime.UtcNow - date;
            char rleChar = (char)0x202B;
            if (ts.TotalMinutes < 1)//seconds ago
                return "ابھی ابھی";
            if (ts.TotalHours < 1)//min ago
            {
                var result = (int)ts.TotalMinutes + "  لمحے قبل";
                return rleChar + result;
            }
            if (ts.TotalDays < 1)//hours ago
            {
                var result = (int)ts.TotalHours + " گھنٹوں پہلے";
                return rleChar + result;
            }
            if (ts.TotalDays < 7)//days ago
            {
                var result = (int)ts.TotalDays + " دنوں پہلے";
                return rleChar + result;
            }
            if (ts.TotalDays < 30.4368)//weeks ago
            {
                var result = (int)(ts.TotalDays / 7) + " ہفتوں پہلے";
                return rleChar + result;
            }
            if (ts.TotalDays < 365.242)//months ago
            {
                var result= (int)(ts.TotalDays / 30.4368) == 1 ? " ماہ پہلے" : (int)(ts.TotalDays / 30.4368) + " مہینوں پہلے";
                return rleChar + result;
            }
            //years ago
            else
            {
                var result = (int)(ts.TotalDays / 365.242) + " برس قبل";
                return rleChar + result;
            }
           
        }
        public async Task<decimal> CalculateSellingPriceOfProductByTaggings(List<DirectCogsInventoryTagging> cogsInventoryTaggings, bool gstInclude = false, decimal sellingPriceOfProduct = 0)
        {
            if (cogsInventoryTaggings.Any())
            {
                var inventoryInbounds = await _inventoryService.GetInventoryInboundByIdsAsync(cogsInventoryTaggings.Select(x => x.InventoryId).ToArray());
                if (inventoryInbounds.Any())
                {
                    //if all inventories have no gst and gstInclude = true
                    if (inventoryInbounds.All(x => x.GstAmount > 0) && !gstInclude)
                        sellingPriceOfProduct = sellingPriceOfProduct / 1.18m;

                    //if any of the inventory have no gst and gstInclude = false
                    if (inventoryInbounds.Any(x => x.GstAmount == 0) && inventoryInbounds.Any(x => x.GstAmount > 0) && !gstInclude)
                        sellingPriceOfProduct = sellingPriceOfProduct / 1.18m;

                    //if any of the inventory have no gst and gstInclude = false
                    if (inventoryInbounds.Count == 1 && inventoryInbounds.Any(x => x.GstAmount > 0) && !gstInclude)
                        sellingPriceOfProduct = sellingPriceOfProduct / 1.18m;
                }
            }
            return sellingPriceOfProduct;
        }


        public async Task<decimal> CalculateBuyingPriceByTaggings(List<DirectCogsInventoryTagging> cogsInventoryTaggings, bool gstInclude = false)
        {
            var buyingPrice = 0m;
            if (cogsInventoryTaggings.Any())
            {
                var inventoryInbounds = await _inventoryService.GetInventoryInboundByIdsAsync(cogsInventoryTaggings.Select(x => x.InventoryId).ToArray());
                if (inventoryInbounds.Any())
                {
                    var cogsCalculationValue = 0m;
                    var quantity = cogsInventoryTaggings.Sum(x => x.Quantity);

                    if (inventoryInbounds.All(x => x.GstAmount > 0) && !gstInclude)
                    {
                        foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                        {
                            var inventoryRate = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
                            cogsCalculationValue += inventoryRate;
                        }
                        buyingPrice = cogsCalculationValue / quantity;
                    }

                    if (inventoryInbounds.All(x => x.GstAmount == 0) && !gstInclude)
                    {
                        foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                        {
                            var inventoryRate = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
                            cogsCalculationValue += inventoryRate;
                        }
                        buyingPrice = cogsCalculationValue / quantity;
                    }

                    if (inventoryInbounds.All(x => x.GstAmount > 0) && gstInclude)
                    {
                        foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                        {
                            var inventoryRate = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
                            cogsCalculationValue += inventoryRate;
                        }
                        buyingPrice = cogsCalculationValue / quantity;
                    }

                    //if all inventories have no gst and gstInclude = true
                    if (inventoryInbounds.All(x => x.GstAmount == 0) && gstInclude)
                    {
                        foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                        {
                            var inventoryRate = cogsInventoryTagging.Quantity * (cogsInventoryTagging.Rate / 1.18m);
                            cogsCalculationValue += inventoryRate;
                        }
                        buyingPrice = cogsCalculationValue / quantity;
                    }

                    //if any of the inventory have no gst and gstInclude = true
                    if (inventoryInbounds.Any(x => x.GstAmount == 0) && inventoryInbounds.Any(x => x.GstAmount > 0) && gstInclude)
                    {
                        var inventoryIds = inventoryInbounds.Where(x => x.GstAmount == 0).Select(x => x.Id).ToList();
                        foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                        {
                            if (inventoryIds.Contains(cogsInventoryTagging.InventoryId))
                            {
                                var inventoryRate = cogsInventoryTagging.Quantity * (cogsInventoryTagging.Rate / 1.18m);
                                cogsCalculationValue += inventoryRate;
                            }
                            else
                            {
                                var inventoryRate = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
                                cogsCalculationValue += inventoryRate;
                            }
                        }
                        buyingPrice = cogsCalculationValue / quantity;
                    }

                    //if any of the inventory have no gst and gstInclude = false
                    if (inventoryInbounds.Any(x => x.GstAmount == 0) && inventoryInbounds.Any(x => x.GstAmount > 0) && !gstInclude)
                    {
                        var inventoryIds = inventoryInbounds.Where(x => x.GstAmount == 0).Select(x => x.Id).ToList();
                        foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                        {
                            if (inventoryIds.Contains(cogsInventoryTagging.InventoryId))
                            {
                                var inventoryRate = cogsInventoryTagging.Quantity * (cogsInventoryTagging.Rate / 1.18m);
                                cogsCalculationValue += inventoryRate;
                            }
                            else
                            {
                                var inventoryRate = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
                                cogsCalculationValue += inventoryRate;
                            }
                        }
                        buyingPrice = cogsCalculationValue / quantity;
                    }

                    if (buyingPrice == 0 && cogsInventoryTaggings.Count == 1)
                        buyingPrice = cogsInventoryTaggings.FirstOrDefault().Rate;
                }
            }
            return buyingPrice;
        }

        public async Task<int> OrderManagement_UploadPicture(byte[] imgBytes, string fileName)
        {
            if (imgBytes == null)
                return 0;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                var qqFileNameParameter = "qqfilename";

                if (string.IsNullOrEmpty(fileName) && _httpContextAccessor.HttpContext.Request.Form.ContainsKey(qqFileNameParameter))
                    fileName = _httpContextAccessor.HttpContext.Request.Form[qqFileNameParameter].ToString();

                var mimType = GetMimeTypeFromImageByteArray(imgBytes);

                var fileExtension = fileInfo.Extension;
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();

                var picture = await _pictureService.InsertPictureAsync(imgBytes, mimType, fileName);

                return picture.Id;
            }
            catch { }

            return 0;
        }

        public int OrderManagement_Upload(byte[] imgBytes, string fileName)
        {
            if (imgBytes == null)
                return 0;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                var qqFileNameParameter = "qqfilename";

                if (string.IsNullOrEmpty(fileName) && _httpContextAccessor.HttpContext.Request.Form.ContainsKey(qqFileNameParameter))
                    fileName = _httpContextAccessor.HttpContext.Request.Form[qqFileNameParameter].ToString();

                var contentType = GetMimeTypeFromImageByteArray(imgBytes);

                var fileExtension = fileInfo.Extension;
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();

                var download = new Download
                {
                    DownloadGuid = Guid.NewGuid(),
                    UseDownloadUrl = false,
                    DownloadUrl = string.Empty,
                    DownloadBinary = imgBytes,
                    ContentType = contentType,
                    //we store filename without extension for downloads
                    Filename = Path.GetFileNameWithoutExtension(fileName),
                    Extension = fileExtension,
                    IsNew = true
                };
                _downloadService.InsertDownloadAsync(download);

                return download.Id;
            }
            catch { }

            return 0;
        }


        public async Task SaveSaleOrderAddressAsync(Request request, Customer buyer)
        {
            //existing address
            var address = (await _customerService.GetAddressesByCustomerIdAsync(request.BuyerId)).FirstOrDefault();
            if (address == null)
            {
                //insert default address (if possible)
                address = new Address
                {
                    FirstName = buyer.FirstName,
                    LastName = buyer.LastName,
                    Email = buyer.Email,
                    Company = buyer.Company,
                    CountryId = buyer.CountryId,
                    StateProvinceId = buyer.StateProvinceId,
                    County = buyer.County,
                    City = buyer.City,
                    Address1 = buyer.StreetAddress,
                    Address2 = buyer.StreetAddress2,
                    ZipPostalCode = buyer.ZipPostalCode,
                    PhoneNumber = buyer.Phone,
                    FaxNumber = buyer.Fax,
                    CreatedOnUtc = buyer.CreatedOnUtc
                };

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.InsertAddressAsync(address);
                await _customerService.InsertCustomerAddressAsync(buyer, address);

                buyer.BillingAddressId = address.Id;
                buyer.ShippingAddressId = address.Id;

                await _customerService.UpdateCustomerAsync(buyer);
            }
            else
            {
                //update address
                address.Email = buyer.Email;
                address.FirstName = buyer.FirstName;
                address.LastName = buyer.LastName;
                address.Email = buyer.Email;
                address.Company = buyer.Company;
                address.CountryId = buyer.CountryId;
                address.StateProvinceId = buyer.StateProvinceId;
                address.County = buyer.County;
                address.City = buyer.City;
                address.Address1 = buyer.StreetAddress;
                address.Address2 = buyer.StreetAddress2;
                address.ZipPostalCode = buyer.ZipPostalCode;
                address.PhoneNumber = buyer.Phone;
                address.FaxNumber = buyer.Fax;

                await _addressService.UpdateAddressAsync(address);

                buyer.BillingAddressId = address.Id;
                buyer.ShippingAddressId = address.Id;

                await _customerService.UpdateCustomerAsync(buyer);
            }
        }


        public async Task<IList<string>> SaveSalesOrderCalculationAsync(Order order, Request request, DirectOrderCalculation directOrderCalculation)
        {
            var warnings = new List<string>();

            if (order == null)
            {
                warnings.Add("Order is null");
                return warnings;
            }

            if (request == null)
            {
                warnings.Add("Request is null");
                return warnings;
            }

            if (directOrderCalculation == null)
            {
                warnings.Add("Direct order calculation is null");
                return warnings;
            }

            var orderCalculation = new OrderCalculation();
            orderCalculation.OrderId = order.Id;
            orderCalculation.BusinessModelId = directOrderCalculation.BusinessModelId;
            orderCalculation.SubTotal = directOrderCalculation.SellingPriceOfProduct * request.Quantity;
            orderCalculation.OrderTotal = directOrderCalculation.SellingPriceOfProduct * request.Quantity;

            var taggedInventories = await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id);
            var buyingPrice = await CalculateBuyingPriceByTaggings(taggedInventories.ToList(), directOrderCalculation.GSTRate > 0 || directOrderCalculation.GSTIncludedInTotalAmount);
            var sellingPriceOfProduct = await CalculateSellingPriceOfProductByTaggings(taggedInventories.ToList(), directOrderCalculation.GSTRate > 0 || directOrderCalculation.GSTIncludedInTotalAmount, directOrderCalculation.SellingPriceOfProduct);

            var calculatedBuyingPrice = buyingPrice;
            var calculatedSellingPriceOfProduct = sellingPriceOfProduct;

            var margin = 0m;
            if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                margin = calculatedSellingPriceOfProduct - calculatedBuyingPrice;

            var discount = 0m;
            if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                discount = calculatedSellingPriceOfProduct - calculatedBuyingPrice;

            #region Standard

            if (orderCalculation.BusinessModelId == (int)BusinessModelEnum.Standard)
            {
                var totalReceivable = directOrderCalculation.SellingPriceOfProduct * request.Quantity;

                orderCalculation.ProductPrice = directOrderCalculation.BuyingPrice;
                orderCalculation.SellingPriceOfProduct = directOrderCalculation.SellingPriceOfProduct;

                //Margin Recievable
                if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                {
                    //Old Logic
                    //orderCalculation.MarginAmount = margin;
                    //orderCalculation.MarginRateType = "Value";
                    //orderCalculation.MarginRate = (margin / 100m);

                    //new logic implemented as per meraj alvi on 07-Aug-2023
                    orderCalculation.MarginRateType = "Value";
                    orderCalculation.MarginAmount = margin * request.Quantity;
                    orderCalculation.MarginRate = margin;

                    orderCalculation.NetRateWithMargin = totalReceivable /*+ orderCalculation.MarginAmount*/;

                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    {
                        orderCalculation.MarginAmount = await _priceCalculationService.RoundPriceAsync(orderCalculation.MarginAmount);
                        orderCalculation.NetRateWithMargin = await _priceCalculationService.RoundPriceAsync(orderCalculation.NetRateWithMargin);
                    }
                }
                else
                {
                    orderCalculation.MarginRate = 0;
                    orderCalculation.MarginRateType = "";
                    orderCalculation.MarginAmount = 0;
                    orderCalculation.NetRateWithMargin = totalReceivable;
                }

                //Discount Recievable
                if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                {
                    //Old Logic
                    //orderCalculation.DiscountAmount = discount;
                    //orderCalculation.DiscountRateType = "Value";
                    //orderCalculation.DiscountRate = (discount / 100m);

                    //new logic implemented as per meraj alvi on 07-Aug-2023
                    orderCalculation.DiscountRateType = "Value";
                    orderCalculation.DiscountAmount = discount * request.Quantity;
                    orderCalculation.DiscountRate = discount;

                    orderCalculation.NetRateWithMargin -= Math.Abs(orderCalculation.DiscountAmount);

                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    {
                        orderCalculation.DiscountAmount = await _priceCalculationService.RoundPriceAsync(orderCalculation.DiscountAmount);
                        orderCalculation.NetRateWithMargin = await _priceCalculationService.RoundPriceAsync(orderCalculation.NetRateWithMargin);
                    }
                }
                else
                {
                    orderCalculation.DiscountRate = 0;
                    orderCalculation.DiscountRateType = "";
                    orderCalculation.DiscountAmount = 0;
                }

                // Inclusive of GST
                //if (orderCalculation.GSTIncludedInTotalAmount)
                //{
                //    orderCalculation.GSTIncludedInTotalAmount = true;
                //    orderCalculation.NetAmountWithoutGST = orderCalculation.NetAmount / 1.17m;
                //    totalReceivable = (orderCalculation.ProductPrice * request.Quantity) / 1.17m;
                //}
                //else
                //{
                //    orderCalculation.GSTIncludedInTotalAmount = false;
                //    orderCalculation.NetAmountWithoutGST = totalReceivable;
                //    totalReceivable = orderCalculation.ProductPrice * request.Quantity;
                //}
                orderCalculation.NetAmountWithoutGST = totalReceivable;
                orderCalculation.MarginRateType = directOrderCalculation.MarginRateType == "%" ? "Percent" : "Value";
                orderCalculation.NetRateWithMargin = directOrderCalculation.NetRateWithMargin;
                //orderCalculation.NetAmountWithoutGST = directOrderCalculation.NetAmountWithoutGST;
                //orderCalculation.DiscountRate = directOrderCalculation.DiscountRate;
                orderCalculation.DiscountRateType = directOrderCalculation.DiscountRateType == "%" ? "Percent" : "Value";
                //orderCalculation.DiscountAmount = directOrderCalculation.NetRateWithMargin;
                orderCalculation.GSTRate = directOrderCalculation.GSTRate;
                orderCalculation.GSTAmount = directOrderCalculation.GSTAmount;
                orderCalculation.WHTAmount = directOrderCalculation.WHTAmount;
                orderCalculation.WHTRate = directOrderCalculation.WHTRate;

                orderCalculation.OrderTotal += orderCalculation.GSTAmount;
                orderCalculation.OrderTotal -= orderCalculation.WHTAmount;
                orderCalculation.SubTotal = orderCalculation.OrderTotal;
            }

            #endregion

            #region All Other Model

            if (orderCalculation.BusinessModelId == (int)BusinessModelEnum.OneOnOne || orderCalculation.BusinessModelId == (int)BusinessModelEnum.Agency || orderCalculation.BusinessModelId == (int)BusinessModelEnum.ForwardBuying
                || orderCalculation.BusinessModelId == (int)BusinessModelEnum.ForwardSelling || orderCalculation.BusinessModelId == (int)BusinessModelEnum.FineCounts)
            {
                if (directOrderCalculation.BuyingPrice == 0 && orderCalculation.BusinessModelId != (int)BusinessModelEnum.ForwardSelling)
                {
                    warnings.Add("Buying price is required");
                    return warnings;
                }

                if (directOrderCalculation.SellingPriceOfProduct == 0)
                {
                    warnings.Add("Selling price of product is required");
                    return warnings;
                }

                var gstAmount = 0m;
                var whtAmount = 0m;

                if (directOrderCalculation.GSTRate > 0)
                {
                    var gstRateReceivableCalculationValue = directOrderCalculation.InvoicedAmount * directOrderCalculation.GSTRate / 100;
                    gstAmount = gstRateReceivableCalculationValue;
                }
                else
                    gstAmount = 0m;

                if (directOrderCalculation.WHTRate > 0)
                {
                    var whtRateReceivableCalculationValue = (directOrderCalculation.InvoicedAmount + gstAmount) * directOrderCalculation.WHTRate / 100;
                    whtAmount = whtRateReceivableCalculationValue;
                }
                else
                    whtAmount = 0m;

                var totalReceivableFromBuyer = directOrderCalculation.SellingPriceOfProduct + gstAmount - whtAmount;

                orderCalculation.BrokerId = directOrderCalculation.BrokerId;
                orderCalculation.BuyerCommissionReceivableUserId = directOrderCalculation.BuyerCommissionReceivableUserId;
                orderCalculation.BuyerCommissionPayableUserId = directOrderCalculation.BuyerCommissionPayableUserId;
                orderCalculation.BuyerCommissionReceivablePerBag = directOrderCalculation.BuyerCommissionReceivablePerBag;
                orderCalculation.BuyerCommissionPayablePerBag = directOrderCalculation.BuyerCommissionPayablePerBag;
                orderCalculation.ProductPrice = calculatedBuyingPrice;
                orderCalculation.SellingPriceOfProduct = calculatedSellingPriceOfProduct;
                orderCalculation.MarginRateType = "Value";
                orderCalculation.DiscountRateType = "Value";

                if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                {
                    //Old Logic
                    //orderCalculation.MarginAmount = margin;
                    //orderCalculation.MarginRate = (margin) / 100m;/

                    //new logic implemented as per meraj alvi on 07-Aug-2023
                    orderCalculation.MarginAmount = margin * request.Quantity;
                    orderCalculation.MarginRate = margin;
                }
                else
                {
                    orderCalculation.MarginAmount = 0m;
                    orderCalculation.MarginRate = 0m;
                }

                if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                {
                    //Old Logic
                    //orderCalculation.DiscountAmount = discount;
                    //orderCalculation.DiscountRate = (discount) / 100m;

                    //new logic implemented as per meraj alvi on 07-Aug-2023
                    orderCalculation.DiscountAmount = discount * request.Quantity;
                    orderCalculation.DiscountRate = discount;
                }
                else
                {
                    orderCalculation.DiscountAmount = 0m;
                    orderCalculation.DiscountRate = 0m;
                }
                orderCalculation.TotalPerBag = totalReceivableFromBuyer + directOrderCalculation.BuyerCommissionReceivablePerBag - directOrderCalculation.BuyerCommissionPayablePerBag;

                var totalReceivableFromBuyerAfterMultiply = totalReceivableFromBuyer * request.Quantity;
                var buyerCommissionAfterMultiply = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                var buyerCommission_ReceivableAfterMultiply = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;

                orderCalculation.InvoicedAmount = directOrderCalculation.InvoicedAmount;
                orderCalculation.BrokerCash = directOrderCalculation.BrokerCash;
                orderCalculation.TotalReceivableBuyer = totalReceivableFromBuyerAfterMultiply;
                //orderCalculation.SubTotal = ((decimal)(totalReceivableFromBuyerAfterMultiply + buyerCommission_ReceivableAfterMultiply - buyerCommissionAfterMultiply));
                orderCalculation.SubTotal = totalReceivableFromBuyerAfterMultiply;
                orderCalculation.OrderTotal = orderCalculation.SubTotal;

                orderCalculation.GSTRate = directOrderCalculation.GSTRate;
                orderCalculation.GSTAmount = gstAmount;

                orderCalculation.WHTRate = directOrderCalculation.WHTRate;
                orderCalculation.WHTAmount = whtAmount;

            }

            #endregion

            #region Lending

            if (orderCalculation.BusinessModelId == (int)BusinessModelEnum.Lending)
            {
                if (directOrderCalculation.SellingPriceOfProduct == 0)
                {
                    warnings.Add("Selling price of product is required");
                    return warnings;
                }

                var gstAmount = 0m;
                var whtAmount = 0m;
                var sellingPrice_FinanceIncome = directOrderCalculation.InvoicedAmount + directOrderCalculation.FinanceIncome;

                if (directOrderCalculation.GSTRate > 0)
                {
                    var gstRateReceivableCalculationValue = (decimal)((float)sellingPrice_FinanceIncome * (float)directOrderCalculation.GSTRate / 100);
                    gstAmount = gstRateReceivableCalculationValue;
                }
                else
                    gstAmount = 0m;

                if (directOrderCalculation.WHTRate > 0)
                {
                    var whtRateReceivableCalculationValue = (decimal)((float)(sellingPrice_FinanceIncome + gstAmount) * (float)directOrderCalculation.WHTRate / 100);
                    whtAmount = whtRateReceivableCalculationValue;
                }
                else
                    whtAmount = 0m;

                orderCalculation.BrokerId = directOrderCalculation.BrokerId;
                orderCalculation.BuyerCommissionReceivableUserId = directOrderCalculation.BuyerCommissionReceivableUserId;
                orderCalculation.BuyerCommissionPayableUserId = directOrderCalculation.BuyerCommissionPayableUserId;
                orderCalculation.BuyerPaymentTerms = directOrderCalculation.BuyerPaymentTerms;
                orderCalculation.TotalFinanceCost = directOrderCalculation.TotalFinanceCost;
                orderCalculation.SupplierCreditTerms = directOrderCalculation.SupplierCreditTerms;
                orderCalculation.FinanceCostPayment = directOrderCalculation.FinanceCostPayment;
                orderCalculation.FinanceCost = directOrderCalculation.SupplierCreditTerms > 0 && directOrderCalculation.TotalFinanceCost > 0 ? directOrderCalculation.TotalFinanceCost / directOrderCalculation.SupplierCreditTerms * directOrderCalculation.BuyerPaymentTerms : 0;
                orderCalculation.InvoicedAmount = directOrderCalculation.InvoicedAmount;
                orderCalculation.BrokerCash = directOrderCalculation.BrokerCash;
                orderCalculation.ProductPrice = calculatedBuyingPrice;
                orderCalculation.SellingPriceOfProduct = directOrderCalculation.SellingPriceOfProduct;
                orderCalculation.FinanceIncome = directOrderCalculation.FinanceIncome;
                orderCalculation.BuyerCommissionReceivablePerBag = directOrderCalculation.BuyerCommissionReceivablePerBag;
                orderCalculation.BuyerCommissionPayablePerBag = directOrderCalculation.BuyerCommissionPayablePerBag;

                orderCalculation.SellingPrice_FinanceIncome = directOrderCalculation.SellingPriceOfProduct + directOrderCalculation.FinanceIncome;
                orderCalculation.MarginRateType = "Value";
                orderCalculation.DiscountRateType = "Value";
                if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                {
                    //Old Logic
                    //orderCalculation.MarginAmount = margin;
                    //orderCalculation.MarginRate = (margin) / 100m;

                    //new logic implemented as per meraj alvi on 07-Aug-2023
                    orderCalculation.MarginAmount = margin * request.Quantity;
                    orderCalculation.MarginRate = margin;

                }
                else
                {
                    orderCalculation.MarginAmount = 0m;
                    orderCalculation.MarginRate = 0m;
                }

                if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                {
                    //Old Logic
                    //orderCalculation.DiscountAmount = discount;
                    //orderCalculation.DiscountRate = (discount) / 100m;

                    //new logic implemented as per meraj alvi on 07-Aug-2023
                    orderCalculation.DiscountAmount = discount * request.Quantity;
                    orderCalculation.DiscountRate = discount;
                }
                else
                {
                    orderCalculation.DiscountAmount = 0m;
                    orderCalculation.DiscountRate = 0m;
                }

                orderCalculation.TotalReceivableBuyer = orderCalculation.SellingPrice_FinanceIncome + gstAmount - whtAmount;

                var totalReceivableBuyer_ReceivableAfterMultiply = orderCalculation.TotalReceivableBuyer * request.Quantity;
                var buyerCommission_ReceivableAfterMultiply = orderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                var buyerCommissionAfterMultiply = orderCalculation.BuyerCommissionPayablePerBag * request.Quantity;

                orderCalculation.GSTRate = directOrderCalculation.GSTRate;
                orderCalculation.GSTAmount = gstAmount;

                orderCalculation.WHTRate = directOrderCalculation.WHTRate;
                orderCalculation.WHTAmount = whtAmount;

                orderCalculation.TotalReceivableBuyer = totalReceivableBuyer_ReceivableAfterMultiply;
                //orderCalculation.SubTotal = orderCalculation.TotalReceivableBuyer + buyerCommission_ReceivableAfterMultiply - buyerCommissionAfterMultiply;
                orderCalculation.SubTotal = orderCalculation.TotalReceivableBuyer;
                orderCalculation.OrderTotal = orderCalculation.SubTotal;
            }

            #endregion

            #region Broker

            if (orderCalculation.BusinessModelId == (int)BusinessModelEnum.Broker)
            {
                if (directOrderCalculation.SellingPriceOfProduct <= 0)
                {
                    warnings.Add("Selling price of product is required");
                    return warnings;
                }

                var gstAmount = 0m;
                var whtAmount = 0m;
                var wholesaleTax = 0m;

                if (directOrderCalculation.GSTRate > 0)
                {
                    var gstRateReceivableCalculationValue = directOrderCalculation.InvoicedAmount * directOrderCalculation.GSTRate / 100;
                    gstAmount = gstRateReceivableCalculationValue;
                }
                else
                    gstAmount = 0m;

                if (directOrderCalculation.WHTRate > 0)
                {
                    var whtRateReceivableCalculationValue = (directOrderCalculation.InvoicedAmount + gstAmount) * directOrderCalculation.WHTRate / 100;
                    whtAmount = whtRateReceivableCalculationValue;
                }
                else
                    whtAmount = 0m;

                if (directOrderCalculation.WholesaleTaxRate > 0)
                {
                    var whtRateReceivableCalculationValue = (directOrderCalculation.InvoicedAmount + gstAmount) * directOrderCalculation.WHTRate / 100;
                    wholesaleTax = whtRateReceivableCalculationValue;
                }
                else
                    wholesaleTax = 0m;

                var totalCommissionReceivableFromBuyerToZaraye = margin;

                orderCalculation.BrokerCash = directOrderCalculation.Price - directOrderCalculation.InvoicedAmount;
                var payableToMill = directOrderCalculation.InvoicedAmount + gstAmount - whtAmount + wholesaleTax;
                orderCalculation.PayableToMill = payableToMill;
                var totalReceivableFromBuyerDirectlyToSupplier = payableToMill;
                var payableToMillAfterMultiply = orderCalculation.PayableToMill * request.Quantity;
                orderCalculation.PayableToMill = payableToMillAfterMultiply;

                orderCalculation.BrokerId = directOrderCalculation.BrokerId;
                orderCalculation.BuyerCommissionReceivableUserId = directOrderCalculation.BuyerCommissionReceivableUserId;
                orderCalculation.BuyerCommissionPayableUserId = directOrderCalculation.BuyerCommissionPayableUserId;
                orderCalculation.InvoicedAmount = directOrderCalculation.InvoicedAmount;
                orderCalculation.BuyerCommissionPayablePerBag = directOrderCalculation.BuyerCommissionPayablePerBag;
                orderCalculation.BuyerCommissionReceivablePerBag = directOrderCalculation.BuyerCommissionReceivablePerBag;
                orderCalculation.ProductPrice = calculatedBuyingPrice;
                orderCalculation.SellingPriceOfProduct = calculatedSellingPriceOfProduct;
                orderCalculation.MarginRateType = "Value";
                orderCalculation.DiscountRateType = "Value";
                orderCalculation.WholesaleTaxRate = directOrderCalculation.WholesaleTaxRate;
                orderCalculation.WholesaleTaxAmount = wholesaleTax;
                if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                {
                    //Old Logic
                    //orderCalculation.MarginAmount = margin;
                    //orderCalculation.MarginRate = (margin) / 100m;

                    //new logic implemented as per meraj alvi on 07-Aug-2023
                    orderCalculation.MarginAmount = margin * request.Quantity;
                    orderCalculation.MarginRate = margin;
                }
                else
                {
                    orderCalculation.MarginAmount = 0m;
                    orderCalculation.MarginRate = 0m;
                }

                if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                {
                    //Old Logic
                    //orderCalculation.DiscountAmount = discount;
                    //orderCalculation.DiscountRate = (discount) / 100m;

                    //new logic implemented as per meraj alvi on 07-Aug-2023
                    orderCalculation.DiscountAmount = discount * request.Quantity;
                    orderCalculation.DiscountRate = discount;
                }
                else
                {
                    orderCalculation.DiscountAmount = 0m;
                    orderCalculation.DiscountRate = 0m;
                }
                orderCalculation.TotalPerBag = totalReceivableFromBuyerDirectlyToSupplier + totalCommissionReceivableFromBuyerToZaraye + directOrderCalculation.BuyerCommissionReceivablePerBag - directOrderCalculation.BuyerCommissionPayablePerBag;

                var totalCommissionReceivableFromBuyerToZaraye_ReceivableAfterMultiply = margin * request.Quantity;
                var totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply = payableToMillAfterMultiply;
                var buyerCommissionAfterMultiply = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                var buyerCommission_ReceivableAfterMultiply = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                orderCalculation.TotalReceivableFromBuyerDirectlyToSupplier = totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply;
                orderCalculation.TotalReceivableFromBuyerDirectlyToSupplier = totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply;
                orderCalculation.SubTotal = totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply + totalCommissionReceivableFromBuyerToZaraye_ReceivableAfterMultiply /*+ buyerCommission_ReceivableAfterMultiply - buyerCommissionAfterMultiply*/;
                orderCalculation.GrossAmount = orderCalculation.SubTotal;
                orderCalculation.OrderTotal = buyerCommission_ReceivableAfterMultiply - buyerCommissionAfterMultiply;

                orderCalculation.GSTRate = directOrderCalculation.GSTRate;
                orderCalculation.GSTAmount = gstAmount;

                orderCalculation.WHTRate = directOrderCalculation.WHTRate;
                orderCalculation.WHTAmount = whtAmount;
            }

            #endregion

            if (warnings.Any())
                return warnings;

            if (order.Id > 0)
                await _orderService.InsertOrderCalculationAsync(orderCalculation);

            return warnings;
        }


        public async Task<IList<string>> PrepareSaleOrderCalculationAsync(Order order, Request request, DirectOrderCalculation model)
        {
            var warnings = new List<string>();

            try
            {
                var saleOrderWarnings = await SaveSalesOrderCalculationAsync(new Order(), request, model);
                return saleOrderWarnings;
            }
            catch (Exception exc)
            {
                warnings.Add(exc.Message);
                return warnings;
            }
        }


        public async Task SavePurchaseOrderCalculationAsync(Order order, DirectOrderCalculation directOrderCalculation)
        {
            if (order == null || directOrderCalculation == null)
                return;

            var orderCalculation = new OrderCalculation();
            orderCalculation.OrderId = order.Id;
            orderCalculation.BusinessModelId = directOrderCalculation.BusinessModelId;
            orderCalculation.SubTotal = directOrderCalculation.Price * directOrderCalculation.Quantity;
            orderCalculation.OrderTotal = directOrderCalculation.Price * directOrderCalculation.Quantity;
            var totalPayble = directOrderCalculation.Price * directOrderCalculation.Quantity;

            #region Standard

            if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Standard)
            {
                if (directOrderCalculation.SubTotal == 0)
                {
                    orderCalculation.ProductPrice = directOrderCalculation.Price;
                    orderCalculation.GrossCommissionAmount = totalPayble;
                    orderCalculation.GrossCommissionRate = totalPayble;
                    orderCalculation.NetAmount = totalPayble;
                    orderCalculation.OrderTotal = totalPayble;
                    orderCalculation.GrossAmount = totalPayble;
                    //totalPayble = totalPayble + orderCalculation.GSTAmount - orderCalculation.WHTAmount;
                    orderCalculation.SubTotal = orderCalculation.OrderTotal;

                }
                else
                {
                    orderCalculation.ProductPrice = directOrderCalculation.Price;
                    orderCalculation.GrossAmount = totalPayble;
                    //orderCalculation.GrossCommissionAmount = GrossCommissionRate;
                    //orderCalculation.GrossCommissionRate = directOrderCalculation.GrossCommissionRate;
                    orderCalculation.GrossCommissionRateType = directOrderCalculation.GrossCommissionRateType.ToLower() == "%" ? "Percent" : "Value";

                    //order gross rate commission
                    if (directOrderCalculation.GrossCommissionRate > 0)
                    {
                        orderCalculation.GrossCommissionRate = directOrderCalculation.GrossCommissionRate;

                        if (orderCalculation.GrossCommissionRateType == "Percent")
                            orderCalculation.GrossCommissionAmount = (decimal)((float)totalPayble * (float)directOrderCalculation.GrossCommissionRate / 100f);
                        else
                            orderCalculation.GrossCommissionAmount = directOrderCalculation.GrossCommissionRate;

                        totalPayble -= orderCalculation.GrossCommissionAmount;
                        orderCalculation.NetAmount = totalPayble;

                        if (_shoppingCartSettings.RoundPricesDuringCalculation)
                        {
                            totalPayble = await _priceCalculationService.RoundPriceAsync(totalPayble);
                            orderCalculation.NetAmount = totalPayble;
                        }
                    }
                    else
                    {
                        orderCalculation.GrossCommissionRate = 0;
                        orderCalculation.GrossCommissionRateType = "";
                        orderCalculation.GrossCommissionAmount = 0;
                        orderCalculation.NetAmount = totalPayble;
                    }

                    //Inclusive of GST
                    if (directOrderCalculation.GSTIncludedInTotalAmount)
                    {
                        orderCalculation.GSTIncludedInTotalAmount = true;
                        orderCalculation.NetAmountWithoutGST = (decimal)((float)orderCalculation.NetAmount / (float)_calculationSettings.GSTPercentageForStandardQuotation /*1.17f*/);
                        totalPayble = orderCalculation.NetAmountWithoutGST;
                    }
                    else
                    {
                        orderCalculation.GSTIncludedInTotalAmount = false;
                        orderCalculation.NetAmountWithoutGST = orderCalculation.NetAmount;
                        totalPayble = orderCalculation.NetAmount;
                    }

                    //GST Payable
                    if (directOrderCalculation.GSTRate > 0)
                    {
                        orderCalculation.GSTRate = directOrderCalculation.GSTRate;
                        orderCalculation.GSTAmount = (decimal)((float)orderCalculation.NetAmountWithoutGST * (float)directOrderCalculation.GSTRate / 100f);

                        if (_shoppingCartSettings.RoundPricesDuringCalculation)
                            orderCalculation.GSTAmount = await _priceCalculationService.RoundPriceAsync(orderCalculation.GSTAmount);
                    }
                    else
                    {
                        orderCalculation.GSTRate = 0;
                        orderCalculation.GSTAmount = 0;
                    }

                    //WHT Payable
                    if (directOrderCalculation.WHTRate > 0)
                    {
                        orderCalculation.WHTRate = directOrderCalculation.WHTRate;
                        orderCalculation.WHTAmount = (decimal)((float)(totalPayble + orderCalculation.GSTAmount) * (float)directOrderCalculation.WHTRate / 100f);

                        if (_shoppingCartSettings.RoundPricesDuringCalculation)
                            orderCalculation.WHTAmount = await _priceCalculationService.RoundPriceAsync(orderCalculation.WHTAmount);
                    }
                    else
                    {
                        orderCalculation.WHTRate = 0;
                        orderCalculation.WHTAmount = 0;
                    }

                    totalPayble = totalPayble + orderCalculation.GSTAmount - orderCalculation.WHTAmount;
                    orderCalculation.SubTotal = totalPayble;
                    orderCalculation.SupplierCreditTerms = directOrderCalculation.SupplierCreditTerms;
                    orderCalculation.OrderTotal = orderCalculation.SubTotal;
                }
            }

            #endregion

            #region All other

            else if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.OneOnOne || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Agency || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.FineCounts || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.ForwardBuying || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.ForwardSelling)
            {
                if (directOrderCalculation.Price <= 0)
                    throw new Exception("Product price is required");

                if (directOrderCalculation.InvoicedAmount <= 0)
                    throw new Exception("Invoice amount is required");

                //if (directOrderCalculation.SupplierCommissionBag <= 0)
                //    throw new Exception("Supplier comission bag is required");

                //if (directOrderCalculation.SupplierCommissionReceivableRate <= 0)
                //    throw new Exception("Supplier commission receivable rate is required");

                orderCalculation.BrokerId = directOrderCalculation.BrokerId;
                orderCalculation.SupplierCommissionReceivableUserId = directOrderCalculation.SupplierCommissionReceivableUserId;
                orderCalculation.SupplierCommissionPayableUserId = directOrderCalculation.SupplierCommissionPayableUserId;
                orderCalculation.ProductPrice = directOrderCalculation.Price;
                orderCalculation.InvoicedAmount = directOrderCalculation.InvoicedAmount;
                orderCalculation.BrokerCash = directOrderCalculation.BrokerCash;
                orderCalculation.SupplierCommissionBag = directOrderCalculation.SupplierCommissionBag;
                orderCalculation.SupplierCommissionReceivableRate = directOrderCalculation.SupplierCommissionReceivableRate;
                orderCalculation.GSTAmount = directOrderCalculation.GSTAmount;
                orderCalculation.GSTRate = directOrderCalculation.GSTRate;
                //orderCalculation.GSTIncludedInTotalAmount = Convert.ToBoolean(form[$"GSTIncludedInTotalAmount_{quotation.Id}"]);
                orderCalculation.WHTAmount = directOrderCalculation.WHTAmount;
                orderCalculation.WHTRate = directOrderCalculation.WHTRate;
                orderCalculation.WholesaleTaxAmount = directOrderCalculation.WholesaleTaxAmount;
                orderCalculation.WholesaleTaxRate = directOrderCalculation.WholesaleTaxRate;
                orderCalculation.SupplierCreditTerms = directOrderCalculation.SupplierCreditTerms;
                orderCalculation.PayableToMill = directOrderCalculation.PayableToMill;
                orderCalculation.PaymentInCash = directOrderCalculation.PaymentInCash;
                //orderCalculation.OrderTotal = directOrderCalculation.OrderTotal;
                //orderCalculation.SupplierCommissionReceivableAmount = (decimal)((float)orderCalculation.SupplierCommissionReceivableRate * (float)orderCalculation.InvoicedAmount);
                orderCalculation.SupplierCommissionReceivableAmount = (decimal)((float)orderCalculation.SupplierCommissionReceivableRate * (float)directOrderCalculation.Quantity);
                orderCalculation.SupplierCommissionReceivable_Summary = directOrderCalculation.SupplierCommissionReceivable_Summary;
                //totalPayble = totalPayble + orderCalculation.GSTAmount - orderCalculation.WHTAmount;
                orderCalculation.SubTotal = directOrderCalculation.SubTotal;
                orderCalculation.OrderTotal = orderCalculation.SubTotal;
            }

            #endregion

            #region Lending

            else if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Lending)
            {
                if (directOrderCalculation.Price <= 0)
                    throw new Exception("Product price is required");

                if (directOrderCalculation.InvoicedAmount <= 0)
                    throw new Exception("Invoice amount is required");

                //if (directOrderCalculation.SupplierCommissionBag <= 0)
                //    throw new Exception("Supplier comission bag is required");

                //if (directOrderCalculation.SupplierCommissionReceivableRate <= 0)
                //    throw new Exception("SupplierCommission receivable rate is required");

                //if (directOrderCalculation.BuyerPaymentTerms <= 0)
                //    throw new Exception("Buyer payment terms is required");

                //if (directOrderCalculation.TotalFinanceCost <= 0)
                //    throw new Exception("Total finance cost is required");

                //if (directOrderCalculation.SupplierCreditTerms <= 0)
                //    throw new Exception("Supplier credit terms is required");

                //if (directOrderCalculation.FinanceCostPayment <= 0)
                //    throw new Exception("Finance cost payment is required");

                //if (directOrderCalculation.FinanceCost <= 0)
                //    throw new Exception("Finance cost is required");

                orderCalculation.BrokerId = directOrderCalculation.BrokerId;
                orderCalculation.SupplierCommissionReceivableUserId = directOrderCalculation.SupplierCommissionReceivableUserId;
                orderCalculation.SupplierCommissionPayableUserId = directOrderCalculation.SupplierCommissionPayableUserId;
                orderCalculation.ProductPrice = directOrderCalculation.Price;
                orderCalculation.InvoicedAmount = directOrderCalculation.InvoicedAmount;
                orderCalculation.BrokerCash = directOrderCalculation.BrokerCash;
                orderCalculation.SupplierCommissionBag = directOrderCalculation.SupplierCommissionBag;
                orderCalculation.SupplierCommissionReceivableRate = directOrderCalculation.SupplierCommissionReceivableRate;

                //orderCalculation.BuyerPaymentTerms = directOrderCalculation.BuyerPaymentTerms;
                orderCalculation.TotalFinanceCost = directOrderCalculation.TotalFinanceCost;
                orderCalculation.SupplierCreditTerms = directOrderCalculation.SupplierCreditTerms;
                orderCalculation.FinanceCostPayment = directOrderCalculation.FinanceCostPayment;
                orderCalculation.FinanceCost = directOrderCalculation.FinanceCost;
                //orderCalculation.InterestAccrued = directOrderCalculation.InterestAccrued;
                orderCalculation.FinanceIncome = directOrderCalculation.SellingPrice_FinanceIncome;

                orderCalculation.GSTAmount = directOrderCalculation.GSTAmount;
                orderCalculation.GSTRate = directOrderCalculation.GSTRate;
                //orderCalculation.GSTIncludedInTotalAmount = Convert.ToBoolean(form[$"GSTIncludedInTotalAmount_{quotation.Id}"]);
                orderCalculation.WHTAmount = directOrderCalculation.WHTAmount;
                orderCalculation.WHTRate = directOrderCalculation.WHTRate;
                orderCalculation.WholesaleTaxAmount = directOrderCalculation.WholesaleTaxAmount;
                orderCalculation.WholesaleTaxRate = directOrderCalculation.WholesaleTaxRate;
                orderCalculation.PayableToMill = directOrderCalculation.PayableToMill;
                orderCalculation.PaymentInCash = directOrderCalculation.PaymentInCash;
                //orderCalculation.OrderTotal = directOrderCalculation.OrderTotal;
                //orderCalculation.SupplierCommissionReceivableAmount = (decimal)((float)orderCalculation.SupplierCommissionReceivableRate * (float)orderCalculation.InvoicedAmount);
                orderCalculation.SupplierCommissionReceivableAmount = (decimal)((float)orderCalculation.SupplierCommissionReceivableRate * (float)directOrderCalculation.Quantity);
                orderCalculation.SupplierCommissionReceivable_Summary = directOrderCalculation.SupplierCommissionReceivable_Summary;
                //totalPayble = totalPayble + orderCalculation.GSTAmount - orderCalculation.WHTAmount;
                orderCalculation.SubTotal = directOrderCalculation.SubTotal;
                orderCalculation.OrderTotal = orderCalculation.SubTotal;
            }

            #endregion

            //#region Broker

            //else if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Broker)
            //{
            //    if (directOrderCalculation.Price <= 0)
            //        throw new Exception("Product price is required");

            //    if (directOrderCalculation.InvoicedAmount <= 0)
            //        throw new Exception("Invoice amount is required");

            //    //if (directOrderCalculation.SupplierCommissionBag <= 0)
            //    //    throw new Exception("Supplier comission bag is required");

            //    //if (directOrderCalculation.SupplierCommissionReceivableRate <= 0)
            //    //    throw new Exception("SupplierCommission receivable rate is required");

            //    orderCalculation.BrokerId = directOrderCalculation.BrokerId;
            //    orderCalculation.SupplierCommissionReceivableUserId = directOrderCalculation.SupplierCommissionReceivableUserId;
            //    orderCalculation.SupplierCommissionPayableUserId = directOrderCalculation.SupplierCommissionPayableUserId;
            //    orderCalculation.ProductPrice = directOrderCalculation.Price;
            //    orderCalculation.InvoicedAmount = directOrderCalculation.InvoicedAmount;
            //    orderCalculation.BrokerCash = directOrderCalculation.BrokerCash;
            //    orderCalculation.SupplierCommissionBag = directOrderCalculation.SupplierCommissionBag;
            //    orderCalculation.SupplierCommissionReceivableRate = directOrderCalculation.SupplierCommissionReceivableRate;
            //    orderCalculation.GSTAmount = directOrderCalculation.GSTAmount;
            //    orderCalculation.GSTRate = directOrderCalculation.GSTRate;
            //    orderCalculation.WHTAmount = directOrderCalculation.WHTAmount;
            //    orderCalculation.WHTRate = directOrderCalculation.WHTRate;
            //    orderCalculation.WholesaleTaxAmount = directOrderCalculation.WholesaleTaxAmount;
            //    orderCalculation.WholesaleTaxRate = directOrderCalculation.WholesaleTaxRate;
            //    orderCalculation.PayableToMill = directOrderCalculation.PayableToMill;
            //    orderCalculation.PaymentInCash = directOrderCalculation.PaymentInCash;
            //    orderCalculation.SupplierCommissionReceivableAmount = (decimal)((float)orderCalculation.SupplierCommissionReceivableRate * (float)orderCalculation.InvoicedAmount);
            //    orderCalculation.SupplierCommissionReceivable_Summary = directOrderCalculation.SupplierCommissionReceivable_Summary;
            //    orderCalculation.GrossAmount = directOrderCalculation.SupplierCommissionBag_Summary - directOrderCalculation.SupplierCommissionReceivable_Summary;
            //    orderCalculation.SubTotal = directOrderCalculation.SupplierCommissionBag_Summary - directOrderCalculation.SupplierCommissionReceivable_Summary;
            //    orderCalculation.OrderTotal = orderCalculation.SubTotal;
            //}

            //#endregion

            if (order.Id > 0)
                await _orderService.InsertOrderCalculationAsync(orderCalculation);
        }


        public async Task<string> OrderManagement_ConvertToXmlAsync(List<AttributesModel> attributeDtos, int productId)
        {
            var attributesXml = "";

            if (attributeDtos is not { Count: > 0 })
            {
                return attributesXml;
            }

            var productAttributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"{ZarayeCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            // there should be only one selected value for this attribute
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                int selectedAttributeValue;
                                var isInt = int.TryParse(selectedAttribute.Value, out selectedAttributeValue);
                                if (isInt && selectedAttributeValue > 0)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedAttributeValue.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            // there could be more than one selected value for this attribute
                            var selectedAttributes = attributeDtos.Where(x => x.Name == controlId);
                            foreach (var selectedAttribute in selectedAttributes)
                            {
                                int selectedAttributeValue;
                                var isInt = int.TryParse(selectedAttribute.Value, out selectedAttributeValue);
                                if (isInt && selectedAttributeValue > 0)
                                {
                                    // currently there is no support for attribute quantity
                                    var quantity = 1;
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedAttributeValue.ToString(), quantity);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only(already server - side selected) values
                            var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                                                .Where(v => v.IsPreSelected)
                                                                .Select(v => v.Id)
                                                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttribute.Value);
                            }
                        }
                        break;
                    case AttributeControlType.NumericTextBox:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                            attribute, selectedAttribute.Value);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                DateTime selectedDate;
                                // Since nopCommerce uses this format to keep the date in the database to keep it consisten we will expect the same format to be passed
                                var validDate = DateTime.TryParseExact(selectedAttribute.Value, "D", CultureInfo.CurrentCulture,
                                                                       DateTimeStyles.None, out selectedDate);

                                if (validDate)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, selectedDate.ToString("D"));
                                }
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            var selectedAttribute = attributeDtos.Where(x => x.Name == controlId).FirstOrDefault();
                            if (selectedAttribute != null)
                            {
                                Guid downloadGuid;
                                Guid.TryParse(selectedAttribute.Value, out downloadGuid);
                                var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                                if (download != null)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                                                                                attribute, download.DownloadGuid.ToString());
                                }
                            }
                        }
                        break;
                }
            }

            return attributesXml;
        }


        public async Task<BusinessModelApiModel> SaleOrder_BusinessModelFormCalculatedJson(DirectOrder directOrder)
        {
            var model = new BusinessModelApiModel();

            var directOrdercaluation = await _orderService.GetDirectOrderCalculationByDirectOrderIdAsync(directOrder.Id);
            if (directOrdercaluation is null)
                return new BusinessModelApiModel();

            model.OrderCalculationId = directOrdercaluation.Id;
            //model.SalePrice = directOrdercaluation.SalePrice;
            model.BuyerName = (await _customerService.GetCustomerByIdAsync(directOrder.BuyerId))?.FullName;
            model.Quantity = directOrdercaluation.Quantity;
            model.RequestId = directOrder.RequestId;

            var request = await _requestService.GetRequestByIdAsync(directOrder.RequestId);
            var taggedInventories = await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id);
            var gstRate = directOrdercaluation.GSTRate;
            var salePrice = directOrdercaluation.SellingPriceOfProduct;
            var buyingPrice = await CalculateBuyingPriceByTaggings(taggedInventories.ToList(), gstRate > 0 || directOrdercaluation.GSTIncludedInTotalAmount);
            var sellingPriceOfProduct = await CalculateSellingPriceOfProductByTaggings(taggedInventories.ToList(), gstRate > 0, salePrice);

            directOrdercaluation.BuyingPrice = buyingPrice;

            ////Prepare Buying Price for first time load 
            //var cogsInventoryTaggings = (await _inventoryService.GetAllCogsInventoryTaggingsAsync(requestId: directOrder.RequestId));
            //if (cogsInventoryTaggings.Any())
            //{
            //    var cogsCalculationValue = 0m;
            //    foreach (var cogsInventoryTagging in cogsInventoryTaggings)
            //    {
            //        var inventoryRateQuantity = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
            //        cogsCalculationValue += inventoryRateQuantity;
            //    }
            //    directOrdercaluation.BuyingPrice = cogsCalculationValue / cogsInventoryTaggings.Sum(x => x.Quantity);
            //}

            #region Standard

            if (directOrder.TransactionModelId == (int)BusinessModelEnum.Standard)
            {
                #region Receivable

                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyingPrice",
                    Label = "Buying price",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BuyingPrice,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyingPrice, true, false)
                });
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SellingPriceOfProduct",
                    Label = "Selling price of product",
                    Required = true,
                    IsReadOnly = false,
                    FieldType = "Decimal",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.SellingPriceOfProduct,
                });
                //Net rate without margin
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GrossAmount_Receivable",
                    Label = "Net rate without margin",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.SellingPriceOfProduct > 0 ? directOrdercaluation.SellingPriceOfProduct * directOrdercaluation.Quantity : 0m,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.SellingPriceOfProduct > 0 ? directOrdercaluation.SellingPriceOfProduct * directOrdercaluation.Quantity : 0m, true, false)
                });
                //Margin type
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "MarginType",
                //    Label = "Margin Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "%",
                //    Value = string.IsNullOrWhiteSpace(directOrdercaluation.MarginRateType) ? "%" : directOrdercaluation.MarginRateType,
                //    toggleOptions = new string[2] { "%", "PKR" }
                //});
                //Margin
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "MarginRate",
                    Label = "Margin",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.MarginAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.MarginAmount, true, false),
                });
                //Discount Rate Type
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "DiscountRateType",
                //    Label = "Promo Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "%",
                //    Value = string.IsNullOrWhiteSpace(directOrdercaluation.DiscountRateType) ? "%" : directOrdercaluation.DiscountRateType,
                //    toggleOptions = new string[2] { "%", "PKR" }
                //});
                //Discount Rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "DiscountRate",
                    Label = "Promo",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.DiscountAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.DiscountAmount, true, false),
                });
                //GST Rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTAmount",
                    Label = "GST",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTRate /*> 0 ? directOrdercaluation.GSTRate : ""*/,
                });
                //WHT Rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTAmount",
                    Label = "WHT",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTRate /*> 0 ? directOrdercaluation.WHTRate : ""*/,
                });

                #endregion

            }

            #endregion

            #region All other modules

            else if (directOrder.TransactionModelId == (int)BusinessModelEnum.OneOnOne || directOrder.TransactionModelId == (int)BusinessModelEnum.Agency || directOrder.TransactionModelId == (int)BusinessModelEnum.FineCounts || directOrder.TransactionModelId == (int)BusinessModelEnum.ForwardBuying || directOrder.TransactionModelId == (int)BusinessModelEnum.ForwardSelling)
            {
                #region Receivable

                //Receivable Calculation
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Receivable Calculation",
                    FieldType = "Section",
                });
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyingPrice",
                    Label = "Buying price",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BuyingPrice,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyingPrice, true, false)
                });
                //Selling price of product
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SellingPriceOfProduct",
                    Label = "Selling price of product",
                    IsReadOnly = false,
                    Required = true,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SellingPriceOfProduct > 0 ? directOrdercaluation.SellingPriceOfProduct : "",
                    NeedRequest = true,
                });
                //Invoice Amount
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "InvoicedAmount",
                    Label = "Invoiced amount",
                    IsReadOnly = false,
                    Required = true,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.InvoicedAmount > 0 ? directOrdercaluation.InvoicedAmount : "",
                    NeedRequest = true,
                });
                //BrokerCash
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BrokerCash",
                    Label = "Broker cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BrokerCash,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BrokerCash, true, false)
                });

                //Broker Id
                if (directOrdercaluation.BrokerCash != 0)
                {
                    var brokerId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BrokerRoleName)).Id;
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { brokerId, supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select broker" });
                    foreach (var broker in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true))
                    {
                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(broker)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = broker.Id.ToString(),
                            Text = $"{roles} - {broker.Id}, {broker.FullName}, {broker.Username}",
                            Selected = directOrdercaluation.BrokerId == broker.Id,
                        });
                    };

                    //Broker
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BrokerId",
                        Label = "Broker",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BrokerId,
                        DropdownOptions = options
                    });
                }

                //Buyer commission receivable per bag
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionReceivablePerBag",
                    Label = "Buyer commission receivable per bag",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerCommissionReceivablePerBag /*> 0 ? directOrdercaluation.BuyerCommissionReceivablePerBag : ""*/,
                    NeedRequest = true,
                });

                //Buyer Commission Receivable UserId
                if (directOrdercaluation.BuyerCommissionReceivablePerBag > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.BuyerCommissionReceivableUserId == sb.Id,
                        });
                    };
                    //Buyer commission receivable user
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BuyerCommissionReceivableUserId",
                        Label = "Buyer commission receivable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BuyerCommissionReceivableUserId,
                        DropdownOptions = options
                    });
                }

                //Buyer commission payable per bag
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionPayablePerBag",
                    Label = "Buyer commission payable per bag",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerCommissionPayablePerBag /*> 0 ? directOrdercaluation.BuyerCommissionPayablePerBag : ""*/,
                    NeedRequest = true,
                });

                //Buyer Commission Payable User Id
                if (directOrdercaluation.BuyerCommissionPayablePerBag > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.BuyerCommissionPayableUserId == sb.Id,
                        });
                    };
                    //Buyer commission receivable user
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BuyerCommissionPayableUserId",
                        Label = "Buyer commission payable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BuyerCommissionPayableUserId,
                        DropdownOptions = options
                    });
                }

                //Margin, Promo & Taxation
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Margin, Promo & Taxation",
                    FieldType = "Section",
                });
                //Margin
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "MarginAmount",
                    Label = "Margin",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.MarginAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.MarginAmount, true, false)
                });
                //Promo
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {

                    Name = "DiscountAmount",
                    Label = "Promo",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = -directOrdercaluation.DiscountAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(-directOrdercaluation.DiscountAmount, true, false)
                });
                //GST Type
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "GSTIncludedInTotalAmount",
                //    Label = "GST Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.GSTIncludedInTotalAmount ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true,
                //});
                //GST rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTRate",
                    Label = "Gst rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTRate /*> 0 ? directOrdercaluation.GSTRate : ""*/,
                    NeedRequest = true,
                });

                //GST Receivable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTAmount",
                    Label = "GST",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.GSTAmount, true, false)
                });
                //WHT Type
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "WhtIncluded",
                //    Label = "WHT Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.WhtIncluded ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true,
                //});


                //WHT rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTRate",
                    Label = "Wht rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTRate /*> 0 ? directOrdercaluation.WHTRate : ""*/,
                    NeedRequest = true,
                });
                //WHT Receivable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTAmount",
                    Label = "WHT",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WHTAmount, true, false)
                });

                //Receivables
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Receivables",
                    FieldType = "Section",
                });
                //Receivable in cash
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalReceivableBuyer",
                    Label = "Receivable in cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = (directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync((directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity, true, false),
                });
                //Receivable to zaraye
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalReceivableBuyer",
                    Label = "Receivable to Zaraye",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount) * request.Quantity,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync((directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount) * request.Quantity, true, false),
                });
                //Total receivable buyer
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalReceivableBuyer",
                    Label = "Total receivable buyer",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.TotalReceivableBuyer,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.TotalReceivableBuyer, true, false),
                });
                //Buyer commission payable 
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionPayable_Summary",
                    Label = "Buyer commission payable per bag",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerCommissionPayable_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyerCommissionPayable_Summary, true, false),
                });
                //Buyer commission receivable 
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionReceivable_Summary",
                    Label = "Buyer commission receivable per bag",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerCommissionReceivable_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyerCommissionReceivable_Summary, true, false),
                });
                //Total receivable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "OrderTotal",
                    Label = "Total receivable",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.OrderTotal,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.OrderTotal, true, false),
                });
                #endregion

            }

            #endregion

            #region Lending

            else if (directOrder.TransactionModelId == (int)BusinessModelEnum.Lending)
            {
                #region Receivable
                //Receivable Calculation
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Receivable Calculation",
                    FieldType = "Section",
                });
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyingPrice",
                    Label = "Buying price",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BuyingPrice,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyingPrice, true, false)
                });
                //SellingPriceOfProduct
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SellingPriceOfProduct",
                    Label = "Selling price of product",
                    Required = true,
                    IsReadOnly = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SellingPriceOfProduct > 0 ? directOrdercaluation.SellingPriceOfProduct : "",
                    NeedRequest = true
                });
                //Invoice Amount
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "InvoicedAmount",
                    Label = "Invoiced amount",
                    IsReadOnly = false,
                    Required = true,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.InvoicedAmount > 0 ? directOrdercaluation.InvoicedAmount : "",
                    NeedRequest = true,
                });
                //BrokerCash
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BrokerCash",
                    Label = "Broker cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BrokerCash,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BrokerCash, true, false)
                });
                //Broker Id
                if (directOrdercaluation.BrokerCash != 0)
                {
                    var brokerId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BrokerRoleName)).Id;
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { brokerId, supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select broker" });
                    foreach (var broker in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true))
                    {
                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(broker)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = broker.Id.ToString(),
                            Text = $"{roles} - {broker.Id}, {broker.FullName}, {broker.Username}",
                            Selected = directOrdercaluation.BrokerId == broker.Id,
                        });
                    };
                    //Broker
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BrokerId",
                        Label = "Broker",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BrokerId,
                        DropdownOptions = options
                    });
                }
                //Finance Cost / Return
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Finance Cost / Return",
                    FieldType = "Section",
                });
                //BuyerPaymentTerms
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerPaymentTerms",
                    Label = "Buyer payment terms",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerPaymentTerms > 0 ? directOrdercaluation.BuyerPaymentTerms : "",
                    //ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyerPaymentTerms, true, false)
                    NeedRequest = true
                });
                ////TotalFinanceCost
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "TotalFinanceCost",
                //    Label = "finance cost per bag",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Decimal",
                //    DefaultValue = "0.000",
                //    Value = directOrdercaluation.TotalFinanceCost > 0 ? directOrdercaluation.TotalFinanceCost : "",
                //    NeedRequest = true
                //});
                ////SupplierCreditTerms
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "SupplierCreditTerms",
                //    Label = "Supplier credit terms",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Decimal",
                //    DefaultValue = "0.000",
                //    Value = directOrdercaluation.SupplierCreditTerms > 0 ? directOrdercaluation.SupplierCreditTerms : "",
                //    NeedRequest = true
                //});
                //FinanceCostPayment
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "FinanceCostPayment",
                    Label = "Finance cost payment",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Dropdown",
                    DefaultValue = "0",
                    Value = directOrdercaluation.FinanceCostPayment,
                    DropdownOptions = new List<SelectListItem>() {
                       new SelectListItem { Text = "Select", Value = "0", Selected = directOrdercaluation.FinanceCostPayment == 0},
                       new SelectListItem { Text = "Cash", Value = "10",Selected = directOrdercaluation.FinanceCostPayment == 10},
                       new SelectListItem { Text = "Directly To Mill", Value = "20",Selected = directOrdercaluation.FinanceCostPayment == 20}
                    }
                });
                //FinanceCost
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "FinanceCost",
                    Label = "Finance cost",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.FinanceCost,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.FinanceCost, true, false)
                });

                //BuyerCommissionReceivablePerBag
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionReceivablePerBag",
                    Label = "Buyer commission receivable per bag",
                    Required = true,
                    IsReadOnly = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerCommissionReceivablePerBag /*> 0 ? directOrdercaluation.BuyerCommissionReceivablePerBag : ""*/,
                    NeedRequest = true
                });
                //Buyer Commission Receivable UserId
                if (directOrdercaluation.BuyerCommissionReceivablePerBag > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.BuyerCommissionReceivableUserId == sb.Id,
                        });
                    };
                    //Buyer commission receivable user
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BuyerCommissionReceivableUserId",
                        Label = "Buyer commission receivable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BuyerCommissionReceivableUserId,
                        DropdownOptions = options
                    });
                }
                //BuyerCommissionPayablePerBag
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionPayablePerBag",
                    Label = "Buyer commission payable per bag",
                    Required = true,
                    IsReadOnly = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerCommissionPayablePerBag /*> 0 ? directOrdercaluation.BuyerCommissionPayablePerBag : ""*/,
                    NeedRequest = true
                });
                //Buyer Commission Payable User Id
                if (directOrdercaluation.BuyerCommissionPayablePerBag > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.BuyerCommissionPayableUserId == sb.Id,
                        });
                    };
                    //Buyer commission receivable user
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BuyerCommissionPayableUserId",
                        Label = "Buyer commission payable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BuyerCommissionPayableUserId,
                        DropdownOptions = options
                    });
                }
                //SellingPriceFinanceIncome
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "FinanceIncome",
                    Label = "Finance income",
                    Required = false,
                    IsReadOnly = false,
                    FieldType = "Decimal",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.FinanceIncome,
                    NeedRequest = true,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.FinanceIncome, true, false)
                });
                //SellingPriceFinanceIncome
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SellingPrice_FinanceIncome",
                    Label = "Selling price finance income",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.SellingPrice_FinanceIncome,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.SellingPrice_FinanceIncome, true, false)
                });
                //Margin, Promo & Taxation
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Margin, Promo & Taxation",
                    FieldType = "Section",
                });
                //Margin
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "MarginAmount",
                    Label = "Margin",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.MarginAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.MarginAmount, true, false)
                });
                //Promo
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "DiscountAmount",
                    Label = "Promo",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = -directOrdercaluation.DiscountAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(-directOrdercaluation.DiscountAmount, true, false)
                });
                //GST Type
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "GSTIncludedInTotalAmount",
                //    Label = "GST Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.GSTIncludedInTotalAmount ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true
                //});

                //GST rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTRate",
                    Label = "Gst rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTRate /*> 0 ? directOrdercaluation.GSTRate : ""*/,
                    NeedRequest = true,
                });
                //GST
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTAmount",
                    Label = "GST",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.GSTAmount, true, false)
                });
                //WHT Type
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "WhtIncluded",
                //    Label = "WHT Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.WhtIncluded ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true
                //});

                //WHT rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTRate",
                    Label = "Wht rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTRate /*> 0 ? directOrdercaluation.WHTRate : ""*/,
                    NeedRequest = true,
                });
                //WHT
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTAmount",
                    Label = "WHT",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WHTAmount, true, false)
                });
                //Buyer Calculation Summary
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Buyer Calculation Summary",
                    FieldType = "Section",
                });
                //Receivable in cash
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalReceivableBuyer",
                    Label = "Receivable in cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.FinanceCostPayment == (int)FinanceCostPaymentStatus.Cash ? (directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount + directOrdercaluation.FinanceIncome) * request.Quantity : (directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync((directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity, true, false),
                });
                //Receivable to zaraye
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalReceivableBuyer",
                    Label = "Receivable to Zaraye",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.FinanceCostPayment == (int)FinanceCostPaymentStatus.DirectlyToMill ? (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount + directOrdercaluation.FinanceIncome) * request.Quantity : (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount) * request.Quantity,
                    //Value = (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount),
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.FinanceCostPayment == (int)FinanceCostPaymentStatus.DirectlyToMill ? (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount + directOrdercaluation.FinanceIncome) * request.Quantity : (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount) * request.Quantity, true, false),
                });
                // TotalReceivableBuyer
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalReceivableBuyer",
                    Label = "Total receivable buyer",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.TotalReceivableBuyer,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.TotalReceivableBuyer, true, false)
                });
                //BuyerCommissionReceivable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionReceivable_Summary",
                    Label = "Buyer commission receivable",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BuyerCommissionReceivable_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyerCommissionReceivable_Summary, true, false)
                });
                //BuyerCommissionPayable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionPayable_Summary",
                    Label = "Buyer commission payable",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BuyerCommissionPayable_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyerCommissionPayable_Summary, true, false)
                });
                //TotalReceivable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "OrderTotal",
                    Label = "Total receivable",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.OrderTotal,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.OrderTotal, true, false)
                });
                #endregion
            }

            #endregion

            #region Broker

            else if (directOrder.TransactionModelId == (int)BusinessModelEnum.Broker)
            {
                #region Receivable
                //Receivable Calculation
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Receivable Calculation",
                    FieldType = "Section",
                });
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyingPrice",
                    Label = "Buying price",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BuyingPrice,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyingPrice, true, false)
                });
                //SellingPriceOfProduct
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SellingPriceOfProduct",
                    Label = "Selling price of product",
                    Required = true,
                    IsReadOnly = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SellingPriceOfProduct > 0 ? directOrdercaluation.SellingPriceOfProduct : "",
                    NeedRequest = true
                });
                //InvoicedAmount
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "InvoicedAmount",
                    Label = "Invoiced amount",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.InvoicedAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.InvoicedAmount, true, false)
                });
                //BrokerCash
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "BrokerCash",
                //    Label = "Broker cash",
                //    IsReadOnly = true,
                //    Required = false,
                //    FieldType = "Label",
                //    DefaultValue = "PKR 0.000",
                //    Value = directOrdercaluation.BrokerCash,
                //    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BrokerCash, true, false)
                //});
                //Broker Id
                if (directOrdercaluation.BrokerCash != 0)
                {
                    var brokerId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BrokerRoleName)).Id;
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { brokerId, supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select broker" });
                    foreach (var broker in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true))
                    {
                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(broker)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = broker.Id.ToString(),
                            Text = $"{roles} - {broker.Id}, {broker.FullName}, {broker.Username}",
                            Selected = directOrdercaluation.BrokerId == broker.Id,
                        });
                    };
                    //Broker
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BrokerId",
                        Label = "Broker",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BrokerId,
                        DropdownOptions = options
                    });
                }
                //BuyerCommissionReceivablePerBag
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionReceivablePerBag",
                    Label = "Buyer commission receivable per bag",
                    Required = false,
                    IsReadOnly = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerCommissionReceivablePerBag /*> 0 ? directOrdercaluation.BuyerCommissionReceivablePerBag : ""*/,
                    NeedRequest = true
                });
                //Buyer Commission Receivable UserId
                if (directOrdercaluation.BuyerCommissionReceivablePerBag > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.BuyerCommissionReceivableUserId == sb.Id,
                        });
                    };
                    //Buyer commission receivable user
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BuyerCommissionReceivableUserId",
                        Label = "Buyer commission receivable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BuyerCommissionReceivableUserId,
                        DropdownOptions = options
                    });
                }

                //BuyerCommissionPayablePerBag
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionPayablePerBag",
                    Label = "Buyer commission payable per bag",
                    Required = false,
                    IsReadOnly = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.BuyerCommissionPayablePerBag /*> 0 ? directOrdercaluation.BuyerCommissionPayablePerBag : ""*/,
                    NeedRequest = true
                });
                //Buyer Commission Payable User Id
                if (directOrdercaluation.BuyerCommissionPayablePerBag > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.BuyerCommissionPayableUserId == sb.Id,
                        });
                    };
                    //Buyer commission receivable user
                    model.Receivables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BuyerCommissionPayableUserId",
                        Label = "Buyer commission payable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BuyerCommissionPayableUserId,
                        DropdownOptions = options
                    });
                }
                //Margin, Promo & Taxation
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Margin, Promo & Taxation",
                    FieldType = "Section",
                });
                //Margin
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "MarginAmount",
                    Label = "Margin",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.MarginAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.MarginAmount, true, false)
                });
                //Promo
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "DiscountAmount",
                    Label = "Promo",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = -directOrdercaluation.DiscountAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(-directOrdercaluation.DiscountAmount, true, false)
                });
                //GST Type
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "GSTIncludedInTotalAmount",
                //    Label = "GST Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.GSTIncludedInTotalAmount ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true
                //});
                //GST rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTRate",
                    Label = "Gst rate %",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTRate /*> 0 ? directOrdercaluation.GSTRate : ""*/,
                    NeedRequest = true,
                });
                //GST
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTAmount",
                    Label = "GST",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.GSTAmount, true, false)
                });
                //WHT Type
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "WhtIncluded",
                //    Label = "WHT Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.WhtIncluded ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true
                //});
                //WHT rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTRate",
                    Label = "Wht rate %",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTRate/* > 0 ? directOrdercaluation.WHTRate : ""*/,
                    NeedRequest = true,
                });
                //WHT
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTAmount",
                    Label = "WHT",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WHTAmount, true, false)
                });
                //Wholesale tax rate
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WholesaleTaxRate",
                    Label = "Wholesale tax rate %",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WholesaleTaxRate/* > 0 ? directOrdercaluation.WholesaleTaxRate : ""*/,
                    NeedRequest = true,
                });
                //Wholesale tax
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WholesaleTaxAmount",
                    Label = "Wholesale tax",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WholesaleTaxAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WholesaleTaxAmount, true, false)
                });
                //Receivables
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Receivables",
                    FieldType = "Section",
                });
                //Receivable in cash
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalReceivableBuyer",
                    Label = "Receivable in cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = (directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync((directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity, true, false),
                });
                ////Receivable to zaraye
                //model.Receivables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "TotalReceivableBuyer",
                //    Label = "Receivable to Zaraye",
                //    IsReadOnly = true,
                //    Required = false,
                //    FieldType = "Label",
                //    DefaultValue = "0.000",
                //    Value = (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount),
                //    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount, true, false),
                //});
                //TotalReceivableFromBuyerDirectlyToSupplier
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalReceivableFromBuyerDirectlyToSupplier",
                    Label = "Total receivable from buyer directly to supplier",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.TotalReceivableFromBuyerDirectlyToSupplier,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.TotalReceivableFromBuyerDirectlyToSupplier, true, false)
                });
                //TotalCommissionReceivableFromBuyerToZaraye
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalCommissionReceivableFromBuyerToZaraye",
                    Label = "Total commission receivable from buyer to zaraye",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.TotalCommissionReceivableFromBuyerToZaraye,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.TotalCommissionReceivableFromBuyerToZaraye, true, false)
                });
                //BuyerCommissionReceivable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionReceivable_Summary",
                    Label = "Buyer commission receivable",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BuyerCommissionReceivable_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyerCommissionReceivable_Summary, true, false)
                });
                //BuyerCommissionPayable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BuyerCommissionPayable_Summary",
                    Label = "Buyer commission payable",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BuyerCommissionPayable_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BuyerCommissionPayable_Summary, true, false)
                });
                //TotalReceivable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GrossAmount",
                    Label = "Gross receivable",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.GrossAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.GrossAmount, true, false)
                });
                //TotalReceivable
                model.Receivables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "OrderTotal",
                    Label = "Total receivable",
                    Required = false,
                    IsReadOnly = true,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.OrderTotal,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.OrderTotal, true, false)
                });
                #endregion
            }

            #endregion

            return model;
        }


        public async Task<BusinessModelApiModel> PurchaseOrder_BusinessModelFormCalculatedJson(DirectOrder directOrder)
        {
            var model = new BusinessModelApiModel();

            var directOrdercaluation = await _orderService.GetDirectOrderCalculationByDirectOrderIdAsync(directOrder.Id);
            if (directOrdercaluation is null)
                return new BusinessModelApiModel();

            model.OrderCalculationId = directOrdercaluation.Id;
            //model.SalePrice = directOrdercaluation.SalePrice;
            model.SupplierName = (await _customerService.GetCustomerByIdAsync(directOrder.SupplierId))?.FullName;
            model.Quantity = directOrdercaluation.Quantity;
            model.RequestId = directOrder.RequestId;
            model.QuotationId = directOrder.QuotationId;
            model.DirectOrderId = directOrder.Id;
            model.BusinessModelName = await _localizationService.GetLocalizedEnumAsync(directOrdercaluation.BusinessModelEnum);

            #region Standard

            if (directOrdercaluation.BusinessModelId == (int)BusinessModelEnum.Standard)
            {
                var grossAmount = directOrdercaluation.Price * directOrdercaluation.Quantity;
                //Gross rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GrossRate",
                    Label = "Gross rate",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = grossAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(grossAmount, true, false)
                });

                //Commission Type
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "CommissionType",
                    Label = "Commission Type",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Toggle",
                    DefaultValue = "%",
                    Value = string.IsNullOrWhiteSpace(directOrdercaluation.GrossCommissionRateType) ? "%" : directOrdercaluation.GrossCommissionRateType,
                    toggleOptions = new string[2] { "%", "PKR" }
                });
                //Commission
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "Commission",
                    Label = "Commission",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GrossCommissionRate /*> 0 ? directOrdercaluation.GrossCommissionRate : ""*/,
                });
                //Inclusive of GST
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTIncludedInTotalAmount",
                    Label = "Inclusive of GST",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Toggle",
                    DefaultValue = "No",
                    Value = directOrdercaluation.GSTIncludedInTotalAmount ? "Yes" : "No",
                    toggleOptions = new string[2] { "Yes", "No" },
                    NeedRequest = true
                });
                //Supplier Credit Terms 
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCreditTerms",
                    Label = "Supplier credit terms",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCreditTerms > 0 ? directOrdercaluation.SupplierCreditTerms : "",
                    NeedRequest = true,
                });
                //GST Rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTRate",
                    Label = "GST",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTRate /*> 0 ? directOrdercaluation.GSTRate : ""*/,
                });
                //WHT Rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTRate",
                    Label = "WHT",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTRate/* > 0 ? directOrdercaluation.WHTRate : ""*/,
                });
            }

            #endregion

            #region All other modules

            else if (directOrdercaluation.BusinessModelId == (int)BusinessModelEnum.OneOnOne || directOrdercaluation.BusinessModelId == (int)BusinessModelEnum.Agency || directOrdercaluation.BusinessModelId == (int)BusinessModelEnum.FineCounts || directOrdercaluation.BusinessModelId == (int)BusinessModelEnum.ForwardBuying || directOrdercaluation.BusinessModelId == (int)BusinessModelEnum.ForwardSelling)
            {
                #region Payable
                //Supplier Calculation
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Supplier Calculation",
                    FieldType = "Section",
                });
                //Product price
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "Price",
                    Label = "Product price",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.Price,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.Price, true, false),
                });
                //Invoice Amount
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "InvoicedAmount",
                    Label = "Invoiced amount",
                    IsReadOnly = false,
                    Required = true,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.InvoicedAmount > 0 ? directOrdercaluation.InvoicedAmount : "",
                    NeedRequest = true,
                });
                //Broker cash
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BrokerCash",
                    Label = "Broker cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "false",
                    Value = directOrdercaluation.BrokerCash,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BrokerCash, true, false)
                });
                //Broker Id
                if (directOrdercaluation.BrokerCash != 0)
                {
                    var brokerId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BrokerRoleName)).Id;
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { brokerId, supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select broker" });
                    foreach (var broker in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true))
                    {
                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(broker)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = broker.Id.ToString(),
                            Text = $"{roles} - {broker.Id}, {broker.FullName}, {broker.Username}",
                            Selected = directOrdercaluation.BrokerId == broker.Id,
                        });
                    };
                    //Broker
                    model.Payables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BrokerId",
                        Label = "Broker",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BrokerId,
                        DropdownOptions = options
                    });
                }
                //Supplier Credit Terms 
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCreditTerms",
                    Label = "Supplier credit terms",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCreditTerms > 0 ? directOrdercaluation.SupplierCreditTerms : "",
                    NeedRequest = true,
                });
                //Supplier commission payable/bag
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCommissionBag",
                    Label = "Supplier commission payable/bag",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCommissionBag /*> 0 ? directOrdercaluation.SupplierCommissionBag : ""*/,
                    NeedRequest = true,
                });
                //Supplier Commission Payable UserId
                if (directOrdercaluation.SupplierCommissionBag > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.SupplierCommissionPayableUserId == sb.Id,
                        });
                    };
                    //Supplier commission receivable user
                    model.Payables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "SupplierCommissionPayableUserId",
                        Label = "Supplier commission payable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.SupplierCommissionPayableUserId,
                        DropdownOptions = options
                    });
                }
                //Supplier commission receivable %
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCommissionReceivableRate",
                    Label = "Supplier commission receivable per bag",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCommissionReceivableRate/* > 0 ? directOrdercaluation.SupplierCommissionReceivableRate : ""*/,
                });

                //Supplier Commission Receivable UserId
                if (directOrdercaluation.SupplierCommissionReceivableRate > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.SupplierCommissionReceivableUserId == sb.Id,
                        });
                    };
                    //Supplier commission receivable user
                    model.Payables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "SupplierCommissionReceivableUserId",
                        Label = "Supplier commission receivable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.SupplierCommissionReceivableUserId,
                        DropdownOptions = options
                    });
                }
                //Taxation
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Taxation",
                    FieldType = "Section",
                });

                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "GSTIncludedInTotalAmount",
                //    Label = "GST Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.GSTIncludedInTotalAmount ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true,
                //});
                //GST rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTRate",
                    Label = "Gst rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTRate /*> 0 ? directOrdercaluation.GSTRate : ""*/,
                    NeedRequest = true,
                });
                //GST
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTAmount",
                    Label = "GST",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.GSTAmount, true, false)
                });
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "WhtIncluded",
                //    Label = "WHT Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.WhtIncluded ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true,
                //});
                //WHT rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTRate",
                    Label = "Wht rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTRate /*> 0 ? directOrdercaluation.WHTRate : ""*/,
                    NeedRequest = true,
                });
                //WHT
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTAmount",
                    Label = "WHT",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WHTAmount, true, false)
                });
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "WholesaletaxIncluded",
                //    Label = "WholeSale tax type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.WholesaletaxIncluded ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true,
                //});

                //Wholesale tax rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WholesaleTaxRate",
                    Label = "Wholesale tax rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WholesaleTaxRate/* > 0 ? directOrdercaluation.WholesaleTaxRate : ""*/,
                    NeedRequest = true,
                });
                //Wholesale tax
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WholesaleTaxAmount",
                    Label = "Wholesale tax",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WholesaleTaxAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WholesaleTaxAmount, true, false)
                });

                //Supplier Calculation Summary
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Supplier Calculation Summary",
                    FieldType = "Section",
                });
                //Payable to mill
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "PayableToMill",
                    Label = "Payable to mill",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.PayableToMill,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.PayableToMill, true, false)
                });
                //Payment in cash
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "PaymentInCash",
                    Label = "Payment in cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.PaymentInCash,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.PaymentInCash, true, false)
                });
                //Total payable
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "OrderTotal",
                    Label = "Total payable",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.OrderTotal,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.OrderTotal, true, false)
                });
                //Supplier commission payable
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCommissionBag_Summary",
                    Label = "Supplier commission payable",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCommissionBag_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.SupplierCommissionBag_Summary, true, false)
                });
                //Supplier commission receivable
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCommissionReceivable_Summary",
                    Label = "Supplier commission receivable",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCommissionReceivable_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.SupplierCommissionReceivable_Summary, true, false)
                });
                #endregion
            }

            #endregion

            #region Lending

            else if (directOrdercaluation.BusinessModelId == (int)BusinessModelEnum.Lending)
            {
                #region Payable

                //Supplier Calculation
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Supplier Calculation",
                    FieldType = "Section",
                });
                //Product price
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "Price",
                    Label = "Product price",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.Price,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.Price, true, false),
                });
                //InvoicedAmount
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "InvoicedAmount",
                    Label = "Invoiced amount",
                    IsReadOnly = false,
                    Required = true,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.InvoicedAmount > 0 ? directOrdercaluation.InvoicedAmount : "",
                    NeedRequest = true
                });
                //BrokerCash
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "BrokerCash",
                    Label = "Broker cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.BrokerCash,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BrokerCash, true, false)
                });
                //Broker Id
                if (directOrdercaluation.BrokerCash != 0)
                {
                    var brokerId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BrokerRoleName)).Id;
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { brokerId, supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select broker" });
                    foreach (var broker in await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds, isActive: true))
                    {
                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(broker)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = broker.Id.ToString(),
                            Text = $"{roles} - {broker.Id}, {broker.FullName}, {broker.Username}",
                            Selected = directOrdercaluation.BrokerId == broker.Id,
                        });
                    };
                    //Broker
                    model.Payables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "BrokerId",
                        Label = "Broker",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.BrokerId,
                        DropdownOptions = options
                    });
                }
                //SupplierCommissionPayableBag
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCommissionBag",
                    Label = "Supplier commission payable/bag",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCommissionBag /*> 0 ? directOrdercaluation.SupplierCommissionBag : ""*/,
                    NeedRequest = true
                });
                //Supplier Commission Payable UserId
                if (directOrdercaluation.SupplierCommissionBag > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    var customerRoleIds = new int[] { supplierRoleId, buyerRoleId };

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { supplierRoleId, buyerRoleId }, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        //var role = "";
                        //var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        //if (customerRoles.Any())
                        //{
                        //    if (customerRoles.Any(x => x.Id == supplierRoleId))
                        //        role = "Supplier";

                        //    if (customerRoles.Any(x => x.Id == buyerRoleId))
                        //        role = "Buyer";
                        //}

                        var roles = string.Join(", ", (await _customerService.GetCustomerRolesAsync(sb)).Where(x => customerRoleIds.Contains(x.Id)).Select(role => role.Name));

                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{roles} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.SupplierCommissionPayableUserId == sb.Id,
                        });
                    };
                    //Supplier commission receivable user
                    model.Payables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "SupplierCommissionPayableUserId",
                        Label = "Supplier commission payable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.SupplierCommissionPayableUserId,
                        DropdownOptions = options
                    });
                }
                //SupplierCommissionReceivablePercent
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCommissionReceivableRate",
                    Label = "Supplier commission receivable per bag",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCommissionReceivableRate /*> 0 ? directOrdercaluation.SupplierCommissionReceivableRate : ""*/,
                    NeedRequest = true
                });
                //Supplier Commission Receivable UserId
                if (directOrdercaluation.SupplierCommissionReceivableRate > 0)
                {
                    var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id;
                    var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id;

                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem { Value = "0", Text = "Select supplier / buyer" });
                    foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { supplierRoleId, buyerRoleId }, isActive: true, pageIndex: 0, pageSize: int.MaxValue))
                    {
                        var role = "";
                        var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
                        if (customerRoles.Any())
                        {
                            if (customerRoles.Any(x => x.Id == supplierRoleId))
                                role = "Supplier";

                            if (customerRoles.Any(x => x.Id == buyerRoleId))
                                role = "Buyer";
                        }
                        options.Add(new SelectListItem
                        {
                            Value = sb.Id.ToString(),
                            Text = $"{role} - {sb.Id},{sb.FullName},{sb.Username}",
                            Selected = directOrdercaluation.SupplierCommissionReceivableUserId == sb.Id,
                        });
                    };
                    //Supplier commission receivable user
                    model.Payables.Add(new BusinessModelApiModel.Fields
                    {
                        Name = "SupplierCommissionReceivableUserId",
                        Label = "Supplier commission receivable user",
                        IsReadOnly = false,
                        Required = true,
                        FieldType = "Dropdown",
                        DefaultValue = "0",
                        Value = directOrdercaluation.SupplierCommissionReceivableUserId,
                        DropdownOptions = options
                    });
                }
                //Finance Cost / Return
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Finance Cost / Return",
                    FieldType = "Section",
                });
                ////BuyerPaymentTerms
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "BuyerPaymentTerms",
                //    Label = "Buyer payment terms",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Decimal",
                //    DefaultValue = "0.000",
                //    Value = directOrdercaluation.BuyerPaymentTerms > 0 ? directOrdercaluation.BuyerPaymentTerms : "",
                //    NeedRequest = true
                //});
                //TotalFinanceCost
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "TotalFinanceCost",
                    Label = "Total finance cost",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.TotalFinanceCost > 0 ? directOrdercaluation.TotalFinanceCost : "",
                    NeedRequest = true
                });
                //SupplierCreditTerms
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCreditTerms",
                    Label = "Supplier credit terms",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.SupplierCreditTerms > 0 ? directOrdercaluation.SupplierCreditTerms : "",
                    NeedRequest = true
                });
                //FinanceCostPayment
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "FinanceCostPayment",
                    Label = "Finance cost payment",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Dropdown",
                    DefaultValue = "0",
                    Value = directOrdercaluation.FinanceCostPayment,
                    DropdownOptions = new List<SelectListItem>() {
                       new SelectListItem { Text = "Select", Value = "0", Selected = directOrdercaluation.FinanceCostPayment == 0},
                       new SelectListItem { Text = "Cash", Value = "10",Selected = directOrdercaluation.FinanceCostPayment == 10},
                       new SelectListItem { Text = "Directly To Mill", Value = "20",Selected = directOrdercaluation.FinanceCostPayment == 20}
                    }
                });
                //FinanceCost
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "FinanceCost",
                    Label = "Finance cost",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "PKR 0.000",
                    Value = directOrdercaluation.FinanceCost,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.FinanceCost, true, false)
                });
                ////Interest Accrued
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "InterestAccrued",
                //    Label = "Interest accrued",
                //    IsReadOnly = true,
                //    Required = false,
                //    FieldType = "Label",
                //    DefaultValue = "0",
                //    Value = directOrdercaluation.InterestAccrued,
                //    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.InterestAccrued, true, false)
                //});
                ////Finance Income
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "SellingPrice_FinanceIncome",
                //    Label = "Finance income",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Decimal",
                //    DefaultValue = "0.0000",
                //    Value = directOrdercaluation.SellingPrice_FinanceIncome > 0 ? directOrdercaluation.SellingPrice_FinanceIncome : "",
                //    NeedRequest = true
                //});
                //Taxation
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Taxation",
                    FieldType = "Section",
                });
                ////GST Type
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "GSTIncludedInTotalAmount",
                //    Label = "GST Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.GSTIncludedInTotalAmount ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true
                //});
                //GST rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTRate",
                    Label = "Gst rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTRate /*> 0 ? directOrdercaluation.GSTRate : ""*/,
                    NeedRequest = true,
                });
                //GST
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "GSTAmount",
                    Label = "GST",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.GSTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.GSTAmount, true, false)
                });
                ////WHT Type
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "WhtIncluded",
                //    Label = "WHT Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.WhtIncluded ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true
                //});
                //WHT rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTRate",
                    Label = "Wht rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTRate /*> 0 ? directOrdercaluation.WHTRate : ""*/,
                    NeedRequest = true,
                });

                //WHT
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WHTAmount",
                    Label = "WHT",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WHTAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WHTAmount, true, false)
                });
                ////Wholesale tax Type
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "WholesaletaxIncluded",
                //    Label = "Wholesale tax Type",
                //    IsReadOnly = false,
                //    Required = false,
                //    FieldType = "Toggle",
                //    DefaultValue = "No",
                //    Value = directOrdercaluation.WholesaletaxIncluded ? "Yes" : "No",
                //    toggleOptions = new string[2] { "Yes", "No" },
                //    NeedRequest = true
                //});
                //Wholesale tax rate
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WholesaleTaxRate",
                    Label = "Wholesale tax rate %",
                    IsReadOnly = false,
                    Required = false,
                    FieldType = "Decimal",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WholesaleTaxRate /*> 0 ? directOrdercaluation.WholesaleTaxRate : ""*/,
                    NeedRequest = true,
                });
                //Wholesale tax
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "WholesaleTaxAmount",
                    Label = "Wholesale tax",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.000",
                    Value = directOrdercaluation.WholesaleTaxAmount,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WholesaleTaxAmount, true, false)
                });
                //Supplier Calculation Summary
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Label = "Supplier Calculation Summary",
                    FieldType = "Section",
                });
                //PayableToMill
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "PayableToMill",
                    Label = "Payable to mill",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.0000",
                    Value = directOrdercaluation.PayableToMill,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.PayableToMill, true, false)
                });
                //PaymentInCash
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "PaymentInCash",
                    Label = "Payment in cash",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.0000",
                    Value = directOrdercaluation.PaymentInCash,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.PaymentInCash, true, false)
                });
                //InterestAccruedInSummary
                //model.Payables.Add(new BusinessModelApiModel.Fields
                //{
                //    Name = "InterestAccrued_Summary",
                //    Label = "Interest accrued",
                //    IsReadOnly = true,
                //    Required = false,
                //    FieldType = "Label",
                //    DefaultValue = "0",
                //    Value = directOrdercaluation.InterestAccrued_Summary,
                //    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.InterestAccrued_Summary, true, false)
                //});
                //TotalPayable
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "OrderTotal",
                    Label = "Total payable",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.0000",
                    Value = directOrdercaluation.OrderTotal,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.OrderTotal, true, false)
                });
                // SupplierCommissionPayable
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCommissionBag_Summary",
                    Label = "Supplier commission payable",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.0000",
                    Value = directOrdercaluation.SupplierCommissionBag_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.SupplierCommissionBag_Summary, true, false)
                });
                //SupplierCommissionReceivable
                model.Payables.Add(new BusinessModelApiModel.Fields
                {
                    Name = "SupplierCommissionReceivable_Summary",
                    Label = "Supplier commission receivable",
                    IsReadOnly = true,
                    Required = false,
                    FieldType = "Label",
                    DefaultValue = "0.0000",
                    Value = directOrdercaluation.SupplierCommissionReceivable_Summary,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.SupplierCommissionReceivable_Summary, true, false)
                });

                #endregion
            }

            #endregion

            //#region Broker

            //else if (directOrdercaluation.BusinessModelId == (int)BusinessModelEnum.Broker)
            //{
            //    #region Payable
            //    //Supplier Calculation
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Label = "Supplier Calculation",
            //        FieldType = "Section",
            //    });
            //    //InvoicedAmount
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "InvoicedAmount",
            //        Label = "Invoiced amount",
            //        IsReadOnly = false,
            //        Required = true,
            //        FieldType = "Decimal",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.InvoicedAmount > 0 ? directOrdercaluation.InvoicedAmount : "",
            //        NeedRequest = true
            //    });
            //    //BrokerCash
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "BrokerCash",
            //        Label = "Broker cash",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "PKR 0.000",
            //        Value = directOrdercaluation.BrokerCash,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.BrokerCash, true, false)
            //    });
            //    //Broker Id
            //    if (directOrdercaluation.BrokerCash != 0)
            //    {
            //        List<SelectListItem> options = new List<SelectListItem>();
            //        options.Add(new SelectListItem { Value = "0", Text = "Select broker" });
            //        foreach (var broker in await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.BrokerRoleName)).Id }, isActive: true, pageIndex: 0, pageSize: 15))
            //        {
            //            options.Add(new SelectListItem
            //            {
            //                Value = broker.Id.ToString(),
            //                Text = $" {broker.Id}, {broker.FullName}, {broker.Username}",
            //                Selected = directOrdercaluation.BrokerId == broker.Id,
            //            });
            //        };
            //        //Broker
            //        model.Payables.Add(new BusinessModelApiModel.Fields
            //        {
            //            Name = "BrokerId",
            //            Label = "Broker",
            //            IsReadOnly = false,
            //            Required = true,
            //            FieldType = "Dropdown",
            //            DefaultValue = "0",
            //            Value = directOrdercaluation.BrokerId,
            //            DropdownOptions = options
            //        });
            //    }
            //    //SupplierCommissionPayableBag
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "SupplierCommissionBag",
            //        Label = "Supplier commission payable/bag",
            //        IsReadOnly = false,
            //        Required = false,
            //        FieldType = "Decimal",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.SupplierCommissionBag > 0 ? directOrdercaluation.SupplierCommissionBag : "",
            //        NeedRequest = true
            //    });
            //    //Supplier Commission Payable UserId
            //    if (directOrdercaluation.SupplierCommissionBag > 0)
            //    {
            //        var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.SupplierRoleName)).Id;
            //        var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.BuyerRoleName)).Id;

            //        List<SelectListItem> options = new List<SelectListItem>();
            //        options.Add(new SelectListItem { Value = "0", Text = "Select supplier/buyer" });
            //        foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { supplierRoleId, buyerRoleId }, isActive: true, pageIndex: 0, pageSize: 15))
            //        {
            //            var role = "";
            //            var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
            //            if (customerRoles.Any())
            //            {
            //                if (customerRoles.Any(x => x.Id == supplierRoleId))
            //                    role = "Supplier";

            //                if (customerRoles.Any(x => x.Id == buyerRoleId))
            //                    role = "Buyer";
            //            }
            //            options.Add(new SelectListItem
            //            {
            //                Value = sb.Id.ToString(),
            //                Text = $"{role} - {sb.Id},{sb.FullName},{sb.Username}",
            //                Selected = directOrdercaluation.SupplierCommissionPayableUserId == sb.Id,
            //            });
            //        };
            //        //Supplier commission receivable user
            //        model.Payables.Add(new BusinessModelApiModel.Fields
            //        {
            //            Name = "SupplierCommissionPayableUserId",
            //            Label = "Supplier commission payable user",
            //            IsReadOnly = false,
            //            Required = true,
            //            FieldType = "Dropdown",
            //            DefaultValue = "0",
            //            Value = directOrdercaluation.SupplierCommissionPayableUserId,
            //            DropdownOptions = options
            //        });
            //    }
            //    //SupplierCommissionReceivablePercent
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "SupplierCommissionReceivableRate",
            //        Label = "Supplier commission receivable %",
            //        IsReadOnly = false,
            //        Required = false,
            //        FieldType = "Decimal",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.SupplierCommissionReceivableRate > 0 ? directOrdercaluation.SupplierCommissionReceivableRate : "",
            //        NeedRequest = true
            //    });
            //    //Supplier Commission Receivable UserId
            //    if (directOrdercaluation.SupplierCommissionReceivableRate > 0)
            //    {
            //        var supplierRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.SupplierRoleName)).Id;
            //        var buyerRoleId = (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.BuyerRoleName)).Id;

            //        List<SelectListItem> options = new List<SelectListItem>();
            //        options.Add(new SelectListItem { Value = "0", Text = "Select supplier/buyer" });
            //        foreach (var sb in await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { supplierRoleId, buyerRoleId }, isActive: true, pageIndex: 0, pageSize: 15))
            //        {
            //            var role = "";
            //            var customerRoles = await _customerService.GetCustomerRolesAsync(sb);
            //            if (customerRoles.Any())
            //            {
            //                if (customerRoles.Any(x => x.Id == supplierRoleId))
            //                    role = "Supplier";

            //                if (customerRoles.Any(x => x.Id == buyerRoleId))
            //                    role = "Buyer";
            //            }
            //            options.Add(new SelectListItem
            //            {
            //                Value = sb.Id.ToString(),
            //                Text = $"{role} - {sb.Id},{sb.FullName},{sb.Username}",
            //                Selected = directOrdercaluation.SupplierCommissionReceivableUserId == sb.Id,
            //            });
            //        };
            //        //Supplier commission receivable user
            //        model.Payables.Add(new BusinessModelApiModel.Fields
            //        {
            //            Name = "SupplierCommissionReceivableUserId",
            //            Label = "Supplier commission receivable user",
            //            IsReadOnly = false,
            //            Required = true,
            //            FieldType = "Dropdown",
            //            DefaultValue = "0",
            //            Value = directOrdercaluation.SupplierCommissionReceivableUserId,
            //            DropdownOptions = options
            //        });
            //    }
            //    //Taxation
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Label = "Taxation",
            //        FieldType = "Section",
            //    });
            //    ////GST Type
            //    //model.Payables.Add(new BusinessModelApiModel.Fields
            //    //{
            //    //    Name = "GSTIncludedInTotalAmount",
            //    //    Label = "GST Type",
            //    //    IsReadOnly = false,
            //    //    Required = false,
            //    //    FieldType = "Toggle",
            //    //    DefaultValue = "No",
            //    //    Value = directOrdercaluation.GSTIncludedInTotalAmount ? "Yes" : "No",
            //    //    toggleOptions = new string[2] { "Yes", "No" },
            //    //    NeedRequest = true
            //    //});
            //    //GST rate
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "GSTRate",
            //        Label = "Gst rate %",
            //        IsReadOnly = false,
            //        Required = false,
            //        FieldType = "Decimal",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.GSTRate > 0 ? directOrdercaluation.GSTRate : "",
            //        NeedRequest = true,
            //    });
            //    //GST
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "GSTAmount",
            //        Label = "GST",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.GSTAmount,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.GSTAmount, true, false)
            //    });
            //    ////WHT Type
            //    //model.Payables.Add(new BusinessModelApiModel.Fields
            //    //{
            //    //    Name = "WhtIncluded",
            //    //    Label = "WHT Type",
            //    //    IsReadOnly = false,
            //    //    Required = false,
            //    //    FieldType = "Toggle",
            //    //    DefaultValue = "No",
            //    //    Value = directOrdercaluation.WhtIncluded ? "Yes" : "No",
            //    //    toggleOptions = new string[2] { "Yes", "No" },
            //    //    NeedRequest = true
            //    //});


            //    //WHT rate
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "WHTRate",
            //        Label = "Wht rate %",
            //        IsReadOnly = false,
            //        Required = false,
            //        FieldType = "Decimal",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.WHTRate > 0 ? directOrdercaluation.WHTRate : "",
            //        NeedRequest = true,
            //    });
            //    //WHT
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "WHTAmount",
            //        Label = "WHT",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.WHTAmount,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WHTAmount, true, false)
            //    });
            //    ////Wholesale Tax Type
            //    //model.Payables.Add(new BusinessModelApiModel.Fields
            //    //{
            //    //    Name = "WholesaletaxIncluded",
            //    //    Label = "Wholesale Tax Type",
            //    //    IsReadOnly = false,
            //    //    Required = false,
            //    //    FieldType = "Toggle",
            //    //    DefaultValue = "No",
            //    //    Value = directOrdercaluation.WholesaletaxIncluded ? "Yes" : "No",
            //    //    toggleOptions = new string[2] { "Yes", "No" },
            //    //    NeedRequest = true
            //    //});
            //    //Wholesale tax rate
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "WholesaleTaxRate",
            //        Label = "Wholesale tax rate %",
            //        IsReadOnly = false,
            //        Required = false,
            //        FieldType = "Decimal",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.WholesaleTaxRate > 0 ? directOrdercaluation.WholesaleTaxRate : "",
            //        NeedRequest = true,
            //    });
            //    //Wholesale tax
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "WholesaleTaxAmount",
            //        Label = "Wholesale tax",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.000",
            //        Value = directOrdercaluation.WholesaleTaxAmount,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.WholesaleTaxAmount, true, false)
            //    });
            //    //Supplier Calculation Summary
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Label = "Supplier Calculation Summary",
            //        FieldType = "Section",
            //    });
            //    //PayableToMill
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "PayableToMill",
            //        Label = "Payable to mill",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.0000",
            //        Value = directOrdercaluation.PayableToMill,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.PayableToMill, true, false)
            //    });
            //    //PaymentInCash
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "PaymentInCash",
            //        Label = "Payment in cash",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.0000",
            //        Value = directOrdercaluation.PaymentInCash,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.PaymentInCash, true, false)
            //    });
            //    //GrossAmount
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "GrossAmount",
            //        Label = "Gross total payable",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.0000",
            //        Value = directOrdercaluation.GrossAmount,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.GrossAmount, true, false)
            //    });
            //    // SupplierCommissionPayable
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "SupplierCommissionBag_Summary",
            //        Label = "Supplier commission payable",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.0000",
            //        Value = directOrdercaluation.SupplierCommissionBag_Summary,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.SupplierCommissionBag_Summary, true, false)
            //    });
            //    //SupplierCommissionReceivable
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "SupplierCommissionReceivable_Summary",
            //        Label = "Supplier commission receivable",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.0000",
            //        Value = directOrdercaluation.SupplierCommissionReceivable_Summary,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.SupplierCommissionReceivable_Summary, true, false)
            //    });
            //    //TotalPayable
            //    model.Payables.Add(new BusinessModelApiModel.Fields
            //    {
            //        Name = "OrderTotal",
            //        Label = "Total payable",
            //        IsReadOnly = true,
            //        Required = false,
            //        FieldType = "Label",
            //        DefaultValue = "0.0000",
            //        Value = directOrdercaluation.OrderTotal,
            //        ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.OrderTotal, true, false)
            //    });
            //    #endregion
            //}

            //#endregion

            return model;
        }

        public decimal ConvertToDecimal(object value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                    return 0m;

                decimal.TryParse(value.ToString(), out decimal parseDeciaml);
                return parseDeciaml;
            }
            catch
            {
                return 0m;
            }
        }

        public async Task<IList<string>> PreparePurchaseOrderCalculationAsync(Quotation quotation, DirectOrderCalculation directOrderCalculation)
        {
            var warnings = new List<string>();
            try
            {
                await SavePurchaseOrderCalculationAsync(new Order(), directOrderCalculation);
            }
            catch (Exception exc)
            {
                warnings.Add(exc.Message);
                return warnings;
            }
            return warnings;
        }
    }
}
