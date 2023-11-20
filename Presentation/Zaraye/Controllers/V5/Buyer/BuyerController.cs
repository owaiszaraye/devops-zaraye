using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Xml;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Models.Api.V5.Buyer;
using Zaraye.Models.Api.V5.Common;
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

namespace Zaraye.Controllers.V5.Security
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("5")]
    [Route("v{version:apiVersion}/buyer")]
    [AuthorizeApi]
    public class BuyerController : BaseApiController
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

        #endregion

        #region Ctor

        public BuyerController(ICustomerRegistrationService customerRegistrationService,
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
            CommonSettings commonSettings
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
        }

        #endregion

        #region Utilities

        [NonAction]
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

        [NonAction]
        public async Task<List<CommonApiModel.ProductItemAttributeApiModel>> Booker_ParseAttributeXml(string attributesXml)
        {
            var attributeDtos = new List<CommonApiModel.ProductItemAttributeApiModel>();
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

                                                attributeDtos.Add(new CommonApiModel.ProductItemAttributeApiModel
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

                                                attributeDtos.Add(new CommonApiModel.ProductItemAttributeApiModel
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

        [NonAction]
        public async Task<List<CommonApiModel.ProductItemAttributeApiModel>> ParseAttributeXml(string attributesXml)
        {
            var attributeDtos = new List<CommonApiModel.ProductItemAttributeApiModel>();
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

                                                attributeDtos.Add(new CommonApiModel.ProductItemAttributeApiModel
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

                                                attributeDtos.Add(new CommonApiModel.ProductItemAttributeApiModel
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

        [NonAction]
        public static string TimeAgo(DateTime? dt)
        {
            if (!dt.HasValue)
                return "";

            TimeSpan span = DateTime.UtcNow - dt.Value;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("{0} {1} ago",
                years, years == 1 ? "year" : "years");
            }

            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("{0} {1} ago",
                months, months == 1 ? "month" : "months");
            }

            if (span.Days > 0)
                return String.Format("{0} {1} ago",
                span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return String.Format("{0} {1} ago",
                span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return String.Format("{0} {1} ago",
                span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            if (span.Seconds > 5)
                return String.Format("{0} seconds ago", span.Seconds);
            if (span.Seconds <= 5)
                return "just now";

            return string.Empty;
        }

        [NonAction]
        public async Task<decimal> GetBuyerProgress(Customer buyer = null)
        {
            decimal Percentage = Math.Round((decimal)((float)100 / 12f), 2);
            decimal TotalPercent = 0;

            if (buyer == null)
                return TotalPercent;

            //var buyerTypeAttributeId = await _genericAttributeService.GetAttributeAsync<int>(buyer, NopCustomerDefaults.BuyerTypeIdAttribute);
            var userType = await _customerService.GetUserTypeByIdAsync(buyer.UserTypeId);
            var agent = await _customerService.GetCustomerByIdAsync(buyer.SupportAgentId);
            var pinLocationAttribute = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute);
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

        [NonAction]
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
                percentage = 100 - (int)Math.Round(((subtractedTotalSeconds / timeElapsedTotalSecondsv) * 100));
            }

            return (totalDays, totalHours, minutes, percentage);
        }

        [NonAction]
        public static string GetMimeTypeFromImageByteArray(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            using (Image image = Image.FromStream(stream))
            {
                return ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == image.RawFormat.Guid).MimeType;
            }
        }

        [NonAction]
        public async Task<int> Buyer_UploadPicture(byte[] imgBytes, string fileName)
        {
            if (imgBytes == null)
                return 0;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                var qqFileNameParameter = "qqfilename";

                if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                    fileName = Request.Form[qqFileNameParameter].ToString();

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

        protected virtual async Task SavePurchaseOrderAddressAsync(Customer supplier)
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
        #endregion

        #region Methods

        #region Dashboard

        [HttpGet("buyer-dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var buyer = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                    return Ok(new { success = false, message = "Buyer not found" });

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
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        //#region Info

        [HttpGet("info")]
        public async Task<IActionResult> Info()
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

            try
            {
                var buyerTypeAttributeId = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.BuyerTypeIdAttribute);
                var userType = await _customerService.GetUserTypeByIdAsync(buyer.UserTypeId > 0 ? Convert.ToInt32(buyer.UserTypeId) : 0);
                var agent = await _customerService.GetCustomerByIdAsync(buyer.SupportAgentId);

                var pinLocationAttribute = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute);
                var pinLocation = !string.IsNullOrWhiteSpace(pinLocationAttribute) ? pinLocationAttribute.Split(",") : new string[] { };
                var checkIndustryAllowCredit = (await _industryService.GetIndustryByIdAsync(buyer.IndustryId)).IsAllowCredit;

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
                    stateId = buyer.StateProvinceId,
                    areaId = buyer.AreaId,
                    city = buyer.City,
                    phone = buyer.Username,
                    industryId = buyer.IndustryId,
                    buyerType = userType != null ? userType.Name : null,
                    buyerTypeId = buyer.UserTypeId,
                    buyerPinLocation = pinLocation.Any() ?
                    new BuyerInfoApiModel.BuyerPinLocationApiModel()
                    {
                        Latitude = pinLocation[0],
                        Longitude = pinLocation[1],
                        Location = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute),
                    } :
                    new BuyerInfoApiModel.BuyerPinLocationApiModel(),
                    AgentInfo = agent is not null ? new
                    {
                        id = agent.Id,
                        email = agent.Email,
                        phonenumber = agent.Username,
                        fullname = agent.FullName
                    } : null,
                    Progress = await GetBuyerProgress(buyer),
                    isCompany = false,
                    isEmployee = false,
                    isAllowCredit = checkIndustryAllowCredit,


                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("info")]
        public async Task<IActionResult> Info([FromBody] BuyerInfoApiModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

            if (!CommonHelper.IsValidEmail(model.Email))
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Common.WrongEmail") });

            try
            {
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

                //if (model.BuyerTypeId > 0)
                //    await _genericAttributeService.SaveAttributeAsync(buyer, NopCustomerDefaults.BuyerTypeIdAttribute, model.BuyerTypeId);

                await _customerService.UpdateCustomerAsync(buyer);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Buyer.Info.Edit.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("info-address")]
        public async Task<IActionResult> InfoAddress([FromBody] BuyerInfoAddressApiModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

            try
            {
                //form fields
                buyer.CountryId = model.CountryId;
                buyer.StateProvinceId = model.StateId;
                buyer.AreaId = model.AreaId;
                buyer.StreetAddress = model.Address;
                buyer.StreetAddress2 = model.Address2;

                if (model.BuyerPinLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, $"{model.BuyerPinLocation.Latitude},{model.BuyerPinLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(buyer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute, model.BuyerPinLocation.Location);
                }

                await _customerService.UpdateCustomerAsync(buyer);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Buyer.Info.Edit.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        //#endregion

        #region Buyer Request

        [HttpGet("buyer-request-history/{active}")]
        public async Task<IActionResult> BuyerRequestHistory(bool active/*List<int> statusIds = null*/)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

            List<object> list = new List<object>();

            try
            {
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
                        var (_totalDays, _totalHours, _minutes, _percentage) = await GetPercentageAndTimeRemaining(buyerRequest.ExpiryDate.Value, buyerRequest.CreatedOnUtc);
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
                        ProductAttributes = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    });
                }

                return Ok(new { success = true, message = "", data = list });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-request/{requestId}")]
        public async Task<IActionResult> BuyerRequest(int requestId)
        {
            if (requestId <= 0)
                return Ok(new { success = false, message = "Buyer request id is required" });

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) /*|| !await _customerService.IsBuyerAsync(buyer)*/)
                return Ok(new { success = false, message = "Buyer not found" });

            var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
            if (buyerRequest is null /*|| buyerRequest.BuyerId != buyer.Id*/)
                return Ok(new { success = false, message = "Buyer request not found" });

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null || product.Id != buyerRequest.ProductId)
                return Ok(new { success = false, message = "Buyer request not found" });

            var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(buyerRequest.ProductId, isAppPublished: true)).FirstOrDefault();
            if (productCategory is null || product.Id != productCategory.ProductId)
                return Ok(new { success = false, message = "Buyer request not found" });

            var category = await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId);
            if (category is null)
                return Ok(new { success = false, message = "Buyer request not found" });

            var industry = await _categoryService.GetCategoryByIdAsync(category.ParentCategoryId);
            if (industry is null)
                return Ok(new { success = false, message = "Buyer request not found" });

            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
            try
            {
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
                   // ProductAttributes = await _requestModelFactory.PrepareProductAttributeModelsAsync(product, buyerRequest),
                    StatusId = buyerRequest.RequestStatusId,
                    Status = await _localizationService.GetLocalizedEnumAsync(buyerRequest.RequestStatus),
                    PaymentDuration = buyerRequest.PaymentDuration
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("buyer-request-add")]
        public async Task<IActionResult> AddBuyerRequest([FromBody] BuyerRequestApiModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                var buyer = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                    return Ok(new { success = false, message = "Buyer not found" });

                if (model.IndustryId <= 0)
                    return Ok(new { success = false, message = "Industry is required" });

                if (model.CategoryId <= 0)
                    return Ok(new { success = false, message = "Category is required" });

                var category = await _categoryService.GetCategoryByIdAsync(model.CategoryId);
                if (category is null)
                    return Ok(new { success = false, message = "Category not found" });

                if (model.ProductId <= 0)
                    return Ok(new { success = false, message = "Product is required" });

                var product = await _productService.GetProductByIdAsync(model.ProductId);
                if (product is null)
                    return Ok(new { success = false, message = "Product not found" });

                if (model.BrandId <= 0 && string.IsNullOrWhiteSpace(model.OtherBrand))
                    return Ok(new { success = false, message = "Brand is required" });

                var warnings = new List<string>();
                var attributesXml = await ConvertToXmlAsync(model.AttributesData, product.Id);
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, product, 1, attributesXml));
                if (warnings.Any())
                    return Ok(new { success = false, message = warnings.ToArray() });

                if (model.Quantity <= 0)
                    return Ok(new { success = false, message = "Quantity is required" });

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

                List<object> list = new List<object>();

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

                //var jsonData = new
                //{
                //    type = "RequestAdd",
                //    message = $"you have receive a new request from {(await _customerService.GetCustomerByIdAsync(buyerRequest.BuyerId))?.FullName} for {buyerRequest.Quantity} {(await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name} {(await _productService.GetProductByIdAsync(buyerRequest.ProductId))?.Name}"
                //};
                //_webSocketServer.NotifyAllClients(JsonConvert.SerializeObject(jsonData));

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Buyer.Request.Created.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("buyer-request-edit/{requestId}")]
        public async Task<IActionResult> EditBuyerRequest(int requestId, [FromBody] BuyerRequestApiModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                if (requestId <= 0)
                    return Ok(new { success = false, message = "Buyer request id is required" });

                var buyer = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                    return Ok(new { success = false, message = "Buyer not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
                if (buyerRequest is null /*|| buyerRequest.BuyerId != buyer.Id*/)
                    return Ok(new { success = false, message = "Buyer request not found" });



                if (/*buyerRequest.RequestStatus == RequestStatus.QuotationChosen ||*/ buyerRequest.RequestStatus == RequestStatus.Approved)
                    return Ok(new { success = false, message = "Buyer request not found" });

                if (model.IndustryId <= 0)
                    return Ok(new { success = false, message = "Industry is required" });

                if (model.CategoryId <= 0)
                    return Ok(new { success = false, message = "Category is required" });

                if (model.ProductId <= 0)
                    return Ok(new { success = false, message = "Product is required" });

                var product = await _productService.GetProductByIdAsync(model.ProductId);
                if (product is null)
                    return Ok(new { success = false, message = "Product not found" });

                var warnings = new List<string>();
                var attributesXml = await ConvertToXmlAsync(model.AttributesData, product.Id);
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, product, 1, attributesXml));
                if (warnings.Any())
                    return Ok(new { success = false, message = warnings.ToArray() });

                if (model.Quantity <= 0)
                    return Ok(new { success = false, message = "Quantity is required" });

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

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Buyer.Request.Updated.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Request Quotation History

        [HttpGet("buyer-request-quotations/{requestId}")]
        public async Task<IActionResult> BuyerRequestQuotationHistory(int requestId, List<int> statusIds = null)
        {
            if (requestId <= 0)
                return Ok(new { success = false, message = "Buyer request id is required" });

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

            var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
            if (buyerRequest is null /*|| buyerRequest.BuyerId != buyer.Id*/)
                return Ok(new { success = false, message = "Buyer request not found" });

            List<object> list = new List<object>();

            if (statusIds is null)
                statusIds = Enum.GetValues(typeof(QuotationStatus)).Cast<int>().ToList();

            try
            {
                var olderBidPrice = 0m;

                var quotations = await _requestService.GetQuotationsByRequestIdAsync(buyerRequest.Id);
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

                    (int totalDays, int totalHours, int minutes, int percentage) = await GetPercentageAndTimeRemaining(quotation.PriceValidity, quotation.CreatedOnUtc);
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

                return Ok(new { success = true, message = "", data = list });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("buyer-request-quotation-approved")]
        public async Task<IActionResult> BuyerRequestQuotationApproved([FromBody] BuyerRequestBidApproveApiModel model)
        {
            try
            {
                var buyer = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                    return Ok(new { success = false, message = "Buyer not found" });

                if (!model.QuotationId.Any())
                    return Ok(new { success = false, message = "At least one quotation is required" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(model.BuyerRequestId);
                if (buyerRequest is null || buyerRequest.RequestStatus == RequestStatus.Pending)
                    return Ok(new { success = false, message = "Invalid buyer request" });

                //try to get a quotation with the specified id
                var quotations = await _quotationService.GetQuotationByIdsAsync(model.QuotationId.ToArray());
                if (!quotations.Any())
                    return Ok(new { success = false, message = "No quotation found with the specified ids" });

                if (quotations.Sum(x => x.Quantity) != buyerRequest.Quantity)
                    return Json(new { success = false, message = "Quotations quantity does not match the request quantity" });

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null)
                    return Ok(new { success = false, message = "Product not found" });

                foreach (var quotation in quotations)
                {
                    var supplier = await _customerService.GetCustomerByIdAsync(quotation.SupplierId);
                    if (supplier is null)
                        return Ok(new { success = false, message = "Supplier not found" });

                    var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(quotation.RfqId);
                    if (requestForQuotation is null)
                        return Ok(new { success = false, message = "Request for quotation not found" });

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
                        return Ok(new { success = false, message = addToCartWarnings });
                    }

                    //save address for supplier 
                    await SavePurchaseOrderAddressAsync(supplier);

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
                        //Insert delivery schedule
                        //foreach (var schedule in await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id))
                        //{
                        //    await _orderService.InsertDeliveryScheduleAsync(new OrderDeliverySchedule
                        //    {
                        //        OrderId = placeOrderResult.PlacedOrder.Id,
                        //        CreatedById = user.Id,
                        //        ExpectedDeliveryDateUtc = schedule.ExpectedDeliveryDateUtc.Value,
                        //        ExpectedShipmentDateUtc = schedule.ExpectedShipmentDateUtc.Value,
                        //        ExpectedQuantity = schedule.ExpectedQuantity.Value,
                        //        ExpectedDeliveryCost = schedule.ExpectedDeliveryCost.Value,
                        //        CreatedOnUtc = DateTime.UtcNow
                        //    });
                        //}

                        //await SavePurchaseOrderCalculationAsync(placeOrderResult.PlacedOrder, directOrderCalculation);

                        //Update Quotation Status
                        quotation.QuotationStatus = QuotationStatus.QuotationSelected;
                        quotation.IsApproved = true;
                        await _quotationService.UpdateQuotationAsync(quotation);

                        //Update request
                        //request.PinLocation_Latitude = directOrder.PinLocation_Latitude;
                        //request.PinLocation_Location = directOrder.PinLocation_Location;
                        //request.PinLocation_Longitude = directOrder.PinLocation_Longitude;
                        //await _requestService.UpdateRequestAsync(request);

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
                        return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Order.Does'nt.Placed.Success") });
                }

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Request.PurchaseOrder.Genearted.Successfully") });

                //var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                //if (product != null)
                //{
                //    foreach (var quotation in quotations)
                //    {
                //        //now let's try adding product to the cart (now including product attribute validation, etc)
                //        var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: /*buyer*/await _workContext.GetCurrentCustomerAsync(),
                //        product: product,
                //        shoppingCartType: ShoppingCartType.ShoppingCart,
                //        storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                //        attributesXml: buyerRequest.ProductAttributeXml,
                //        quantity: quotation.Quantity, overridePrice: quotation.QuotationPrice, sellerBid: quotation, brandId: quotation.BrandId, supplierId: quotation.SupplierId);
                //        if (addToCartWarnings.Any())
                //            return Ok(new { success = false, message = string.Join(",", addToCartWarnings) });
                //    }

                //    //existing address
                //    var address = (await _customerService.GetAddressesByCustomerIdAsync(buyerRequest.BuyerId)).FirstOrDefault();
                //    if (address == null)
                //    {
                //        //insert default address (if possible)
                //        address = new Address
                //        {
                //            FirstName = buyer.FirstName,
                //            LastName = buyer.LastName,
                //            Email = buyer.Email,
                //            Company = buyer.Company,
                //            CountryId = buyer.CountryId,
                //            StateProvinceId = buyer.StateProvinceId,
                //            County = (await _CountryService.GetCountryByIdAsync(buyer.CountryId)).Name,
                //            City = buyer.City,
                //            Address1 = buyer.StreetAddress,
                //            Address2 = buyer.StreetAddress2,
                //            ZipPostalCode = buyer.ZipPostalCode,
                //            PhoneNumber = buyer.Phone,
                //            FaxNumber = buyer.Fax,
                //            CreatedOnUtc = buyer.CreatedOnUtc
                //        };

                //        //some validation
                //        if (address.CountryId == 0)
                //            address.CountryId = null;
                //        if (address.StateProvinceId == 0)
                //            address.StateProvinceId = null;

                //        await _addressService.InsertAddressAsync(address);
                //        await _customerService.InsertCustomerAddressAsync(buyer, address);

                //        buyer.BillingAddressId = address.Id;
                //        buyer.ShippingAddressId = address.Id;

                //        await _customerService.UpdateCustomerAsync(buyer);
                //    }
                //    else
                //    {
                //        //update address

                //        address.FirstName = buyer.FirstName;
                //        address.LastName = buyer.LastName;
                //        address.Email = buyer.Email;
                //        address.Company = buyer.Company;
                //        address.CountryId = buyer.CountryId;
                //        address.StateProvinceId = buyer.StateProvinceId;
                //        address.County = (await _CountryService.GetCountryByIdAsync(buyer.CountryId)).Name;
                //        address.City = buyer.City;
                //        address.Address1 = buyer.StreetAddress;
                //        address.Address2 = buyer.StreetAddress2;
                //        address.ZipPostalCode = buyer.ZipPostalCode;
                //        address.PhoneNumber = buyer.Phone;
                //        address.FaxNumber = buyer.Fax;

                //        await _addressService.UpdateAddressAsync(address);

                //        (await _workContext.GetCurrentCustomerAsync()).BillingAddressId = address.Id;
                //        await _customerService.UpdateCustomerAsync(await _workContext.GetCurrentCustomerAsync());
                //    }

                //    //place order
                //    var processPaymentRequest = new ProcessPaymentRequest
                //    {
                //        StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                //    };

                //    _paymentService.GenerateOrderGuid(processPaymentRequest);
                //    processPaymentRequest.StoreId = (await _storeContext.GetCurrentStoreAsync()).Id;
                //    processPaymentRequest.CustomerId = buyerRequest.BuyerId;
                //    processPaymentRequest.RequestId = buyerRequest.Id;
                //    processPaymentRequest.OrderTypeId = (int)OrderType.DirectOrder;
                //    processPaymentRequest.SubTotal_Payable = quotations.Sum(x => x.SubTotal_Payable);
                //    processPaymentRequest.TotalPayble = quotations.Sum(x => x.TotalPayble);
                //    processPaymentRequest.SubTotal_Receivable = quotations.Sum(x => x.SubTotal_Receivable);
                //    processPaymentRequest.TotalReceivable = quotations.Sum(x => x.TotalReceivable);
                //    //processPaymentRequest.OrderTotal = quotations.Sum(x => x.TotalReceivable);
                //    processPaymentRequest.Quotations = quotations;

                //    var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                //    if (placeOrderResult.Success)
                //    {
                //        //HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                //        //var postProcessPaymentRequest = new PostProcessPaymentRequest
                //        //{
                //        //    Order = placeOrderResult.PlacedOrder
                //        //};

                //        //var paymentMethod = await _paymentPluginManager
                //        //    .LoadPluginBySystemNameAsync(placeOrderResult.PlacedOrder.PaymentMethodSystemName, buyer, (await _storeContext.GetCurrentStoreAsync()).Id);
                //        //if (paymentMethod == null)
                //        //    //payment method could be null if order total is 0
                //        //    //success
                //        //    return Json(new { success = 1 });

                //        //await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                //        var oldstatus = await _localizationService.GetLocalizedEnumAsync(buyerRequest.RequestStatus);
                //        var oldJson = JsonConvert.SerializeObject(buyerRequest);

                //        buyerRequest.OrderId = placeOrderResult.PlacedOrder.Id;
                //        //buyerRequest.SellerBidId = quotation.Id;
                //        buyerRequest.RequestStatus = RequestStatus.Verified;
                //        buyerRequest.DeliveryAddress = model.DeliveryAddress;
                //        buyerRequest.DeliveryAddress2 = model.DeliveryAddress2;
                //        buyerRequest.PinLocation_Latitude = model.PinLocation.Latitude;
                //        buyerRequest.PinLocation_Longitude = model.PinLocation.Longitude;
                //        buyerRequest.PinLocation_Location = model.PinLocation.Location;
                //        await _requestService.UpdateRequestAsync(buyerRequest);

                //        await _customerActivityService.InsertActivityAsync("ChangeStatusBuyerRequest",
                //              string.Format(await _localizationService.GetResourceAsync("ActivityLog.QuotationSelectedFromAdmin"), oldstatus, await _localizationService.GetLocalizedEnumAsync(buyerRequest.BuyerRequestStatus), 0), buyerRequest, oldJson: oldJson);

                //        foreach (var quotation in quotations)
                //        {
                //            var oldsupplierstatus = await _localizationService.GetLocalizedEnumAsync(quotation.QuotationStatus);
                //            var oldJsonSeller = JsonConvert.SerializeObject(quotation);

                //            quotation.QuotationStatus = QuotationStatus.QuotationSelected;
                //            await _quotationService.UpdateQuotationAsync(quotation);
                //            var newJsonSeller = JsonConvert.SerializeObject(quotation);

                //            await _customerActivityService.InsertActivityAsync("ChangeStatusSupplierQuotation",
                //                  string.Format(await _localizationService.GetResourceAsync("ActivityLog.ChangeStatusSupplierQuotationFromAdminForMarkAsQuotation"), oldsupplierstatus, await _localizationService.GetLocalizedEnumAsync(quotation.SellerBidStatus)), quotation, oldJson: oldJsonSeller, newJson: newJsonSeller);
                //        }

                //        return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Buyer.Quotation.Approved.Success") });
                //    }
                //    else
                //    {
                //        return Ok(new { success = false, message = string.Join(",", placeOrderResult.Errors) });
                //    }
                //}
                //else
                //{
                //    return Ok(new { success = false, message = "Invalid buyer request product" });
                //}
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer order

        [HttpGet("buyer-order-list/{active}")]
        public async Task<IActionResult> BuyerOrderList(bool active)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

            var orders = await _orderService.SearchOrdersAsync(customerId: buyer.Id, getOnlyBuyersActiveOrdersForApi: active);
            if (!orders.Any())
                return Ok(new { success = false, message = "Orders not found" });

            try
            {
                var data = new List<object>();
                foreach (var order in orders)
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

                    data.Add(new
                    {
                        OrderId = order.Id,
                        OrderCustomNumber = order.CustomOrderNumber,
                        OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                        OrderStatusId = order.OrderStatusId,
                        PaymentDueDate = paymentDueDate,
                        Industry = industry,
                        IndustryId = industryId,
                    });
                }

                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-order-detail/{orderId}")]
        public async Task<IActionResult> BuyerOrderDetail(int orderId)
        {
            if (orderId <= 0)
                return Ok(new { success = false, message = "Buyer order id is required" });

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) /*|| !await _customerService.IsBuyerAsync(buyer)*/)
                return Ok(new { success = false, message = "Buyer not found" });

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                return Ok(new { success = false, message = "Order not found" });

            try
            {
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
                        ProductAttributesInfo = await ParseAttributeXml(buyerRequest.ProductAttributeXml),
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
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-order-summary/{orderId}")]
        public async Task<IActionResult> BuyerOrderSummary(int orderId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                return Ok(new { success = false, message = "Order not found" });

            var orderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(orderId);
            if (orderCalculation is null)
                return Ok(new { success = false, message = "Order calculation not found" });

            var totalReceived = await _shipmentService.GetOrderPaidAmount(order);
            var totalAmountBalance = orderCalculation.OrderTotal - totalReceived;

            try
            {
                var data = new
                {
                    TotalReceivables = await _priceFormatter.FormatPriceAsync(orderCalculation.OrderTotal),
                    TotalReceived = await _priceFormatter.FormatPriceAsync(totalReceived),
                    TotalBalance = await _priceFormatter.FormatPriceAsync(totalAmountBalance),
                    PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus)
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        //[HttpGet("buyer-order-payment-history")]
        //public async Task<IActionResult> BuyerOrderPaymentHistory()
        //{
        //    var buyer = await _workContext.GetCurrentCustomerAsync();
        //    if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
        //        return Ok(new { success = false, message = "Buyer not found" });

        //    var ordersData = new List<BuyerOrderPaymentApiModel>();

        //    var orders = await _orderService.SearchOrdersAsync(customerId: buyer.Id);
        //    foreach (var order in orders)
        //    {
        //        if (order.OrderStatusId != (int)OrderStatus.Cancelled)
        //        {
        //            ordersData.Add(new BuyerOrderPaymentApiModel
        //            {
        //                CustomOrderNumber = order.CustomOrderNumber,
        //                DateFormatted = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
        //                Date = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)),
        //                Type = "Debit",
        //                Amount = await _priceFormatter.FormatPriceAsync(order.TotalReceivable)
        //            });
        //        }
        //    }

        //    var orderPayments = await _orderService.GetAllOrderPaymentsAsync(buyerId: buyer.Id, paymentType: "Receivable", isApproved: true);
        //    foreach (var orderPayment in orderPayments)
        //    {
        //        var order = await _orderService.GetOrderByIdAsync(orderPayment.OrderId);
        //        if (order is null)
        //            continue;

        //        ordersData.Add(new BuyerOrderPaymentApiModel
        //        {
        //            CustomOrderNumber = order.CustomOrderNumber,
        //            DateFormatted = (await _dateTimeHelper.ConvertToUserTimeAsync(orderPayment.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
        //            Date = (await _dateTimeHelper.ConvertToUserTimeAsync(orderPayment.CreatedOnUtc, DateTimeKind.Utc)),
        //            Type = "Credit",
        //            Amount = await _priceFormatter.FormatPriceAsync(orderPayment.Amount)
        //        });
        //    }

        //    var orderTotal = orders.Where(x => x.OrderStatusId != (int)OrderStatus.Cancelled).Sum(x => x.TotalReceivable);
        //    var orderTotalPaid = orderPayments.Sum(x => x.Amount);
        //    var orderTotalBalance = orderTotal - orderTotalPaid;

        //    try
        //    {
        //        ordersData = ordersData.OrderByDescending(x => x.Date).ToList();
        //        var data = new
        //        {
        //            Orders = ordersData,
        //            Summary = new
        //            {
        //                ReportUrl = _storeContext.GetCurrentStore().Url + "buyerpaymenthistory/",
        //                TotalOutstandingBalance = await _priceFormatter.FormatPriceAsync(orderTotalBalance),
        //            }
        //        };
        //        return Ok(new { success = true, message = "", data });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        #endregion

        #region Buyer Reports

        [HttpGet("buyer-request-status-summary")]
        public async Task<IActionResult> BuyerRequestStatusSummary()
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

            try
            {
                var lastRequestUpdated = await _requestService.GetLastUpdatedRecordByStatusAsync(buyerId: buyer.Id);

                var data = new
                {
                    Pending = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Pending }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Pending, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Pending ? true : false
                    },
                    Verified = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Verified }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Verified, buyer.Id)),
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
                        LastUpdated = TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Approved, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Approved ? true : false
                    },
                    Complete = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Complete }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Complete, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Complete ? true : false
                    },
                    Cancelled = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Cancelled }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Cancelled, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Cancelled ? true : false
                    },
                    Expired = new
                    {
                        Count = (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Expired }, buyerId: buyer.Id, getOnlyTotalCount: true)).TotalCount,
                        LastUpdated = TimeAgo(await _requestService.GetLastUpdatedDateByStatusAsync(statusId: (int)RequestStatus.Expired, buyerId: buyer.Id)),
                        IsNew = lastRequestUpdated != null && lastRequestUpdated.RequestStatusId == (int)RequestStatus.Expired ? true : false
                    }
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Support Agent

        [HttpGet("support-agent-info")]
        public async Task<IActionResult> SupportAgentInfo()
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

            if (buyer.SupportAgentId == 0)
                return Ok(new { success = false, message = "Support agent not found" });

            try
            {
                var agent = await _customerService.GetCustomerByIdAsync(buyer.SupportAgentId);
                if (agent is null)
                    return Ok(new { success = false, message = "Support agent not found" });

                var data = new
                {
                    id = agent.Id,
                    email = agent.Email,
                    phonenumber = agent.Username,
                    fullname = agent.FullName
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Ledger

        [HttpGet("buyer-ledger-detail")]
        public async Task<IActionResult> BuyerLedgerDetail(DateTime? startDate, DateTime? endDate, int pageSize, int pageIndex)
        {
            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) || !await _customerService.IsBuyerAsync(buyer))
                return Ok(new { success = false, message = "Buyer not found" });

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
                //var data = ().Select(async ledger =>
                // {
                //     return new
                //     {
                //         dateFormate = ledger.Date.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(ledger.Date.Value, DateTimeKind.Utc)).ToString() : "",
                //         sku = ledger.ProductName,
                //         productId = ledger.ProductId,
                //         quantityFormated = $"{ledger.Quantity} {ledger.QuantityType}",
                //         debitFormater = ledger.Debit > 0 || ledger.Debit < 0 ? await _priceFormatter.FormatPriceAsync(ledger.Debit, true, false) : "",
                //         debit = ledger.Debit,
                //         creditFormater = ledger.Credit > 0 ? await _priceFormatter.FormatPriceAsync(ledger.Credit, true, false) : "",
                //         credit = ledger.Credit,
                //         balanceFormater = ledger.Balance < 0 ? await _priceFormatter.FormatPriceAsync(ledger.Balance < 0 ? Math.Abs(ledger.Balance) : ledger.Balance, true, false) + "CR" : await _priceFormatter.FormatPriceAsync(ledger.Balance, true, false) + "DR",
                //         balance = ledger.Balance,
                //         orderId = ledger.OrderId,
                //         customOrderNumber = (await _orderService.GetOrderByIdAsync(ledger.OrderId))?.CustomOrderNumber,
                //         shipmentId = ledger.ShipmentId,
                //         description = ledger.Description,
                //         brand = ledger.BrandId > 0 ? (await _manufacturerService.GetManufacturerByIdAsync(ledger.BrandId)).Name : ledger.OtherBrand,
                //     };
                // }).Select(t => t.Result).ToList();

                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Contract

        [HttpPost("buyer-contact-upload-signature")]
        public async Task<IActionResult> Buyer_BuyerContactUploadSignature([FromBody] BuyerContractUploadSignatureModel model)
        {
            if (model.imgBytes == null)
                return Ok(new { success = false, message = "Image is required" });

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return Ok(new { success = false, message = "Order not found." });

            var buyerContract = await _orderService.GetContractByIdAsync(model.ContactId);
            if (buyerContract == null)
                return Ok(new { success = false, message = "Buyer contract not found." });

            buyerContract.SignaturePictureId = await Buyer_UploadPicture(model.imgBytes, $"{buyerContract.ContractGuid}");
            await _orderService.UpdateContractAsync(buyerContract);

            await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}");

            return Ok(new
            {
                success = true,
                message = "",
                BuyerContract = new
                {
                    contractId = buyerContract.Id,
                    downloadUrl = $"{_storeContext.GetCurrentStore().Url}download/get-contract/{buyerContract.ContractGuid}",
                    previewUrl = $"{_storeContext.GetCurrentStore().Url}files/contracts/{buyerContract.ContractGuid}.pdf",
                    contractSignature = buyerContract.SignaturePictureId > 0 ? true : false
                }
            });
        }

        #endregion

        #region Applied Credit Customer

        [HttpPost("apply-for-credit-customer")]
        public async Task<IActionResult> ApplyForCreditCustomer([FromBody] ApplyCreditCustomerModel model)
        {
            if (string.IsNullOrEmpty(model.FullName))
                return Ok(new { success = false, message = "FullName is required" });
            if (string.IsNullOrEmpty(model.RegisteredphoneNumber))
                return Ok(new { success = false, message = "RegisteredphoneNumber is required" });
            if (string.IsNullOrEmpty(model.BusinessAddress))
                return Ok(new { success = false, message = "BusinessAddress is required" });
            if (string.IsNullOrEmpty(model.CnicFrontImageByte))
                return Ok(new { success = false, message = "Cnic Front is required" });
            if (string.IsNullOrEmpty(model.CnicBackImageByte))
                return Ok(new { success = false, message = "Cnic Back is required" });
            if (string.IsNullOrEmpty(model.Cnic))
                return Ok(new { success = false, message = "Cnic number is required" });

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            Guid objfront = Guid.NewGuid();
            Guid objback = Guid.NewGuid();

            var cincfront = await _amazonS3BuketService.UploadBase64FileAsync(model.CnicFrontImageByte, _commonSettings.BucketName, objfront.ToString() + ".jpg");
            var cincback = await _amazonS3BuketService.UploadBase64FileAsync(model.CnicBackImageByte, _commonSettings.BucketName, objback.ToString() + ".jpg");
            var applyCreditCustomer = new AppliedCreditCustomer
            {
                CustomerId = user.Id,
                //FullName = model.FullName,
                //RegisteredPhoneNumber = model.RegisteredphoneNumber,
                //BusinessAddress = model.BusinessAddress,
                //CnicFront = cincfront,
                //CnicBack = cincback,
                //Cnic = model.Cnic,
                StatusId = (int)AppliedCreditCustomerStatusEnum.Pending,
                CreatedOnUtc = DateTime.UtcNow,
                CreatedById = user.Id
            };

            await _customerService.InsertAppliedCreditCustomerAsync(applyCreditCustomer);

            return Ok(new { success = true, message = "Record has been added." });
        }

        [HttpGet("apply-for-credit-customer")]
        public async Task<IActionResult> GetApplyForCreditCustomer()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user) && !await _customerService.IsBuyerAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var getcustomer = await _customerService.GetAppliedCreditCustomerByCustomerIdAsync(user.Id);
            if (getcustomer == null)
                return Ok(new { success = false, message = "record not found" });
            if (getcustomer.StatusId == (int)AppliedCreditCustomerStatusEnum.Rejected)
                return Ok(new { success = false, message = "record not found" });

            var data = new
            {
                CustomerId = getcustomer.CustomerId,
              //  FullName = getcustomer.FullName,
                //RegisteredPhoneNumber = getcustomer.RegisteredPhoneNumber,
                //BusinessAddress = getcustomer.BusinessAddress,
                //CnicFront = getcustomer.CnicFront,
                //CnicBack = getcustomer.CnicBack,
                //Cnic = getcustomer.Cnic,
                Status = await _localizationService.GetLocalizedEnumAsync<AppliedCreditCustomerStatusEnum>((AppliedCreditCustomerStatusEnum)getcustomer.StatusId),
                CreatedOnUtc = getcustomer.CreatedOnUtc,
                CreatedById = getcustomer.CreatedById
            };

            return Ok(new { success = true, message = "", data });
        }


        #endregion

        #endregion
    }
}