using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Payments;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Events;
using Zaraye.Core.Shipping;
using Zaraye.Services.Authentication;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.CustomerLedgers;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.Helpers;
using Zaraye.Services.Inventory;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Media;
using Zaraye.Services.Messages;
using Zaraye.Services.Orders;
using Zaraye.Services.Payments;
using Zaraye.Services.Security;
using Zaraye.Services.Shipping;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Xml;
using Zaraye.Models.Api.V5.Common;
using Zaraye.Models.Api.V5.OrderManagement;
using Zaraye.Models.Api.V5.Security;
using static Zaraye.Models.Api.V5.OrderManagement.OrderManagementApiModel;

namespace Zaraye.Controllers.V5.OrderManagement
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("5")]
    [Route("v{version:apiVersion}/order-management")]
    [AuthorizeApi]
    public class OrderManagementController : BaseApiController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAddressService _addressService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly IMeasureService _measureService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IOrderService _orderService;
        private readonly IManufacturerService _brandService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShipmentService _shipmentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly HttpClient _httpClient;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CalculationSettings _calculationSettings;
        private readonly IRequestService _requestService;
        private readonly IInventoryService _inventoryService;
        private readonly IQuotationService _quotationService;
        private readonly IIndustryService _industryService;
        private readonly IShippingService _shippingService;
        private readonly ICustomerLedgerService _customerLedgerService;

        #endregion

        #region Ctor

        public OrderManagementController(ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IWorkflowMessageService workflowMessageService,
            IStoreContext storeContext,
            IWorkContext workContext,
            ICustomerActivityService customerActivityService,
            IConfiguration config,
            IAuthenticationService authenticationService,
            IEventPublisher eventPublisher,
            IAddressService addressService,
            LocalizationSettings localizationSettings,
            IProductService productService,
            ICategoryService categoryService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IShoppingCartService shoppingCartService,
            IProductAttributeService productAttributeService,
            IDownloadService downloadService,
            IMeasureService measureService,
            ICustomNumberFormatter customNumberFormatter,
            IOrderService orderService,
            IManufacturerService brandService,
            IPaymentService paymentService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShipmentService shipmentService,
            IPriceCalculationService priceCalculationService,
            HttpClient client,
            ShoppingCartSettings shoppingCartSettings,
            CalculationSettings calculationSettings,
            IRequestService requestService,
            IInventoryService inventoryService,
            IQuotationService quotationService,
            IIndustryService industryService,
            IShippingService shippingService,
            ICustomerLedgerService customerLedgerService)
        {
            _storeContext = storeContext;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _customerActivityService = customerActivityService;
            _authenticationService = authenticationService;
            _eventPublisher = eventPublisher;
            _addressService = addressService;
            _localizationSettings = localizationSettings;
            _productService = productService;
            _categoryService = categoryService;
            _dateTimeHelper = dateTimeHelper;
            _priceFormatter = priceFormatter;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
            _productAttributeService = productAttributeService;
            _downloadService = downloadService;
            _measureService = measureService;
            _customNumberFormatter = customNumberFormatter;
            _orderService = orderService;
            _brandService = brandService;
            _paymentService = paymentService;
            _orderProcessingService = orderProcessingService;
            _permissionService = permissionService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _shipmentService = shipmentService;
            _priceCalculationService = priceCalculationService;
            _httpClient = client;
            _shoppingCartSettings = shoppingCartSettings;
            _calculationSettings = calculationSettings;
            _requestService = requestService;
            _inventoryService = inventoryService;
            _quotationService = quotationService;
            _industryService = industryService;
            _shippingService = shippingService;
            _customerLedgerService = customerLedgerService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected async Task<decimal> CalculateSellingPriceOfProductByTaggings(List<DirectCogsInventoryTagging> cogsInventoryTaggings, bool gstInclude = false, decimal sellingPriceOfProduct = 0)
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
                    if (inventoryInbounds.Count == 1 && (inventoryInbounds.Any(x => x.GstAmount > 0) && !gstInclude))
                        sellingPriceOfProduct = sellingPriceOfProduct / 1.18m;
                }
            }
            return sellingPriceOfProduct;
        }

        [NonAction]
        protected async Task<decimal> CalculateBuyingPriceByTaggings(List<DirectCogsInventoryTagging> cogsInventoryTaggings, bool gstInclude = false)
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
        public async Task<int> OrderManagement_UploadPicture(byte[] imgBytes, string fileName)
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

        [NonAction]
        public int OrderManagement_Upload(byte[] imgBytes, string fileName)
        {
            if (imgBytes == null)
                return 0;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                var qqFileNameParameter = "qqfilename";

                if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                    fileName = Request.Form[qqFileNameParameter].ToString();

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

        [NonAction]
        protected virtual async Task SaveSaleOrderAddressAsync(Request request, Customer buyer)
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

        [NonAction]
        protected virtual async Task<IList<string>> SaveSalesOrderCalculationAsync(Order order, Request request, DirectOrderCalculation directOrderCalculation)
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

            var taggedInventories = (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id));
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

                var totalReceivableFromBuyer = ((decimal)((directOrderCalculation.SellingPriceOfProduct + gstAmount) - whtAmount));

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
                orderCalculation.TotalPerBag = ((decimal)(totalReceivableFromBuyer + directOrderCalculation.BuyerCommissionReceivablePerBag - directOrderCalculation.BuyerCommissionPayablePerBag));

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
                    var gstRateReceivableCalculationValue = ((decimal)((float)sellingPrice_FinanceIncome * (float)directOrderCalculation.GSTRate / 100));
                    gstAmount = gstRateReceivableCalculationValue;
                }
                else
                    gstAmount = 0m;

                if (directOrderCalculation.WHTRate > 0)
                {
                    var whtRateReceivableCalculationValue = ((decimal)((float)(sellingPrice_FinanceIncome + gstAmount) * (float)directOrderCalculation.WHTRate / 100));
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
                orderCalculation.FinanceCost = directOrderCalculation.SupplierCreditTerms > 0 && directOrderCalculation.TotalFinanceCost > 0 ? (directOrderCalculation.TotalFinanceCost / directOrderCalculation.SupplierCreditTerms) * directOrderCalculation.BuyerPaymentTerms : 0;
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

                orderCalculation.TotalReceivableBuyer = (orderCalculation.SellingPrice_FinanceIncome + gstAmount) - (whtAmount);

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
                var payableToMill = ((decimal)((directOrderCalculation.InvoicedAmount + gstAmount) - (whtAmount) + (wholesaleTax)));
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
                orderCalculation.TotalPerBag = (totalReceivableFromBuyerDirectlyToSupplier + totalCommissionReceivableFromBuyerToZaraye + directOrderCalculation.BuyerCommissionReceivablePerBag - directOrderCalculation.BuyerCommissionPayablePerBag);

                var totalCommissionReceivableFromBuyerToZaraye_ReceivableAfterMultiply = margin * request.Quantity;
                var totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply = payableToMillAfterMultiply;
                var buyerCommissionAfterMultiply = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                var buyerCommission_ReceivableAfterMultiply = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                orderCalculation.TotalReceivableFromBuyerDirectlyToSupplier = totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply;
                orderCalculation.TotalReceivableFromBuyerDirectlyToSupplier = totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply;
                orderCalculation.SubTotal = (totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply + totalCommissionReceivableFromBuyerToZaraye_ReceivableAfterMultiply /*+ buyerCommission_ReceivableAfterMultiply - buyerCommissionAfterMultiply*/);
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

        [NonAction]
        public virtual async Task<IList<string>> PrepareSaleOrderCalculationAsync(Order order, Request request, DirectOrderCalculation model)
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

        [NonAction]
        protected virtual async Task SavePurchaseOrderCalculationAsync(Order order, DirectOrderCalculation directOrderCalculation)
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
                        orderCalculation.NetAmountWithoutGST = ((decimal)((float)orderCalculation.NetAmount / (float)_calculationSettings.GSTPercentageForStandardQuotation /*1.17f*/));
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
                        orderCalculation.WHTAmount = ((decimal)((float)(totalPayble + orderCalculation.GSTAmount) * (float)directOrderCalculation.WHTRate / 100f));

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

        [NonAction]
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

        [NonAction]
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
            var taggedInventories = (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id));
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
                    Value = ((directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity),
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
                    Value = (decimal)(directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount) * request.Quantity,
                    ValueFormatted = await _priceFormatter.FormatPriceAsync((decimal)(directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount) * request.Quantity, true, false),
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
                    Value = directOrdercaluation.FinanceCostPayment == (int)FinanceCostPaymentStatus.Cash ? ((directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount + directOrdercaluation.FinanceIncome) * request.Quantity) : ((directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity),
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
                    Value = directOrdercaluation.FinanceCostPayment == (int)FinanceCostPaymentStatus.DirectlyToMill ? ((directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount + directOrdercaluation.FinanceIncome) * request.Quantity) : (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount) * request.Quantity,
                    //Value = (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount),
                    ValueFormatted = await _priceFormatter.FormatPriceAsync(directOrdercaluation.FinanceCostPayment == (int)FinanceCostPaymentStatus.DirectlyToMill ? ((directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount + directOrdercaluation.FinanceIncome) * request.Quantity) : (directOrdercaluation.InvoicedAmount + directOrdercaluation.GSTAmount - directOrdercaluation.WHTAmount) * request.Quantity, true, false),
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
                    Value = ((directOrdercaluation.SellingPriceOfProduct - directOrdercaluation.InvoicedAmount) * request.Quantity),
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

        [NonAction]
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

        [NonAction]
        public virtual async Task<IList<string>> PreparePurchaseOrderCalculationAsync(Quotation quotation, DirectOrderCalculation directOrderCalculation)
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

        #endregion

        #region Common Api

        //[HttpGet("suppliers-by-order/{orderId}")]
        //public async Task<IActionResult> OrderManagement_GetAllSuppliersByOrder(int orderId)
        //{
        //    var user = await _workContext.GetCurrentCustomerAsync();
        //    if (!await _customerService.IsRegisteredAsync(user))
        //        return Ok(new { success = false, message = "User not found" });

        //    var order = await _orderService.GetOrderByIdAsync(orderId);
        //    if (order == null || order.Deleted)
        //        return Ok(new { success = false, message = "Order not found" });

        //    //todo
        //    List<object> supplier_list = new List<object>();

        //    var mappings = await _quotationService.GetAllRequestQuotationApprovedMappingAsync(orderId: order.Id);
        //    if (mappings.Any())
        //    {
        //        var suppliers = (await _customerService.GetCustomersByIdsAsync(mappings.Select(x => x.SupplierId).Distinct().ToArray()));
        //        foreach (var item in suppliers)
        //        {
        //            supplier_list.Add(new
        //            {
        //                Value = item.Id,
        //                Text = item.FullName
        //            });
        //        }
        //    }

        //    return Ok(new { success = true, message = "", data = supplier_list });
        //}        //[HttpGet("suppliers-by-order/{orderId}")]
        //public async Task<IActionResult> OrderManagement_GetAllSuppliersByOrder(int orderId)
        //{
        //    var user = await _workContext.GetCurrentCustomerAsync();
        //    if (!await _customerService.IsRegisteredAsync(user))
        //        return Ok(new { success = false, message = "User not found" });

        //    var order = await _orderService.GetOrderByIdAsync(orderId);
        //    if (order == null || order.Deleted)
        //        return Ok(new { success = false, message = "Order not found" });

        //    //todo
        //    List<object> supplier_list = new List<object>();

        //    var mappings = await _quotationService.GetAllRequestQuotationApprovedMappingAsync(orderId: order.Id);
        //    if (mappings.Any())
        //    {
        //        var suppliers = (await _customerService.GetCustomersByIdsAsync(mappings.Select(x => x.SupplierId).Distinct().ToArray()));
        //        foreach (var item in suppliers)
        //        {
        //            supplier_list.Add(new
        //            {
        //                Value = item.Id,
        //                Text = item.FullName
        //            });
        //        }
        //    }

        //    return Ok(new { success = true, message = "", data = supplier_list });
        //}

        [HttpGet("user-info")]
        public async Task<IActionResult> OrderManagement_Info()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            try
            {
                var data = new
                {
                    info = new
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Phone = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CompanyName = user.Company,
                        CountryId = user.CountryId,
                        StateId = user.StateProvinceId,
                        Address = user.StreetAddress,
                        City = user.City,
                        BookerType = user.BookerType,
                        role = new
                        {
                            //New
                            isDemandAssociate = await _customerService.IsDemandAssociateAsync(user),
                            isSupply = await _customerService.IsSupplyAsync(user),
                            isSupplyAssociate = await _customerService.IsSupplyAssociateAsync(user),
                            isGroundOperations = await _customerService.IsGroundOperationsAsync(user),
                            isOPerationAssociate = await _customerService.IsOPerationAssociateAsync(user),
                            isOPerationLead = await _customerService.IsOPerationLeadAsync(user),
                            isBusinessLead = await _customerService.IsBusinessLeadAsync(user),
                            isLiveOps = await _customerService.IsLiveOpsAsync(user),
                            isControlTower = await _customerService.IsControlTowerAsync(user),

                            //Old
                            isDemand = await _customerService.IsDemandAsync(user),
                            //isSupply = await _customerService.IsSupplyAsync(user),
                            isOperations = await _customerService.IsOperationsAsync(user),
                            isFinance = await _customerService.IsFinanceAsync(user),
                            isBusinessHead = await _customerService.IsBusinessHeadAsync(user),
                            isFinanceHead = await _customerService.IsFinanceHeadAsync(user),
                            isOpsHead = await _customerService.IsOpsHeadAsync(user)
                        },
                        permission = new
                        {
                            CreateRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.CreateRequest),
                            RejectRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.RejectRequest),
                            InitiateRfq = await _permissionService.AuthorizeAsync(StandardPermissionProvider.InitiateRfq),
                            CreateSalesOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.CreateSalesOrder),
                            CreateQuotation = await _permissionService.AuthorizeAsync(StandardPermissionProvider.CreateQuotation),
                            ChooseQuotationToCreatePurchaseOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ChooseQuotationToCreatePurchaseOrder),
                            //CreatePurchaseOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.CreatePurchaseOrder),
                            GenerateDeliveryRequestOnSaleOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.GenerateDeliveryRequestOnSaleOrder),
                            GeneratePickupRequestOnPurchaseOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.GeneratePickupRequestOnPurchaseOrder),
                            SignSalesOrderContract = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SignSalesOrderContract),
                            SignPurchaseOrderContract = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SignPurchaseOrderContract),
                            RespondToDeleiveryRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.RespondToDeleiveryRequest),
                            RespondToPickupRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.RespondToPickupRequest),
                            ReassignAgentOnOperationTicket = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ReassignAgentOnOperationTicket),
                            BuyerRegistration = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBuyerRegistration),
                            SupplierRegistration = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSupplierRegistration),

                            //Demand
                            //ManageBuyerRegistration = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBuyerRegistration),
                            ManagePlaceRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceRequest),
                            ManagePlaceOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceOrder),
                            ManagePlaceDeliveryRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceDeliveryRequest),
                            ManagePlaceSalesReturnRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceSalesReturnRequest),

                            //Supply
                            //ManageSupplierRegistration = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSupplierRegistration),
                            ManagePlaceQuotation = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceQuotation),
                            ManageRaisePo = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRaisePo),

                            //
                            ManageUploadGrns = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUploadGrn),
                            ManageUploadProofofPayments = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUploadProofofPayment),
                            //ManageRespondtoAssignedTicketOps = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRespondtoAssignedTicketOps),
                            ManageApproveorRejectPowithAttachments = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageApproveorRejectPowithAttachment),
                            //ManageReassignedTicketsFinance = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRespondToAssignedTicketsFinance),
                            ManageReassignonTickets = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReassignonTicket),

                            ManagePoTickets = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePOTicket),
                            ManageDeliveryRequestTicket = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDeliveryRequestTicket),
                            ManageSaleReturnTicket = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSaleReturnTicket),
                            ManageAgent = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAgent),
                            ManageRaisePoCompleteAndIncomplete = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRaisePoCompleteAndIncomplete),
                            ManageSalesReturnCompleteAndIncomplete = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSalesReturnCompleteAndIncomplete),
                            ManageInvantoryRate = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageInventoryRate),
                            InventoryRate = await _permissionService.AuthorizeAsync(StandardPermissionProvider.InventoryRate),

                            PurchaseOrderShipmentRequestApproved = await _permissionService.AuthorizeAsync(StandardPermissionProvider.PurchaseOrderShipmentRequestApproved),
                            PurchaseOrderShipmentShipped = await _permissionService.AuthorizeAsync(StandardPermissionProvider.PurchaseOrderShipmentShipped),
                            PurchaseOrderShipmentDelivered = await _permissionService.AuthorizeAsync(StandardPermissionProvider.PurchaseOrderShipmentDelivered),
                            SaleOrderShipmentRequestApproved = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SaleOrderShipmentRequestApproved),
                            SaleOrderShipmentShipped = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SaleOrderShipmentShipped),
                            SaleOrderShipmentDelivered = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SaleOrderShipmentDelivered)
                        }
                    }
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("user-info")]
        public async Task<IActionResult> OrderManagement_Info([FromBody] OrderManagementApiModel.BuyerInfoModel model)
        {
            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (!CommonHelper.IsValidEmail(model.Email))
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Common.WrongEmail") });

            if (string.IsNullOrWhiteSpace(model.Phone))
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Fields.Username.Required") });

            try
            {
                //username 
                if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
                {
                    var userName = model.Phone.Trim();
                    if (!user.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change username
                        await _customerRegistrationService.SetUsernameAsync(user, userName);

                        //re-authenticate
                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalCustomerIfImpersonated == null)
                            await _authenticationService.SignInAsync(user, true);
                    }
                }

                //email
                var email = model.Email.Trim();
                if (!user.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                {
                    //change email
                    var requireValidation = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                    await _customerRegistrationService.SetEmailAsync(user, email, requireValidation);

                    //do not authenticate users in impersonation mode
                    if (_workContext.OriginalCustomerIfImpersonated == null)
                    {
                        //re-authenticate (if usernames are disabled)
                        if (!_customerSettings.UsernamesEnabled && !requireValidation)
                            await _authenticationService.SignInAsync(user, true);
                    }
                }

                //form fields
                if (_customerSettings.FirstNameEnabled)
                    user.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    user.LastName = model.LastName;
                if (_customerSettings.StreetAddressEnabled)
                    user.StreetAddress = model.Address;
                if (_customerSettings.CompanyEnabled)
                    user.Company = model.Company;
                if (_customerSettings.CountryEnabled)
                    user.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    user.StateProvinceId = model.StateId;
                if (_customerSettings.PhoneEnabled)
                    user.Phone = model.Phone;

                await _customerService.UpdateCustomerAsync(user);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Booker.Info.Edit.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-live-ops-agents")]
        public async Task<IActionResult> OrderManagement_AllOpsAgents()
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var allOpsAgents = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.OperationsRoleName)).Id })).ToList();
                var data = allOpsAgents.Select(e =>
                {
                    return new
                    {
                        Value = e.Id,
                        Text = e.FullName
                    };
                }).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("bank-details-by-supplier/{supplierId}")]
        public async Task<IActionResult> OrderManagement_GetAllBankDetailsBySupplier(int supplierId)
        {
            var supplier = await _customerService.GetCustomerByIdAsync(supplierId);
            if (supplier == null)
                return Ok(new { success = false, message = "Supplier not found." });

            var bankDetails = (await _customerService.GetAllBankDetailAsync(supplier.Id)).ToList();
            var result = (from bankDetail in bankDetails
                          select new { Value = bankDetail.Id, Text = bankDetail.BankName + ", " + bankDetail.AccountTitle + ", " + bankDetail.AccountNumber }).ToList();

            //result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Common.None") });

            return Ok(new { success = true, message = "", result });
        }

        [HttpGet("all-finance-agents")]
        public async Task<IActionResult> OrderManagement_AllFinanceAgents()
        {
            try
            {
                //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceDeliveryRequest))
                //    return Ok(new { success = false, message = "don't have permission" });

                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var allFinanaceAgents = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.FinanceRoleName)).Id })).ToList();
                var data = allFinanaceAgents.Select(e =>
                {
                    return new
                    {
                        Value = e.Id,
                        Text = e.FullName
                    };
                }).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Buyer Booker

        [AllowAnonymous]
        [HttpPost("buyer-registration")]
        public async Task<IActionResult> OrderManagement_BuyerRegistration([FromBody] AccountApiModel.BookerBuyerRegisterApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Register.Result.Disabled") });

            if (!CommonHelper.IsValidEmail(model.Email))
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Common.WrongEmail") });

            if (string.IsNullOrWhiteSpace(model.Phone))
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Fields.Username.Required") });

            var customer = new Customer
            {
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                Source = "Tijara-APP",
                CreatedBy = user.Id
            };
            await _customerService.InsertCustomerAsync(customer);

            var customerEmail = model.Email?.Trim();
            var customerUserName = model.Phone?.Trim();

            var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                customerUserName,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                (await _storeContext.GetCurrentStoreAsync()).Id,
                true,
                isBuyer: true);

            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (registrationResult.Success)
            {
                //form fields
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                if (_customerSettings.StreetAddressEnabled)
                    customer.StreetAddress = model.Address;
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                customer.Gst = model.BuyerGST;
                if (model.IndustryId > 0)
                    customer.IndustryId = model.IndustryId;
                if (model.BuyerType > 0)
                    customer.UserTypeId = model.BuyerType;

                await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.VatNumberAttribute, model.BuyerNTN);

                if (model.BookerCurrentLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredBookerCurrentLocationAttribute, $"{model.BookerCurrentLocation.Latitude},{model.BookerCurrentLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredBookerCurrentLocationAddressAttribute, model.BookerCurrentLocation.Location);
                }

                if (model.BuyerPinLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, $"{model.BuyerPinLocation.Latitude},{model.BuyerPinLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute, model.BuyerPinLocation.Location);
                }

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Company = customer.Company,
                    CountryId = customer.CountryId > 0
                            ? (int?)customer.CountryId
                            : null,
                    StateProvinceId = customer.StateProvinceId > 0
                            ? (int?)customer.StateProvinceId
                            : null,
                    County = customer.County,
                    City = customer.City,
                    Address1 = customer.StreetAddress,
                    Address2 = customer.StreetAddress2,
                    ZipPostalCode = customer.ZipPostalCode,
                    PhoneNumber = customer.Phone,
                    FaxNumber = customer.Fax,
                    CreatedOnUtc = customer.CreatedOnUtc
                };
                if (await _addressService.IsAddressValidAsync(defaultAddress))
                {
                    //some validation
                    if (defaultAddress.CountryId == 0)
                        defaultAddress.CountryId = null;
                    if (defaultAddress.StateProvinceId == 0)
                        defaultAddress.StateProvinceId = null;
                    //set default address
                    //customer.Addresses.Add(defaultAddress);

                    await _addressService.InsertAddressAsync(defaultAddress);
                    await _customerService.InsertCustomerAddressAsync(customer, defaultAddress);

                    customer.BillingAddressId = defaultAddress.Id;
                    customer.ShippingAddressId = defaultAddress.Id;

                    await _customerService.UpdateCustomerAsync(customer);
                }

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    await _workflowMessageService.SendBuyerRegisteredNotificationMessageAsync(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

                //send customer welcome message
                await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewBuyer",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewBuyer"), customer.Id), customer);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Account.Register.Result.Standard") });
            }

            return Ok(new { success = false, message = string.Join(",", registrationResult.Errors) });
        }

        [HttpGet("search-buyers/{name}")]
        public async Task<IActionResult> OrderManagement_SearchBuyers(string name = "")
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (string.IsNullOrEmpty(name))
                    return Ok(new { success = false, message = "Buyer name is required" });

                var buyers = (await _customerService.GetAllCustomersAsync(isActive: true,
                    customerRoleIds: new int[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id },
                    fullName: name)).ToList();

                var data = await buyers.SelectAwait(async b =>
                {
                    var buyerTypeAttributeId = await _genericAttributeService.GetAttributeAsync<string>(b, ZarayeCustomerDefaults.BuyerTypeIdAttribute);

                    return new
                    {
                        Id = b.Id,
                        FullName = b.FullName,
                        Email = b.Email,
                        Phone = b.Username,
                        BuyerType = (await _customerService.GetUserTypeByIdAsync(buyerTypeAttributeId != null ? Convert.ToInt32(buyerTypeAttributeId) : 0))?.Name
                    };
                }).ToListAsync();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("buyer-request-add")]
        public async Task<IActionResult> OrderManagement_AddBuyerRequest([FromBody] OrderManagementApiModel.BuyerRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (model.BuyerId <= 0)
                    return Ok(new { success = false, message = "Buyer is required" });

                if (model.BrandId <= 0)
                    return Ok(new { success = false, message = "Brand is required" });

                if (model.IndustryId <= 0)
                    return Ok(new { success = false, message = "Industry is required" });

                if (model.CategoryId <= 0)
                    return Ok(new { success = false, message = "Category is required" });

                if (model.ProductId <= 0)
                    return Ok(new { success = false, message = "Product is required" });

                if (model.CountryId <= 0)
                    return Ok(new { success = false, message = "Country model is required" });

                if (model.CityId <= 0)
                    return Ok(new { success = false, message = "City model is required" });

                if (model.AreaId <= 0)
                    return Ok(new { success = false, message = "Area is required" });

                var product = await _productService.GetProductByIdAsync(model.ProductId);
                if (product is null)
                    return Ok(new { success = false, message = "Product not found" });

                var industry = await _industryService.GetIndustryByIdAsync(model.IndustryId);
                if (industry is null)
                    return Ok(new { success = false, message = "Industry not found" });

                var category = await _categoryService.GetCategoryByIdAsync(model.CategoryId);
                if (category is null)
                    return Ok(new { success = false, message = "Category not found" });

                var brand = await _brandService.GetManufacturerByIdAsync(model.BrandId);
                if (brand is null)
                    return Ok(new { success = false, message = "Brand not found" });

                var warnings = new List<string>();
                var attributesData = new List<AttributesModel>();
                foreach (var attribute in model.AttributesData)
                {
                    attributesData.Add(new AttributesModel
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    });
                }
                var attributesXml = await OrderManagement_ConvertToXmlAsync(attributesData, product.Id);
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, product, 1, attributesXml));
                if (warnings.Any())
                    return Ok(new { success = false, message = warnings.ToArray() });

                if (model.Quantity <= 0)
                    return Ok(new { success = false, message = "Quantity is required" });

                List<object> list = new List<object>();

                var request = new Request
                {
                    BookerId = user.Id,
                    BuyerId = model.BuyerId,
                    IndustryId = model.IndustryId,
                    CategoryId = model.CategoryId,
                    ProductId = model.ProductId,
                    ProductAttributeXml = attributesXml,
                    RequestStatus = RequestStatus.Verified,
                    DeliveryDate = model.DeliveryDate,// (DateTime)_dateTimeHelper.ConvertToUtcTime(model.DeliveryDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()),
                    Quantity = model.Quantity,
                    BrandId = model.BrandId,
                    DeliveryAddress = model.DeliveryAddress,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                    AreaId = model.AreaId,
                    BusinessModelId = (int)BusinessModelEnum.Standard,
                    PaymentDuration = model.PaymentTerms,
                    Deleted = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(category.ExpiryDays > 0 ? category.ExpiryDays : 2),
                    Source = "Tijara",
                    RequestTypeId = (int)RequestTypeEnum.External
                };
                await _requestService.InsertRequestAsync(request);

                //generate and set custom request number
                request.CustomRequestNumber = _customNumberFormatter.GenerateRequestCustomNumber(request);
                await _requestService.UpdateRequestAsync(request);

                await _customerActivityService.InsertActivityAsync("AddBuyerRequest",
                    await _localizationService.GetResourceAsync("ActivityLog.AddBuyerRequest"), request);

                //send notification to all [supply] users
                //var allSuplyUsers = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.SupplyRoleName)).Id })).ToList();
                //foreach (var suplyUser in allSuplyUsers)
                //{
                //    SendPushNotifications(
                //        _pushNotificationService, _configuration, _logger,
                //        customerId: suplyUser.Id,
                //        title: await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceRequest.Title"),
                //        body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceRequest.Body"), request.CustomRequestNumber, await _localizationService.GetLocalizedAsync(industry, x => x.Name), await _localizationService.GetLocalizedAsync(product, x => x.Name)),
                //        entityId: request.Id, entityName: "BuyerRequest",
                //        data: new Dictionary<string, string>()
                //        {
                //            { "entityId", request.Id.ToString() },
                //            { "entityName", "BuyerRequest" },
                //            { "industryId", industry.Id.ToString() }
                //        });
                //}

                var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(request.Id);
                var totalRfqRemainingQty = request.Quantity - totalUsedQuantity;

                bool isAvailable = false;
                decimal remainingQty = 0;

                var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
                if (inventoryGroup is not null)
                {
                    var totalInventoryInboundQty = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.StockQuantity);
                    remainingQty = totalInventoryInboundQty - request.Quantity;
                    isAvailable = true;
                }

                //var jsonData = new
                //{
                //    type = "RequestAdd",
                //    message = $"you have receive a new request from {(await _customerService.GetCustomerByIdAsync(request.BuyerId))?.FullName} for {request.Quantity} {(await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name} {(await _productService.GetProductByIdAsync(request.ProductId))?.Name}"
                //};
                //_webSocketServer.NotifyAllClients(JsonConvert.SerializeObject(jsonData));

                return Ok(new
                {
                    success = true,
                    message = string.Format(await _localizationService.GetResourceAsync("TijaraApp.CustomMessage"), isAvailable ? remainingQty : request.Quantity, (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty, await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(request.BrandId)), x => x.Name)),
                    inventoryAvailable = isAvailable,
                    requestId = request.Id,
                    inventoryRemainingQty = isAvailable ? remainingQty <= 0 ? Math.Abs(remainingQty) : remainingQty : request.Quantity,
                    totalRfqRemainingQty = totalRfqRemainingQty
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reject-request")]
        public async Task<IActionResult> OrderManagement_RejectRequest([FromBody] OrderManagementApiModel.RejectModel model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                if (model.Id <= 0)
                    return Ok(new { success = false, message = "Request id is required" });

                var request = await _requestService.GetRequestByIdAsync(model.Id);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                if (string.IsNullOrWhiteSpace(model.RejectedReason))
                    return Ok(new { success = false, message = "Rejected reason is required" });

                if (model.RejectedReason == "Other" && string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                    return Ok(new { success = false, message = "Rejected other reason is required" });

                request.RequestStatus = RequestStatus.Cancelled;

                if (model.RejectedReason == "Other" && !string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                    request.RejectedReason = $"Other - {model.RejectedOtherReason}";
                else
                    request.RejectedReason = model.RejectedReason;

                await _requestService.UpdateRequestAsync(request);

                //Cancelled rfq
                var requestForQuotations = await _requestService.GetAllRequestForQuotationAsync(requestId: request.Id);
                foreach (var requestForQuotation in requestForQuotations)
                {
                    requestForQuotation.RequestForQuotationStatus = RequestForQuotationStatus.Cancelled;
                    await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

                    //Cancelled quotation
                    var quotations = await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id);
                    foreach (var quotation in quotations)
                    {
                        quotation.QuotationStatus = QuotationStatus.Cancelled;
                        await _quotationService.UpdateQuotationAsync(quotation);
                    }
                }

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("RequestForQuotation.Rejected.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("request-for-quotation-add")]
        public async Task<IActionResult> OrderManagement_AddRequestForQuotation([FromBody] OrderManagementApiModel.RequestForQuotationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (model.RequestId <= 0)
                    return Ok(new { success = false, message = "Request id is required" });

                var request = await _requestService.GetRequestByIdAsync(model.RequestId);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(request.Id);
                var totalRemaining = request.Quantity - totalUsedQuantity;

                if (model.Quantity > totalRemaining || model.Quantity <= 0)
                    return Ok(new { success = false, message = "request.Quantity.Error" });

                var requestForQuotation = new RequestForQuotation()
                {
                    RequestId = request.Id,
                    Quantity = model.Quantity,
                    RfqStatusId = (int)RequestForQuotationStatus.Verified,
                    CreatedOnUtc = DateTime.UtcNow,
                    BookerId = user.Id,
                    Source = "Tijara"
                };
                await _requestService.InsertRequestForQuotationAsync(requestForQuotation);

                //generate and set custom rfq number
                requestForQuotation.CustomRfqNumber = _customNumberFormatter.GenerateRequestForQuotationCustomNumber(requestForQuotation);
                await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

                //await _customerActivityService.InsertActivityAsync("AddRequestForQuotation",
                //    await _localizationService.GetResourceAsync("ActivityLog.AddRequestForQuotation"), request);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Buyer.RequestForQuotation.Add.Successfully"), rfqId = requestForQuotation.Id });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-request-history")]
        public async Task<IActionResult> OrderManagement_RequestHistory(int pageIndex = 0, int pageSize = 10)
        {
            var booker = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(booker) /*|| !await _customerService.IsBookerAsync(booker)*/)
                return Ok(new { success = false, message = "Booker not found" });

            List<Request> allBuyerRequests = new List<Request>();
            List<OrderManagementApiModel.Requests> activeBuyerRequests = new List<OrderManagementApiModel.Requests>();
            List<OrderManagementApiModel.Requests> pastBuyerRequets = new List<OrderManagementApiModel.Requests>();
            List<int> requestStatusIds = new List<int> { (int)RequestStatus.Verified, (int)RequestStatus.Processing, (int)RequestStatus.Approved, (int)RequestStatus.Complete, (int)RequestStatus.Cancelled, (int)RequestStatus.UnVerified, (int)RequestStatus.Expired };
            try
            {
                //Get all buyer request
                var requests = await _requestService.GetAllRequestAsync(pageIndex: pageIndex, pageSize: pageSize, bsIds: requestStatusIds/*, bookerId: booker.Id*/);
                allBuyerRequests = requests.ToList();
                if (allBuyerRequests.Any())
                {
                    var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);

                    //Get Active Request List Filter By Date 
                    var activeBuyerRequestDates = allBuyerRequests.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.RequestStatusId == (int)RequestStatus.Verified || x.RequestStatusId == (int)RequestStatus.Processing || x.RequestStatusId == (int)RequestStatus.Approved || x.RequestStatusId == (int)RequestStatus.Complete).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                    foreach (var activeBuyerRequestDate in activeBuyerRequestDates)
                    {
                        var model = new OrderManagementApiModel.Requests();
                        var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                        if (currentTime.ToString("dd/MM/yyyy") == activeBuyerRequestDate.Key)
                            model.Date = "Today";
                        else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == activeBuyerRequestDate.Key)
                            model.Date = "Yesterday";
                        else
                            model.Date = activeBuyerRequestDate.Key;

                        foreach (var activeBuyerRequest in activeBuyerRequestDate)
                        {
                            var product = await _productService.GetProductByIdAsync(activeBuyerRequest.ProductId);
                            if (product is null)
                                continue;

                            var category = await _categoryService.GetCategoryByIdAsync(activeBuyerRequest.CategoryId);
                            if (category is null)
                                continue;

                            var industry = await _industryService.GetIndustryByIdAsync(activeBuyerRequest.IndustryId);
                            if (industry is null)
                                continue;

                            var buyer = await _customerService.GetCustomerByIdAsync(activeBuyerRequest.BuyerId);
                            if (buyer is null)
                                continue;

                            var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(activeBuyerRequest.Id);
                            var totalRfqRemainingQty = activeBuyerRequest.Quantity - totalUsedQuantity;

                            bool isAvailable = false;
                            decimal remainingQty = 0;
                            int orderId = 0;
                            string customOrderNumber = "-";
                            string InventoryStatus = "";
                            decimal totalInventoryInboundQty = 0;

                            var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(activeBuyerRequest.ProductId, activeBuyerRequest.BrandId, activeBuyerRequest.ProductAttributeXml);
                            if (inventoryGroup is not null)
                            {
                                var inventoryInboundQty = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.StockQuantity);
                                totalInventoryInboundQty = inventoryInboundQty - (await _inventoryService.GetAllInventoryOutboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.OutboundQuantity);
                                remainingQty = totalInventoryInboundQty - activeBuyerRequest.Quantity;
                                isAvailable = true;
                            }

                            if (activeBuyerRequest.RequestStatusId == (int)RequestStatus.Processing || activeBuyerRequest.RequestStatusId == (int)RequestStatus.Approved || activeBuyerRequest.RequestStatusId == (int)RequestStatus.Complete)
                            {
                                var order = await _orderService.GetOrderByRequestIdAsync(activeBuyerRequest.Id);
                                if (order is not null)
                                {
                                    orderId = order.Id;
                                    customOrderNumber = order.CustomOrderNumber;
                                }
                            }

                            if(orderId == 0)
                            {
                                if (inventoryGroup is null || totalInventoryInboundQty == 0)
                                    InventoryStatus = "Not Available";
                                if (isAvailable && remainingQty >= 0)
                                    InventoryStatus = "Available";
                                else if (remainingQty < 0 && totalInventoryInboundQty > 0)
                                    InventoryStatus = "Partially available";
                            }
                            else
                                InventoryStatus = "Available";

                            model.Data.Add(new OrderManagementApiModel.BuyerRequestData
                            {
                                Id = activeBuyerRequest.Id,
                                CustomRequestNumber = activeBuyerRequest.CustomRequestNumber,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                IndustryId = industry.Id,
                                IndustryName = industry.Name,
                                BuyerId = buyer.Id,
                                BuyerName = buyer.FullName,
                                BrandName = activeBuyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(activeBuyerRequest.BrandId)), x => x.Name) : "-",
                                BrandId = activeBuyerRequest.BrandId,
                                Quantity = activeBuyerRequest.Quantity,
                                DeliveryAddress = activeBuyerRequest.DeliveryAddress,
                                //TotalQuotations = (await _directOrderCalculationService.GetAllQuotationAsync(requestId: activeBuyerRequest.Id, sbIds: new List<int> { (int)QuotationStatus.Verified, (int)QuotationStatus.QuotationSelected })).Count,
                                ExpiryDate = activeBuyerRequest.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(activeBuyerRequest.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                                StatusId = activeBuyerRequest.RequestStatusId,
                                Status = await _localizationService.GetLocalizedEnumAsync(activeBuyerRequest.RequestStatus),
                                AttributesInfo = await Booker_ParseAttributeXml(activeBuyerRequest.ProductAttributeXml),
                                InventoryStatus = InventoryStatus,
                                RemainingQuantity = isAvailable ? remainingQty : activeBuyerRequest.Quantity,
                                TotalRfqRemainingQty = totalRfqRemainingQty,
                                OrderId = orderId,
                                CustomOrderNumber = customOrderNumber,
                            });
                        }
                        activeBuyerRequests.Add(model);
                    }

                    //Get Active Request List Filter By Date 
                    var pastBuyerRequests = allBuyerRequests.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.RequestStatusId == (int)RequestStatus.UnVerified || x.RequestStatusId == (int)RequestStatus.Cancelled || x.RequestStatusId == (int)RequestStatus.Expired).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                    foreach (var pastRequest in pastBuyerRequests)
                    {
                        var model = new OrderManagementApiModel.Requests();
                        if (currentTime.ToString("dd/MM/yyyy") == pastRequest.Key)
                            model.Date = "Today";
                        else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == pastRequest.Key)
                            model.Date = "Yesterday";
                        else
                            model.Date = pastRequest.Key;

                        foreach (var pastBuyerRequest in pastRequest)
                        {
                            var product = await _productService.GetProductByIdAsync(pastBuyerRequest.ProductId);
                            if (product is null)
                                continue;

                            var category = await _categoryService.GetCategoryByIdAsync(pastBuyerRequest.CategoryId);
                            if (category is null)
                                continue;

                            var industry = await _industryService.GetIndustryByIdAsync(pastBuyerRequest.IndustryId);
                            if (industry is null)
                                continue;

                            var buyer = await _customerService.GetCustomerByIdAsync(pastBuyerRequest.BuyerId);
                            if (buyer is null)
                                continue;

                            var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(pastBuyerRequest.Id);
                            var totalRfqRemainingQty = pastBuyerRequest.Quantity - totalUsedQuantity;

                            bool isAvailable = false;
                            decimal remainingQty = 0;
                            bool isGroup = false;
                            int orderId = 0;
                            string customOrderNumber = "-";
                            string InventoryStatus = "";
                            decimal totalInventoryInboundQty = 0;

                            var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(pastBuyerRequest.ProductId, pastBuyerRequest.BrandId, pastBuyerRequest.ProductAttributeXml);
                            if (inventoryGroup is not null)
                            {
                                var inventoryInboundQty = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.StockQuantity);
                                totalInventoryInboundQty = inventoryInboundQty - (await _inventoryService.GetAllInventoryOutboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.OutboundQuantity);
                                remainingQty = totalInventoryInboundQty - pastBuyerRequest.Quantity;
                                isAvailable = true;
                            }

                            if (pastBuyerRequest.RequestStatusId == (int)RequestStatus.Processing || pastBuyerRequest.RequestStatusId == (int)RequestStatus.Approved || pastBuyerRequest.RequestStatusId == (int)RequestStatus.Complete)
                            {
                                var order = await _orderService.GetOrderByRequestIdAsync(pastBuyerRequest.Id);
                                if (order is not null)
                                {
                                    orderId = order.Id;
                                    customOrderNumber = order.CustomOrderNumber;
                                }
                            }

                            if (orderId == 0)
                            {
                                if (inventoryGroup is null || totalInventoryInboundQty == 0)
                                    InventoryStatus = "Not Available";
                                if (isAvailable && remainingQty >= 0)
                                    InventoryStatus = "Available";
                                else if (remainingQty < 0 && totalInventoryInboundQty > 0)
                                    InventoryStatus = "Partially available";
                            }
                            else
                                InventoryStatus = "Available";

                            model.Data.Add(new OrderManagementApiModel.BuyerRequestData
                            {
                                Id = pastBuyerRequest.Id,
                                CustomRequestNumber = pastBuyerRequest.CustomRequestNumber,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                IndustryId = industry.Id,
                                IndustryName = industry.Name,
                                BuyerId = buyer.Id,
                                BuyerName = buyer.FullName,
                                BrandName = pastBuyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(pastBuyerRequest.BrandId)), x => x.Name) : "-",
                                BrandId = pastBuyerRequest.BrandId,
                                Quantity = pastBuyerRequest.Quantity,
                                DeliveryAddress = pastBuyerRequest.DeliveryAddress,
                                //TotalQuotations = (await _sellerBidService.GetAllSellerBidAsync(buyerRequestId: pastBuyerRequest.Id, sbIds: new List<int> { (int)RequestStatus.QuotedToBuyer, (int)RequestStatus. })).Count,
                                ExpiryDate = pastBuyerRequest.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(pastBuyerRequest.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                                StatusId = pastBuyerRequest.RequestStatusId,
                                Status = await _localizationService.GetLocalizedEnumAsync(pastBuyerRequest.RequestStatus),
                                AttributesInfo = await Booker_ParseAttributeXml(pastBuyerRequest.ProductAttributeXml),
                                InventoryStatus = InventoryStatus,
                                RemainingQuantity = isAvailable ? remainingQty : pastBuyerRequest.Quantity,
                                TotalRfqRemainingQty = totalRfqRemainingQty,
                                OrderId = orderId,
                                CustomOrderNumber = customOrderNumber,
                            });
                        }
                        pastBuyerRequets.Add(model);
                    }

                    var data = new List<object>();
                    if (activeBuyerRequests.Any())
                    {
                        data.Add(new
                        {
                            DataType = await _localizationService.GetResourceAsync("Buyer.ActiveRequestForQuotation"),
                            Data = activeBuyerRequests
                        });
                    }
                    if (pastBuyerRequets.Any())
                    {
                        data.Add(new
                        {
                            DataType = await _localizationService.GetResourceAsync("Buyer.PastRequestForQuotation"),
                            Data = pastBuyerRequets
                        });
                    }
                    return Ok(new { success = true, message = "", data, totalPages = requests.TotalPages, currentPage = requests.PageIndex });
                }

                return Ok(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-request/{requestId}")]
        public async Task<IActionResult> OrderManagement_GetBuyerRequest(int requestId)
        {
            if (requestId <= 0)
                return Ok(new { success = false, message = "Buyer request id is required" });

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
            if (buyerRequest is null)
                return Ok(new { success = false, message = "Buyer request not found" });

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null || product.Id != buyerRequest.ProductId)
                return Ok(new { success = false, message = "Buyer request product not found" });

            var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
            if (industry is null)
                return Ok(new { success = false, message = "Buyer request industry not found" });

            try
            {
                var data = new
                {
                    Id = buyerRequest.Id,
                    CustomRequestNumber = buyerRequest.CustomRequestNumber,
                    BuyerId = buyerRequest.BuyerId,
                    BuyerName = await _customerService.GetCustomerFullNameAsync(buyerRequest.BuyerId),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    IndustryId = industry.Id,
                    IndustryName = industry.Name,
                    Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                    BrandId = buyerRequest.BrandId,
                    Qty = buyerRequest.Quantity,
                    QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                    DeliveryAddress = buyerRequest.DeliveryAddress,
                    DeliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM, yyyy"),
                    //DeliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("ddd MMM MM yyyy"),
                    //TotalQuotations = await _directOrderCalculationService.GetQuotationsCountAsync(bsIds: new List<int> { (int)QuotationStatus.Verified }, buyerRequestId: buyerRequest.Id),
                    StatusId = buyerRequest.RequestStatusId,
                    Status = await _localizationService.GetLocalizedEnumAsync(buyerRequest.RequestStatus),
                    //Comment = buyerRequest.Comment,
                    ProductAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-requests")]
        public async Task<IActionResult> OrderManagement_GetAllRequests(int industryId, int pageIndex = 0, int pageSize = 10)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (industryId <= 0)
                return Ok(new { success = false, message = "Industry id is required" });

            var industry = await _industryService.GetIndustryByIdAsync(industryId);
            if (industry == null)
                return Ok(new { success = false, message = "Industry not found" });

            List<object> list = new List<object>();
            //IPagedList<object> pagedList = new PagedList<object>(list, pageIndex, pageSize);

            try
            {
                var requests = await _requestService.GetAllRequestAsync(pageIndex: pageIndex, pageSize: pageSize, bsIds: new List<int> { (int)RequestStatus.Verified, (int)RequestStatus.Processing, (int)RequestStatus.Approved, (int)RequestStatus.Complete },
                    industryId: industryId, excludeRfqForApp: true);

                foreach (var request in requests)
                {
                    //if (request.RequestStatusId == (int)RequestStatus.Processing || request.RequestStatusId == (int)RequestStatus.Approved || request.RequestStatusId == (int)RequestStatus.Complete)
                    //{
                    //    var order = await _orderService.GetOrderByRequestIdAsync(request.Id);
                    //    if (order is not null)
                    //    {
                    //        orderId = order.Id;
                    //        customOrderNumber = order.CustomOrderNumber;
                    //    }
                    //}

                    var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(request.Id);
                    var totalRfqRemainingQty = request.Quantity - totalUsedQuantity;

                    //if (totalRfqRemainingQty == 0)
                    //    continue;

                    var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
                    if (category is null)
                        continue;

                    var product = await _productService.GetProductByIdAsync(request.ProductId);
                    if (product is null)
                        continue;

                    var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);
                    if (buyer is null)
                        continue;

                    bool isAvailable = false;
                    decimal remainingQty = 0;
                    bool isGroup = false;
                    //int orderId = 0;
                    //string customOrderNumber = "-";
                    string InventoryStatus = "";
                    decimal totalInventoryInboundQty = 0;

                    var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
                    if (inventoryGroup is not null)
                    {
                        var inventoryInboundQty = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.StockQuantity);
                        totalInventoryInboundQty = inventoryInboundQty - (await _inventoryService.GetAllInventoryOutboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.OutboundQuantity);
                        remainingQty = totalInventoryInboundQty - request.Quantity;
                        isAvailable = true;
                    }

                    if (inventoryGroup is null || totalInventoryInboundQty == 0)
                        InventoryStatus = "Not Available";
                    if (isAvailable && remainingQty >= 0)
                        InventoryStatus = "Available";
                    else if (remainingQty < 0 && totalInventoryInboundQty > 0)
                        InventoryStatus = "Partially available";

                    list.Add(new
                    {
                        Id = request.Id,
                        CustomRequestNumber = request.CustomRequestNumber,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        IndustryId = industry.Id,
                        IndustryName = industry.Name,
                        BuyerId = buyer.Id,
                        BuyerName = buyer.FullName,
                        BrandName = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(request.BrandId)), x => x.Name) : "-",
                        BrandId = request.BrandId,
                        Quantity = request.Quantity,
                        DeliveryAddress = request.DeliveryAddress,
                        ExpiryDate = request.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(request.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                        StatusId = request.RequestStatusId,
                        Status = await _localizationService.GetLocalizedEnumAsync(request.RequestStatus),
                        AttributesInfo = await Booker_ParseAttributeXml(request.ProductAttributeXml),
                        InventoryStatus = InventoryStatus,
                        RemainingQuantity = isAvailable ? remainingQty : request.Quantity,
                        TotalRfqRemainingQty = totalRfqRemainingQty,
                    });
                }

                return Ok(new { success = true, message = "", data = list, totalPages = requests.TotalPages, currentPage = requests.PageIndex });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-purchase-order-detail-for-shipment-request/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_GetPurchaseOrderDetailForDeliveryRequest(int expectedShipmentId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(expectedShipmentId);
                if (expectedShipment is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                var order = await _orderService.GetOrderByIdAsync(expectedShipment.OrderId);
                if (order is null)
                    return Ok(new { success = false, message = "Order not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
                if (orderItem is null)
                    return Ok(new { success = false, message = "Order item not found" });

                var quotation = await _quotationService.GetQuotationByIdAsync(order.QuotationId.Value);
                if (quotation is null)
                    return Ok(new { success = false, message = "Quotation not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null || product.Id != buyerRequest.ProductId)
                    return Ok(new { success = false, message = "Buyer request product not found" });

                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                if (customer is null)
                    return Ok(new { success = false, message = "Customer not found" });

                //decimal remaining = 0;
                //var shipments = ((await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Where(x => x.ShippedDateUtc.HasValue)).ToList();
                //foreach (var shipment in shipments)
                //    remaining += (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).Sum(x => x.Quantity);

                var data = new object();
                data = new
                {
                    orderId = order.Id,
                    customOrderNumber = order.CustomOrderNumber,
                    buyerId = customer.Id,
                    buyerName = customer.FullName,
                    //suplierId = customer.Id,
                    //suplierName = customer.FullName,
                    brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                    leftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, expectedShipment.Id),
                    //leftQuantity = expectedShipment.ExpectedQuantity - remaining,
                    totalQuantity = quotation.Quantity,
                    expectedShipmentQuantity = expectedShipment.ExpectedQuantity,
                    ProductName = product.Name,
                    ProductAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    countryId = buyerRequest.CountryId,
                    stateId = buyerRequest.CityId,
                    areaId = buyerRequest.AreaId,
                    streetAddress = buyerRequest.DeliveryAddress,
                    contactNo = customer.Phone
                };

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = true, message = ex.Message });
            }
        }

        [HttpGet("get-sale-order-detail-for-shipment-request/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_GetSaleOrderDetailForDeliveryRequest(int expectedShipmentId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(expectedShipmentId);
                if (expectedShipment is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                var order = await _orderService.GetOrderByIdAsync(expectedShipment.OrderId);
                if (order is null)
                    return Ok(new { success = false, message = "Order not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
                if (orderItem is null)
                    return Ok(new { success = false, message = "Order item not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null || product.Id != buyerRequest.ProductId)
                    return Ok(new { success = false, message = "Buyer request product not found" });

                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                if (customer is null)
                    return Ok(new { success = false, message = "Customer not found" });

                //decimal remaining = 0;
                //var shipments = ((await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Where(x => x.ShippedDateUtc.HasValue)).ToList();
                //foreach (var shipment in shipments)
                //    remaining += (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).Sum(x => x.Quantity);

                var data = new object();
                data = new
                {
                    orderId = order.Id,
                    customOrderNumber = order.CustomOrderNumber,
                    buyerId = customer.Id,
                    buyerName = customer.FullName,
                    //suplierId = customer.Id,
                    //suplierName = customer.FullName,
                    brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                    leftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, expectedShipment.Id),
                    //leftQuantity = expectedShipment.ExpectedQuantity - remaining,
                    totalQuantity = buyerRequest.Quantity,
                    expectedShipmentQuantity = expectedShipment.ExpectedQuantity,
                    ProductName = product.Name,
                    ProductAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    countryId = buyerRequest.CountryId,
                    stateId = buyerRequest.CityId,
                    areaId = buyerRequest.AreaId,
                    streetAddress = buyerRequest.DeliveryAddress,
                    contactNo = customer.Phone
                };

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = true, message = ex.Message });
            }
        }

        [HttpPost("add-shipment-request")]
        public async Task<IActionResult> OrderManagement_AddShipmentRequest([FromBody] OrderManagementApiModel.OrderDeliveryRequestModel model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (model.ExpectedShipmentId <= 0)
                    return Ok(new { success = false, message = "Expected shipment id is required" });

                if (model.CountryId <= 0)
                    return Ok(new { success = false, message = "Country is required" });

                if (model.CityId <= 0)
                    return Ok(new { success = false, message = "City is required" });

                if (model.AreaId <= 0)
                    return Ok(new { success = false, message = "Area is required" });

                if (model.AgentId <= 0)
                    return Ok(new { success = false, message = "Agent is required" });

                if (model.Quantity <= 0)
                    return Ok(new { success = false, message = "Quantity is required" });

                if (model.BagsDirectlyFromWarehouse && model.WarehouseId == 0)
                    return Ok(new { success = false, message = "Warehouse is required" });

                var deliveryScedule = await _orderService.GetDeliveryScheduleByIdAsync(model.ExpectedShipmentId);
                if (deliveryScedule is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                var order = await _orderService.GetOrderByIdAsync(deliveryScedule.OrderId);
                if (order is null || order.Deleted)
                    return Ok(new { success = false, message = "Order not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
                if (orderItem is null)
                    return Ok(new { success = false, message = "Order item not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                //decimal remaining = 0;
                //var shipments = ((await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Where(x => x.ShippedDateUtc.HasValue)).ToList();
                //foreach (var shipment in shipments)
                //    remaining += (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).Sum(x => x.Quantity);

                decimal totalLeftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, deliveryScedule.Id);
                //decimal totalLeftQuantity = deliveryScedule.ExpectedQuantity - remaining;
                if (totalLeftQuantity <= 0)
                    return Ok(new { success = false, message = "There is no left quantity " });

                if (model.Quantity > totalLeftQuantity)
                    return Ok(new { success = false, message = "Left Quantity ", leftQuantity = totalLeftQuantity });

                var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
                if (industry is null)
                    return Ok(new { success = false, message = "Industry not found" });

                var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                if (category is null)
                    return Ok(new { success = false, message = "Category not found" });

                List<object> list = new List<object>();

                var orderDeliveryRequest = new OrderDeliveryRequest
                {
                    OrderId = order.Id,
                    OrderDeliveryScheduleId = deliveryScedule.Id,
                    StatusId = (int)OrderDeliveryRequestEnum.Pending,
                    BagsDirectlyFromSupplier = model.BagsDirectlyFromSupplier,
                    BagsDirectlyFromWarehouse = model.BagsDirectlyFromWarehouse,
                    WarehouseId = model.WarehouseId,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                    AreaId = model.AreaId,
                    StreetAddress = model.StreetAddress,
                    ContactNumber = model.ContactNumber,
                    Quantity = model.Quantity,
                    //ShipmentDateUtc = model.ShipmentDateUtc,
                    Deleted = false,
                    AgentId = model.AgentId,
                    TicketExpiryDate = DateTime.UtcNow.AddDays(category.TicketExpiryDays > 0 ? category.TicketExpiryDays : 1),
                    TicketPirority = category.TicketPirority > 0 ? category.TicketPirority : (int)TicketEnum.Medium
                };
                await _orderService.InsertOrderDeliveryRequestAsync(orderDeliveryRequest);

                //send notification to ops agent user
                //SendPushNotifications(
                //   _pushNotificationService, _configuration, _logger,
                //   customerId: orderDeliveryRequest.AgentId,
                //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceDeliveryRequest.Title"),
                //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceDeliveryRequest.Body"), await _customerService.GetCustomerFullNameAsync(orderDeliveryRequest.AgentId), await _localizationService.GetLocalizedAsync(industry, x => x.Name)),
                //   entityId: orderDeliveryRequest.Id, entityName: "OrderDeliveryRequest",
                //   data: new Dictionary<string, string>()
                //   {
                //        { "entityId", orderDeliveryRequest.Id.ToString() },
                //        { "entityName", "OrderDeliveryRequest" },
                //        { "orderDeliveryRequestId", orderDeliveryRequest.Id.ToString() }
                //   });

                ////send notification to all [OpsHead] users
                //var allOpsHeadUsers = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.OpsHeadRoleName)).Id })).ToList();
                //foreach (var opsHeadUser in allOpsHeadUsers)
                //{
                //    SendPushNotifications(
                //   _pushNotificationService, _configuration, _logger,
                //   customerId: opsHeadUser.Id,
                //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceDeliveryRequest.Title"),
                //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceDeliveryRequest.Body"), await _customerService.GetCustomerFullNameAsync(orderDeliveryRequest.AgentId), await _localizationService.GetLocalizedAsync(industry, x => x.Name)),
                //   entityId: orderDeliveryRequest.Id, entityName: "OrderDeliveryRequest",
                //   data: new Dictionary<string, string>()
                //   {
                //        { "entityId", orderDeliveryRequest.Id.ToString() },
                //        { "entityName", "OrderDeliveryRequest" },
                //        { "orderDeliveryRequestId", orderDeliveryRequest.Id.ToString() }
                //   });
                //}

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Order.Delivery.Request.Created.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sale-order-delivery-request-detail/{expectedShipmentRequestId}")]
        public async Task<IActionResult> OrderManagement_SaleOrderDeliveryRequestDeatil(int expectedShipmentRequestId, string type = "ExpectedShipment")
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                if (string.IsNullOrWhiteSpace(type))
                    return Ok(new { success = false, message = "type is required" });

                var data = new object();
                if (type == "ExpectedShipment")
                {
                    var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(expectedShipmentRequestId);
                    if (deliveryRequest is null)
                        return Ok(new { success = false, message = "delivery request not found" });

                    var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                    if (order is null)
                        return Ok(new { success = false, message = "delivery request order not found" });

                    var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                    if (buyerRequest is null)
                        return Ok(new { success = false, message = "Buyer Request not found" });

                    var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                    if (product is null || product.Id != buyerRequest.ProductId)
                        return Ok(new { success = false, message = "Buyer request product not found" });

                    data = new
                    {
                        id = deliveryRequest.Id,
                        orderId = deliveryRequest.OrderId,
                        orderCustomNumber = order.CustomOrderNumber,
                        shipmentId = deliveryRequest.StatusId == (int)OrderDeliveryRequestEnum.Complete ? (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id))?.Id : 0,
                        wareHouseId = deliveryRequest.WarehouseId,
                        wareHouseName = deliveryRequest.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(deliveryRequest.WarehouseId)).Name : "",
                        statusId = deliveryRequest.StatusId,
                        status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                        bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                        bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                        countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                        countryId = deliveryRequest.CountryId,
                        cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                        cityId = deliveryRequest.CityId,
                        areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                        areaId = deliveryRequest.AreaId,
                        streetaddress = deliveryRequest.StreetAddress,
                        contactnumber = deliveryRequest.ContactNumber,
                        totalQuantity = buyerRequest.Quantity,
                        deliveredQuantity = deliveryRequest.Quantity,
                        agentId = deliveryRequest.AgentId,
                        agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                        timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                        priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                        requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                        buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                        brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                        rejectedReason = deliveryRequest.RejectedReason,
                        productName = product.Name,
                        productAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    };
                }

                if (type == "Shipment")
                {
                    var shipment = await _shipmentService.GetShipmentByIdAsync(expectedShipmentRequestId);
                    if (shipment is null)
                        return Ok(new { success = false, message = "Shipment not found" });

                    var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                    if (deliveryRequest is null)
                        return Ok(new { success = false, message = "delivery request not found" });

                    var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                    if (order is null)
                        return Ok(new { success = false, message = "delivery request order not found" });

                    var request = await _requestService.GetRequestByIdAsync(order.RequestId);
                    if (request is null)
                        return Ok(new { success = false, message = "Buyer Request not found" });

                    var product = await _productService.GetProductByIdAsync(request.ProductId);
                    if (product is null || product.Id != request.ProductId)
                        return Ok(new { success = false, message = "Buyer request product not found" });

                    var inventories = new List<object>();

                    //Prepare inventories
                    var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
                    if (inventoryGroup is not null)
                    {
                        var inventoryInbounds = new List<InventoryInbound>();

                        if (request.BusinessModelId == (int)BusinessModelEnum.Broker)
                            inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Where(x => x.InventoryInboundStatusEnum == InventoryInboundStatusEnum.Physical && x.BusinessModelId == (int)BusinessModelEnum.Broker).OrderBy(x => x.Id).ToList();
                        else
                            inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Where(x => x.InventoryInboundStatusEnum == InventoryInboundStatusEnum.Physical && x.BusinessModelId != (int)BusinessModelEnum.Broker).OrderBy(x => x.Id).ToList();

                        foreach (var inventoryInbound in inventoryInbounds)
                        {
                            if (inventoryInbound.ShipmentId.HasValue)
                            {
                                var inventoryShipment = await _shipmentService.GetShipmentByIdAsync(inventoryInbound.ShipmentId.Value);
                                if (inventoryShipment is not null)
                                {
                                    if (inventoryShipment.IsDirectOrder && inventoryInbound.InventoryInboundStatusEnum == InventoryInboundStatusEnum.Virtual)
                                    {
                                        if (inventoryShipment.BuyerId != order.CustomerId)
                                            continue;
                                    }
                                }
                            }

                            var outboundQty = (await _inventoryService.GetAllInventoryOutboundsAsync(InventoryInboundId: inventoryInbound.Id)).Sum(x => x.OutboundQuantity);
                            var balanceQuantity = inventoryInbound.StockQuantity - outboundQty;
                            if (balanceQuantity > 0)
                                inventories.Add(new { InventoryId = inventoryInbound.Id, TotalQuantity = inventoryInbound.StockQuantity, BalanceQuantity = balanceQuantity, OutboundQuantity = 0 });
                        }
                    }
                    var picture = (await _pictureService.GetPictureByIdAsync(shipment.PictureId));
                    data = new
                    {
                        id = deliveryRequest.Id,
                        orderId = deliveryRequest.OrderId,
                        orderCustomNumber = order.CustomOrderNumber,
                        wareHouseId = shipment.WarehouseId,
                        wareHouseName = shipment.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(shipment.WarehouseId)).Name : "",
                        statusId = deliveryRequest.StatusId,
                        status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                        bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                        bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                        countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                        countryId = deliveryRequest.CountryId,
                        cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                        cityId = deliveryRequest.CityId,
                        areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                        areaId = deliveryRequest.AreaId,
                        streetaddress = deliveryRequest.StreetAddress,
                        contactnumber = deliveryRequest.ContactNumber,
                        totalQuantity = request.Quantity,
                        deliveredQuantity = deliveryRequest.Quantity,
                        shipmentdate = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("MM/dd/yyyy h:mm:ss tt") /*deliveryRequest.ShipmentDateUtc*/,
                        agentId = deliveryRequest.AgentId,
                        agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                        timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                        priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                        requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                        buyerName = (await _customerService.GetCustomerByIdAsync(customerId: request.BuyerId))?.FullName,
                        brand = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(request.BrandId)), x => x.Name) : "-",
                        rejectedReason = deliveryRequest.RejectedReason,
                        productName = product.Name,
                        productAttributesInfo = await Booker_ParseAttributeXml(request.ProductAttributeXml),
                        canShip = shipment.ShippedDateUtc.HasValue,
                        canDelivered = shipment.DeliveryDateUtc.HasValue,
                        inventories,

                        //Shipped Detail
                        shippedDetail = new
                        {
                            expectedDateShipped = shipment.ExpectedDateShipped.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ExpectedDateShipped.Value, DateTimeKind.Utc)).ToString() : "",
                            expectedDateDelivered = shipment.ExpectedDateDelivered.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ExpectedDateDelivered.Value, DateTimeKind.Utc)).ToString() : "",
                            expectedDeliveryCost = shipment.ExpectedDeliveryCost,
                            expectedQuantity = shipment.ExpectedQuantity,
                            transporterId = shipment.TransporterId,
                            transporterName = shipment.TransporterId > 0 ? (await _customerService.GetCustomerByIdAsync(shipment.TransporterId))?.FullName : "",
                            vehicleId = shipment.VehicleId,
                            transportvehicleName = (await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId)) != null ? $"{(await _customerService.GetVehiclePortfolioByIdAsync((await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId)).VehicleId)).Name} - {(await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId))?.VehicleNumber}" : null,
                            vehicleNumber = shipment.VehicleNumber,
                            routeType = shipment.RouteTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.RouteType) : "",
                            deliveryStatus = await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryStatus),
                            pickupAddress = shipment.PickupAddress,
                            actualShippedQuantity = shipment.ActualShippedQuantity,
                            actualShippedQuantityReason = shipment.ActualShippedQuantityReason,
                            costOnZaraye = shipment.CostOnZarayeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.CostOnZaraye) : "",
                            shippedDateUtc = shipment.ShippedDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString() : "",
                        },

                        //Delivered Detail
                        deliveredDetail = new
                        {
                            deliveryDateUtc = shipment.DeliveryDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString() : "",
                            shipmentDeliveryAddress = shipment.ShipmentDeliveryAddress,
                            picture = shipment.PictureId > 0 ? await _pictureService.GetPictureUrlAsync(shipment.PictureId) : "",
                            fullPictureUrl = picture != null ? (await _pictureService.GetPictureUrlAsync(picture)).Url : "",
                            transporterType = shipment.TransporterTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.TransporterType) : "",
                            deliveryType = shipment.DeliveryTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryType) : "",
                            deliveryTiming = shipment.DeliveryTimingId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryTimingEnum) : "",
                            deliveryDelayedReason = shipment.DeliveryDelayedReasonId > 0 ? (await _shipmentService.GetDeliveryTimeReasonByIdAsync(shipment.DeliveryDelayedReasonId))?.Name : "",
                            deliveryCostType = shipment.DeliveryCostTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryCostType) : "",
                            deliveryCost = shipment.DeliveryCost,
                            warehouseId = shipment.WarehouseId,
                            freightCharges = shipment.FreightCharges,
                            labourCharges = shipment.LabourCharges,
                            deliveryCostReason = shipment.DeliveryCostReasonId > 0 ? (await _shipmentService.GetDeliveryCostReasonByIdAsync(shipment.DeliveryCostReasonId))?.Name : "",
                            actualDeliveredQuantity = shipment.ActualDeliveredQuantity,
                            actualDeliveredQuantityReason = shipment.ActualDeliveredQuantityReason,
                            labourType = shipment.LabourTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.LabourType) : "",
                        },
                    };
                }

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Direct Sale Order

        [HttpGet("check-direct-sale-order/{requestId}")]
        public async Task<IActionResult> OrderManagement_CheckDirectSaleOrderExist(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "User not found" });

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                return Ok(new { success = false, message = "Request not found" });

            try
            {
                var directOrder = await _orderService.GetDirectOrderByRequestId(request.Id);
                if (directOrder is null)
                    return Ok(new { success = true, message = "Data not found", data = false });

                return Ok(new { success = true, message = "Data found", data = true, directOrderId = directOrder.Id });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    data = false
                });
            }
        }

        [HttpPost("direct-new-sale-order-process/{requestId}")]
        public async Task<IActionResult> OrderManagement_DirectSaleOrderProcess(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                return Ok(new { success = false, message = "Buyer request not found" });

            try
            {
                var directOrder = await _orderService.GetDirectOrderByRequestId(request.Id);
                if (directOrder is not null)
                {
                    var directOrderDeliverySchedules = await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id);
                    foreach (var directOrderDeliverySchedule in directOrderDeliverySchedules)
                        await _orderService.DeleteDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                    //Remove direct order supplier info
                    var directOrderCalculations = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                    foreach (var directOrderCalculation in directOrderCalculations)
                        await _orderService.DeleteDirectOrderCalculationAsync(directOrderCalculation);

                    var directCogsInventoryTaggings = (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id));
                    foreach (var tagging in directCogsInventoryTaggings)
                    {
                        //delete Direct Cogs Inventory Taggings
                        await _inventoryService.DeleteDirectCogsInventoryTaggingAsync(tagging);
                    }

                    await _orderService.DeleteDirectOrderAsync(directOrder);
                }
                //New Direct Order
                var newDirectOrder = new DirectOrder
                {
                    BookerId = user.Id,
                    BuyerId = request.BuyerId,
                    RequestId = request.Id,
                    TransactionModelId = request.BusinessModelId,
                    OrderTypeId = (int)OrderType.SaleOrder
                };
                await _orderService.InsertDirectOrderAsync(newDirectOrder);
                //Add default direct order delivery schedule 
                await _orderService.InsertDirectOrderDeliveryScheduleAsync(new DirectOrderDeliverySchedule
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpectedDeliveryDateUtc = null,
                    ExpectedShipmentDateUtc = null,
                    ExpectedDeliveryCost = 0,
                    ExpectedQuantity = null,
                    DirectOrderId = newDirectOrder.Id
                });
                //Add default direct order calculation 
                await _orderService.InsertDirectOrderCalculationAsync(new DirectOrderCalculation
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    DirectOrderId = newDirectOrder.Id,
                    Quantity = request.Quantity
                });
                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("New.Direct.Order.Created"), directOrderId = newDirectOrder.Id });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sale-order-form-load/{requestId}")]
        public async Task<IActionResult> OrderManagement_GetSaleOrderFormLoad(int requestId)
        {
            if (requestId <= 0)
                return Ok(new { success = false, message = "Buyer request id is required" });

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
            if (buyerRequest is null)
                return Ok(new { success = false, message = "Buyer request not found" });

            var directOrder = await _orderService.GetDirectOrderByRequestId(buyerRequest.Id);
            if (directOrder is null)
                return Ok(new { success = false, message = "Direct order not found" });

            try
            {
                var data = new
                {
                    requestId = buyerRequest.Id,
                    countryId = buyerRequest.CountryId,
                    countryName = buyerRequest.CountryId > 0 ? (await _countryService.GetCountryByIdAsync(buyerRequest.CountryId))?.Name : "-",
                    cityId = buyerRequest.CityId,
                    cityName = buyerRequest.CityId > 0 ? (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.CityId))?.Name : "-",
                    areaId = buyerRequest.CountryId,
                    areaName = buyerRequest.AreaId > 0 ? (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.AreaId))?.Name : "-",
                    quantity = buyerRequest.Quantity,
                    buyerName = (await _customerService.GetCustomerByIdAsync(buyerRequest.BuyerId))?.FullName,
                    businessModel = (await _localizationService.GetLocalizedEnumAsync(directOrder.TransactionModelEnum)),
                    businessModelId = directOrder.TransactionModelId,
                    streetAddress = directOrder.StreetAddress,
                    industrtyId = buyerRequest.IndustryId,
                    PinLocation = new
                    {
                        location = directOrder.PinLocation_Location,
                        longitude = directOrder.PinLocation_Longitude,
                        latitude = directOrder.PinLocation_Latitude
                    },
                    interGeography = directOrder.InterGeography.HasValue ? directOrder.InterGeography.Value : false,
                    expectedShipments = (await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id)).Select(async d =>
                    {
                        return new
                        {
                            id = d.Id,
                            expectedDeliveryDateUtc = d.ExpectedDeliveryDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedDeliveryDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                            expectedShipmentDateUtc = d.ExpectedShipmentDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedShipmentDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                            expectedQuantity = d.ExpectedQuantity.HasValue ? d.ExpectedQuantity.ToString() : "",
                            expectedDeliveryCost = d.ExpectedDeliveryCost.HasValue ? d.ExpectedDeliveryCost.ToString() : "",
                            createdOnUtc = d.CreatedOnUtc,
                        };
                    }).Select(t => t.Result).ToList(),
                    directOrderId = directOrder.Id
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("sale-order-info")]
        public async Task<IActionResult> DirectOrder_SaleOrderInfo([FromBody] DirectOrderApiModel.DirectOrderInfoModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (model.RequestId <= 0)
                return Ok(new { success = false, message = "Buyer request id is required" });

            var buyerRequest = await _requestService.GetRequestByIdAsync(model.RequestId);
            if (buyerRequest is null)
                return Ok(new { success = false, message = "Buyer request not found" });
            try
            {
                var directOrder = await _orderService.GetDirectOrderByRequestId(model.RequestId);
                if (directOrder is null)
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("DirectOrder.NotFound") });

                //Update direct order
                if (model.StreetAddress is not null)
                    directOrder.StreetAddress = model.StreetAddress;

                if (model.TransactionModelId is not null && model.TransactionModelId.HasValue)
                {
                    directOrder.TransactionModelId = model.TransactionModelId.Value;
                    buyerRequest.BusinessModelId = model.TransactionModelId.Value;
                    await _requestService.UpdateRequestAsync(buyerRequest);

                    var cogsInventoryTaggings = (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: buyerRequest.Id));
                    if (cogsInventoryTaggings.Any())
                    {
                        foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                            await _inventoryService.DeleteDirectCogsInventoryTaggingAsync(cogsInventoryTagging);
                    }
                }

                if (model.PinLocation is not null)
                {
                    directOrder.PinLocation_Latitude = model.PinLocation.Latitude;
                    directOrder.PinLocation_Longitude = model.PinLocation.Longitude;
                    directOrder.PinLocation_Location = model.PinLocation.Location;
                }

                if (model.InterGeography.HasValue)
                    directOrder.InterGeography = model.InterGeography.Value;

                await _orderService.UpdateDirectOrderAsync(directOrder);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("DirectOrder.infos.Update") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        //[HttpGet("sale-order-businessmodel-form-load/{requestId}")]
        //public async Task<IActionResult> OrderManagement_GetSaleOrderBusinessModelFormLoad(int requestId)
        //{
        //    if (requestId <= 0)
        //        return Ok(new { success = false, message = "Buyer request id is required" });

        //    var user = await _workContext.GetCurrentCustomerAsync();
        //    if (!await _customerService.IsRegisteredAsync(user))
        //        return Ok(new { success = false, message = "user not found" });

        //    var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
        //    if (buyerRequest is null)
        //        return Ok(new { success = false, message = "Buyer request not found" });

        //    try
        //    {
        //        var data = new
        //        {
        //            requestId = buyerRequest.Id,
        //            transactionModel = await _localizationService.GetLocalizedEnumAsync(buyerRequest.BusinessModelEnum),
        //            quantity = buyerRequest.Quantity
        //        };
        //        return Ok(new { success = true, message = "", data });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        [HttpGet("sale-order-business-model-form-Json")]
        public async Task<IActionResult> OrderManagement_SaleOrderBusinessModelFormJson(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var directOrder = await _orderService.GetDirectOrderByRequestId(requestId);
            if (directOrder is null)
                return Ok(new { success = false, message = "Direct order not found" });

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                return Ok(new { success = false, message = "Request not found" });

            var directOrderCalculations = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);

            var businessModels = new List<BusinessModelApiModel>();
            try
            {
                foreach (var orderCalculation in directOrderCalculations)
                {
                    var model = new BusinessModelApiModel();

                    var directOrderSupplierInfo = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                    if (directOrderSupplierInfo is not null)
                        businessModels.Add(await SaleOrder_BusinessModelFormCalculatedJson(directOrder));
                }

                return Ok(new { success = true, message = "", data = new { bunsinessModelFields = businessModels, businessModelName = await _localizationService.GetLocalizedEnumAsync(directOrder.TransactionModelEnum), businessModelId = directOrder.TransactionModelId } });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("sale-order-business-model-form-calculation")]
        public async Task<IActionResult> OrderManagement_SaleOrderBusinessModelFormCalculation(BusinessModelApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var directOrderCalculation = await _orderService.GetDirectOrderCalculationByIdAsync(model.OrderCalculationId);
            if (directOrderCalculation is null)
                return Ok(new { success = false, message = "Order calculation not found" });

            var directOrder = await _orderService.GetDirectOrderByRequestId(model.RequestId);
            if (directOrder is null)
                return Ok(new { success = false, message = "Direct order not found" });

            var request = await _requestService.GetRequestByIdAsync(model.RequestId);
            if (request is null)
                return Ok(new { success = false, message = "Request not found" });

            var taggedInventories = (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id));
            if (directOrder.TransactionModelId != (int)BusinessModelEnum.ForwardSelling)
            {
                if (taggedInventories.Sum(x => x.Quantity) > request.Quantity)
                    return Ok(new { success = false, message = "tagged inventory quantity is greater than request quantity" });

                if (taggedInventories.Sum(x => x.Quantity) != request.Quantity)
                    return Ok(new { success = false, message = "Sum of inventory should be equal to request quantity" });
            }

            var gstRateValue = 0m;
            var gstRate = model.Receivables.Where(x => x.Name == "GSTAmount").FirstOrDefault().Value;
            if (gstRate is not null)
                gstRateValue = ConvertToDecimal(gstRate);

            var brokerWhtRateValue = 0m;
            var brokerWholesaleTaxRateValue = 0m;

            if (request.BusinessModelId == (int)BusinessModelEnum.Broker)
            {
                var brokerWhtRate = model.Receivables.Where(x => x.Name == "WHTRate").FirstOrDefault().Value;
                if (brokerWhtRate is not null)
                    brokerWhtRateValue = ConvertToDecimal(brokerWhtRate);

                var wholesaleTaxRate = model.Receivables.Where(x => x.Name == "WholesaleTaxRate").FirstOrDefault().Value;
                if (wholesaleTaxRate is not null)
                    brokerWholesaleTaxRateValue = ConvertToDecimal(wholesaleTaxRate);

                var brokerInventory = await _inventoryService.GetInventoryInboundByIdAsync(taggedInventories.Select(x => x.InventoryId).FirstOrDefault());
                if (brokerInventory is not null)
                {
                    gstRateValue = brokerInventory.GstRate;
                    brokerWhtRateValue = brokerInventory.WhtRate;
                    brokerWholesaleTaxRateValue = brokerInventory.WholeSaleTaxRate;
                    directOrderCalculation.InvoicedAmount = directOrderCalculation.BuyingPrice;
                }
            }

            var salePriceValue = 0m;
            var salePrice = model.Receivables.Where(x => x.Name == "SellingPriceOfProduct").FirstOrDefault().Value;
            if (salePrice is not null)
                salePriceValue = ConvertToDecimal(salePrice);

            var buyingPrice = await CalculateBuyingPriceByTaggings(taggedInventories.ToList(), gstRateValue > 0);
            var sellingPriceOfProduct = await CalculateSellingPriceOfProductByTaggings(taggedInventories.ToList(), gstRateValue > 0, salePriceValue);

            var calculatedBuyingPrice = buyingPrice;
            var calculatedSellingPriceOfProduct = sellingPriceOfProduct;

            var margin = 0m;
            if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                margin = calculatedSellingPriceOfProduct - calculatedBuyingPrice;

            var discount = 0m;
            if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                discount = calculatedSellingPriceOfProduct - calculatedBuyingPrice;

            model.SalePrice = salePriceValue;
            try
            {
                #region Standard

                if (directOrder.TransactionModelId == (int)BusinessModelEnum.Standard)
                {
                    var totalPayble = model.SalePrice * request.Quantity;
                    var totalReceivable = model.SalePrice * request.Quantity;

                    var includeGstInTotalAmount = model.Payables.Where(x => x.Name == "GSTIncludedInTotalAmount")?.FirstOrDefault()?.Value;

                    #region Receivable

                    foreach (var item in model.Receivables)
                    {
                        if (item.Name == "BuyingPrice")
                            directOrderCalculation.BuyingPrice = buyingPrice;

                        if (item.Name == "SellingPriceOfProduct")
                            directOrderCalculation.SellingPriceOfProduct = model.SalePrice;

                        //if (item.Name == "MarginType")
                        //    directOrderCalculation.MarginRateType = (string)item.Value;

                        //Margin Recievable
                        else if (item.Name == "MarginRate")
                        {
                            //var marginType = model.Receivables.Where(x => x.Name == "MarginType")?.FirstOrDefault()?.Value;

                            //directOrderCalculation.MarginRate = ConvertToDecimal(item.Value);
                            //directOrderCalculation.Receivable_Margintype_0 = (string)marginType;

                            if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                            {
                                //Old Logic
                                //directOrderCalculation.MarginAmount = margin;
                                //directOrderCalculation.MarginRate = (margin / 100m);
                                //directOrderCalculation.MarginRateType = "PKR";

                                //new logic implemented as per meraj alvi on 07-Aug-2023
                                directOrderCalculation.MarginAmount = margin * request.Quantity;
                                directOrderCalculation.MarginRateType = "PKR";
                                directOrderCalculation.MarginRate = margin;

                                directOrderCalculation.NetRateWithMargin = totalReceivable /*+ directOrderCalculation.MarginAmount*/;

                                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                {
                                    directOrderCalculation.MarginRate = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.MarginRate);
                                    directOrderCalculation.NetRateWithMargin = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.NetRateWithMargin);
                                }
                            }
                            else
                            {
                                directOrderCalculation.MarginAmount = 0;
                                directOrderCalculation.MarginRate = 0;
                                directOrderCalculation.NetRateWithMargin = totalReceivable;
                            }
                        }

                        //Discount Recievable
                        else if (item.Name == "DiscountRate")
                        {
                            if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                            {
                                //Old Logic
                                //directOrderCalculation.DiscountAmount = discount;
                                //directOrderCalculation.DiscountRate = (discount / 100m);
                                //directOrderCalculation.DiscountRateType = "PKR";

                                //new logic implemented as per meraj alvi on 07-Aug-2023
                                directOrderCalculation.DiscountAmount = discount * request.Quantity;
                                directOrderCalculation.DiscountRateType = "PKR";
                                directOrderCalculation.DiscountRate = discount;

                                directOrderCalculation.NetRateWithMargin -= Math.Abs(directOrderCalculation.DiscountAmount);

                                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                {
                                    directOrderCalculation.DiscountAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.DiscountAmount);
                                    directOrderCalculation.NetRateWithMargin = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.NetRateWithMargin);
                                }
                            }
                            else
                            {
                                directOrderCalculation.DiscountRate = 0;
                                //directOrderCalculation.Receivable_Promotype_0 = "";
                                directOrderCalculation.DiscountAmount = 0;
                            }
                        }

                        //GST Recievable
                        else if (item.Name == "GSTAmount")
                        {
                            if (ConvertToDecimal(item.Value) > 0)
                            {
                                directOrderCalculation.GSTRate = ConvertToDecimal(item.Value);
                                directOrderCalculation.GSTAmount = directOrderCalculation.NetRateWithMargin * (directOrderCalculation.GSTRate / 100m);

                                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                    directOrderCalculation.GSTAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.GSTAmount);
                            }
                            else
                            {
                                directOrderCalculation.GSTRate = 0;
                                directOrderCalculation.GSTAmount = 0;
                            }
                        }

                        //WHT Recievable
                        else if (item.Name == "WHTAmount")
                        {
                            if (ConvertToDecimal(item.Value) > 0)
                            {
                                directOrderCalculation.WHTRate = ConvertToDecimal(item.Value);
                                directOrderCalculation.WHTAmount = (directOrderCalculation.NetRateWithMargin + directOrderCalculation.GSTAmount) * (ConvertToDecimal(item.Value) / 100m);

                                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                    directOrderCalculation.WHTAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.WHTAmount);
                            }
                            else
                            {
                                directOrderCalculation.WHTRate = 0;
                                directOrderCalculation.WHTAmount = 0;
                            }

                            //if (directOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Standard)
                            //{
                            //    totalReceivable += directOrderCalculation.MarginAmount;
                            //    totalReceivable -= directOrderCalculation.DiscountAmount;
                            //}

                            totalReceivable += directOrderCalculation.GSTAmount;
                            totalReceivable -= directOrderCalculation.WHTAmount;
                            directOrderCalculation.SubTotal = totalReceivable;
                        }
                    }

                    #endregion
                }

                #endregion

                #region All other business module

                else if (directOrder.TransactionModelId == (int)BusinessModelEnum.OneOnOne || directOrder.TransactionModelId == (int)BusinessModelEnum.Agency || directOrder.TransactionModelId == (int)BusinessModelEnum.FineCounts || directOrder.TransactionModelId == (int)BusinessModelEnum.ForwardBuying || directOrder.TransactionModelId == (int)BusinessModelEnum.ForwardSelling)
                {
                    #region Receivable
                    var totalReceivable = model.SalePrice * request.Quantity;
                    var gstAmount_Receivable = 0m;
                    var whtAmount_Receivable = 0m;
                    var totalReceivableFromBuyer = 0m;
                    var totalReceivableFromBuyerAfterMultiply = 0m;

                    foreach (var item in model.Receivables)
                    {
                        if (item.Name is not null)
                        {
                            var receivable_SellingPriceOfProduct = ConvertToDecimal(model.Receivables.Where(x => x.Name == "SellingPriceOfProduct")?.FirstOrDefault().Value);

                            if (item.Name == "SellingPriceOfProduct")
                            {
                                directOrderCalculation.SellingPriceOfProduct = receivable_SellingPriceOfProduct;
                            }

                            else if (item.Name == "BuyingPrice")
                            {
                                directOrderCalculation.BuyingPrice = calculatedBuyingPrice;
                                //var cogsInventoryTaggings = (await _inventoryService.GetAllCogsInventoryTaggingsAsync(requestId: request.Id));
                                //cogsInventoryTaggings = (await _inventoryService.GetAllCogsInventoryTaggingsAsync(requestId: request.Id));
                                //if (cogsInventoryTaggings.Any())
                                //{
                                //    var cogsCalculationValue = 0m;
                                //    foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                                //    {
                                //        var inventoryRateQuantity = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
                                //        cogsCalculationValue += inventoryRateQuantity;
                                //    }
                                //    directOrderCalculation.BuyingPrice = cogsCalculationValue / cogsInventoryTaggings.Sum(x => x.Quantity);
                                //}
                            }
                            else if (item.Name == "InvoicedAmount")
                                directOrderCalculation.InvoicedAmount = ConvertToDecimal(item.Value);

                            //GST Rate
                            else if (item.Name == "GSTAmount")
                            {
                                var gstRatePercent = ConvertToDecimal(model.Receivables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);
                                if (((decimal)gstRatePercent) > 0)
                                {
                                    var gstRateCalculationValue = directOrderCalculation.InvoicedAmount * (decimal)gstRatePercent / 100;
                                    gstAmount_Receivable = Math.Round(gstRateCalculationValue, 2);

                                    directOrderCalculation.GSTIncludedInTotalAmount = true;
                                    directOrderCalculation.GSTRate = ConvertToDecimal(gstRatePercent);
                                    directOrderCalculation.GSTAmount = gstAmount_Receivable;

                                }
                                else
                                {
                                    gstAmount_Receivable = 0m;
                                    directOrderCalculation.GSTIncludedInTotalAmount = false;
                                    directOrderCalculation.GSTRate = 0m;
                                    directOrderCalculation.GSTAmount = 0m;
                                }
                            }

                            else if (item.Name == "BrokerCash")
                                directOrderCalculation.BrokerCash = directOrderCalculation.SellingPriceOfProduct - directOrderCalculation.InvoicedAmount;

                            else if (item.Name == "BrokerId")
                                directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                            //WHT Rate
                            else if (item.Name == "WHTAmount")
                            {
                                var whtRate = ConvertToDecimal(model.Receivables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                                if (((decimal)whtRate) > 0)
                                {
                                    var whtRateClaculationValue = ((directOrderCalculation.InvoicedAmount + gstAmount_Receivable) * (decimal)whtRate / 100);
                                    whtAmount_Receivable = Math.Round(whtRateClaculationValue, 2);

                                    directOrderCalculation.WhtIncluded = true;
                                    directOrderCalculation.WHTRate = ConvertToDecimal(whtRate);
                                    directOrderCalculation.WHTAmount = whtAmount_Receivable;
                                }
                                else
                                {
                                    whtAmount_Receivable = 0m;
                                    directOrderCalculation.WhtIncluded = false;
                                    directOrderCalculation.WHTRate = 0m;
                                    directOrderCalculation.WHTAmount = 0m;
                                }
                            }

                            //Buyer comission receivable per bag
                            else if (item.Name == "BuyerCommissionReceivablePerBag")
                            {
                                directOrderCalculation.BuyerCommissionReceivablePerBag = ConvertToDecimal(item.Value);
                                directOrderCalculation.BuyerCommissionReceivable_Summary = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                            }

                            //Buyer comission payable per bag
                            else if (item.Name == "BuyerCommissionPayablePerBag")
                            {
                                directOrderCalculation.BuyerCommissionPayablePerBag = ConvertToDecimal(item.Value);
                                directOrderCalculation.BuyerCommissionPayable_Summary = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                            }

                            else if (item.Name == "BuyerCommissionPayableUserId")
                                directOrderCalculation.BuyerCommissionPayableUserId = Convert.ToInt32(item.Value);

                            else if (item.Name == "BuyerCommissionReceivableUserId")
                                directOrderCalculation.BuyerCommissionReceivableUserId = Convert.ToInt32(item.Value);

                            else if (item.Name == "TotalReceivableBuyer")
                            {
                                totalReceivableFromBuyer = ((decimal)((receivable_SellingPriceOfProduct + gstAmount_Receivable) - whtAmount_Receivable));
                                directOrderCalculation.TotalPerBag = ((decimal)(totalReceivableFromBuyer + directOrderCalculation.BuyerCommissionReceivablePerBag - directOrderCalculation.BuyerCommissionPayablePerBag));
                                totalReceivableFromBuyerAfterMultiply = totalReceivableFromBuyer * request.Quantity;
                                directOrderCalculation.TotalReceivableBuyer = totalReceivableFromBuyerAfterMultiply;
                            }

                            else if (item.Name == "MarginAmount")
                            {
                                if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                                {
                                    //Old Logic
                                    //directOrderCalculation.MarginRateType = "PKR";
                                    //directOrderCalculation.MarginAmount = margin;
                                    //directOrderCalculation.MarginRate = margin / 100m;

                                    //new logic implemented as per meraj alvi on 07-Aug-2023
                                    directOrderCalculation.MarginRateType = "PKR";
                                    directOrderCalculation.MarginAmount = margin * request.Quantity;
                                    directOrderCalculation.MarginRate = margin;
                                }
                                else
                                {
                                    directOrderCalculation.MarginRateType = "";
                                    directOrderCalculation.MarginAmount = 0m;
                                    directOrderCalculation.MarginRate = 0m;
                                }
                            }

                            else if (item.Name == "DiscountAmount")
                            {
                                if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                                {
                                    //Old Logic
                                    //directOrderCalculation.DiscountRateType = "PKR";
                                    //directOrderCalculation.DiscountRate = discount / 100m;
                                    //directOrderCalculation.DiscountAmount = discount;


                                    //new logic implemented as per meraj alvi on 07-Aug-2023
                                    directOrderCalculation.DiscountRateType = "PKR";
                                    directOrderCalculation.DiscountAmount = discount * request.Quantity;
                                    directOrderCalculation.DiscountRate = discount;
                                }
                                else
                                {
                                    directOrderCalculation.DiscountRateType = "";
                                    directOrderCalculation.DiscountRate = 0m;
                                    directOrderCalculation.DiscountAmount = 0m;
                                }
                            }

                            else if (item.Name == "OrderTotal")
                            {
                                var buyerCommissionAfterMultiply = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                                var buyerCommission_ReceivableAfterMultiply = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                                //directOrderCalculation.SubTotal = ((decimal)(totalReceivableFromBuyerAfterMultiply + buyerCommission_ReceivableAfterMultiply - buyerCommissionAfterMultiply));
                                directOrderCalculation.SubTotal = totalReceivableFromBuyerAfterMultiply;

                                //totalReceivable += directOrderCalculation.MarginAmount;
                                //totalReceivable -= directOrderCalculation.DiscountAmount;
                                //totalReceivable += directOrderCalculation.GSTAmount;
                                //totalReceivable -= directOrderCalculation.WHTAmount;
                                //directOrderCalculation.SubTotal = totalReceivable;
                                directOrderCalculation.OrderTotal = directOrderCalculation.SubTotal;
                            }
                        }
                    }

                    #endregion
                }

                #endregion

                #region Lending

                else if (directOrder.TransactionModelId == (int)BusinessModelEnum.Lending)
                {
                    #region Receivable

                    var totalReceivable = model.SalePrice * request.Quantity;
                    var gstAmount_Receivable = 0m;
                    var whtAmount_Receivable = 0m;
                    //var margin = 0m;
                    //var discount = 0m;
                    var totalReceivableFromBuyerAfterMultiply = 0m;
                    var sellingPrice_FinanceIncome = ConvertToDecimal(model.Receivables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value) + ConvertToDecimal(model.Receivables.Where(x => x.Name == "FinanceIncome")?.FirstOrDefault().Value);

                    foreach (var item in model.Receivables)
                    {
                        if (item.Name is not null)
                        {
                            var receivable_SellingPriceOfProduct = ConvertToDecimal(model.Receivables.Where(x => x.Name == "SellingPriceOfProduct")?.FirstOrDefault().Value);

                            if (item.Name == "SellingPriceOfProduct")
                                directOrderCalculation.SellingPriceOfProduct = ConvertToDecimal(item.Value);

                            else if (item.Name == "BuyingPrice")
                            {
                                directOrderCalculation.BuyingPrice = calculatedBuyingPrice;
                            }
                            //GST
                            //else if (item.Name == "GSTAmount")
                            //{
                            //    var gstInclusive = model.Receivables.Where(x => x.Name == "GSTIncludedInTotalAmount")?.FirstOrDefault()?.Value;
                            //    if (((string)gstInclusive).ToLower() == "yes")
                            //    {
                            //        var gstRateCalculationValue = sellingPrice_FinanceIncome * _calculationSettings.GSTPercentageForQuotation;
                            //        gstAmount_Receivable = Math.Round(gstRateCalculationValue, 2);

                            //        directOrderCalculation.GSTIncludedInTotalAmount = true;
                            //        directOrderCalculation.GSTRate = _calculationSettings.GSTPercentageForQuotation * 100;
                            //        directOrderCalculation.GSTAmount = gstAmount_Receivable;
                            //    }
                            //    else
                            //    {
                            //        gstAmount_Receivable = 0m;
                            //        directOrderCalculation.GSTIncludedInTotalAmount = false;
                            //        directOrderCalculation.GSTRate = 0m;
                            //        directOrderCalculation.GSTAmount = 0m;
                            //    }
                            //}

                            //GST Rate
                            else if (item.Name == "GSTAmount")
                            {
                                var gstRatePercent = ConvertToDecimal(model.Receivables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);
                                if (((decimal)gstRatePercent) > 0)
                                {
                                    var invoiceAmount = ConvertToDecimal(model.Receivables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value);
                                    var financeCostPayment = Convert.ToInt32(model.Receivables.Where(x => x.Name == "FinanceCostPayment")?.FirstOrDefault().Value);
                                    var gstRateCalculationValue = invoiceAmount * (decimal)gstRatePercent / 100m;

                                    if (financeCostPayment == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                        gstRateCalculationValue = ((decimal)((decimal)sellingPrice_FinanceIncome * (decimal)gstRatePercent / 100m));

                                    gstAmount_Receivable = Math.Round(gstRateCalculationValue, 2);

                                    directOrderCalculation.GSTIncludedInTotalAmount = true;
                                    directOrderCalculation.GSTRate = ConvertToDecimal(gstRatePercent);
                                    directOrderCalculation.GSTAmount = gstAmount_Receivable;

                                }
                                else
                                {
                                    gstAmount_Receivable = 0m;
                                    directOrderCalculation.GSTIncludedInTotalAmount = false;
                                    directOrderCalculation.GSTRate = 0m;
                                    directOrderCalculation.GSTAmount = 0m;
                                }
                            }

                            //WHT
                            //else if (item.Name == "WHTAmount")
                            //{
                            //    var whtInclusive = model.Receivables.Where(x => x.Name == "WhtIncluded")?.FirstOrDefault()?.Value;
                            //    if (((string)whtInclusive).ToLower() == "yes")
                            //    {
                            //        var whtRateClaculationValue = ((sellingPrice_FinanceIncome + gstAmount_Receivable) * _calculationSettings.WHTPercentageForQuotation);
                            //        whtAmount_Receivable = Math.Round(whtRateClaculationValue, 2);

                            //        directOrderCalculation.WhtIncluded = true;
                            //        directOrderCalculation.WHTRate = _calculationSettings.WHTPercentageForQuotation * 100;
                            //        directOrderCalculation.WHTAmount = whtAmount_Receivable;
                            //    }
                            //    else
                            //    {
                            //        whtAmount_Receivable = 0m;
                            //        directOrderCalculation.WhtIncluded = false;
                            //        directOrderCalculation.WHTRate = 0m;
                            //        directOrderCalculation.WHTAmount = 0m;
                            //    }
                            //}

                            //WHT Rate
                            else if (item.Name == "WHTAmount")
                            {
                                var whtRate = ConvertToDecimal(model.Receivables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                                if (((decimal)whtRate) > 0)
                                {
                                    var whtRateClaculationValue = ((sellingPrice_FinanceIncome + gstAmount_Receivable) * (decimal)whtRate / 100);
                                    whtAmount_Receivable = Math.Round(whtRateClaculationValue, 2);

                                    directOrderCalculation.WhtIncluded = true;
                                    directOrderCalculation.WHTRate = ConvertToDecimal(whtRate);
                                    directOrderCalculation.WHTAmount = whtAmount_Receivable;
                                }
                                else
                                {
                                    whtAmount_Receivable = 0m;
                                    directOrderCalculation.WhtIncluded = false;
                                    directOrderCalculation.WHTRate = 0m;
                                    directOrderCalculation.WHTAmount = 0m;
                                }
                            }

                            //Buyer comission receivable per bag
                            else if (item.Name == "BuyerCommissionReceivablePerBag")
                            {
                                directOrderCalculation.BuyerCommissionReceivablePerBag = ConvertToDecimal(item.Value);
                                directOrderCalculation.BuyerCommissionReceivable_Summary = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                            }

                            //Buyer comission payable per bag
                            else if (item.Name == "BuyerCommissionPayablePerBag")
                            {
                                directOrderCalculation.BuyerCommissionPayablePerBag = ConvertToDecimal(item.Value);
                                directOrderCalculation.BuyerCommissionPayable_Summary = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                            }

                            else if (item.Name == "BuyerCommissionPayableUserId")
                                directOrderCalculation.BuyerCommissionPayableUserId = Convert.ToInt32(item.Value);

                            else if (item.Name == "BuyerCommissionReceivableUserId")
                                directOrderCalculation.BuyerCommissionReceivableUserId = Convert.ToInt32(item.Value);

                            else if (item.Name == "InvoicedAmount")
                                directOrderCalculation.InvoicedAmount = ConvertToDecimal(item.Value);

                            else if (item.Name == "BrokerCash")
                                directOrderCalculation.BrokerCash = directOrderCalculation.SellingPriceOfProduct - directOrderCalculation.InvoicedAmount;


                            else if (item.Name == "BrokerId")
                                directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                            else if (item.Name == "BuyerPaymentTerms")
                                directOrderCalculation.BuyerPaymentTerms = Convert.ToInt32(item.Value);

                            else if (item.Name == "TotalFinanceCost")
                                directOrderCalculation.TotalFinanceCost = ConvertToDecimal(item.Value);

                            else if (item.Name == "SupplierCreditTerms")
                                directOrderCalculation.SupplierCreditTerms = ConvertToDecimal(item.Value);

                            else if (item.Name == "FinanceCostPayment")
                                directOrderCalculation.FinanceCostPayment = Convert.ToInt32(item.Value);

                            else if (item.Name == "FinanceCost")
                                directOrderCalculation.FinanceCost = directOrderCalculation.SupplierCreditTerms > 0 && directOrderCalculation.TotalFinanceCost > 0 ? (directOrderCalculation.TotalFinanceCost / directOrderCalculation.SupplierCreditTerms) * directOrderCalculation.BuyerPaymentTerms : 0;

                            else if (item.Name == "FinanceIncome")
                                directOrderCalculation.FinanceIncome = ConvertToDecimal(item.Value);

                            else if (item.Name == "SellingPrice_FinanceIncome")
                                directOrderCalculation.SellingPrice_FinanceIncome = directOrderCalculation.SellingPriceOfProduct + directOrderCalculation.FinanceIncome;

                            else if (item.Name == "MarginAmount")
                            {
                                if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                                {
                                    //Old Logic
                                    //directOrderCalculation.MarginRateType = "PKR";
                                    //directOrderCalculation.MarginAmount = margin;
                                    //directOrderCalculation.MarginRate = margin / 100m;

                                    //new logic implemented as per meraj alvi on 07-Aug-2023
                                    directOrderCalculation.MarginRateType = "PKR";
                                    directOrderCalculation.MarginAmount = margin * request.Quantity;
                                    directOrderCalculation.MarginRate = margin;
                                }
                                else
                                {
                                    directOrderCalculation.MarginRateType = "";
                                    directOrderCalculation.MarginAmount = 0m;
                                    directOrderCalculation.MarginRate = 0m;
                                }
                            }
                            else if (item.Name == "DiscountAmount")
                            {
                                if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                                {
                                    //Old Logic
                                    //directOrderCalculation.DiscountRateType = "PKR";
                                    //directOrderCalculation.DiscountRate = discount / 100m;
                                    //directOrderCalculation.DiscountAmount = discount;

                                    //new logic implemented as per meraj alvi on 07-Aug-2023
                                    directOrderCalculation.DiscountRateType = "PKR";
                                    directOrderCalculation.DiscountAmount = discount * request.Quantity;
                                    directOrderCalculation.DiscountRate = discount;
                                }
                                else
                                {
                                    directOrderCalculation.DiscountRateType = "";
                                    directOrderCalculation.DiscountRate = 0m;
                                    directOrderCalculation.DiscountAmount = 0m;
                                }
                            }

                            else if (item.Name == "TotalReceivableBuyer")
                            {
                                directOrderCalculation.TotalReceivableBuyer = ((decimal)((directOrderCalculation.SellingPrice_FinanceIncome + gstAmount_Receivable) - whtAmount_Receivable));
                                totalReceivableFromBuyerAfterMultiply = directOrderCalculation.TotalReceivableBuyer * request.Quantity;
                                directOrderCalculation.TotalReceivableBuyer = totalReceivableFromBuyerAfterMultiply;
                            }
                            else if (item.Name == "OrderTotal")
                            {
                                var buyerCommission_ReceivableAfterMultiply = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                                var buyerCommissionAfterMultiply = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                                //directOrderCalculation.SubTotal = ((decimal)(directOrderCalculation.TotalReceivableBuyer + buyerCommission_ReceivableAfterMultiply - buyerCommissionAfterMultiply));
                                directOrderCalculation.SubTotal = directOrderCalculation.TotalReceivableBuyer;
                                //totalReceivable += directOrderCalculation.MarginAmount;
                                //totalReceivable -= directOrderCalculation.DiscountAmount;
                                //totalReceivable += directOrderCalculation.GSTAmount;
                                //totalReceivable -= directOrderCalculation.WHTAmount;
                                directOrderCalculation.OrderTotal = directOrderCalculation.SubTotal;
                            }
                        }
                    }

                    #endregion
                }

                #endregion

                #region Broker

                if (directOrder.TransactionModelId == (int)BusinessModelEnum.Broker)
                {
                    var totalReceivable = model.SalePrice * request.Quantity;
                    #region Receivable

                    var gstAmount_Receivable = 0m;
                    var whtAmount_Receivable = 0m;
                    var wholeSaleTaxAmount_Receivable = 0m;
                    //var calculatedSellingPriceOfProduct_Receivable = 0m;

                    foreach (var item in model.Receivables)
                    {
                        if (item.Name is not null)
                        {
                            var receivable_SellingPriceOfProduct = ConvertToDecimal(model.Receivables.Where(x => x.Name == "SellingPriceOfProduct")?.FirstOrDefault().Value);
                            //calculatedSellingPriceOfProduct_Receivable = receivable_SellingPriceOfProduct;
                            if (item.Name == "SellingPriceOfProduct")
                            {
                                directOrderCalculation.SellingPriceOfProduct = receivable_SellingPriceOfProduct;
                            }
                            else if (item.Name == "BuyingPrice")
                            {
                                directOrderCalculation.BuyingPrice = calculatedBuyingPrice;
                                //var cogsInventoryTaggings = (await _inventoryService.GetAllCogsInventoryTaggingsAsync(requestId: request.Id));
                                //cogsInventoryTaggings = (await _inventoryService.GetAllCogsInventoryTaggingsAsync(requestId: request.Id));
                                //if (cogsInventoryTaggings.Any())
                                //{
                                //    var cogsCalculationValue = 0m;
                                //    foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                                //    {
                                //        var inventoryRateQuantity = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
                                //        cogsCalculationValue += inventoryRateQuantity;
                                //    }
                                //    directOrderCalculation.BuyingPrice = cogsCalculationValue / cogsInventoryTaggings.Sum(x => x.Quantity);
                                //}
                            }
                            else if (item.Name == "InvoicedAmount")
                                directOrderCalculation.InvoicedAmount = ConvertToDecimal(item.Value);

                            else if (item.Name == "GSTRate")
                                directOrderCalculation.GSTRate = gstRateValue;
                            //GST
                            //else if (item.Name == "GSTAmount")
                            //{
                            //    var gstInclusive = model.Receivables.Where(x => x.Name == "GSTIncludedInTotalAmount").FirstOrDefault().Value;

                            //    if (((string)gstInclusive).ToLower() == "yes")
                            //    {
                            //        var gstRateCalculationValue = ((decimal)((float)receivable_SellingPriceOfProduct * (float)_calculationSettings.GSTPercentageForQuotation));
                            //        gstAmount_Receivable = gstRateCalculationValue;

                            //        directOrderCalculation.GSTIncludedInTotalAmount = true;
                            //        directOrderCalculation.GSTRate = _calculationSettings.GSTPercentageForQuotation * 100;
                            //        directOrderCalculation.GSTAmount = gstAmount_Receivable;
                            //    }
                            //    else
                            //    {
                            //        gstAmount_Receivable = 0m;
                            //        directOrderCalculation.GSTIncludedInTotalAmount = false;
                            //        directOrderCalculation.GSTRate = 0m;
                            //        directOrderCalculation.GSTAmount = 0m;
                            //    }
                            //}

                            //GST Rate
                            else if (item.Name == "GSTAmount")
                            {
                                var gstRatePercent = ConvertToDecimal(model.Receivables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);
                                if (((decimal)gstRatePercent) > 0)
                                {
                                    var gstRateCalculationValue = directOrderCalculation.InvoicedAmount * (decimal)gstRatePercent / 100;
                                    gstAmount_Receivable = Math.Round(gstRateCalculationValue, 2);

                                    directOrderCalculation.GSTIncludedInTotalAmount = true;
                                    directOrderCalculation.GSTRate = ConvertToDecimal(gstRatePercent);
                                    directOrderCalculation.GSTAmount = gstAmount_Receivable;

                                }
                                else
                                {
                                    gstAmount_Receivable = 0m;
                                    directOrderCalculation.GSTIncludedInTotalAmount = false;
                                    directOrderCalculation.GSTRate = 0m;
                                    directOrderCalculation.GSTAmount = 0m;
                                }
                            }

                            //WHT
                            //else if (item.Name == "WHTAmount")
                            //{
                            //    var gstInclusive = model.Receivables.Where(x => x.Name == "WhtIncluded")?.FirstOrDefault()?.Value;
                            //    if (((string)gstInclusive).ToLower() == "yes")
                            //    {
                            //        var whtRateClaculationValue = ((decimal)(((float)receivable_SellingPriceOfProduct + (float)gstAmount_Receivable) * (float)_calculationSettings.WHTPercentageForQuotation));
                            //        whtAmount_Receivable = whtRateClaculationValue;

                            //        directOrderCalculation.WhtIncluded = true;
                            //        directOrderCalculation.WHTRate = _calculationSettings.WHTPercentageForQuotation * 100;
                            //        directOrderCalculation.WHTAmount = whtAmount_Receivable;
                            //    }
                            //    else
                            //    {
                            //        whtAmount_Receivable = 0m;
                            //        directOrderCalculation.WhtIncluded = false;
                            //        directOrderCalculation.WHTRate = 0m;
                            //        directOrderCalculation.WHTAmount = 0m;
                            //    }
                            //}


                            //WHT Rate
                            else if (item.Name == "WHTAmount")
                            {
                                //var whtRate = ConvertToDecimal(model.Receivables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                                if (((decimal)brokerWhtRateValue) > 0)
                                {
                                    var whtRateClaculationValue = ((directOrderCalculation.InvoicedAmount + gstAmount_Receivable) * (decimal)brokerWhtRateValue / 100);
                                    whtAmount_Receivable = whtRateClaculationValue;

                                    directOrderCalculation.WhtIncluded = true;
                                    directOrderCalculation.WHTRate = ConvertToDecimal(brokerWhtRateValue);
                                    directOrderCalculation.WHTAmount = whtAmount_Receivable;
                                }
                                else
                                {
                                    whtAmount_Receivable = 0m;
                                    directOrderCalculation.WhtIncluded = false;
                                    directOrderCalculation.WHTRate = 0m;
                                    directOrderCalculation.WHTAmount = 0m;
                                }
                            }

                            //Wholesale Tax
                            else if (item.Name == "WholesaleTaxAmount")
                            {
                                //var wholesaletaxRate = ConvertToDecimal(model.Receivables.Where(x => x.Name == "WholesaleTaxRate")?.FirstOrDefault().Value);
                                if (((decimal)brokerWholesaleTaxRateValue) > 0)
                                {
                                    //var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));
                                    var wholesaletaxCalculationValue = ((decimal)((float)(directOrderCalculation.InvoicedAmount + gstAmount_Receivable - Math.Abs(whtAmount_Receivable)) * (float)brokerWholesaleTaxRateValue / 100));
                                    wholeSaleTaxAmount_Receivable = wholesaletaxCalculationValue;

                                    directOrderCalculation.WholesaletaxIncluded = true;
                                    directOrderCalculation.WholesaleTaxRate = ConvertToDecimal(brokerWholesaleTaxRateValue);
                                    directOrderCalculation.WholesaleTaxAmount = wholeSaleTaxAmount_Receivable;
                                }
                                else
                                {
                                    wholeSaleTaxAmount_Receivable = 0m;
                                    directOrderCalculation.WholesaletaxIncluded = false;
                                    directOrderCalculation.WholesaleTaxRate = 0;
                                    directOrderCalculation.WholesaleTaxAmount = 0m;
                                }
                            }

                            //Buyer commission receivable per bag
                            else if (item.Name == "BuyerCommissionReceivablePerBag")
                                directOrderCalculation.BuyerCommissionReceivablePerBag = ConvertToDecimal(item.Value);

                            else if (item.Name == "BuyerCommissionPayablePerBag")
                                directOrderCalculation.BuyerCommissionPayablePerBag = ConvertToDecimal(item.Value);

                            else if (item.Name == "BrokerCash")
                                directOrderCalculation.BrokerCash = directOrderCalculation.SellingPriceOfProduct - directOrderCalculation.InvoicedAmount;

                            else if (item.Name == "BrokerId")
                                directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                            else if (item.Name == "BuyerCommissionPayableUserId")
                                directOrderCalculation.BuyerCommissionPayableUserId = Convert.ToInt32(item.Value);

                            else if (item.Name == "BuyerCommissionReceivableUserId")
                                directOrderCalculation.BuyerCommissionReceivableUserId = Convert.ToInt32(item.Value);

                            //Margin
                            else if (item.Name == "MarginAmount")
                            {
                                if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                                {
                                    //old Logic
                                    //directOrderCalculation.MarginRateType = "PKR";
                                    //directOrderCalculation.MarginAmount = margin;
                                    //directOrderCalculation.MarginRate = margin / 100m;

                                    //new logic implemented as per meraj alvi on 07-Aug-2023
                                    directOrderCalculation.MarginRateType = "PKR";
                                    directOrderCalculation.MarginAmount = margin * request.Quantity;
                                    directOrderCalculation.MarginRate = margin;
                                }
                                else
                                {
                                    directOrderCalculation.MarginRateType = "";
                                    directOrderCalculation.MarginAmount = 0m;
                                    directOrderCalculation.MarginRate = 0m;
                                }
                                //if (directOrderCalculation.SellingPriceOfProduct > directOrderCalculation.BuyingPrice)
                                //    margin = directOrderCalculation.SellingPriceOfProduct - directOrderCalculation.BuyingPrice;

                                //directOrderCalculation.MarginAmount = margin;
                                //directOrderCalculation.MarginRateType = "PKR";
                            }

                            //Promo
                            else if (item.Name == "DiscountAmount")
                            {
                                if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                                {
                                    //Old Logic
                                    //directOrderCalculation.DiscountRateType = "PKR";
                                    //directOrderCalculation.DiscountRate = discount / 100m;
                                    //directOrderCalculation.DiscountAmount = discount;

                                    //new logic implemented as per meraj alvi on 07-Aug-2023
                                    directOrderCalculation.DiscountRateType = "PKR";
                                    directOrderCalculation.DiscountAmount = discount * request.Quantity;
                                    directOrderCalculation.DiscountRate = discount;
                                }
                                else
                                {
                                    directOrderCalculation.DiscountRateType = "";
                                    directOrderCalculation.DiscountRate = 0m;
                                    directOrderCalculation.DiscountAmount = 0m;
                                }
                                //if (directOrderCalculation.SellingPriceOfProduct < directOrderCalculation.BuyingPrice)
                                //    promo = directOrderCalculation.SellingPriceOfProduct - directOrderCalculation.BuyingPrice;

                                //directOrderCalculation.DiscountAmount = promo;
                                //directOrderCalculation.DiscountRateType = "PKR";
                            }

                            //Total receivable from buyer directly to supplier
                            else if (item.Name == "TotalReceivableFromBuyerDirectlyToSupplier")
                            {
                                var payableToMill = ((decimal)((float)((directOrderCalculation.InvoicedAmount + directOrderCalculation.GSTAmount) - (directOrderCalculation.WHTAmount) + wholeSaleTaxAmount_Receivable)));
                                directOrderCalculation.TotalReceivableFromBuyerDirectlyToSupplier = payableToMill * request.Quantity;
                            }

                            //Total commission receivable from buyer to zaraye
                            else if (item.Name == "TotalCommissionReceivableFromBuyerToZaraye")
                                directOrderCalculation.TotalCommissionReceivableFromBuyerToZaraye = directOrderCalculation.MarginAmount * request.Quantity;

                            //Buyer commission receivable 
                            else if (item.Name == "BuyerCommissionReceivable_Summary")
                                directOrderCalculation.BuyerCommissionReceivable_Summary = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;

                            //Buyer commission payable
                            else if (item.Name == "BuyerCommissionPayable_Summary")
                                directOrderCalculation.BuyerCommissionPayable_Summary = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;

                            //Total receivable
                            else if (item.Name == "OrderTotal")
                            {
                                var payableToMill = ((directOrderCalculation.InvoicedAmount + directOrderCalculation.GSTAmount) - (directOrderCalculation.WHTAmount));
                                directOrderCalculation.PayableToMill = payableToMill;
                                var totalReceivableFromBuyerDirectlyToSupplier = payableToMill;
                                var payableToMillAfterMultiply = directOrderCalculation.PayableToMill * request.Quantity;
                                var totalCommissionReceivableFromBuyerToZaraye_ReceivableAfterMultiply = margin * request.Quantity;
                                var totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply = payableToMillAfterMultiply;
                                directOrderCalculation.TotalPerBag = directOrderCalculation.BuyerCommissionReceivablePerBag - directOrderCalculation.BuyerCommissionPayablePerBag;
                                directOrderCalculation.SubTotal = (totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply + totalCommissionReceivableFromBuyerToZaraye_ReceivableAfterMultiply /*+ directOrderCalculation.BuyerCommissionReceivable_Summary - directOrderCalculation.BuyerCommissionPayable_Summary*/);
                                directOrderCalculation.GrossAmount = directOrderCalculation.SubTotal;
                                directOrderCalculation.OrderTotal = directOrderCalculation.BuyerCommissionReceivable_Summary - directOrderCalculation.BuyerCommissionPayable_Summary;

                            }

                        }
                    }

                    #endregion
                }

                #endregion

                await _orderService.UpdateDirectOrderCalculationAsync(directOrderCalculation);

                return Ok(new { success = true, message = "", data = await SaleOrder_BusinessModelFormCalculatedJson(directOrder) });
            }

            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("direct-sale-order-placed/{requestId}")]
        public async Task<IActionResult> OrderManagement_DirectSaleOrder(int requestId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var request = await _requestService.GetRequestByIdAsync(requestId);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);
                if (buyer is null)
                    return Ok(new { success = false, message = "Buyer not found" });

                var directOrder = await _orderService.GetDirectOrderByRequestId(request.Id);
                if (directOrder is null)
                    return Ok(new { success = false, message = "Direct order not found" });

                var directOrderCalculation = await _orderService.GetDirectOrderCalculationByDirectOrderIdAsync(directOrder.Id);
                if (directOrderCalculation is null)
                    return Ok(new { success = false, message = "Direct order calculation not found" });

                if (directOrderCalculation.BuyingPrice == 0 && directOrder.TransactionModelId != (int)BusinessModelEnum.ForwardSelling)
                    return Ok(new { success = false, message = "Buying price is required" });

                if (directOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Standard)
                {
                    if (directOrderCalculation.BrokerCash > 0 && directOrderCalculation.BrokerId == 0)
                        return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.BrokerId.Required") });

                    if (directOrderCalculation.BuyerCommissionPayableUserId > 0 && directOrderCalculation.BuyerCommissionPayablePerBag == 0)
                        return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.BuyerCommissionPayableUserId.Required") });

                    if (directOrderCalculation.BuyerCommissionReceivableUserId > 0 && directOrderCalculation.BuyerCommissionReceivablePerBag == 0)
                        return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.BuyerCommissionReceivableUserId.Required") });
                }

                //clear cart 
                var cartItems = await _shoppingCartService.GetShoppingCartAsync(buyer, ShoppingCartType.ShoppingCart, storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
                foreach (var cartItem in cartItems)
                    await _shoppingCartService.DeleteShoppingCartItemAsync(cartItem);

                var product = await _productService.GetProductByIdAsync(request.ProductId);
                if (product != null)
                {
                    //validate order calculation
                    directOrderCalculation.BusinessModelId = directOrder.TransactionModelId;
                    var warnings = await PrepareSaleOrderCalculationAsync(new Order(), request, directOrderCalculation);
                    if (warnings.Any())
                        return Ok(new { success = false, message = warnings });

                    if (!warnings.Any())
                    {
                        //now let's try adding product to the cart (now including product attribute validation, etc)
                        var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: buyer,
                        product: product,
                        shoppingCartType: ShoppingCartType.ShoppingCart,
                        storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                        attributesXml: request.ProductAttributeXml,
                        quantity: request.Quantity, overridePrice: directOrderCalculation.SellingPriceOfProduct, brandId: request.BrandId);

                        if (addToCartWarnings.Any())
                            return Ok(new { success = false, message = addToCartWarnings });

                        //save address for buyer 
                        await SaveSaleOrderAddressAsync(request, buyer);

                        //place order
                        var processPaymentRequest = new ProcessPaymentRequest
                        {
                            StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                        };

                        _paymentService.GenerateOrderGuid(processPaymentRequest);
                        processPaymentRequest.CustomerId = request.BuyerId;
                        processPaymentRequest.RequestId = request.Id;
                        processPaymentRequest.OrderTypeId = (int)OrderType.SaleOrder;
                        processPaymentRequest.OrderTotal = directOrderCalculation.SellingPriceOfProduct * request.Quantity;
                        processPaymentRequest.SalePrice = directOrderCalculation.BuyingPrice;
                        processPaymentRequest.Quotation = null;
                        processPaymentRequest.Source = "Tijara";

                        var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                        if (placeOrderResult.Success)
                        {
                            directOrderCalculation.BusinessModelId = directOrder.TransactionModelId;

                            //save sales order calculation
                            await SaveSalesOrderCalculationAsync(placeOrderResult.PlacedOrder, request, directOrderCalculation);

                            //Save Cogs Inventory Taggings
                            var directCogsInventoryTaggings = (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: requestId));
                            foreach (var tagging in directCogsInventoryTaggings)
                            {
                                await _inventoryService.InsertCogsInventoryTaggingAsync(new CogsInventoryTagging
                                {
                                    InventoryId = tagging.InventoryId,
                                    Quantity = tagging.Quantity,
                                    Rate = tagging.Rate,
                                    RequestId = tagging.RequestId,
                                    GrossQuantity = tagging.Quantity
                                });

                                //delete Direct Cogs Inventory Taggings
                                await _inventoryService.DeleteDirectCogsInventoryTaggingAsync(tagging);
                            }

                            //Save delivery schedule
                            foreach (var schedule in await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id))
                            {
                                await _orderService.InsertDeliveryScheduleAsync(new OrderDeliverySchedule
                                {
                                    OrderId = placeOrderResult.PlacedOrder.Id,
                                    CreatedById = user.Id,
                                    ExpectedDeliveryDateUtc = schedule.ExpectedDeliveryDateUtc.Value,
                                    ExpectedShipmentDateUtc = schedule.ExpectedShipmentDateUtc.Value,
                                    ExpectedQuantity = schedule.ExpectedQuantity.Value,
                                    ExpectedDeliveryCost = schedule.ExpectedDeliveryCost.Value,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }

                            //Update request status
                            request.RequestStatus = RequestStatus.Approved;
                            request.BusinessModelId = directOrder.TransactionModelId;
                            request.PinLocation_Latitude = directOrder.PinLocation_Latitude;
                            request.PinLocation_Location = directOrder.PinLocation_Location;
                            request.PinLocation_Longitude = directOrder.PinLocation_Longitude;
                            await _requestService.UpdateRequestAsync(request);

                            //Remove temp
                            var directOrderDeliverySchedules = await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id);
                            foreach (var directOrderDeliverySchedule in directOrderDeliverySchedules)
                                await _orderService.DeleteDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                            //Remove direct order supplier info
                            var directOrderCalculations = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                            foreach (var directorderCalculation in directOrderCalculations)
                                await _orderService.DeleteDirectOrderCalculationAsync(directorderCalculation);

                            await _orderService.DeleteDirectOrderAsync(directOrder);

                            return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Request.SalesOrder.Genearted.Successfully") });
                        }
                        else
                            return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Order.Does'nt.Placed.Success") });
                    }
                }

                return Ok(new { success = false, message = "Product not found" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("sale-order-list")]
        public async Task<IActionResult> OrderManagement_SaleOrderList(bool showActiveOrders = true, int pageIndex = 0, int pageSize = 10)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "User not found" });

            List<Order> allOrders = new List<Order>();
            List<Orders> activeOrders = new List<Orders>();
            List<Orders> pastOrders = new List<Orders>();
            List<int> orderStatusIds = new List<int>();

            if (showActiveOrders)
                orderStatusIds.AddRange(new List<int> { (int)OrderStatus.Pending, (int)OrderStatus.Processing });
            else
                orderStatusIds.AddRange(new List<int> { (int)OrderStatus.Cancelled, (int)OrderStatus.Complete });

            var allOrdersPageList = await _orderService.SearchOrdersAsync(osIds: orderStatusIds, orderTypeId: (int)OrderType.SaleOrder, pageIndex: pageIndex, pageSize: pageSize);

            allOrders = allOrdersPageList.ToList();
            if (allOrders.Any())
            {
                var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);
                //Get Active Request List Filter By Date 
                var activeOrderDates = allOrders.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.OrderStatusId == (int)OrderStatus.Pending || x.OrderStatusId == (int)OrderStatus.Processing).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                foreach (var activeOrderDate in activeOrderDates)
                {
                    var model = new Orders();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == activeOrderDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == activeOrderDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = activeOrderDate.Key;

                    foreach (var order in activeOrderDate)
                    {
                        var paymentDueDate = "";
                        var industry = "";

                        if (order.RequestId > 0)
                        {
                            var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
                            if (buyerRequest is not null)
                            {
                                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";


                                var orderShipments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.SaleOrder));
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

                            var product = await _productService.GetProductByIdAsync((await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault().ProductId);
                            if (product == null)
                                continue;

                            model.Data.Add(new OrdersData
                            {
                                OrderId = order.Id,
                                OrderCustomNumber = order.CustomOrderNumber,
                                CreatedOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                                OrderStatusId = order.OrderStatusId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                BuyerId = buyerRequest.BuyerId,
                                BuyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                                BrandId = buyerRequest.BrandId,
                                BrandName = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                                BookerId = buyerRequest.BookerId,
                                BookerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BookerId))?.FullName,
                                Quantity = buyerRequest.Quantity,
                                PaymentDueDate = paymentDueDate,
                                Industry = industry,
                            });
                        }

                    }
                    activeOrders.Add(model);
                }

                var pastOrderDates = allOrders.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.OrderStatusId == (int)OrderStatus.Cancelled).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                foreach (var pastOrderDate in pastOrderDates)
                {
                    var model = new Orders();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == pastOrderDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == pastOrderDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = pastOrderDate.Key;

                    foreach (var order in pastOrderDate)
                    {
                        var paymentDueDate = "";
                        var industry = "";

                        if (order.RequestId > 0)
                        {
                            var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
                            if (buyerRequest is not null)
                            {
                                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";


                                var orderShipments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.SaleOrder));
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

                            var product = await _productService.GetProductByIdAsync((await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault().ProductId);
                            if (product == null)
                                continue;

                            model.Data.Add(new OrdersData
                            {
                                OrderId = order.Id,
                                OrderCustomNumber = order.CustomOrderNumber,
                                CreatedOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                                OrderStatusId = order.OrderStatusId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                BuyerId = buyerRequest.BuyerId,
                                BuyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                                BrandId = buyerRequest.BrandId,
                                BrandName = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                                BookerId = buyerRequest.BookerId,
                                BookerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BookerId))?.FullName,
                                Quantity = buyerRequest.Quantity,
                                PaymentDueDate = paymentDueDate,
                                Industry = industry,
                            });
                        }
                    }
                    pastOrders.Add(model);
                }

                var data = new List<object>();
                if (activeOrders.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Order.ActiveOrder"),
                        Data = activeOrders
                    });
                }
                if (pastOrders.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Order.PastOrder"),
                        Data = pastOrders
                    });
                }

                return Ok(new { success = true, message = "", data = data, totalPages = allOrdersPageList.TotalPages, currentPage = allOrdersPageList.PageIndex });

            }
            else
            {
                return Ok(new { success = false, message = "No orders found." });
            }
        }

        [HttpGet("sale-order-detail/{orderId}")]
        public async Task<IActionResult> OrderManagement_SaleOrderDetail(int orderId)
        {
            if (orderId <= 0)
                return Ok(new { success = false, message = "Buyer order id is required" });

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) /*|| !await _customerService.IsBuyerAsync(buyer)*/)
                return Ok(new { success = false, message = "Buyer not found" });

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                return Ok(new { success = false, message = "Order not found" });

            var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
            if (buyerRequest is null || buyerRequest.Deleted)
                return Ok(new { success = false, message = "Order request not found" });

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

                industry = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";
                quantity = buyerRequest.Quantity;

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is not null)
                {
                    productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
                    qtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty;
                }

                brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-";
                deliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM yy");

                var city = (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.CityId));
                if (city != null)
                    deliveryAddress += city.Name + ", ";

                var area = (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.AreaId));
                if (area != null)
                    deliveryAddress += area.Name + ", ";

                if (!string.IsNullOrWhiteSpace(buyerRequest.DeliveryAddress))
                    deliveryAddress += buyerRequest.DeliveryAddress + ", " + buyerRequest.DeliveryAddress2;

                if (order.ShippingStatusId == (int)ShippingStatus.Delivered || order.ShippingStatusId == (int)ShippingStatus.Shipped)
                {
                    var orderShipment = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.SaleOrder)).FirstOrDefault();
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
                    var totalNumberOfItemsCanBeAddedToShipment = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
                    if (totalNumberOfItemsCanBeAddedToShipment <= 0)
                        continue;

                    //yes, we have at least one item to create a new shipment
                    shipmentBalanceQuantity += await _priceCalculationService.RoundPriceAsync(totalNumberOfItemsCanBeAddedToShipment);
                }

                var buyerContract = await _orderService.GetContractByOrderIdAsync(order.Id);
                if (buyerContract is not null)
                    await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}");

                var data = new
                {
                    OrderInfo = new
                    {
                        OrderId = order.Id,
                        CustomOrderNumber = order.CustomOrderNumber,
                        OrderStatusId = order.OrderStatusId,
                        OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                        Industry = industry,
                        Product = productName,
                        Brand = brand,
                        TotalQuantity = quantity,
                        QtyType = qtyType,
                        DeliveryDate = deliveryDate,
                        OrderCreatedDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
                        OrderCreatedTime = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                        BuyerDeliveryAddress = deliveryAddress,
                        BookerId = buyerRequest.BookerId,
                        BookerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BookerId))?.FullName,
                        BuyerRequestId = buyerRequest.Id,
                        BuyerName = (await _customerService.GetCustomerByIdAsync(customerId: order.CustomerId))?.FullName,
                        BuyerEmail = (await _customerService.GetCustomerByIdAsync(customerId: order.CustomerId))?.Email,
                        ProductAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                        BuyerContract = new
                        {
                            contractId = buyerContract != null ? buyerContract.Id : 0,
                            downloadUrl = buyerContract != null ? _storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}" : "",
                            previewUrl = buyerContract != null ? _storeContext.GetCurrentStore().Url + $"files/contracts/{buyerContract.ContractGuid}.pdf" : "",
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
                        NoOfShipmentNumber = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Count(),
                        QuantityLeft = shipmentBalanceQuantity,
                        Shippments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.SaleOrder)).Select(async s =>
                        {
                            return new
                            {
                                ShippmentId = s.Id,
                                ShipmentStatus = (await _localizationService.GetLocalizedEnumAsync(s.DeliveryStatus)),
                            };
                        }).ToList(),
                        PinLocation = new
                        {
                            Location = buyerRequest.PinLocation_Location,
                            Longitude = buyerRequest.PinLocation_Longitude,
                            Latitude = buyerRequest.PinLocation_Latitude
                        }
                    },
                    expectedShipments = await _orderService.GetAllDeliveryScheduleAsync(orderId: order.Id).Result.SelectAwait(async b =>
                    {
                        return new
                        {
                            expectedDeliveryDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(b.ExpectedDeliveryDateUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                            expectedShipmentDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(b.ExpectedShipmentDateUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                            expectedDeliveryCost = b.ExpectedDeliveryCost,
                            Quantity = b.ExpectedQuantity
                        };
                    }).ToListAsync(),
                    //Requests = await _orderService.SearchOrderDeliveryRequestsAsync(orderId: orderId).Result.SelectAwait(async b =>
                    //{
                    //    return new
                    //    {
                    //        id = b.Id,
                    //        orderId = b.OrderId,
                    //        orderCustomNumber = (await _orderService.GetOrderByIdAsync(b.OrderId)).CustomOrderNumber,
                    //        //directOrderCalculationId = b.QuotationId,
                    //        //directOrderCalculationNumber = (await _quotationService.GetQuotationByIdAsync(b.QuotationId))?.CustomQuotationNumber,
                    //        statusId = b.StatusId,
                    //        status = await _localizationService.GetLocalizedEnumAsync(b.OrderDeliveryRequestEnum),
                    //        bagsDirectlyFromSupplier = b.BagsDirectlyFromSupplier,
                    //        bagsDirectlyFromWarehouse = b.BagsDirectlyFromWarehouse,
                    //        countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(b.CountryId), x => x.Name),
                    //        countryId = b.CountryId,
                    //        cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(b.CityId), x => x.Name),
                    //        cityId = b.CityId,
                    //        areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(b.AreaId), x => x.Name),
                    //        areaId = b.AreaId,
                    //        streetaddress = b.StreetAddress,
                    //        contactnumber = b.ContactNumber,
                    //        totalQuantity = quantity,
                    //        deliveredQuantity = b.Quantity,
                    //        shipmentdate = b.ShipmentDateUtc,
                    //        agentId = b.AgentId,
                    //        agent = (await _customerService.GetCustomerByIdAsync(b.AgentId))?.FullName,
                    //        timeRemaining = b.TicketExpiryDate.HasValue ? b.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || b.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? b.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + b.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                    //        date = (await _dateTimeHelper.ConvertToUserTimeAsync(b.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                    //        priority = await _localizationService.GetLocalizedEnumAsync(b.TicketEnum),
                    //        requester = (await _customerService.GetCustomerByIdAsync(b.CreatedById))?.FullName,
                    //        buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                    //        brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-"
                    //    };
                    //}).ToListAsync(),
                    //POHistory = await _orderService.GetAllOrderPaymentsAsync(paymentType: "Payable", orderId: orderId).Result.OrderByDescending(x => x.Id).SelectAwait(async b =>
                    //{
                    //    return new
                    //    {
                    //        id = b.Id,
                    //        orderId = b.OrderId,
                    //        orderCustomNumber = (await _orderService.GetOrderByIdAsync(b.OrderId)).CustomOrderNumber,
                    //        supplierId = b.SupplierId,
                    //        supllierName = (await _customerService.GetCustomerByIdAsync(b.SupplierId))?.FullName,
                    //        directOrderCalculationId = b.QuotationId,
                    //        directOrderCalculationNumber = (await _sellerBidService.GetSellerBidByIdAsync(b.QuotationId))?.CustomQuotationNumber,
                    //        statusId = b.OrderPaymentStatusId,
                    //        status = await _localizationService.GetLocalizedEnumAsync(b.OrderPaymentStatus),
                    //        deliveredQuantity = b.Quantity,
                    //        date = (await _dateTimeHelper.ConvertToUserTimeAsync(b.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                    //        time = (await _dateTimeHelper.ConvertToUserTimeAsync(b.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                    //    };
                    //}).ToListAsync(),
                    //ReturnHistory = await _orderService.SearchOrderSalesReturnRequestsAsync(orderId: orderId).Result.SelectAwait(async salesReturnRequests =>
                    //{
                    //    return new
                    //    {
                    //        id = salesReturnRequests.Id,
                    //        orderId = salesReturnRequests.OrderId,
                    //        supplierId = salesReturnRequests.SupplierId,
                    //        supllierName = (await _customerService.GetCustomerByIdAsync(salesReturnRequests.SupplierId))?.FullName,
                    //        directOrderCalculationId = salesReturnRequests.QuotationId,
                    //        directOrderCalculationNumber = (await _sellerBidService.GetSellerBidByIdAsync(salesReturnRequests.QuotationId))?.CustomQuotationNumber,
                    //        returnRequestDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(salesReturnRequests.ReturnRequestDateUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                    //        quantity = salesReturnRequests.Quantity,
                    //        pickupAddress = salesReturnRequests.PickupAddress,
                    //        dropOffAddress = salesReturnRequests.DropOffAddress,
                    //        isInventory = salesReturnRequests.IsInventory,
                    //        returnReason = salesReturnRequests.ReturnReason,
                    //        agentId = salesReturnRequests.AgentId,
                    //        agent = (await _customerService.GetCustomerByIdAsync(salesReturnRequests.AgentId))?.FullName,
                    //        createdOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(salesReturnRequests.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                    //        statusId = salesReturnRequests.StatusId,
                    //        status = await _localizationService.GetLocalizedEnumAsync(salesReturnRequests.orderSalesReturnRequestEnum),
                    //    };
                    //}).ToListAsync()
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("buyer-order-summary/{orderId}")]
        public async Task<IActionResult> OrderManagement_BuyerOrderSummary(int orderId)
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

        #endregion

        #region Cost Of Goods Selling

        [HttpGet("cogs-inventory-list/{requestId}")]
        public async Task<IActionResult> OrderManagement_CogsInventoryList(int requestId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                var request = await _requestService.GetRequestByIdAsync(requestId);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                var data = await ((await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: requestId)).SelectAwait(async cg =>
                {
                    return new
                    {
                        id = cg.Id,
                        rate = (await _inventoryService.GetInventoryInboundByIdAsync(cg.InventoryId))?.PurchaseRate,
                        quantity = cg.Quantity,
                        inventoryId = cg.InventoryId
                    };
                }).ToListAsync());

                return Ok(new { success = true, data, requestQuantity = request.Quantity });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-inventories-by-request/{requestId}")]
        public async Task<IActionResult> OrderManagement_GetInventoriesByRequest(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                return Ok(new { success = false, message = "Request not found" });

            try
            {
                var data = new List<object>();

                //prepare available warehouses
                var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
                if (inventoryGroup is not null)
                {
                    var inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).OrderBy(x => x.Id).ToList();

                    if (request.BusinessModelId == (int)BusinessModelEnum.Broker)
                        inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Where(x => x.BusinessModelId == request.BusinessModelId).OrderBy(x => x.Id).ToList();
                    else
                        inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Where(x => x.BusinessModelId != (int)(BusinessModelEnum.Broker)).OrderBy(x => x.Id).ToList();

                    foreach (var inventoryInbound in inventoryInbounds)
                    {
                        //var outboundQty = (await _inventoryService.GetAllInventoryOutboundsAsync(InventoryInboundId: inventoryInbound.Id)).Sum(x => x.OutboundQuantity);
                        var inStockQuantity = inventoryInbound.StockQuantity; //- outboundQty;

                        var cogsInventoryTaggedQuantity = (await _inventoryService.GetAllCogsInventoryTaggingsAsync(inventoryInbound.Id)).Sum(x => x.Quantity);
                        var balanceQuantity = inStockQuantity - cogsInventoryTaggedQuantity;
                        if (balanceQuantity > 0)
                            data.Add(new { Text = "Inventory # : " + inventoryInbound.Id + " - " + "Quantity : " + balanceQuantity, Value = inventoryInbound.Id.ToString(), Quantity = balanceQuantity, totalQuantity = inStockQuantity, outBoundQuantity = balanceQuantity });
                    }
                }

                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add-cogs-inventory-tagging")]
        public async Task<IActionResult> OrderManagement_AddCogsInventoryTagging([FromBody] DirectOrderApiModel.DirectCogsInventoryTaggingModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });
            try
            {
                var request = await _requestService.GetRequestByIdAsync(model.requestId);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                var inventoryInbound = await _inventoryService.GetInventoryInboundByIdAsync(model.inventoryId);
                if (inventoryInbound is not null)
                {
                    if (model.quantity > inventoryInbound.StockQuantity)
                        return Json(new { success = false, message = "Quantity exceed from the inventory quantity" });
                }

                var cogsInventoryTaggings = (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id));
                if (cogsInventoryTaggings.Any())
                {
                    var checkQuantity = cogsInventoryTaggings.Sum(x => x.Quantity) + model.quantity;
                    if (checkQuantity > request.Quantity)
                        return Json(new { success = false, message = "Quantity exceed from the request quantity" });
                }

                var actualCogsInventoryTaggings = (await _inventoryService.GetAllCogsInventoryTaggingsAsync(requestId: request.Id));

                //if (cogsInventoryTaggings.Any(x => x.InventoryId == model.inventoryId) && actualCogsInventoryTaggings.Any(x => x.InventoryId == model.inventoryId))
                //    return Json(new { success = false, message = "Cogs tagging already exist for the selected inventory" });

                if ((cogsInventoryTaggings.Sum(x => x.Quantity) + actualCogsInventoryTaggings.Sum(x => x.Quantity) + model.quantity) > request.Quantity)
                    return Json(new { success = false, message = "Quantity exceed from the request quantity" });

                if (!cogsInventoryTaggings.Any(x => x.InventoryId == model.inventoryId) && !actualCogsInventoryTaggings.Any(x => x.InventoryId == model.inventoryId))
                {
                    if (request.BusinessModelId == (int)BusinessModelEnum.Broker && cogsInventoryTaggings.Count >= 1)
                        return Json(new { success = false, message = "Do not add multiple inventories in broker" });
                    else
                    {
                        await _inventoryService.InsertDirectCogsInventoryTaggingAsync(new DirectCogsInventoryTagging
                        {
                            InventoryId = model.inventoryId,
                            Quantity = model.quantity,
                            Rate = (await _inventoryService.GetInventoryInboundByIdAsync(model.inventoryId)).PurchaseRate,
                            RequestId = request.Id,
                        });
                    }
                }
                else
                    return Json(new { success = false, message = "Cogs tagging already exist for the selected inventory" });

                //decimal buyingPrice = 0m;
                //Prepare Buying Price for first time load 
                //if (cogsInventoryTaggings.Any())
                //{
                //    var cogsCalculationValue = 0m;
                //    foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                //    {
                //        var inventoryRateQuantity = cogsInventoryTagging.Quantity * cogsInventoryTagging.Rate;
                //        cogsCalculationValue += inventoryRateQuantity;
                //    }
                //    buyingPrice = cogsCalculationValue / cogsInventoryTaggings.Sum(x => x.Quantity);
                //}
                //else
                //    mappingAlreadyExist = true;


                var taggedInventories = (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id));
                decimal buyingPrice = await CalculateBuyingPriceByTaggings(taggedInventories.ToList());
                var buyingPriceFormatted = await _priceFormatter.FormatPriceAsync(buyingPrice, true, false);

                return Ok(new { success = true, message = "Cogs inventory tagging add", buyingPrice, buyingPriceFormatted });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("delete-cogs-inventory-tagging/{id}")]
        public async Task<IActionResult> OrderManagement_DeleteCogsInventoryTagging(int id)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "User not found" });

            try
            {
                var cogsInventoryTagging = await _inventoryService.GetDirectCogsInventoryTaggingByIdAsync(id);
                if (cogsInventoryTagging is null)
                    return Ok(new { success = false, message = "Cogs inventory mapping not found" });

                await _inventoryService.DeleteDirectCogsInventoryTaggingAsync(cogsInventoryTagging);

                return Json(new { success = true, message = "cogs inventory taggings deleted successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Temp Order Expected shipments

        [HttpGet("add-expected-shipment/{tempOrderId}")]
        public async Task<IActionResult> OrderManagement_TempOrder_AddExpectedShipment(int tempOrderId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });
            try
            {
                var tempOrder = await _orderService.GetDirectOrderByIdAsync(tempOrderId);
                if (tempOrder is null)
                    return Ok(new { success = false, message = "Direct order not found" });

                //Add default direct order delivery schedule 
                await _orderService.InsertDirectOrderDeliveryScheduleAsync(new DirectOrderDeliverySchedule
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpectedDeliveryDateUtc = null,
                    ExpectedShipmentDateUtc = null,
                    ExpectedDeliveryCost = 0,
                    ExpectedQuantity = null,
                    DirectOrderId = tempOrder.Id
                });

                var deliveryschedules = (await _orderService.GetAllDirectOrderDeliveryScheduleAsync(tempOrder.Id)).Select(async d =>
                {
                    return new
                    {
                        id = d.Id,
                        expectedDeliveryDateUtc = d.ExpectedDeliveryDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedDeliveryDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                        expectedShipmentDateUtc = d.ExpectedShipmentDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedShipmentDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                        expectedQuantity = d.ExpectedQuantity.HasValue ? d.ExpectedQuantity.ToString() : "",
                        expectedDeliveryCost = d.ExpectedDeliveryCost.HasValue ? d.ExpectedDeliveryCost.ToString() : "",
                        createdOnUtc = d.CreatedOnUtc,
                        directOrderId = d.DirectOrderId
                    };
                }).Select(t => t.Result).ToList();
                return Ok(new { success = true, message = "Expected shipment add", data = deliveryschedules });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("update-expected-shipment")]
        public async Task<IActionResult> OrderManagement_TempOrder_UpdateExpectedShipment([FromBody] DirectOrderApiModel.DirectOrderDeliveryScheduleModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });
            try
            {
                var tempOrder = await _orderService.GetDirectOrderByIdAsync(model.tempOrderId);
                if (tempOrder is null)
                    return Ok(new { success = false, message = "Temp order not found" });

                var directOrderDeliverySchedule = await _orderService.GetDirectOrderDeliveryScheduleByIdAsync(model.Id);
                if (directOrderDeliverySchedule is null)
                    return Ok(new { success = false, message = "Delivery schedule not found" });

                if (model.DeliveryDate.HasValue)
                {
                    var date = model.DeliveryDate.Value.Subtract(DateTime.UtcNow);
                    if (date.Hours < 0 || date.Minutes < 0)
                        return Ok(new { success = false, message = "Invalid Date" });

                    directOrderDeliverySchedule.ExpectedDeliveryDateUtc = model.DeliveryDate.Value;
                }
                if (model.ShipmentDate.HasValue)
                {
                    var date = model.ShipmentDate.Value.Subtract(DateTime.UtcNow);
                    if (date.Hours < 0 || date.Minutes < 0)
                        return Ok(new { success = false, message = "Invalid Date" });

                    directOrderDeliverySchedule.ExpectedShipmentDateUtc = model.ShipmentDate.Value;
                }

                if (model.DeliveryCost > 0)
                    directOrderDeliverySchedule.ExpectedDeliveryCost = model.DeliveryCost;

                if (model.Quantity > 0)
                    directOrderDeliverySchedule.ExpectedQuantity = model.Quantity;

                await _orderService.UpdateDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                return Ok(new { success = true, message = "Expected delivery schedule updated" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("delete-expected-shipment/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_TempOrder_DeleteExpectedDeliverySchedule(int expectedShipmentId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "User not found" });

            try
            {
                var expectedshipment = await _orderService.GetDirectOrderDeliveryScheduleByIdAsync(expectedShipmentId);
                if (expectedshipment is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                await _orderService.DeleteDirectOrderDeliveryScheduleAsync(expectedshipment);

                return Ok(new { success = true, message = "Expected delivery schedule deleted" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Supplier Booker

        [AllowAnonymous]
        [HttpPost("seller-registration")]
        public async Task<IActionResult> OrderManagement_SellerRegistration([FromBody] AccountApiModel.BookerSellerRegisterApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (!ModelState.IsValid)
                return Ok(new { success = false, message = GetModelErrors(ModelState) });

            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Register.Result.Disabled") });

            if (!CommonHelper.IsValidEmail(model.Email))
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Common.WrongEmail") });

            if (string.IsNullOrWhiteSpace(model.Phone))
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Fields.Username.Required") });

            var customer = new Customer
            {
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                Source = "Tijara-APP",
                CreatedBy = user.Id
            };
            await _customerService.InsertCustomerAsync(customer);

            var customerEmail = model.Email?.Trim();
            var customerUserName = model.Phone?.Trim();

            var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                customerUserName,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                (await _storeContext.GetCurrentStoreAsync()).Id,
                true,
                isSupplier: true);

            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (registrationResult.Success)
            {
                //form fields
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                if (_customerSettings.StreetAddressEnabled)
                    customer.StreetAddress = model.Address;
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                customer.Gst = model.SupplierGST;
                if (model.IndustryId > 0)
                    customer.IndustryId = model.IndustryId;
                if (model.SupplierType > 0)
                    customer.UserTypeId = model.SupplierType;

                await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.VatNumberAttribute, model.SupplierNTN);

                if (model.BookerCurrentLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredBookerCurrentLocationAttribute, $"{model.BookerCurrentLocation.Latitude},{model.BookerCurrentLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredBookerCurrentLocationAddressAttribute, model.BookerCurrentLocation.Location);
                }

                if (model.SellerPinLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, $"{model.SellerPinLocation.Latitude},{model.SellerPinLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute, model.SellerPinLocation.Location);
                }

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Company = customer.Company,
                    CountryId = customer.CountryId > 0
                            ? (int?)customer.CountryId
                            : null,
                    StateProvinceId = customer.StateProvinceId > 0
                            ? (int?)customer.StateProvinceId
                            : null,
                    County = customer.County,
                    City = customer.City,
                    Address1 = customer.StreetAddress,
                    Address2 = customer.StreetAddress2,
                    ZipPostalCode = customer.ZipPostalCode,
                    PhoneNumber = customer.Phone,
                    FaxNumber = customer.Fax,
                    CreatedOnUtc = customer.CreatedOnUtc
                };
                if (await _addressService.IsAddressValidAsync(defaultAddress))
                {
                    //some validation
                    if (defaultAddress.CountryId == 0)
                        defaultAddress.CountryId = null;
                    if (defaultAddress.StateProvinceId == 0)
                        defaultAddress.StateProvinceId = null;
                    //set default address
                    //customer.Addresses.Add(defaultAddress);

                    await _addressService.InsertAddressAsync(defaultAddress);

                    await _customerService.InsertCustomerAddressAsync(customer, defaultAddress);

                    customer.BillingAddressId = defaultAddress.Id;
                    customer.ShippingAddressId = defaultAddress.Id;
                }

                if (model.ProductIds != null && model.ProductIds.Any())
                {
                    foreach (var productId in model.ProductIds)
                    {
                        //insert the new product supplier mapping
                        await _productService.InsertSupplierProductAsync(new SupplierProduct
                        {
                            SupplierId = customer.Id,
                            ProductId = productId,
                            Published = true,
                            DisplayOrder = 1
                        });
                    }
                }

                await _customerService.UpdateCustomerAsync(customer);

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    await _workflowMessageService.SendSupplierRegisteredNotificationMessageAsync(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

                //send customer welcome message
                await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Account.Register.Result.Standard") });
            }

            return Ok(new { success = false, message = string.Join(",", registrationResult.Errors) });
        }

        [HttpGet("search-suppliers/{name}")]
        public async Task<IActionResult> OrderManagement_SearchSuppliers(string name = "")
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (string.IsNullOrEmpty(name))
                    return Ok(new { success = false, message = "Name is required" });

                var customerRoles = await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName);

                var suppliers = (await _customerService.GetAllCustomersAsync(isActive: true, customerRoleIds: new int[] { customerRoles.Id }, fullName: name));
                if (suppliers.Any())
                {
                    var data = suppliers.ToList().Select(async b =>
                    {
                        return new
                        {
                            Id = b.Id,
                            FullName = b.FullName,
                            Email = b.Email,
                            Phone = b.Username,
                            IndustryId = await _genericAttributeService.GetAttributeAsync<int>(b, ZarayeCustomerDefaults.SupplierIndustryIdAttribute)
                        };
                    }).Select(t => t.Result).ToList();

                    return Ok(new { success = true, data });
                }

                return Ok(new { success = false, message = "no supplier found" });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search-suppliers-by-product/{productId}")]
        public async Task<IActionResult> OrderManagement_SearchSuppliersByProductId(int productId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (productId == 0)
                    return Ok(new { success = false, message = "Product id is required" });

                var suppliers = (await _customerService.GetAllCustomersAsync(
                    customerRoleIds: new int[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id },
                    productId: productId)).ToList();

                IList<object> data = new List<object>();

                foreach (var supplier in suppliers)
                {
                    data.Add(new
                    {
                        Value = supplier.Id,
                        Text = supplier.FullName,
                    });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("request-for-quotation-history")]
        public async Task<IActionResult> OrderManagement_GetRequestForQuotationHistory()
        {
            var supplier = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(supplier) /*|| !await _customerService.IsBookerAsync(booker)*/)
                return Ok(new { success = false, message = "Booker not found" });

            List<RequestForQuotation> allrequestForQuotations = new List<RequestForQuotation>();
            List<RequestForQuotationsModel> activeRequestForQuotations = new List<RequestForQuotationsModel>();
            List<RequestForQuotationsModel> pastRequestForQuotations = new List<RequestForQuotationsModel>();
            List<int> requestStatusIds = new List<int> { (int)RequestForQuotationStatus.Verified, (int)RequestForQuotationStatus.Cancelled, (int)RequestForQuotationStatus.UnVerified, (int)RequestForQuotationStatus.Expired };
            try
            {
                //Get all buyer request
                allrequestForQuotations = (await _requestService.GetAllRequestForQuotationAsync(rfqsIds: requestStatusIds, bookerId: supplier.Id)).ToList();
                if (allrequestForQuotations.Any())
                {
                    var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);

                    //Get Active Request List Filter By Date 
                    var activeRequestForQuotationDates = allrequestForQuotations.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.RfqStatusId == (int)RequestForQuotationStatus.Verified).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                    foreach (var activeRequestForQuotationDate in activeRequestForQuotationDates)
                    {
                        var model = new RequestForQuotationsModel();
                        var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                        if (currentTime.ToString("dd/MM/yyyy") == activeRequestForQuotationDate.Key)
                            model.Date = "Today";
                        else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == activeRequestForQuotationDate.Key)
                            model.Date = "Yesterday";
                        else
                            model.Date = activeRequestForQuotationDate.Key;

                        foreach (var activeRequestForQuotation in activeRequestForQuotationDate)
                        {
                            var request = await _requestService.GetRequestByIdAsync(activeRequestForQuotation.RequestId);
                            if (request is null)
                                continue;

                            var product = await _productService.GetProductByIdAsync(request.ProductId);
                            if (product is null)
                                continue;

                            var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
                            if (category is null)
                                continue;

                            var industry = await _industryService.GetIndustryByIdAsync(request.IndustryId);
                            if (industry is null)
                                continue;

                            var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);
                            if (buyer is null)
                                continue;

                            model.Data.Add(new RequestForQuotationData
                            {
                                Id = activeRequestForQuotation.Id,
                                CustomNumber = activeRequestForQuotation.CustomRfqNumber,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                IndustryId = industry.Id,
                                IndustryName = industry.Name,
                                BuyerId = buyer.Id,
                                BuyerName = buyer.FullName,
                                BrandName = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(request.BrandId)), x => x.Name) : "-",
                                BrandId = request.BrandId,
                                Quantity = activeRequestForQuotation.Quantity,
                                DeliveryAddress = request.DeliveryAddress,
                                //TotalQuotations = (await _sellerBidService.GetAllSellerBidAsync(buyerRequestId: activeRequestForQuotation.Id, sbIds: new List<int> { (int)QuotationStatus.Verified })).Count,
                                ExpiryDate = request.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(request.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                                StatusId = activeRequestForQuotation.RfqStatusId,
                                Status = await _localizationService.GetLocalizedEnumAsync(activeRequestForQuotation.RequestForQuotationStatus),
                                AttributesInfo = await Booker_ParseAttributeXml(request.ProductAttributeXml)
                            });
                        }
                        activeRequestForQuotations.Add(model);
                    }

                    //Get Active Request List Filter By Date 
                    var pastRequestForQuotationsDates = allrequestForQuotations.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.RfqStatusId == (int)RequestForQuotationStatus.UnVerified || x.RfqStatusId == (int)RequestForQuotationStatus.Cancelled || x.RfqStatusId == (int)RequestForQuotationStatus.Expired).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                    foreach (var pastRequestForQuotationsDate in pastRequestForQuotationsDates)
                    {
                        var model = new RequestForQuotationsModel();
                        if (currentTime.ToString("dd/MM/yyyy") == pastRequestForQuotationsDate.Key)
                            model.Date = "Today";
                        else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == pastRequestForQuotationsDate.Key)
                            model.Date = "Yesterday";
                        else
                            model.Date = pastRequestForQuotationsDate.Key;

                        foreach (var pastRequestForQuotation in pastRequestForQuotationsDate)
                        {
                            var request = await _requestService.GetRequestByIdAsync(pastRequestForQuotation.RequestId);
                            if (request is null)
                                continue;

                            var product = await _productService.GetProductByIdAsync(request.ProductId);
                            if (product is null)
                                continue;

                            var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
                            if (category is null)
                                continue;

                            var industry = await _industryService.GetIndustryByIdAsync(request.IndustryId);
                            if (industry is null)
                                continue;

                            var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);
                            if (buyer is null)
                                continue;

                            model.Data.Add(new RequestForQuotationData
                            {
                                Id = pastRequestForQuotation.Id,
                                CustomNumber = pastRequestForQuotation.CustomRfqNumber,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                IndustryId = industry.Id,
                                IndustryName = industry.Name,
                                BuyerId = buyer.Id,
                                BuyerName = buyer.FullName,
                                BrandName = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(request.BrandId)), x => x.Name) : "-",
                                BrandId = request.BrandId,
                                Quantity = pastRequestForQuotation.Quantity,
                                DeliveryAddress = request.DeliveryAddress,
                                //TotalQuotations = (await _sellerBidService.GetAllSellerBidAsync(buyerRequestId: request.Id, sbIds: new List<int> { (int)SellerBidStatus.QuotedToBuyer })).Count,
                                ExpiryDate = request.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(request.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                                StatusId = pastRequestForQuotation.RfqStatusId,
                                Status = await _localizationService.GetLocalizedEnumAsync(pastRequestForQuotation.RequestForQuotationStatus),
                                AttributesInfo = await Booker_ParseAttributeXml(request.ProductAttributeXml)
                            });
                        }
                        pastRequestForQuotations.Add(model);
                    }

                    var data = new List<object>();
                    if (activeRequestForQuotations.Any())
                    {
                        data.Add(new
                        {
                            DataType = await _localizationService.GetResourceAsync("Buyer.ActiveRequests"),
                            Data = activeRequestForQuotations
                        });
                    }
                    if (pastRequestForQuotations.Any())
                    {
                        data.Add(new
                        {
                            DataType = await _localizationService.GetResourceAsync("Buyer.PastRequests"),
                            Data = pastRequestForQuotations
                        });
                    }

                    return Ok(new { success = true, message = "", data });
                }

                return Ok(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("quotation-history")]
        public async Task<IActionResult> OrderManagement_QuotationHistory()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            List<OrderManagementApiModel.Bids> activeBids = new List<OrderManagementApiModel.Bids>();
            List<OrderManagementApiModel.Bids> pastBids = new List<OrderManagementApiModel.Bids>();
            List<Quotation> allBids = new List<Quotation>();
            List<int> bidsStatusIds = new List<int> { (int)QuotationStatus.Verified, (int)QuotationStatus.Cancelled, (int)QuotationStatus.UnVerified, (int)QuotationStatus.Expired };

            try
            {
                allBids = (await _quotationService.GetAllQuotationAsync(sbIds: bidsStatusIds/*, bookerId: user.Id*/)).ToList();
                if (allBids.Any())
                {
                    //Prepare Active quotations
                    var activequotations = allBids.Where(x => x.QuotationStatusId == (int)QuotationStatus.Verified).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                    foreach (var activequotation in activequotations)
                    {
                        var model = new OrderManagementApiModel.Bids();
                        model.Date = activequotation.Key;
                        foreach (var quotation in activequotation)
                        {
                            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(quotation.RfqId);
                            if (requestForQuotation is null)
                                continue;

                            var buyerRequest = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
                            if (buyerRequest is null)
                                continue;

                            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                            if (product is null)
                                continue;

                            var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                            if (category is null)
                                continue;

                            model.Data.Add(new OrderManagementApiModel.Bids.BidsData
                            {
                                Id = quotation.Id,
                                CustomQuotationNumber = quotation.CustomQuotationNumber,
                                BuyerRequestId = buyerRequest.Id,
                                SupplierId = quotation.SupplierId,
                                SupplierName = await _customerService.GetCustomerFullNameAsync(quotation.SupplierId),
                                BuyerId = buyerRequest.BuyerId,
                                BuyerName = (await _customerService.GetCustomerByIdAsync(buyerRequest.BuyerId))?.FullName,
                                ProductName = product.Name,
                                Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                                BrandId = quotation.BrandId,
                                Quantity = quotation.Quantity,
                                QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                                UnitPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice),
                                BidPrice = await _priceFormatter.FormatPriceAsync((quotation.QuotationPrice * quotation.Quantity)),
                                //PriceValidity = (await _dateTimeHelper.ConvertToUserTimeAsync(quotation.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy hh:mm tt") /*quotation.PriceValidity.ToString("dd MMM, yyyy hh:mm tt")*/,
                                ExpiryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(quotation.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy hh:mm tt"),
                                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quotation.CreatedOnUtc, DateTimeKind.Utc),
                                Status = await _localizationService.GetLocalizedEnumAsync(quotation.QuotationStatus),
                                StatusId = quotation.QuotationStatusId,
                                IndustryName = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-",
                                CategoryName = (await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId))?.Name ?? "-"
                            });
                        }
                        activeBids.Add(model);
                    }

                    //Prepare Active quotations
                    var pastquotations = allBids.Where(x => x.QuotationStatusId == (int)QuotationStatus.Cancelled || x.QuotationStatusId == (int)QuotationStatus.UnVerified || x.QuotationStatusId == (int)QuotationStatus.Expired).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                    foreach (var pastquotation in pastquotations)
                    {
                        var model = new OrderManagementApiModel.Bids();
                        model.Date = pastquotation.Key;
                        foreach (var quotation in pastquotation)
                        {
                            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(quotation.RfqId);
                            if (requestForQuotation is null)
                                continue;

                            var buyerRequest = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
                            if (buyerRequest is null)
                                continue;

                            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                            if (product is null)
                                continue;

                            var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                            if (category is null)
                                continue;

                            model.Data.Add(new OrderManagementApiModel.Bids.BidsData
                            {
                                Id = quotation.Id,
                                CustomQuotationNumber = quotation.CustomQuotationNumber,
                                BuyerRequestId = buyerRequest.Id,
                                SupplierId = quotation.SupplierId,
                                SupplierName = await _customerService.GetCustomerFullNameAsync(quotation.SupplierId),
                                BuyerId = buyerRequest.BuyerId,
                                BuyerName = (await _customerService.GetCustomerByIdAsync(buyerRequest.BuyerId))?.FullName,
                                ProductName = product.Name,
                                Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                                BrandId = quotation.BrandId,
                                Quantity = quotation.Quantity,
                                QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                                UnitPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice),
                                BidPrice = await _priceFormatter.FormatPriceAsync((quotation.QuotationPrice * quotation.Quantity)),
                                //BidPrice = quotation.SellerBidStatus == SellerBidStatus.Pending ? await _priceFormatter.FormatPriceAsync(quotation.BidPrice) : await _priceFormatter.FormatPriceAsync(quotation.TotalPayble),
                                //PriceValidity = (await _dateTimeHelper.ConvertToUserTimeAsync(quotation.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy hh:mm tt") /*quotation.PriceValidity.ToString("dd MMM, yyyy hh:mm tt")*/,
                                ExpiryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(quotation.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy hh:mm tt"),
                                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quotation.CreatedOnUtc, DateTimeKind.Utc),
                                Status = await _localizationService.GetLocalizedEnumAsync(quotation.QuotationStatus),
                                StatusId = quotation.QuotationStatusId,
                                IndustryName = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-",
                                CategoryName = (await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId))?.Name ?? "-"
                            });
                        }
                        pastBids.Add(model);
                    }

                    var data = new List<object>();
                    if (activeBids.Count > 0)
                    {
                        data.Add(new
                        {
                            DataType = await _localizationService.GetResourceAsync("Supplier.ActiveQuotations"),
                            Data = activeBids
                        });
                    }
                    if (pastBids.Count > 0)
                    {
                        data.Add(new
                        {
                            DataType = await _localizationService.GetResourceAsync("Supplier.PastQuotations"),
                            Data = pastBids
                        });
                    }

                    return Ok(new { success = true, data });
                }
                return Ok(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("seller-market-data")]
        public async Task<IActionResult> OrderManagement_MarketDataBySupplier(int industryId, int categoryId = 0, int productId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            //get parameters to filter buyer request
            var startDateUtc = startDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(startDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()) : null;
            var endDateUtc = endDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(endDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1) : null;

            var industry = await _industryService.GetIndustryByIdAsync(industryId);
            if (industry == null)
                return Ok(new { success = false, message = "Industry not found" });

            List<object> list = new List<object>();

            try
            {
                var requestForQuotations = (await _requestService.GetAllRequestForQuotationAsync(rfqsIds: new List<int> { (int)RequestForQuotationStatus.Verified },
                    industryId: industryId, categoryId: categoryId, productId: productId, startDateUtc: startDateUtc, endDateUtc: endDateUtc)).ToList();

                foreach (var requestForQuotation in requestForQuotations)
                {
                    var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
                    if (request is null)
                        continue;

                    var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
                    if (category is null)
                        continue;

                    var product = await _productService.GetProductByIdAsync(request.ProductId);
                    if (product is null)
                        continue;

                    var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);
                    if (buyer is null)
                        continue;

                    list.Add(new
                    {
                        Id = requestForQuotation.Id,
                        CustomNumber = requestForQuotation.CustomRfqNumber,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        IndustryId = industry.Id,
                        IndustryName = industry.Name,
                        BuyerId = buyer.Id,
                        BuyerName = buyer.FullName,
                        BrandName = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(request.BrandId)), x => x.Name) : "-",
                        BrandId = request.BrandId,
                        Quantity = requestForQuotation.Quantity,
                        DeliveryAddress = request.DeliveryAddress,
                        DeliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(request.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM, yyyy"),
                        TotalQuotations = (await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id, sbIds: new List<int> { (int)QuotationStatus.Pending, (int)QuotationStatus.Verified })).Count,
                        ExpiryDate = request.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(request.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                        StatusId = requestForQuotation.RfqStatusId,
                        Status = await _localizationService.GetLocalizedEnumAsync(requestForQuotation.RequestForQuotationStatus),
                        AttributesInfo = await Booker_ParseAttributeXml(request.ProductAttributeXml)
                    });
                }

                return Ok(new { success = true, message = "", data = list });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("request-for-quotation/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_GetRequestForQuotation(int requestForQuotationId)
        {
            if (requestForQuotationId <= 0)
                return Ok(new { success = false, message = "Request for quotation id is required" });

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return Ok(new { success = false, message = "Request for quotation not found" });

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return Ok(new { success = false, message = "Request not found" });

            var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
                return Ok(new { success = false, message = "Category not found" });

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product is null)
                return Ok(new { success = false, message = "Product not found" });

            try
            {
                var data = new
                {
                    Id = requestForQuotation.Id,
                    buyerRequestId = requestForQuotation.RequestId,
                    customRfqNumber = requestForQuotation.CustomRfqNumber,
                    quantity = requestForQuotation.Quantity,
                    category = category.Name,
                    product = product.Name,
                    productId = product.Id,
                    deliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(request.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM, yyyy"),
                    status = await _localizationService.GetLocalizedEnumAsync(requestForQuotation.RequestForQuotationStatus),
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("quotation-add-multiple/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_AddQuotationMultiple(int requestForQuotationId, [FromBody] List<OrderManagementApiModel.RFQQuotationsModel> model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (requestForQuotationId <= 0)
                    return Ok(new { success = false, message = "Request for quotation id is required" });

                var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
                if (requestForQuotation is null)
                    return Ok(new { success = false, message = "Request for quotation not found" });

                var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                //if (model.Sum(x => x.Quantity) != requestForQuotation.Quantity)
                //    return Ok(new { success = false, message = "Quantity should be same to request for quotation quantity" });

                var product = await _productService.GetProductByIdAsync(request.ProductId);
                if (product == null || product.Deleted || !product.AppPublished)
                    return Ok(new { success = false, message = "Buyer request product not found" });

                var currentUserTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);

                ////Check Duplicate Quotation
                //var quotationValidate = model.GroupBy(x => new { x.SupplierId/*, x.BrandId*/ })
                //                       .Where(g => g.Count() > 1).Select(g => g.Key).ToList();

                //if (quotationValidate.Count > 0)
                //    return Ok(new { success = false, message = "Duplicate quotation is not accepted" });

                foreach (var directOrderCalculation in model)
                {
                    var supplier = await _customerService.GetCustomerByIdAsync(directOrderCalculation.SupplierId);
                    if (supplier == null)
                        return Ok(new { success = false, message = "Supplier not found" });

                    if (directOrderCalculation.Quantity <= 0)
                        return Ok(new { success = false, message = "Quantity should be greater than 0" });

                    //if (directOrderCalculation.BrandId <= 0)
                    //    return Ok(new { success = false, message = "Brand is required" });

                    if (directOrderCalculation.BusinessModelId <= 0)
                        return Ok(new { success = false, message = "Business model id is required" });

                    if (directOrderCalculation.PriceValidity <= currentUserTime)
                        return Ok(new { success = false, message = "Invalid price validity" });

                    var newdirectOrderCalculation = new Quotation
                    {
                        RfqId = requestForQuotation.Id,
                        BookerId = user.Id,
                        SupplierId = directOrderCalculation.SupplierId,
                        QuotationStatus = QuotationStatus.Verified,
                        BrandId = request.BrandId,
                        QuotationPrice = directOrderCalculation.Price,
                        Quantity = directOrderCalculation.Quantity,
                        BusinessModelId = directOrderCalculation.BusinessModelId,
                        PriceValidity = directOrderCalculation.PriceValidity,
                        IsApproved = false,
                        Deleted = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        Source = "Tijara"
                    };
                    await _quotationService.InsertQuotationAsync(newdirectOrderCalculation);

                    //generate and set custom Quotation number
                    newdirectOrderCalculation.CustomQuotationNumber = _customNumberFormatter.GenerateQuotationCustomNumber(newdirectOrderCalculation);
                    await _quotationService.UpdateQuotationAsync(newdirectOrderCalculation);

                    //await _customerActivityService.InsertActivityAsync("AddSupplierQuotation",
                    //    await _localizationService.GetResourceAsync("ActivityLog.AddSupplierQuotationFromTijaraApp"), newdirectOrderCalculation);
                }

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Seller.Bid.Created.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-quotations-by-rfq-Id/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_GetAllQuotationsByRFQId(int requestForQuotationId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                if (requestForQuotationId <= 0)
                    return Ok(new { success = false, message = "Request id is required" });

                var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
                if (requestForQuotation is null)
                    return Ok(new { success = false, message = "Request for directOrderCalculation not found" });

                var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                var industry = await _industryService.GetIndustryByIdAsync(request.IndustryId);
                if (industry is null)
                    return Ok(new { success = false, message = "Industry not found" });

                var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
                if (category is null)
                    return Ok(new { success = false, message = "Category not found" });

                var product = await _productService.GetProductByIdAsync(request.ProductId);
                if (product is null)
                    return Ok(new { success = false, message = "Product not found" });

                var data = await (await _quotationService.GetAllQuotationAsync(sbIds: new List<int> { (int)QuotationStatus.Verified }, RfqId: requestForQuotation.Id)).SelectAwait(async qu =>
                {
                    return new
                    {
                        id = qu.Id,
                        customNumber = qu.CustomQuotationNumber,
                        rfqId = qu.RfqId,
                        status = await _localizationService.GetLocalizedEnumAsync(qu.QuotationStatus),
                        expiryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(qu.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy"),
                        industry = industry.Name,
                        category = category.Name,
                        product = product.Name,
                        formatPrice = await _priceFormatter.FormatPriceAsync(qu.QuotationPrice, true, false),
                        price = qu.QuotationPrice,
                        totalFormatPrice = await _priceFormatter.FormatPriceAsync(qu.QuotationPrice * qu.Quantity, true, false),
                        brandId = qu.BrandId,
                        brand = (await _brandService.GetManufacturerByIdAsync(qu.BrandId))?.Name,
                        supplierId = qu.SupplierId,
                        supplierName = (await _customerService.GetCustomerByIdAsync(qu.SupplierId))?.FullName,
                        quantity = qu.Quantity,
                        businessModelId = qu.BusinessModelId,
                        businessModelName = await _localizationService.GetLocalizedEnumAsync(qu.BusinessModelEnum),
                        //priceValidity = (await _dateTimeHelper.ConvertToUserTimeAsync(qu.PriceValidity, DateTimeKind.Utc)).ToString()
                    };
                }).ToListAsync();
                return Ok(new { success = true, data = data });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("request-for-quotation-reject")]
        public async Task<IActionResult> OrderManagement_RejectRequestForQuotation([FromBody] OrderManagementApiModel.RejectModel model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                if (model.Id <= 0)
                    return Ok(new { success = false, message = "Request for quotation id is required" });

                var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(model.Id);
                if (requestForQuotation is null)
                    return Ok(new { success = false, message = "Request for quotation not found" });

                if (string.IsNullOrWhiteSpace(model.RejectedReason))
                    return Ok(new { success = false, message = "Rejected reason is required" });

                if (model.RejectedReason == "Other" && string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                    return Ok(new { success = false, message = "Rejected other reason is required" });

                requestForQuotation.RequestForQuotationStatus = RequestForQuotationStatus.Cancelled;

                if (model.RejectedReason == "Other" && !string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                    requestForQuotation.RejectedReason = $"Other - {model.RejectedOtherReason}";
                else
                    requestForQuotation.RejectedReason = model.RejectedReason;

                await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

                var quotations = await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id);
                foreach (var quotation in quotations)
                {
                    quotation.QuotationStatus = QuotationStatus.Cancelled;
                    await _quotationService.UpdateQuotationAsync(quotation);

                    //send notification to supply user
                    //SendPushNotifications(
                    //   _pushNotificationService, _configuration, _logger,
                    //   customerId: quotation.BookerId,
                    //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.QuotationOnRejectedRequest.Title"),
                    //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.QuotationOnRejectedRequest.Body"), quotation.CustomQuotationNumber),
                    //   entityId: quotation.Id, entityName: "SellerBid",
                    //   data: new Dictionary<string, string>()
                    //   {
                    //        { "entityId", quotation.Id.ToString() },
                    //        { "entityName", "Quotation" }
                    //   });
                }

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("RequestForQuotation.Rejected.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("get-supplier-contract/{orderId}/{supplierId}")]
        public async Task<IActionResult> OrderManagement_GetSupplierContract(int orderId, int supplierId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var supplier = await _customerService.GetCustomerByIdAsync(supplierId);
            if (!await _customerService.IsRegisteredAsync(supplier) && !await _customerService.IsSupplierAsync(supplier))
                return Ok(new { success = false, message = "supplier not found" });

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted || order.OrderStatus == OrderStatus.Cancelled)
                return Ok(new { success = false, message = "Order not found." });

            var supplierContract = (await _orderService.GetAllContractAsync(orderId: orderId, supplierId: supplierId)).FirstOrDefault();
            if (supplierContract is null)
                return Ok(new { success = false, message = "Contract not found." });

            return Ok(new
            {
                success = true,
                message = "",
                SuuplierContract = new
                {
                    contractId = supplierContract.Id,
                    downloadUrl = _storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}",
                    previewUrl = _storeContext.GetCurrentStore().Url + $"files/contracts/{supplierContract.ContractGuid}.pdf",
                    contractSignature = supplierContract.SignaturePictureId > 0 ? true : false
                }
            });
        }

        [HttpGet("get-order-detail-for-pickup-shedules/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_GetOrderDetailForPickupSchedule(int expectedShipmentId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(expectedShipmentId);
                if (expectedShipment is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                var order = await _orderService.GetOrderByIdAsync(expectedShipment.OrderId);
                if (order is null)
                    return Ok(new { success = false, message = "Order not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
                if (orderItem is null)
                    return Ok(new { success = false, message = "Order item not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null || product.Id != buyerRequest.ProductId)
                    return Ok(new { success = false, message = "Buyer request product not found" });

                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                if (customer is null)
                    return Ok(new { success = false, message = "Customer not found" });

                //decimal remaining = 0;
                //var shipments = ((await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Where(x => x.ShippedDateUtc.HasValue)).ToList();
                //foreach (var shipment in shipments)
                //    remaining += (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).Sum(x => x.Quantity);

                var data = new object();
                data = new
                {
                    orderId = order.Id,
                    customOrderNumber = order.CustomOrderNumber,
                    //buyerId = customer.Id,
                    //buyerName = customer.FullName,
                    suplierId = customer.Id,
                    suplierName = customer.FullName,
                    brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                    leftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, expectedShipment.Id),
                    //leftQuantity = orderItem.Quantity - remaining,
                    totalQuantity = buyerRequest.Quantity,
                    expectedShipmentQuantity = expectedShipment.ExpectedQuantity,
                    ProductName = product.Name,
                    ProductAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    countryId = buyerRequest.CountryId,
                    stateId = buyerRequest.CityId,
                    areaId = buyerRequest.AreaId,
                    streetAddress = buyerRequest.DeliveryAddress,
                    contactNo = customer.Phone
                };

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = true, message = ex.Message });
            }
        }

        [HttpPost("add-pickup-request")]
        public async Task<IActionResult> OrderManagement_AddPickupRequest([FromBody] OrderManagementApiModel.OrderDeliveryRequestModel model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (model.ExpectedShipmentId <= 0)
                    return Ok(new { success = false, message = "Expected shipment id is required" });

                if (model.CountryId <= 0)
                    return Ok(new { success = false, message = "Country is required" });

                if (model.CityId <= 0)
                    return Ok(new { success = false, message = "City is required" });

                if (model.AreaId <= 0)
                    return Ok(new { success = false, message = "Area is required" });

                if (model.AgentId <= 0)
                    return Ok(new { success = false, message = "Agent is required" });

                if (model.Quantity <= 0)
                    return Ok(new { success = false, message = "Quantity is required" });

                if (model.BagsDirectlyFromWarehouse && model.WarehouseId == 0)
                    return Ok(new { success = false, message = "Warehouse is required" });

                var deliveryScedule = await _orderService.GetDeliveryScheduleByIdAsync(model.ExpectedShipmentId);
                if (deliveryScedule is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                var order = await _orderService.GetOrderByIdAsync(deliveryScedule.OrderId);
                if (order is null || order.Deleted)
                    return Ok(new { success = false, message = "Order not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
                if (orderItem is null)
                    return Ok(new { success = false, message = "Order item not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                //decimal remaining = 0;
                //var shipments = ((await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Where(x => x.ShippedDateUtc.HasValue)).ToList();
                //foreach (var shipment in shipments)
                //    remaining += (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).Sum(x => x.Quantity);

                decimal totalLeftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, model.ExpectedShipmentId);
                //decimal totalLeftQuantity = deliveryScedule.ExpectedQuantity - remaining;
                if (totalLeftQuantity <= 0)
                    return Ok(new { success = false, message = "There is no left quantity " });

                if (model.Quantity > totalLeftQuantity)
                    return Ok(new { success = false, message = "Left Quantity ", leftQuantity = totalLeftQuantity });

                var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
                if (industry is null)
                    return Ok(new { success = false, message = "Industry not found" });

                var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                if (category is null)
                    return Ok(new { success = false, message = "Category not found" });

                List<object> list = new List<object>();

                var orderDeliveryRequest = new OrderDeliveryRequest
                {
                    OrderId = order.Id,
                    OrderDeliveryScheduleId = deliveryScedule.Id,
                    StatusId = (int)OrderDeliveryRequestEnum.Pending,
                    BagsDirectlyFromSupplier = model.BagsDirectlyFromSupplier,
                    BagsDirectlyFromWarehouse = model.BagsDirectlyFromWarehouse,
                    WarehouseId = model.WarehouseId,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                    AreaId = model.AreaId,
                    StreetAddress = model.StreetAddress,
                    ContactNumber = model.ContactNumber,
                    Quantity = model.Quantity,
                    //ShipmentDateUtc = model.ShipmentDateUtc,
                    Deleted = false,
                    AgentId = model.AgentId,
                    TicketExpiryDate = DateTime.UtcNow.AddDays(category.TicketExpiryDays > 0 ? category.TicketExpiryDays : 1),
                    TicketPirority = category.TicketPirority > 0 ? category.TicketPirority : (int)TicketEnum.Medium
                };
                await _orderService.InsertOrderDeliveryRequestAsync(orderDeliveryRequest);

                //send notification to ops agent user
                //SendPushNotifications(
                //   _pushNotificationService, _configuration, _logger,
                //   customerId: orderDeliveryRequest.AgentId,
                //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceDeliveryRequest.Title"),
                //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceDeliveryRequest.Body"), await _customerService.GetCustomerFullNameAsync(orderDeliveryRequest.AgentId), await _localizationService.GetLocalizedAsync(industry, x => x.Name)),
                //   entityId: orderDeliveryRequest.Id, entityName: "OrderDeliveryRequest",
                //   data: new Dictionary<string, string>()
                //   {
                //        { "entityId", orderDeliveryRequest.Id.ToString() },
                //        { "entityName", "OrderDeliveryRequest" },
                //        { "orderDeliveryRequestId", orderDeliveryRequest.Id.ToString() }
                //   });

                ////send notification to all [OpsHead] users
                //var allOpsHeadUsers = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.OpsHeadRoleName)).Id })).ToList();
                //foreach (var opsHeadUser in allOpsHeadUsers)
                //{
                //    SendPushNotifications(
                //   _pushNotificationService, _configuration, _logger,
                //   customerId: opsHeadUser.Id,
                //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceDeliveryRequest.Title"),
                //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.PlaceDeliveryRequest.Body"), await _customerService.GetCustomerFullNameAsync(orderDeliveryRequest.AgentId), await _localizationService.GetLocalizedAsync(industry, x => x.Name)),
                //   entityId: orderDeliveryRequest.Id, entityName: "OrderDeliveryRequest",
                //   data: new Dictionary<string, string>()
                //   {
                //        { "entityId", orderDeliveryRequest.Id.ToString() },
                //        { "entityName", "OrderDeliveryRequest" },
                //        { "orderDeliveryRequestId", orderDeliveryRequest.Id.ToString() }
                //   });
                //}

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Order.Delivery.Request.Created.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("pickup-request-detail/{expectedShipmentId}")]
        public async Task<IActionResult> OrderManagement_PickupRequestDeatil(int expectedShipmentId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(expectedShipmentId);
                if (deliveryRequest is null)
                    return Ok(new { success = false, message = "delivery request not found" });

                var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                if (order is null)
                    return Ok(new { success = false, message = "delivery request order not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null || product.Id != buyerRequest.ProductId)
                    return Ok(new { success = false, message = "Buyer request product not found" });

                var deliveryRequestDetail = (new
                {
                    id = deliveryRequest.Id,
                    //orderId = deliveryRequest.OrderId,
                    //orderCustomNumber = order.CustomOrderNumber,
                    wareHouseId = deliveryRequest.WarehouseId,
                    wareHouseName = deliveryRequest.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(deliveryRequest.WarehouseId)).Name : "",
                    statusId = deliveryRequest.StatusId,
                    status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                    bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                    bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                    countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                    countryId = deliveryRequest.CountryId,
                    cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                    cityId = deliveryRequest.CityId,
                    areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                    areaId = deliveryRequest.AreaId,
                    streetaddress = deliveryRequest.StreetAddress,
                    contactnumber = deliveryRequest.ContactNumber,
                    totalQuantity = buyerRequest.Quantity,
                    deliveredQuantity = deliveryRequest.Quantity,
                    shipmentdate = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("MM/dd/yyyy h:mm:ss tt") /*deliveryRequest.ShipmentDateUtc*/,
                    agentId = deliveryRequest.AgentId,
                    agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                    timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                    date = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                    priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                    requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                    buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                    brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                    rejectedReason = deliveryRequest.RejectedReason,
                    productName = product.Name,
                    productAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    deliveryDate = deliveryRequest.CreatedOnUtc.ToString(),
                });

                return Ok(new { success = true, data = deliveryRequestDetail });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add-purchase-order-shipment-request")]
        public async Task<IActionResult> OrderManagement_AddPurchaseOrderShipmentRequest([FromBody] OrderManagementApiModel.OrderDeliveryRequestModel model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (model.ExpectedShipmentId <= 0)
                    return Ok(new { success = false, message = "Expected shipment id is required" });

                if (model.CountryId <= 0)
                    return Ok(new { success = false, message = "Country is required" });

                if (model.CityId <= 0)
                    return Ok(new { success = false, message = "City is required" });

                if (model.AreaId <= 0)
                    return Ok(new { success = false, message = "Area is required" });

                if (model.AgentId <= 0)
                    return Ok(new { success = false, message = "Agent is required" });

                if (model.Quantity <= 0)
                    return Ok(new { success = false, message = "Quantity is required" });

                if (model.WarehouseId == 0)
                    return Ok(new { success = false, message = "Warehouse is required" });

                var deliveryScedule = await _orderService.GetDeliveryScheduleByIdAsync(model.ExpectedShipmentId);
                if (deliveryScedule is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                var order = await _orderService.GetOrderByIdAsync(deliveryScedule.OrderId);
                if (order is null || order.Deleted)
                    return Ok(new { success = false, message = "Order not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
                if (orderItem is null)
                    return Ok(new { success = false, message = "Order item not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                //decimal remaining = 0;
                //var shipments = ((await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Where(x => x.ShippedDateUtc.HasValue)).ToList();
                //foreach (var shipment in shipments)
                //    remaining += (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).Sum(x => x.Quantity);

                decimal totalLeftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, deliveryScedule.Id);
                //decimal totalLeftQuantity = deliveryScedule.ExpectedQuantity - remaining;
                if (totalLeftQuantity <= 0)
                    return Ok(new { success = false, message = "There is no left quantity " });

                if (model.Quantity > totalLeftQuantity)
                    return Ok(new { success = false, message = "Left Quantity ", leftQuantity = totalLeftQuantity });

                var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
                if (industry is null)
                    return Ok(new { success = false, message = "Industry not found" });

                var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                if (category is null)
                    return Ok(new { success = false, message = "Category not found" });

                List<object> list = new List<object>();

                var orderDeliveryRequest = new OrderDeliveryRequest
                {
                    OrderId = order.Id,
                    OrderDeliveryScheduleId = deliveryScedule.Id,
                    StatusId = (int)OrderDeliveryRequestEnum.Pending,
                    //BagsDirectlyFromSupplier = model.BagsDirectlyFromSupplier,
                    //BagsDirectlyFromWarehouse = model.BagsDirectlyFromWarehouse,
                    WarehouseId = model.WarehouseId,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                    AreaId = model.AreaId,
                    StreetAddress = model.StreetAddress,
                    ContactNumber = model.ContactNumber,
                    Quantity = model.Quantity,
                    //ShipmentDateUtc = model.ShipmentDateUtc,
                    Deleted = false,
                    AgentId = model.AgentId,
                    TicketExpiryDate = DateTime.UtcNow.AddDays(category.TicketExpiryDays > 0 ? category.TicketExpiryDays : 1),
                    TicketPirority = category.TicketPirority > 0 ? category.TicketPirority : (int)TicketEnum.Medium
                };
                await _orderService.InsertOrderDeliveryRequestAsync(orderDeliveryRequest);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Order.Delivery.Request.Created.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("purchase-order-delivery-request-detail/{expectedShipmentRequestId}")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderDeliveryRequestDeatil(int expectedShipmentRequestId, string type = "ExpectedShipment")
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                if (string.IsNullOrWhiteSpace(type))
                    return Ok(new { success = false, message = "type is required" });

                var data = new object();
                if (type == "ExpectedShipment")
                {
                    var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(expectedShipmentRequestId);
                    if (deliveryRequest is null)
                        return Ok(new { success = false, message = "delivery request not found" });

                    var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                    if (order is null)
                        return Ok(new { success = false, message = "delivery request order not found" });

                    var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                    if (buyerRequest is null)
                        return Ok(new { success = false, message = "Buyer Request not found" });

                    var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                    if (product is null || product.Id != buyerRequest.ProductId)
                        return Ok(new { success = false, message = "Buyer request product not found" });

                    data = new
                    {
                        id = deliveryRequest.Id,
                        orderId = deliveryRequest.OrderId,
                        orderCustomNumber = order.CustomOrderNumber,
                        shipmentId = deliveryRequest.StatusId == (int)OrderDeliveryRequestEnum.Complete ? (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id))?.Id : 0,
                        wareHouseId = deliveryRequest.WarehouseId,
                        wareHouseName = deliveryRequest.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(deliveryRequest.WarehouseId)).Name : "",
                        statusId = deliveryRequest.StatusId,
                        status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                        //bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                        //bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                        countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                        countryId = deliveryRequest.CountryId,
                        cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                        cityId = deliveryRequest.CityId,
                        areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                        areaId = deliveryRequest.AreaId,
                        streetaddress = deliveryRequest.StreetAddress,
                        contactnumber = deliveryRequest.ContactNumber,
                        totalQuantity = buyerRequest.Quantity,
                        deliveredQuantity = deliveryRequest.Quantity,
                        agentId = deliveryRequest.AgentId,
                        agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                        timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                        priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                        requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                        buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                        brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                        rejectedReason = deliveryRequest.RejectedReason,
                        productName = product.Name,
                        productAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    };
                }
                if (type == "Shipment")
                {
                    var shipment = await _shipmentService.GetShipmentByIdAsync(expectedShipmentRequestId);
                    if (shipment is null)
                        return Ok(new { success = false, message = "Shipment not found" });

                    var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                    if (deliveryRequest is null)
                        return Ok(new { success = false, message = "delivery request not found" });

                    var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                    if (order is null)
                        return Ok(new { success = false, message = "delivery request order not found" });

                    var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                    if (buyerRequest is null)
                        return Ok(new { success = false, message = "Buyer Request not found" });

                    var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                    if (product is null || product.Id != buyerRequest.ProductId)
                        return Ok(new { success = false, message = "Buyer request product not found" });

                    var picture = (await _pictureService.GetPictureByIdAsync(shipment.PictureId));
                    data = new
                    {
                        id = deliveryRequest.Id,
                        orderId = deliveryRequest.OrderId,
                        orderCustomNumber = order.CustomOrderNumber,
                        statusId = deliveryRequest.StatusId,
                        status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                        //bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                        //bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                        countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                        countryId = deliveryRequest.CountryId,
                        cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                        cityId = deliveryRequest.CityId,
                        areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                        areaId = deliveryRequest.AreaId,
                        streetaddress = deliveryRequest.StreetAddress,
                        contactnumber = deliveryRequest.ContactNumber,
                        totalQuantity = buyerRequest.Quantity,
                        deliveredQuantity = deliveryRequest.Quantity,
                        shipmentdate = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("MM/dd/yyyy h:mm:ss tt") /*deliveryRequest.ShipmentDateUtc*/,
                        agentId = deliveryRequest.AgentId,
                        agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                        timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                        priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                        requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                        buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                        brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
                        rejectedReason = deliveryRequest.RejectedReason,
                        productName = product.Name,
                        productAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                        canShip = shipment.ShippedDateUtc.HasValue,
                        canDelivered = shipment.DeliveryDateUtc.HasValue,

                        //Shipped Detail
                        shippedDetail = new
                        {
                            expectedDateShipped = shipment.ExpectedDateShipped.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ExpectedDateShipped.Value, DateTimeKind.Utc)).ToString() : "",
                            expectedDateDelivered = shipment.ExpectedDateDelivered.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ExpectedDateDelivered.Value, DateTimeKind.Utc)).ToString() : "",
                            expectedDeliveryCost = shipment.ExpectedDeliveryCost,
                            expectedQuantity = shipment.ExpectedQuantity,
                            actualShippedQuantity = shipment.ActualShippedQuantity,
                            transporterId = shipment.TransporterId,
                            wareHouseId = shipment.WarehouseId,
                            wareHouseName = shipment.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(shipment.WarehouseId)).Name : "",
                            transporterName = shipment.TransporterId > 0 ? (await _customerService.GetCustomerByIdAsync(shipment.TransporterId))?.FullName : "",
                            vehicleId = shipment.VehicleId,
                            transportvehicleName = (await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId)) != null ? $"{(await _customerService.GetVehiclePortfolioByIdAsync((await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId)).VehicleId)).Name} - {(await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId))?.VehicleNumber}" : null,
                            vehicleNumber = shipment.VehicleNumber,
                            routeType = shipment.RouteTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.RouteType) : "",
                            deliveryStatus = await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryStatus),
                            pickupAddress = shipment.PickupAddress,
                            shippedDateUtc = shipment.ShippedDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString() : "",
                            onLabourCharges = shipment.OnLabourCharges,
                            costOnZaraye = shipment.CostOnZarayeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.CostOnZaraye) : ""
                        },

                        //Delivered Detail
                        deliveredDetail = new
                        {
                            deliveryDateUtc = shipment.DeliveryDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString() : "",
                            shipmentDeliveryAddress = shipment.ShipmentDeliveryAddress,
                            picture = shipment.PictureId > 0 ? await _pictureService.GetPictureUrlAsync(shipment.PictureId) : "",
                            fullPictureUrl = picture != null ? (await _pictureService.GetPictureUrlAsync(picture)).Url : "",
                            transporterType = shipment.TransporterTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.TransporterType) : "",
                            deliveryType = shipment.DeliveryTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryType) : "",
                            deliveryTiming = shipment.DeliveryTimingId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryTimingEnum) : "",
                            deliveryDelayedReason = shipment.DeliveryDelayedReasonId > 0 ? (await _shipmentService.GetDeliveryTimeReasonByIdAsync(shipment.DeliveryDelayedReasonId))?.Name : "",
                            deliveryCostType = shipment.DeliveryCostTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryCostType) : "",
                            deliveryCost = shipment.DeliveryCost,
                            warehouseId = shipment.WarehouseId,
                            freightCharges = shipment.FreightCharges,
                            labourCharges = shipment.LabourCharges,
                            actualDeliveredQuantity = shipment.ActualDeliveredQuantity,
                            deliveryCostReason = shipment.DeliveryCostReasonId > 0 ? (await _shipmentService.GetDeliveryCostReasonByIdAsync(shipment.DeliveryCostReasonId))?.Name : "",
                            costOnZaraye = shipment.LabourTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.LabourType) : ""
                        },
                    };
                }

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Direct Purchase Order

        [HttpGet("check-direct-purchase-order/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_CheckDirectPurchaseOrderExist(int requestForQuotationId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "User not found" });

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return Ok(new { success = false, message = "Request for quotation not found" });

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return Ok(new { success = false, message = "Request  not found" });

            try
            {
                var directOrders = await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id);
                if (!directOrders.Any())
                    return Ok(new { success = true, message = "Data not found", data = false });

                return Ok(new { success = true, message = "Data found", data = true });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message,
                    data = false
                });
            }
        }

        [HttpPost("direct-new-purchase-order-process/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_DirecPurchaseOrderProcess(int requestForQuotationId, [FromBody] List<OrderManagementApiModel.RFQQuotationsModel> model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (requestForQuotationId <= 0)
                return Ok(new { success = false, message = "Request for quotation id is required" });

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return Ok(new { success = false, message = "Request for quotation not found" });

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return Ok(new { success = false, message = "Request not found" });

            //if (model.Sum(x => x.Quantity) != requestForQuotation.Quantity)
            //    return Ok(new { success = false, message = "Quantity should be same to request for quotation quantity" });

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product == null || product.Deleted || !product.AppPublished)
                return Ok(new { success = false, message = "Buyer request product not found" });

            ////Check Duplicate Quotation
            //var quotations = model.GroupBy(x => new { x.SupplierId/*, x.BrandId*/ })
            //                       .Where(g => g.Count() > 1)
            //                       .Select(g => g.Key)
            //                       .ToList();

            //if (quotations.Count > 0)
            //    return Ok(new { success = false, message = "Duplicate supplier quotations is not accepted" });

            try
            {
                var directOrderExists = (await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id)).ToList();
                foreach (var directOrderExist in directOrderExists)
                {
                    var directOrderDeliverySchedules = await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrderExist.Id);
                    foreach (var directOrderDeliverySchedule in directOrderDeliverySchedules)
                        await _orderService.DeleteDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                    //Remove direct order supplier info
                    var directOrderCalculationsExists = await _orderService.GetAllDirectOrderCalculationAsync(directOrderExist.Id);
                    foreach (var directOrderCalculationsExist in directOrderCalculationsExists)
                        await _orderService.DeleteDirectOrderCalculationAsync(directOrderCalculationsExist);

                    await _orderService.DeleteDirectOrderAsync(directOrderExist);
                }

                var quotationIds = model.Select(x => x.Id).ToArray();
                var allQuotations = (await _quotationService.GetQuotationByIdsAsync(quotationIds)).ToList();

                //var directOrderCalculations = (await _quotationService.GetAllQuotationAsync(sbIds: new List<int> { (int)QuotationStatus.Verified }, RfqId: requestForQuotation.Id)).ToList();
                foreach (var quotation in allQuotations)
                {
                    //New Direct Order
                    var newDirectOrder = new DirectOrder
                    {
                        BookerId = user.Id,
                        BuyerId = request.BuyerId,
                        RequestId = request.Id,
                        TransactionModelId = request.BusinessModelId,
                        QuotationId = quotation.Id,
                        BrandId = quotation.BrandId,
                        RequestForQuotationId = requestForQuotation.Id,
                        SupplierId = quotation.SupplierId,
                        Quantity = quotation.Quantity,
                        OrderTypeId = (int)OrderType.PurchaseOrder
                    };
                    await _orderService.InsertDirectOrderAsync(newDirectOrder);
                    //Add default direct order delivery schedule 
                    await _orderService.InsertDirectOrderDeliveryScheduleAsync(new DirectOrderDeliverySchedule
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        ExpectedDeliveryDateUtc = null,
                        ExpectedShipmentDateUtc = null,
                        ExpectedQuantity = null,
                        ExpectedDeliveryCost = 0,
                        DirectOrderId = newDirectOrder.Id
                    });
                    //Add default direct order calculation 
                    await _orderService.InsertDirectOrderCalculationAsync(new DirectOrderCalculation
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        DirectOrderId = newDirectOrder.Id,
                        Quantity = quotation.Quantity,
                        Price = quotation.QuotationPrice,
                        BusinessModelId = quotation.BusinessModelId
                    });
                }

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("New.Direct.Order.Created") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("generate-po")]
        public async Task<IActionResult> OrderManagement_GeneratePurchaseDirectOrder(int requestForQuotationId, [FromBody] List<OrderManagementApiModel.RFQQuotationsModel> model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                if (requestForQuotationId <= 0)
                    return Ok(new { success = false, message = "Request for quotation id is required" });

                var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
                if (requestForQuotation is null)
                    return Ok(new { success = false, message = "Request for quotation not found" });

                var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                if (model.Sum(x => x.Quantity) != requestForQuotation.Quantity)
                    return Ok(new { success = false, message = "Quantity should be same to request for quotation quantity" });

                var product = await _productService.GetProductByIdAsync(request.ProductId);
                if (product == null || product.Deleted || !product.AppPublished)
                    return Ok(new { success = false, message = "Buyer request product not found" });

                var currentUserTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);

                ////Check Duplicate Quotation
                //var quotations = model.GroupBy(x => new { x.SupplierId })
                //                       .Where(g => g.Count() > 1)
                //                       .Select(g => g.Key)
                //                       .ToList();

                //if (quotations.Count > 0)
                //    return Ok(new { success = false, message = "Duplicate supplier quotations is not accepted" });

                foreach (var quotation in model)
                {
                    //New Direct Order
                    var newDirectOrder = new DirectOrder
                    {
                        BookerId = user.Id,
                        BuyerId = request.BuyerId,
                        RequestId = request.Id,
                        TransactionModelId = request.BusinessModelId,
                        QuotationId = quotation.Id,
                        RequestForQuotationId = requestForQuotation.Id,
                        SupplierId = quotation.SupplierId,
                        Quantity = quotation.Quantity,
                        OrderTypeId = (int)OrderType.PurchaseOrder
                    };
                    await _orderService.InsertDirectOrderAsync(newDirectOrder);
                    //Add default direct order delivery schedule 
                    await _orderService.InsertDirectOrderDeliveryScheduleAsync(new DirectOrderDeliverySchedule
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        ExpectedDeliveryDateUtc = null,
                        ExpectedShipmentDateUtc = null,
                        ExpectedQuantity = null,
                        ExpectedDeliveryCost = 0,
                        DirectOrderId = newDirectOrder.Id
                    });
                    //Add default direct order calculation 
                    await _orderService.InsertDirectOrderCalculationAsync(new DirectOrderCalculation
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        DirectOrderId = newDirectOrder.Id,
                        Quantity = request.Quantity,
                        Price = quotation.Price
                    });
                }

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Seller.Bid.Created.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-direct-order-by-request-for-quotation/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_GetAllDirectOrderByRequestForQuotationId(int requestForQuotationId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return Ok(new { success = false, message = "Request for quotation not found" });

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return Ok(new { success = false, message = "Request not found" });

            try
            {

                var data = new
                {
                    directOrder = await (await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id)).SelectAwait(async dir =>
                    {
                        return new
                        {
                            id = dir.Id,
                            supplierId = dir.SupplierId,
                            supplier = (await _customerService.GetCustomerByIdAsync(dir.SupplierId))?.FullName,
                            quantity = dir.Quantity,
                            countryId = dir.CountryId,
                            cityId = dir.CityId,
                            areaId = dir.AreaId,
                            streetAddress = dir.StreetAddress,
                            interGeography = dir.InterGeography.HasValue ? dir.InterGeography.Value : false,
                            pinlocation = new
                            {
                                latitude = dir.PinLocation_Latitude,
                                longitude = dir.PinLocation_Longitude,
                                location = dir.PinLocation_Location,
                            },
                            expectedShipments = (await _orderService.GetAllDirectOrderDeliveryScheduleAsync(dir.Id)).Select(async d =>
                            {
                                return new
                                {
                                    id = d.Id,
                                    expectedDeliveryDateUtc = d.ExpectedDeliveryDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedDeliveryDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                                    expectedShipmentDateUtc = d.ExpectedShipmentDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedShipmentDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                                    expectedQuantity = d.ExpectedQuantity.HasValue ? d.ExpectedQuantity.ToString() : "",
                                    expectedDeliveryCost = d.ExpectedDeliveryCost.HasValue ? d.ExpectedDeliveryCost.ToString() : "",
                                    createdOnUtc = d.CreatedOnUtc,
                                    directOrderId = d.DirectOrderId
                                };
                            }).Select(t => t.Result).ToList(),
                        };

                    }).ToListAsync()
                };

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("direct-purchase-order-detail-info/{directOrderId}")]
        public async Task<IActionResult> OrderManagement_DirectPurchaseOrderInfo(int directOrderId, [FromBody] DirectOrderApiModel.DirectOrderInfoModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var directOrder = await _orderService.GetDirectOrderByIdAsync(directOrderId);
            if (directOrder is null)
                return Ok(new { success = false, message = "Direct order not found" });

            try
            {
                if (model.CountryId.HasValue)
                    directOrder.CountryId = model.CountryId.Value;

                if (model.CityId.HasValue)
                    directOrder.CityId = model.CityId.Value;

                if (model.AreaId.HasValue)
                    directOrder.AreaId = model.AreaId.Value;

                if (model.StreetAddress is not null)
                    directOrder.StreetAddress = model.StreetAddress;

                if (model.PinLocation is not null)
                {
                    directOrder.PinLocation_Latitude = model.PinLocation.Latitude;
                    directOrder.PinLocation_Longitude = model.PinLocation.Longitude;
                    directOrder.PinLocation_Location = model.PinLocation.Location;
                }

                if (model.InterGeography.HasValue)
                    directOrder.InterGeography = model.InterGeography.Value;

                await _orderService.UpdateDirectOrderAsync(directOrder);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("DirectOrder.infos.Update") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("purchase-order-business-model-form-Json/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderBusinessModelFormJson(int requestForQuotationId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return Ok(new { success = false, message = "Request for quotation not found" });

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return Ok(new { success = false, message = "Request not found" });

            var directOrders = await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id);

            var businessModels = new List<BusinessModelApiModel>();
            try
            {
                foreach (var directOrder in directOrders)
                {
                    var model = new BusinessModelApiModel();

                    var directOrderSupplierInfo = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                    if (directOrderSupplierInfo is not null)
                        businessModels.Add(await PurchaseOrder_BusinessModelFormCalculatedJson(directOrder));
                }

                return Ok(new { success = true, message = "", data = new { bunsinessModelFields = businessModels, businessModelName = await _localizationService.GetLocalizedEnumAsync(request.BusinessModelEnum) } });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("purchase-order-business-model-form-calculation")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderBusinessModelFormCalculation(BusinessModelApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var directOrderCalculation = await _orderService.GetDirectOrderCalculationByIdAsync(model.OrderCalculationId);
            if (directOrderCalculation is null)
                return Ok(new { success = false, message = "Order calculation not found" });

            var directOrder = await _orderService.GetDirectOrderByQuotationId(model.QuotationId);
            if (directOrder is null)
                return Ok(new { success = false, message = "Direct order not found" });

            try
            {
                #region Standard

                if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Standard)
                {
                    var totalPayble = directOrderCalculation.Price * directOrderCalculation.Quantity;
                    var totalReceivable = directOrderCalculation.Price * directOrderCalculation.Quantity;

                    var includeGstInTotalAmount = model.Payables.Where(x => x.Name == "GSTIncludedInTotalAmount")?.FirstOrDefault()?.Value;

                    #region Payable

                    foreach (var item in model.Payables)
                    {
                        if (item.Name is not null)
                        {
                            if (item.Name == "GrossRate")
                                directOrderCalculation.GrossCommissionRate = totalPayble;

                            else if (item.Name == "CommissionType")
                                directOrderCalculation.GrossCommissionRateType = (string)item.Value;

                            else if (item.Name == "Commission")
                            {
                                //var comissionType = model.Payables.Where(x => x.Name == "CommissionType")?.FirstOrDefault()?.Value;

                                directOrderCalculation.GrossCommissionRate = ConvertToDecimal(item.Value);
                                //directOrderCalculation.Payable_ComissionType_0 = (string)comissionType/*.ToLower() == "true" ? "Percent" : "Value"*/;

                                if (ConvertToDecimal(item.Value) > 0)
                                {
                                    if (directOrderCalculation.GrossCommissionRateType == "%")
                                        directOrderCalculation.GrossCommissionAmount = (decimal)((float)totalPayble * (float)directOrderCalculation.GrossCommissionRate / 100f);
                                    else
                                        directOrderCalculation.GrossCommissionAmount = directOrderCalculation.GrossCommissionRate;

                                    totalPayble -= directOrderCalculation.GrossCommissionRate;
                                    directOrderCalculation.NetAmount = totalPayble;

                                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                    {
                                        totalPayble = await _priceCalculationService.RoundPriceAsync(totalPayble);
                                        directOrderCalculation.NetAmount = totalPayble;
                                    }
                                }
                                else
                                {
                                    directOrderCalculation.GrossCommissionRate = 0;
                                    //directOrderCalculation.Payable_ComissionType_0 = "";
                                    directOrderCalculation.GrossCommissionAmount = 0;
                                    directOrderCalculation.NetAmount = totalPayble;
                                }
                            }

                            //Inclusive of GST
                            else if (item.Name == "GSTIncludedInTotalAmount")
                            {
                                if (((string)item.Value).ToLower() == "yes")
                                {
                                    directOrderCalculation.GSTIncludedInTotalAmount = true;
                                    directOrderCalculation.NetAmountWithoutGST = (decimal)((float)totalPayble / (float)_calculationSettings.GSTPercentageForStandardQuotation/* 1.17f*/);
                                    totalPayble = directOrderCalculation.NetAmountWithoutGST;
                                    totalReceivable = ((decimal)((float)(directOrderCalculation.Price * directOrderCalculation.Quantity) / (float)_calculationSettings.GSTPercentageForStandardQuotation /*1.17f*/));
                                }
                                else
                                {
                                    directOrderCalculation.GSTIncludedInTotalAmount = false;
                                    directOrderCalculation.NetAmountWithoutGST = totalPayble;
                                    totalPayble = directOrderCalculation.NetAmount;
                                    totalReceivable = directOrderCalculation.Price * directOrderCalculation.Quantity;

                                }
                            }
                            else if (item.Name == "SupplierCreditTerms")
                                directOrderCalculation.SupplierCreditTerms = ConvertToDecimal(item.Value);
                            //GST
                            else if (item.Name == "GSTRate")
                            {
                                if (ConvertToDecimal(item.Value) > 0)
                                {
                                    directOrderCalculation.GSTRate = ConvertToDecimal(item.Value);
                                    directOrderCalculation.GSTAmount = (decimal)((float)directOrderCalculation.NetAmountWithoutGST * (float)ConvertToDecimal(item.Value) / 100f);

                                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                        directOrderCalculation.GSTAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.GSTAmount);
                                }
                                else
                                {
                                    directOrderCalculation.GSTRate = 0;
                                    directOrderCalculation.GSTAmount = 0;
                                }
                            }

                            //WHT
                            else if (item.Name == "WHTRate")
                            {
                                if (ConvertToDecimal(item.Value) > 0)
                                {
                                    directOrderCalculation.WHTRate = ConvertToDecimal(item.Value);
                                    directOrderCalculation.WHTAmount = ((decimal)((float)(totalPayble + directOrderCalculation.GSTAmount) * ((float)directOrderCalculation.WHTRate / 100f)));

                                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                        directOrderCalculation.WHTAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.WHTAmount);
                                }
                                else
                                {
                                    directOrderCalculation.WHTRate = 0;
                                    directOrderCalculation.WHTAmount = 0;
                                }
                                totalPayble = totalPayble + directOrderCalculation.GSTAmount - directOrderCalculation.WHTAmount;
                                directOrderCalculation.SubTotal = totalPayble;
                                directOrderCalculation.OrderTotal = directOrderCalculation.SubTotal;
                            }

                        }
                    }

                    #endregion

                }

                #endregion

                #region All other business module

                else if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.OneOnOne || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Agency || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.FineCounts || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.ForwardBuying || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.ForwardSelling)
                {
                    #region Payable

                    var gstAmount_Payable = 0m;
                    var whtAmount_Payable = 0m;
                    var wholesaleTax = 0m;
                    var payableToMill_Payable = 0m;
                    var payableToMillAfterMultiply = 0m;
                    var paymentInCash_Payable = 0m;
                    var paymentInCash_PayableAfterMultiply = 0m;

                    foreach (var item in model.Payables)
                    {
                        if (item.Name is not null)
                        {
                            var InvoicedAmount_Payable = ConvertToDecimal(model.Payables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value);

                            //GST
                            if (item.Name == "GSTAmount")
                            {
                                var gstRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);

                                if (((decimal)gstRate) > 0)
                                {
                                    var gstRateCalculationValue = ((decimal)((float)InvoicedAmount_Payable * (float)gstRate / 100));
                                    gstAmount_Payable = gstRateCalculationValue;

                                    directOrderCalculation.GSTIncludedInTotalAmount = true;
                                    directOrderCalculation.GSTRate = ConvertToDecimal((decimal)gstRate);
                                    directOrderCalculation.GSTAmount = gstAmount_Payable;
                                }
                                else
                                {
                                    gstAmount_Payable = 0m;
                                    directOrderCalculation.GSTIncludedInTotalAmount = false;
                                    directOrderCalculation.GSTRate = 0;
                                    directOrderCalculation.GSTAmount = gstAmount_Payable;
                                }
                            }

                            //WHT
                            else if (item.Name == "WHTAmount")
                            {
                                var whtRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                                if (((decimal)whtRate) > 0m)
                                {
                                    var whtRateClaculationValue = ((decimal)(((float)InvoicedAmount_Payable + (float)gstAmount_Payable) * (float)whtRate / 100));
                                    whtAmount_Payable = whtRateClaculationValue;

                                    directOrderCalculation.WhtIncluded = true;
                                    directOrderCalculation.WHTRate = ConvertToDecimal((decimal)whtRate);//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                                    directOrderCalculation.WHTAmount = whtAmount_Payable;
                                }
                                else
                                {
                                    whtAmount_Payable = 0m;
                                    directOrderCalculation.WhtIncluded = false;
                                    directOrderCalculation.WHTRate = 0;//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                                    directOrderCalculation.WHTAmount = whtAmount_Payable;
                                }
                            }

                            //Wholesale Tax
                            else if (item.Name == "WholesaleTaxAmount")
                            {
                                var wholesaletaxRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "WholesaleTaxRate")?.FirstOrDefault().Value);
                                if (((decimal)wholesaletaxRate) > 0)
                                {
                                    //var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));
                                    var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * (float)wholesaletaxRate / 100));
                                    wholesaleTax = wholesaletaxCalculationValue;

                                    directOrderCalculation.WholesaletaxIncluded = true;
                                    directOrderCalculation.WholesaleTaxRate = ConvertToDecimal(wholesaletaxRate);
                                    directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                                }
                                else
                                {
                                    wholesaleTax = 0m;
                                    directOrderCalculation.WholesaletaxIncluded = false;
                                    directOrderCalculation.WholesaleTaxRate = 0;
                                    directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                                }
                            }

                            else if (item.Name == "Price")
                            {
                                directOrderCalculation.Price = ConvertToDecimal(item.Value);
                                //directOrderCalculation.Receivable_ProductPrice_10 = ConvertToDecimal(item.Value);
                            }

                            else if (item.Name == "InvoicedAmount")
                                directOrderCalculation.InvoicedAmount = InvoicedAmount_Payable;

                            else if (item.Name == "BrokerCash")
                                directOrderCalculation.BrokerCash = directOrderCalculation.Price - directOrderCalculation.InvoicedAmount;

                            else if (item.Name == "BrokerId")
                                directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                            else if (item.Name == "PayableToMill")
                            {
                                payableToMill_Payable = ((decimal)((InvoicedAmount_Payable + gstAmount_Payable) - (whtAmount_Payable) + (wholesaleTax)));
                                directOrderCalculation.PayableToMill = payableToMill_Payable;

                                payableToMillAfterMultiply = directOrderCalculation.PayableToMill * directOrderCalculation.Quantity;
                                directOrderCalculation.PayableToMill = payableToMillAfterMultiply;
                            }
                            else if (item.Name == "PaymentInCash")
                            {
                                paymentInCash_Payable = directOrderCalculation.BrokerCash;
                                directOrderCalculation.PaymentInCash = paymentInCash_Payable;

                                paymentInCash_PayableAfterMultiply = directOrderCalculation.PaymentInCash * directOrderCalculation.Quantity;
                                directOrderCalculation.PaymentInCash = paymentInCash_PayableAfterMultiply;
                            }

                            else if (item.Name == "OrderTotal")
                            {
                                //var totalPayable = directOrderCalculation.PayableToMill + directOrderCalculation.PaymentInCash; //(model.ProductPrice_Payable + gstAmount_Payable) - (whtAmount_Payable) + wholesaleTax;
                                directOrderCalculation.TotalPerBag = payableToMill_Payable + paymentInCash_Payable;
                                directOrderCalculation.OrderTotal = payableToMillAfterMultiply + paymentInCash_PayableAfterMultiply;
                                directOrderCalculation.GrossCommissionAmount = directOrderCalculation.OrderTotal;
                                directOrderCalculation.SubTotal = directOrderCalculation.OrderTotal;
                            }
                            else if (item.Name == "SupplierCreditTerms")
                                directOrderCalculation.SupplierCreditTerms = ConvertToDecimal(item.Value);
                            else if (item.Name == "SupplierCommissionBag")
                            {
                                directOrderCalculation.SupplierCommissionBag = ConvertToDecimal(item.Value);
                                directOrderCalculation.SupplierCommissionBag_Summary = ConvertToDecimal(item.Value) * directOrderCalculation.Quantity;
                            }

                            else if (item.Name == "SupplierCommissionPayableUserId")
                                directOrderCalculation.SupplierCommissionPayableUserId = Convert.ToInt32(item.Value);

                            else if (item.Name == "SupplierCommissionReceivableRate")
                            {
                                directOrderCalculation.SupplierCommissionReceivableRate = ConvertToDecimal(item.Value);
                                //var supplierCommissionReceivableRate_Payable = (float)directOrderCalculation.SupplierCommissionReceivableRate / 100f;
                                //directOrderCalculation.SupplierCommissionReceivable_Summary = (decimal)((float)directOrderCalculation.SupplierCommissionReceivableRate * (float)directOrderCalculation.InvoicedAmount) * directOrderCalculation.Quantity;
                                directOrderCalculation.SupplierCommissionReceivable_Summary = directOrderCalculation.SupplierCommissionReceivableRate * directOrderCalculation.Quantity;
                            }

                            else if (item.Name == "SupplierCommissionReceivableUserId")
                                directOrderCalculation.SupplierCommissionReceivableUserId = Convert.ToInt32(item.Value);

                        }
                    }

                    #endregion
                }

                #endregion

                #region Lending

                else if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Lending)
                {
                    #region Payable

                    var gstAmount_Payable = 0m;
                    var whtAmount_Payable = 0m;
                    var wholesaleTax = 0m;
                    var payableToMillAfterMultiply = 0m;
                    var paymentInCash_Payable = 0m;
                    var paymentInCash_PayableAfterMultiply = 0m;

                    foreach (var item in model.Payables)
                    {
                        var InvoicedAmount_Payable = ConvertToDecimal(model.Payables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value);

                        if (item.Name is not null)
                        {
                            var totalFinanceCost_20 = ConvertToDecimal(model.Payables.Where(x => x.Name == "TotalFinanceCost")?.FirstOrDefault().Value);
                            var financeCostPaymentId = Convert.ToInt32(model.Payables.Where(x => x.Name == "FinanceCostPayment").FirstOrDefault().Value);

                            //GST rate
                            if (item.Name == "GSTAmount")
                            {
                                var gstRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);

                                if (((decimal)gstRate) > 0)
                                {
                                    var gstRateCalculationValue = ((decimal)((float)InvoicedAmount_Payable * (float)gstRate / 100));

                                    if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                        gstRateCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + ConvertToDecimal(totalFinanceCost_20)) * (float)gstRate / 100));

                                    gstAmount_Payable = gstRateCalculationValue;

                                    directOrderCalculation.GSTIncludedInTotalAmount = true;
                                    directOrderCalculation.GSTRate = ConvertToDecimal((decimal)gstRate);
                                    directOrderCalculation.GSTAmount = gstAmount_Payable;
                                }
                                else
                                {
                                    gstAmount_Payable = 0m;
                                    directOrderCalculation.GSTIncludedInTotalAmount = false;
                                    directOrderCalculation.GSTRate = 0;
                                    directOrderCalculation.GSTAmount = gstAmount_Payable;
                                }
                            }

                            ////GST
                            //if (item.Name == "GSTAmount")
                            //{
                            //    var gstInclusive = model.Payables.Where(x => x.Name == "GSTIncludedInTotalAmount").FirstOrDefault().Value;

                            //    if (((string)gstInclusive).ToLower() == "yes")
                            //    {
                            //        var gstRateCalculationValue = ((decimal)((float)InvoicedAmount_Payable * (float)_calculationSettings.GSTPercentageForQuotation));

                            //        if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                            //            gstRateCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + ConvertToDecimal(totalFinanceCost_20)) * (float)_calculationSettings.GSTPercentageForQuotation));

                            //        gstAmount_Payable = gstRateCalculationValue;

                            //        directOrderCalculation.GSTIncludedInTotalAmount = true;
                            //        directOrderCalculation.GSTRate = _calculationSettings.GSTPercentageForQuotation * 100;
                            //        directOrderCalculation.GSTAmount = gstAmount_Payable;
                            //    }
                            //    else
                            //    {
                            //        gstAmount_Payable = 0m;
                            //        directOrderCalculation.GSTIncludedInTotalAmount = false;
                            //        directOrderCalculation.GSTRate = 0m;
                            //        directOrderCalculation.GSTAmount = gstAmount_Payable;
                            //    }
                            //}


                            //WHT rate
                            else if (item.Name == "WHTAmount")
                            {
                                var whtRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                                if (((decimal)whtRate) > 0)
                                {
                                    var whtRateClaculationValue = ((decimal)(((float)InvoicedAmount_Payable + (float)gstAmount_Payable) * (float)whtRate / 100));

                                    if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                        whtRateClaculationValue = ((decimal)(((float)(InvoicedAmount_Payable + totalFinanceCost_20) + (float)gstAmount_Payable) * (float)whtRate / 100));

                                    whtAmount_Payable = whtRateClaculationValue;

                                    directOrderCalculation.WhtIncluded = true;
                                    directOrderCalculation.WHTRate = ConvertToDecimal((decimal)whtRate);//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                                    directOrderCalculation.WHTAmount = whtAmount_Payable;
                                }
                                else
                                {
                                    whtAmount_Payable = 0m;
                                    directOrderCalculation.WhtIncluded = false;
                                    directOrderCalculation.WHTRate = 0;//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                                    directOrderCalculation.WHTAmount = whtAmount_Payable;
                                }
                            }

                            //WHT
                            //else if (item.Name == "WHTAmount")
                            //{
                            //    var whtInclusive = model.Payables.Where(x => x.Name == "WhtIncluded")?.FirstOrDefault()?.Value;
                            //    if (((string)whtInclusive).ToLower() == "yes")
                            //    {
                            //        var whtRateClaculationValue = ((decimal)(((float)InvoicedAmount_Payable + (float)gstAmount_Payable) * (float)_calculationSettings.WHTPercentageForQuotation));

                            //        if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                            //            whtRateClaculationValue = ((decimal)(((float)(InvoicedAmount_Payable + totalFinanceCost_20) + (float)gstAmount_Payable) * (float)_calculationSettings.WHTPercentageForQuotation));

                            //        whtAmount_Payable = whtRateClaculationValue;

                            //        directOrderCalculation.WhtIncluded = true;
                            //        directOrderCalculation.WHTRate = _calculationSettings.WHTPercentageForQuotation * 100;
                            //        directOrderCalculation.WHTAmount = whtAmount_Payable;
                            //    }
                            //    else
                            //    {
                            //        whtAmount_Payable = 0m;
                            //        directOrderCalculation.WhtIncluded = false;
                            //        directOrderCalculation.WHTRate = 0m;
                            //        directOrderCalculation.WHTAmount = whtAmount_Payable;
                            //    }
                            //}

                            //Wholesale Tax rate
                            else if (item.Name == "WholesaleTaxAmount")
                            {
                                var wholesaletaxRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "WholesaleTaxRate")?.FirstOrDefault().Value);
                                if (((decimal)wholesaletaxRate) > 0)
                                {
                                    //var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));
                                    var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * (float)wholesaletaxRate / 100));

                                    if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                        wholesaletaxCalculationValue = ((decimal)((float)((InvoicedAmount_Payable + totalFinanceCost_20) + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * (float)wholesaletaxRate / 100));

                                    wholesaleTax = wholesaletaxCalculationValue;

                                    directOrderCalculation.WholesaletaxIncluded = true;
                                    directOrderCalculation.WholesaleTaxRate = ConvertToDecimal(wholesaletaxRate);
                                    directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                                }
                                else
                                {
                                    wholesaleTax = 0m;
                                    directOrderCalculation.WholesaletaxIncluded = false;
                                    directOrderCalculation.WholesaleTaxRate = 0;
                                    directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                                }
                            }

                            ////Wholesale Tax
                            //else if (item.Name == "WholesaleTaxAmount")
                            //{
                            //    var wholesaleTaxInclusive = model.Payables.Where(x => x.Name == "WholesaletaxIncluded").FirstOrDefault().Value;
                            //    if (((string)wholesaleTaxInclusive).ToLower() == "yes")
                            //    {
                            //        var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));

                            //        if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                            //            wholesaletaxCalculationValue = ((decimal)((float)((InvoicedAmount_Payable + totalFinanceCost_20) + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));

                            //        wholesaleTax = wholesaletaxCalculationValue;

                            //        directOrderCalculation.WholesaletaxIncluded = true;
                            //        directOrderCalculation.WholesaleTaxRate = 0.1m;
                            //        directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                            //    }
                            //    else
                            //    {
                            //        wholesaleTax = 0m;
                            //        directOrderCalculation.WholesaletaxIncluded = false;
                            //        directOrderCalculation.WholesaleTaxRate = 0m;
                            //        directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                            //    }
                            //}

                            else if (item.Name == "Price")
                            {
                                directOrderCalculation.Price = ConvertToDecimal(item.Value);
                                //directOrderCalculation.Receivable_ProductPrice_20 = ConvertToDecimal(item.Value);
                            }

                            else if (item.Name == "InvoicedAmount")
                                directOrderCalculation.InvoicedAmount = ConvertToDecimal(item.Value);

                            else if (item.Name == "BrokerCash")
                                directOrderCalculation.BrokerCash = directOrderCalculation.Price - directOrderCalculation.InvoicedAmount;

                            //else if (item.Name == "BuyerPaymentTerms")
                            //    directOrderCalculation.BuyerPaymentTerms = ConvertToDecimal(item.Value);

                            else if (item.Name == "TotalFinanceCost")
                                directOrderCalculation.TotalFinanceCost = ConvertToDecimal(item.Value);
                            else if (item.Name == "SupplierCreditTerms")
                                directOrderCalculation.SupplierCreditTerms = ConvertToDecimal(item.Value);

                            else if (item.Name == "FinanceCostPayment")
                                directOrderCalculation.FinanceCostPayment = Convert.ToInt32(item.Value);

                            //else if (item.Name == "SellingPrice_FinanceIncome")
                            //    directOrderCalculation.SellingPrice_FinanceIncome = ConvertToDecimal(item.Value);

                            else if (item.Name == "FinanceCost")
                                directOrderCalculation.FinanceCost = directOrderCalculation.TotalFinanceCost;
                            //directOrderCalculation.FinanceCost = directOrderCalculation.SupplierCreditTerms > 0 && directOrderCalculation.TotalFinanceCost > 0 ? (directOrderCalculation.TotalFinanceCost / directOrderCalculation.SupplierCreditTerms) /** directOrderCalculation.BuyerPaymentTerms*/ : 0;

                            //else if (item.Name == "InterestAccrued")
                            //{
                            //    directOrderCalculation.InterestAccrued = directOrderCalculation.TotalFinanceCost - directOrderCalculation.FinanceCost;
                            //    directOrderCalculation.InterestAccrued_Summary = directOrderCalculation.InterestAccrued * directOrderCalculation.Quantity;
                            //}

                            //else if (item.Name == "InterestAccrued_Summary")
                            //{
                            //    var interestAccrued_Payable = directOrderCalculation.TotalFinanceCost - directOrderCalculation.FinanceCost;
                            //    directOrderCalculation.InterestAccrued_Summary = interestAccrued_Payable * directOrderCalculation.Quantity;
                            //}

                            else if (item.Name == "BrokerId")
                                directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                            else if (item.Name == "PayableToMill")
                            {
                                var payableToMill_Payable = ((InvoicedAmount_Payable + gstAmount_Payable) - (whtAmount_Payable) + wholesaleTax);

                                if (directOrderCalculation.FinanceCostPayment == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                {
                                    payableToMill_Payable = ((decimal)(InvoicedAmount_Payable + directOrderCalculation.TotalFinanceCost + gstAmount_Payable) - (whtAmount_Payable) + (wholesaleTax));
                                    //payableToMill_Payable = ((decimal)(InvoicedAmount_Payable + directOrderCalculation.TotalFinanceCost));
                                }

                                payableToMillAfterMultiply = payableToMill_Payable * directOrderCalculation.Quantity;
                                directOrderCalculation.PayableToMill = payableToMillAfterMultiply;
                            }

                            else if (item.Name == "PaymentInCash")
                            {
                                if (directOrderCalculation.FinanceCostPayment == (int)FinanceCostPaymentStatus.Cash)
                                    paymentInCash_Payable = directOrderCalculation.BrokerCash + directOrderCalculation.TotalFinanceCost;
                                else
                                    paymentInCash_Payable = directOrderCalculation.BrokerCash;

                                directOrderCalculation.PaymentInCash = paymentInCash_Payable;
                                paymentInCash_PayableAfterMultiply = directOrderCalculation.PaymentInCash * directOrderCalculation.Quantity;
                                directOrderCalculation.PaymentInCash = paymentInCash_PayableAfterMultiply;
                            }

                            else if (item.Name == "OrderTotal")
                            {
                                //var totalPayable = directOrderCalculation.Payable_PaymentInCash_20 + directOrderCalculation.Payable_PaymentInCash_20; //(model.ProductPrice_Payable + gstAmount_Payable) - (whtAmount_Payable) + wholesaleTax;
                                directOrderCalculation.TotalPerBag = directOrderCalculation.PayableToMill + paymentInCash_Payable;
                                directOrderCalculation.OrderTotal = payableToMillAfterMultiply + paymentInCash_PayableAfterMultiply;

                                directOrderCalculation.GrossCommissionAmount = directOrderCalculation.OrderTotal;
                                directOrderCalculation.SubTotal = directOrderCalculation.OrderTotal;
                            }

                            else if (item.Name == "SupplierCommissionBag")
                            {
                                directOrderCalculation.SupplierCommissionBag = ConvertToDecimal(item.Value);
                                directOrderCalculation.SupplierCommissionBag_Summary = ConvertToDecimal(item.Value) * directOrderCalculation.Quantity;
                            }

                            else if (item.Name == "SupplierCommissionReceivableRate")
                            {
                                directOrderCalculation.SupplierCommissionReceivableRate = ConvertToDecimal(item.Value);
                                //var supplierCommissionReceivableRate_Payable = directOrderCalculation.SupplierCommissionReceivableRate / 100;
                                //directOrderCalculation.SupplierCommissionReceivable_Summary = (decimal)((float)directOrderCalculation.SupplierCommissionReceivableRate * (float)InvoicedAmount_Payable) * directOrderCalculation.Quantity;
                                directOrderCalculation.SupplierCommissionReceivable_Summary = directOrderCalculation.SupplierCommissionReceivableRate * directOrderCalculation.Quantity;
                            }

                            else if (item.Name == "SupplierCommissionPayableUserId")
                                directOrderCalculation.SupplierCommissionPayableUserId = Convert.ToInt32(item.Value);

                            else if (item.Name == "SupplierCommissionReceivableUserId")
                                directOrderCalculation.SupplierCommissionReceivableUserId = Convert.ToInt32(item.Value);
                        }
                    }

                    #endregion
                }

                #endregion

                //#region Broker

                //if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Broker)
                //{
                //    #region Payable

                //    var gstAmount_Payable = 0m;
                //    var whtAmount_Payable = 0m;
                //    var wholesaleTax = 0m;
                //    var payableToMill_Payable = 0m;
                //    var payableToMillAfterMultiply = 0m;
                //    var paymentInCash_Payable = 0m;
                //    var paymentInCash_PayableAfterMultiply = 0m;

                //    foreach (var item in model.Payables)
                //    {
                //        if (item.Name is not null)
                //        {
                //            var InvoicedAmount_Payable = ConvertToDecimal(model.Payables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value);

                //            //GST rate
                //            if (item.Name == "GSTAmount")
                //            {
                //                var gstRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);

                //                if (((decimal)gstRate) > 0)
                //                {
                //                    var gstRateCalculationValue = ((decimal)((float)InvoicedAmount_Payable * (float)gstRate / 100));
                //                    gstAmount_Payable = gstRateCalculationValue;

                //                    directOrderCalculation.GSTIncludedInTotalAmount = true;
                //                    directOrderCalculation.GSTRate = ConvertToDecimal((decimal)gstRate);
                //                    directOrderCalculation.GSTAmount = gstAmount_Payable;
                //                }
                //                else
                //                {
                //                    gstAmount_Payable = 0m;
                //                    directOrderCalculation.GSTIncludedInTotalAmount = false;
                //                    directOrderCalculation.GSTRate = 0;
                //                    directOrderCalculation.GSTAmount = gstAmount_Payable;
                //                }
                //            }

                //            ////GST
                //            //if (item.Name == "GSTAmount")
                //            //{
                //            //    var gstInclusive = model.Payables.Where(x => x.Name == "GSTIncludedInTotalAmount").FirstOrDefault().Value;

                //            //    if (((string)gstInclusive).ToLower() == "yes")
                //            //    {
                //            //        var gstRateCalculationValue = ((decimal)((float)InvoicedAmount_Payable * (float)_calculationSettings.GSTPercentageForQuotation));
                //            //        gstAmount_Payable = gstRateCalculationValue;

                //            //        directOrderCalculation.GSTIncludedInTotalAmount = true;
                //            //        directOrderCalculation.GSTRate = _calculationSettings.GSTPercentageForQuotation * 100;
                //            //        directOrderCalculation.GSTAmount = gstAmount_Payable;
                //            //    }
                //            //    else
                //            //    {
                //            //        gstAmount_Payable = 0m;
                //            //        directOrderCalculation.GSTIncludedInTotalAmount = false;
                //            //        directOrderCalculation.GSTRate = 0m;
                //            //        directOrderCalculation.GSTAmount = gstAmount_Payable;
                //            //    }
                //            //}

                //            //WHT
                //            else if (item.Name == "WHTAmount")
                //            {
                //                var whtRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                //                if (((decimal)whtRate) > 0)
                //                {
                //                    var whtRateClaculationValue = ((decimal)(((float)InvoicedAmount_Payable + (float)gstAmount_Payable) * (float)whtRate / 100));
                //                    whtAmount_Payable = whtRateClaculationValue;

                //                    directOrderCalculation.WhtIncluded = true;
                //                    directOrderCalculation.WHTRate = ConvertToDecimal((decimal)whtRate);//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                //                    directOrderCalculation.WHTAmount = whtAmount_Payable;
                //                }
                //                else
                //                {
                //                    whtAmount_Payable = 0m;
                //                    directOrderCalculation.WhtIncluded = false;
                //                    directOrderCalculation.WHTRate = 0;//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                //                    directOrderCalculation.WHTAmount = whtAmount_Payable;
                //                }
                //            }

                //            ////WHT
                //            //else if (item.Name == "WHTAmount")
                //            //{
                //            //    var whtInclusive = model.Payables.Where(x => x.Name == "WhtIncluded")?.FirstOrDefault()?.Value;
                //            //    if (((string)whtInclusive).ToLower() == "yes")
                //            //    {
                //            //        var whtRateClaculationValue = ((decimal)(((float)InvoicedAmount_Payable + (float)gstAmount_Payable) * (float)_calculationSettings.WHTPercentageForQuotation));
                //            //        whtAmount_Payable = whtRateClaculationValue;

                //            //        directOrderCalculation.WhtIncluded = true;
                //            //        directOrderCalculation.WHTRate = _calculationSettings.WHTPercentageForQuotation * 100;
                //            //        directOrderCalculation.WHTAmount = whtAmount_Payable;
                //            //    }
                //            //    else
                //            //    {
                //            //        whtAmount_Payable = 0m;
                //            //        directOrderCalculation.WhtIncluded = false;
                //            //        directOrderCalculation.WHTRate = 0m;
                //            //        directOrderCalculation.WHTAmount = whtAmount_Payable;
                //            //    }
                //            //}

                //            //Wholesale Tax
                //            else if (item.Name == "WholesaleTaxAmount")
                //            {
                //                var wholesaletaxRate = ConvertToDecimal(model.Payables.Where(x => x.Name == "WholesaleTaxRate")?.FirstOrDefault().Value);
                //                if (((decimal)wholesaletaxRate) > 0)
                //                {
                //                    var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * (float)wholesaletaxRate / 100));
                //                    wholesaleTax = wholesaletaxCalculationValue;

                //                    directOrderCalculation.WholesaletaxIncluded = true;
                //                    directOrderCalculation.WholesaleTaxRate = ConvertToDecimal(wholesaletaxRate);
                //                    directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                //                }
                //                else
                //                {
                //                    wholesaleTax = 0m;
                //                    directOrderCalculation.WholesaletaxIncluded = false;
                //                    directOrderCalculation.WholesaleTaxRate = 0;
                //                    directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                //                }
                //            }

                //            ////Wholesale Tax
                //            //else if (item.Name == "WholesaleTaxAmount")
                //            //{
                //            //    var wholesaleTaxInclusive = model.Payables.Where(x => x.Name == "WholesaletaxIncluded").FirstOrDefault().Value;
                //            //    if (((string)wholesaleTaxInclusive).ToLower() == "yes")
                //            //    {
                //            //        var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));
                //            //        wholesaleTax = wholesaletaxCalculationValue;

                //            //        directOrderCalculation.WholesaletaxIncluded = true;
                //            //        directOrderCalculation.WholesaleTaxRate = 0.1m;
                //            //        directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                //            //    }
                //            //    else
                //            //    {
                //            //        wholesaleTax = 0m;
                //            //        directOrderCalculation.WholesaletaxIncluded = false;
                //            //        directOrderCalculation.WholesaleTaxRate = 0m;
                //            //        directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                //            //    }
                //            //}

                //            else if (item.Name == "Price")
                //            {
                //                directOrderCalculation.Price = ConvertToDecimal(item.Value);
                //                //directOrderCalculation.Receivable_ProductPrice_60 = ConvertToDecimal(item.Value);
                //            }
                //            else if (item.Name == "InvoicedAmount")
                //                directOrderCalculation.InvoicedAmount = InvoicedAmount_Payable;

                //            else if (item.Name == "BrokerCash")
                //                directOrderCalculation.BrokerCash = directOrderCalculation.Price - directOrderCalculation.InvoicedAmount;

                //            else if (item.Name == "BrokerId")
                //                directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                //            else if (item.Name == "SupplierCommissionBag")
                //                directOrderCalculation.SupplierCommissionBag = ConvertToDecimal(item.Value);

                //            else if (item.Name == "SupplierCommissionReceivableRate")
                //            {
                //                directOrderCalculation.SupplierCommissionReceivableRate = ConvertToDecimal(item.Value);
                //                var supplierCommissionReceivableRate_Payable = directOrderCalculation.SupplierCommissionReceivableRate / 100;
                //                directOrderCalculation.SupplierCommissionReceivable_Summary = (decimal)((float)supplierCommissionReceivableRate_Payable * (float)InvoicedAmount_Payable) * directOrderCalculation.Quantity;
                //            }
                //            else if (item.Name == "PayableToMill")
                //            {
                //                payableToMill_Payable = ((decimal)((InvoicedAmount_Payable + gstAmount_Payable) - (whtAmount_Payable) + (wholesaleTax)));
                //                directOrderCalculation.PayableToMill = payableToMill_Payable;
                //                payableToMillAfterMultiply = directOrderCalculation.PayableToMill * directOrderCalculation.Quantity;
                //                directOrderCalculation.PayableToMill = payableToMillAfterMultiply;
                //            }

                //            else if (item.Name == "PaymentInCash")
                //            {
                //                paymentInCash_Payable = directOrderCalculation.BrokerCash;
                //                directOrderCalculation.PaymentInCash = paymentInCash_Payable;
                //                paymentInCash_PayableAfterMultiply = directOrderCalculation.PaymentInCash * directOrderCalculation.Quantity;
                //                directOrderCalculation.PaymentInCash = paymentInCash_PayableAfterMultiply;
                //            }

                //            if (item.Name == "SupplierCommissionBag_Summary")
                //                directOrderCalculation.SupplierCommissionBag_Summary = directOrderCalculation.SupplierCommissionBag * directOrderCalculation.Quantity;

                //            else if (item.Name == "SupplierCommissionPayableUserId")
                //                directOrderCalculation.SupplierCommissionPayableUserId = Convert.ToInt32(item.Value);

                //            else if (item.Name == "SupplierCommissionReceivableUserId")
                //                directOrderCalculation.SupplierCommissionReceivableUserId = Convert.ToInt32(item.Value);

                //            else if (item.Name == "OrderTotal")
                //            {
                //                directOrderCalculation.TotalPerBag = payableToMill_Payable + paymentInCash_Payable;
                //                directOrderCalculation.OrderTotal = directOrderCalculation.SupplierCommissionBag_Summary - directOrderCalculation.SupplierCommissionReceivable_Summary;
                //                directOrderCalculation.GrossCommissionAmount = directOrderCalculation.OrderTotal;
                //                directOrderCalculation.SubTotal = directOrderCalculation.OrderTotal;
                //                directOrderCalculation.GrossAmount = payableToMillAfterMultiply + paymentInCash_PayableAfterMultiply;
                //            }
                //        }
                //    }

                //    #endregion
                //}

                //#endregion

                await _orderService.UpdateDirectOrderCalculationAsync(directOrderCalculation);

                return Ok(new { success = true, message = "", data = await PurchaseOrder_BusinessModelFormCalculatedJson(directOrder) });
            }

            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("direct-purchase-order-placed/{requestForQuotationId}")]
        public async Task<IActionResult> OrderManagement_DirectPurchaseOrderPlaced(int requestForQuotationId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
                if (requestForQuotation is null)
                    return Ok(new { success = false, message = "Request for directOrderCalculation not found" });

                var directOrders = (await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id)).ToList();

                foreach (var directOrder in directOrders)
                {
                    var supplier = await _customerService.GetCustomerByIdAsync(directOrder.SupplierId);
                    if (supplier is null)
                        return Ok(new { success = false, message = "Supplier not found" });

                    var request = await _requestService.GetRequestByIdAsync(directOrder.RequestId);
                    if (request is null)
                        return Ok(new { success = false, message = "Request not found" });

                    var directOrderCalculation = await _orderService.GetDirectOrderCalculationByDirectOrderIdAsync(directOrder.Id);
                    if (directOrderCalculation is null)
                        return Ok(new { success = false, message = "Direct order calculation not found" });

                    if (directOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Standard)
                    {
                        if (directOrderCalculation.BrokerCash > 0 && directOrderCalculation.BrokerId == 0)
                            return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.BrokerId.Required") });

                        if (directOrderCalculation.SupplierCommissionPayableUserId > 0 && directOrderCalculation.SupplierCommissionBag == 0)
                            return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.SupplierCommissionPayableUserId.Required") });

                        if (directOrderCalculation.SupplierCommissionReceivableUserId > 0 && directOrderCalculation.SupplierCommissionReceivableRate == 0)
                            return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.SupplierCommissionReceivableUserId.Required") });
                    }

                    var product = await _productService.GetProductByIdAsync(request.ProductId);
                    if (product is null)
                        return Ok(new { success = false, message = "Product not found" });

                    var quotation = await _quotationService.GetQuotationByIdAsync(directOrder.QuotationId);
                    if (quotation is null)
                        return Ok(new { success = false, message = "Quotation not found" });

                    //Clear Cart
                    var cartItems = await _shoppingCartService.GetShoppingCartAsync(supplier, ShoppingCartType.ShoppingCart, storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
                    foreach (var cartItem in cartItems)
                        await _shoppingCartService.DeleteShoppingCartItemAsync(cartItem);

                    var warning = await PreparePurchaseOrderCalculationAsync(quotation, directOrderCalculation);
                    if (warning.Any())
                        return Ok(new { success = false, message = warning });

                    //now let's try adding product to the cart (now including product attribute validation, etc)
                    var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: supplier,
                        product: product,
                        shoppingCartType: ShoppingCartType.ShoppingCart,
                        storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                        attributesXml: request.ProductAttributeXml,
                        quantity: directOrderCalculation.Quantity, overridePrice: directOrderCalculation.Price, brandId: request.BrandId);
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
                    processPaymentRequest.CustomerId = directOrder.SupplierId;
                    processPaymentRequest.RequestId = request.Id;
                    processPaymentRequest.RFQId = directOrder.RequestForQuotationId;
                    processPaymentRequest.QuotationId = directOrder.QuotationId;
                    processPaymentRequest.OrderTypeId = (int)OrderType.PurchaseOrder;
                    processPaymentRequest.OrderTotal = directOrderCalculation.Price * directOrderCalculation.Quantity;
                    processPaymentRequest.SalePrice = directOrderCalculation.Price;
                    processPaymentRequest.Quotation = quotation;
                    processPaymentRequest.Source = "Tijara";

                    var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                    if (placeOrderResult.Success)
                    {
                        //Insert delivery schedule
                        foreach (var schedule in await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id))
                        {
                            await _orderService.InsertDeliveryScheduleAsync(new OrderDeliverySchedule
                            {
                                OrderId = placeOrderResult.PlacedOrder.Id,
                                CreatedById = user.Id,
                                ExpectedDeliveryDateUtc = schedule.ExpectedDeliveryDateUtc.Value,
                                ExpectedShipmentDateUtc = schedule.ExpectedShipmentDateUtc.Value,
                                ExpectedQuantity = schedule.ExpectedQuantity.Value,
                                ExpectedDeliveryCost = schedule.ExpectedDeliveryCost.Value,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                        }

                        await SavePurchaseOrderCalculationAsync(placeOrderResult.PlacedOrder, directOrderCalculation);

                        //Update Quotation Status
                        quotation.QuotationStatus = QuotationStatus.QuotationSelected;
                        quotation.IsApproved = true;
                        await _quotationService.UpdateQuotationAsync(quotation);

                        //Update request
                        request.PinLocation_Latitude = directOrder.PinLocation_Latitude;
                        request.PinLocation_Location = directOrder.PinLocation_Location;
                        request.PinLocation_Longitude = directOrder.PinLocation_Longitude;
                        await _requestService.UpdateRequestAsync(request);

                        //Update Rfq status
                        requestForQuotation.RequestForQuotationStatus = RequestForQuotationStatus.Processing;
                        await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

                        //Remove Temp
                        var directOrderDeliverySchedules = await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id);
                        foreach (var directOrderDeliverySchedule in directOrderDeliverySchedules)
                            await _orderService.DeleteDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                        //Remove direct order supplier info
                        var directOrderCalculationsExists = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                        foreach (var directOrderCalculationsExist in directOrderCalculationsExists)
                            await _orderService.DeleteDirectOrderCalculationAsync(directOrderCalculationsExist);

                        await _orderService.DeleteDirectOrderAsync(directOrder);
                    }
                    else
                        return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Order.Does'nt.Placed.Success") });
                }

                var quotationsUnselected = (await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id)).Where(x => !x.IsApproved).ToList();
                foreach (var unselectQuotation in quotationsUnselected)
                {
                    unselectQuotation.QuotationStatus = QuotationStatus.QuotationUnSelected;
                    await _quotationService.UpdateQuotationAsync(unselectQuotation);
                }

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Request.PurchaseOrder.Genearted.Successfully") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("purchase-order-detail/{orderId}")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderDetail(int orderId)
        {
            if (orderId <= 0)
                return Ok(new { success = false, message = "Buyer order id is required" });

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) /*|| !await _customerService.IsBuyerAsync(buyer)*/)
                return Ok(new { success = false, message = "Buyer not found" });

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                return Ok(new { success = false, message = "Order not found" });

            var quotation = await _quotationService.GetQuotationByIdAsync(order.QuotationId.Value);
            if (quotation is null)
                return Ok(new { success = false, message = "Quotation not found" });

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(order.RFQId.Value);
            if (requestForQuotation is null)
                return Ok(new { success = false, message = "Order request for quotation not found" });

            var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
            if (buyerRequest is null || buyerRequest.Deleted)
                return Ok(new { success = false, message = "Order request not found" });

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

                industry = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";
                quantity = quotation.Quantity;

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is not null)
                {
                    productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
                    qtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty;
                }

                brand = quotation.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(quotation.BrandId)), x => x.Name) : "-";
                deliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM yy");

                var city = (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.CityId));
                if (city != null)
                    deliveryAddress += city.Name + ", ";

                var area = (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.AreaId));
                if (area != null)
                    deliveryAddress += area.Name + ", ";

                if (!string.IsNullOrWhiteSpace(buyerRequest.DeliveryAddress))
                    deliveryAddress += buyerRequest.DeliveryAddress + ", " + buyerRequest.DeliveryAddress2;

                if (order.ShippingStatusId == (int)ShippingStatus.Delivered || order.ShippingStatusId == (int)ShippingStatus.Shipped)
                {
                    var orderShipment = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.PurchaseOrder)).FirstOrDefault();
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
                    var totalNumberOfItemsCanBeAddedToShipment = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
                    if (totalNumberOfItemsCanBeAddedToShipment <= 0)
                        continue;

                    //yes, we have at least one item to create a new shipment
                    shipmentBalanceQuantity += await _priceCalculationService.RoundPriceAsync(totalNumberOfItemsCanBeAddedToShipment);
                }

                var supplierContract = await _orderService.GetContractByOrderIdAsync(order.Id);
                if (supplierContract is not null)
                    await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}");

                var data = new
                {
                    OrderInfo = new
                    {
                        OrderId = order.Id,
                        CustomOrderNumber = order.CustomOrderNumber,
                        OrderStatusId = order.OrderStatusId,
                        OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                        Industry = industry,
                        Product = productName,
                        Brand = brand,
                        TotalQuantity = quantity,
                        QtyType = qtyType,
                        DeliveryDate = deliveryDate,
                        OrderCreatedDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
                        OrderCreatedTime = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                        BuyerDeliveryAddress = deliveryAddress,
                        BookerId = quotation.BookerId,
                        BookerName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.BookerId))?.FullName,
                        BuyerRequestId = buyerRequest.Id,
                        SupplierName = (await _customerService.GetCustomerByIdAsync(customerId: order.CustomerId))?.FullName,
                        SupplierEmail = (await _customerService.GetCustomerByIdAsync(customerId: order.CustomerId))?.Email,
                        ProductAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
                        SupplierContract = new
                        {
                            contractId = supplierContract != null ? supplierContract.Id : 0,
                            downloadUrl = supplierContract != null ? _storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}" : "",
                            previewUrl = supplierContract != null ? _storeContext.GetCurrentStore().Url + $"files/contracts/{supplierContract.ContractGuid}.pdf" : "",
                            contractSignature = supplierContract != null && supplierContract.SignaturePictureId > 0 ? true : false
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
                        NoOfShipmentNumber = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Count(),
                        QuantityLeft = shipmentBalanceQuantity,
                        Shippments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.PurchaseOrder)).Select(s =>
                        {
                            return new
                            {
                                ShippmentId = s.Id,
                                ShipmentStatus = _localizationService.GetLocalizedEnumAsync(s.DeliveryStatus).Result,
                            };
                        }).ToList(),
                        PinLocation = new
                        {
                            Location = buyerRequest.PinLocation_Location,
                            Longitude = buyerRequest.PinLocation_Longitude,
                            Latitude = buyerRequest.PinLocation_Latitude
                        }
                    },
                    expectedShipments = await _orderService.GetAllDeliveryScheduleAsync(orderId: order.Id).Result.SelectAwait(async b =>
                    {
                        return new
                        {
                            expectedDeliveryDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(b.ExpectedDeliveryDateUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                            expectedShipmentDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(b.ExpectedShipmentDateUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                            expectedDeliveryCost = b.ExpectedDeliveryCost,
                            Quantity = b.ExpectedQuantity
                        };
                    }).ToListAsync(),
                    //Requests = await _orderService.SearchOrderDeliveryRequestsAsync(orderId: orderId).Result.SelectAwait(async b =>
                    //{
                    //    return new
                    //    {
                    //        id = b.Id,
                    //        orderId = b.OrderId,
                    //        orderCustomNumber = ((await _orderService.GetOrderByIdAsync(b.OrderId)).CustomOrderNumber,
                    //        statusId = b.StatusId,
                    //        status = await _localizationService.GetLocalizedEnumAsync(b.OrderDeliveryRequestEnum),
                    //        bagsDirectlyFromSupplier = b.BagsDirectlyFromSupplier,
                    //        bagsDirectlyFromWarehouse = b.BagsDirectlyFromWarehouse,
                    //        countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(b.CountryId), x => x.Name),
                    //        countryId = b.CountryId,
                    //        cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(b.CityId), x => x.Name),
                    //        cityId = b.CityId,
                    //        areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(b.AreaId), x => x.Name),
                    //        areaId = b.AreaId,
                    //        streetaddress = b.StreetAddress,
                    //        contactnumber = b.ContactNumber,
                    //        totalQuantity = quantity,
                    //        deliveredQuantity = b.Quantity,
                    //        shipmentdate = b.ShipmentDateUtc,
                    //        agentId = b.AgentId,
                    //        agent = (await _customerService.GetCustomerByIdAsync(b.AgentId))?.FullName,
                    //        timeRemaining = b.TicketExpiryDate.HasValue ? b.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || b.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? b.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + b.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                    //        date = (await _dateTimeHelper.ConvertToUserTimeAsync(b.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                    //        priority = await _localizationService.GetLocalizedEnumAsync(b.TicketEnum),
                    //        requester = (await _customerService.GetCustomerByIdAsync(b.CreatedById))?.FullName,
                    //        buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                    //        brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-"
                    //    };
                    //}).ToListAsync(),
                };
                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("purchase-order-list")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderList(bool showActiveOrders = true, int pageIndex = 0, int pageSize = 10)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "User not found" });

            List<Order> allOrders = new List<Order>();
            List<Orders> activeOrders = new List<Orders>();
            List<Orders> pastOrders = new List<Orders>();
            List<int> orderStatusIds = new List<int>();

            if (showActiveOrders)
                orderStatusIds.AddRange(new List<int> { (int)OrderStatus.Pending, (int)OrderStatus.Processing });
            else
                orderStatusIds.AddRange(new List<int> { (int)OrderStatus.Cancelled, (int)OrderStatus.Complete });

            var allOrdersPageList = await _orderService.SearchOrdersAsync(osIds: orderStatusIds, orderTypeId: (int)OrderType.PurchaseOrder, pageIndex: pageIndex, pageSize: pageSize);

            allOrders = allOrdersPageList.ToList();
            if (allOrders.Any())
            {
                var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);
                //Get Active Request List Filter By Date 
                var activeOrderDates = allOrders.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.OrderStatusId == (int)OrderStatus.Pending || x.OrderStatusId == (int)OrderStatus.Processing).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                foreach (var activeOrderDate in activeOrderDates)
                {
                    var model = new Orders();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == activeOrderDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == activeOrderDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = activeOrderDate.Key;

                    foreach (var order in activeOrderDate)
                    {
                        var paymentDueDate = "";
                        var industry = "";

                        if (order.RequestId > 0)
                        {
                            var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
                            if (buyerRequest is not null)
                            {
                                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";


                                var orderShipments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.PurchaseOrder));
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

                            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(order.RFQId.Value);
                            if (requestForQuotation is null)
                                continue;

                            var quotation = await _quotationService.GetQuotationByIdAsync(order.QuotationId.Value);
                            if (quotation is null)
                                continue;

                            var product = await _productService.GetProductByIdAsync((await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault().ProductId);
                            if (product == null)
                                continue;

                            model.Data.Add(new OrdersData
                            {
                                OrderId = order.Id,
                                OrderCustomNumber = order.CustomOrderNumber,
                                CreatedOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                                OrderStatusId = order.OrderStatusId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                SupplierId = quotation.SupplierId,
                                SupplierName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.SupplierId))?.FullName,
                                BrandId = quotation.BrandId,
                                BrandName = quotation.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(quotation.BrandId)), x => x.Name) : "-",
                                BookerId = quotation.BookerId,
                                BookerName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.BookerId))?.FullName,
                                Quantity = quotation.Quantity,
                                PaymentDueDate = paymentDueDate,
                                Industry = industry,
                            });
                        }

                    }
                    activeOrders.Add(model);
                }

                var pastOrderDates = allOrders.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.OrderStatusId == (int)OrderStatus.Cancelled).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                foreach (var pastOrderDate in pastOrderDates)
                {
                    var model = new Orders();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == pastOrderDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == pastOrderDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = pastOrderDate.Key;

                    foreach (var order in pastOrderDate)
                    {
                        var paymentDueDate = "";
                        var industry = "";

                        if (order.RequestId > 0)
                        {
                            var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
                            if (buyerRequest is not null)
                            {
                                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";


                                var orderShipments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.PurchaseOrder));
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

                            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(order.RFQId.Value);
                            if (requestForQuotation is null)
                                continue;

                            var quotation = await _quotationService.GetQuotationByIdAsync(order.QuotationId.Value);
                            if (quotation is null)
                                continue;

                            var product = await _productService.GetProductByIdAsync((await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault().ProductId);
                            if (product == null)
                                continue;

                            model.Data.Add(new OrdersData
                            {
                                OrderId = order.Id,
                                OrderCustomNumber = order.CustomOrderNumber,
                                CreatedOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                                OrderStatusId = order.OrderStatusId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                SupplierId = quotation.SupplierId,
                                SupplierName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.SupplierId))?.FullName,
                                BrandId = quotation.BrandId,
                                BrandName = quotation.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(quotation.BrandId)), x => x.Name) : "-",
                                BookerId = quotation.BookerId,
                                BookerName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.BookerId))?.FullName,
                                Quantity = quotation.Quantity,
                                PaymentDueDate = paymentDueDate,
                                Industry = industry,
                            });
                        }
                    }
                    pastOrders.Add(model);
                }

                var data = new List<object>();
                if (activeOrders.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Order.ActiveOrder"),
                        Data = activeOrders
                    });
                }
                if (pastOrders.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Order.PastOrder"),
                        Data = pastOrders
                    });
                }

                return Ok(new { success = true, message = "", data = data, totalPages = allOrdersPageList.TotalPages, currentPage = allOrdersPageList.PageIndex });

            }
            else
            {
                return Ok(new { success = false, message = "No orders found." });
            }
        }

        [HttpGet("supplier-order-summary/{orderId}")]
        public async Task<IActionResult> OrderManagement_SupplierOrderSummary(int orderId)
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

            var totalAmountPaid = await _shipmentService.GetOrderPaidAmount(order);
            var totalAmountBalance = orderCalculation.OrderTotal - totalAmountPaid;

            try
            {
                var data = new
                {
                    TotalPaybles = await _priceFormatter.FormatPriceAsync(orderCalculation.OrderTotal),
                    TotalPaid = await _priceFormatter.FormatPriceAsync(totalAmountPaid),
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
        #endregion

        #region Order Delivery Request

        [HttpPost("Sale-order-accept-shipment-request/{shipmentRequestId}")]
        public async Task<IActionResult> OrderManagement_SaleOrderAcceptShipmentRequest(int shipmentRequestId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                var shipmentRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipmentRequestId);
                if (shipmentRequest is null)
                    return Ok(new { success = false, message = "Shipment request not found" });

                var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(shipmentRequest.OrderDeliveryScheduleId);
                if (expectedShipment is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                var order = await _orderService.GetOrderByIdAsync(shipmentRequest.OrderId);
                if (order is null)
                    return Ok(new { success = false, message = "Order not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                //Delivery request update 
                shipmentRequest.VerifiedUserId = (await _workContext.GetCurrentCustomerAsync()).Id;
                shipmentRequest.StatusId = (int)OrderDeliveryRequestEnum.Complete;
                await _orderService.UpdateOrderDeliveryRequestAsync(shipmentRequest);

                var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);

                //Add shipment from delivery request
                var shipment = new Shipment
                {
                    OrderId = shipmentRequest.OrderId,
                    //TrackingNumber = model.TrackingNumber,
                    TotalWeight = null,
                    //AdminComment = model.AdminComment,
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpectedDateShipped = expectedShipment.ExpectedShipmentDateUtc,
                    ExpectedDateDelivered = expectedShipment.ExpectedDeliveryDateUtc,
                    ExpectedDeliveryCost = expectedShipment.ExpectedDeliveryCost,
                    ExpectedQuantity = shipmentRequest.Quantity,
                    PaymentStatusId = (int)PaymentStatus.Pending,
                    DeliveryStatusId = (int)DeliveryStatus.Pending,
                    ShipmentType = ShipmentType.SaleOrder,
                    DeliveryRequestId = shipmentRequest.Id,
                    Source = "Tijara",
                };

                var shipmentItems = new List<ShipmentItem>();

                foreach (var orderItem in orderItems)
                {
                    var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                    //create a shipment item
                    shipmentItems.Add(new ShipmentItem
                    {
                        OrderItemId = orderItem.Id,
                        Quantity = shipmentRequest.Quantity,
                        WarehouseId = 0
                    });
                }

                //if we have at least one item in the shipment, then save it
                if (shipmentItems.Any())
                {
                    await _shipmentService.InsertShipmentAsync(shipment);

                    //generate and set custom shipment number
                    shipment.CustomShipmentNumber = _customNumberFormatter.GenerateSOShipmentCustomNumber(shipment);
                    await _shipmentService.UpdateShipmentAsync(shipment);

                    foreach (var shipmentItem in shipmentItems)
                    {
                        shipmentItem.ShipmentId = shipment.Id;
                        await _shipmentService.InsertShipmentItemAsync(shipmentItem);
                    }

                    //add a note
                    await _orderService.InsertOrderNoteAsync(new OrderNote
                    {
                        OrderId = order.Id,
                        Note = "A shipment has been added",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    await _eventPublisher.PublishAsync(new ShipmentCreatedEvent(shipment));

                    return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Shipment.create.Success"), shipmentId = shipment.Id });
                }

                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Shipment.does'nt.create") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Purchase-order-accept-shipment-request/{shipmentRequestId}")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderAcceptShipmentRequest(int shipmentRequestId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "user not found" });

                var shipmentRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipmentRequestId);
                if (shipmentRequest is null)
                    return Ok(new { success = false, message = "Shipment request not found" });

                var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(shipmentRequest.OrderDeliveryScheduleId);
                if (expectedShipment is null)
                    return Ok(new { success = false, message = "Expected shipment not found" });

                var order = await _orderService.GetOrderByIdAsync(shipmentRequest.OrderId);
                if (order is null)
                    return Ok(new { success = false, message = "Order not found" });

                var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(order.RFQId.Value);
                if (requestForQuotation is null)
                    return Ok(new { success = false, message = "Request for quotation not found" });

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return Ok(new { success = false, message = "Buyer Request not found" });

                //Delivery request update 
                shipmentRequest.VerifiedUserId = (await _workContext.GetCurrentCustomerAsync()).Id;
                shipmentRequest.StatusId = (int)OrderDeliveryRequestEnum.Complete;
                await _orderService.UpdateOrderDeliveryRequestAsync(shipmentRequest);

                var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);

                //Add shipment from delivery request
                var shipment = new Shipment
                {
                    OrderId = shipmentRequest.OrderId,
                    //TrackingNumber = model.TrackingNumber,
                    TotalWeight = null,
                    WarehouseId = shipmentRequest.WarehouseId,
                    //AdminComment = model.AdminComment,
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpectedDateShipped = expectedShipment.ExpectedShipmentDateUtc,
                    ExpectedDateDelivered = expectedShipment.ExpectedDeliveryDateUtc,
                    ExpectedDeliveryCost = expectedShipment.ExpectedDeliveryCost,
                    ExpectedQuantity = shipmentRequest.Quantity,
                    PaymentStatusId = (int)PaymentStatus.Pending,
                    DeliveryStatusId = (int)DeliveryStatus.Pending,
                    ShipmentType = ShipmentType.PurchaseOrder,
                    DeliveryRequestId = shipmentRequest.Id,
                    Source = "Tijara",
                };

                var shipmentItems = new List<ShipmentItem>();

                foreach (var orderItem in orderItems)
                {
                    var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                    //create a shipment item
                    shipmentItems.Add(new ShipmentItem
                    {
                        OrderItemId = orderItem.Id,
                        Quantity = shipmentRequest.Quantity,
                        WarehouseId = shipment.WarehouseId
                    });
                }

                //if we have at least one item in the shipment, then save it
                if (shipmentItems.Any())
                {
                    await _shipmentService.InsertShipmentAsync(shipment);

                    //generate and set custom shipment number
                    shipment.CustomShipmentNumber = _customNumberFormatter.GenerateSOShipmentCustomNumber(shipment);
                    await _shipmentService.UpdateShipmentAsync(shipment);

                    foreach (var shipmentItem in shipmentItems)
                    {
                        shipmentItem.ShipmentId = shipment.Id;
                        await _shipmentService.InsertShipmentItemAsync(shipmentItem);
                    }

                    //add inventory inbound
                    await _inventoryService.AddInventoryInboundAsync(order, requestForQuotation, shipment);

                    //add a note
                    await _orderService.InsertOrderNoteAsync(new OrderNote
                    {
                        OrderId = order.Id,
                        Note = "A shipment has been added",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    await _eventPublisher.PublishAsync(new ShipmentCreatedEvent(shipment));

                    return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Shipment.create.Success"), shipmentId = shipment.Id });
                }

                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Shipment.does'nt.create") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("shipment-request-reject")]
        public async Task<IActionResult> OrderManagement_ShipmentRequestReject([FromBody] OrderManagementApiModel.RejectDeliveryRequest model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                if (model.ShipmentRequestId <= 0)
                    return Ok(new { success = false, message = "Shipment request id is required" });

                var shipmentyRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(model.ShipmentRequestId);
                if (shipmentyRequest is null)
                    return Ok(new { success = false, message = "Delivery request not found" });

                if (string.IsNullOrWhiteSpace(model.RejectedReason))
                    return Ok(new { success = false, message = "Rejected reason is required" });

                //if (model.RejectedReason == "Other" && string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                //    return Ok(new { success = false, message = "Rejected other reason is required" });

                //deliveryRequest.OrderDeliveryRequestEnum = OrderDeliveryRequestEnum.Cancelled;

                //if (model.RejectedReason == "Other" && !string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                //    model.RejectedReason = $"Other - {model.RejectedOtherReason}";
                //else
                //    deliveryRequest.RejectedReason = model.RejectedReason;

                shipmentyRequest.StatusId = (int)OrderDeliveryRequestEnum.Cancelled;
                await _orderService.UpdateOrderDeliveryRequestAsync(shipmentyRequest);

                //send notification to ops assinged  user
                //SendPushNotifications(
                //   _pushNotificationService, _configuration, _logger,
                //   customerId: deliveryRequest.CreatedBy,
                //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.DeliveryRequestIncompletion.Title"),
                //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.DeliveryRequestIncompletion.Body"), deliveryRequest.RejectedReason),
                //   entityId: deliveryRequest.Id, entityName: "OrderDeliveryRequest",
                //   data: new Dictionary<string, string>()
                //   {
                //        { "entityId", deliveryRequest.Id.ToString() },
                //        { "entityName", "OrderDetail" },
                //        { "orderDeliveryRequestId", deliveryRequest.Id.ToString() },
                //        { "orderId", deliveryRequest.OrderId.ToString() }
                //   });

                //send notification to all [OpsHead] users
                //var allOpsHeadUsers = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.OpsHeadRoleName)).Id })).ToList();
                //foreach (var opsHeadUser in allOpsHeadUsers)
                //{
                // SendPushNotifications(
                //_pushNotificationService, _configuration, _logger,
                //customerId: opsHeadUser.Id,
                //title: await _localizationService.GetResourceAsync("PushNotification.Tijara.DeliveryRequestIncompletion.Title"),
                //body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.DeliveryRequestIncompletion.Body"), deliveryRequest.RejectedReason),
                //entityId: deliveryRequest.Id, entityName: "OrderDeliveryRequest",
                //data: new Dictionary<string, string>()
                //{
                //     { "entityId", deliveryRequest.Id.ToString() },
                //     { "entityName", "OrderDeliveryRequest" },
                //     { "orderDeliveryRequestId", deliveryRequest.Id.ToString() }
                //});
                //}

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Delivery.Cancelled.Rejected.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("re-assign-agent")]
        public async Task<IActionResult> OrderManagement_ReAssignAgent([FromBody] OrderManagementApiModel.ReAssignAgent model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                if (model.Id <= 0)
                    return Ok(new { success = false, message = "Id is required" });

                if (model.agentId <= 0)
                    return Ok(new { success = false, message = "AgentId is required" });

                if (string.IsNullOrWhiteSpace(model.type))
                    return Ok(new { success = false, message = "type is required" });

                var agent = await _customerService.GetCustomerByIdAsync(model.agentId);
                if (agent is null)
                    return Ok(new { success = false, message = "Agent not found" });

                if (model.type == "ShipmentRequest")
                {
                    var deliverRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(model.Id);
                    if (deliverRequest is null)
                        return Ok(new { success = false, message = "Delivery request not found" });

                    var oldAgentId = deliverRequest.AgentId;
                    deliverRequest.AgentId = agent.Id;
                    await _orderService.UpdateOrderDeliveryRequestAsync(deliverRequest);

                    //send notification to new agent user
                    //SendPushNotifications(
                    //   _pushNotificationService, _configuration, _logger,
                    //   customerId: deliverRequest.AgentId,
                    //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.ReassignOnTicketsNewAgent.Title"),
                    //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.ReassignOnTicketsNewAgent.Body"), "DeliveryRequest"),
                    //   entityId: deliverRequest.Id, entityName: "OrderDeliveryRequest",
                    //   data: new Dictionary<string, string>()
                    //   {
                    //        { "entityId", deliverRequest.Id.ToString() },
                    //        { "entityName", "Tickets" }
                    //   });

                    //send notification to old agent user
                    //SendPushNotifications(
                    //   _pushNotificationService, _configuration, _logger,
                    //   customerId: oldAgentId,
                    //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.ReassignOnTicketsOldAgent.Title"),
                    //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.ReassignOnTicketsOldAgent.Body"), "DeliveryRequest"),
                    //   entityId: deliverRequest.Id, entityName: "OrderDeliveryRequest",
                    //   data: new Dictionary<string, string>()
                    //   {
                    //        { "entityId", deliverRequest.Id.ToString() },
                    //        { "entityName", "Tickets" }
                    //   });

                    return Ok(new { success = true, message = "Delivery request updated succesfuly" });
                }
                //else if (model.type == "SalesReturn")
                //{
                //    var saleReturn = await _orderService.GetOrderSalesReturnRequestByIdAsync(model.Id);
                //    if (saleReturn is null)
                //        return Ok(new { success = false, message = "Sales return request not found" });

                //    var oldAgentId = saleReturn.AgentId;
                //    saleReturn.AgentId = agent.Id;
                //    await _orderService.UpdateOrderSalesReturnRequestAsync(saleReturn);

                //    //send notification to new agent user
                //    //SendPushNotifications(
                //    //   _pushNotificationService, _configuration, _logger,
                //    //   customerId: saleReturn.AgentId,
                //    //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.ReassignOnTicketsNewAgent.Title"),
                //    //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.ReassignOnTicketsNewAgent.Body"), "SalesReturn"),
                //    //   entityId: saleReturn.Id, entityName: "OrderSalesReturnRequest",
                //    //   data: new Dictionary<string, string>()
                //    //   {
                //    //        { "entityId", saleReturn.Id.ToString() },
                //    //        { "entityName", "Tickets" }
                //    //   });

                //    //send notification to old agent user
                //    //SendPushNotifications(
                //    //   _pushNotificationService, _configuration, _logger,
                //    //   customerId: oldAgentId,
                //    //   title: await _localizationService.GetResourceAsync("PushNotification.Tijara.ReassignOnTicketsOldAgent.Title"),
                //    //   body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.ReassignOnTicketsOldAgent.Body"), "SalesReturn"),
                //    //   entityId: saleReturn.Id, entityName: "OrderSalesReturnRequest",
                //    //   data: new Dictionary<string, string>()
                //    //   {
                //    //        { "entityId", saleReturn.Id.ToString() },
                //    //        { "entityName", "Tickets" }
                //    //   });

                //    return Ok(new { success = true, message = "Sale return request updated succesfuly" });
                //}
                //else if (type == "OrderPayment")
                //{
                //    var po = await _orderService.GetOrderPaymentByIdAsync(id);
                //    if (po is null)
                //        return Ok(new { success = false, message = "Order payment not found" });

                //    po.AgentId = agent.Id;
                //    await _orderService.UpdateOrderPaymentAsync(po);

                //    return Ok(new { success = true, message = "Order payment updated succesfuly" });
                //}

                return Ok(new { success = false, message = "Invalid type" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        //Dont have permission
        [HttpPost("order-delivery-request-mark-as-incomplete/{deliveryRequestId}")]
        public async Task<IActionResult> OrderManagement_OrderDeliveryRequestMarkAsIncomplete(int deliveryRequestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var delievryRequestForm = await _orderService.GetOrderDeliveryRequestByIdAsync(deliveryRequestId);
            if (delievryRequestForm is null)
                return Ok(new { success = false, message = "Delivery order request not found" });

            delievryRequestForm.StatusId = (int)OrderDeliveryRequestEnum.Cancelled;
            delievryRequestForm.VerifiedUserId = user.Id;
            await _orderService.UpdateOrderDeliveryRequestAsync(delievryRequestForm);

            return Ok(new { success = true, message = "" });
        }

        #endregion

        #region Payment orders

        //[HttpGet("raise-po-by-quotation/{orderId}/{supplierId}/{quotationId}")]
        //public async Task<IActionResult> OrderManagement_RaisePoByQuotation(int orderId, int supplierId, int quotationId)
        //{
        //    var user = await _workContext.GetCurrentCustomerAsync();
        //    if (!await _customerService.IsRegisteredAsync(user))
        //        return Ok(new { success = false, message = "user not found" });

        //    if (orderId <= 0)
        //        return Ok(new { success = false, message = "Order id is required" });

        //    var order = await _orderService.GetOrderByIdAsync(orderId);
        //    if (order == null)
        //        return Ok(new { success = false, message = "Order not found." });

        //    var orderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(order.Id);
        //    if (orderCalculation is null)
        //        return Ok(new { success = false, message = "Order calculation not found." });

        //    if (supplierId <= 0)
        //        return Ok(new { success = false, message = "Supplier id is required" });

        //    var supplier = await _customerService.GetCustomerByIdAsync(supplierId);
        //    if (supplier == null)
        //        return Ok(new { success = false, message = "Supplier not found." });

        //    if (quotationId <= 0)
        //        return Ok(new { success = false, message = "quotation id is required" });

        //    var supplierQuotation = await _quotationService.GetQuotationByIdAsync(quotationId);
        //    if (supplierQuotation == null)
        //        return Ok(new { success = false, message = "Supplier quotation not found." });

        //    var buyerRequest = (await _requestService.GetRequestByIdAsync(order.RequestId));
        //    if (buyerRequest == null)
        //        return Ok(new { success = false, message = "Buyer request not found." });

        //    var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
        //    if (product == null)
        //        return Ok(new { success = false, message = "Product not found." });

        //    var productItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
        //    if (productItem == null)
        //        return Ok(new { success = false, message = "Product item not found." });

        //    var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        //    var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

        //    //Supplier total payable
        //    //var supplierTotalPayble = 0m;

        //    //var quotationMappings = await _quotationService.GetAllRequestQuotationApprovedMappingAsync(orderId: order.Id, buyerRequestId: order.BuyerRequestId, supplierId: supplier.Id);
        //    //foreach (var mapping in quotationMappings)
        //    //{
        //    //    var quotation = await _quotationService.GetQuotationByIdAsync(mapping.QuotationId);
        //    //    if (quotation is null)
        //    //        continue;

        //    //    supplierTotalPayble += quotation.TotalPayble;
        //    //}

        //    //var orderPayment = (await _ledgerService.GetAllPaymentsAsync(orderId: order.Id)).ToList();

        //    var data = new
        //    {
        //        ZarayeRepresentative = new
        //        {
        //            name = user.FullName
        //        },
        //        sellerinfo = new
        //        {
        //            SupplierId = supplier.Id,
        //            SupplierEmail = supplier.Email,
        //            SupplierFullname = supplier.FullName,
        //            SupplierCnic = !string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(supplier, NopCustomerDefaults.CnicAttribute)) ? await _genericAttributeService.GetAttributeAsync<string>(supplier, NopCustomerDefaults.CnicAttribute) : "N/A",
        //            SupplierNtnNumber = !string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(supplier, NopCustomerDefaults.VatNumberAttribute)) ? await _genericAttributeService.GetAttributeAsync<string>(supplier, NopCustomerDefaults.VatNumberAttribute) : "N/A",
        //            SupplierType = (await _customerService.GetUserTypeByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(supplier, NopCustomerDefaults.SupplierTypeIdAttribute)))?.Name,
        //            Payable = order.OrderTotal
        //        },
        //        orderDetails = new
        //        {
        //            ProductName = product.Name,
        //            //ProductAttributesInfo = await _productAttributeFormatter.FormatAttributesAsync(product, buyerRequest.ProductAttributeXml),
        //            ProductAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
        //            ProductBrand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-",
        //            ExpectedDateOfDelivery = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString(),
        //            ProductQuantity = buyerRequest.Quantity.ToString(),
        //            PerBagRate = (await _priceFormatter.FormatPriceAsync(productItem.UnitPriceInclTax, true, false)),
        //            UnitRateWithCalculationAmount = await _priceFormatter.FormatPriceAsync(order.OrderTotal / buyerRequest.Quantity, true, false),
        //            GST_Payable = $"{await _priceFormatter.FormatOrderPriceAsync(orderCalculation.GSTRate, primaryStoreCurrency.Rate, primaryStoreCurrency.CurrencyCode, false, primaryStoreCurrency, languageId, null, false)} inc GST {orderCalculation.GSTRate.ToString("0.00")}",
        //            WHT_Payable = $"{await _priceFormatter.FormatOrderPriceAsync(orderCalculation.WHTRate, primaryStoreCurrency.Rate, primaryStoreCurrency.CurrencyCode, false, primaryStoreCurrency, languageId, null, false)} inc GST {orderCalculation.WHTRate.ToString("0.00")}"
        //        },
        //        payables = new
        //        {
        //            TotalPayble = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false),
        //            //totalRemainingQty = await _orderService.gettota(order.Id, supplierQuotation.Id) - _orderService.GetTotalOrderPaymentQuantityAsync(order.Id, supplierQuotation.Id),
        //            TotalPayblePaid = await _priceFormatter.FormatOrderPriceAsync(await _shipmentService.GetOrderPaidAmount(order), primaryStoreCurrency.Rate, primaryStoreCurrency.CurrencyCode, false, primaryStoreCurrency, languageId, null, false),
        //        }
        //    };

        //    return Ok(new { success = true, message = "", data });
        //}

        //[HttpGet("raise-po-detail/{poId}")]
        //public async Task<IActionResult> OrderManagement_RaisePoDetail(int poId)
        //{
        //    var user = await _workContext.GetCurrentCustomerAsync();
        //    if (!await _customerService.IsRegisteredAsync(user))
        //        return Ok(new { success = false, message = "user not found" });

        //    var orderPayment = await _ledgerService.GetPaymentByIdAsync(poId);
        //    if (orderPayment == null)
        //        return Ok(new { success = false, message = "PO not found." });

        //    var shipmentPaymentMappings = (await _ledgerService.GetAllShipmentPaymentMappingsAsync(paymentId : orderPayment.Id));

        //    var order = await _orderService.GetOrderByIdAsync(orderPayment.IsBusienssApproved);
        //    if (order == null)
        //        return Ok(new { success = false, message = "Order not found." });

        //    var supplier = await _customerService.GetCustomerByIdAsync(orderPayment.SupplierId);
        //    if (supplier == null)
        //        return Ok(new { success = false, message = "Supplier not found." });

        //    var supplierQuotation = await _sellerService.GetSellerBidByIdAsync(orderPayment.QuotationId);
        //    if (supplierQuotation == null)
        //        return Ok(new { success = false, message = "Supplier quotation not found." });

        //    //Todo
        //    var buyerRequest = (await _requestService.GetRequestByIdAsync(order.requestId));
        //    if (buyerRequest == null)
        //        return Ok(new { success = false, message = "Buyer request not found." });

        //    var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
        //    if (product == null)
        //        return Ok(new { success = false, message = "Product not found." });

        //    var productItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
        //    if (productItem == null)
        //        return Ok(new { success = false, message = "Product item not found." });

        //    var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        //    var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

        //    //Supplier total payable
        //    var supplierTotalPayble = 0m;

        //    var quotationMappings = await _sellerService.GetAllRequestQuotationApprovedMappingAsync(orderId: order.Id, buyerRequestId: order.BuyerRequestId, supplierId: supplier.Id);
        //    foreach (var mapping in quotationMappings)
        //    {
        //        var quotation = await _sellerService.GetSellerBidByIdAsync(mapping.QuotationId);
        //        if (quotation is null)
        //            continue;

        //        supplierTotalPayble += quotation.TotalPayble;
        //    }

        //    var download = await _downloadService.GetDownloadByIdAsync(orderPayment.PurchaseInvoiceId);

        //    var data = new
        //    {
        //        zarayeRepresentative = new
        //        {
        //            name = user.FullName
        //        },
        //        sellerInfo = new
        //        {
        //            SupplierId = supplier.Id,
        //            SupplierEmail = supplier.Email,
        //            SupplierFullname = supplier.FullName,
        //            SupplierCnic = !string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(supplier, NopCustomerDefaults.CnicAttribute)) ? await _genericAttributeService.GetAttributeAsync<string>(supplier, NopCustomerDefaults.CnicAttribute) : "N/A",
        //            SupplierNtnNumber = !string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(supplier, NopCustomerDefaults.VatNumberAttribute)) ? await _genericAttributeService.GetAttributeAsync<string>(supplier, NopCustomerDefaults.VatNumberAttribute) : "N/A",
        //            SupplierType = (await _customerService.GetUserTypeByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(supplier, NopCustomerDefaults.SupplierTypeIdAttribute)))?.Name,
        //            Payable = supplierTotalPayble
        //        },
        //        orderDetail = new
        //        {
        //            orderId = order.Id,
        //            orderCustomNumber = order.CustomOrderNumber,
        //            ProductName = product.Name,
        //            QuotationId = supplierQuotation.Id,
        //            QuotationNumber = supplierQuotation.CustomQuotationNumber,
        //            //ProductAttributesInfo = await _productAttributeFormatter.FormatAttributesAsync(product, buyerRequest.ProductAttributeXml),
        //            ProductAttributesInfo = await Booker_ParseAttributeXml(buyerRequest.ProductAttributeXml),
        //            ProductBrand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : buyerRequest.OtherBrand,
        //            ExpectedDateOfDelivery = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString(),
        //            ProductQuantity = buyerRequest.Quantity.ToString(),
        //            PerBagRate = (await _priceFormatter.FormatPriceAsync(productItem.UnitPriceInclTax, true, false)),
        //            UnitRateWithCalculationAmount = await _priceFormatter.FormatPriceAsync(order.TotalPayble / buyerRequest.Quantity, true, false),
        //            GST_Payable = $"{await _priceFormatter.FormatOrderPriceAsync(supplierQuotation.GST_Payable, primaryStoreCurrency.Rate, primaryStoreCurrency.CurrencyCode, false, primaryStoreCurrency, languageId, null, false)} inc GST {supplierQuotation.GSTRate_Payable.ToString("0.00")}",
        //            WHT_Payable = $"{await _priceFormatter.FormatOrderPriceAsync(supplierQuotation.WHT_Payable, primaryStoreCurrency.Rate, primaryStoreCurrency.CurrencyCode, false, primaryStoreCurrency, languageId, null, false)} inc GST {supplierQuotation.WHTRate_Payable.ToString("0.00")}"
        //        },
        //        payableDetail = new
        //        {
        //            TotalPayble = await _priceFormatter.FormatPriceAsync(order.TotalPayble, true, false),
        //            totalRemainingQty = await _orderService.GetTotalOrderQuantityAsync(order.Id, supplierQuotation.Id) - _orderService.GetTotalOrderPaymentQuantityAsync(order.Id, supplierQuotation.Id),
        //            TotalPayblePaid = await _priceFormatter.FormatOrderPriceAsync((await _orderService.GetOrderPaymentsByOrderIdAsync(order.Id, "Payable", isApproved: true)).Sum(o => o.Amount), primaryStoreCurrency.Rate, primaryStoreCurrency.CurrencyCode, false, primaryStoreCurrency, languageId, null, false),
        //        },
        //        paymentDetail = new
        //        {
        //            AccountNumber = orderPayment.AccountNumber,
        //            AccountTitle = orderPayment.AccountTitle,
        //            PaymentDueDate = (await _dateTimeHelper.ConvertToUserTimeAsync(orderPayment.PaymentDueDate, DateTimeKind.Utc)).ToString("MM/dd/yyyy h:mm:ss tt"),
        //            DateOfInvoice = (await _dateTimeHelper.ConvertToUserTimeAsync(orderPayment.DateOfInvoice, DateTimeKind.Utc)).ToString("MM/dd/yyyy h:mm:ss tt"),
        //            PaymentTerm = orderPayment.PaymentTerm,
        //            Quantity = orderPayment.Quantity,
        //            Note = orderPayment.Note,
        //            status = await _localizationService.GetLocalizedEnumAsync(orderPayment.OrderPaymentStatus),
        //            comment = orderPayment.Comment,
        //            file = new
        //            {
        //                downloadUrl = download != null ? _storeContext.GetCurrentStore().Url + $"download/getfileupload/{download.DownloadGuid}" : "",
        //                fileName = download != null ? download.Filename : ""
        //            }
        //        }
        //    };

        //    return Ok(new { success = true, message = "", data });
        //}

        //[HttpPost("raise-po")]
        //public async Task<IActionResult> OrderManagement_RaisePo([FromBody] OrderManagementApiModel.RaisePoAddModel model)
        //{
        //    try
        //    {
        //        //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRaisePo))
        //        //    return Ok(new { success = false, message = "don't have permission" });

        //        var user = await _workContext.GetCurrentCustomerAsync();
        //        if (!await _customerService.IsRegisteredAsync(user))
        //            return Ok(new { success = false, message = "user not found" });

        //        bool isUpload = false;

        //        if (model.orderId <= 0)
        //            return Ok(new { success = false, message = "Order id is required" });

        //        if (model.supplierId <= 0)
        //            return Ok(new { success = false, message = "Supplier id is required" });

        //        var supplier = await _customerService.GetCustomerByIdAsync(model.supplierId);
        //        if (supplier == null)
        //            return Ok(new { success = false, message = "Supplier not found." });

        //        if (model.QuotationId <= 0)
        //            return Ok(new { success = false, message = "quotation id is required" });

        //        if (model.AddOrderPaymentQuantity_Payable <= 0)
        //            return Ok(new { success = false, message = "quantity id is required" });

        //        if (model.imgBytes is not null && !string.IsNullOrWhiteSpace(model.fileName))
        //            isUpload = true;

        //        //custom validations 

        //        var order = await _orderService.GetOrderByIdAsync(model.orderId);
        //        if (order == null)
        //            return Ok(new { success = false, message = "Order not found." });

        //        var buyerRequest = await _buyerService.GetBuyerRequestByIdAsync(order.BuyerRequestId);
        //        if (buyerRequest == null)
        //            return Ok(new { success = false, message = "Buyer request not found." });

        //        var industry = await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId);
        //        if (industry == null)
        //            return Ok(new { success = false, message = "Industry not found." });

        //        var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
        //        if (category == null)
        //            return Ok(new { success = false, message = "Category not found." });

        //        if (string.IsNullOrWhiteSpace(model.PaymentTerm))
        //            return Ok(new { success = false, message = "Payment Term is required" });

        //        if (string.IsNullOrWhiteSpace(model.BankName))
        //            return Ok(new { success = false, message = "Bank Name is required" });

        //        if (string.IsNullOrWhiteSpace(model.AccountNumber))
        //            return Ok(new { success = false, message = "Account Number is required" });

        //        if (string.IsNullOrWhiteSpace(model.AccountTitle))
        //            return Ok(new { success = false, message = "Account Title is required" });

        //        if (model.AddOrderPaymentQuantity_Payable > 0)
        //        {
        //            var quotation = await _sellerService.GetSellerBidByIdAsync(model.QuotationId);
        //            if (quotation != null)
        //            {
        //                var totalRemainingQty = /*quotation.Quantity*/ await _orderService.GetTotalOrderQuantityAsync(model.orderId, quotation.Id) - _orderService.GetTotalOrderPaymentQuantityAsync(order.Id, model.QuotationId);
        //                if (totalRemainingQty == 0)
        //                    return Ok(new { success = false, message = "there is no remaining quantity left" });
        //                if (model.AddOrderPaymentQuantity_Payable > totalRemainingQty)
        //                    return Ok(new { success = false, message = $"{string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Payments.Qunatity"), Math.Round(totalRemainingQty))}" });

        //                model.UnitRateWithCalculation = quotation.TotalPayble / quotation.Quantity;
        //            }
        //        }

        //        var orderPayment = new OrderPayment
        //        {
        //            OrderId = order.Id,
        //            QuotationId = model.QuotationId,
        //            Amount = model.UnitRateWithCalculation * model.AddOrderPaymentQuantity_Payable,
        //            Note = model.AddOrderPaymentMessage_Payable,
        //            PaymentType = "Payable",
        //            CreatedById = (await _workContext.GetCurrentCustomerAsync()).Id,
        //            CreatedOnUtc = DateTime.UtcNow,
        //            OrderPaymentStatus = OrderPaymentStatus.Pending,
        //            PurchaseInvoiceId = isUpload ? OrderManagement_Upload(model.imgBytes, model.fileName) : 0,
        //            BankName = model.BankName,
        //            AccountNumber = model.AccountNumber,
        //            AccountTitle = model.AccountTitle,
        //            PaymentDueDate = model.PaymentDueDate,
        //            DateOfInvoice = model.DateOfInvoice,
        //            PaymentTerm = model.PaymentTerm,
        //            Quantity = model.AddOrderPaymentQuantity_Payable,
        //            SupplierId = model.supplierId,
        //            BankDetailId = model.BankDetailId,
        //            ExpiryDate = DateTime.UtcNow.AddDays(category.TicketExpiryDays > 0 ? category.TicketExpiryDays : 1),
        //            PriorityId = category.TicketPirority > 0 ? category.TicketPirority : (int)TicketEnum.Medium
        //        };
        //        await _orderService.InsertOrderPaymentAsync(orderPayment);

        //        //add a note
        //        await _orderService.InsertOrderNoteAsync(new OrderNote
        //        {
        //            OrderId = order.Id,
        //            Note = $"Order supplier payable has been added payment order # {orderPayment.Id} against SupplierId # {model.supplierId}",
        //            DisplayToCustomer = false,
        //            CreatedOnUtc = DateTime.UtcNow
        //        });

        //        //send notification to all [BH] users
        //        var allBHUsers = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.BusinessHeadRoleName)).Id })).ToList();
        //        foreach (var bhUser in allBHUsers)
        //        {
        //            SendPushNotifications(
        //                _pushNotificationService, _configuration, _logger,
        //                customerId: bhUser.Id,
        //                title: await _localizationService.GetResourceAsync("PushNotification.Tijara.RaisePO.Title"),
        //                body: string.Format(await _localizationService.GetResourceAsync("PushNotification.Tijara.RaisePO.Body"), orderPayment.Id, await _customerService.GetCustomerFullNameAsync(orderPayment.SupplierId), await _localizationService.GetLocalizedAsync(industry, x => x.Name)),
        //                entityId: orderPayment.Id, entityName: "OrderPayment",
        //                data: new Dictionary<string, string>()
        //                {
        //                    { "entityId", orderPayment.Id.ToString() },
        //                    { "entityName", "RaisePODetail" },
        //                    { "poId", orderPayment.Id.ToString() }
        //                });
        //        }

        //        return Ok(new { success = true, message = "rais po inserted" });
        //    }
        //    catch (Exception ex)
        //    {
        //        Ok(new { success = false, message = ex.Message });
        //    }

        //    return Ok(new { success = false, message = "" });
        //}
        [HttpGet("all-tickets")]
        public async Task<IActionResult> OrderManagement_GetAllTickets(string status = "", string orderby = "")
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user) && !await _customerService.IsOperationsAsync(user) && !await _customerService.IsBusinessHeadAsync(user) &&
                    !await _customerService.IsFinanceAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                List<DrANDSrList> todo = new List<DrANDSrList>();
                //List<DrANDSrList> done = new List<DrANDSrList>();
                List<DRAndSRGenericList> combineList = new List<DRAndSRGenericList>();
                List<int> deliveryRequestsStatusIds = new List<int>();
                List<int> saleReturnRequestsStatusIds = new List<int>();
                List<int> poStatusIds = new List<int>();


                if (status == "Pending")
                {
                    deliveryRequestsStatusIds.AddRange(new List<int> { (int)OrderDeliveryRequestEnum.Pending, (int)OrderDeliveryRequestEnum.Expired });
                    saleReturnRequestsStatusIds.AddRange(new List<int> { (int)OrderSalesReturnRequestEnum.Pending, (int)OrderSalesReturnRequestEnum.Expired });
                }
                if (status == "Accepted")
                {
                    deliveryRequestsStatusIds.AddRange(new List<int> { (int)OrderDeliveryRequestEnum.Complete });
                    saleReturnRequestsStatusIds.AddRange(new List<int> { (int)OrderSalesReturnRequestEnum.Complete });
                }
                if (status == "Rejected")
                {
                    deliveryRequestsStatusIds.AddRange(new List<int> { (int)OrderDeliveryRequestEnum.Cancelled });
                    saleReturnRequestsStatusIds.AddRange(new List<int> { (int)OrderSalesReturnRequestEnum.Cancelled });
                }

                int agentId = 0;
                if (await _customerService.IsOperationsAsync(user) && !await _customerService.IsOpsHeadAsync(user))
                    agentId = user.Id;

                if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDeliveryRequestTicket))
                {
                    //Get delivery Requests
                    var orderDeliveryRequests = (await _orderService.SearchOrderDeliveryRequestsAsync(sIds: deliveryRequestsStatusIds, agentId: agentId)).ToList();
                    foreach (var deliveryRequest in orderDeliveryRequests)
                    {
                        var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                        if (order is null)
                            continue;

                        var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                        if (buyerRequest is null)
                            continue;

                        if (order.OrderTypeId == (int)OrderType.PurchaseOrder)
                        {
                            combineList.Add(new DRAndSRGenericList
                            {
                                Id = (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id)) == null ? deliveryRequest.Id : (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id)).Id,
                                Type = "PickupRequest",
                                ShipmentType = (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id)) == null ? "ExpectedShipment" : "Shipment",
                                AgentId = deliveryRequest.AgentId,
                                AgentName = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                                TimeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                                Date = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                                Priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                                Requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                                StatusId = deliveryRequest.StatusId,
                                Status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                                OrderType = await _localizationService.GetLocalizedEnumAsync(order.OrderType),
                                OrderId = order.Id,
                                CustomeOrderNumber = order.CustomOrderNumber,
                                CustomerId = order.CustomerId,
                                FullName = (await _customerService.GetCustomerByIdAsync(order.CustomerId))?.FirstName
                            });
                        }
                        if (order.OrderTypeId == (int)OrderType.SaleOrder)
                        {
                            combineList.Add(new DRAndSRGenericList
                            {
                                Id = (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id)) == null ? deliveryRequest.Id : (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id)).Id,
                                Type = "ShipmentRequest",
                                ShipmentType = (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id)) == null ? "ExpectedShipment" : "Shipment",
                                AgentId = deliveryRequest.AgentId,
                                AgentName = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                                TimeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                                Date = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                                Priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                                Requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                                StatusId = deliveryRequest.StatusId,
                                Status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                                OrderType = await _localizationService.GetLocalizedEnumAsync(order.OrderType),
                                OrderId = order.Id,
                                CustomeOrderNumber = order.CustomOrderNumber,
                                CustomerId = order.CustomerId,
                                FullName = (await _customerService.GetCustomerByIdAsync(order.CustomerId))?.FirstName
                            });
                        }
                    }
                }

                var model = new DrANDSrList();

                if (combineList.Any())
                {
                    var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);
                    List<IGrouping<string, DRAndSRGenericList>> todoGroupingLists = null;

                    if (orderby == "Asc")
                    {
                        todoGroupingLists = combineList.OrderBy(x => x.CreatedOnUtc)
                        .GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy"))
                        .ToList();

                    }
                    if (orderby == "Desc")
                    {
                        todoGroupingLists = combineList.OrderByDescending(x => x.CreatedOnUtc).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                    }

                    foreach (var todoGrouping in todoGroupingLists)
                    {
                        var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                        if (currentTime.ToString("dd/MM/yyyy") == todoGrouping.Key)
                            model.Date = "Today";
                        else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == todoGrouping.Key)
                            model.Date = "Yesterday";
                        else
                            model.Date = todoGrouping.Key;

                        foreach (var todoGroup in todoGrouping)
                        {
                            model.Data.Add(new DRAndSRData
                            {
                                Id = todoGroup.Id,
                                Type = todoGroup.Type,
                                AgentName = todoGroup.AgentName,
                                TimeRemaining = todoGroup.TimeRemaining,
                                Date = todoGroup.Date,
                                Priority = todoGroup.Priority,
                                Requester = todoGroup.Requester,
                                StatusId = todoGroup.StatusId,
                                Status = todoGroup.Status,
                                ShipmentType = todoGroup.ShipmentType,
                                OrderType = todoGroup.OrderType,
                                OrderId = todoGroup.OrderId,
                                CustomeOrderNumber = (await _orderService.GetOrderByIdAsync(todoGroup.OrderId)).CustomOrderNumber,
                                CustomerId = todoGroup.CustomerId,
                                FullName = (await _customerService.GetCustomerByIdAsync(todoGroup.CustomerId))?.FirstName

                            });
                        }
                        todo.Add(model);
                    }


                    //var doneGroupingLists = combineList.OrderByDescending(x => x.CreatedOnUtc).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                    //foreach (var doneGrouping in doneGroupingLists)
                    //{
                    //    //var model = new DrANDSrList();
                    //    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    //    if (currentTime.ToString("dd/MM/yyyy") == doneGrouping.Key)
                    //        model.Date = "Today";
                    //    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == doneGrouping.Key)
                    //        model.Date = "Yesterday";
                    //    else
                    //        model.Date = doneGrouping.Key;

                    //    foreach (var doneGroup in doneGrouping)
                    //    {
                    //        model.Data.Add(new DRAndSRData
                    //        {
                    //            Id = doneGroup.Id,
                    //            Type = doneGroup.Type,
                    //            AgentName = doneGroup.AgentName,
                    //            TimeRemaining = doneGroup.TimeRemaining,
                    //            Date = doneGroup.Date,
                    //            Priority = doneGroup.Priority,
                    //            Requester = doneGroup.Requester,
                    //            StatusId = doneGroup.StatusId,
                    //            Status = doneGroup.Status,
                    //            ShipmentType = doneGroup.ShipmentType,
                    //            OrderType = doneGroup.OrderType,
                    //            OrderId = doneGroup.OrderId,
                    //            CustomeOrderNumber = (await _orderService.GetOrderByIdAsync(doneGroup.OrderId)).CustomOrderNumber,
                    //            CustomerId = doneGroup.CustomerId,
                    //            FullName = (await _customerService.GetCustomerByIdAsync(doneGroup.CustomerId))?.FirstName
                    //        });
                    //    }
                    //    todo.Add(model);
                    //}

                    return Ok(new { success = true, data = model });
                }
                else
                {
                    return Ok(new { success = true, message = "No data found." });
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        #endregion

        #region Buyer Contract

        [HttpPost("buyer-contact-upload-signature")]
        public async Task<IActionResult> OrderManagement_BuyerContactUploadSignature([FromBody] OrderManagementApiModel.BuyerContactUploadSignatureModel model)
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

            buyerContract.SignaturePictureId = await OrderManagement_UploadPicture(model.imgBytes, $"{buyerContract.ContractGuid}");
            await _orderService.UpdateContractAsync(buyerContract);

            await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}");

            return Ok(new
            {
                success = true,
                message = "",
                BuyerContract = new
                {
                    contractId = buyerContract.Id,
                    downloadUrl = _storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}",
                    previewUrl = _storeContext.GetCurrentStore().Url + $"files/contracts/{buyerContract.ContractGuid}.pdf",
                    contractSignature = buyerContract.SignaturePictureId > 0 ? true : false
                }
            });
        }

        [HttpPost("supplier-contact-upload-signature")]
        public async Task<IActionResult> OrderManagement_SupplierContactUploadSignature([FromBody] OrderManagementApiModel.BuyerContactUploadSignatureModel model)
        {
            if (model.imgBytes == null)
                return Ok(new { success = false, message = "Image is required" });

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return Ok(new { success = false, message = "Order not found." });

            var supplierContract = await _orderService.GetContractByIdAsync(model.ContactId);
            if (supplierContract == null)
                return Ok(new { success = false, message = "Buyer contract not found." });

            supplierContract.SignaturePictureId = await OrderManagement_UploadPicture(model.imgBytes, $"{supplierContract.ContractGuid}");
            await _orderService.UpdateContractAsync(supplierContract);

            await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}");

            return Ok(new
            {
                success = true,
                message = "",
                SupplierContract = new
                {
                    contractId = supplierContract.Id,
                    downloadUrl = _storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}",
                    previewUrl = _storeContext.GetCurrentStore().Url + $"files/contracts/{supplierContract.ContractGuid}.pdf",
                    contractSignature = supplierContract.SignaturePictureId > 0 ? true : false
                }
            });
        }

        #endregion

        #region Expected Shipments

        [HttpGet("get-expected-shipments-by-orderId/{orderId}")]
        public async Task<IActionResult> OrderManagement_GetExpectedShipmentsByOrderId(int orderId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order is null)
                    return Ok(new { success = false, message = "Order not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(orderId)).FirstOrDefault();
                if (orderItem is null)
                    return Ok(new { success = false, message = "Order item not found" });

                var request = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (request is null)
                    return Ok(new { success = false, message = "Request not found" });

                var product = await _productService.GetProductByIdAsync(request.ProductId);
                if (product is null)
                    return Ok(new { success = false, message = "Product not found" });

                //decimal remaining = 0;
                //var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(order.Id);
                ////Prepare actual shipped reqmaining
                //foreach (var shipment in (shipments.Where(x => x.ShippedDateUtc.HasValue)).ToList())
                //    remaining += (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).Sum(x => x.Quantity);
                ////Prepare actual delivered reqmaining
                //foreach (var shipment in (shipments.Where(x => x.ShippedDateUtc.HasValue)).ToList())
                //    remaining += (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).Sum(x => x.Quantity);

                var data = new
                {
                    orderNo = order.CustomOrderNumber,
                    productId = product.Id,
                    productName = product.Name,
                    productAttributesInfo = await Booker_ParseAttributeXml(request.ProductAttributeXml),
                    brand = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(request.BrandId)), x => x.Name) : "-",
                    leftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryScheduleQuantityByOrderIdAsync(order.Id),
                    //leftQuantity = orderItem.Quantity - remaining,
                    totalQuantity = orderItem.Quantity,
                    expectedShipments = (await _orderService.GetAllDeliveryScheduleAsync(order.Id)).Select(async d =>
                    {
                        return new
                        {
                            id = d.Id,
                            expectedDeliveryDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedDeliveryDateUtc, DateTimeKind.Utc),
                            expectedShipmentDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedShipmentDateUtc, DateTimeKind.Utc),
                            expectedQuantity = d.ExpectedQuantity > 0 ? d.ExpectedQuantity.ToString() : "",
                            expectedDeliveryCost = d.ExpectedDeliveryCost,
                            createdOnUtc = d.CreatedOnUtc,
                            orderId = d.OrderId,
                            requestRemaninigQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, d.Id),
                            //requestRemaninigQuantity = orderItem.Quantity - remaining,
                            deliveryRequests = (await _orderService.SearchOrderDeliveryRequestsAsync(d.Id)).Select(async dr =>
                            {
                                return new
                                {
                                    id = dr.Id,
                                    //remianing = orderItem.Quantity - ((await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)) != null ? /*Shipped condition*/ (await _shipmentService.GetShipmentByDeliveryRequestId(d.Id)).ShippedDateUtc.HasValue ? (await _shipmentService.GetShipmentByDeliveryRequestId(d.Id)).ActualShippedQuantity : /*Delivered condition*/ (await _shipmentService.GetShipmentByDeliveryRequestId(d.Id)).DeliveryDateUtc.HasValue ? (await _shipmentService.GetShipmentByDeliveryRequestId(d.Id)).ActualDeliveredQuantity : _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, d.Id)),
                                    shipmentId = dr.StatusId == (int)OrderDeliveryRequestEnum.Complete ? (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id))?.Id : 0,
                                    statusId = dr.StatusId,
                                    status = (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)) != null && (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).DeliveryDateUtc.HasValue ? "Delivered" : (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)) != null && (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).ShippedDateUtc.HasValue ? "Shipped" : await _localizationService.GetLocalizedEnumAsync(dr.OrderDeliveryRequestEnum),
                                    quantity = (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)) != null && (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).DeliveryDateUtc.HasValue ? (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).ActualDeliveredQuantity : (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)) != null && (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).ShippedDateUtc.HasValue ? (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id))?.ActualShippedQuantity : dr.Quantity
                                };
                            }).Select(t => t.Result).ToList(),
                        };
                    }).Select(t => t.Result).ToList(),
                };

                return Ok(new { success = true, message = "", data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Sale order Shipments

        [HttpGet("get-all-shipments")]
        public async Task<IActionResult> OrderManagement_GetAllShipments(int ShipmentTypeId = 0)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var data = await (await _shipmentService.GetAllShipmentsAsync(shipmentTypeId: ShipmentTypeId)).SelectAwait(async sh =>
                {
                    //var order = await _orderService.GetOrderByIdAsync(sh.OrderId);
                    //if (order is null)
                    //    return Ok(new { success = false, message = "Order not found" });

                    //var shipmentRequest = await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId);
                    //if (shipmentRequest is null)
                    //    return Ok(new { success = false, message = "Shipment request not found" });

                    return new
                    {
                        orderNumber = (await _orderService.GetOrderByIdAsync(sh.OrderId))?.CustomOrderNumber,
                        orderId = (await _orderService.GetOrderByIdAsync(sh.OrderId))?.Id,
                        shipmentNumber = sh.CustomShipmentNumber,
                        shipmentId = sh.Id,
                        shipmentStatus = await _localizationService.GetLocalizedEnumAsync((await _orderService.GetOrderByIdAsync(sh.OrderId)).ShippingStatus),
                        shipmentRequestStatus = (await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId)) != null ? await _localizationService.GetLocalizedEnumAsync((await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId)).OrderDeliveryRequestEnum) : "",
                        agent = (await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId)) != null ? (await _customerService.GetCustomerByIdAsync((await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId)).AgentId))?.FullName : "",
                        trackingNumber = sh.TrackingNumber != null ? sh.TrackingNumber : "",
                        adminComment = sh.AdminComment != null ? sh.AdminComment : "",
                        expectedDateShipped = sh.ExpectedDateShipped,
                        expectedDateDelivered = sh.ExpectedDateDelivered,
                        expectedQuantity = sh.ExpectedQuantity,
                        expectedDeliveryCost = sh.ExpectedDeliveryCost,
                        shipmentType = await _localizationService.GetLocalizedEnumAsync(sh.ShipmentType)
                    };
                }).ToListAsync();

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("sale-order-mark-as-shipped")]
        public async Task<IActionResult> OrderManagement_SaleOrderMarkAsShipped([FromBody] OrderManagementApiModel.MaskAsShippedModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (model.ShipmentId <= 0)
                return Ok(new { success = false, message = "Shipment id is required" });

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.ShipmentId);
            if (shipment is null)
                return Ok(new { success = false, message = "Shipment not found" });

            try
            {
                var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
                if (order is null)
                    return Ok(new { success = false, message = "Order not found" });

                var request = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (request == null)
                    return Ok(new { success = false, message = "Request not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
                if (orderItem == null)
                    return Ok(new { success = false, message = "Order item not found" });

                var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault(x => x.OrderItemId == orderItem.Id);
                if (shipmentItem == null)
                    return Ok(new { success = false, message = "Shipment item not found" });

                if (model.TransporterId <= 0)
                    return Ok(new { success = false, message = "Transporter id is required" });

                if (model.VehicleId <= 0)
                    return Ok(new { success = false, message = "Vehicle id is required" });

                if (model.VehicleId > 0 && string.IsNullOrWhiteSpace(model.VehicleNumber))
                    return Ok(new { success = false, message = "Vehicle number is required" });

                if (model.RouteTypeId <= 0)
                    return Ok(new { success = false, message = "RouteType id is required" });

                //Vallidate inventory outbound
                if (model.Inventories.Count == 0)
                    return Ok(new { success = false, message = "Inventory is required" });

                if (model.ActualShippedQuantity < shipmentItem.Quantity && string.IsNullOrWhiteSpace(model.ActualShippedQuantityReason))
                    return Ok(new { success = false, message = "Actual shipped quantity reason is required" });

                var selectedOutboundQuantity = 0m;
                foreach (var inboundInventory in model.Inventories)
                {
                    if (inboundInventory.OutboundQuantity > inboundInventory.BalanceQuantity)
                        return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Shipment.InputQuanity.Exceeded.OutboundQuantity") });
                    else
                        selectedOutboundQuantity += inboundInventory.OutboundQuantity;
                }

                if (selectedOutboundQuantity != model.ActualShippedQuantity)
                    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Shipment.Inventory.Invalid.Quantity") });

                foreach (var inbound in model.Inventories)
                {
                    var totalRemainingQty = inbound.OutboundQuantity;
                    if (totalRemainingQty <= 0)
                        continue;

                    var inventyoryInbound = await _inventoryService.GetInventoryInboundByIdAsync(inbound.InventoryId);
                    if (inventyoryInbound is null)
                        return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Admin.Shipment.Inventory.Not.Found") });

                    //Add Inventory OutBound
                    await _inventoryService.AddInventoryOutboundAsync(inventyoryInbound, order, shipment, orderItem, totalRemainingQty);
                }

                var shipDate = shipment.ShippedDateUtc.HasValue;
                shipment.TransporterId = model.TransporterId;
                shipment.VehicleId = model.VehicleId;
                shipment.VehicleNumber = model.VehicleNumber;
                shipment.RouteTypeId = model.RouteTypeId;
                shipment.DeliveryStatusId = (int)DeliveryStatus.EnRoute;
                shipment.PickupAddress = model.PickupAddress;
                shipment.ShippedDateUtc = DateTime.UtcNow;
                shipment.OnLabourCharges = model.Laborcharges;
                shipment.CostOnZarayeId = model.CostOnZarayeId;
                shipment.ActualShippedQuantity = model.ActualShippedQuantity;
                shipment.ActualShippedQuantityReason = model.ActualShippedQuantityReason;
                await _shipmentService.UpdateShipmentAsync(shipment);

                if (!string.IsNullOrWhiteSpace(model.ActualShippedQuantityReason) && model.ActualShippedQuantity < shipmentItem.Quantity)
                {
                    shipmentItem.Quantity = model.ActualShippedQuantity;
                    await _shipmentService.UpdateShipmentItemAsync(shipmentItem);

                    //update delivery request
                    var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                    if (deliveryRequest is not null)
                    {
                        deliveryRequest.Quantity = model.ActualShippedQuantity;
                        await _orderService.UpdateOrderDeliveryRequestAsync(deliveryRequest);
                    }
                }

                var allTransporterLedger = await _customerLedgerService.GetAllTransporterLedgerByShipmentAsync(shipment.Id, new string[] { "Loading charges" });
                if (allTransporterLedger.Any())
                {
                    foreach (var ledger in allTransporterLedger)
                        await _customerLedgerService.DeleteCustomerLedgerAsync(ledger);
                }

                if (model.Laborcharges > 0)
                {
                    var transpoter = await _customerService.GetCustomerByIdAsync(model.TransporterId);
                    if (transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.Yes)
                    {
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                    }
                    else if (transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.No)
                    {
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                    }
                    else if (!transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.Yes)
                    {
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                    }
                    else if (!transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.No)
                    {
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                    }
                }

                //check whether we have more items to ship
                if (await _orderService.HasItemsToAddToShipmentAsync(order) || await _orderService.HasItemsToShipAsync(order))
                    order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
                else
                    order.ShippingStatusId = (int)ShippingStatus.Shipped;
                await _orderService.UpdateOrderAsync(order);


                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Mark.as.shipped.success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("sale-order-mark-as-delivered")]
        public async Task<IActionResult> OrderManagement_SaleOrderMarkAsDelivered([FromBody] OrderManagementApiModel.MaskAsDeliveredModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (model.ShipmentId <= 0)
                return Ok(new { success = false, message = "Shipment id is required" });

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.ShipmentId);
            if (shipment is null)
                return Ok(new { success = false, message = "Shipment not found" });

            var saleOrder = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (saleOrder == null)
                return Ok(new { success = false, message = "Sale order not found" });

            var saleOrderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(shipment.OrderId);
            if (saleOrderCalculation == null)
                return Ok(new { success = false, message = "Sale order calcultaion not found" });

            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault();
            if (shipmentItem == null)
                return Ok(new { success = false, message = "Shipment item not found" });

            if (model.ActualDeliveredQuantity < shipmentItem.Quantity && string.IsNullOrWhiteSpace(model.ActualDeliveredQuantityReason))
                return Ok(new { success = false, message = "Actual delivered quantity reason is required" });

            try
            {
                if (model.DeliveryTypeId <= 0)
                    throw new Exception("Select delivery type");
                if (model.TransporterTypeId <= 0)
                    throw new Exception("Select transporter type");
                if (model.DeliveryTimingId == 0)
                    throw new Exception("Select delivery time");
                if (model.DeliveryTimingId == (int)DeliveryTiming.Delayed && model.DeliveryDelayedReasonId == 0)
                    throw new Exception("Select delivery time reason");
                if (string.IsNullOrWhiteSpace(model.FileName))
                    throw new Exception("Picture is required");

                var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);

                //sum both fileds value then check is > 0
                var deliveryCost = shipment.OnLabourCharges + model.FreightCharges + model.LabourCharges;

                if (model.DeliveryCostTypeId != (int)DeliveryCostType.FreeDelivery && model.DeliveryCostTypeId != (int)DeliveryCostType.BuyerSelfPickup && deliveryCost <= 0)
                    throw new Exception("Enter delivery cost");

                if (shipment.ExpectedDeliveryCost > 0 && deliveryCost > shipment.ExpectedDeliveryCost && model.DeliveryCostReasonId == 0)
                    throw new Exception("Select delivery cost reason");

                if (model.DeliveryCostTypeId == (int)DeliveryCostType.FreeDelivery || model.DeliveryCostTypeId == (int)DeliveryCostType.BuyerSelfPickup)
                {
                    model.FreightCharges = 0;
                    model.LabourCharges = 0;
                    deliveryCost = 0;
                }

                var deliveredDate = shipment.DeliveryDateUtc.HasValue;
                shipment.PictureId = model.FileName != null ? await OrderManagement_UploadPicture(model.ImageBytes, model.FileName) : 0;
                shipment.DeliveryCostTypeId = model.DeliveryCostTypeId;
                shipment.DeliveryTypeId = model.DeliveryTypeId;
                shipment.TransporterTypeId = model.TransporterTypeId;
                shipment.DeliveryCost = deliveryCost;
                //shipment.WarehouseId = model.WarehouseId;
                shipment.FreightCharges = model.FreightCharges;
                shipment.LabourCharges = model.LabourCharges;
                shipment.DeliveryTimingId = model.DeliveryTimingId;
                shipment.DeliveryDelayedReasonId = model.DeliveryDelayedReasonId;
                shipment.DeliveryCostReasonId = model.DeliveryCostReasonId;
                shipment.LabourTypeId = model.LabourTypeId;
                shipment.ShipmentDeliveryAddress = model.ShipmentDeliveryAddress;
                shipment.ActualDeliveredQuantity = model.ActualDeliveredQuantity;
                shipment.ActualDeliveredQuantityReason = model.ActualDeliveredQuantityReason;
                await _shipmentService.UpdateShipmentAsync(shipment);

                var allTransporterLedger = await _customerLedgerService.GetAllTransporterLedgerByShipmentAsync(shipment.Id, new string[] { "Frieght charges", "UnLoading charges" });
                if (allTransporterLedger.Any())
                {
                    foreach (var ledger in allTransporterLedger)
                        await _customerLedgerService.DeleteCustomerLedgerAsync(ledger);
                }

                if (/*shipment.DeliveryCostType != DeliveryCostType.FulfilledBySupplier &&*/ shipment.DeliveryCostType != DeliveryCostType.FreeDelivery)
                {
                    var transpoter = await _customerService.GetCustomerByIdAsync(shipment.TransporterId);
                    if (transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.Yes)
                    {
                        if (shipment.FreightCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                        if (shipment.LabourCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                    }
                    else if (!transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.Yes)
                    {
                        if (shipment.FreightCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: transpoter.Id, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);
                        //await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id); Align to khurram

                        if (shipment.LabourCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                    }
                    else if (transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.No)
                    {
                        if (shipment.FreightCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                        if (shipment.LabourCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                    }
                    else if (!transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.No)
                    {
                        if (shipment.FreightCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                        if (shipment.LabourCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.ActualDeliveredQuantityReason) && model.ActualDeliveredQuantity < shipmentItem.Quantity)
                {
                    shipmentItem.Quantity = model.ActualDeliveredQuantity;
                    await _shipmentService.UpdateShipmentItemAsync(shipmentItem);

                    //update delivery request
                    var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                    if (deliveryRequest is not null)
                    {
                        deliveryRequest.Quantity = model.ActualDeliveredQuantity;
                        await _orderService.UpdateOrderDeliveryRequestAsync(deliveryRequest);
                    }
                }

                await _orderProcessingService.DeliverAsync(shipment, true);

                if (saleOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Broker)
                {
                    var shipmentDeliveredAmount = shipment.DeliveredAmount;

                    #region Finance Income

                    if (saleOrderCalculation.FinanceIncome > 0)
                    {
                        if (saleOrderCalculation.FinanceCostPayment == (int)FinanceCostPaymentStatus.Cash)
                        {
                            var calculatedFinanceIncomeValue = saleOrderCalculation.FinanceIncome * shipmentItem.Quantity;
                            shipmentDeliveredAmount = shipmentDeliveredAmount - calculatedFinanceIncomeValue;
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Finance Income", debit: calculatedFinanceIncomeValue, credit: 0, shipmentId: shipment.Id);
                        }
                        else
                        {
                            var calculatedFinanceIncome = (saleOrderCalculation.FinanceIncome * ((saleOrderCalculation.GSTRate / 100) + 1) * (1 - (saleOrderCalculation.WHTRate / 100)) * ((saleOrderCalculation.WholesaleTaxRate / 100) + 1));
                            calculatedFinanceIncome = calculatedFinanceIncome * shipmentItem.Quantity;
                            shipmentDeliveredAmount = shipmentDeliveredAmount - calculatedFinanceIncome;
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Finance Income", debit: calculatedFinanceIncome, credit: 0, shipmentId: shipment.Id);
                        }
                    }

                    #endregion

                    #region Deliver Ledger

                    //Add Customer Ledger for buyer delivery
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Delivery", debit: shipment.DeliveredAmount, credit: 0, shipmentId: shipment.Id);

                    #endregion

                    #region Broker Cash Ledger

                    if (saleOrderCalculation.BrokerId > 0)
                    {
                        var calculatedBrokerCash = saleOrderCalculation.BrokerCash * shipmentItem.Quantity;
                        if (saleOrderCalculation.BrokerCash > 0)
                        {
                            //Add Customer Ledger for positive broker cash
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrderCalculation.BrokerId, description: "Broker Cash", debit: calculatedBrokerCash, credit: 0, shipmentId: shipment.Id);
                        }
                        else if (saleOrderCalculation.BrokerCash < 0)
                        {
                            //Add Customer Ledger for negative broker cash
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrderCalculation.BrokerId, description: "Broker Cash", debit: 0, credit: Math.Abs(calculatedBrokerCash), shipmentId: shipment.Id);
                        }
                    }

                    #endregion
                }

                #region Buyer Commission Receivable Ledger

                if (saleOrderCalculation.BuyerCommissionReceivableUserId > 0 && saleOrderCalculation.BuyerCommissionReceivablePerBag > 0)
                {
                    var calculatedBuyerCommissionReceivablePerBag = saleOrderCalculation.BuyerCommissionReceivablePerBag * shipmentItem.Quantity;

                    //Add Customer Ledger for Buyer Commission Receivable
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrderCalculation.BuyerCommissionReceivableUserId, description: "Buyer Commission Receivable", debit: calculatedBuyerCommissionReceivablePerBag, credit: 0, shipmentId: shipment.Id);
                }

                #endregion

                #region Buyer Commission Payable Ledger

                if (saleOrderCalculation.BuyerCommissionPayableUserId > 0 && saleOrderCalculation.BuyerCommissionPayablePerBag > 0)
                {
                    var calculatedBuyerCommissionPayablePerBag = saleOrderCalculation.BuyerCommissionPayablePerBag * shipmentItem.Quantity;

                    //Add Customer Ledger for Buyer Commission Payable
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrderCalculation.BuyerCommissionPayableUserId, description: "Buyer Commission Payable", debit: 0, credit: calculatedBuyerCommissionPayablePerBag, shipmentId: shipment.Id);
                }

                #endregion

                #region Margin Ledger

                if (saleOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Broker && saleOrderCalculation.MarginAmount > 0)
                {
                    var calculatedMargin = saleOrderCalculation.MarginAmount * shipmentItem.Quantity;

                    //Add Customer Ledger for Buyer Margin
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Margin", debit: calculatedMargin, credit: 0, shipmentId: shipment.Id);
                }

                #endregion

                //Add Customer Ledger for supplier delivery
                //await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Delivery", debit: shipment.DeliveredAmount, credit: 0, shipmentId: shipment.Id);

                //Add customer ledger for delivery cost on buyer
                if (shipment.DeliveryCostType == DeliveryCostType.DeliveryCostOnBuyer && shipment.DeliveryCost > 0)
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Delivery cost on buyer", debit: shipment.DeliveryCost, credit: 0, shipmentId: shipment.Id, updateRecord: true);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Mark.as.delivered.success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("purchase-order-mark-as-shipped")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderMarkAsShipped([FromBody] OrderManagementApiModel.MaskAsShippedModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (model.ShipmentId <= 0)
                return Ok(new { success = false, message = "Shipment id is required" });

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.ShipmentId);
            if (shipment is null)
                return Ok(new { success = false, message = "Shipment not found" });

            try
            {
                var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
                if (order is null)
                    return Ok(new { success = false, message = "Order not found" });

                var request = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (request == null)
                    return Ok(new { success = false, message = "Request not found" });

                var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
                if (orderItem == null)
                    return Ok(new { success = false, message = "Order item not found" });

                var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault(x => x.OrderItemId == orderItem.Id);
                if (shipmentItem == null)
                    return Ok(new { success = false, message = "Shipment item not found" });

                if (model.TransporterId <= 0)
                    return Ok(new { success = false, message = "Transporter id is required" });

                if (model.VehicleId <= 0)
                    return Ok(new { success = false, message = "Vehicle id is required" });

                if (model.VehicleId > 0 && string.IsNullOrWhiteSpace(model.VehicleNumber))
                    return Ok(new { success = false, message = "Vehicle number is required" });

                if (model.RouteTypeId <= 0)
                    return Ok(new { success = false, message = "RouteType id is required" });

                if (model.ActualShippedQuantity < shipmentItem.Quantity && string.IsNullOrWhiteSpace(model.ActualShippedQuantityReason))
                    return Ok(new { success = false, message = "Actual shipped quantity reason is required" });

                shipment.TransporterId = model.TransporterId;
                shipment.WarehouseId = model.WarehouseId;
                shipment.VehicleId = model.VehicleId;
                shipment.VehicleNumber = model.VehicleNumber;
                shipment.RouteTypeId = model.RouteTypeId;
                shipment.DeliveryStatusId = (int)DeliveryStatus.EnRoute;
                shipment.PickupAddress = model.PickupAddress;
                shipment.OnLabourCharges = model.Laborcharges;
                shipment.ShippedDateUtc = DateTime.UtcNow;
                shipment.CostOnZarayeId = model.CostOnZarayeId;
                shipment.ActualShippedQuantity = model.ActualShippedQuantity;
                shipment.ActualShippedQuantityReason = model.ActualShippedQuantityReason;
                await _shipmentService.UpdateShipmentAsync(shipment);

                if (!string.IsNullOrWhiteSpace(model.ActualShippedQuantityReason) && model.ActualShippedQuantity < shipmentItem.Quantity)
                {
                    shipmentItem.Quantity = model.ActualShippedQuantity;
                    await _shipmentService.UpdateShipmentItemAsync(shipmentItem);

                    //update delivery request
                    var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                    if (deliveryRequest is not null)
                    {
                        deliveryRequest.Quantity = model.ActualShippedQuantity;
                        await _orderService.UpdateOrderDeliveryRequestAsync(deliveryRequest);
                    }
                }
                //Update inventory inbound to 'InTransit'
                var inventoryInbound = _inventoryService.GetInventoryInboundByShipmentId(shipment.Id);
                if (inventoryInbound != null)
                {
                    inventoryInbound.InventoryInboundStatusEnum = InventoryInboundStatusEnum.InTransit;
                    await _inventoryService.UpdateInventoryInboundAsync(inventoryInbound);
                }

                var allTransporterLedger = await _customerLedgerService.GetAllTransporterLedgerByShipmentAsync(shipment.Id, new string[] { "Loading charges" });
                if (allTransporterLedger.Any())
                {
                    foreach (var ledger in allTransporterLedger)
                        await _customerLedgerService.DeleteCustomerLedgerAsync(ledger);
                }

                if (model.Laborcharges > 0)
                {
                    var transpoter = await _customerService.GetCustomerByIdAsync(model.TransporterId);
                    if (transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.Yes)
                    {
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                    }
                    else if (transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.No)
                    {
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                    }
                    else if (!transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.Yes)
                    {
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                    }
                    else if (!transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.No)
                    {
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                    }
                }

                //check whether we have more items to ship
                if (await _orderService.HasItemsToAddToShipmentAsync(order) || await _orderService.HasItemsToShipAsync(order))
                    order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
                else
                    order.ShippingStatusId = (int)ShippingStatus.Shipped;
                await _orderService.UpdateOrderAsync(order);


                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Mark.as.shipped.success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("purchase-order-mark-as-delivered")]
        public async Task<IActionResult> OrderManagement_PurchaseOrderMarkAsDelivered([FromBody] OrderManagementApiModel.MaskAsDeliveredModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return Ok(new { success = false, message = "user not found" });

            if (model.ShipmentId <= 0)
                return Ok(new { success = false, message = "Shipment id is required" });

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.ShipmentId);
            if (shipment is null)
                return Ok(new { success = false, message = "Shipment not found" });

            var purchaseOrder = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (purchaseOrder == null)
                return Ok(new { success = false, message = "Purchase order not found" });

            var purchaseOrderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(shipment.OrderId);
            if (purchaseOrderCalculation == null)
                return Ok(new { success = false, message = "Purchase order calculation not found" });

            var quotation = await _quotationService.GetQuotationByIdAsync(purchaseOrder.QuotationId.Value);
            if (quotation == null)
                return Ok(new { success = false, message = "Quotation not found" });

            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault();
            if (shipmentItem == null)
                return Ok(new { success = false, message = "Shipment item not found" });

            if (model.ActualDeliveredQuantity < shipmentItem.Quantity && string.IsNullOrWhiteSpace(model.ActualDeliveredQuantityReason))
                return Ok(new { success = false, message = "Actual delivered quantity reason is required" });

            try
            {
                if (model.DeliveryTypeId <= 0)
                    throw new Exception("Select delivery type");
                if (model.TransporterTypeId <= 0)
                    throw new Exception("Select transporter type");
                if (model.DeliveryTimingId == 0)
                    throw new Exception("Select delivery time");
                if (model.DeliveryTimingId == (int)DeliveryTiming.Delayed && model.DeliveryDelayedReasonId == 0)
                    throw new Exception("Select delivery time reason");
                if (string.IsNullOrWhiteSpace(model.FileName))
                    throw new Exception("Picture is required");

                var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);

                //sum both fileds value then check is > 0
                var deliveryCost = shipment.OnLabourCharges + model.FreightCharges + model.LabourCharges;

                if (model.DeliveryCostTypeId != (int)DeliveryCostType.FreeDelivery && model.DeliveryCostTypeId != (int)DeliveryCostType.BuyerSelfPickup && deliveryCost <= 0)
                    throw new Exception("Enter delivery cost");

                if (shipment.ExpectedDeliveryCost > 0 && deliveryCost > shipment.ExpectedDeliveryCost && model.DeliveryCostReasonId == 0)
                    throw new Exception("Select delivery cost reason");

                if (model.DeliveryCostTypeId == (int)DeliveryCostType.FreeDelivery || model.DeliveryCostTypeId == (int)DeliveryCostType.BuyerSelfPickup)
                {
                    model.FreightCharges = 0;
                    model.LabourCharges = 0;
                    deliveryCost = 0;
                }

                shipment.PictureId = model.FileName != null ? await OrderManagement_UploadPicture(model.ImageBytes, model.FileName) : 0;
                shipment.DeliveryCostTypeId = model.DeliveryCostTypeId;
                shipment.DeliveryTypeId = model.DeliveryTypeId;
                shipment.TransporterTypeId = model.TransporterTypeId;
                shipment.DeliveryCost = deliveryCost;
                shipment.FreightCharges = model.FreightCharges;
                shipment.LabourCharges = model.LabourCharges;
                shipment.DeliveryTimingId = model.DeliveryTimingId;
                shipment.DeliveryDelayedReasonId = model.DeliveryDelayedReasonId;
                shipment.DeliveryCostReasonId = model.DeliveryCostReasonId;
                shipment.ShipmentDeliveryAddress = model.ShipmentDeliveryAddress;
                shipment.LabourTypeId = model.LabourTypeId;
                shipment.WarehouseId = model.WarehouseId;
                shipment.ActualDeliveredQuantity = model.ActualDeliveredQuantity;
                shipment.ActualDeliveredQuantityReason = model.ActualDeliveredQuantityReason;
                await _shipmentService.UpdateShipmentAsync(shipment);

                var allTransporterLedger = await _customerLedgerService.GetAllTransporterLedgerByShipmentAsync(shipment.Id, new string[] { "Frieght charges", "UnLoading charges" });
                if (allTransporterLedger.Any())
                {
                    foreach (var ledger in allTransporterLedger)
                        await _customerLedgerService.DeleteCustomerLedgerAsync(ledger);
                }

                if (/*shipment.DeliveryCostType != DeliveryCostType.FulfilledBySupplier &&*/ shipment.DeliveryCostType != DeliveryCostType.FreeDelivery)
                {
                    var transpoter = await _customerService.GetCustomerByIdAsync(shipment.TransporterId);
                    if (transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.Yes)
                    {
                        if (shipment.FreightCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                        if (shipment.LabourCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                    }
                    else if (!transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.Yes)
                    {
                        if (shipment.FreightCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: transpoter.Id, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);
                        //await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id); Align to khurram


                        if (shipment.LabourCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                    }
                    else if (transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.No)
                    {
                        if (shipment.FreightCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                        if (shipment.LabourCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                    }
                    else if (!transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.No)
                    {
                        if (shipment.FreightCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                        if (shipment.LabourCharges > 0)
                            await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                    }
                }

                //Update inventoru inbound to 'Physical'
                var inventoryInbound = _inventoryService.GetInventoryInboundByShipmentId(shipment.Id);
                if (inventoryInbound != null)
                {
                    inventoryInbound.InventoryInboundStatusEnum = InventoryInboundStatusEnum.Physical;
                    await _inventoryService.UpdateInventoryInboundAsync(inventoryInbound);
                }

                if (!string.IsNullOrWhiteSpace(model.ActualDeliveredQuantityReason) && model.ActualDeliveredQuantity < shipmentItem.Quantity)
                {
                    shipmentItem.Quantity = model.ActualDeliveredQuantity;
                    await _shipmentService.UpdateShipmentItemAsync(shipmentItem);

                    //update delivery request
                    var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                    if (deliveryRequest is not null)
                    {
                        deliveryRequest.Quantity = model.ActualDeliveredQuantity;
                        await _orderService.UpdateOrderDeliveryRequestAsync(deliveryRequest);
                    }
                }

                await _orderProcessingService.DeliverAsync(shipment, true);

                #region Deliver Ledger

                //Add Customer Ledger for supplier delivery
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Delivery", debit: 0, credit: shipment.DeliveredAmount, shipmentId: shipment.Id);

                #endregion

                var calculatedPayableToMiLL = (decimal)(purchaseOrderCalculation.PayableToMill / quotation.Quantity) * shipmentItem.Quantity;

                #region Finance Cost

                if (purchaseOrderCalculation.FinanceCost > 0)
                {
                    if (purchaseOrderCalculation.FinanceCostPayment == (int)FinanceCostPaymentStatus.Cash)
                    {
                        var calculatedFinanceCostValue = (purchaseOrderCalculation.FinanceCost / quotation.Quantity) * shipmentItem.Quantity;
                        calculatedPayableToMiLL = calculatedPayableToMiLL - calculatedFinanceCostValue;
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Finance Cost", debit: 0, credit: calculatedFinanceCostValue, shipmentId: shipment.Id);
                    }
                    else
                    {
                        var calculatedFinanceCost = ((purchaseOrderCalculation.FinanceCost / quotation.Quantity) * ((purchaseOrderCalculation.GSTRate / 100) + 1) * (1 - (purchaseOrderCalculation.WHTRate / 100)) * ((purchaseOrderCalculation.WholesaleTaxRate / 100) + 1));
                        calculatedFinanceCost = calculatedFinanceCost * shipmentItem.Quantity;
                        calculatedPayableToMiLL = calculatedPayableToMiLL - calculatedFinanceCost;
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Finance Cost", debit: 0, credit: calculatedFinanceCost, shipmentId: shipment.Id);
                    }
                }

                #endregion

                if (purchaseOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Standard)
                {
                    #region Broker Cash Ledger

                    if (purchaseOrderCalculation.BrokerId > 0)
                    {
                        //var calculatedPayableToMiLL = (decimal)(purchaseOrderCalculation.PayableToMill / quotation.Quantity) * shipmentItem.Quantity;
                        var calculatedPaymentInCash = (decimal)(purchaseOrderCalculation.PaymentInCash / quotation.Quantity) * shipmentItem.Quantity;
                        //New Condition
                        if (purchaseOrderCalculation.BrokerId == purchaseOrder.CustomerId)
                        {
                            //Supplier record
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Payable to mill", debit: 0, credit: calculatedPayableToMiLL, shipmentId: shipment.Id);
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Payment in cash", debit: 0, credit: calculatedPaymentInCash, shipmentId: shipment.Id);
                        }
                        else if (purchaseOrderCalculation.BrokerId != purchaseOrder.CustomerId)
                        {
                            //Supplier record
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Payable to mill", debit: 0, credit: calculatedPayableToMiLL, shipmentId: shipment.Id);

                            //Broker record
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.BrokerId, description: "Payment in cash", debit: 0, credit: calculatedPaymentInCash, shipmentId: shipment.Id);
                        }

                        var calculatedBrokerCash = purchaseOrderCalculation.BrokerCash * shipmentItem.Quantity;
                        if (purchaseOrderCalculation.BrokerCash > 0)
                        {
                            //Add Customer Ledger for positive broker cash
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.BrokerId, description: "Broker Cash", debit: 0, credit: calculatedBrokerCash, shipmentId: shipment.Id);
                        }
                        else if (purchaseOrderCalculation.BrokerCash < 0)
                        {
                            //Add Customer Ledger for negative broker cash
                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.BrokerId, description: "Broker Cash", debit: Math.Abs(calculatedBrokerCash), credit: 0, shipmentId: shipment.Id);
                        }
                    }
                    else if (purchaseOrderCalculation.BrokerId == 0 && purchaseOrder.CustomerId > 0)
                    {
                        //Supplier record
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Payable to mill", debit: 0, credit: calculatedPayableToMiLL, shipmentId: shipment.Id);
                    }
                    #endregion
                }
                #region Supplier Commission Receivable Ledger

                if (purchaseOrderCalculation.SupplierCommissionReceivableUserId > 0 && purchaseOrderCalculation.SupplierCommissionReceivableAmount > 0)
                {
                    var calculatedSupplierCommissionReceivablePerBag = purchaseOrderCalculation.SupplierCommissionReceivableAmount * shipmentItem.Quantity;

                    //Add Customer Ledger for Supplier Commission Receivable
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.SupplierCommissionReceivableUserId, description: "Supplier Commission Receivable", debit: calculatedSupplierCommissionReceivablePerBag, credit: 0, shipmentId: shipment.Id);
                }

                #endregion

                #region Supplier Commission Payable Ledger

                if (purchaseOrderCalculation.SupplierCommissionPayableUserId > 0 && purchaseOrderCalculation.SupplierCommissionBag > 0)
                {
                    var calculatedSupplierCommissionPayablePerBag = purchaseOrderCalculation.SupplierCommissionBag * shipmentItem.Quantity;

                    //Add Customer Ledger for Supplier Commission Payable
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.SupplierCommissionPayableUserId, description: "Supplier Commission Payable", debit: 0, credit: calculatedSupplierCommissionPayablePerBag, shipmentId: shipment.Id);
                }

                #endregion

                //Add Customer Ledger for supplier delivery
                //await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Delivery", debit: 0, credit: shipment.DeliveredAmount, shipmentId: shipment.Id);

                //Add customer ledger for delivery cost on supplier
                if (shipment.DeliveryCostType == DeliveryCostType.FulfilledBySupplier && shipment.DeliveryCost > 0)
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Delivery cost on supplier", debit: 0, credit: shipment.DeliveryCost, shipmentId: shipment.Id, updateRecord: true);

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Mark.as.delivered.success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-transporters")]
        public async Task<IActionResult> OrderManagement_GetAllTransporters()
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var transporters = await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.TransporterRoleName)).Id });

                var result = new List<SelectListItem>();
                //clone the list to ensure that "selected" property is not set
                foreach (var item in transporters)
                {
                    result.Add(new SelectListItem
                    {
                        Text = item.FullName,
                        Value = item.Id.ToString()
                    });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-vehicles-by-transporter/{transporterId}")]
        public async Task<IActionResult> OrderManagement_GetVehiclesByTransporterId(int transporterId)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "User not found" });

                var transporter = await _customerService.GetCustomerByIdAsync(transporterId);
                if (transporter is null)
                    return Ok(new { success = false, message = "Transporter not found" });

                var result = new List<SelectListItem>();
                var TransportVehiclelist = (await _customerService.GetTransporterVehicleByTransporterIdAsync(Convert.ToInt32(transporter.Id))).ToList();

                //clone the list to ensure that "selected" property is not set
                foreach (var item in TransportVehiclelist)
                {
                    result.Add(new SelectListItem
                    {
                        Text = $"{_customerService.GetVehiclePortfolioByIdAsync(item.VehicleId).Result.Name}",
                        Value = item.Id.ToString()
                    });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}