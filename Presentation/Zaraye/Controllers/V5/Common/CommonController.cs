using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.PriceDiscovery;
using Zaraye.Services;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.Configuration;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.Helpers;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Media;
using Zaraye.Services.Messages;
using Zaraye.Services.Orders;
using Zaraye.Services.PriceDiscovery;
using Zaraye.Services.Shipping;
using System.Globalization;
using System.Text;
using System.Xml;
using Zaraye.Models.Api.V5.Common;
using Zaraye.Models.Api.V5.OrderManagement;

namespace Zaraye.Controllers.V5.Security
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("5")]
    [Route("v{version:apiVersion}/common")]
    public class CommonController : BaseApiController
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

        #endregion

        #region Ctor

        public CommonController(
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
            ISettingService settingService)
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
        }

        #endregion

        #region Utilities

        [NonAction]
        public async Task<string> Common_ConvertToXmlAsync(List<CommonApiModel.CombinationAttributeApiModel> attributeDtos, int productId)
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

        #endregion

        #region Country / Cities / Areas

        [HttpGet("all-countries")]
        public async Task<IActionResult> AllCountries()
        {
            try
            {
                var data = (await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id, isAppPublished: true)).Select(async c =>
                {
                    return new
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-states/{countryId}")]
        public async Task<IActionResult> AllStates(int countryId)
        {
            if (countryId <= 0)
                return Ok(new { success = false, message = "Country id is required" });

            try
            {
                var data = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(countryId, (await _workContext.GetWorkingLanguageAsync()).Id)).Select(async c =>
                {
                    return new
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-areas/{cityId}")]
        public async Task<IActionResult> AllAreas(int cityId)
        {
            if (cityId <= 0)
                return Ok(new { success = false, message = "City id is required" });

            try
            {
                var data = (await _stateProvinceService.GetAllAreasByCityIdAsync(cityId)).Select(async c =>
                {
                    return new
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
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
                var product = await _productService.GetProductByIdAsync(productId);
                if (product is null)
                    return Ok(new { success = false, message = "Product not found" });

                var unit = await _measureService.GetMeasureWeightByIdAsync(product.UnitId);
                if (unit is null)
                    return Ok(new { success = false, message = "Unit not found" });

                return Ok(new { success = true, data = new { unitId = unit.Id, unitName = unit.Name } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-brands/{productId}")]
        public async Task<IActionResult> AllBrands(int productId)
        {
            try
            {
                var data = (await _manufacturerService.GetAllBrandsByProductIdAsync(productId, isAppPublished: true)).Select(async i =>
                {
                    return new
                    {
                        Text = await _localizationService.GetLocalizedAsync(i, x => x.Name),
                        Value = i.Id.ToString()
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-industries")]
        public async Task<IActionResult> AllIndustries(string type = null)
        {
            try
            {
                var data = (await _industryService.GetAllIndustriesAsync("", (await _storeContext.GetCurrentStoreAsync()).Id, isAppPublished: true)).Select(async i =>
                {
                    var categoryIds = new List<int>();

                    if (type != "Request")
                        categoryIds.AddRange((await _categoryService.GetAllCategoriesByIndustryIdAsync(i.Id, isAppPublished: true)).Select(x => x.Id));

                    return new
                    {
                        Text = await _localizationService.GetLocalizedAsync(i, x => x.Name),
                        Value = i.Id.ToString(),
                        PictureUrl = await _pictureService.GetPictureUrlAsync(i.PictureId > 0 ? i.PictureId : 0, 0, showDefaultPicture: true),
                        NumberOfProducts = type == "Request" ?
                        (await _requestService.GetAllRequestAsync(bsIds: new List<int> { (int)RequestStatus.Verified }, industryId: i.Id, getOnlyTotalCount: true)).TotalCount :
                        await _productService.GetNumberOfProductsInCategoryAsync(categoryIds, (await _storeContext.GetCurrentStoreAsync()).Id)
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-categories/{industryId}")]
        public async Task<IActionResult> AllCategories(int industryId, bool showNumberOfProducts = true)
        {
            if (industryId <= 0)
                return Ok(new { success = false, message = "Industry id is required" });

            try
            {
                var data = (await _categoryService.GetAllCategoriesByIndustryIdAsync(industryId, isAppPublished: true)).Select(async c =>
                {
                    var categoryIds = new List<int> { c.Id };
                    categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(c.Id, (await _storeContext.GetCurrentStoreAsync()).Id));

                    return new
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        PictureUrl = await _pictureService.GetPictureUrlAsync(c.PictureId > 0 ? c.PictureId : 0, 0, showDefaultPicture: true),
                        NumberOfProducts = showNumberOfProducts ? await _productService.GetNumberOfProductsInCategoryAsync(categoryIds, (await _storeContext.GetCurrentStoreAsync()).Id) : 0
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("categories-and-products/{industryId}")]
        public async Task<IActionResult> CategoriesAndProductsByIndustryId(int industryId)
        {
            if (industryId <= 0)
                return Ok(new { success = false, message = "Industry id is required" });

            try
            {
                List<object> data = new List<object>();

                var categories = await _categoryService.GetAllCategoriesByIndustryIdAsync(industryId, /*overridePublished: false,*/ isAppPublished: true);
                foreach (var category in categories)
                {
                    data.Add(new
                    {
                        CategoryId = category.Id,
                        CategoryName = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                        Products = (await _productService.GetAllProductsByCategoryIdAsync(category.Id, /*overridePublished: false,*/ isAppPublished: true)).Select(async p =>
                        {
                            return new
                            {
                                ProductId = p.Id.ToString(),
                                ProductName = await _localizationService.GetLocalizedAsync(p, x => x.Name)
                            };
                        }).Select(t => t.Result).ToList()
                    });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-products")]
        public async Task<IActionResult> AllProducts(int categoryId = 0, List<int> categoryIds = null)
        {
            //if (categoryId <= 0)
            //    return Ok(new { success = false, message = "Category id is required" });

            try
            {
                if (categoryId > 0)
                {
                    var data = (await _productService.GetAllProductsByCategoryIdAsync(categoryId, /*overridePublished: false,*/ isAppPublished: true/*, brandId*/)).Select(async c =>
                    {
                        return new
                        {
                            Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                            Value = c.Id.ToString(),
                        };
                    }).Select(t => t.Result).ToList();

                    return Ok(new { success = true, data });
                }

                if (categoryIds != null && categoryIds.Any())
                {
                    var data = (await _productService.GetAllProductsByCategoryIdsAsync(categoryIds.ToArray())).Select(async c =>
                    {
                        return new
                        {
                            Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                            Value = c.Id.ToString(),
                        };
                    }).Select(t => t.Result).ToList();

                    return Ok(new { success = true, data });
                }

                return Ok(new { success = false });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-products/{categoryId}")]
        public async Task<IActionResult> AllProducts(int categoryId = 0)
        {
            if (categoryId <= 0)
                return Ok(new { success = false, message = "Category id is required" });

            try
            {
                var data = (await _productService.GetAllProductsByCategoryIdAsync(categoryId, /*overridePublished: false,*/ isAppPublished: true)).Select(async c =>
                {
                    return new
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("product-attributes/{productId}")]
        public async Task<IActionResult> AllProductAttributes(int productId, string attributesXml = "")
        {
            if (productId <= 0)
                return Ok(new { success = false, message = "product id is required" });

            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null || product.Deleted)
                    return Ok(new { success = false, message = "product not found" });

                //var parseAttributes = await ParseAttributeXml(attributesXml);

                //var data = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id)).Select(async attribute =>
                //{
                //    var productAttrubute = await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);

                //    return new CommonApiModel.ProductAttributesApiModel
                //    {
                //        Id = attribute.Id,
                //        ProductId = product.Id,
                //        ProductAttributeId = attribute.ProductAttributeId,
                //        Name = string.IsNullOrWhiteSpace(attribute.TextPrompt) ? await _localizationService.GetLocalizedAsync(productAttrubute, x => x.Name) : await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                //        Description = await _localizationService.GetLocalizedAsync(productAttrubute, x => x.Description),
                //        TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                //        IsRequired = attribute.IsRequired,
                //        AttributeControlType = (int)attribute.AttributeControlType,
                //        DefaultValue = await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue),
                //        ValidationMinLength = attribute.ValidationMinLength,
                //        ValidationMaxLength = attribute.ValidationMaxLength,
                //        HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml),
                //        Values = (await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id)).Select(async attributeValue => new CommonApiModel.ProductAttributesApiModel.ProductAttributeValueApiModel
                //        {
                //            Id = attributeValue.Id,
                //            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                //            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                //            //IsPreSelected = !parseAttributes.Any() ? attributeValue.IsPreSelected : parseAttributes.Any(x => x.AttributeId == attribute.ProductAttributeId && x.ValueId == attributeValue.Id && x.AttributeControlTypeId == attribute.AttributeControlTypeId),
                //            IsPreSelected = attributeValue.IsPreSelected,
                //            CustomerEntersQty = attributeValue.CustomerEntersQty,
                //            Quantity = attributeValue.Quantity
                //        }).Select(t => t.Result).ToList()
                //    };
                //}).Select(t => t.Result).ToList();

                var data = new List<CommonApiModel.ProductAttributesApiModel>();

                var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                foreach (var attribute in productAttributeMapping)
                {
                    var productAttrubute = await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);

                    var attributeModel = new CommonApiModel.ProductAttributesApiModel
                    {
                        Id = attribute.Id,
                        ProductId = product.Id,
                        ProductAttributeId = attribute.ProductAttributeId,
                        Name = string.IsNullOrWhiteSpace(attribute.TextPrompt) ? await _localizationService.GetLocalizedAsync(productAttrubute, x => x.Name) : await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                        Description = await _localizationService.GetLocalizedAsync(productAttrubute, x => x.Description),
                        TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                        IsRequired = attribute.IsRequired,
                        AttributeControlType = (int)attribute.AttributeControlType,
                        DefaultValue = attributesXml != null ? null : await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue),
                        ValidationMinLength = attribute.ValidationMinLength,
                        ValidationMaxLength = attribute.ValidationMaxLength,
                        HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml),
                    };

                    if (attribute.ShouldHaveValues())
                    {
                        var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                        foreach (var attributeValue in attributeValues)
                        {
                            var valueModel = new CommonApiModel.ProductAttributesApiModel.ProductAttributeValueApiModel
                            {
                                Id = attributeValue.Id,
                                Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                                ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                                IsPreSelected = attributeValue.IsPreSelected,
                                CustomerEntersQty = attributeValue.CustomerEntersQty,
                                Quantity = attributeValue.Quantity
                            };
                            attributeModel.Values.Add(valueModel);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(attributesXml))
                    {
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlType.DropdownList:
                            case AttributeControlType.RadioList:
                            case AttributeControlType.Checkboxes:
                            case AttributeControlType.ColorSquares:
                            case AttributeControlType.ImageSquares:
                                {
                                    if (!string.IsNullOrEmpty(attributesXml))
                                    {
                                        //clear default selection
                                        foreach (var item in attributeModel.Values)
                                            item.IsPreSelected = false;

                                        //select new values
                                        var selectedValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
                                        foreach (var attributeValue in selectedValues)
                                            foreach (var item in attributeModel.Values)
                                                if (attributeValue.Id == item.Id)
                                                {
                                                    item.IsPreSelected = true;

                                                    //set customer entered quantity
                                                    if (attributeValue.CustomerEntersQty)
                                                        item.Quantity = attributeValue.Quantity;
                                                }
                                    }
                                }

                                break;
                            case AttributeControlType.ReadonlyCheckboxes:
                                {
                                    //values are already pre-set

                                    //set customer entered quantity
                                    if (!string.IsNullOrEmpty(attributesXml))
                                    {
                                        foreach (var attributeValue in (await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml))
                                            .Where(value => value.CustomerEntersQty))
                                        {
                                            var item = attributeModel.Values.FirstOrDefault(value => value.Id == attributeValue.Id);
                                            if (item != null)
                                                item.Quantity = attributeValue.Quantity;
                                        }
                                    }
                                }

                                break;
                            case AttributeControlType.TextBox:
                            case AttributeControlType.NumericTextBox:
                            case AttributeControlType.MultilineTextbox:
                                {
                                    if (!string.IsNullOrEmpty(attributesXml))
                                    {
                                        var enteredText = _productAttributeParser.ParseValues(attributesXml, attribute.Id);
                                        if (enteredText.Any())
                                            attributeModel.DefaultValue = enteredText[0];
                                    }
                                }

                                break;
                            case AttributeControlType.Datepicker:
                                {
                                    //keep in mind my that the code below works only in the current culture
                                    var selectedDateStr = _productAttributeParser.ParseValues(attributesXml, attribute.Id);
                                    if (selectedDateStr.Any())
                                    {
                                        if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture, DateTimeStyles.None, out var selectedDate))
                                        {
                                            //successfully parsed
                                            attributeModel.SelectedDay = selectedDate.Day;
                                            attributeModel.SelectedMonth = selectedDate.Month;
                                            attributeModel.SelectedYear = selectedDate.Year;
                                        }
                                    }
                                }

                                break;
                            case AttributeControlType.FileUpload:
                                {
                                    if (!string.IsNullOrEmpty(attributesXml))
                                    {
                                        var downloadGuidStr = _productAttributeParser.ParseValues(attributesXml, attribute.Id).FirstOrDefault();
                                        Guid.TryParse(downloadGuidStr, out var downloadGuid);
                                        var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                                        if (download != null)
                                            attributeModel.DefaultValue = download.DownloadGuid.ToString();
                                    }
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    data.Add(attributeModel);
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("product-attribute-change/{productId}")]
        public async Task<IActionResult> ProductAttributeChange(int productId, [FromBody] List<CommonApiModel.CombinationAttributeApiModel> attributesData)
        {
            if (productId <= 0)
                return Ok(new { success = false, message = "product id is required" });

            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null || product.Deleted)
                    return Ok(new { success = false, message = "product not found" });

                var attributeXml = await Common_ConvertToXmlAsync(attributesData, product.Id);

                //conditional attributes
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

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        productId,
                        enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                        disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-app-versions")]
        public async Task<IActionResult> AllAppVersions(string platform = "", string type = "Consumer App")
        {
            try
            {
                var data = (await _appVersionService.GetAllAppVersionsAsync(type: type, platform: platform, onlyForceUpdate: true, pageIndex: 0, pageSize: 1)).Select(i =>
                {
                    return new
                    {
                        Version = i.Version
                    };
                }).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-buyer-types")]
        public IActionResult AllBuyerTypes()
        {
            try
            {
                List<object> data = new List<object>();

                var buyerTypes = _customerService.GetAllUserTypesAsync(type: "Buyer").Result;
                foreach (var type in buyerTypes)
                {
                    data.Add(new
                    {
                        Text = type.Name,
                        Value = type.Id.ToString()
                    });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-supplier-types")]
        public IActionResult AllSupplierTypes()
        {
            try
            {
                List<object> data = new List<object>();

                var supplierTypes = _customerService.GetAllUserTypesAsync(type: "Supplier").Result;
                foreach (var type in supplierTypes)
                {
                    data.Add(new
                    {
                        Text = type.Name,
                        Value = type.Id.ToString()
                    });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
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
                if (string.IsNullOrEmpty(resourceName))
                    return Ok(new { success = false, message = "Resource name is required" });

                if (languageId <= 0)
                    return Ok(new { success = false, message = "Language id is required" });

                var JSONString = new StringBuilder();

                //var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

                //get locale resources
                var localeResources = (await _localizationService.GetAllResourceValuesAsync(languageId, loadPublicLocales: null))
                    .OrderBy(localeResource => localeResource.Key).AsQueryable();

                //filter locale resources
                if (!string.IsNullOrEmpty(resourceName))
                    localeResources = localeResources.Where(l => l.Key.ToLowerInvariant().Contains(resourceName.ToLowerInvariant()));

                var listLocaleResources = await localeResources.ToListAsync();

                JSONString.Append("{");
                int count = 0;
                foreach (var dr in listLocaleResources)
                {
                    count++;
                    if (count == listLocaleResources.Count)
                    {
                        JSONString.Append("\"" + dr.Key.ToString() + "\":" + "\"" + dr.Value.Value.Replace("\"", "").Replace(@"\", "-").ToString() + "\"}");
                    }
                    else
                    {
                        JSONString.Append("\"" + dr.Key.ToString() + "\":" + "\"" + dr.Value.Value.Replace("\"", "").Replace(@"\", "-").ToString() + "\",");
                    }
                }

                JSONString = JSONString.Replace(@"?", "");
                JSONString = JSONString.Replace(@"!", "");

                JSONString = JSONString.Replace('"', '\"');

                JToken token = JObject.Parse(JSONString.ToString());

                //var data = listLocaleResources.Select(localeResource =>
                //{
                //    return new
                //    {
                //        Id = localeResource.Value.Key,
                //        ResourceName = localeResource.Key,
                //        ResourceValue = localeResource.Value.Value
                //    };
                //}).ToList();

                return Ok(new { success = true, data = token });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-languages")]
        public async Task<IActionResult> AllLanguages()
        {
            try
            {
                var data = (await _languageService.GetAllLanguagesAsync()).Select(l =>
                {
                    return new
                    {
                        Id = l.Id,
                        Name = l.Name,
                        LanguageCulture = l.LanguageCulture,
                        UniqueSeoCode = l.UniqueSeoCode,
                        FlagImageFileName = l.FlagImageFileName,
                        Rtl = l.Rtl
                    };
                }).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("set-language/{languageId}")]
        public async Task<IActionResult> SetLanguage(int languageId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);
            if (language == null || !language.Published)
                return Ok(new { success = false, message = "Language not found" });

            await _workContext.SetWorkingLanguageAsync(language);

            return Ok(new { success = true, message = "" });
        }

        #endregion

        #region Buyer Request Reject

        [HttpGet("rejected-reasons")]
        public IActionResult AllRejectedReasons()
        {
            try
            {
                var reasons = new List<SelectListItem>();

                reasons.Add(new SelectListItem { Value = "Sales Return", Text = "Sales Return" });
                reasons.Add(new SelectListItem { Value = "Tax Compliance Issue", Text = "Tax Compliance Issue" });
                reasons.Add(new SelectListItem { Value = "Availability Issue", Text = "Availability Issue" });
                reasons.Add(new SelectListItem { Value = "Price Issue (Cheaper prices available in Market)", Text = "Price Issue (Cheaper prices available in Market)" });
                reasons.Add(new SelectListItem { Value = "Credit Issue (20+)", Text = "Credit Issue (20+)" });
                reasons.Add(new SelectListItem { Value = "Delivery Issue", Text = "Delivery Issue" });
                reasons.Add(new SelectListItem { Value = "Customer side issue", Text = "Customer side issue" });
                reasons.Add(new SelectListItem { Value = "No response", Text = "No response" });
                reasons.Add(new SelectListItem { Value = "Late Response", Text = "Late Response" });
                reasons.Add(new SelectListItem { Value = "Financial Constraints", Text = "Financial Constraints" });
                reasons.Add(new SelectListItem { Value = "Customer Old Payment Due", Text = "Customer Old Payment Due" });
                reasons.Add(new SelectListItem { Value = "Market Volatility", Text = "Market Volatility" });
                reasons.Add(new SelectListItem { Value = "Wrong Quantity Entered", Text = "Wrong Quantity Entered" });
                reasons.Add(new SelectListItem { Value = "Wrong Product Selected", Text = "Wrong Product Selected" });
                reasons.Add(new SelectListItem { Value = "Duplicate Entry", Text = "Duplicate Entry" });
                reasons.Add(new SelectListItem { Value = "Dummy Entry", Text = "Dummy Entry" });
                reasons.Add(new SelectListItem { Value = "Incorrect Information", Text = "Incorrect Information" });
                reasons.Add(new SelectListItem { Value = "Incorrect Information & Duplicate order", Text = "Incorrect Information & Duplicate order" });
                reasons.Add(new SelectListItem { Value = "Low demand", Text = "Low demand" });
                reasons.Add(new SelectListItem { Value = "Financial Constraint (Non GST)", Text = "Financial Constraint (Non GST)" });
                reasons.Add(new SelectListItem { Value = "Other", Text = "Other" });

                return Ok(new { success = true, data = reasons });
            }
            catch (Exception ex)
            {
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
                var businessModules = new List<object>();

                var availableStatusItems = await BusinessModelEnum.Standard.ToSelectListAsync(false);

                foreach (var statusItem in availableStatusItems)
                {
                    businessModules.Add(statusItem);
                }

                return Ok(new { success = true, data = businessModules });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-modofpayment")]
        public async Task<IActionResult> AllModeOfPayment()
        {
            try
            {
                var modofpayment = new List<object>();
                var availableStatusItems = await ModeOfPayment.Bank.ToSelectListAsync(false);
                foreach (var item in availableStatusItems)
                    modofpayment.Add(new SelectListItem { Value = item.Value, Text = item.Text });

                return Ok(new { success = true, data = modofpayment });
            }
            catch (Exception ex)
            {
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
                var user = await _workContext.GetCurrentCustomerAsync();
                var data = await (await _priceDiscoveryService.GetGetTodayRatesListAsync(statusId: (int)DailyRateEnum.Approve, published: true)).SelectAwait(async r =>
                {
                    var picture = (await _productService.GetProductPicturesByProductIdAsync(r.ProductId))?.FirstOrDefault();
                    var rate = await GetFinalRate(r.Id, user, r.SupplierId, r.Rate, r.CategoryId, r.IncludeFirstMile);

                    var formatPrice = await _priceFormatter.FormatPriceAsync(rate, true, false);
                    if (r.IncludeGst)
                        formatPrice = $"{formatPrice}+";

                    List<decimal> previousRates = new List<decimal>();
                    if (!string.IsNullOrWhiteSpace(r.PreviousRates))
                        previousRates = r.PreviousRates.Split(',').Select(decimal.Parse).ToList();

                    return new
                    {
                        r.CategoryId,
                        r.IndustryId,
                        industry = r.IndustryName,
                        r.BrandId,
                        brand = r.BrandName,
                        r.ProductId,
                        product = r.ProductName,
                        sku = r.ProductSku,
                        picture = (await _pictureService.GetPictureUrlAsync(picture != null ? picture.PictureId : 0)),
                        formatPrice = formatPrice,
                        price = rate,
                        createdOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(r.CreatedOnUtc, DateTimeKind.Utc)).ToString(),
                        isFavourite = user == null ? false : (await _priceDiscoveryService.GetFavouriteRateGroupAsync(user.Id, r.ProductId, r.AttributeValueId, r.BrandId)) is null ? false : true,
                        ProductAttributes = await ParseAttributeXml(r.AttributeXml),
                        productattributesxml = r.AttributeXml,
                        attributeValueId = r.AttributeValueId,
                        attributeValue = r.AttributeValue,
                        RateMA = previousRates.ToArray()
                    };
                }).ToListAsync();
                return Ok(new { success = true, data = data });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-rates-by-industryId/{IndustryId}")]
        public async Task<IActionResult> OrderManagement_GetAllRatesByIndustryId(int IndustryId, int CategoryId = 0)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();

                var data = await (await _priceDiscoveryService.GetGetTodayRatesListAsync(statusId: (int)DailyRateEnum.Approve, published: true, industryId: IndustryId,categoryId: CategoryId)).SelectAwait(async r =>
                {
                    var picture = (await _productService.GetProductPicturesByProductIdAsync(r.ProductId))?.FirstOrDefault();
                    var rate = await GetFinalRate(r.Id, user, r.SupplierId, r.Rate, r.CategoryId, r.IncludeFirstMile);

                    var formatPrice = await _priceFormatter.FormatPriceAsync(rate, true, false);
                    if (r.IncludeGst)
                        formatPrice = $"{formatPrice}+";

                    List<decimal> previousRates = new List<decimal>();
                    if (!string.IsNullOrWhiteSpace(r.PreviousRates))
                        previousRates = r.PreviousRates.Split(',').Select(decimal.Parse).ToList();

                    return new
                    {
                        r.CategoryId,
                        r.IndustryId,
                        industry = r.IndustryName,
                        r.BrandId,
                        brand = r.BrandName,
                        r.ProductId,
                        product = r.ProductName,
                        sku = r.ProductSku,
                        picture = (await _pictureService.GetPictureUrlAsync(picture != null ? picture.PictureId : 0)),
                        formatPrice = formatPrice,
                        price = rate,
                        createdOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(r.CreatedOnUtc, DateTimeKind.Utc)).ToString(),
                        isFavourite = user == null ? false : (await _priceDiscoveryService.GetFavouriteRateGroupAsync(user.Id, r.ProductId, r.AttributeValueId, r.BrandId)) is null ? false : true,
                        ProductAttributes = await ParseAttributeXml(r.AttributeXml),
                        productattributesxml = r.AttributeXml,
                        attributeValueId = r.AttributeValueId,
                        attributeValue = r.AttributeValue,
                        RateMA = previousRates.ToArray()
                    };
                }).ToListAsync();
                return Ok(new { success = true, data = data });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-favourite-rates")]
        public async Task<IActionResult> OrderManagement_GetAllFavouriteRates()
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                var data = await (await _priceDiscoveryService.GetGetTodayRatesListAsync(statusId: (int)DailyRateEnum.Approve, published: true, userId: user.Id)).SelectAwait(async r =>
                {
                    var picture = (await _productService.GetProductPicturesByProductIdAsync(r.ProductId))?.FirstOrDefault();
                    var rate = await GetFinalRate(r.Id, user, r.SupplierId, r.Rate, r.CategoryId, r.IncludeFirstMile);

                    var formatPrice = await _priceFormatter.FormatPriceAsync(rate, true, false);
                    if (r.IncludeGst)
                        formatPrice = $"{formatPrice}+";

                    List<decimal> previousRates = new List<decimal>();
                    if (!string.IsNullOrWhiteSpace(r.PreviousRates))
                        previousRates = r.PreviousRates.Split(',').Select(decimal.Parse).ToList();

                    return new
                    {
                        r.CategoryId,
                        r.IndustryId,
                        industry = r.IndustryName,
                        r.BrandId,
                        brand = r.BrandName,
                        r.ProductId,
                        product = r.ProductName,
                        sku = r.ProductSku,
                        picture = (await _pictureService.GetPictureUrlAsync(picture != null ? picture.PictureId : 0)),
                        formatPrice = formatPrice,
                        price = rate,
                        createdOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(r.CreatedOnUtc, DateTimeKind.Utc)).ToString(),
                        isFavourite = user == null ? false : (await _priceDiscoveryService.GetFavouriteRateGroupAsync(user.Id, r.ProductId, r.AttributeValueId, r.BrandId)) is null ? false : true,
                        ProductAttributes = await ParseAttributeXml(r.AttributeXml),
                        productattributesxml = r.AttributeXml,
                        attributeValueId = r.AttributeValueId,
                        attributeValue = r.AttributeValue,
                        RateMA = previousRates.ToArray()
                    };
                }).ToListAsync();
                return Ok(new { success = true, data = data });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("rate-add")]
        public async Task<IActionResult> OrderManagement_AddRate([FromBody] OrderManagementApiModel.RateModel model)
        {
            try
            {
                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Rate.Add.Successfully") });

                //if (!ModelState.IsValid)
                //    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                //var user = await _workContext.GetCurrentCustomerAsync();
                //if (!await _customerService.IsRegisteredAsync(user))
                //    return Ok(new { success = false, message = "user not found" });

                //if (model.IndustryId <= 0)
                //    return Ok(new { success = false, message = "Industry id is required" });

                //if (model.CategoryId <= 0)
                //    return Ok(new { success = false, message = "Category id is required" });

                //if (model.ProductId <= 0)
                //    return Ok(new { success = false, message = "Product id is required" });

                //if (model.BrandId <= 0)
                //    return Ok(new { success = false, message = "Brand id is required" });

                //if (model.Price <= 0)
                //    return Ok(new { success = false, message = "Price is required" });

                //var product = await _productService.GetProductByIdAsync(model.ProductId);
                //if (product is null)
                //    return Ok(new { success = false, message = "Product not found" });

                //var category = await _categoryService.GetCategoryByIdAsync(model.CategoryId);
                //if (category is null)
                //    return Ok(new { success = false, message = "Category not found" });

                //var industry = await _industryService.GetIndustryByIdAsync(model.IndustryId);
                //if (industry is null)
                //    return Ok(new { success = false, message = "Industry not found" });

                //var brand = await _manufacturerService.GetManufacturerByIdAsync(model.BrandId);
                //if (brand is null)
                //    return Ok(new { success = false, message = "Brand not found" });

                ////Attribute 
                //var warnings = new List<string>();
                //var attributesData = new List<CommonApiModel.CombinationAttributeApiModel>();
                //foreach (var attribute in model.AttributesData)
                //{
                //    attributesData.Add(new CommonApiModel.CombinationAttributeApiModel
                //    {
                //        Name = attribute.Name,
                //        Value = attribute.Value
                //    });
                //}

                //var attributesXml = await Common_ConvertToXmlAsync(attributesData, product.Id);
                //warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, product, 1, attributesXml));
                //if (warnings.Any())
                //    return Ok(new { success = false, message = warnings.ToArray() });

                //var rate = new Rate()
                //{
                //    Price = model.Price,
                //};
                //await _productService.InsertRateAsync(rate, industry.Id, category.Id, product.Id, attributesXml, brand.Id);

                //await _customerActivityService.InsertActivityAsync("AdRate",
                //    await _localizationService.GetResourceAsync("ActivityLog.AdRate"), rate);

                //return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Rate.Add.Successfully") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("rate-add-by-groupId/{groupId}")]
        public async Task<IActionResult> OrderManagement_AddRateByGroupId(int groupId, decimal price)
        {
            try
            {
                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Rate.Add.Successfully") });

                //if (!ModelState.IsValid)
                //    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                //var user = await _workContext.GetCurrentCustomerAsync();
                //if (!await _customerService.IsRegisteredAsync(user))
                //    return Ok(new { success = false, message = "user not found" });

                //if (groupId <= 0)
                //    return Ok(new { success = false, message = "Group id is required" });

                //if (price <= 0)
                //    return Ok(new { success = false, message = "Category id is required" });

                //var group = await _productService.GetRateGroupByIdAsync(groupId);
                //if (group is null)
                //    return Ok(new { success = false, message = "Group not found" });

                //var rate = new Rate
                //{
                //    Price = price,
                //    RateGroupId = groupId,
                //};

                //await _productService.InsertRateAsync(rate);

                //await _customerActivityService.InsertActivityAsync("AddRate",
                //    await _localizationService.GetResourceAsync("ActivityLog.AdRate"), rate);

                //return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Rate.Add.Successfully") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("favourite-rate-group")]
        public async Task<IActionResult> OrderManagement_FavouriteRateGroup(int productId, int attributeValueId, int brandId, bool isWishlist)
        {
            try
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
                    }
                    return Ok(new { success = true, message = "Add to Favourite" });
                }
                else
                {
                    if (favouriteRateGroup is not null)
                    {
                        await _priceDiscoveryService.DeleteFavouriteRateGroupAsync(favouriteRateGroup);
                        return Ok(new { success = true, message = "Remove to Favourite" });
                    }
                    else
                    {
                        return Ok(new { success = true, message = "Favourite Group not Found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        [HttpGet("all-buyers")]
        public async Task<IActionResult> AllBuyers()
        {
            try
            {
                var buyers = (await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id })).ToList();
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

        [HttpGet("all-bookers")]
        public async Task<IActionResult> AllBookers()
        {
            try
            {
                var bookerRole = await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BookerRoleName);
                var bookers = await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { bookerRole.Id });

                var data = bookers.Select(b => new
                {
                    Id = b.Id,
                    FullName = b.FullName,
                    Email = b.Email,
                    Phone = b.Username
                }).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-roles")]
        public IActionResult AllRoles()
        {
            try
            {
                var roles = new List<SelectListItem>();

                roles.Add(new SelectListItem { Value = "Demand", Text = "Demand" });
                roles.Add(new SelectListItem { Value = "Supply", Text = "Supply" });
                roles.Add(new SelectListItem { Value = "Operations", Text = "Operations" });
                roles.Add(new SelectListItem { Value = "Finance", Text = "Finance" });
                roles.Add(new SelectListItem { Value = "BusinessHead", Text = "Business Head" });
                roles.Add(new SelectListItem { Value = "FinanceHead", Text = "Finance Head" });
                roles.Add(new SelectListItem { Value = "OpsHead", Text = "Ops Head" });

                return Ok(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-jaizas")]
        public async Task<IActionResult> AllJaizas()
        {
            try
            {
                var previouseRow = (await _jaizaService.GetAllJaizaAsync()).OrderByDescending(x => x.Id).Skip(1).Select(x => x.Id).FirstOrDefault();
                var currentDollarRate = await _orderService.GetDollarRatePKR();
                var data = (await _jaizaService.GetAllJaizaAsync()).Select(async c =>
                {
                    return new
                    {
                        Id = c.Id,
                        Prediction = await _localizationService.GetLocalizedAsync(c, x => x.Prediction),
                        PredictionPublished = c.predictionPublished,
                        Recommendation = await _localizationService.GetLocalizedAsync(c, x => x.Recommendation),
                        RecommendationPublished = c.RecommendationPublished,
                        PredictionPicture = await _pictureService.GetPictureUrlAsync(c.predictionPictureId),
                        RecommendationPicture = await _pictureService.GetPictureUrlAsync(c.RecommendationPictureId),
                        Rate = await _priceFormatter.FormatPriceAsync(c.Rate, true, false),
                        UnitName = (await _measureService.GetMeasureWeightByIdAsync(c.UnitId))?.Name,
                        RatePublished = c.RatePublished,
                        Type = c.Type,
                        IsUp = previouseRow > 0 ? c.Rate > (await _jaizaService.GetJaizaByIdAsync(previouseRow)).Rate ? true : false : false,
                        DollarRate = new
                        {
                            Rate = currentDollarRate,
                            Isup = currentDollarRate > c.DollarRate ? true : false
                        }
                    };
                }).Select(t => t.Result).OrderByDescending(a => a.Id).FirstOrDefault();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-faqs")]
        public async Task<IActionResult> AllFaqs()
        {
            try
            {
                var data = (await _faqService.GetAllFaqAsync()).Select(async c =>
                {
                    return new
                    {
                        Question = await _localizationService.GetLocalizedAsync(c, x => x.Question),
                        Answer = await _localizationService.GetLocalizedAsync(c, x => x.Answer),
                    };
                }).Select(t => t.Result).ToList();

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("add-feedBack")]
        public async Task<IActionResult> AddAppFeedBack([FromBody] AppFeedBackApiModel model)
        {
            try
            {
                var user = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(user))
                    return Ok(new { success = false, message = "Invalid user" });

                if (!ModelState.IsValid)
                    return Ok(new { success = false, message = GetModelErrors(ModelState) });

                if (string.IsNullOrWhiteSpace(model.Feedback))
                    return Ok(new { success = false, message = "Feedback is required" });

                if (model.Rating <= 0)
                    return Ok(new { success = false, message = "Rating is required" });

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

                return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Common.AppFeedBack.Created.Success") });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-current-store")]
        public async Task<IActionResult> GetCurrentStore()
        {
            try
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                return Ok(new { success = true, data = new { storeId = store.Id, storeName = store.Name, storeUrl = store.Url } });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-warehouse")]
        public IActionResult AllWarehouse()
        {
            try
            {
                List<object> data = new List<object>();

                var wareHouses = _shippingService.GetAllWarehousesAsync().Result;
                foreach (var wareHouse in wareHouses)
                {
                    data.Add(new
                    {
                        Text = wareHouse.Name,
                        Value = wareHouse.Id.ToString()
                    });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-delayed-reason")]
        public async Task<IActionResult> AllDelayedReasons()
        {
            try
            {
                List<object> data = new List<object>();

                var availableDeliveryTimeReasons = await _shipmentService.GetAllDeliveryTimeReasonAsync();
                foreach (var timeReason in availableDeliveryTimeReasons)
                {
                    data.Add(new
                    {
                        Value = timeReason.Id.ToString(),
                        Text = timeReason.Name
                    });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all-delivery-cost-reasons")]
        public async Task<IActionResult> AllDeliveryCostReasons()
        {
            try
            {
                List<object> data = new List<object>();

                var availableDeliveryCostReasons = await _shipmentService.GetAllDeliveryCostReasonAsync();
                foreach (var deliveryCostReason in availableDeliveryCostReasons)
                {
                    data.Add(new
                    {
                        Value = deliveryCostReason.Id.ToString(),
                        Text = deliveryCostReason.Name
                    });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}