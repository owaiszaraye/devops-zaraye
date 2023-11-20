using ClosedXML.Excel;
using LinqToDB;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Directory;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Logging;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.Messages;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Payments;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Domain.Stores;
using Zaraye.Core.Domain.Tax;
using Zaraye.Core.Http;
using Zaraye.Core.Infrastructure;
using Zaraye.Data;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.Configuration;
using Zaraye.Services.CustomerLedgers;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.ExportImport.Help;
using Zaraye.Services.Helpers;
using Zaraye.Services.Inventory;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Media;
using Zaraye.Services.Messages;
using Zaraye.Services.Orders;
using Zaraye.Services.Seo;
using Zaraye.Services.Shipping;
using Zaraye.Services.Shipping.Date;
using Zaraye.Services.Stores;
using Picture = Zaraye.Core.Domain.Media.Picture;

namespace Zaraye.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAddressService _addressService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IZarayeDataProvider _dataProvider;
        private readonly IDateRangeService _dateRangeService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILogger _logger;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IZarayeFileProvider _fileProvider;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IShippingService _shippingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ICampaignService _campaignService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRequestService _requestService;
        private readonly IShipmentService _shipmentService;
        private readonly IQuotationService _quotationService;
        private readonly IInventoryService _inventoryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerLedgerService _customerLedgerService;
        private readonly ISettingService _settingService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IOrderProcessingService _orderProcessingService;

        #endregion

        #region Ctor

        public ImportManager(CatalogSettings catalogSettings,
            IAddressService addressService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            ICategoryService categoryService,
            ICountryService countryService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            ICustomNumberFormatter customNumberFormatter,
            IZarayeDataProvider dataProvider,
            IDateRangeService dateRangeService,
            IHttpClientFactory httpClientFactory,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ILogger logger,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IZarayeFileProvider fileProvider,
            IOrderService orderService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            IServiceScopeFactory serviceScopeFactory,
            IShippingService shippingService,
            ISpecificationAttributeService specificationAttributeService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            TaxSettings taxSettings,
            ICampaignService campaignService,
            IGenericAttributeService genericAttributeService,
            IRequestService requestService,
            IShipmentService shipmentService,
            IQuotationService quotationService,
            IInventoryService inventoryService,
            IDateTimeHelper dateTimeHelper,
            ICustomerLedgerService customerLedgerService,
            ISettingService settingService,
            IReturnRequestService returnRequestService,
            IOrderProcessingService orderProcessingService)
        {
            _addressService = addressService;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _countryService = countryService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _customNumberFormatter = customNumberFormatter;
            _dataProvider = dataProvider;
            _dateRangeService = dateRangeService;
            _httpClientFactory = httpClientFactory;
            _fileProvider = fileProvider;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _logger = logger;
            _manufacturerService = manufacturerService;
            _measureService = measureService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _productTagService = productTagService;
            _productTemplateService = productTemplateService;
            _serviceScopeFactory = serviceScopeFactory;
            _shippingService = shippingService;
            _specificationAttributeService = specificationAttributeService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _taxSettings = taxSettings;
            _campaignService = campaignService;
            _genericAttributeService = genericAttributeService;
            _requestService = requestService;
            _shipmentService = shipmentService;
            _quotationService = quotationService;
            _inventoryService = inventoryService;
            _dateTimeHelper = dateTimeHelper;
            _customerLedgerService = customerLedgerService;
            _settingService = settingService;
            _returnRequestService = returnRequestService;
            _orderProcessingService = orderProcessingService;
        }

        #endregion

        #region Utilities

        private static ExportedAttributeType GetTypeOfExportedAttribute(IXLWorksheet defaultWorksheet, List<IXLWorksheet> localizedWorksheets, PropertyManager<ExportProductAttribute, Language> productAttributeManager, PropertyManager<ExportSpecificationAttribute, Language> specificationAttributeManager, int iRow)
        {
            productAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

            if (productAttributeManager.IsCaption)
            {
                foreach (var worksheet in localizedWorksheets)
                    productAttributeManager.ReadLocalizedFromXlsx(worksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

                return ExportedAttributeType.ProductAttribute;
            }

            specificationAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

            if (specificationAttributeManager.IsCaption)
            {
                foreach (var worksheet in localizedWorksheets)
                    specificationAttributeManager.ReadLocalizedFromXlsx(worksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

                return ExportedAttributeType.SpecificationAttribute;
            }

            return ExportedAttributeType.NotSpecified;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private static async Task SetOutLineForSpecificationAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
        {
            var attributeType = (cellValue ?? string.Empty).ToString();

            if (attributeType.Equals("AttributeType", StringComparison.InvariantCultureIgnoreCase))
            {
                worksheet.Row(endRow).OutlineLevel = 1;
            }
            else
            {
                if ((await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false))
                    .Any(p => p.Text.Equals(attributeType, StringComparison.InvariantCultureIgnoreCase)))
                    worksheet.Row(endRow).OutlineLevel = 1;
                else if (int.TryParse(attributeType, out var attributeTypeId) && Enum.IsDefined(typeof(SpecificationAttributeType), attributeTypeId))
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
        }

        private static void CopyDataToNewFile(ImportProductMetadata metadata, IXLWorksheet worksheet, string filePath, int startRow, int endRow, int endCell)
        {
            using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
            // ok, we can run the real code of the sample now
            using var workbook = new XLWorkbook(stream);
            // uncomment this line if you want the XML written out to the outputDir
            //xlPackage.DebugMode = true; 

            // get handles to the worksheets
            var outWorksheet = workbook.Worksheets.Add(typeof(Product).Name);
            metadata.Manager.WriteDefaultCaption(outWorksheet);
            var outRow = 2;
            for (var row = startRow; row <= endRow; row++)
            {
                outWorksheet.Row(outRow).OutlineLevel = worksheet.Row(row).OutlineLevel;
                for (var cell = 1; cell <= endCell; cell++)
                {
                    outWorksheet.Row(outRow).Cell(cell).Value = worksheet.Row(row).Cell(cell).Value;
                }

                outRow += 1;
            }

            workbook.Save();
        }

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            for (var i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

            //set to jpeg in case mime type cannot be found
            return mimeType ?? _pictureService.GetPictureContentTypeByFileExtension(_fileProvider.GetFileExtension(filePath));
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the image or null if the image has not changed
        /// </returns>
        protected virtual async Task<Picture> LoadPictureAsync(string picturePath, string name, int? picId = null)
        {
            if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath))
                return null;

            var mimeType = GetMimeTypeFromFilePath(picturePath);
            if (string.IsNullOrEmpty(mimeType))
                return null;

            var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing product pictures
                var existingPicture = await _pictureService.GetPictureByIdAsync(picId.Value);
                if (existingPicture != null)
                {
                    var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                    //picture binary after validation (like in database)
                    var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, name);
                    if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                        existingBinary.SequenceEqual(newPictureBinary))
                    {
                        pictureAlreadyExists = true;
                    }
                }
            }

            if (pictureAlreadyExists)
                return null;

            var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(name));
            return newPicture;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task LogPictureInsertErrorAsync(string picturePath, Exception ex)
        {
            var extension = _fileProvider.GetFileExtension(picturePath);
            var name = _fileProvider.GetFileNameWithoutExtension(picturePath);

            var point = string.IsNullOrEmpty(extension) ? string.Empty : ".";
            var fileName = _fileProvider.FileExists(picturePath) ? $"{name}{point}{extension}" : string.Empty;

            await _logger.ErrorAsync($"Insert picture failed (file name: {fileName})", ex);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ImportProductImagesUsingServicesAsync(IList<ProductPictureMetadata> productPictureMetadata)
        {
            foreach (var product in productPictureMetadata)
            {
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (string.IsNullOrEmpty(picturePath))
                        continue;

                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    if (string.IsNullOrEmpty(mimeType))
                        continue;

                    var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
                    var pictureAlreadyExists = false;
                    if (!product.IsNew)
                    {
                        //compare with existing product pictures
                        var existingPictures = await _pictureService.GetPicturesByProductIdAsync(product.ProductItem.Id);
                        foreach (var existingPicture in existingPictures)
                        {
                            var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                            //picture binary after validation (like in database)
                            var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, picturePath);
                            if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                !existingBinary.SequenceEqual(newPictureBinary))
                                continue;
                            //the same picture content
                            pictureAlreadyExists = true;
                            break;
                        }
                    }

                    if (pictureAlreadyExists)
                        continue;

                    try
                    {
                        var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(product.ProductItem.Name));
                        await _productService.InsertProductPictureAsync(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1,
                            ProductId = product.ProductItem.Id
                        });
                        await _productService.UpdateProductAsync(product.ProductItem);
                    }
                    catch (Exception ex)
                    {
                        await LogPictureInsertErrorAsync(picturePath, ex);
                    }
                }
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ImportProductImagesUsingHashAsync(IList<ProductPictureMetadata> productPictureMetadata, IList<Product> allProductsBySku)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var trimByteCount = _dataProvider.SupportedLengthOfBinaryHash - 1;
            var productsImagesIds = await _productService.GetProductsImagesIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());

            var allProductPictureIds = productsImagesIds.SelectMany(p => p.Value);

            var allPicturesHashes = allProductPictureIds.Any() ? await _dataProvider.GetFieldHashesAsync<PictureBinary>(p => allProductPictureIds.Contains(p.PictureId),
                p => p.PictureId, p => p.BinaryData) : new Dictionary<int, string>();

            foreach (var product in productPictureMetadata)
            {
                foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
                {
                    if (string.IsNullOrEmpty(picturePath))
                        continue;
                    try
                    {
                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        if (string.IsNullOrEmpty(mimeType))
                            continue;

                        var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
                        var pictureAlreadyExists = false;
                        var seoFileName = await _pictureService.GetPictureSeNameAsync(product.ProductItem.Name);

                        if (!product.IsNew)
                        {
                            var newImageHash = HashHelper.CreateHash(
                                newPictureBinary,
                                ExportImportDefaults.ImageHashAlgorithm,
                                trimByteCount);

                            var newValidatedImageHash = HashHelper.CreateHash(
                                await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, seoFileName),
                                ExportImportDefaults.ImageHashAlgorithm,
                                trimByteCount);

                            var imagesIds = productsImagesIds.ContainsKey(product.ProductItem.Id)
                                ? productsImagesIds[product.ProductItem.Id]
                                : Array.Empty<int>();

                            pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key))
                                .Select(p => p.Value)
                                .Any(p =>
                                    p.Equals(newImageHash, StringComparison.OrdinalIgnoreCase) ||
                                    p.Equals(newValidatedImageHash, StringComparison.OrdinalIgnoreCase));
                        }

                        if (pictureAlreadyExists)
                            continue;

                        var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, seoFileName);

                        await _productService.InsertProductPictureAsync(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1,
                            ProductId = product.ProductItem.Id
                        });

                        await _productService.UpdateProductAsync(product.ProductItem);
                    }
                    catch (Exception ex)
                    {
                        await LogPictureInsertErrorAsync(picturePath, ex);
                    }
                }
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<(string seName, bool isParentCategoryExists)> UpdateCategoryByXlsxAsync(Category category, PropertyManager<Category, Language> manager, Dictionary<string, ValueTask<Category>> allCategories, bool isNew)
        {
            var seName = string.Empty;
            var isParentCategoryExists = true;
            var isParentCategorySet = false;

            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "Name":
                        category.Name = property.StringValue.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                        break;
                    case "Description":
                        category.Description = property.StringValue;
                        break;
                    case "CategoryTemplateId":
                        category.CategoryTemplateId = property.IntValue;
                        break;
                    case "MetaKeywords":
                        category.MetaKeywords = property.StringValue;
                        break;
                    case "MetaDescription":
                        category.MetaDescription = property.StringValue;
                        break;
                    case "MetaTitle":
                        category.MetaTitle = property.StringValue;
                        break;
                    case "ParentCategoryId":
                        if (!isParentCategorySet)
                        {
                            var parentCategory = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == property.IntValue);
                            isParentCategorySet = parentCategory != null;

                            isParentCategoryExists = isParentCategorySet || property.IntValue == 0;

                            category.ParentCategoryId = parentCategory?.Id ?? property.IntValue;
                        }

                        break;
                    case "ParentCategoryName":
                        if (_catalogSettings.ExportImportCategoriesUsingCategoryName && !isParentCategorySet)
                        {
                            var categoryName = manager.GetDefaultProperty("ParentCategoryName").StringValue;
                            if (!string.IsNullOrEmpty(categoryName))
                            {
                                var parentCategory = allCategories.ContainsKey(categoryName)
                                    //try find category by full name with all parent category names
                                    ? await allCategories[categoryName]
                                    //try find category by name
                                    : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));

                                if (parentCategory != null)
                                {
                                    category.ParentCategoryId = parentCategory.Id;
                                    isParentCategorySet = true;
                                }
                                else
                                {
                                    isParentCategoryExists = false;
                                }
                            }
                        }

                        break;
                    case "Picture":
                        var picture = await LoadPictureAsync(manager.GetDefaultProperty("Picture").StringValue, category.Name, isNew ? null : (int?)category.PictureId);
                        if (picture != null)
                            category.PictureId = picture.Id;
                        break;
                    case "PageSize":
                        category.PageSize = property.IntValue;
                        break;
                    case "AllowCustomersToSelectPageSize":
                        category.AllowCustomersToSelectPageSize = property.BooleanValue;
                        break;
                    case "PageSizeOptions":
                        category.PageSizeOptions = property.StringValue;
                        break;
                    case "ShowOnHomepage":
                        category.ShowOnHomepage = property.BooleanValue;
                        break;
                    case "PriceRangeFiltering":
                        category.PriceRangeFiltering = property.BooleanValue;
                        break;
                    case "PriceFrom":
                        category.PriceFrom = property.DecimalValue;
                        break;
                    case "PriceTo":
                        category.PriceTo = property.DecimalValue;
                        break;
                    case "AutomaticallyCalculatePriceRange":
                        category.ManuallyPriceRange = property.BooleanValue;
                        break;
                    case "IncludeInTopMenu":
                        category.IncludeInTopMenu = property.BooleanValue;
                        break;
                    case "Published":
                        category.Published = property.BooleanValue;
                        break;
                    case "DisplayOrder":
                        category.DisplayOrder = property.IntValue;
                        break;
                    case "SeName":
                        seName = property.StringValue;
                        break;
                }
            }

            category.UpdatedOnUtc = DateTime.UtcNow;
            return (seName, isParentCategoryExists);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<(Category category, bool isNew, string curentCategoryBreadCrumb)> GetCategoryFromXlsxAsync(PropertyManager<Category, Language> manager, IXLWorksheet worksheet, int iRow, Dictionary<string, ValueTask<Category>> allCategories)
        {
            manager.ReadDefaultFromXlsx(worksheet, iRow);

            //try get category from database by ID
            var category = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == manager.GetDefaultProperty("Id")?.IntValue);

            if (_catalogSettings.ExportImportCategoriesUsingCategoryName && category == null)
            {
                var categoryName = manager.GetDefaultProperty("Name").StringValue;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    category = allCategories.ContainsKey(categoryName)
                        //try find category by full name with all parent category names
                        ? await allCategories[categoryName]
                        //try find category by name
                        : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));
                }
            }

            var isNew = category == null;

            category ??= new Category();

            var curentCategoryBreadCrumb = string.Empty;

            if (isNew)
            {
                category.CreatedOnUtc = DateTime.UtcNow;
                //default values
                category.PageSize = _catalogSettings.DefaultCategoryPageSize;
                category.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
                category.Published = true;
                category.IncludeInTopMenu = true;
                category.AllowCustomersToSelectPageSize = true;
            }
            else
                curentCategoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);

            return (category, isNew, curentCategoryBreadCrumb);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SaveCategoryAsync(bool isNew, Category category, Dictionary<string, ValueTask<Category>> allCategories, string curentCategoryBreadCrumb, bool setSeName, string seName)
        {
            if (isNew)
                await _categoryService.InsertCategoryAsync(category);
            else
                await _categoryService.UpdateCategoryAsync(category);

            var categoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
            if (!allCategories.ContainsKey(categoryBreadCrumb))
                allCategories.Add(categoryBreadCrumb, new ValueTask<Category>(category));
            if (!string.IsNullOrEmpty(curentCategoryBreadCrumb) && allCategories.ContainsKey(curentCategoryBreadCrumb) &&
                categoryBreadCrumb != curentCategoryBreadCrumb)
                allCategories.Remove(curentCategoryBreadCrumb);

            //search engine name
            if (setSeName)
                await _urlRecordService.SaveSlugAsync(category, await _urlRecordService.ValidateSeNameAsync(category, seName, category.Name, true), 0);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ImportCategoryLocalizedAsync(Category category, WorkbookMetadata<Category> metadata, PropertyManager<Category, Language> manager, int iRow, IList<Language> languages)
        {
            if (!metadata.LocalizedWorksheets.Any())
                return;

            var setSeName = metadata.LocalizedProperties.Any(p => p.PropertyName == "SeName");
            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                manager.CurrentLanguage = language;
                manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

                foreach (var property in manager.GetLocalizedProperties)
                {
                    string localizedName = null;

                    switch (property.PropertyName)
                    {
                        case "Name":
                            localizedName = property.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.Name, localizedName, language.Id);
                            break;
                        case "Description":
                            await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.Description, property.StringValue, language.Id);
                            break;
                        case "MetaKeywords":
                            await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.MetaKeywords, property.StringValue, language.Id);
                            break;
                        case "MetaDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.MetaDescription, property.StringValue, language.Id);
                            break;
                        case "MetaTitle":
                            await _localizedEntityService.SaveLocalizedValueAsync(category, m => m.MetaTitle, property.StringValue, language.Id);
                            break;
                        case "SeName":
                            //search engine name
                            if (setSeName)
                            {
                                var lSeName = await _urlRecordService.ValidateSeNameAsync(category, property.StringValue, localizedName, false);
                                await _urlRecordService.SaveSlugAsync(category, lSeName, language.Id);
                            }
                            break;
                    }
                }
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ImportManufaturerLocalizedAsync(Manufacturer manufacturer, WorkbookMetadata<Manufacturer> metadata, PropertyManager<Manufacturer, Language> manager, int iRow, IList<Language> languages)
        {
            if (!metadata.LocalizedWorksheets.Any())
                return;

            var setSeName = metadata.LocalizedProperties.Any(p => p.PropertyName == "SeName");
            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                manager.CurrentLanguage = language;
                manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

                foreach (var property in manager.GetLocalizedProperties)
                {
                    string localizedName = null;

                    switch (property.PropertyName)
                    {
                        case "Name":
                            localizedName = property.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.Name, localizedName, language.Id);
                            break;
                        case "Description":
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.Description, property.StringValue, language.Id);
                            break;
                        case "MetaKeywords":
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaKeywords, property.StringValue, language.Id);
                            break;
                        case "MetaDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaDescription, property.StringValue, language.Id);
                            break;
                        case "MetaTitle":
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaTitle, property.StringValue, language.Id);
                            break;
                        case "SeName":
                            //search engine name
                            if (setSeName)
                            {
                                var localizedSeName = await _urlRecordService.ValidateSeNameAsync(manufacturer, property.StringValue, localizedName, false);
                                await _urlRecordService.SaveSlugAsync(manufacturer, localizedSeName, language.Id);
                            }
                            break;
                    }
                }
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SetOutLineForProductAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
        {
            try
            {
                var aid = Convert.ToInt32(cellValue ?? -1);

                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(aid);

                if (productAttribute != null)
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
            catch (FormatException)
            {
                if ((cellValue ?? string.Empty).ToString() == "AttributeId")
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ImportProductAttributeAsync(ImportProductMetadata metadata, Product lastLoadedProduct, IList<Language> languages, int iRow)
        {
            var productAttributeManager = metadata.ProductAttributeManager;
            if (!_catalogSettings.ExportImportProductAttributes || lastLoadedProduct == null || productAttributeManager.IsCaption)
                return;

            var productAttributeId = productAttributeManager.GetDefaultProperty("AttributeId").IntValue;
            var attributeControlTypeId = productAttributeManager.GetDefaultProperty("AttributeControlType").IntValue;

            var productAttributeValueId = productAttributeManager.GetDefaultProperty("ProductAttributeValueId").IntValue;
            var associatedProductId = productAttributeManager.GetDefaultProperty("AssociatedProductId").IntValue;
            var valueName = productAttributeManager.GetDefaultProperty("ValueName").StringValue;
            var attributeValueTypeId = productAttributeManager.GetDefaultProperty("AttributeValueType").IntValue;
            var colorSquaresRgb = productAttributeManager.GetDefaultProperty("ColorSquaresRgb").StringValue;
            var imageSquaresPictureId = productAttributeManager.GetDefaultProperty("ImageSquaresPictureId").IntValue;
            var priceAdjustment = productAttributeManager.GetDefaultProperty("PriceAdjustment").DecimalValue;
            var priceAdjustmentUsePercentage = productAttributeManager.GetDefaultProperty("PriceAdjustmentUsePercentage").BooleanValue;
            var weightAdjustment = productAttributeManager.GetDefaultProperty("WeightAdjustment").DecimalValue;
            var cost = productAttributeManager.GetDefaultProperty("Cost").DecimalValue;
            var customerEntersQty = productAttributeManager.GetDefaultProperty("CustomerEntersQty").BooleanValue;
            var quantity = productAttributeManager.GetDefaultProperty("Quantity").IntValue;
            var isPreSelected = productAttributeManager.GetDefaultProperty("IsPreSelected").BooleanValue;
            var displayOrder = productAttributeManager.GetDefaultProperty("DisplayOrder").IntValue;
            var pictureId = productAttributeManager.GetDefaultProperty("PictureId").IntValue;
            var textPrompt = productAttributeManager.GetDefaultProperty("AttributeTextPrompt").StringValue;
            var isRequired = productAttributeManager.GetDefaultProperty("AttributeIsRequired").BooleanValue;
            var attributeDisplayOrder = productAttributeManager.GetDefaultProperty("AttributeDisplayOrder").IntValue;

            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(lastLoadedProduct.Id))
                .FirstOrDefault(pam => pam.ProductAttributeId == productAttributeId);

            if (productAttributeMapping == null)
            {
                //insert mapping
                productAttributeMapping = new ProductAttributeMapping
                {
                    ProductId = lastLoadedProduct.Id,
                    ProductAttributeId = productAttributeId,
                    TextPrompt = textPrompt,
                    IsRequired = isRequired,
                    AttributeControlTypeId = attributeControlTypeId,
                    DisplayOrder = attributeDisplayOrder
                };
                await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMapping);
            }
            else
            {
                productAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
                productAttributeMapping.TextPrompt = textPrompt;
                productAttributeMapping.IsRequired = isRequired;
                productAttributeMapping.DisplayOrder = attributeDisplayOrder;
                await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
            }

            var pav = (await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMapping.Id))
                .FirstOrDefault(p => p.Id == productAttributeValueId);

            //var pav = await _productAttributeService.GetProductAttributeValueByIdAsync(productAttributeValueId);

            var attributeControlType = (AttributeControlType)attributeControlTypeId;

            if (pav == null)
            {
                switch (attributeControlType)
                {
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.TextBox:
                        if (productAttributeMapping.ValidationRulesAllowed())
                        {
                            productAttributeMapping.ValidationMinLength = productAttributeManager.GetDefaultProperty("ValidationMinLength")?.IntValueNullable;
                            productAttributeMapping.ValidationMaxLength = productAttributeManager.GetDefaultProperty("ValidationMaxLength")?.IntValueNullable;
                            productAttributeMapping.ValidationFileMaximumSize = productAttributeManager.GetDefaultProperty("ValidationFileMaximumSize")?.IntValueNullable;
                            productAttributeMapping.ValidationFileAllowedExtensions = productAttributeManager.GetDefaultProperty("ValidationFileAllowedExtensions")?.StringValue;
                            productAttributeMapping.DefaultValue = productAttributeManager.GetDefaultProperty("DefaultValue")?.StringValue;

                            await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
                        }

                        return;
                }

                pav = new ProductAttributeValue
                {
                    ProductAttributeMappingId = productAttributeMapping.Id,
                    AttributeValueType = (AttributeValueType)attributeValueTypeId,
                    AssociatedProductId = associatedProductId,
                    Name = valueName,
                    PriceAdjustment = priceAdjustment,
                    PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage,
                    WeightAdjustment = weightAdjustment,
                    Cost = cost,
                    IsPreSelected = isPreSelected,
                    DisplayOrder = displayOrder,
                    ColorSquaresRgb = colorSquaresRgb,
                    ImageSquaresPictureId = imageSquaresPictureId,
                    CustomerEntersQty = customerEntersQty,
                    Quantity = quantity,
                    PictureId = pictureId
                };

                await _productAttributeService.InsertProductAttributeValueAsync(pav);
            }
            else
            {
                pav.AttributeValueTypeId = attributeValueTypeId;
                pav.AssociatedProductId = associatedProductId;
                pav.Name = valueName;
                pav.ColorSquaresRgb = colorSquaresRgb;
                pav.ImageSquaresPictureId = imageSquaresPictureId;
                pav.PriceAdjustment = priceAdjustment;
                pav.PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage;
                pav.WeightAdjustment = weightAdjustment;
                pav.Cost = cost;
                pav.CustomerEntersQty = customerEntersQty;
                pav.Quantity = quantity;
                pav.IsPreSelected = isPreSelected;
                pav.DisplayOrder = displayOrder;
                pav.PictureId = pictureId;

                await _productAttributeService.UpdateProductAttributeValueAsync(pav);
            }

            if (!metadata.LocalizedWorksheets.Any())
                return;

            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                productAttributeManager.CurrentLanguage = language;
                productAttributeManager.ReadLocalizedFromXlsx(lWorksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

                valueName = productAttributeManager.GetLocalizedProperty("ValueName").StringValue;
                textPrompt = productAttributeManager.GetLocalizedProperty("AttributeTextPrompt").StringValue;

                await _localizedEntityService.SaveLocalizedValueAsync(pav, p => p.Name, valueName, language.Id);
                await _localizedEntityService.SaveLocalizedValueAsync(productAttributeMapping, p => p.TextPrompt, textPrompt, language.Id);

                switch (attributeControlType)
                {
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.TextBox:
                        if (productAttributeMapping.ValidationRulesAllowed())
                        {
                            var defaultValue = productAttributeManager.GetLocalizedProperty("DefaultValue")?.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(productAttributeMapping, p => p.DefaultValue, defaultValue, language.Id);
                        }

                        return;
                }
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ImportSpecificationAttributeAsync(ImportProductMetadata metadata, Product lastLoadedProduct, IList<Language> languages, int iRow)
        {
            var specificationAttributeManager = metadata.SpecificationAttributeManager;
            if (!_catalogSettings.ExportImportProductSpecificationAttributes || lastLoadedProduct == null || specificationAttributeManager.IsCaption)
                return;

            var attributeTypeId = specificationAttributeManager.GetDefaultProperty("AttributeType").IntValue;
            var allowFiltering = specificationAttributeManager.GetDefaultProperty("AllowFiltering").BooleanValue;
            var specificationAttributeOptionId = specificationAttributeManager.GetDefaultProperty("SpecificationAttributeOptionId").IntValue;
            var productId = lastLoadedProduct.Id;
            var customValue = specificationAttributeManager.GetDefaultProperty("CustomValue").StringValue;
            var displayOrder = specificationAttributeManager.GetDefaultProperty("DisplayOrder").IntValue;
            var showOnProductPage = specificationAttributeManager.GetDefaultProperty("ShowOnProductPage").BooleanValue;

            //if specification attribute option isn't set, try to get first of possible specification attribute option for current specification attribute
            if (specificationAttributeOptionId == 0)
            {
                var specificationAttribute = specificationAttributeManager.GetDefaultProperty("SpecificationAttribute").IntValue;
                specificationAttributeOptionId =
                    (await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(
                        specificationAttribute))
                    .FirstOrDefault()?.Id ?? specificationAttributeOptionId;
            }

            var productSpecificationAttribute = specificationAttributeOptionId == 0
                ? null
                : (await _specificationAttributeService.GetProductSpecificationAttributesAsync(productId, specificationAttributeOptionId)).FirstOrDefault();

            var isNew = productSpecificationAttribute == null;

            if (isNew)
                productSpecificationAttribute = new ProductSpecificationAttribute();

            if (attributeTypeId != (int)SpecificationAttributeType.Option)
                //we allow filtering only for "Option" attribute type
                allowFiltering = false;

            //we don't allow CustomValue for "Option" attribute type
            if (attributeTypeId == (int)SpecificationAttributeType.Option)
                customValue = null;

            productSpecificationAttribute.AttributeTypeId = attributeTypeId;
            productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOptionId;
            productSpecificationAttribute.ProductId = productId;
            productSpecificationAttribute.CustomValue = customValue;
            productSpecificationAttribute.AllowFiltering = allowFiltering;
            productSpecificationAttribute.ShowOnProductPage = showOnProductPage;
            productSpecificationAttribute.DisplayOrder = displayOrder;

            if (isNew)
                await _specificationAttributeService.InsertProductSpecificationAttributeAsync(productSpecificationAttribute);
            else
                await _specificationAttributeService.UpdateProductSpecificationAttributeAsync(productSpecificationAttribute);

            if (!metadata.LocalizedWorksheets.Any())
                return;

            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                specificationAttributeManager.CurrentLanguage = language;
                specificationAttributeManager.ReadLocalizedFromXlsx(lWorksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

                customValue = specificationAttributeManager.GetLocalizedProperty("CustomValue").StringValue;
                await _localizedEntityService.SaveLocalizedValueAsync(productSpecificationAttribute, p => p.CustomValue, customValue, language.Id);
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<string> DownloadFileAsync(string urlString, IList<string> downloadedFiles)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;

            if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
                return urlString;

            if (!_catalogSettings.ExportImportAllowDownloadImages)
                return string.Empty;

            //ensure that temp directory is created
            var tempDirectory = _fileProvider.MapPath(ExportImportDefaults.UploadsTempPath);
            _fileProvider.CreateDirectory(tempDirectory);

            var fileName = _fileProvider.GetFileName(urlString);
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var filePath = _fileProvider.Combine(tempDirectory, fileName);
            try
            {
                var client = _httpClientFactory.CreateClient(ZarayeHttpDefaults.DefaultHttpClient);
                var fileData = await client.GetByteArrayAsync(urlString);
                await using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    fs.Write(fileData, 0, fileData.Length);

                downloadedFiles?.Add(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("Download image failed", ex);
            }

            return string.Empty;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<ImportProductMetadata> PrepareImportProductDataAsync(IXLWorkbook workbook, IList<Language> languages)
        {
            //the columns
            var metadata = GetWorkbookMetadata<Product>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Product, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var productAttributeProperties = new[]
            {
                new PropertyByName<ExportProductAttribute, Language>("AttributeId"),
                new PropertyByName<ExportProductAttribute, Language>("AttributeName"),
                new PropertyByName<ExportProductAttribute, Language>("DefaultValue"),
                new PropertyByName<ExportProductAttribute, Language>("ValidationMinLength"),
                new PropertyByName<ExportProductAttribute, Language>("ValidationMaxLength"),
                new PropertyByName<ExportProductAttribute, Language>("ValidationFileAllowedExtensions"),
                new PropertyByName<ExportProductAttribute, Language>("ValidationFileMaximumSize"),
                new PropertyByName<ExportProductAttribute, Language>("AttributeTextPrompt"),
                new PropertyByName<ExportProductAttribute, Language>("AttributeIsRequired"),
                new PropertyByName<ExportProductAttribute, Language>("AttributeControlType"),
                new PropertyByName<ExportProductAttribute, Language>("AttributeDisplayOrder"),
                new PropertyByName<ExportProductAttribute, Language>("ProductAttributeValueId"),
                new PropertyByName<ExportProductAttribute, Language>("ValueName"),
                new PropertyByName<ExportProductAttribute, Language>("AttributeValueType"),
                new PropertyByName<ExportProductAttribute, Language>("AssociatedProductId"),
                new PropertyByName<ExportProductAttribute, Language>("ColorSquaresRgb"),
                new PropertyByName<ExportProductAttribute, Language>("ImageSquaresPictureId"),
                new PropertyByName<ExportProductAttribute, Language>("PriceAdjustment"),
                new PropertyByName<ExportProductAttribute, Language>("PriceAdjustmentUsePercentage"),
                new PropertyByName<ExportProductAttribute, Language>("WeightAdjustment"),
                new PropertyByName<ExportProductAttribute, Language>("Cost"),
                new PropertyByName<ExportProductAttribute, Language>("CustomerEntersQty"),
                new PropertyByName<ExportProductAttribute, Language>("Quantity"),
                new PropertyByName<ExportProductAttribute, Language>("IsPreSelected"),
                new PropertyByName<ExportProductAttribute, Language>("DisplayOrder"),
                new PropertyByName<ExportProductAttribute, Language>("PictureId")
            };

            var productAttributeLocalizedProperties = new[]
            {
                new PropertyByName<ExportProductAttribute, Language>("DefaultValue"),
                new PropertyByName<ExportProductAttribute, Language>("AttributeTextPrompt"),
                new PropertyByName<ExportProductAttribute, Language>("ValueName")
            };

            var productAttributeManager = new PropertyManager<ExportProductAttribute, Language>(productAttributeProperties, _catalogSettings, productAttributeLocalizedProperties, languages);

            var specificationAttributeProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute, Language>("AttributeType", (p, l) => p.AttributeTypeId),
                new PropertyByName<ExportSpecificationAttribute, Language>("SpecificationAttribute", (p, l) => p.SpecificationAttributeId),
                new PropertyByName<ExportSpecificationAttribute, Language>("CustomValue", (p, l) => p.CustomValue),
                new PropertyByName<ExportSpecificationAttribute, Language>("SpecificationAttributeOptionId", (p, l) => p.SpecificationAttributeOptionId),
                new PropertyByName<ExportSpecificationAttribute, Language>("AllowFiltering", (p, l) => p.AllowFiltering),
                new PropertyByName<ExportSpecificationAttribute, Language>("ShowOnProductPage", (p, l) => p.ShowOnProductPage),
                new PropertyByName<ExportSpecificationAttribute, Language>("DisplayOrder", (p, l) => p.DisplayOrder)
            };

            var specificationAttributeLocalizedProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute, Language>("CustomValue")
            };

            var specificationAttributeManager = new PropertyManager<ExportSpecificationAttribute, Language>(specificationAttributeProperties, _catalogSettings, specificationAttributeLocalizedProperties, languages);

            var endRow = 2;
            var allCategories = new List<string>();
            var allSku = new List<string>();

            var tempProperty = manager.GetDefaultProperty("Categories");
            var categoryCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            tempProperty = manager.GetDefaultProperty("SKU");
            var skuCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            var allManufacturers = new List<string>();
            tempProperty = manager.GetDefaultProperty("Manufacturers");
            var manufacturerCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            var allStores = new List<string>();
            tempProperty = manager.GetDefaultProperty("LimitedToStores");
            var limitedToStoresCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            if (_catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities)
            {
                productAttributeManager.SetSelectList("AttributeControlType", await AttributeControlType.TextBox.ToSelectListAsync(useLocalization: false));
                productAttributeManager.SetSelectList("AttributeValueType", await AttributeValueType.Simple.ToSelectListAsync(useLocalization: false));

                specificationAttributeManager.SetSelectList("AttributeType", await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false));
                specificationAttributeManager.SetSelectList("SpecificationAttribute", (await _specificationAttributeService
                    .GetSpecificationAttributesAsync())
                    .Select(sa => sa as BaseEntity)
                    .ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty));

                manager.SetSelectList("ProductType", await ProductType.SimpleProduct.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("DownloadActivationType",
                    await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("ManageInventoryMethod",
                    await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("LowStockActivity",
                    await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("BackorderMode", await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("RecurringCyclePeriod",
                    await RecurringProductCyclePeriod.Days.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("RentalPricePeriod", await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false));

                manager.SetSelectList("ProductTemplate",
                    (await _productTemplateService.GetAllProductTemplatesAsync()).Select(pt => pt as BaseEntity)
                        .ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty));
                manager.SetSelectList("DeliveryDate",
                    (await _dateRangeService.GetAllDeliveryDatesAsync()).Select(dd => dd as BaseEntity)
                        .ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
                manager.SetSelectList("ProductAvailabilityRange",
                    (await _dateRangeService.GetAllProductAvailabilityRangesAsync()).Select(range => range as BaseEntity)
                        .ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty));
                manager.SetSelectList("BasepriceUnit",
                    (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity)
                        .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
                manager.SetSelectList("BasepriceBaseUnit",
                    (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity)
                        .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
            }

            var allAttributeIds = new List<int>();
            var allSpecificationAttributeOptionIds = new List<int>();

            var attributeIdCellNum = 1 + ExportProductAttribute.ProductAttributeCellOffset;
            var specificationAttributeOptionIdCellNum =
                specificationAttributeManager.GetIndex("SpecificationAttributeOptionId") +
                ExportProductAttribute.ProductAttributeCellOffset;

            var productsInFile = new List<int>();

            //find end of data
            var typeOfExportedAttribute = ExportedAttributeType.NotSpecified;
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => defaultWorksheet.Row(endRow).Cell(property.PropertyOrderPosition))
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                if (new[] { 1, 2 }.Select(cellNum => defaultWorksheet.Row(endRow).Cell(cellNum))
                        .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString())) &&
                    defaultWorksheet.Row(endRow).OutlineLevel == 0)
                {
                    var cellValue = defaultWorksheet.Row(endRow).Cell(attributeIdCellNum).Value;
                    await SetOutLineForProductAttributeRowAsync(cellValue, defaultWorksheet, endRow);
                    await SetOutLineForSpecificationAttributeRowAsync(cellValue, defaultWorksheet, endRow);
                }

                if (defaultWorksheet.Row(endRow).OutlineLevel != 0)
                {
                    var newTypeOfExportedAttribute = GetTypeOfExportedAttribute(defaultWorksheet, metadata.LocalizedWorksheets, productAttributeManager, specificationAttributeManager, endRow);

                    //skip caption row
                    if (newTypeOfExportedAttribute != ExportedAttributeType.NotSpecified && newTypeOfExportedAttribute != typeOfExportedAttribute)
                    {
                        typeOfExportedAttribute = newTypeOfExportedAttribute;
                        endRow++;
                        continue;
                    }

                    switch (typeOfExportedAttribute)
                    {
                        case ExportedAttributeType.ProductAttribute:
                            productAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, endRow,
                                ExportProductAttribute.ProductAttributeCellOffset);
                            if (int.TryParse((defaultWorksheet.Row(endRow).Cell(attributeIdCellNum).Value ?? string.Empty).ToString(), out var aid))
                            {
                                allAttributeIds.Add(aid);
                            }

                            break;
                        case ExportedAttributeType.SpecificationAttribute:
                            specificationAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, endRow, ExportProductAttribute.ProductAttributeCellOffset);

                            if (int.TryParse((defaultWorksheet.Row(endRow).Cell(specificationAttributeOptionIdCellNum).Value ?? string.Empty).ToString(), out var saoid))
                            {
                                allSpecificationAttributeOptionIds.Add(saoid);
                            }

                            break;
                    }

                    endRow++;
                    continue;
                }

                if (categoryCellNum > 0)
                {
                    var categoryIds = defaultWorksheet.Row(endRow).Cell(categoryCellNum).Value?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(categoryIds))
                        allCategories.AddRange(categoryIds
                            .Split(new[] { ";", ">>" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                            .Distinct());
                }

                if (skuCellNum > 0)
                {
                    var sku = defaultWorksheet.Row(endRow).Cell(skuCellNum).Value?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(sku))
                        allSku.Add(sku);
                }

                if (manufacturerCellNum > 0)
                {
                    var manufacturerIds = defaultWorksheet.Row(endRow).Cell(manufacturerCellNum).Value?.ToString() ??
                                          string.Empty;
                    if (!string.IsNullOrEmpty(manufacturerIds))
                        allManufacturers.AddRange(manufacturerIds
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                }

                if (limitedToStoresCellNum > 0)
                {
                    var storeIds = defaultWorksheet.Row(endRow).Cell(limitedToStoresCellNum).Value?.ToString() ??
                                          string.Empty;
                    if (!string.IsNullOrEmpty(storeIds))
                        allStores.AddRange(storeIds
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                }

                //counting the number of products
                productsInFile.Add(endRow);

                endRow++;
            }

            //performance optimization, the check for the existence of the categories in one SQL request
            var notExistingCategories = await _categoryService.GetNotExistingCategoriesAsync(allCategories.ToArray());
            if (notExistingCategories.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.CategoriesDontExist"), string.Join(", ", notExistingCategories)));
            }

            //performance optimization, the check for the existence of the manufacturers in one SQL request
            var notExistingManufacturers = await _manufacturerService.GetNotExistingManufacturersAsync(allManufacturers.ToArray());
            if (notExistingManufacturers.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.ManufacturersDontExist"), string.Join(", ", notExistingManufacturers)));
            }

            //performance optimization, the check for the existence of the product attributes in one SQL request
            var notExistingProductAttributes = await _productAttributeService.GetNotExistingAttributesAsync(allAttributeIds.ToArray());
            if (notExistingProductAttributes.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.ProductAttributesDontExist"), string.Join(", ", notExistingProductAttributes)));
            }

            //performance optimization, the check for the existence of the specification attribute options in one SQL request
            var notExistingSpecificationAttributeOptions = await _specificationAttributeService.GetNotExistingSpecificationAttributeOptionsAsync(allSpecificationAttributeOptionIds.Where(saoId => saoId != 0).ToArray());
            if (notExistingSpecificationAttributeOptions.Any())
            {
                throw new ArgumentException($"The following specification attribute option ID(s) don't exist - {string.Join(", ", notExistingSpecificationAttributeOptions)}");
            }

            //performance optimization, the check for the existence of the stores in one SQL request
            var notExistingStores = await _storeService.GetNotExistingStoresAsync(allStores.ToArray());
            if (notExistingStores.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.StoresDontExist"), string.Join(", ", notExistingStores)));
            }

            return new ImportProductMetadata
            {
                EndRow = endRow,
                Manager = manager,
                Properties = defaultProperties,
                ProductsInFile = productsInFile,
                ProductAttributeManager = productAttributeManager,
                DefaultWorksheet = defaultWorksheet,
                LocalizedWorksheets = metadata.LocalizedWorksheets,
                SpecificationAttributeManager = specificationAttributeManager,
                SkuCellNum = skuCellNum,
                AllSku = allSku
            };
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ImportProductsFromSplitedXlsxAsync(IXLWorksheet worksheet, ImportProductMetadata metadata)
        {
            foreach (var path in SplitProductFile(worksheet, metadata))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                // Resolve
                var importManager = EngineContext.Current.Resolve<IImportManager>(scope);

                using var sr = new StreamReader(path);
                await importManager.ImportProductsFromXlsxAsync(sr.BaseStream);

                try
                {
                    _fileProvider.DeleteFile(path);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private IList<string> SplitProductFile(IXLWorksheet worksheet, ImportProductMetadata metadata)
        {
            var fileIndex = 1;
            var fileName = Guid.NewGuid().ToString();
            var endCell = metadata.Properties.Max(p => p.PropertyOrderPosition);

            var filePaths = new List<string>();

            while (true)
            {
                var curIndex = fileIndex * _catalogSettings.ExportImportProductsCountInOneFile;

                var startRow = metadata.ProductsInFile[(fileIndex - 1) * _catalogSettings.ExportImportProductsCountInOneFile];

                var endRow = metadata.CountProductsInFile > curIndex + 1
                    ? metadata.ProductsInFile[curIndex - 1]
                    : metadata.EndRow;

                var filePath = $"{_fileProvider.MapPath(ExportImportDefaults.UploadsTempPath)}/{fileName}_part_{fileIndex}.xlsx";

                CopyDataToNewFile(metadata, worksheet, filePath, startRow, endRow, endCell);

                filePaths.Add(filePath);
                fileIndex += 1;

                if (endRow == metadata.EndRow)
                    break;
            }

            return filePaths;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<(ImportOrderMetadata, IXLWorksheet)> PrepareImportOrderDataAsync(IXLWorkbook workbook)
        {
            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Order>(workbook, languages);
            var worksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;

            var manager = new PropertyManager<Order, Language>(defaultProperties, _catalogSettings);

            var orderItemProperties = new[]
            {
                new PropertyByName<OrderItem, Language>("OrderItemGuid"),
                new PropertyByName<OrderItem, Language>("Name"),
                new PropertyByName<OrderItem, Language>("Sku"),
                new PropertyByName<OrderItem, Language>("PriceExclTax"),
                new PropertyByName<OrderItem, Language>("PriceInclTax"),
                new PropertyByName<OrderItem, Language>("Quantity"),
                new PropertyByName<OrderItem, Language>("DiscountExclTax"),
                new PropertyByName<OrderItem, Language>("DiscountInclTax"),
                new PropertyByName<OrderItem, Language>("TotalExclTax"),
                new PropertyByName<OrderItem, Language>("TotalInclTax")
            };

            var orderItemManager = new PropertyManager<OrderItem, Language>(orderItemProperties, _catalogSettings);

            var endRow = 2;
            var allOrderGuids = new List<Guid>();

            var tempProperty = manager.GetDefaultProperty("OrderGuid");
            var orderGuidCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            tempProperty = manager.GetDefaultProperty("CustomerGuid");
            var customerGuidCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            manager.SetSelectList("OrderStatus", await OrderStatus.Cancelled.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("ShippingStatus", await ShippingStatus.Delivered.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("PaymentStatus", await PaymentStatus.Pending.ToSelectListAsync(useLocalization: false));

            var allCustomerGuids = new List<Guid>();

            var allOrderItemSkus = new List<string>();

            var countOrdersInFile = 0;

            //find end of data
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => worksheet.Row(endRow).Cell(property.PropertyOrderPosition))
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                if (worksheet.Row(endRow).OutlineLevel != 0)
                {
                    orderItemManager.ReadDefaultFromXlsx(worksheet, endRow, 2);

                    //skip caption row
                    if (!orderItemManager.IsCaption)
                    {
                        allOrderItemSkus.Add(orderItemManager.GetDefaultProperty("Sku").StringValue);
                    }

                    endRow++;
                    continue;
                }

                if (orderGuidCellNum > 0)
                {
                    var orderGuidString = worksheet.Row(endRow).Cell(orderGuidCellNum).Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(orderGuidString) && Guid.TryParse(orderGuidString, out var orderGuid))
                        allOrderGuids.Add(orderGuid);
                }

                if (customerGuidCellNum > 0)
                {
                    var customerGuidString = worksheet.Row(endRow).Cell(customerGuidCellNum).Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(customerGuidString) && Guid.TryParse(customerGuidString, out var customerGuid))
                        allCustomerGuids.Add(customerGuid);
                }

                //counting the number of orders
                countOrdersInFile++;

                endRow++;
            }

            //performance optimization, the check for the existence of the customers in one SQL request
            var notExistingCustomerGuids = await _customerService.GetNotExistingCustomersAsync(allCustomerGuids.ToArray());
            if (notExistingCustomerGuids.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Import.CustomersDontExist"), string.Join(", ", notExistingCustomerGuids)));
            }

            //performance optimization, the check for the existence of the order items in one SQL request
            var notExistingProductSkus = await _productService.GetNotExistingProductsAsync(allOrderItemSkus.ToArray());
            if (notExistingProductSkus.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Import.ProductsDontExist"), string.Join(", ", notExistingProductSkus)));
            }

            return (new ImportOrderMetadata
            {
                EndRow = endRow,
                Manager = manager,
                Properties = defaultProperties,
                CountOrdersInFile = countOrdersInFile,
                OrderItemManager = orderItemManager,
                AllOrderGuids = allOrderGuids,
                AllCustomerGuids = allCustomerGuids
            }, worksheet);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ImportOrderItemAsync(PropertyManager<OrderItem, Language> orderItemManager, Order lastLoadedOrder)
        {
            if (lastLoadedOrder == null || orderItemManager.IsCaption)
                return;

            var orderItemGuid = Guid.TryParse(orderItemManager.GetDefaultProperty("OrderItemGuid").StringValue, out var guidValue) ? guidValue : Guid.NewGuid();
            var sku = orderItemManager.GetDefaultProperty("Sku").StringValue;
            var priceExclTax = orderItemManager.GetDefaultProperty("PriceExclTax").DecimalValue;
            var priceInclTax = orderItemManager.GetDefaultProperty("PriceInclTax").DecimalValue;
            var quantity = orderItemManager.GetDefaultProperty("Quantity").IntValue;
            var discountExclTax = orderItemManager.GetDefaultProperty("DiscountExclTax").DecimalValue;
            var discountInclTax = orderItemManager.GetDefaultProperty("DiscountInclTax").DecimalValue;
            var totalExclTax = orderItemManager.GetDefaultProperty("TotalExclTax").DecimalValue;
            var totalInclTax = orderItemManager.GetDefaultProperty("TotalInclTax").DecimalValue;

            var orderItemProduct = await _productService.GetProductBySkuAsync(sku);
            var orderItem = (await _orderService.GetOrderItemsAsync(lastLoadedOrder.Id)).FirstOrDefault(f => f.OrderItemGuid == orderItemGuid);

            if (orderItem == null)
            {
                //insert order item
                orderItem = new OrderItem
                {
                    DiscountAmountExclTax = discountExclTax,
                    DiscountAmountInclTax = discountInclTax,
                    OrderId = lastLoadedOrder.Id,
                    OrderItemGuid = orderItemGuid,
                    PriceExclTax = totalExclTax,
                    PriceInclTax = totalInclTax,
                    ProductId = orderItemProduct.Id,
                    Quantity = quantity,
                    OriginalProductCost = orderItemProduct.ProductCost,
                    UnitPriceExclTax = priceExclTax,
                    UnitPriceInclTax = priceInclTax
                };
                await _orderService.InsertOrderItemAsync(orderItem);
            }
            else
            {
                //update order item
                orderItem.DiscountAmountExclTax = discountExclTax;
                orderItem.DiscountAmountInclTax = discountInclTax;
                orderItem.OrderId = lastLoadedOrder.Id;
                orderItem.PriceExclTax = totalExclTax;
                orderItem.PriceInclTax = totalInclTax;
                orderItem.Quantity = quantity;
                orderItem.UnitPriceExclTax = priceExclTax;
                orderItem.UnitPriceInclTax = priceInclTax;
                await _orderService.UpdateOrderItemAsync(orderItem);
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ImportProductLocalizedAsync(Product product, ImportProductMetadata metadata, int iRow, IList<Language> languages)
        {
            if (metadata.LocalizedWorksheets.Any())
            {
                var manager = metadata.Manager;
                foreach (var language in languages)
                {
                    var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                    if (lWorksheet == null)
                        continue;

                    manager.CurrentLanguage = language;
                    manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

                    foreach (var property in manager.GetLocalizedProperties)
                    {
                        string localizedName = null;

                        switch (property.PropertyName)
                        {
                            case "Name":
                                localizedName = property.StringValue;
                                await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.Name, localizedName, language.Id);
                                break;
                            case "ShortDescription":
                                await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.ShortDescription, property.StringValue, language.Id);
                                break;
                            case "FullDescription":
                                await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.FullDescription, property.StringValue, language.Id);
                                break;
                            case "MetaKeywords":
                                await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaKeywords, property.StringValue, language.Id);
                                break;
                            case "MetaDescription":
                                await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaDescription, property.StringValue, language.Id);
                                break;
                            case "MetaTitle":
                                await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaTitle, property.StringValue, language.Id);
                                break;
                            case "SeName":
                                //search engine name
                                var localizedSeName = await _urlRecordService.ValidateSeNameAsync(product, property.StringValue, localizedName, false);
                                await _urlRecordService.SaveSlugAsync(product, localizedSeName, language.Id);
                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get excel workbook metadata
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="workbook">Excel workbook</param>
        /// <param name="languages">Languages</param>
        /// <returns>Workbook metadata</returns>
        public static WorkbookMetadata<T> GetWorkbookMetadata<T>(IXLWorkbook workbook, IList<Language> languages)
        {
            // get the first worksheet in the workbook
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new ZarayeException("No worksheet found");

            var properties = new List<PropertyByName<T, Language>>();
            var localizedProperties = new List<PropertyByName<T, Language>>();
            var localizedWorksheets = new List<IXLWorksheet>();

            var poz = 1;
            while (true)
            {
                try
                {
                    var cell = worksheet.Row(1).Cell(poz);

                    if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                        break;

                    poz += 1;
                    properties.Add(new PropertyByName<T, Language>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            foreach (var ws in workbook.Worksheets.Skip(1))
                if (languages.Any(l => l.UniqueSeoCode.Equals(ws.Name, StringComparison.InvariantCultureIgnoreCase)))
                    localizedWorksheets.Add(ws);

            if (localizedWorksheets.Any())
            {
                // get the first worksheet in the workbook
                var localizedWorksheet = localizedWorksheets.First();

                poz = 1;
                while (true)
                {
                    try
                    {
                        var cell = localizedWorksheet.Row(1).Cell(poz);

                        if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                            break;

                        poz += 1;
                        localizedProperties.Add(new PropertyByName<T, Language>(cell.Value.ToString()));
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            return new WorkbookMetadata<T>()
            {
                DefaultProperties = properties,
                LocalizedProperties = localizedProperties,
                DefaultWorksheet = worksheet,
                LocalizedWorksheets = localizedWorksheets
            };
        }

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportProductsFromXlsxAsync(Stream stream)
        {
            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            using var workbook = new XLWorkbook(stream);
            var downloadedFiles = new List<string>();

            var metadata = await PrepareImportProductDataAsync(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;

            if (_catalogSettings.ExportImportSplitProductsFile && metadata.CountProductsInFile > _catalogSettings.ExportImportProductsCountInOneFile)
            {
                await ImportProductsFromSplitedXlsxAsync(defaultWorksheet, metadata);
                return;
            }

            //performance optimization, load all products by SKU in one SQL request
            var allProductsBySku = await _productService.GetProductsBySkuAsync(metadata.AllSku.ToArray());

            //performance optimization, load all categories IDs for products in one SQL request
            var allProductsCategoryIds = await _categoryService.GetProductCategoryIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());

            //performance optimization, load all categories in one SQL request
            Dictionary<CategoryKey, Category> allCategories;
            try
            {
                var allCategoryList = await _categoryService.GetAllCategoriesAsync(showHidden: true);

                allCategories = await allCategoryList
                    .ToDictionaryAwaitAsync(async c => await CategoryKey.CreateCategoryKeyAsync(c, _categoryService, allCategoryList, _storeMappingService), c => new ValueTask<Category>(c));
            }
            catch (ArgumentException)
            {
                //categories with the same name are not supported in the same category level
                throw new ArgumentException(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.CategoriesWithSameNameNotSupported"));
            }

            //performance optimization, load all manufacturers IDs for products in one SQL request
            var allProductsManufacturerIds = await _manufacturerService.GetProductManufacturerIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());

            //performance optimization, load all manufacturers in one SQL request
            var allManufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);

            //performance optimization, load all stores in one SQL request
            var allStores = await _storeService.GetAllStoresAsync();

            //product to import images
            var productPictureMetadata = new List<ProductPictureMetadata>();

            Product lastLoadedProduct = null;
            var typeOfExportedAttribute = ExportedAttributeType.NotSpecified;

            for (var iRow = 2; iRow < metadata.EndRow; iRow++)
            {
                if (defaultWorksheet.Row(iRow).OutlineLevel != 0)
                {
                    if (lastLoadedProduct == null)
                        continue;

                    var newTypeOfExportedAttribute = GetTypeOfExportedAttribute(defaultWorksheet, metadata.LocalizedWorksheets, metadata.ProductAttributeManager, metadata.SpecificationAttributeManager, iRow);

                    //skip caption row
                    if (newTypeOfExportedAttribute != ExportedAttributeType.NotSpecified &&
                        newTypeOfExportedAttribute != typeOfExportedAttribute)
                    {
                        typeOfExportedAttribute = newTypeOfExportedAttribute;
                        continue;
                    }

                    switch (typeOfExportedAttribute)
                    {
                        case ExportedAttributeType.ProductAttribute:
                            await ImportProductAttributeAsync(metadata, lastLoadedProduct, languages, iRow);
                            break;
                        case ExportedAttributeType.SpecificationAttribute:
                            await ImportSpecificationAttributeAsync(metadata, lastLoadedProduct, languages, iRow);
                            break;
                        case ExportedAttributeType.NotSpecified:
                        default:
                            continue;
                    }

                    continue;
                }

                metadata.Manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var product = metadata.SkuCellNum > 0 ? allProductsBySku.FirstOrDefault(p => p.Sku == metadata.Manager.GetDefaultProperty("SKU").StringValue) : null;

                var isNew = product == null;

                product ??= new Product();

                //some of previous values
                var previousStockQuantity = product.StockQuantity;
                var previousWarehouseId = product.WarehouseId;
                var prevTotalStockQuantity = await _productService.GetTotalStockQuantityAsync(product);

                if (isNew)
                    product.CreatedOnUtc = DateTime.UtcNow;

                foreach (var property in metadata.Manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "ProductType":
                            product.ProductTypeId = property.IntValue;
                            break;
                        case "ParentGroupedProductId":
                            product.ParentGroupedProductId = property.IntValue;
                            break;
                        case "VisibleIndividually":
                            product.VisibleIndividually = property.BooleanValue;
                            break;
                        case "Name":
                            product.Name = property.StringValue;
                            break;
                        case "ShortDescription":
                            product.ShortDescription = property.StringValue;
                            break;
                        case "FullDescription":
                            product.FullDescription = property.StringValue;
                            break;
                        case "ProductTemplate":
                            product.ProductTemplateId = property.IntValue;
                            break;
                        case "ShowOnHomepage":
                            product.ShowOnHomepage = property.BooleanValue;
                            break;
                        case "DisplayOrder":
                            product.DisplayOrder = property.IntValue;
                            break;
                        case "MetaKeywords":
                            product.MetaKeywords = property.StringValue;
                            break;
                        case "MetaDescription":
                            product.MetaDescription = property.StringValue;
                            break;
                        case "MetaTitle":
                            product.MetaTitle = property.StringValue;
                            break;
                        case "AllowCustomerReviews":
                            product.AllowCustomerReviews = property.BooleanValue;
                            break;
                        case "Published":
                            product.Published = property.BooleanValue;
                            break;
                        case "SKU":
                            product.Sku = property.StringValue;
                            break;
                        case "ManufacturerPartNumber":
                            product.ManufacturerPartNumber = property.StringValue;
                            break;
                        case "Gtin":
                            product.Gtin = property.StringValue;
                            break;
                        case "RequireOtherProducts":
                            product.RequireOtherProducts = property.BooleanValue;
                            break;
                        case "RequiredProductIds":
                            product.RequiredProductIds = property.StringValue;
                            break;
                        case "AutomaticallyAddRequiredProducts":
                            product.AutomaticallyAddRequiredProducts = property.BooleanValue;
                            break;
                        case "IsDownload":
                            product.IsDownload = property.BooleanValue;
                            break;
                        case "DownloadId":
                            product.DownloadId = property.IntValue;
                            break;
                        case "UnlimitedDownloads":
                            product.UnlimitedDownloads = property.BooleanValue;
                            break;
                        case "MaxNumberOfDownloads":
                            product.MaxNumberOfDownloads = property.IntValue;
                            break;
                        case "DownloadActivationType":
                            product.DownloadActivationTypeId = property.IntValue;
                            break;
                        case "HasSampleDownload":
                            product.HasSampleDownload = property.BooleanValue;
                            break;
                        case "SampleDownloadId":
                            product.SampleDownloadId = property.IntValue;
                            break;
                        case "HasUserAgreement":
                            product.HasUserAgreement = property.BooleanValue;
                            break;
                        case "UserAgreementText":
                            product.UserAgreementText = property.StringValue;
                            break;
                        case "IsRecurring":
                            product.IsRecurring = property.BooleanValue;
                            break;
                        case "RecurringCycleLength":
                            product.RecurringCycleLength = property.IntValue;
                            break;
                        case "RecurringCyclePeriod":
                            product.RecurringCyclePeriodId = property.IntValue;
                            break;
                        case "RecurringTotalCycles":
                            product.RecurringTotalCycles = property.IntValue;
                            break;
                        case "IsRental":
                            product.IsRental = property.BooleanValue;
                            break;
                        case "RentalPriceLength":
                            product.RentalPriceLength = property.IntValue;
                            break;
                        case "RentalPricePeriod":
                            product.RentalPricePeriodId = property.IntValue;
                            break;
                        case "IsShipEnabled":
                            product.IsShipEnabled = property.BooleanValue;
                            break;
                        case "IsFreeShipping":
                            product.IsFreeShipping = property.BooleanValue;
                            break;
                        case "ShipSeparately":
                            product.ShipSeparately = property.BooleanValue;
                            break;
                        case "AdditionalShippingCharge":
                            product.AdditionalShippingCharge = property.DecimalValue;
                            break;
                        case "DeliveryDate":
                            product.DeliveryDateId = property.IntValue;
                            break;
                        case "IsTaxExempt":
                            product.IsTaxExempt = property.BooleanValue;
                            break;
                        case "TaxCategory":
                            product.TaxCategoryId = property.IntValue;
                            break;
                        case "IsTelecommunicationsOrBroadcastingOrElectronicServices":
                            product.IsTelecommunicationsOrBroadcastingOrElectronicServices = property.BooleanValue;
                            break;
                        case "ManageInventoryMethod":
                            product.ManageInventoryMethodId = property.IntValue;
                            break;
                        case "ProductAvailabilityRange":
                            product.ProductAvailabilityRangeId = property.IntValue;
                            break;
                        case "UseMultipleWarehouses":
                            product.UseMultipleWarehouses = property.BooleanValue;
                            break;
                        case "WarehouseId":
                            product.WarehouseId = property.IntValue;
                            break;
                        case "StockQuantity":
                            product.StockQuantity = property.IntValue;
                            break;
                        case "DisplayStockAvailability":
                            product.DisplayStockAvailability = property.BooleanValue;
                            break;
                        case "DisplayStockQuantity":
                            product.DisplayStockQuantity = property.BooleanValue;
                            break;
                        case "MinStockQuantity":
                            product.MinStockQuantity = property.IntValue;
                            break;
                        case "LowStockActivity":
                            product.LowStockActivityId = property.IntValue;
                            break;
                        case "NotifyAdminForQuantityBelow":
                            product.NotifyAdminForQuantityBelow = property.IntValue;
                            break;
                        case "BackorderMode":
                            product.BackorderModeId = property.IntValue;
                            break;
                        case "AllowBackInStockSubscriptions":
                            product.AllowBackInStockSubscriptions = property.BooleanValue;
                            break;
                        case "OrderMinimumQuantity":
                            product.OrderMinimumQuantity = property.IntValue;
                            break;
                        case "OrderMaximumQuantity":
                            product.OrderMaximumQuantity = property.IntValue;
                            break;
                        case "AllowedQuantities":
                            product.AllowedQuantities = property.StringValue;
                            break;
                        case "AllowAddingOnlyExistingAttributeCombinations":
                            product.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
                            break;
                        case "NotReturnable":
                            product.NotReturnable = property.BooleanValue;
                            break;
                        case "DisableBuyButton":
                            product.DisableBuyButton = property.BooleanValue;
                            break;
                        case "DisableWishlistButton":
                            product.DisableWishlistButton = property.BooleanValue;
                            break;
                        case "AvailableForPreOrder":
                            product.AvailableForPreOrder = property.BooleanValue;
                            break;
                        case "PreOrderAvailabilityStartDateTimeUtc":
                            product.PreOrderAvailabilityStartDateTimeUtc = property.DateTimeNullable;
                            break;
                        case "CallForPrice":
                            product.CallForPrice = property.BooleanValue;
                            break;
                        case "Price":
                            product.Price = property.DecimalValue;
                            break;
                        case "OldPrice":
                            product.OldPrice = property.DecimalValue;
                            break;
                        case "ProductCost":
                            product.ProductCost = property.DecimalValue;
                            break;
                        case "CustomerEntersPrice":
                            product.CustomerEntersPrice = property.BooleanValue;
                            break;
                        case "MinimumCustomerEnteredPrice":
                            product.MinimumCustomerEnteredPrice = property.DecimalValue;
                            break;
                        case "MaximumCustomerEnteredPrice":
                            product.MaximumCustomerEnteredPrice = property.DecimalValue;
                            break;
                        case "BasepriceEnabled":
                            product.BasepriceEnabled = property.BooleanValue;
                            break;
                        case "BasepriceAmount":
                            product.BasepriceAmount = property.DecimalValue;
                            break;
                        case "BasepriceUnit":
                            product.BasepriceUnitId = property.IntValue;
                            break;
                        case "BasepriceBaseAmount":
                            product.BasepriceBaseAmount = property.DecimalValue;
                            break;
                        case "BasepriceBaseUnit":
                            product.BasepriceBaseUnitId = property.IntValue;
                            break;
                        case "MarkAsNew":
                            product.MarkAsNew = property.BooleanValue;
                            break;
                        case "MarkAsNewStartDateTimeUtc":
                            product.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
                            break;
                        case "MarkAsNewEndDateTimeUtc":
                            product.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
                            break;
                        case "Weight":
                            product.Weight = property.DecimalValue;
                            break;
                        case "Length":
                            product.Length = property.DecimalValue;
                            break;
                        case "Width":
                            product.Width = property.DecimalValue;
                            break;
                        case "Height":
                            product.Height = property.DecimalValue;
                            break;
                        case "IsLimitedToStores":
                            product.LimitedToStores = property.BooleanValue;
                            break;
                    }
                }

                //set some default values if not specified
                if (isNew && metadata.Properties.All(p => p.PropertyName != "ProductType"))
                    product.ProductType = ProductType.SimpleProduct;
                if (isNew && metadata.Properties.All(p => p.PropertyName != "VisibleIndividually"))
                    product.VisibleIndividually = true;
                if (isNew && metadata.Properties.All(p => p.PropertyName != "Published"))
                    product.Published = true;

                product.UpdatedOnUtc = DateTime.UtcNow;

                if (isNew)
                    await _productService.InsertProductAsync(product);
                else
                    await _productService.UpdateProductAsync(product);

                //quantity change history
                if (isNew || previousWarehouseId == product.WarehouseId)
                {
                    await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                        product.WarehouseId, await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ImportProduct.Edit"));
                }
                //warehouse is changed 
                else
                {
                    //compose a message
                    var oldWarehouseMessage = string.Empty;
                    if (previousWarehouseId > 0)
                    {
                        var oldWarehouse = await _shippingService.GetWarehouseByIdAsync(previousWarehouseId);
                        if (oldWarehouse != null)
                            oldWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                    }

                    var newWarehouseMessage = string.Empty;
                    if (product.WarehouseId > 0)
                    {
                        var newWarehouse = await _shippingService.GetWarehouseByIdAsync(product.WarehouseId);
                        if (newWarehouse != null)
                            newWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                    }

                    var message = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ImportProduct.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    await _productService.AddStockQuantityHistoryEntryAsync(product, -previousStockQuantity, 0, previousWarehouseId, message);
                    await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);
                }

                if (!isNew &&
                    product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    product.BackorderMode == BackorderMode.NoBackorders &&
                    product.AllowBackInStockSubscriptions &&
                    await _productService.GetTotalStockQuantityAsync(product) > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    product.Published &&
                    !product.Deleted)
                {
                    await _backInStockSubscriptionService.SendNotificationsToSubscribersAsync(product);
                }

                var tempProperty = metadata.Manager.GetDefaultProperty("SeName");

                //search engine name
                var seName = tempProperty?.StringValue ?? (isNew ? string.Empty : await _urlRecordService.GetSeNameAsync(product, 0));
                await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(product, seName, product.Name, true), 0);

                //save product localized data
                await ImportProductLocalizedAsync(product, metadata, iRow, languages);

                tempProperty = metadata.Manager.GetDefaultProperty("Categories");

                if (tempProperty != null)
                {
                    var categoryList = tempProperty.StringValue;

                    //category mappings
                    var categories = isNew || !allProductsCategoryIds.ContainsKey(product.Id) ? Array.Empty<int>() : allProductsCategoryIds[product.Id];

                    var storesIds = product.LimitedToStores
                        ? (await _storeMappingService.GetStoresIdsWithAccessAsync(product)).ToList()
                        : new List<int>();

                    var importedCategories = await categoryList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(categoryName => new CategoryKey(categoryName, storesIds))
                        .SelectAwait(async categoryKey =>
                        {
                            var rez = (allCategories.ContainsKey(categoryKey) ? allCategories[categoryKey].Id : allCategories.Values.FirstOrDefault(c => c.Name == categoryKey.Key)?.Id) ??
                                      allCategories.FirstOrDefault(p =>
                                    p.Key.Key.Equals(categoryKey.Key, StringComparison.InvariantCultureIgnoreCase))
                                .Value?.Id;

                            if (!rez.HasValue && int.TryParse(categoryKey.Key, out var id))
                                rez = id;

                            //TODO: Let's ignore these category and write a log message instead of interrupting the import of the product
                            if (!rez.HasValue)
                                //database doesn't contain the imported category
                                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.DatabaseNotContainCategory"), categoryKey.Key));

                            return rez.Value;
                        }).ToListAsync();

                    foreach (var categoryId in importedCategories)
                    {
                        if (categories.Any(c => c == categoryId))
                            continue;

                        var productCategory = new ProductCategory
                        {
                            ProductId = product.Id,
                            CategoryId = categoryId,
                            IsFeaturedProduct = false,
                            DisplayOrder = 1
                        };
                        await _categoryService.InsertProductCategoryAsync(productCategory);
                    }

                    //delete product categories
                    var deletedProductCategories = await categories.Where(categoryId => !importedCategories.Contains(categoryId))
                        .SelectAwait(async categoryId => (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true)).FirstOrDefault(pc => pc.CategoryId == categoryId)).Where(pc => pc != null).ToListAsync();

                    foreach (var deletedProductCategory in deletedProductCategories)
                        await _categoryService.DeleteProductCategoryAsync(deletedProductCategory);
                }

                tempProperty = metadata.Manager.GetDefaultProperty("Manufacturers");
                if (tempProperty != null)
                {
                    var manufacturerList = tempProperty.StringValue;

                    //manufacturer mappings
                    var manufacturers = isNew || !allProductsManufacturerIds.ContainsKey(product.Id) ? Array.Empty<int>() : allProductsManufacturerIds[product.Id];

                    int? getManufacturerId(string x)
                    {
                        var id = allManufacturers.FirstOrDefault(m => m.Name == x.Trim())?.Id;

                        if (id != null)
                            return id;

                        //TODO: we should add a log message for situation where not exists manufacturer
                        return int.TryParse(x, out var parsedId) ? parsedId : null;
                    }

                    var importedManufacturers = manufacturerList
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(getManufacturerId).Where(id => id.HasValue).ToList();

                    foreach (var manufacturerId in importedManufacturers)
                    {
                        if (manufacturers.Any(c => c == manufacturerId))
                            continue;

                        var productManufacturer = new ProductManufacturer
                        {
                            ProductId = product.Id,
                            ManufacturerId = manufacturerId.Value,
                            IsFeaturedProduct = false,
                            DisplayOrder = 1
                        };
                        await _manufacturerService.InsertProductManufacturerAsync(productManufacturer);
                    }

                    //delete product manufacturers
                    var deletedProductsManufacturers = await manufacturers.Where(manufacturerId => !importedManufacturers.Contains(manufacturerId))
                        .SelectAwait(async manufacturerId => (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id)).FirstOrDefault(pc => pc.ManufacturerId == manufacturerId)).ToListAsync();
                    foreach (var deletedProductManufacturer in deletedProductsManufacturers.Where(m => m != null))
                        await _manufacturerService.DeleteProductManufacturerAsync(deletedProductManufacturer);
                }

                tempProperty = metadata.Manager.GetDefaultProperty("ProductTags");
                if (tempProperty != null)
                {
                    var productTags = tempProperty.StringValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

                    //searching existing product tags by their id
                    var productTagIds = productTags.Where(pt => int.TryParse(pt, out var _)).Select(int.Parse);

                    var productTagsByIds = (await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Where(pt => productTagIds.Contains(pt.Id)).ToList();

                    productTags.AddRange(productTagsByIds.Select(pt => pt.Name));
                    var filter = productTagsByIds.Select(pt => pt.Id.ToString()).ToList();

                    //product tag mappings
                    await _productTagService.UpdateProductTagsAsync(product, productTags.Where(pt => !filter.Contains(pt)).ToArray());
                }

                tempProperty = metadata.Manager.GetDefaultProperty("LimitedToStores");
                if (tempProperty != null)
                {
                    var limitedToStoresList = tempProperty.StringValue;

                    var importedStores = product.LimitedToStores ? limitedToStoresList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => allStores.FirstOrDefault(store => store.Name == x.Trim())?.Id ?? int.Parse(x.Trim())).ToList() : new List<int>();

                    await _productService.UpdateProductStoreMappingsAsync(product, importedStores);
                }

                var picture1 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture1")?.StringValue, downloadedFiles);
                var picture2 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture2")?.StringValue, downloadedFiles);
                var picture3 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture3")?.StringValue, downloadedFiles);

                productPictureMetadata.Add(new ProductPictureMetadata
                {
                    ProductItem = product,
                    Picture1Path = picture1,
                    Picture2Path = picture2,
                    Picture3Path = picture3,
                    IsNew = isNew
                });

                lastLoadedProduct = product;

                //update "HasTierPrices" and "HasDiscountsApplied" properties
                //_productService.UpdateHasTierPricesProperty(product);
                //_productService.UpdateHasDiscountsApplied(product);
            }

            if (_mediaSettings.ImportProductImagesUsingHash && await _pictureService.IsStoreInDbAsync())
                await ImportProductImagesUsingHashAsync(productPictureMetadata, allProductsBySku);
            else
                await ImportProductImagesUsingServicesAsync(productPictureMetadata);

            foreach (var downloadedFile in downloadedFiles)
            {
                if (!_fileProvider.FileExists(downloadedFile))
                    continue;

                try
                {
                    _fileProvider.DeleteFile(downloadedFile);
                }
                catch
                {
                    // ignored
                }
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("ImportProducts", string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportProducts"), metadata.CountProductsInFile));
        }

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of imported subscribers
        /// </returns>
        public virtual async Task<int> ImportNewsletterSubscribersFromTxtAsync(Stream stream)
        {
            var count = 0;
            using (var reader = new StreamReader(stream))
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var tmp = line.Split(',');

                    if (tmp.Length > 3)
                        throw new ZarayeException("Wrong file format");

                    var isActive = true;

                    var store = await _storeContext.GetCurrentStoreAsync();
                    var storeId = store.Id;

                    //"email" field specified
                    var email = tmp[0].Trim();

                    if (!CommonHelper.IsValidEmail(email))
                        continue;

                    //"active" field specified
                    if (tmp.Length >= 2)
                        isActive = bool.Parse(tmp[1].Trim());

                    //"storeId" field specified
                    if (tmp.Length == 3)
                        storeId = int.Parse(tmp[2].Trim());

                    //import
                    var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, storeId);
                    if (subscription != null)
                    {
                        subscription.Email = email;
                        subscription.Active = isActive;
                        await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                    }
                    else
                    {
                        subscription = new NewsLetterSubscription
                        {
                            Active = isActive,
                            CreatedOnUtc = DateTime.UtcNow,
                            Email = email,
                            StoreId = storeId,
                            NewsLetterSubscriptionGuid = Guid.NewGuid()
                        };
                        await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                    }

                    count++;
                }

            await _customerActivityService.InsertActivityAsync("ImportNewsLetterSubscriptions",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportNewsLetterSubscriptions"), count));

            return count;
        }

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="writeLog">Indicates whether to add logging</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of imported states
        /// </returns>
        public virtual async Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true)
        {
            var count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var tmp = line.Split(',');

                    if (tmp.Length != 5)
                        throw new ZarayeException("Wrong file format");

                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    var published = bool.Parse(tmp[3].Trim());
                    var displayOrder = int.Parse(tmp[4].Trim());

                    var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryTwoLetterIsoCode);
                    if (country == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true);
                    var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (state != null)
                    {
                        state.Abbreviation = abbreviation;
                        state.Published = published;
                        state.DisplayOrder = displayOrder;
                        await _stateProvinceService.UpdateStateProvinceAsync(state);
                    }
                    else
                    {
                        state = new StateProvince
                        {
                            CountryId = country.Id,
                            Name = name,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplayOrder = displayOrder
                        };
                        await _stateProvinceService.InsertStateProvinceAsync(state);
                    }

                    count++;
                }
            }

            //activity log
            if (writeLog)
            {
                await _customerActivityService.InsertActivityAsync("ImportStates",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportStates"), count));
            }

            return count;
        }

        /// <summary>
        /// Import manufacturers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportManufacturersFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Manufacturer>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Manufacturer, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;
            var setSeName = defaultProperties.Any(p => p.PropertyName == "SeName");

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                    .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manager.GetDefaultProperty("Id").IntValue);

                var isNew = manufacturer == null;

                manufacturer ??= new Manufacturer();

                if (isNew)
                {
                    manufacturer.CreatedOnUtc = DateTime.UtcNow;

                    //default values
                    manufacturer.PageSize = _catalogSettings.DefaultManufacturerPageSize;
                    manufacturer.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
                    manufacturer.Published = true;
                    manufacturer.AllowCustomersToSelectPageSize = true;
                }

                var seName = string.Empty;

                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "Name":
                            manufacturer.Name = property.StringValue;
                            break;
                        case "Description":
                            manufacturer.Description = property.StringValue;
                            break;
                        case "ManufacturerTemplateId":
                            manufacturer.ManufacturerTemplateId = property.IntValue;
                            break;
                        case "MetaKeywords":
                            manufacturer.MetaKeywords = property.StringValue;
                            break;
                        case "MetaDescription":
                            manufacturer.MetaDescription = property.StringValue;
                            break;
                        case "MetaTitle":
                            manufacturer.MetaTitle = property.StringValue;
                            break;
                        case "Picture":
                            var picture = await LoadPictureAsync(manager.GetDefaultProperty("Picture").StringValue, manufacturer.Name, isNew ? null : (int?)manufacturer.PictureId);

                            if (picture != null)
                                manufacturer.PictureId = picture.Id;

                            break;
                        case "PageSize":
                            manufacturer.PageSize = property.IntValue;
                            break;
                        case "AllowCustomersToSelectPageSize":
                            manufacturer.AllowCustomersToSelectPageSize = property.BooleanValue;
                            break;
                        case "PageSizeOptions":
                            manufacturer.PageSizeOptions = property.StringValue;
                            break;
                        case "PriceRangeFiltering":
                            manufacturer.PriceRangeFiltering = property.BooleanValue;
                            break;
                        case "PriceFrom":
                            manufacturer.PriceFrom = property.DecimalValue;
                            break;
                        case "PriceTo":
                            manufacturer.PriceTo = property.DecimalValue;
                            break;
                        case "AutomaticallyCalculatePriceRange":
                            manufacturer.ManuallyPriceRange = property.BooleanValue;
                            break;
                        case "Published":
                            manufacturer.Published = property.BooleanValue;
                            break;
                        case "DisplayOrder":
                            manufacturer.DisplayOrder = property.IntValue;
                            break;
                        case "SeName":
                            seName = property.StringValue;
                            break;
                    }
                }

                manufacturer.UpdatedOnUtc = DateTime.UtcNow;

                if (isNew)
                    await _manufacturerService.InsertManufacturerAsync(manufacturer);
                else
                    await _manufacturerService.UpdateManufacturerAsync(manufacturer);

                //search engine name
                if (setSeName)
                    await _urlRecordService.SaveSlugAsync(manufacturer, await _urlRecordService.ValidateSeNameAsync(manufacturer, seName, manufacturer.Name, true), 0);

                //save manufacturer localized data
                await ImportManufaturerLocalizedAsync(manufacturer, metadata, manager, iRow, languages);

                iRow++;
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("ImportManufacturers",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportManufacturers"), iRow - 2));
        }

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportCategoriesFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Category>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Category, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;
            var setSeName = defaultProperties.Any(p => p.PropertyName == "SeName");

            //performance optimization, load all categories in one SQL request
            var allCategories = await (await _categoryService
                .GetAllCategoriesAsync(showHidden: true))
                .GroupByAwait(async c => await _categoryService.GetFormattedBreadCrumbAsync(c))
                .ToDictionaryAsync(c => c.Key, c => c.FirstAsync());

            var saveNextTime = new List<int>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                //get category by data in xlsx file if it possible, or create new category
                var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, defaultWorksheet, iRow, allCategories);

                //update category by data in xlsx file
                var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, isNew);

                if (isParentCategoryExists)
                {
                    //if parent category exists in database then save category into database
                    await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);

                    //save category localized data
                    await ImportCategoryLocalizedAsync(category, metadata, manager, iRow, languages);
                }
                else
                {
                    //if parent category doesn't exists in database then try save category into database next time
                    saveNextTime.Add(iRow);
                }

                iRow++;
            }

            var needSave = saveNextTime.Any();

            while (needSave)
            {
                var remove = new List<int>();

                //try to save unsaved categories
                foreach (var rowId in saveNextTime)
                {
                    //get category by data in xlsx file if it possible, or create new category
                    var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, defaultWorksheet, rowId, allCategories);
                    //update category by data in xlsx file
                    var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, isNew);

                    if (!isParentCategoryExists)
                        continue;

                    //if parent category exists in database then save category into database
                    await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);

                    //save category localized data
                    await ImportCategoryLocalizedAsync(category, metadata, manager, rowId, languages);

                    remove.Add(rowId);
                }

                saveNextTime.RemoveAll(item => remove.Contains(item));

                needSave = remove.Any() && saveNextTime.Any();
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("ImportCategories",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportCategories"), iRow - 2 - saveNextTime.Count));

            if (!saveNextTime.Any())
                return;

            var categoriesName = new List<string>();

            foreach (var rowId in saveNextTime)
            {
                manager.ReadDefaultFromXlsx(defaultWorksheet, rowId);
                categoriesName.Add(manager.GetDefaultProperty("Name").StringValue);
            }

            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Import.CategoriesArentImported"), string.Join(", ", categoriesName)));
        }

        public virtual async Task ImportCampaignEmailsFromXlsxAsync(Stream stream, Campaign campaign)
        {
            using var workbook = new XLWorkbook(stream);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Category>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            //the columns
            //var properties = GetPropertiesByExcelCells<CampaignEmail>(worksheet);

            //var manager = new PropertyManager<CampaignEmail>(properties, _catalogSettings);

            var manager = new PropertyManager<Category, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            var campaignEmaillist = new List<CampaignEmailModel>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                   .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                   .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var campaignEmailModel = new CampaignEmailModel();

                foreach (var property in manager.GetDefaultProperties)
                {


                    switch (property.PropertyName)
                    {
                        case "Email":
                            campaignEmailModel.Email = property.StringValue.Trim();
                            break;

                        case "Active":
                            campaignEmailModel.Active = property.BooleanValue;
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(campaignEmailModel.Email))
                    campaignEmaillist.Add(campaignEmailModel);

                iRow++;
            }

            foreach (var list in campaignEmaillist)
            {
                //try to get a campaign with the specified id
                var campaignemail = await _campaignService.GetCampaignByEmailAsync(campaign.Id, list.Email);
                if (campaignemail == null)
                {
                    await _campaignService.InsertCampaignEmailAsync(new CampaignEmail
                    {
                        CampaignId = campaign.Id,
                        Email = list.Email,
                        Active = list.Active,
                        CreatedOnUtc = DateTime.UtcNow,
                    });

                }
                else
                {
                    campaignemail.Active = list.Active;
                    await _campaignService.UpdateCampaignEmailAsync(campaignemail);
                }
            }
        }

        /// <summary>
        /// Import orders from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportOrdersFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var downloadedFiles = new List<string>();

            (var metadata, var worksheet) = await PrepareImportOrderDataAsync(workbook);

            //performance optimization, load all orders by guid in one SQL request
            var allOrdersByGuids = await _orderService.GetOrdersByGuidsAsync(metadata.AllOrderGuids.ToArray());

            //performance optimization, load all customers by guid in one SQL request
            var allCustomersByGuids = await _customerService.GetCustomersByGuidsAsync(metadata.AllCustomerGuids.ToArray());

            Order lastLoadedOrder = null;

            for (var iRow = 2; iRow < metadata.EndRow; iRow++)
            {
                //imports product attributes
                if (worksheet.Row(iRow).OutlineLevel != 0)
                {
                    if (lastLoadedOrder == null)
                        continue;

                    metadata.OrderItemManager.ReadDefaultFromXlsx(worksheet, iRow, 2);

                    //skip caption row
                    if (!metadata.OrderItemManager.IsCaption)
                    {
                        await ImportOrderItemAsync(metadata.OrderItemManager, lastLoadedOrder);
                    }
                    continue;
                }

                metadata.Manager.ReadDefaultFromXlsx(worksheet, iRow);

                var order = allOrdersByGuids.FirstOrDefault(p => p.OrderGuid == metadata.Manager.GetDefaultProperty("OrderGuid").GuidValue);

                var isNew = order == null;

                order ??= new Order();

                Address orderBillingAddress = null;
                Address orderAddress = null;

                if (isNew)
                    order.CreatedOnUtc = DateTime.UtcNow;
                else
                {
                    orderBillingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                    orderAddress = await _addressService.GetAddressByIdAsync((order.PickupInStore ? order.PickupAddressId : order.ShippingAddressId) ?? 0);
                }

                orderBillingAddress ??= new Address();
                orderAddress ??= new Address();

                var customer = allCustomersByGuids.FirstOrDefault(p => p.CustomerGuid.ToString() == metadata.Manager.GetDefaultProperty("CustomerGuid").StringValue);

                foreach (var property in metadata.Manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "StoreId":
                            if (await _storeService.GetStoreByIdAsync(property.IntValue) is Store orderStore)
                                order.StoreId = property.IntValue;
                            else
                                order.StoreId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
                            break;
                        case "OrderGuid":
                            order.OrderGuid = property.GuidValue;
                            break;
                        case "CustomerId":
                            order.CustomerId = customer?.Id ?? 0;
                            break;
                        case "OrderStatus":
                            order.OrderStatus = (OrderStatus)property.PropertyValue;
                            break;
                        case "PaymentStatus":
                            order.PaymentStatus = (PaymentStatus)property.PropertyValue;
                            break;
                        case "ShippingStatus":
                            order.ShippingStatus = (ShippingStatus)property.PropertyValue;
                            break;
                        case "OrderSubtotalInclTax":
                            order.OrderSubtotalInclTax = property.DecimalValue;
                            break;
                        case "OrderSubtotalExclTax":
                            order.OrderSubtotalExclTax = property.DecimalValue;
                            break;
                        case "OrderSubTotalDiscountInclTax":
                            order.OrderSubTotalDiscountInclTax = property.DecimalValue;
                            break;
                        case "OrderSubTotalDiscountExclTax":
                            order.OrderSubTotalDiscountExclTax = property.DecimalValue;
                            break;
                        case "OrderShippingInclTax":
                            order.OrderShippingInclTax = property.DecimalValue;
                            break;
                        case "OrderShippingExclTax":
                            order.OrderShippingExclTax = property.DecimalValue;
                            break;
                        case "PaymentMethodAdditionalFeeInclTax":
                            order.PaymentMethodAdditionalFeeInclTax = property.DecimalValue;
                            break;
                        case "PaymentMethodAdditionalFeeExclTax":
                            order.PaymentMethodAdditionalFeeExclTax = property.DecimalValue;
                            break;
                        case "TaxRates":
                            order.TaxRates = property.StringValue;
                            break;
                        case "OrderTax":
                            order.OrderTax = property.DecimalValue;
                            break;
                        case "OrderTotal":
                            order.OrderTotal = property.DecimalValue;
                            break;
                        case "RefundedAmount":
                            order.RefundedAmount = property.DecimalValue;
                            break;
                        case "OrderDiscount":
                            order.OrderDiscount = property.DecimalValue;
                            break;
                        case "CurrencyRate":
                            order.CurrencyRate = property.DecimalValue;
                            break;
                        case "CustomerCurrencyCode":
                            order.CustomerCurrencyCode = property.StringValue;
                            break;
                        case "AffiliateId":
                            order.AffiliateId = property.IntValue;
                            break;
                        case "PaymentMethodSystemName":
                            order.PaymentMethodSystemName = property.StringValue;
                            break;
                        case "ShippingPickupInStore":
                            order.PickupInStore = property.BooleanValue;
                            break;
                        case "ShippingMethod":
                            order.ShippingMethod = property.StringValue;
                            break;
                        case "ShippingRateComputationMethodSystemName":
                            order.ShippingRateComputationMethodSystemName = property.StringValue;
                            break;
                        case "CustomValuesXml":
                            order.CustomValuesXml = property.StringValue;
                            break;
                        case "VatNumber":
                            order.VatNumber = property.StringValue;
                            break;
                        case "CreatedOnUtc":
                            order.CreatedOnUtc = DateTime.TryParse(property.StringValue, out var createdOnUtc) ? createdOnUtc : DateTime.UtcNow;
                            break;
                        case "BillingFirstName":
                            orderBillingAddress.FirstName = property.StringValue;
                            break;
                        case "BillingLastName":
                            orderBillingAddress.LastName = property.StringValue;
                            break;
                        case "BillingPhoneNumber":
                            orderBillingAddress.PhoneNumber = property.StringValue;
                            break;
                        case "BillingEmail":
                            orderBillingAddress.Email = property.StringValue;
                            break;
                        case "BillingFaxNumber":
                            orderBillingAddress.FaxNumber = property.StringValue;
                            break;
                        case "BillingCompany":
                            orderBillingAddress.Company = property.StringValue;
                            break;
                        case "BillingAddress1":
                            orderBillingAddress.Address1 = property.StringValue;
                            break;
                        case "BillingAddress2":
                            orderBillingAddress.Address2 = property.StringValue;
                            break;
                        case "BillingCity":
                            orderBillingAddress.City = property.StringValue;
                            break;
                        case "BillingCounty":
                            orderBillingAddress.County = property.StringValue;
                            break;
                        case "BillingStateProvinceAbbreviation":
                            if (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(property.StringValue) is StateProvince billingState)
                                orderBillingAddress.StateProvinceId = billingState.Id;
                            break;
                        case "BillingZipPostalCode":
                            orderBillingAddress.ZipPostalCode = property.StringValue;
                            break;
                        case "BillingCountryCode":
                            if (await _countryService.GetCountryByTwoLetterIsoCodeAsync(property.StringValue) is Country billingCountry)
                                orderBillingAddress.CountryId = billingCountry.Id;
                            break;
                        case "ShippingFirstName":
                            orderAddress.FirstName = property.StringValue;
                            break;
                        case "ShippingLastName":
                            orderAddress.LastName = property.StringValue;
                            break;
                        case "ShippingPhoneNumber":
                            orderAddress.PhoneNumber = property.StringValue;
                            break;
                        case "ShippingEmail":
                            orderAddress.Email = property.StringValue;
                            break;
                        case "ShippingFaxNumber":
                            orderAddress.FaxNumber = property.StringValue;
                            break;
                        case "ShippingCompany":
                            orderAddress.Company = property.StringValue;
                            break;
                        case "ShippingAddress1":
                            orderAddress.Address1 = property.StringValue;
                            break;
                        case "ShippingAddress2":
                            orderAddress.Address2 = property.StringValue;
                            break;
                        case "ShippingCity":
                            orderAddress.City = property.StringValue;
                            break;
                        case "ShippingCounty":
                            orderAddress.County = property.StringValue;
                            break;
                        case "ShippingStateProvinceAbbreviation":
                            if (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(property.StringValue) is StateProvince shippingState)
                                orderAddress.StateProvinceId = shippingState.Id;
                            break;
                        case "ShippingZipPostalCode":
                            orderAddress.ZipPostalCode = property.StringValue;
                            break;
                        case "ShippingCountryCode":
                            if (await _countryService.GetCountryByTwoLetterIsoCodeAsync(property.StringValue) is Country shippingCountry)
                                orderAddress.CountryId = shippingCountry.Id;
                            break;
                    }
                }

                //check order address field values from excel
                if (string.IsNullOrWhiteSpace(orderAddress.FirstName) && string.IsNullOrWhiteSpace(orderAddress.LastName) && string.IsNullOrWhiteSpace(orderAddress.Email))
                    orderAddress = null;

                //insert or update billing address
                if (orderBillingAddress.Id == 0)
                {
                    await _addressService.InsertAddressAsync(orderBillingAddress);
                    order.BillingAddressId = orderBillingAddress.Id;
                }
                else
                    await _addressService.UpdateAddressAsync(orderBillingAddress);

                //insert or update shipping/pickup address
                if (orderAddress != null)
                {
                    if (orderAddress.Id == 0)
                    {
                        await _addressService.InsertAddressAsync(orderAddress);

                        if (order.PickupInStore)
                            order.PickupAddressId = orderAddress.Id;
                        else
                            order.ShippingAddressId = orderAddress.Id;
                    }
                    else
                        await _addressService.UpdateAddressAsync(orderAddress);
                }
                else
                    order.ShippingAddressId = null;

                //set some default values if not specified
                if (isNew)
                {
                    //customer language
                    var customerLanguage = await _languageService.GetLanguageByIdAsync(customer?.LanguageId ?? 0);
                    if (customerLanguage == null || !customerLanguage.Published)
                        customerLanguage = await _workContext.GetWorkingLanguageAsync();
                    order.CustomerLanguageId = customerLanguage.Id;

                    //tax display type
                    order.CustomerTaxDisplayType = _taxSettings.TaxDisplayType;

                    //set other default values
                    order.AllowStoringCreditCardNumber = false;
                    order.AuthorizationTransactionCode = string.Empty;
                    order.AuthorizationTransactionId = string.Empty;
                    order.AuthorizationTransactionResult = string.Empty;
                    order.CaptureTransactionId = string.Empty;
                    order.CaptureTransactionResult = string.Empty;
                    order.CardCvv2 = string.Empty;
                    order.CardExpirationMonth = string.Empty;
                    order.CardExpirationYear = string.Empty;
                    order.CardName = string.Empty;
                    order.CardNumber = string.Empty;
                    order.CardType = string.Empty;
                    order.CustomerIp = string.Empty;
                    order.CustomOrderNumber = string.Empty;
                    order.MaskedCreditCardNumber = string.Empty;
                    order.RefundedAmount = decimal.Zero;
                    order.SubscriptionTransactionId = string.Empty;

                    await _orderService.InsertOrderAsync(order);

                    //generate and set custom order number
                    order.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(order);
                    await _orderService.UpdateOrderAsync(order);
                }
                else
                    await _orderService.UpdateOrderAsync(order);

                lastLoadedOrder = order;
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("ImportOrders", string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportOrders"), metadata.CountOrdersInFile));
        }

        public virtual async Task ImportAreasFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Manufacturer>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Manufacturer, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            var areaList = new List<AreaModel>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                     .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                     .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var area = new AreaModel();

                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "Area":
                            area.Area = property.StringValue.Trim();
                            break;
                        case "City":
                            area.City = property.StringValue.Trim();
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(area.Area) && !string.IsNullOrWhiteSpace(area.City))
                    areaList.Add(area);
                iRow++;
            }

            foreach (var arealist in areaList)
            {
                var city = await _stateProvinceService.GetStateProvinceByNameAsync(arealist.City, 169);
                if (city == null)
                    continue;

                var existingArea = await _stateProvinceService.GetAreaByNameAsync(arealist.Area, city.Id, 169);
                if (existingArea == null)
                {
                    await _stateProvinceService.InsertStateProvinceAsync(new StateProvince
                    {
                        Name = arealist.Area,
                        CountryId = 169,
                        Published = true,
                        ParentId = city.Id,
                        DisplayOrder = 0
                    });
                }
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("ImportAreaa",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportAreaa"), iRow - 2));
        }

        public virtual async Task ImportBuyerTypeFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<BuyerTypeData>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<BuyerTypeData, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            var buyerDataList = new List<BuyerTypeData>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var buyerData = new BuyerTypeData();

                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "Buyer ID":
                            buyerData.BuyerId = property.IntValue;
                            break;
                        case "BuyerType":
                            buyerData.BuyerType = property.StringValue;
                            break;
                    }
                }

                buyerDataList.Add(buyerData);
                iRow++;
            }

            foreach (var data in buyerDataList)
            {
                var buyer = await _customerService.GetCustomerByIdAsync(data.BuyerId);
                if (buyer == null)
                    continue;

                if (!string.IsNullOrWhiteSpace(data.BuyerType))
                {
                    var buyerType = await _customerService.GetUserTypeByNameAsync(data.BuyerType);
                    if (buyerType is not null)
                        await _genericAttributeService.SaveAttributeAsync(buyer, ZarayeCustomerDefaults.BuyerTypeIdAttribute, buyerType.Id);
                }
            }
        }

        public virtual async Task ImportRequestFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            await _dataProvider.SetTableIdentAsync<Request>(1);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Request>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Request, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            var requestList = new List<Request>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var requestData = new Request();
                var id = 0;
                var brand = "";
                //DateTime? deliveryDate = DateTime.UtcNow;
                //DateTime? updatedOn = DateTime.UtcNow;
                //DateTime? createOn = DateTime.UtcNow;
                var businessModelId = 0;
                var requestStatusId = 0;
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "Id":
                            requestData.Id = property.IntValue;
                            break;
                        case "Brand":
                            brand = property.StringValue;
                            break;
                        case "CategoryId":
                            requestData.CategoryId = property.IntValue;
                            break;
                        case "ProductId":
                            requestData.ProductId = property.IntValue;
                            break;
                        case "IndustryId":
                            requestData.IndustryId = property.IntValue;
                            break;
                        case "BrandId":
                            requestData.BrandId = property.IntValue;
                            break;
                        //case "TypeID":
                        //    requestData.TypeID = property.IntValue;
                        //    break;
                        case "CustomRequestNumber":
                            requestData.CustomRequestNumber = property.StringValue;
                            break;
                        case "BuyerId":
                            requestData.BuyerId = property.IntValue;
                            break;
                        case "ProductAttributeXml":
                            requestData.ProductAttributeXml = property.StringValue;
                            break;
                        case "Quantity":
                            requestData.Quantity = property.DecimalValue;
                            break;
                        case "DeliveryAddress":
                            requestData.DeliveryAddress = property.StringValue;
                            break;
                        case "DeliveryAddress2":
                            requestData.DeliveryAddress2 = property.StringValue;
                            break;
                        case "DeliveryDate":
                            requestData.DeliveryDate = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "RequestStatusId":
                            requestStatusId = property.IntValue;
                            break;
                        case "BookerId":
                            requestData.BookerId = property.IntValue;
                            break;
                        case "PaymentDuration":
                            requestData.PaymentDuration = property.IntValue;
                            break;
                        case "RejectedReason":
                            requestData.RejectedReason = property.StringValue;
                            break;
                        case "CountryId":
                            requestData.CountryId = property.IntValue;
                            break;
                        case "CityId":
                            requestData.CityId = property.IntValue;
                            break;
                        case "AreaId":
                            requestData.AreaId = property.IntValue;
                            break;
                        case "ExpiryDate":
                            requestData.ExpiryDate = property.DateTimeNullable;
                            break;
                        case "PinLocation_Latitude":
                            requestData.PinLocation_Latitude = property.StringValue;
                            break;
                        case "PinLocation_Longitude":
                            requestData.PinLocation_Longitude = property.StringValue;
                            break;
                        case "PinLocation_Location":
                            requestData.PinLocation_Location = property.StringValue;
                            break;
                        case "BusinessModelId":
                            businessModelId = property.IntValue;
                            break;
                        case "InterGeography":
                            requestData.InterGeography = property.IntValue == 1 ? true : false;
                            break;
                        case "CreatedById":
                            requestData.CreatedById = property.IntValue;
                            break;
                        case "UpdatedById":
                            requestData.UpdatedById = property.IntValue;
                            break;
                        case "Deleted":
                            requestData.Deleted = property.IntValue == 1 ? true : false;
                            break;
                        case "DeletedById":
                            requestData.DeletedById = property.IntValue;
                            break;
                        case "CreatedOnUtc":
                            requestData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "UpdatedOnUtc":
                            requestData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "Source":
                            requestData.Source = property.StringValue;
                            break;
                    }
                }

                requestData.RequestTypeId = (int)RequestTypeEnum.External;

                //------------------------------------------------------------------------------------------------------------------------------//
                // Business ModelId Conditions
                if (businessModelId == 0)
                    requestData.BusinessModelId = 10;

                if (businessModelId == 10)
                    requestData.BusinessModelId = 20;

                if (businessModelId == 20)
                    requestData.BusinessModelId = 30;

                if (businessModelId == 30)
                    requestData.BusinessModelId = 40;

                if (businessModelId == 40)
                    requestData.BusinessModelId = 50;

                if (businessModelId == 50)
                    requestData.BusinessModelId = 60;

                if (businessModelId == 60)
                    requestData.BusinessModelId = 70;

                if (businessModelId == 70)
                    requestData.BusinessModelId = 80;

                //------------------------------------------------------------------------------------------------------------------------------//
                // Request StatusId Conditions
                if (requestStatusId == 30)
                    requestData.RequestStatusId = 20;

                if (requestStatusId == 40 || requestStatusId == 100)
                    requestData.RequestStatusId = 30;

                if (requestStatusId == 50)
                    requestData.RequestStatusId = 40;

                if (requestStatusId == 60)
                    requestData.RequestStatusId = 50;

                if (requestStatusId == 70)
                    requestData.RequestStatusId = 60;

                if (requestStatusId == 80)
                    requestData.RequestStatusId = 70;

                if (requestStatusId == 90)
                    requestData.RequestStatusId = 80;

                //------------------------------------------------------------------------------------------------------------------------------//
                if (requestData.IndustryId == 0)
                {
                    requestData.IndustryId = 3;
                }
                if (requestData.BrandId == 0 && string.IsNullOrWhiteSpace(brand))
                {
                    var manufacturer = await _manufacturerService.GetManufacturerByNameAsync("No brand found");
                    if (manufacturer != null)
                    {
                        requestData.BrandId = manufacturer.Id;
                    }
                    else
                    {
                        var newManufacturer = new Manufacturer();
                        newManufacturer.Name = "No brand found";
                        newManufacturer.IndustryId = requestData.IndustryId;
                        await _manufacturerService.InsertManufacturerAsync(newManufacturer);
                        requestData.BrandId = newManufacturer.Id;
                    }
                }
                if (requestData.BrandId == 0 && !string.IsNullOrWhiteSpace(brand))
                {
                    var manufacturer = await _manufacturerService.GetManufacturerByNameAsync(brand);
                    if (manufacturer != null)
                    {
                        requestData.BrandId = manufacturer.Id;
                    }
                    else
                    {
                        var newManufacturer = new Manufacturer();
                        newManufacturer.Name = brand;
                        newManufacturer.IndustryId = requestData.IndustryId;
                        await _manufacturerService.InsertManufacturerAsync(newManufacturer);
                        requestData.BrandId = newManufacturer.Id;
                    }
                }

                if (string.IsNullOrWhiteSpace(requestData.CustomRequestNumber))
                {
                    requestData.CustomRequestNumber = _customNumberFormatter.GenerateRequestCustomNumber(requestData);
                }
                id = requestData.Id;

                await _requestService.InsertRequestAsync(requestData);

                var requestForQuotation = new RequestForQuotation()
                {
                    RequestId = id,
                    Quantity = requestData.Quantity,
                    RfqStatusId = (int)RequestForQuotationStatus.Pending,
                    CreatedOnUtc = DateTime.UtcNow,
                    Source = "Admin"
                };
                await _requestService.InsertRequestForQuotationAsync(requestForQuotation);
                requestForQuotation.CustomRfqNumber = _customNumberFormatter.GenerateRequestForQuotationCustomNumber(requestForQuotation);
                await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

                //await _requestService.InsertRequestAsync(requestData);
                //requestList.Add(requestData);
                iRow++;
            }
        }

        public virtual async Task ImportQuotationFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Quotation>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Quotation, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;
            var brand = "";
            var buyerRequestId = 0;
            //DateTime? priceValidity = DateTime.UtcNow;
            //DateTime? updatedOn = DateTime.UtcNow;
            //DateTime? createOn = DateTime.UtcNow;
            var businessModelId = 0;
            var quotationStatusId = 0;
            var quotationList = new List<Quotation>();

            StringBuilder logMessage = new StringBuilder();
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var quotationData = new Quotation();
                var id = 0;
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "Id":
                            quotationData.Id = property.IntValue;
                            break;
                        case "SupplierId":
                            quotationData.SupplierId = property.IntValue;
                            break;
                        case "BuyerRequestId":
                            buyerRequestId = property.IntValue;
                            break;
                        case "CustomQuotationNumber":
                            quotationData.CustomQuotationNumber = property.StringValue;
                            break;
                        case "QuotationStatusId":
                            quotationStatusId = property.IntValue;
                            break;
                        case "BrandId":
                            quotationData.BrandId = property.IntValue;
                            break;
                        case "QuotationPrice":
                            quotationData.QuotationPrice = property.DecimalValue;
                            break;
                        case "Quantity":
                            quotationData.Quantity = property.DecimalValue;
                            break;
                        case "PriceValidity":
                            quotationData.PriceValidity = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "BookerId":
                            quotationData.BookerId = property.IntValue;
                            break;
                        case "RejectedReason":
                            quotationData.RejectedReason = property.StringValue;
                            break;
                        case "CreatedById":
                            quotationData.CreatedById = property.IntValue;
                            break;
                        case "UpdatedById":
                            quotationData.UpdatedById = property.IntValue;
                            break;
                        case "Deleted":
                            quotationData.Deleted = property.IntValue == 1 ? true : false;
                            break;
                        case "DeletedById":
                            quotationData.DeletedById = property.IntValue;
                            break;
                        case "CreatedOnUtc":
                            quotationData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "UpdatedOnUtc":
                            quotationData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "IsApproved":
                            quotationData.IsApproved = property.IntValue == 1 ? true : false;
                            break;
                        case "BusinessModelId":
                            businessModelId = property.IntValue;
                            break;
                        case "Source":
                            quotationData.Source = property.StringValue;
                            break;
                        case "Brand":
                            brand = property.StringValue;
                            break;
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------//
                // Business ModelId Conditions

                if (businessModelId == 0)
                    quotationData.BusinessModelId = 10;

                if (businessModelId == 10)
                    quotationData.BusinessModelId = 20;

                if (businessModelId == 20)
                    quotationData.BusinessModelId = 30;

                if (businessModelId == 30)
                    quotationData.BusinessModelId = 40;

                if (businessModelId == 40)
                    quotationData.BusinessModelId = 50;

                if (businessModelId == 50)
                    quotationData.BusinessModelId = 60;

                if (businessModelId == 60)
                    quotationData.BusinessModelId = 70;

                if (businessModelId == 70)
                    quotationData.BusinessModelId = 80;

                //------------------------------------------------------------------------------------------------------------------------------//
                // Quotation Status Id Conditions

                if (quotationStatusId == 30)
                    quotationData.QuotationStatusId = 20;

                if (quotationStatusId == 40 || quotationStatusId == 20 || quotationStatusId == 120)
                {
                    quotationData.QuotationStatusId = 30;
                    quotationData.IsApproved = true;
                }

                if (quotationStatusId == 30)
                    quotationData.QuotationStatusId = 20;

                if (quotationStatusId == 80)
                    quotationData.QuotationStatusId = 50;

                if (quotationStatusId == 60 || quotationStatusId == 70)
                    quotationData.QuotationStatusId = 40;

                if (quotationStatusId == 90)
                    quotationData.QuotationStatusId = 60;

                if (quotationStatusId == 50)
                    quotationData.QuotationStatusId = 70;

                if (quotationStatusId == 100)
                    quotationData.QuotationStatusId = 80;

                if (quotationStatusId == 110)
                    quotationData.QuotationStatusId = 90;

                //------------------------------------------------------------------------------------------------------------------------------//

                var request = await _requestService.GetRequestByIdAsync(buyerRequestId);
                if (request != null)
                {
                    if (quotationData.BrandId == 0 && string.IsNullOrWhiteSpace(brand))
                    {
                        var manufacturer = await _manufacturerService.GetManufacturerByNameAsync("No brand found");
                        if (manufacturer != null)
                        {
                            quotationData.BrandId = manufacturer.Id;
                        }
                        else
                        {
                            var newManufacturer = new Manufacturer();
                            newManufacturer.Name = "No brand found";
                            newManufacturer.IndustryId = request.IndustryId;
                            await _manufacturerService.InsertManufacturerAsync(newManufacturer);
                            quotationData.BrandId = newManufacturer.Id;
                        }
                    }
                    if (quotationData.BrandId == 0 && !string.IsNullOrWhiteSpace(brand))
                    {
                        var manufacturer = await _manufacturerService.GetManufacturerByNameAsync(brand);
                        if (manufacturer != null)
                        {
                            quotationData.BrandId = manufacturer.Id;
                        }
                        else
                        {
                            var newManufacturer = new Manufacturer();
                            newManufacturer.Name = brand;
                            newManufacturer.IndustryId = request.IndustryId;
                            await _manufacturerService.InsertManufacturerAsync(newManufacturer);
                            quotationData.BrandId = newManufacturer.Id;
                        }
                    }


                    quotationData.CustomQuotationNumber = _customNumberFormatter.GenerateQuotationCustomNumber(quotationData);

                    quotationData.RfqId = (await _requestService.GetRequestForQuotationByRequestIdAsync(buyerRequestId))?.Id ?? 0;
                    await _quotationService.InsertQuotationAsync(quotationData);
                }
                else
                    logMessage.Append("Buyer Request Ids not found for Quotation: " + buyerRequestId + Environment.NewLine);

                iRow++;
            }
            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Quotation ", logMessage.ToString());
        }

        public virtual async Task ImportOrderFromXlsxAsync(Stream stream)
        {
            try
            {
                using var workbook = new XLWorkbook(stream);
                if (workbook == null)
                    throw new ZarayeException("No worksheet found");

                var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //the columns
                var metadata = GetWorkbookMetadata<Order>(workbook, languages);
                var defaultWorksheet = metadata.DefaultWorksheet;
                var defaultProperties = metadata.DefaultProperties;
                var localizedProperties = metadata.LocalizedProperties;

                var manager = new PropertyManager<Order, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

                var iRow = 2;
                var paymentStatusId = 0;
                //DateTime? updatedOn = DateTime.UtcNow;
                //DateTime? createOn = DateTime.UtcNow;
                var orderList = new List<Order>();
                StringBuilder logMessage = new StringBuilder();

                var orderDataList = new List<Order>();
                while (true)
                {
                    var allColumnsAreEmpty = manager.GetDefaultProperties
                      .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                      .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                    var orderData = new Order();
                    foreach (var property in manager.GetDefaultProperties)
                    {
                        switch (property.PropertyName.Trim())
                        {
                            case "Id":
                                orderData.Id = property.IntValue;
                                break;
                            case "BillingAddressId":
                                orderData.BillingAddressId = property.IntValue;
                                break;
                            case "CustomerId":
                                orderData.CustomerId = property.IntValue;
                                break;
                            case "PickupAddressId":
                                orderData.PickupAddressId = property.IntValueNullable;
                                break;
                            case "ShippingAddressId":
                                orderData.ShippingAddressId = property.IntValueNullable;
                                break;
                            case "OrderGuid":
                                orderData.OrderGuid = Guid.NewGuid();
                                break;
                            case "StoreId":
                                orderData.StoreId = property.IntValue;
                                break;
                            case "PickupInStore":
                                orderData.PickupInStore = property.IntValue == 1 ? true : false;
                                break;
                            case "OrderStatusId":
                                orderData.OrderStatusId = property.IntValue;
                                break;
                            case "ShippingStatusId":
                                orderData.ShippingStatusId = property.IntValue;
                                break;
                            case "PaymentStatusId":
                                paymentStatusId = property.IntValue;
                                break;
                            case "PaymentMethodSystemName":
                                orderData.PaymentMethodSystemName = property.StringValue;
                                break;
                            case "CustomerCurrencyCode":
                                orderData.CustomerCurrencyCode = property.StringValue;
                                break;
                            case "CurrencyRate":
                                orderData.CurrencyRate = property.DecimalValue;
                                break;
                            case "CustomerTaxDisplayTypeId":
                                orderData.CustomerTaxDisplayTypeId = property.IntValue;
                                break;
                            case "VatNumber":
                                orderData.VatNumber = property.StringValue;
                                break;
                            case "OrderSubtotalInclTax":
                                orderData.OrderSubtotalInclTax = property.DecimalValue;
                                break;
                            case "OrderSubtotalExclTax":
                                orderData.OrderSubtotalExclTax = property.DecimalValue;
                                break;
                            case "OrderSubTotalDiscountInclTax":
                                orderData.OrderSubTotalDiscountInclTax = property.DecimalValue;
                                break;
                            case "OrderSubTotalDiscountExclTax":
                                orderData.OrderSubTotalDiscountExclTax = property.DecimalValue;
                                break;
                            case "OrderShippingInclTax":
                                orderData.OrderShippingInclTax = property.DecimalValue;
                                break;
                            case "OrderShippingExclTax":
                                orderData.OrderShippingExclTax = property.DecimalValue;
                                break;
                            case "PaymentMethodAdditionalFeeInclTax":
                                orderData.PaymentMethodAdditionalFeeInclTax = property.DecimalValue;
                                break;
                            case "PaymentMethodAdditionalFeeExclTax":
                                orderData.PaymentMethodAdditionalFeeExclTax = property.DecimalValue;
                                break;
                            case "TaxRates":
                                orderData.TaxRates = property.StringValue;
                                break;
                            case "OrderTax":
                                orderData.OrderTax = property.DecimalValue;
                                break;
                            case "OrderDiscount":
                                orderData.OrderDiscount = property.DecimalValue;
                                break;
                            case "OrderTotal":
                                orderData.OrderTotal = property.DecimalValue;
                                break;
                            case "RefundedAmount":
                                orderData.RefundedAmount = property.DecimalValue;
                                break;
                            case "RewardPointsHistoryEntryId":
                                orderData.RewardPointsHistoryEntryId = property.IntValueNullable;
                                break;
                            case "CheckoutAttributeDescription":
                                orderData.CheckoutAttributeDescription = property.StringValue;
                                break;
                            case "CheckoutAttributesXml":
                                orderData.CheckoutAttributesXml = property.StringValue;
                                break;
                            case "CustomerLanguageId":
                                orderData.CustomerLanguageId = property.IntValue;
                                break;
                            case "AffiliateId":
                                orderData.AffiliateId = property.IntValue;
                                break;
                            case "CustomerIp":
                                orderData.CustomerIp = property.StringValue;
                                break;
                            case "AllowStoringCreditCardNumber":
                                orderData.AllowStoringCreditCardNumber = property.IntValue == 1 ? true : false;
                                break;
                            case "CardType":
                                orderData.CardType = property.StringValue;
                                break;
                            case "CardName":
                                orderData.CardName = property.StringValue;
                                break;
                            case "CardNumber":
                                orderData.CardNumber = property.StringValue;
                                break;
                            case "MaskedCreditCardNumber":
                                orderData.MaskedCreditCardNumber = property.StringValue;
                                break;
                            case "CardCvv2":
                                orderData.CardCvv2 = property.StringValue;
                                break;
                            case "CardExpirationMonth":
                                orderData.CardExpirationMonth = property.StringValue;
                                break;
                            case "CardExpirationYear":
                                orderData.CardExpirationYear = property.StringValue;
                                break;
                            case "AuthorizationTransactionId":
                                orderData.AuthorizationTransactionId = property.StringValue;
                                break;
                            case "AuthorizationTransactionCode":
                                orderData.AuthorizationTransactionCode = property.StringValue;
                                break;
                            case "AuthorizationTransactionResult":
                                orderData.AuthorizationTransactionResult = property.StringValue;
                                break;
                            case "CaptureTransactionId":
                                orderData.CaptureTransactionId = property.StringValue;
                                break;
                            case "CaptureTransactionResult":
                                orderData.CaptureTransactionResult = property.StringValue;
                                break;
                            case "SubscriptionTransactionId":
                                orderData.SubscriptionTransactionId = property.StringValue;
                                break;
                            case "PaidDateUtc":
                                orderData.PaidDateUtc = property.DateTimeNullable;
                                break;
                            case "ShippingMethod":
                                orderData.ShippingMethod = property.StringValue;
                                break;
                            case "ShippingRateComputationMethodSystemName":
                                orderData.ShippingRateComputationMethodSystemName = property.StringValue;
                                break;
                            case "CustomValuesXml":
                                orderData.CustomValuesXml = property.StringValue;
                                break;
                            case "Deleted":
                                orderData.Deleted = property.IntValue == 1 ? true : false;
                                break;
                            case "CreatedOnUtc":
                                orderData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "RedeemedRewardPointsEntryId":
                                orderData.RedeemedRewardPointsEntryId = property.IntValue;
                                break;
                            case "UpdatedOnUtc":
                                orderData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "CreatedById":
                                orderData.CreatedById = property.IntValue;
                                break;
                            case "UpdatedById":
                                orderData.UpdatedById = property.IntValue;
                                break;
                            case "DeletedById":
                                orderData.DeletedById = property.IntValue;
                                break;
                            case "RequestId":
                                orderData.RequestId = property.IntValue;
                                break;
                            case "RFQId":
                                orderData.RFQId = property.IntValue;
                                break;
                            case "OrderTypeId":
                                orderData.OrderTypeId = property.IntValue;
                                break;
                            case "QuotationId":
                                orderData.QuotationId = property.IntValue;
                                break;
                            case "ProcessingDateUtc":
                                orderData.ProcessingDateUtc = property.DateTimeNullable;
                                break;
                            case "RejectedReason":
                                orderData.RejectedReason = property.StringValue;
                                break;
                            case "ParentOrderId":
                                orderData.QuotationId = property.IntValue;
                                break;
                            case "Source":
                                orderData.Source = property.StringValue;
                                break;
                        }
                    }

                    orderData.PaymentStatusId = paymentStatusId;
                    if (paymentStatusId == 60)
                        orderData.PaymentStatusId = 20;

                    orderDataList.Add(orderData);
                    iRow++;
                }

                foreach (var orderData in orderDataList)
                {
                    try
                    {
                        var request = await _requestService.GetRequestByIdAsync(orderData.RequestId);
                        if (request != null)
                        {
                            if (string.IsNullOrWhiteSpace(orderData.CustomOrderNumber))
                                orderData.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(orderData);

                            await _orderService.InsertOrderAsync(orderData);
                        }
                        else
                            logMessage.Append("Request Ids not found for Order Info: " + orderData.RequestId + Environment.NewLine);



                    }
                    catch (Exception ex)
                    {

                        await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Order Info " + orderData.Id.ToString(), ex.Message);
                    }
                }

                if (logMessage.Length > 0)
                    await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Order Info ", logMessage.ToString());

            }
            catch (Exception ex)
            {

            }
        }

        public virtual async Task ImportShipmentFromXlsxAsync(Stream stream)
        {
            try
            {
                using var workbook = new XLWorkbook(stream);
                if (workbook == null)
                    throw new ZarayeException("No worksheet found");

                var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //the columns
                var metadata = GetWorkbookMetadata<Shipment>(workbook, languages);
                var defaultWorksheet = metadata.DefaultWorksheet;
                var defaultProperties = metadata.DefaultProperties;
                var localizedProperties = metadata.LocalizedProperties;

                var manager = new PropertyManager<Shipment, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

                var iRow = 2;
                StringBuilder logMessage = new StringBuilder();
                var shipmentList = new List<Shipment>();

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetDefaultProperties
                      .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                      .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                    var shipmentData = new Shipment();
                    var id = 0;
                    foreach (var property in manager.GetDefaultProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Id":
                                shipmentData.Id = property.IntValue;
                                break;
                            case "OrderId":
                                shipmentData.OrderId = property.IntValue;
                                break;
                            case "TrackingNumber":
                                shipmentData.TrackingNumber = property.StringValue;
                                break;
                            case "TotalWeight":
                                shipmentData.TotalWeight = property.DecimalValue;
                                break;
                            case "ShippedDateUtc":
                                shipmentData.ShippedDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "DeliveryDateUtc":
                                shipmentData.DeliveryDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ReadyForPickupDateUtc":
                                shipmentData.ReadyForPickupDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow.ToString() : property.PropertyValue);
                                break;
                            case "AdminComment":
                                shipmentData.AdminComment = property.StringValue;
                                break;
                            case "CreatedOnUtc":
                                shipmentData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "UpdatedOnUtc":
                                shipmentData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "CreatedById":
                                shipmentData.CreatedById = property.IntValue;
                                break;
                            case "UpdatedById":
                                shipmentData.UpdatedById = property.IntValue;
                                break;
                            case "Deleted":
                                shipmentData.Deleted = property.IntValue == 1 ? true : false;
                                break;
                            case "DeletedById":
                                shipmentData.DeletedById = property.IntValue;
                                break;
                            case "TransporterId":
                                shipmentData.TransporterId = property.IntValue;
                                break;
                            case "VehicleId":
                                shipmentData.VehicleId = property.IntValue;
                                break;
                            case "DeliveryStatusId":
                                shipmentData.DeliveryStatusId = property.IntValue;
                                break;
                            case "PickupAddress":
                                shipmentData.PickupAddress = property.StringValue;
                                break;
                            case "RouteTypeId":
                                shipmentData.RouteTypeId = property.IntValue;
                                break;
                            case "ShipmentDeliveryAddress":
                                shipmentData.ShipmentDeliveryAddress = property.StringValue;
                                break;
                            case "PictureId":
                                shipmentData.PictureId = property.IntValue;
                                break;
                            case "TransporterTypeId":
                                shipmentData.TransporterTypeId = property.IntValue;
                                break;
                            case "DeliveryTypeId":
                                shipmentData.DeliveryTypeId = property.IntValue;
                                break;
                            case "DeliveryCostReasonId":
                                shipmentData.DeliveryCostReasonId = property.IntValue;
                                break;
                            case "DeliveryTimingId":
                                shipmentData.DeliveryTimingId = property.IntValue;
                                break;
                            case "DeliveryCostTypeId":
                                shipmentData.DeliveryCostTypeId = property.IntValue;
                                break;
                            case "DeliveryDelayedReasonId":
                                shipmentData.DeliveryDelayedReasonId = property.IntValue;
                                break;
                            case "WarehouseId":
                                shipmentData.WarehouseId = property.IntValue;
                                break;
                            case "DeliveryCost":
                                shipmentData.DeliveryCost = property.IntValue;
                                break;
                            case "FreightCharges":
                                shipmentData.FreightCharges = property.IntValue;
                                break;
                            case "LabourCharges":
                                shipmentData.LabourCharges = property.IntValue;
                                break;
                            case "ExpectedDateShipped":
                                shipmentData.ExpectedDateShipped = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ExpectedDateDelivered":
                                shipmentData.ExpectedDateDelivered = Convert.ToDateTime(string.IsNullOrEmpty(property.PropertyValue.ToString()) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ExpectedDeliveryCost":
                                shipmentData.ExpectedDeliveryCost = property.DecimalValue;
                                break;
                            case "ExpectedQuantity":
                                shipmentData.ExpectedQuantity = property.DecimalValue;
                                break;
                            case "DeliveredOrderAmount":
                                shipmentData.DeliveredAmount = property.DecimalValue;
                                break;
                            case "ShipmentTypeId":
                                shipmentData.ShipmentTypeId = property.IntValue;
                                break;
                            case "PaymentStatusid":
                                shipmentData.PaymentStatusId = property.IntValue;
                                break;
                            case "CustomShipmentNumber":
                                shipmentData.CustomShipmentNumber = property.StringValue;
                                break;
                            case "VehicleNumber":
                                shipmentData.VehicleNumber = property.StringValue;
                                break;
                            case "IsDirectOrder":
                                shipmentData.IsDirectOrder = property.IntValue == 1 ? true : false;
                                break;
                            case "BuyerId":
                                shipmentData.BuyerId = property.IntValue;
                                break;
                            case "DeliveryRequestId":
                                shipmentData.DeliveryRequestId = property.IntValue;
                                break;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(shipmentData.CustomShipmentNumber))
                    {
                        if (shipmentData.ShipmentTypeId == (int)ShipmentType.PurchaseOrder)
                        {
                            shipmentData.CustomShipmentNumber = _customNumberFormatter.GeneratePOShipmentCustomNumber(shipmentData);
                        }
                        if (shipmentData.ShipmentTypeId == (int)ShipmentType.SaleOrder)
                        {
                            shipmentData.CustomShipmentNumber = _customNumberFormatter.GenerateSOShipmentCustomNumber(shipmentData);
                        }

                    }
                    shipmentList.Add(shipmentData);
                    iRow++;
                }
                foreach (var shipmentData in shipmentList)
                {
                    try
                    {
                        var order = await _orderService.GetOrderByIdAsync(shipmentData.OrderId);
                        if (order != null)
                        {

                            await _shipmentService.InsertShipmentAsync(shipmentData);
                            var saleOrder = await _orderService.GetOrderByIdAsync(shipmentData.OrderId);
                            if (saleOrder != null)
                            {
                                //Add Customer Ledger for buyer delivery
                                await _customerLedgerService.AddCustomerLedgerAsync(date: shipmentData.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Delivery", debit: shipmentData.DeliveredAmount, credit: 0, shipmentId: shipmentData.Id);

                                //Add customer ledger for delivery cost on buyer
                                if (shipmentData.DeliveryCostType == DeliveryCostType.DeliveryCostOnBuyer && shipmentData.DeliveryCost > 0)
                                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipmentData.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Delivery cost on buyer", debit: shipmentData.DeliveryCost, credit: 0, shipmentId: shipmentData.Id, updateRecord: true);
                            }
                        }
                        else
                            logMessage.Append("Order Ids not found for Shipment: " + shipmentData.OrderId + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {

                        await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Shipment Info " + shipmentData.Id.ToString(), ex.Message);
                    }
                }

                if (logMessage.Length > 0)
                    await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Shipment ", logMessage.ToString());
            }
            catch (Exception ex)
            {

            }
        }

        public virtual async Task ImportShipmentItemFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<ShipmentItem>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<ShipmentItem, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;
            var brand = "";
            var buyerRequestId = 0;
            //DateTime? priceValidity = DateTime.UtcNow;
            //DateTime? updatedOn = DateTime.UtcNow;
            //DateTime? createOn = DateTime.UtcNow;
            var shipmentItemList = new List<ShipmentItem>();
            StringBuilder logMessage = new StringBuilder();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var shipmentItemData = new ShipmentItem();
                var id = 0;
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "id":
                            shipmentItemData.Id = property.IntValue;
                            break;
                        case "shipmentid":
                            shipmentItemData.ShipmentId = property.IntValue;
                            break;
                        case "orderitemid":
                            shipmentItemData.OrderItemId = property.IntValue;
                            break;
                        case "Quantity":
                            shipmentItemData.Quantity = property.DecimalValue;
                            break;
                        case "warehouseid":
                            shipmentItemData.WarehouseId = property.IntValue;
                            break;
                    }
                }
                var _shipment = await _shipmentService.GetShipmentByIdAsync(shipmentItemData.ShipmentId);
                var _orderItem = await _orderService.GetOrderItemByIdAsync(shipmentItemData.OrderItemId);

                if (_shipment != null && _orderItem != null)
                {
                    await _shipmentService.InsertShipmentItemAsync(shipmentItemData);
                }
                else
                {
                    logMessage.Append("Shipment Item: Sipment Item not found against the 'Order Item' Id " + shipmentItemData.OrderItemId + " && 'Shipment Id' " + shipmentItemData.ShipmentId + Environment.NewLine);
                }

                iRow++;
            }
            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For shipment Item ", logMessage.ToString());
        }

        public virtual async Task ImportShipmentDropoffHistoryFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<ShipmentDropOffHistory>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<ShipmentDropOffHistory, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;
            var brand = "";
            var buyerRequestId = 0;
            //DateTime? priceValidity = DateTime.UtcNow;
            //DateTime? updatedOn = DateTime.UtcNow;
            //DateTime? createOn = DateTime.UtcNow;
            StringBuilder logMessage = new StringBuilder();
            var shipmentDropOffHistoryList = new List<ShipmentDropOffHistory>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var shipmentDropOffHistoryData = new ShipmentDropOffHistory();
                var id = 0;
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "id":
                            shipmentDropOffHistoryData.Id = property.IntValue;
                            break;
                        case "shipmentid":
                            shipmentDropOffHistoryData.ShipmentId = property.IntValue;
                            break;
                        case "TransporterId":
                            shipmentDropOffHistoryData.TransporterId = property.IntValue;
                            break;
                        case "VehicleId":
                            shipmentDropOffHistoryData.VehicleId = property.IntValue;
                            break;
                        case "DropoffLocation":
                            shipmentDropOffHistoryData.DropoffLocation = property.StringValue;
                            break;
                        case "DeliveryCost":
                            shipmentDropOffHistoryData.DeliveryCost = property.DecimalValue;
                            break;
                        case "TransporterTypeId":
                            shipmentDropOffHistoryData.TransporterTypeId = property.IntValue;
                            break;
                        case "RouteTypeId":
                            shipmentDropOffHistoryData.RouteTypeId = property.IntValue;
                            break;
                        case "CreatedOnUtc":
                            shipmentDropOffHistoryData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "CreatedById":
                            shipmentDropOffHistoryData.CreatedById = property.IntValue;
                            break;
                        case "UpdatedOnUtc":
                            shipmentDropOffHistoryData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "UpdatedById":
                            shipmentDropOffHistoryData.UpdatedById = property.IntValue;
                            break;
                        case "Deleted":
                            shipmentDropOffHistoryData.Deleted = property.IntValue == 1 ? true : false;
                            break;

                    }
                }

                var _shipment = await _shipmentService.GetShipmentByIdAsync(shipmentDropOffHistoryData.ShipmentId);

                if (_shipment != null)
                {
                    await _shipmentService.InsertShipmentDropOffHistoryAsync(shipmentDropOffHistoryData);
                }
                else
                {
                    logMessage.Append("Shipment Drop Off History : Shipment Drop Off History not found against the 'Shipment Id' " + shipmentDropOffHistoryData.ShipmentId + Environment.NewLine);
                }

                iRow++;
            }
            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Shipment Drop Off History ", logMessage.ToString());
        }

        public virtual async Task ImportOrderItemFromXlsxAsync(Stream stream)
        {
            try
            {
                using var workbook = new XLWorkbook(stream);
                if (workbook == null)
                    throw new ZarayeException("No worksheet found");

                var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //the columns
                var metadata = GetWorkbookMetadata<OrderItem>(workbook, languages);
                var defaultWorksheet = metadata.DefaultWorksheet;
                var defaultProperties = metadata.DefaultProperties;
                var localizedProperties = metadata.LocalizedProperties;

                var manager = new PropertyManager<OrderItem, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

                var iRow = 2;
                DateTime? rentalStartDateUtc = DateTime.UtcNow;
                DateTime? rentalEndDateUtc = DateTime.UtcNow;
                var orderitemList = new List<OrderItem>();
                StringBuilder logMessage = new StringBuilder();

                var orderitemDataList = new List<OrderItem>();
                while (true)
                {
                    var allColumnsAreEmpty = manager.GetDefaultProperties
                      .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                      .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                    var orderitemData = new OrderItem();
                    foreach (var property in manager.GetDefaultProperties)
                    {
                        switch (property.PropertyName.Trim())
                        {
                            case "Id":
                                orderitemData.Id = property.IntValue;
                                break;
                            case "OrderId":
                                orderitemData.OrderId = property.IntValue;
                                break;
                            case "ProductId":
                                orderitemData.ProductId = property.IntValue;
                                break;
                            case "OrderItemGuid":
                                orderitemData.OrderItemGuid = Guid.NewGuid();
                                break;
                            case "Quantity":
                                orderitemData.Quantity = property.DecimalValue;
                                break;
                            case "UnitPriceInclTax":
                                orderitemData.UnitPriceInclTax = property.DecimalValue;
                                break;
                            case "UnitPriceExclTax":
                                orderitemData.UnitPriceExclTax = property.DecimalValue;
                                break;
                            case "PriceInclTax":
                                orderitemData.PriceInclTax = property.DecimalValue;
                                break;
                            case "PriceExclTax":
                                orderitemData.PriceExclTax = property.DecimalValue;
                                break;
                            case "DiscountAmountInclTax":
                                orderitemData.DiscountAmountInclTax = property.DecimalValue;
                                break;
                            case "DiscountAmountExclTax":
                                orderitemData.DiscountAmountExclTax = property.DecimalValue;
                                break;
                            case "OriginalProductCost":
                                orderitemData.OriginalProductCost = property.DecimalValue;
                                break;
                            case "AttributeDescription":
                                orderitemData.AttributeDescription = property.StringValue;
                                break;
                            case "AttributesXml":
                                orderitemData.AttributesXml = property.StringValue;
                                break;
                            case "DownloadCount":
                                orderitemData.DownloadCount = property.IntValue;
                                break;
                            case "IsDownloadActivated":
                                orderitemData.IsDownloadActivated = property.IntValue == 1 ? true : false;
                                break;
                            case "LicenseDownloadId":
                                orderitemData.LicenseDownloadId = property.IntValue;
                                break;
                            case "ItemWeight":
                                orderitemData.ItemWeight = property.DecimalValue;
                                break;
                            case "RentalStartDateUtc":
                                rentalStartDateUtc = property.DateTimeNullable;
                                break;
                            case "RentalEndDateUtc":
                                rentalEndDateUtc = property.DateTimeNullable;
                                break;
                            case "BrandId":
                                orderitemData.BrandId = property.IntValue;
                                break;
                        }
                    }

                    orderitemData.RentalStartDateUtc = rentalStartDateUtc.HasValue ? rentalStartDateUtc.Value : DateTime.UtcNow;
                    orderitemData.RentalEndDateUtc = rentalEndDateUtc.HasValue ? rentalEndDateUtc.Value : DateTime.UtcNow;
                    orderitemDataList.Add(orderitemData);
                    iRow++;
                }

                foreach (var orderitemData in orderitemDataList)
                {
                    try
                    {
                        var order = await _orderService.GetOrderByIdAsync(orderitemData.OrderId);
                        if (order != null)
                        {
                            await _orderService.InsertOrderItemAsync(orderitemData);
                        }
                        else
                            logMessage.Append("Order Ids not found for OrderItem Info: " + orderitemData.OrderId + Environment.NewLine);


                    }
                    catch (Exception ex)
                    {

                        await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For OrderItem Info " + orderitemData.Id.ToString(), ex.Message);
                    }
                }

                if (logMessage.Length > 0)
                    await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For OrderItem Info ", logMessage.ToString());

            }
            catch (Exception ex)
            {

            }
        }

        public virtual async Task ImportInboundFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            await _dataProvider.SetTableIdentAsync<InventoryInbound>(1);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<InventoryInbound>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<InventoryInbound, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            var inboundList = new List<InventoryInbound>();
            StringBuilder logMessage = new StringBuilder();
            // rashid.transport@zaraye.co
            var transpoter = await _customerService.GetCustomerByIdAsync(48677);
            var VehicleTransport = (await _customerService.GetTransporterVehicleByTransporterIdAsync(transpoter.Id)).FirstOrDefault();
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var inboundData = new InventoryInbound();
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "Id":
                            inboundData.Id = property.IntValue;
                            break;
                        case "inventorypurchaseorderId":
                            inboundData.PurchaseOrderId = property.IntValue;
                            break;
                        case "SupplierId":
                            inboundData.SupplierId = property.IntValue;
                            break;
                        case "ShipmentId":
                            inboundData.ShipmentId = property.IntValue;
                            break;
                        case "IndustryId":
                            inboundData.IndustryId = property.IntValue;
                            break;
                        case "CategoryId":
                            inboundData.CategoryId = property.IntValue;
                            break;
                        case "ProductId":
                            inboundData.ProductId = property.IntValue;
                            break;
                        case "WarehouseId":
                            inboundData.WarehouseId = property.IntValue;
                            break;
                        case "BrandId":
                            inboundData.BrandId = property.IntValue;
                            break;
                        case "ProductAttributesXml":
                            inboundData.ProductAttributesXml = property.StringValue;
                            break;
                        case "WholesaleTax":
                            inboundData.WholesaleTax = property.IntValue == 1 ? true : false;
                            break;
                        case "StockQuantity":
                            inboundData.StockQuantity = property.DecimalValue;
                            break;
                        case "PurchaseRate":
                            inboundData.PurchaseRate = property.DecimalValue;
                            break;
                        case "TotalPurchaseValue":
                            inboundData.TotalPurchaseValue = property.DecimalValue;
                            break;
                        case "CreatedOnUtc":
                            inboundData.CreatedOnUtc = Convert.ToDateTime(property.PropertyValue);
                            break;
                        case "UpdatedOnUtc":
                            inboundData.UpdatedOnUtc = Convert.ToDateTime(property.PropertyValue);
                            break;
                        case "CreatedById":
                            inboundData.CreatedById = property.IntValue;
                            break;
                        case "updatedbyid":
                            inboundData.UpdatedById = property.IntValue;
                            break;
                        case "Deleted":
                            inboundData.Deleted = property.IntValue == 1 ? true : false;
                            break;
                        case "DeletedById":
                            inboundData.DeletedById = property.IntValue;
                            break;
                        case "inventoryinboundstatusid":
                            inboundData.InventoryInboundStatusId = property.IntValue;
                            break;
                        case "InventoryTypeId":
                            inboundData.InventoryTypeId = property.IntValue;
                            break;
                        case "GstRate":
                            inboundData.GstRate = property.DecimalValue;
                            break;
                        case "GstAmount":
                            inboundData.GstAmount = property.DecimalValue;
                            break;
                        case "WhtRate":
                            inboundData.WhtRate = property.DecimalValue;
                            break;
                        case "WhtAmount":
                            inboundData.WhtAmount = property.DecimalValue;
                            break;
                        case "WholeSaleTaxRate":
                            inboundData.WholeSaleTaxRate = property.DecimalValue;
                            break;
                        case "WholeSaleTaxAmount":
                            inboundData.WholeSaleTaxAmount = property.DecimalValue;
                            break;
                    }
                }
                try
                {
                    if (inboundData.BrandId == 0)
                    {
                        var manufacturer = await _manufacturerService.GetManufacturerByNameAsync("No brand found");
                        if (manufacturer != null)
                        {
                            inboundData.BrandId = manufacturer.Id;
                        }
                    }
                    var isError = true;
                    var warehouse = await _shippingService.GetWarehouseByIdAsync(inboundData.WarehouseId);
                    if (warehouse == null)
                    {
                        var warehousefound = (await _shippingService.GetAllWarehousesAsync("No warehouse found")).FirstOrDefault();
                        if (warehousefound == null)
                        {
                            var warehouseinsert = new Warehouse
                            {
                                Name = "No warehouse found",
                            };
                            await _shippingService.InsertWarehouseAsync(warehouseinsert);
                            inboundData.WarehouseId = warehouseinsert.Id;
                        }
                        else
                        {
                            inboundData.WarehouseId = warehousefound.Id;
                        }
                    }

                    var supplier = await _customerService.GetCustomerByIdAsync(inboundData.SupplierId);
                    if (supplier == null)
                    {
                        logMessage.Append("supplier Id not found for Inbound: " + inboundData.Id + Environment.NewLine);
                        isError = false;
                    }

                    if (isError)
                    {
                        #region Request

                        var purchaseRequest = new Request();
                        purchaseRequest.CategoryId = inboundData.CategoryId;
                        purchaseRequest.ProductId = inboundData.ProductId;
                        purchaseRequest.IndustryId = inboundData.IndustryId;
                        purchaseRequest.BrandId = inboundData.BrandId;
                        purchaseRequest.BuyerId = inboundData.SupplierId;
                        purchaseRequest.ProductAttributeXml = inboundData.ProductAttributesXml;
                        purchaseRequest.Quantity = inboundData.StockQuantity;
                        purchaseRequest.RequestStatusId = (int)RequestStatus.Complete;
                        purchaseRequest.BookerId = 0;
                        purchaseRequest.PaymentDuration = 0;
                        purchaseRequest.CountryId = 0;
                        purchaseRequest.CityId = 0;
                        purchaseRequest.AreaId = 0;
                        purchaseRequest.BusinessModelId = (int)BusinessModelEnum.Standard;
                        purchaseRequest.InterGeography = false;
                        purchaseRequest.Source = "Migration";
                        purchaseRequest.DeliveryDate = inboundData.CreatedOnUtc;
                        purchaseRequest.RequestTypeId = (int)RequestTypeEnum.Internal;
                        purchaseRequest.CreatedOnUtc = inboundData.CreatedOnUtc;

                        await _requestService.InsertRequestAsync(purchaseRequest);
                        purchaseRequest.CustomRequestNumber = _customNumberFormatter.GeneratePurchaseRequestCustomNumber(purchaseRequest);
                        await _requestService.UpdateRequestAsync(purchaseRequest);

                        #endregion

                        #region RFQ

                        var requestForQuotation = new RequestForQuotation()
                        {
                            RequestId = purchaseRequest.Id,
                            Quantity = inboundData.StockQuantity,
                            RfqStatusId = (int)RequestForQuotationStatus.Complete,
                            CreatedOnUtc = inboundData.CreatedOnUtc,
                            Source = "Migration"
                        };
                        await _requestService.InsertRequestForQuotationAsync(requestForQuotation);
                        requestForQuotation.CustomRfqNumber = _customNumberFormatter.GenerateRequestForQuotationCustomNumber(requestForQuotation);
                        await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);
                        #endregion

                        #region Quotation

                        var quotation = new Quotation
                        {
                            RfqId = requestForQuotation.Id,
                            BusinessModelId = (int)BusinessModelEnum.Standard,
                            SupplierId = inboundData.SupplierId,
                            QuotationStatusId = (int)QuotationStatus.Approved,
                            BrandId = inboundData.BrandId,
                            QuotationPrice = inboundData.PurchaseRate,
                            Quantity = inboundData.StockQuantity,
                            PriceValidity = _dateTimeHelper.ConvertToUtcTime(inboundData.CreatedOnUtc),
                            CreatedOnUtc = inboundData.CreatedOnUtc,
                            Source = "Migration",
                            IsApproved = true
                        };

                        await _quotationService.InsertQuotationAsync(quotation);
                        quotation.CustomQuotationNumber = _customNumberFormatter.GenerateQuotationCustomNumber(quotation);
                        await _quotationService.UpdateQuotationAsync(quotation);

                        #endregion

                        #region Order

                        var customer = await _customerService.GetCustomerByIdAsync(inboundData.SupplierId);
                        var order = new Order
                        {
                            StoreId = 1,
                            OrderGuid = Guid.NewGuid(),
                            CustomerId = inboundData.SupplierId,
                            CustomerLanguageId = 1,
                            CustomerIp = "127.0.0.1",
                            OrderSubtotalInclTax = decimal.Zero,
                            OrderSubtotalExclTax = decimal.Zero,
                            OrderSubTotalDiscountInclTax = decimal.Zero,
                            OrderSubTotalDiscountExclTax = decimal.Zero,
                            OrderShippingInclTax = decimal.Zero,
                            OrderShippingExclTax = decimal.Zero,
                            PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                            PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                            TaxRates = "0:0;",
                            OrderTax = decimal.Zero,
                            RefundedAmount = decimal.Zero,
                            OrderDiscount = decimal.Zero,
                            CheckoutAttributeDescription = string.Empty,
                            CheckoutAttributesXml = string.Empty,
                            CustomerCurrencyCode = "PKR",
                            OrderTotal = inboundData.TotalPurchaseValue,
                            CurrencyRate = 1M,
                            AffiliateId = 0,
                            OrderStatusId = (int)OrderStatus.Complete,
                            ShippingStatusId = (int)ShippingStatus.Delivered,
                            PaymentStatusId = (int)PaymentStatus.Paid,
                            AllowStoringCreditCardNumber = false,
                            CardType = string.Empty,
                            CardName = string.Empty,
                            CardNumber = string.Empty,
                            MaskedCreditCardNumber = string.Empty,
                            CardCvv2 = string.Empty,
                            CardExpirationMonth = string.Empty,
                            CardExpirationYear = string.Empty,
                            PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                            AuthorizationTransactionId = string.Empty,
                            AuthorizationTransactionCode = string.Empty,
                            AuthorizationTransactionResult = string.Empty,
                            CaptureTransactionId = string.Empty,
                            CaptureTransactionResult = string.Empty,
                            SubscriptionTransactionId = string.Empty,
                            PaymentStatus = PaymentStatus.Paid,
                            PaidDateUtc = DateTime.UtcNow,
                            BillingAddressId = (int)customer.BillingAddressId,
                            ShippingAddressId = (int)customer.ShippingAddressId,
                            ShippingStatus = ShippingStatus.NotYetShipped,
                            ShippingMethod = "Ground",
                            PickupInStore = false,
                            ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
                            CustomValuesXml = string.Empty,
                            VatNumber = string.Empty,
                            CreatedOnUtc = inboundData.CreatedOnUtc,
                            UpdatedOnUtc = inboundData.UpdatedOnUtc,
                            CreatedById = inboundData.CreatedById,
                            UpdatedById = inboundData.UpdatedById,
                            CustomOrderNumber = string.Empty,
                            Source = "Migration",
                            RFQId = requestForQuotation.Id,
                            RequestId = purchaseRequest.Id,
                            QuotationId = quotation.Id,
                            OrderTypeId = (int)OrderType.PurchaseOrder

                        };

                        await _orderService.InsertOrderAsync(order);
                        order.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(order);
                        await _orderService.UpdateOrderAsync(order);
                        #endregion

                        #region orderitem
                        var orderItem = new OrderItem
                        {
                            OrderItemGuid = Guid.NewGuid(),
                            OrderId = order.Id,
                            ProductId = inboundData.ProductId,
                            BrandId = inboundData.BrandId,
                            UnitPriceInclTax = inboundData.PurchaseRate,
                            UnitPriceExclTax = inboundData.PurchaseRate,
                            PriceInclTax = inboundData.TotalPurchaseValue,
                            PriceExclTax = inboundData.TotalPurchaseValue,
                            OriginalProductCost = inboundData.PurchaseRate,
                            AttributeDescription = string.Empty,
                            AttributesXml = inboundData.ProductAttributesXml,
                            Quantity = inboundData.StockQuantity,
                            DiscountAmountInclTax = decimal.Zero,
                            DiscountAmountExclTax = decimal.Zero,
                            DownloadCount = 0,
                            IsDownloadActivated = false,
                            LicenseDownloadId = 0,
                            ItemWeight = null,
                            RentalStartDateUtc = null,
                            RentalEndDateUtc = null,
                        };

                        await _orderService.InsertOrderItemAsync(orderItem);

                        #endregion

                        #region  Request_Quotation_Approved_Mapping 
                        await _requestService.InsertRequestRfqQuotationMappingAsync(new RequestRfqQuotationMapping
                        {
                            OrderId = order.Id,
                            QuotationId = quotation.Id,
                            RfqId = requestForQuotation.Id,
                            RequestId = purchaseRequest.Id
                        });
                        #endregion

                        #region orderCalculation

                        var orderCalculation = new OrderCalculation
                        {
                            BusinessModelId = (int)BusinessModelEnum.Standard,
                            OrderId = order.Id,
                            WholesaleTaxAmount = inboundData.WholeSaleTaxAmount,
                            WholesaleTaxRate = inboundData.WholeSaleTaxRate,
                            WHTAmount = inboundData.WhtAmount,
                            WHTRate = inboundData.WhtRate,
                            GSTAmount = inboundData.GstAmount,
                            GSTRate = inboundData.GstAmount,
                            NetRateWithMargin = inboundData.TotalPurchaseValue,
                            DiscountRate = 0,
                            DiscountRateType = "",
                            DiscountAmount = 0,
                            MarginRate = 0,
                            MarginRateType = "",
                            MarginAmount = 0,
                            NetAmountWithoutGST = inboundData.TotalPurchaseValue - inboundData.GstAmount,
                            ProductPrice = inboundData.PurchaseRate,
                            SellingPriceOfProduct = inboundData.PurchaseRate,
                            SubTotal = inboundData.TotalPurchaseValue,
                            OrderTotal = inboundData.TotalPurchaseValue
                        };

                        await _orderService.InsertOrderCalculationAsync(orderCalculation);
                        #endregion
                        var shipment = (await _shipmentService.GetAllShipmentsAsync()).FirstOrDefault();
                        inboundData.PurchaseOrderId = order.Id;
                        inboundData.ShipmentId = shipment.Id;
                        await _inventoryService.InsertInventoryInboundAsync(inboundData);
                    }
                }
                catch (Exception ex)
                {
                    await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Inbound " + inboundData.Id, ex.Message);
                }
                iRow++;
            }

            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Inbound ", logMessage.ToString());
        }

        public virtual async Task ImportInboundShipmentFromXlsxAsync(Stream stream)
        {
            try
            {
                using var workbook = new XLWorkbook(stream);
                if (workbook == null)
                    throw new ZarayeException("No worksheet found");

                var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //the columns
                var metadata = GetWorkbookMetadata<Shipment>(workbook, languages);
                var defaultWorksheet = metadata.DefaultWorksheet;
                var defaultProperties = metadata.DefaultProperties;
                var localizedProperties = metadata.LocalizedProperties;

                var manager = new PropertyManager<Shipment, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

                var iRow = 2;
                var inventoryid = 0;
                StringBuilder logMessage = new StringBuilder();
                var shipmentList = new List<Shipment>();

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetDefaultProperties
                      .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                      .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                    var shipmentData = new Shipment();
                    var id = 0;
                    foreach (var property in manager.GetDefaultProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "id":
                                shipmentData.Id = property.IntValue;
                                break;
                            case "inventoryid":
                                inventoryid = property.IntValue;
                                break;
                            case "TrackingNumber":
                                shipmentData.TrackingNumber = property.StringValue;
                                break;
                            case "TotalWeight":
                                shipmentData.TotalWeight = property.DecimalValue;
                                break;
                            case "ShippedDateUtc":
                                shipmentData.ShippedDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "DeliveryDateUtc":
                                shipmentData.DeliveryDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ReadyForPickupDateUtc":
                                shipmentData.ReadyForPickupDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "AdminComment":
                                shipmentData.AdminComment = property.StringValue;
                                break;
                            case "CreatedOnUtc":
                                shipmentData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "UpdatedOnUtc":
                                shipmentData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "CreatedById":
                                shipmentData.CreatedById = property.IntValue;
                                break;
                            case "UpdatedById":
                                shipmentData.UpdatedById = property.IntValue;
                                break;
                            case "Deleted":
                                shipmentData.Deleted = property.IntValue == 1 ? true : false;
                                break;
                            case "DeletedById":
                                shipmentData.DeletedById = property.IntValue;
                                break;
                            case "TransporterId":
                                shipmentData.TransporterId = property.IntValue;
                                break;
                            case "VehicleId":
                                shipmentData.VehicleId = property.IntValue;
                                break;
                            case "DeliveryStatusId":
                                shipmentData.DeliveryStatusId = property.IntValue;
                                break;
                            case "PickupAddress":
                                shipmentData.PickupAddress = property.StringValue;
                                break;
                            case "RouteTypeId":
                                shipmentData.RouteTypeId = property.IntValue;
                                break;
                            case "ShipmentDeliveryAddress":
                                shipmentData.ShipmentDeliveryAddress = property.StringValue;
                                break;
                            case "PictureId":
                                shipmentData.PictureId = property.IntValue;
                                break;
                            case "TransporterTypeId":
                                shipmentData.TransporterTypeId = property.IntValue;
                                break;
                            case "DeliveryTypeId":
                                shipmentData.DeliveryTypeId = property.IntValue;
                                break;
                            case "DeliveryCostReasonId":
                                shipmentData.DeliveryCostReasonId = property.IntValue;
                                break;
                            case "DeliveryTiming":
                                shipmentData.DeliveryTimingId = property.IntValue;
                                break;
                            case "DeliveryCostTypeId":
                                shipmentData.DeliveryCostTypeId = property.IntValue;
                                break;
                            case "DeliveryDelayedReasonId":
                                shipmentData.DeliveryDelayedReasonId = property.IntValue;
                                break;
                            case "WarehouseId":
                                shipmentData.WarehouseId = property.IntValue;
                                break;
                            case "DeliveryCost":
                                shipmentData.DeliveryCost = property.IntValue;
                                break;
                            case "FreightCharges":
                                shipmentData.FreightCharges = property.IntValue;
                                break;
                            case "LabourCharges":
                                shipmentData.LabourCharges = property.IntValue;
                                break;
                            case "ExpectedDateShipped":
                                shipmentData.ExpectedDateShipped = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ExpectedDateDelivered":
                                shipmentData.ExpectedDateDelivered = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ExpectedDeliveryCost":
                                shipmentData.ExpectedDeliveryCost = property.DecimalValue;
                                break;
                            case "ExpectedQuantity":
                                shipmentData.ExpectedQuantity = property.DecimalValue;
                                break;
                            case "DeliveredOrderAmount":
                                shipmentData.DeliveredAmount = property.DecimalValue;
                                break;
                            case "ShipmentTypeId":
                                shipmentData.ShipmentTypeId = property.IntValue;
                                break;
                            case "PaymentStatusId":
                                shipmentData.PaymentStatusId = property.IntValue;
                                break;
                            case "CustomShipmentNumber":
                                shipmentData.CustomShipmentNumber = property.StringValue;
                                break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(shipmentData.CustomShipmentNumber))
                    {
                        if (shipmentData.ShipmentTypeId == (int)ShipmentType.PurchaseOrder)
                        {
                            shipmentData.CustomShipmentNumber = _customNumberFormatter.GeneratePOShipmentCustomNumber(shipmentData);
                        }
                    }

                    try
                    {
                        var inventory = await _inventoryService.GetInventoryInboundByIdAsync(inventoryid);
                        if (inventory != null)
                        {
                            var order = await _orderService.GetOrderByIdAsync(inventory.PurchaseOrderId.Value);
                            if (order != null)
                            {
                                shipmentData.OrderId = order.Id;
                                await _shipmentService.InsertShipmentAsync(shipmentData);
                                inventory.ShipmentId = shipmentData.Id;
                                await _inventoryService.UpdateInventoryInboundAsync(inventory);
                                var purchaseOrder = await _orderService.GetOrderByIdAsync(shipmentData.OrderId);
                                if (purchaseOrder != null)
                                {

                                    //Add Customer Ledger for supplier delivery
                                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipmentData.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Delivery", debit: 0, credit: shipmentData.DeliveredAmount, shipmentId: shipmentData.Id);

                                    //Add customer ledger for delivery cost on supplier
                                    if (shipmentData.DeliveryCostType == DeliveryCostType.FulfilledBySupplier && shipmentData.DeliveryCost > 0)
                                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipmentData.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Delivery cost on supplier", debit: 0, credit: shipmentData.DeliveryCost, shipmentId: shipmentData.Id, updateRecord: true);
                                }

                            }
                            else
                                logMessage.Append("Order Ids not found for Shipment: " + shipmentData.OrderId + Environment.NewLine);
                        }
                        else
                            logMessage.Append("Inventory Ids not found for Shipment: " + inventoryid + Environment.NewLine);


                    }
                    catch (Exception ex)
                    {

                        await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Shipment Info " + shipmentData.Id.ToString(), ex.Message);
                    }
                    iRow++;
                }

                if (logMessage.Length > 0)
                    await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Shipment ", logMessage.ToString());
            }
            catch (Exception ex)
            {

            }
        }

        public virtual async Task ImportInboundShipmentItemFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<ShipmentItem>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<ShipmentItem, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;
            DateTime? priceValidity = DateTime.UtcNow;
            DateTime? updatedOn = DateTime.UtcNow;
            DateTime? createOn = DateTime.UtcNow;
            var shipmentItemList = new List<ShipmentItem>();
            StringBuilder logMessage = new StringBuilder();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var shipmentItemData = new ShipmentItem();
                var id = 0;
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "id":
                            shipmentItemData.Id = property.IntValue;
                            break;
                        case "ShipmentId":
                            shipmentItemData.ShipmentId = property.IntValue;
                            break;
                        case "orderitemid":
                            shipmentItemData.OrderItemId = property.IntValue;
                            break;
                        case "Quantity":
                            shipmentItemData.Quantity = property.DecimalValue;
                            break;
                        case "warehouseid":
                            shipmentItemData.WarehouseId = property.IntValue;
                            break;
                    }
                }

                var warehouse = await _shippingService.GetWarehouseByIdAsync(shipmentItemData.WarehouseId);
                if (warehouse == null)
                {
                    var warehousefound = (await _shippingService.GetAllWarehousesAsync("No warehouse found")).FirstOrDefault();
                    if (warehousefound == null)
                    {
                        var warehouseinsert = new Warehouse
                        {
                            Name = "No warehouse found",
                        };
                        await _shippingService.InsertWarehouseAsync(warehouseinsert);
                        shipmentItemData.WarehouseId = warehouseinsert.Id;
                    }
                    else
                    {
                        shipmentItemData.WarehouseId = warehousefound.Id;
                    }
                }
                var _shipment = await _shipmentService.GetShipmentByIdAsync(shipmentItemData.ShipmentId);



                if (_shipment != null)
                {
                    var _order = await _orderService.GetOrderByIdAsync(_shipment.OrderId);
                    var _orderItem = (await _orderService.GetOrderItemsAsync(_order.Id)).FirstOrDefault();
                    if (_order != null && _orderItem != null)
                    {
                        shipmentItemData.OrderItemId = _orderItem.Id;
                        await _shipmentService.InsertShipmentItemAsync(shipmentItemData);
                    }
                    else
                    {
                        logMessage.Append("Shipment Item: Order Item not found against the 'Order ' Id " + _order.Id + Environment.NewLine);

                    }

                }
                else
                {
                    logMessage.Append("Shipment Item: Shipment Item not found against the 'Order Item' Id " + shipmentItemData.OrderItemId + " && 'Shipment Id' " + shipmentItemData.ShipmentId + Environment.NewLine);
                }

                iRow++;
            }
            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For shipment Item ", logMessage.ToString());
        }

        public virtual async Task ImportOutboundFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            await _dataProvider.SetTableIdentAsync<InventoryOutbound>(1);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<InventoryOutbound>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<InventoryOutbound, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            var OutboundList = new List<InventoryInbound>();
            StringBuilder logMessage = new StringBuilder();
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var OutboundData = new InventoryOutbound();
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "id":
                            OutboundData.Id = property.IntValue;
                            break;
                        case "inventoryinboundid":
                            OutboundData.InventoryInboundId = property.IntValue;
                            break;
                        case "saleorderid":
                            OutboundData.SaleOrderId = property.IntValue;
                            break;
                        case "orderitemid":
                            OutboundData.OrderItemId = property.IntValue;
                            break;
                        case "shipmentid":
                            OutboundData.ShipmentId = property.IntValue;
                            break;
                        case "deleted":
                            OutboundData.Deleted = property.IntValue == 1 ? true : false;
                            break;
                        case "outboundquantity":
                            OutboundData.OutboundQuantity = property.DecimalValue;
                            break;
                        case "CreatedOnUtc":
                            OutboundData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "UpdatedOnUtc":
                            OutboundData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "CreatedById":
                            OutboundData.CreatedById = property.IntValue;
                            break;
                        case "updatedbyid":
                            OutboundData.UpdatedById = property.IntValue;
                            break;
                        case "DeletedById":
                            OutboundData.DeletedById = property.IntValue;
                            break;
                        case "inventoryoutboundstatusid":
                            OutboundData.InventoryOutboundStatusId = property.IntValue;
                            break;
                        case "InventoryTypeId":
                            OutboundData.InventoryOutboundTypeId = property.IntValue;
                            break;
                    }
                }

                var inbound = await _inventoryService.GetInventoryInboundByIdAsync(OutboundData.InventoryInboundId);
                var saleOrder = await _orderService.GetOrderByIdAsync(OutboundData.SaleOrderId);
                var saleOrderitem = await _orderService.GetOrderItemByIdAsync(OutboundData.OrderItemId);
                var shipment = await _shipmentService.GetShipmentByIdAsync(OutboundData.ShipmentId);
                if (inbound != null && saleOrder != null && saleOrderitem != null && shipment != null)
                {
                    OutboundData.InventoryGroupId = inbound.InventoryGroupId;
                    await _inventoryService.InsertInventoryOutboundAsync(OutboundData);
                }
                else
                {
                    if (inbound == null)
                        logMessage.Append("inbound Id " + OutboundData.InventoryInboundId + " not found for outbound: " + OutboundData.Id + Environment.NewLine);

                    else if (saleOrder == null)
                        logMessage.Append("saleOrder Id " + OutboundData.SaleOrderId + " not found for outbound: " + OutboundData.Id + Environment.NewLine);

                    else if (shipment == null)
                        logMessage.Append("shipment Id " + OutboundData.ShipmentId + " not found for outbound: " + OutboundData.Id + Environment.NewLine);

                    else if (saleOrderitem == null)
                        logMessage.Append("Sale Order item Id " + OutboundData.OrderItemId + " not found for outbound: " + OutboundData.Id + Environment.NewLine);
                }
                //await _requestService.InsertRequestAsync(requestData);
                //requestList.Add(requestData);
                iRow++;
            }

            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For outbound ", logMessage.ToString());
        }

        public virtual async Task ImportOrderCalculationFromXlsxAsync(Stream stream)
        {
            try
            {
                using var workbook = new XLWorkbook(stream);
                if (workbook == null)
                    throw new ZarayeException("No worksheet found");

                var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //the columns
                var metadata = GetWorkbookMetadata<OrderCalculation>(workbook, languages);
                var defaultWorksheet = metadata.DefaultWorksheet;
                var defaultProperties = metadata.DefaultProperties;
                var localizedProperties = metadata.LocalizedProperties;

                var manager = new PropertyManager<OrderCalculation, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

                var iRow = 2;
                var brand = "";
                var buyerRequestId = 0;
                //DateTime? priceValidity = DateTime.UtcNow;
                //DateTime? updatedOn = DateTime.UtcNow;
                //DateTime? createOn = DateTime.UtcNow;
                var businessModelId = 0;
                var financeCostPayment = 0;
                StringBuilder logMessage = new StringBuilder();
                var ordercalucaltionList = new List<OrderCalculation>();

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetDefaultProperties
                      .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                      .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                    var ordercalulationData = new OrderCalculation();
                    var id = 0;
                    foreach (var property in manager.GetDefaultProperties)
                    {
                        switch (property.PropertyName.Trim())
                        {
                            case "OrderId":
                                ordercalulationData.OrderId = property.IntValue;
                                break;
                            case "BusinessModelId":
                                businessModelId = property.IntValue;
                                break;
                            case "GrossCommissionRate":
                                ordercalulationData.GrossCommissionRate = property.DecimalValue;
                                break;
                            case "GrossCommissionRateType":
                                ordercalulationData.GrossCommissionRateType = property.StringValue;
                                break;
                            case "GrossCommissionAmount":
                                ordercalulationData.GrossCommissionAmount = property.DecimalValue;
                                break;
                            case "GSTIncludedInTotalAmount":
                                ordercalulationData.GSTIncludedInTotalAmount = property.IntValue == 1 ? true : false;
                                break;
                            case "NetAmount":
                                ordercalulationData.NetAmount = property.DecimalValue;
                                break;
                            case "NetAmountWithoutGST":
                                ordercalulationData.NetAmountWithoutGST = property.DecimalValue;
                                break;
                            case "NetRateWithMargin":
                                ordercalulationData.NetRateWithMargin = property.DecimalValue;
                                break;
                            case "SubTotal":
                                ordercalulationData.SubTotal = property.DecimalValue;
                                break;
                            case "OrderTotal":
                                ordercalulationData.OrderTotal = property.DecimalValue;
                                break;
                            case "TotalPerBag":
                                ordercalulationData.TotalPerBag = property.DecimalValue;
                                break;
                            case "GSTRate":
                                ordercalulationData.GSTRate = property.DecimalValue;
                                break;
                            case "GSTAmount":
                                ordercalulationData.GSTAmount = property.DecimalValue;
                                break;
                            case "WHTRate":
                                ordercalulationData.WHTRate = property.DecimalValue;
                                break;
                            case "WHTAmount":
                                ordercalulationData.WHTAmount = property.DecimalValue;
                                break;
                            case "WholesaleTaxRate":
                                ordercalulationData.WholesaleTaxRate = property.DecimalValue;
                                break;
                            case "WholesaleTaxAmount":
                                ordercalulationData.WholesaleTaxAmount = property.DecimalValue;
                                break;
                            case "MarginRate":
                                ordercalulationData.MarginRate = property.DecimalValue;
                                break;
                            case "MarginRateType":
                                ordercalulationData.MarginRateType = property.StringValue;
                                break;
                            case "MarginAmount":
                                ordercalulationData.MarginAmount = property.DecimalValue;
                                break;
                            case "DiscountRate":
                                ordercalulationData.DiscountRate = property.DecimalValue;
                                break;
                            case "DiscountRateType":
                                ordercalulationData.DiscountRateType = property.StringValue;
                                break;
                            case "DiscountAmount":
                                ordercalulationData.DiscountAmount = property.DecimalValue;
                                break;
                            case "ProductPrice":
                                ordercalulationData.ProductPrice = property.DecimalValue;
                                break;
                            case "SellingPriceOfProduct":
                                ordercalulationData.SellingPriceOfProduct = property.DecimalValue;
                                break;
                            case "FinanceIncome":
                                ordercalulationData.FinanceIncome = property.DecimalValue;
                                break;
                            case "BuyerCommissionReceivablePerBag":
                                ordercalulationData.BuyerCommissionReceivablePerBag = property.DecimalValue;
                                break;
                            case "BuyerCommissionPayablePerBag":
                                ordercalulationData.BuyerCommissionPayablePerBag = property.DecimalValue;
                                break;
                            case "SellingPrice_FinanceIncome":
                                ordercalulationData.SellingPrice_FinanceIncome = property.DecimalValue;
                                break;
                            case "TotalReceivableBuyer":
                                ordercalulationData.TotalReceivableBuyer = property.DecimalValue;
                                break;
                            case "TotalReceivableFromBuyerDirectlyToSupplier":
                                ordercalulationData.TotalReceivableFromBuyerDirectlyToSupplier = property.DecimalValue;
                                break;
                            case "InvoicedAmount":
                                ordercalulationData.InvoicedAmount = property.DecimalValue;
                                break;
                            case "BrokerCash":
                                ordercalulationData.BrokerCash = property.DecimalValue;
                                break;
                            case "SupplierCommissionBag":
                                ordercalulationData.SupplierCommissionBag = property.DecimalValue;
                                break;
                            case "SupplierCommissionReceivableRate":
                                ordercalulationData.SupplierCommissionReceivableRate = property.DecimalValue;
                                break;
                            case "SupplierCommissionReceivableAmount":
                                ordercalulationData.SupplierCommissionReceivableAmount = property.DecimalValue;
                                break;
                            case "SupplierCommissionReceivableType":
                                ordercalulationData.SupplierCommissionReceivableType = property.StringValue;
                                break;
                            case "BuyerPaymentTerms":
                                ordercalulationData.BuyerPaymentTerms = property.DecimalValue;
                                break;
                            case "TotalFinanceCost":
                                ordercalulationData.TotalFinanceCost = property.DecimalValue;
                                break;
                            case "SupplierCreditTerms":
                                ordercalulationData.SupplierCreditTerms = property.DecimalValue;
                                break;
                            case "FinanceCostPayment":
                                financeCostPayment = property.IntValue;
                                break;
                            case "FinanceCost":
                                ordercalulationData.FinanceCost = property.DecimalValue;
                                break;
                            case "InterestAccrued":
                                ordercalulationData.InterestAccrued = property.DecimalValue;
                                break;
                            case "PayableToMill":
                                ordercalulationData.PayableToMill = property.DecimalValue;
                                break;
                            case "PaymentInCash":
                                ordercalulationData.PaymentInCash = property.DecimalValue;
                                break;
                            case "BuyerCommission":
                                ordercalulationData.BuyerCommission = property.DecimalValue;
                                break;
                            case "SupplierCommissionReceivable_Summary":
                                ordercalulationData.SupplierCommissionReceivable_Summary = property.DecimalValue;
                                break;
                            case "GrossAmount":
                                ordercalulationData.GrossAmount = property.DecimalValue;
                                break;
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------//
                    // Business ModelId Conditions

                    if (businessModelId == 0)
                        ordercalulationData.BusinessModelId = 10;

                    if (businessModelId == 10)
                        ordercalulationData.BusinessModelId = 20;

                    if (businessModelId == 20)
                        ordercalulationData.BusinessModelId = 30;

                    if (businessModelId == 30)
                        ordercalulationData.BusinessModelId = 40;

                    if (businessModelId == 40)
                        ordercalulationData.BusinessModelId = 50;

                    if (businessModelId == 50)
                        ordercalulationData.BusinessModelId = 60;

                    if (businessModelId == 60)
                        ordercalulationData.BusinessModelId = 70;

                    if (businessModelId == 70)
                        ordercalulationData.BusinessModelId = 80;

                    //------------------------------------------------------------------------------------------------------------------------------//

                    ordercalucaltionList.Add(ordercalulationData);
                    iRow++;

                }
                foreach (var ordercalculationData in ordercalucaltionList)
                {
                    try
                    {
                        var order = await _orderService.GetOrderByIdAsync(ordercalculationData.OrderId);
                        if (order != null)
                        {
                            await _orderService.InsertOrderCalculationAsync(ordercalculationData);
                        }
                        else
                            logMessage.Append("Order Ids not found for Order Calculation Info: " + ordercalculationData.OrderId + Environment.NewLine);


                    }
                    catch (Exception ex)
                    {
                        await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Order Calculation Info " + ordercalculationData.Id.ToString(), ex.Message);
                    }
                }
                if (logMessage.Length > 0)
                    await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Order Calculation Info ", logMessage.ToString());
            }
            catch
            {

            }
        }

        public virtual async Task ImportPaymentFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);

            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Payment>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Payment, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            var paymentList = new List<Payment>();
            StringBuilder logMessage = new StringBuilder();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var paymentData = new Payment();
                var id = 0;
                var bankname = "";
                var accountnumber = "";
                var accounttitle = "";
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "Id":
                            paymentData.Id = property.IntValue;
                            break;
                        case "Amount":
                            paymentData.Amount = property.DecimalValue;
                            break;
                        case "CustomerId":
                            paymentData.CustomerId = property.IntValue;
                            break;
                        case "BankDetailId":
                            paymentData.BankDetailId = property.IntValue;
                            break;
                        case "PaymentType":
                            paymentData.PaymentType = property.StringValue;
                            break;
                        case "PaymentDateUtc":
                            paymentData.PaymentDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "ModeOfPaymentId":
                            paymentData.ModeOfPaymentId = property.IntValue;
                            break;
                        case "CompanyAccountTitle":
                            paymentData.CompanyAccountTitle = property.StringValue;
                            break;
                        case "VerificationNumber":
                            paymentData.VerificationNumber = property.StringValue;
                            break;
                        case "Comments":
                            paymentData.Comments = property.StringValue;
                            break;
                        case "CreatedOnUtc":
                            paymentData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "UpdatedOnUtc":
                            paymentData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "CreatedById":
                            paymentData.CreatedById = property.IntValue;
                            break;
                        case "UpdatedById":
                            paymentData.UpdatedById = property.IntValue;
                            break;
                        case "Deleted":
                            paymentData.Deleted = property.IntValue == 1 ? true : false;
                            break;
                        case "DeletedById":
                            paymentData.DeletedById = property.IntValue;
                            break;
                        case "isBusinessApproved":
                            paymentData.IsBusienssApproved = property.IntValue == 1 ? true : false;
                            break;
                        case "BusinessId":
                            paymentData.BusinessId = property.IntValue;
                            break;
                        case "BusinessComment":
                            paymentData.BusinessComment = property.StringValue;
                            break;
                        case "IsFinanceApproved":
                            paymentData.IsFinanceApproved = property.IntValue == 1 ? true : false;
                            break;
                        case "FinanceId":
                            paymentData.FinanceId = property.IntValue;
                            break;
                        case "FinanceComment":
                            paymentData.FinanceComment = property.StringValue;
                            break;
                        case "PaymentStatusId":
                            paymentData.PaymentStatusId = property.IntValue;
                            break;
                        case "BusinessActionDateUtc":
                            paymentData.BusinessActionDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "FinanceActionDateUtc":
                            paymentData.FinanceActionDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "CustomPaymentNumber":
                            paymentData.CustomPaymentNumber = property.StringValue;
                            break;
                        //case "AdjustAmount":
                        //    paymentData.AdjustAmount = property.DecimalValue;
                        //    break;
                        case "CompanyBankAccountId":
                            paymentData.CompanyBankAccountId = property.IntValue;
                            break;
                        case "bankname":
                            bankname = property.StringValue;
                            break;
                        case "accountnumber":
                            accountnumber = property.StringValue;
                            break;
                        case "accounttitle":
                            accounttitle = property.StringValue;
                            break;
                    }
                }

                var paymentId = 0;
                paymentData.CustomPaymentNumber = _customNumberFormatter.GeneratePaymentCustomNumber(paymentData);
                var checkBankDetail = await _customerService.GetBankDetailByBAAAsync(bankname, accountnumber, accounttitle);

                if (!string.IsNullOrWhiteSpace(bankname) && !string.IsNullOrWhiteSpace(accountnumber) && !string.IsNullOrWhiteSpace(accounttitle))
                {
                    if (checkBankDetail == null)
                    {
                        var bankDetail = new BankDetail();
                        bankDetail.BankName = bankname;
                        bankDetail.AccountNumber = accountnumber;
                        bankDetail.AccountTitle = accounttitle;
                        bankDetail.CustomerId = paymentData.CustomerId;
                        bankDetail.Published = true;
                        bankDetail.CreatedOnUtc = DateTime.UtcNow;
                        bankDetail.Deleted = false;
                        await _customerService.InsertBankDetailAsync(bankDetail);

                        paymentData.BankDetailId = bankDetail.Id;
                        paymentId = paymentData.Id;
                        await _customerLedgerService.InsertPaymentAsync(paymentData);
                    }
                    else
                    {
                        paymentData.BankDetailId = checkBankDetail.Id;
                        paymentId = paymentData.Id;
                        await _customerLedgerService.InsertPaymentAsync(paymentData);
                    }
                }
                else
                {
                    var _fullName = await _customerService.GetCustomerFullNameAsync(paymentData.CustomerId);
                    if (checkBankDetail == null)
                    {
                        var bankDetail = new BankDetail();
                        bankDetail.BankName = "N/A";
                        bankDetail.AccountNumber = "N/A";
                        bankDetail.AccountTitle = _fullName;
                        bankDetail.CustomerId = paymentData.CustomerId;
                        bankDetail.Published = true;
                        bankDetail.CreatedOnUtc = DateTime.UtcNow;
                        bankDetail.Deleted = false;
                        await _customerService.InsertBankDetailAsync(bankDetail);

                        paymentData.BankDetailId = bankDetail.Id;
                        paymentId = paymentData.Id;
                        await _customerLedgerService.InsertPaymentAsync(paymentData);
                    }
                    else
                    {
                        paymentData.BankDetailId = checkBankDetail.Id;
                        paymentId = paymentData.Id;
                        await _customerLedgerService.InsertPaymentAsync(paymentData);
                    }
                }

                //Add Customer Ledger for supplier payment paid
                await _customerLedgerService.AddCustomerLedgerAsync(date: paymentData.PaymentDateUtc, customerId: paymentData.CustomerId, description: "Payment", debit: paymentData.Amount, credit: 0, paymentId: paymentId);

                iRow++;
            }

            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Payment Info ", logMessage.ToString());
        }

        public virtual async Task ImportPaymentShipmentMappingFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<ShipmentPaymentMapping>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<ShipmentPaymentMapping, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            StringBuilder logMessage = new StringBuilder();
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var shipmentPaymentMappingData = new ShipmentPaymentMapping();
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "id":
                            shipmentPaymentMappingData.Id = property.IntValue;
                            break;
                        case "shipmentid":
                            shipmentPaymentMappingData.ShipmentId = property.IntValue;
                            break;
                        case "paymentid":
                            shipmentPaymentMappingData.PaymentId = property.IntValue;
                            break;
                        case "amount":
                            shipmentPaymentMappingData.Amount = property.DecimalValue;
                            break;
                        case "createdonutc":
                            shipmentPaymentMappingData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "createdbyid":
                            shipmentPaymentMappingData.CreatedById = property.IntValue;
                            break;
                    }
                }

                var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentPaymentMappingData.ShipmentId);
                if (shipment != null)
                {
                    var payment = await _customerLedgerService.GetPaymentByIdAsync(shipmentPaymentMappingData.PaymentId);
                    if (payment != null)
                    {
                        payment.AdjustAmount += shipmentPaymentMappingData.Amount;
                        await _customerLedgerService.UpdatePaymentAsync(payment);

                        var _order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
                        if (payment.AdjustAmount == shipment.DeliveredAmount)
                        {
                            shipment.PaymentStatusId = (int)PaymentStatus.Paid;
                            await _shipmentService.UpdateShipmentAsync(shipment);

                            if (_order != null)
                            {
                                _order.PaymentStatusId = (int)PaymentStatus.Paid;
                                await _orderService.UpdateOrderAsync(_order);
                            }
                        }
                        if (payment.AdjustAmount > 0)
                        {
                            _order.PaymentStatusId = (int)PaymentStatus.PartiallyPaid;
                            await _orderService.UpdateOrderAsync(_order);
                        }
                        else if (payment.AdjustAmount <= 0)
                        {
                            _order.PaymentStatusId = (int)PaymentStatus.Pending;
                            await _orderService.UpdateOrderAsync(_order);
                        }

                        await _customerLedgerService.InsertShipmentPaymentMappingAsync(new ShipmentPaymentMapping
                        {
                            Id = shipmentPaymentMappingData.Id,
                            PaymentId = payment.Id,
                            ShipmentId = shipment.Id,
                            Amount = shipmentPaymentMappingData.Amount,
                            CreatedById = shipmentPaymentMappingData.CreatedById,
                            CreatedOnUtc = shipmentPaymentMappingData.CreatedOnUtc
                        });
                    }
                    else
                        logMessage.Append("shipment Payment Mapping: Payment not found: " + shipmentPaymentMappingData.PaymentId + Environment.NewLine);
                }
                else
                    logMessage.Append("shipment Payment Mapping: Sipment not found: " + shipmentPaymentMappingData.ShipmentId + Environment.NewLine);

                iRow++;
            }
            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For shipment Payment Mapping ", logMessage.ToString());
        }

        public virtual async Task ImportInventoryPaymentFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Payment>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Payment, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            var paymentList = new List<Payment>();
            StringBuilder logMessage = new StringBuilder();
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var paymentData = new Payment();
                var paymentId = 0;
                var inventoryId = 0;
                var bankname = "";
                var accountnumber = "";
                var accounttitle = "";
                var paymentterm = "";
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "id":
                            paymentData.Id = property.IntValue;
                            break;
                        case "amount":
                            paymentData.Amount = property.IntValue;
                            break;
                        case "inventoryid":
                            inventoryId = property.IntValue;
                            break;
                        case "Customerid":
                            paymentData.CustomerId = property.IntValue;
                            break;
                        case "BankDetailId":
                            paymentData.BankDetailId = property.IntValue;
                            break;
                        case "PaymentType":
                            paymentData.PaymentType = property.StringValue;
                            break;
                        case "PaymentDateUtc":
                            paymentData.PaymentDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "ModeOfPaymentId":
                            paymentData.ModeOfPaymentId = property.IntValue;
                            break;
                        case "CompanyAccountTitle":
                            paymentData.CompanyAccountTitle = property.StringValue;
                            break;
                        case "VerificationNumber":
                            paymentData.VerificationNumber = property.StringValue;
                            break;
                        case "Comments":
                            paymentData.Comments = property.StringValue;
                            break;
                        case "CreatedOnUtc":
                            paymentData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "UpdatedOnUtc":
                            paymentData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "CreatedById":
                            paymentData.CreatedById = property.IntValue;
                            break;
                        case "UpdatedById":
                            paymentData.UpdatedById = property.IntValue;
                            break;
                        case "Deleted":
                            paymentData.Deleted = property.IntValue == 1 ? true : false;
                            break;
                        case "DeletedById":
                            paymentData.DeletedById = property.IntValue;
                            break;
                        case "IsBusienssApproved":
                            paymentData.IsBusienssApproved = property.IntValue == 1 ? true : false;
                            break;
                        case "BusinessId":
                            paymentData.BusinessId = property.IntValue;
                            break;
                        case "BusinessComment":
                            paymentData.BusinessComment = property.StringValue;
                            break;
                        case "IsFinanceApproved":
                            paymentData.IsFinanceApproved = property.IntValue == 1 ? true : false;
                            break;
                        case "FinanceId":
                            paymentData.FinanceId = property.IntValue;
                            break;
                        case "FinanceComment":
                            paymentData.FinanceComment = property.StringValue;
                            break;
                        case "PaymentStatusId":
                            paymentData.PaymentStatusId = property.IntValue;
                            break;
                        case "BusinessActionDateUtc":
                            paymentData.BusinessActionDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "FinanceActionDateUtc":
                            paymentData.FinanceActionDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "CustomPaymentNumber":
                            paymentData.CustomPaymentNumber = property.StringValue;
                            break;
                        //case "AdjustAmount":
                        //    paymentData.AdjustAmount = property.DecimalValue;
                        //    break;
                        case "CompanyBankAccountId":
                            paymentData.CompanyBankAccountId = property.IntValue;
                            break;
                        case "bankname":
                            bankname = property.StringValue;
                            break;
                        case "accountnumber":
                            accountnumber = property.StringValue;
                            break;
                        case "accounttitle":
                            accounttitle = property.StringValue;
                            break;
                        case "paymentterm":
                            paymentterm = property.StringValue;
                            break;
                    }
                }

                if (paymentData.CustomerId > 0)
                {
                    paymentData.CustomPaymentNumber = _customNumberFormatter.GeneratePaymentCustomNumber(paymentData);
                    var checkBankDetail = await _customerService.GetBankDetailByBAAAsync(bankname, accountnumber, accounttitle);
                    if (!string.IsNullOrWhiteSpace(bankname) && !string.IsNullOrWhiteSpace(accountnumber) && !string.IsNullOrWhiteSpace(accounttitle))
                    {
                        if (checkBankDetail == null)
                        {
                            var bankDetail = new BankDetail();
                            bankDetail.BankName = bankname;
                            bankDetail.AccountNumber = accountnumber;
                            bankDetail.AccountTitle = accounttitle;
                            bankDetail.CustomerId = paymentData.CustomerId;
                            bankDetail.Published = true;
                            bankDetail.CreatedOnUtc = DateTime.UtcNow;
                            bankDetail.Deleted = false;
                            await _customerService.InsertBankDetailAsync(bankDetail);

                            paymentData.BankDetailId = bankDetail.Id;
                            paymentId = paymentData.Id;
                            await _customerLedgerService.InsertPaymentAsync(paymentData);
                        }
                        else
                        {
                            paymentData.BankDetailId = checkBankDetail.Id;
                            paymentId = paymentData.Id;
                            await _customerLedgerService.InsertPaymentAsync(paymentData);
                        }
                    }
                    else
                    {
                        var _fullName = await _customerService.GetCustomerFullNameAsync(paymentData.CustomerId);
                        if (checkBankDetail == null)
                        {
                            var bankDetail = new BankDetail();
                            bankDetail.BankName = "N/A";
                            bankDetail.AccountNumber = "N/A";
                            bankDetail.AccountTitle = _fullName;
                            bankDetail.CustomerId = paymentData.CustomerId;
                            bankDetail.Published = true;
                            bankDetail.CreatedOnUtc = DateTime.UtcNow;
                            bankDetail.Deleted = false;
                            await _customerService.InsertBankDetailAsync(bankDetail);

                            paymentData.BankDetailId = bankDetail.Id;
                            paymentId = paymentData.Id;
                            await _customerLedgerService.InsertPaymentAsync(paymentData);
                        }
                        else
                        {
                            paymentData.BankDetailId = checkBankDetail.Id;
                            paymentId = paymentData.Id;
                            await _customerLedgerService.InsertPaymentAsync(paymentData);
                        }
                    }
                    //Add Customer Ledger for supplier payment paid
                    await _customerLedgerService.AddCustomerLedgerAsync(date: paymentData.PaymentDateUtc, customerId: paymentData.CustomerId, description: "Payment", debit: paymentData.Amount, credit: 0, paymentId: paymentId);
                }
                iRow++;
            }

            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Payment Info ", logMessage.ToString());
        }

        public virtual async Task ImportInventoryPaymentShipmentMappingFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            if (workbook == null)
                throw new ZarayeException("No worksheet found");

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<ShipmentPaymentMapping>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<ShipmentPaymentMapping, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;

            StringBuilder logMessage = new StringBuilder();
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                  .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                  .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var shipmentPaymentMappingData = new ShipmentPaymentMapping();
                //DateTime? createOn = DateTime.UtcNow;
                int inventoryid = 0;
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName.Trim())
                    {
                        case "id":
                            shipmentPaymentMappingData.Id = property.IntValue;
                            break;
                        case "inventoryid":
                            inventoryid = property.IntValue;
                            break;
                        case "shipmentid":
                            shipmentPaymentMappingData.ShipmentId = property.IntValue;
                            break;
                        case "paymentid":
                            shipmentPaymentMappingData.PaymentId = property.IntValue;
                            break;
                        case "amount":
                            shipmentPaymentMappingData.Amount = property.DecimalValue;
                            break;
                        case "createdonutc":
                            shipmentPaymentMappingData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                            break;
                        case "createdbyid":
                            shipmentPaymentMappingData.CreatedById = property.IntValue;
                            break;
                    }
                }

                var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentPaymentMappingData.ShipmentId);
                if (shipment != null)
                {
                    var payment = await _customerLedgerService.GetPaymentByIdAsync(shipmentPaymentMappingData.PaymentId);
                    if (payment != null)
                    {
                        payment.AdjustAmount += shipmentPaymentMappingData.Amount;
                        await _customerLedgerService.UpdatePaymentAsync(payment);

                        var _order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
                        if (payment.AdjustAmount == shipment.DeliveredAmount)
                        {
                            shipment.PaymentStatusId = (int)PaymentStatus.Paid;
                            await _shipmentService.UpdateShipmentAsync(shipment);
                            if (_order != null)
                            {
                                _order.PaymentStatusId = (int)PaymentStatus.Paid;
                                await _orderService.UpdateOrderAsync(_order);
                            }
                        }
                        if (payment.AdjustAmount > 0)
                        {
                            _order.PaymentStatusId = (int)PaymentStatus.PartiallyPaid;
                            await _orderService.UpdateOrderAsync(_order);
                        }
                        else if (payment.AdjustAmount <= 0)
                        {
                            _order.PaymentStatusId = (int)PaymentStatus.Pending;
                            await _orderService.UpdateOrderAsync(_order);
                        }

                        await _customerLedgerService.InsertShipmentPaymentMappingAsync(new ShipmentPaymentMapping
                        {
                            Id = shipmentPaymentMappingData.Id,
                            PaymentId = payment.Id,
                            ShipmentId = shipment.Id,
                            Amount = shipmentPaymentMappingData.Amount,
                            CreatedById = shipmentPaymentMappingData.CreatedById,
                            CreatedOnUtc = shipmentPaymentMappingData.CreatedOnUtc
                        });
                    }
                    else
                        logMessage.Append("shipment Payment Mapping: Payment not found: " + shipmentPaymentMappingData.PaymentId + Environment.NewLine);
                }
                else
                    logMessage.Append("shipment Payment Mapping: Sipment not found: " + shipmentPaymentMappingData.ShipmentId + Environment.NewLine);

                iRow++;
            }
            if (logMessage.Length > 0)
                await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For shipment Payment Mapping ", logMessage.ToString());
        }

        public virtual async Task ImportReturnSaleOrderFromXlsxAsync(Stream stream)
        {
            try
            {
                using var workbook = new XLWorkbook(stream);
                if (workbook == null)
                    throw new ZarayeException("No worksheet found");

                var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //the columns
                var metadata = GetWorkbookMetadata<Shipment>(workbook, languages);
                var defaultWorksheet = metadata.DefaultWorksheet;
                var defaultProperties = metadata.DefaultProperties;
                var localizedProperties = metadata.LocalizedProperties;

                var manager = new PropertyManager<Shipment, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

                var iRow = 2;
                StringBuilder logMessage = new StringBuilder();
                var shipmentList = new List<Shipment>();

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetDefaultProperties
                      .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                      .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                    var shipmentData = new Shipment();
                    var id = 0;
                    decimal quantity = 0m;
                    foreach (var property in manager.GetDefaultProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Id":
                                shipmentData.Id = property.IntValue;
                                break;
                            case "OrderId":
                                shipmentData.OrderId = property.IntValue;
                                break;
                            case "TrackingNumber":
                                shipmentData.TrackingNumber = property.StringValue;
                                break;
                            case "TotalWeight":
                                shipmentData.TotalWeight = property.DecimalValue;
                                break;
                            case "ShippedDateUtc":
                                shipmentData.ShippedDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "DeliveryDateUtc":
                                shipmentData.DeliveryDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ReadyForPickupDateUtc":
                                shipmentData.ReadyForPickupDateUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "AdminComment":
                                shipmentData.AdminComment = property.StringValue;
                                break;
                            case "CreatedOnUtc":
                                shipmentData.CreatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "UpdatedOnUtc":
                                shipmentData.UpdatedOnUtc = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "CreatedById":
                                shipmentData.CreatedById = property.IntValue;
                                break;
                            case "UpdatedById":
                                shipmentData.UpdatedById = property.IntValue;
                                break;
                            case "Deleted":
                                shipmentData.Deleted = property.IntValue == 1 ? true : false;
                                break;
                            case "DeletedById":
                                shipmentData.DeletedById = property.IntValue;
                                break;
                            case "TransporterId":
                                shipmentData.TransporterId = property.IntValue;
                                break;
                            case "VehicleId":
                                shipmentData.VehicleId = property.IntValue;
                                break;
                            case "DeliveryStatusId":
                                shipmentData.DeliveryStatusId = property.IntValue;
                                break;
                            case "PickupAddress":
                                shipmentData.PickupAddress = property.StringValue;
                                break;
                            case "RouteTypeId":
                                shipmentData.RouteTypeId = property.IntValue;
                                break;
                            case "ShipmentDeliveryAddress":
                                shipmentData.ShipmentDeliveryAddress = property.StringValue;
                                break;
                            case "PictureId":
                                shipmentData.PictureId = property.IntValue;
                                break;
                            case "TransporterTypeId":
                                shipmentData.TransporterTypeId = property.IntValue;
                                break;
                            case "DeliveryTypeId":
                                shipmentData.DeliveryTypeId = property.IntValue;
                                break;
                            case "DeliveryCostReasonId":
                                shipmentData.DeliveryCostReasonId = property.IntValue;
                                break;
                            case "DeliveryTimingId":
                                shipmentData.DeliveryTimingId = property.IntValue;
                                break;
                            case "DeliveryCostTypeId":
                                shipmentData.DeliveryCostTypeId = property.IntValue;
                                break;
                            case "DeliveryDelayedReasonId":
                                shipmentData.DeliveryDelayedReasonId = property.IntValue;
                                break;
                            case "WarehouseId":
                                shipmentData.WarehouseId = property.IntValue;
                                break;
                            case "DeliveryCost":
                                shipmentData.DeliveryCost = property.IntValue;
                                break;
                            case "FreightCharges":
                                shipmentData.FreightCharges = property.IntValue;
                                break;
                            case "LabourCharges":
                                shipmentData.LabourCharges = property.IntValue;
                                break;
                            case "ExpectedDateShipped":
                                shipmentData.ExpectedDateShipped = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ExpectedDateDelivered":
                                shipmentData.ExpectedDateDelivered = Convert.ToDateTime(string.IsNullOrEmpty(property.StringValue) || property.StringValue == " " ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ExpectedDeliveryCost":
                                shipmentData.ExpectedDeliveryCost = property.DecimalValue;
                                break;
                            case "ExpectedQuantity":
                                shipmentData.ExpectedQuantity = property.DecimalValue;
                                break;
                            case "DeliveredOrderAmount":
                                shipmentData.DeliveredAmount = property.DecimalValue;
                                break;
                            case "ShipmentTypeId":
                                shipmentData.ShipmentTypeId = property.IntValue;
                                break;
                            case "PaymentStatusid":
                                shipmentData.PaymentStatusId = property.IntValue;
                                break;
                            //case "ParentShipmentId":
                            //    shipmentData.CustomShipmentNumber = property.StringValue;
                            //    break;
                            case "quantity":
                                quantity = property.DecimalValue;
                                break;
                            case "IsDirectOrder":
                                shipmentData.IsDirectOrder = property.IntValue == 1 ? true : false;
                                break;
                            case "BuyerId":
                                shipmentData.BuyerId = property.IntValue;
                                break;
                            case "DeliveryRequestId":
                                shipmentData.DeliveryRequestId = property.IntValue;
                                break;
                            case "ShipmentReturnTypeId":
                                shipmentData.ShipmentReturnTypeId = property.IntValue;
                                break;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(shipmentData.CustomShipmentNumber))
                    {
                        if (shipmentData.ShipmentTypeId == (int)ShipmentType.ReturnSaleOrder)
                        {
                            shipmentData.CustomShipmentNumber = _customNumberFormatter.GenerateSOShipmentCustomNumber(shipmentData);
                        }
                    }
                    try
                    {
                        var order = await _orderService.GetOrderByIdAsync(shipmentData.OrderId);
                        if (order != null)
                        {
                            var parentRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                            if (parentRequest is not null)
                            {
                                var returnOrderId = await _orderProcessingService.ReOrderAsync(order, Math.Abs(quantity));
                                shipmentData.OrderId = returnOrderId;
                                await _shipmentService.InsertShipmentAsync(shipmentData);

                                var shipmentItem = new ShipmentItem();
                                shipmentItem.ShipmentId = shipmentData.Id;
                                shipmentItem.OrderItemId = (await _orderService.GetOrderItemsAsync(returnOrderId)).FirstOrDefault().Id;
                                shipmentItem.Quantity = Math.Abs(quantity);
                                shipmentItem.WarehouseId = shipmentData.WarehouseId;
                                await _shipmentService.InsertShipmentItemAsync(shipmentItem);

                                var returnSaleOrder = await _orderService.GetOrderByIdAsync(returnOrderId);
                                if (returnSaleOrder is not null)
                                {
                                    var request = await _requestService.GetRequestByIdAsync(returnSaleOrder.RequestId);
                                    if (request is not null)
                                    {
                                        await _orderProcessingService.DeliverAsync(shipmentData, true, shipmentData.DeliveryDateUtc);

                                        //add inventory inbound (Return)
                                        await _inventoryService.AddInventoryInboundReturnAsync(inboundQuantity: shipmentItem.Quantity, returnSaleOrder, request, shipmentData);

                                        //Add Customer Ledger for buyer delivery
                                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipmentData.DeliveryDateUtc.Value, customerId: returnSaleOrder.CustomerId, description: "Return", debit: 0, credit: shipmentData.DeliveredAmount, shipmentId: shipmentData.Id);

                                        //Add customer ledger for delivery cost on buyer
                                        if (shipmentData.DeliveryCostType == DeliveryCostType.DeliveryCostOnBuyer && shipmentData.DeliveryCost > 0)
                                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipmentData.DeliveryDateUtc.Value, customerId: returnSaleOrder.CustomerId, description: "Return delivery cost on buyer", debit: 0, credit: shipmentData.DeliveryCost, shipmentId: shipmentData.Id, updateRecord: true);
                                    }
                                }
                            }
                        }
                        else
                            logMessage.Append("Order Ids not found for Return Shipment: " + shipmentData.OrderId + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {

                        await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Return Shipment Info " + shipmentData.Id.ToString(), ex.Message);
                    }
                    iRow++;
                }
                if (logMessage.Length > 0)
                    await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Return Shipment ", logMessage.ToString());
            }
            catch (Exception ex)
            {

            }
        }

        public virtual async Task ImportReturnPurchaseOrderFromXlsxAsync(Stream stream)
        {
            try
            {
                using var workbook = new XLWorkbook(stream);
                if (workbook == null)
                    throw new ZarayeException("No worksheet found");

                var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //the columns
                var metadata = GetWorkbookMetadata<Shipment>(workbook, languages);
                var defaultWorksheet = metadata.DefaultWorksheet;
                var defaultProperties = metadata.DefaultProperties;
                var localizedProperties = metadata.LocalizedProperties;

                var manager = new PropertyManager<Shipment, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

                var iRow = 2;
                StringBuilder logMessage = new StringBuilder();
                var shipmentList = new List<Shipment>();

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetDefaultProperties
                      .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                      .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                    var shipmentData = new Shipment();
                    var id = 0;
                    decimal quantity = 0m;
                    foreach (var property in manager.GetDefaultProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Id":
                                shipmentData.Id = property.IntValue;
                                break;
                            case "OrderId":
                                shipmentData.OrderId = property.IntValue;
                                break;
                            case "TrackingNumber":
                                shipmentData.TrackingNumber = property.StringValue;
                                break;
                            case "TotalWeight":
                                shipmentData.TotalWeight = property.DecimalValue;
                                break;
                            case "ShippedDateUtc":
                                shipmentData.ShippedDateUtc = Convert.ToDateTime(property.PropertyValue == "" ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "DeliveryDateUtc":
                                shipmentData.DeliveryDateUtc = Convert.ToDateTime(property.PropertyValue == "" ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ReadyForPickupDateUtc":
                                shipmentData.ReadyForPickupDateUtc = Convert.ToDateTime(property.PropertyValue == "" ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "AdminComment":
                                shipmentData.AdminComment = property.StringValue;
                                break;
                            case "CreatedOnUtc":
                                shipmentData.CreatedOnUtc = Convert.ToDateTime(property.PropertyValue == "" ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "UpdatedOnUtc":
                                shipmentData.UpdatedOnUtc = Convert.ToDateTime(property.PropertyValue == "" ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "CreatedById":
                                shipmentData.CreatedById = property.IntValue;
                                break;
                            case "UpdatedById":
                                shipmentData.UpdatedById = property.IntValue;
                                break;
                            case "Deleted":
                                shipmentData.Deleted = property.BooleanValue;
                                break;
                            case "DeletedById":
                                shipmentData.DeletedById = property.IntValue;
                                break;
                            case "TransporterId":
                                shipmentData.TransporterId = property.IntValue;
                                break;
                            case "VehicleId":
                                shipmentData.VehicleId = property.IntValue;
                                break;
                            case "DeliveryStatusId":
                                shipmentData.DeliveryStatusId = property.IntValue;
                                break;
                            case "PickupAddress":
                                shipmentData.PickupAddress = property.StringValue;
                                break;
                            case "RouteTypeId":
                                shipmentData.RouteTypeId = property.IntValue;
                                break;
                            case "ShipmentDeliveryAddress":
                                shipmentData.ShipmentDeliveryAddress = property.StringValue;
                                break;
                            case "PictureId":
                                shipmentData.PictureId = property.IntValue;
                                break;
                            case "TransporterTypeId":
                                shipmentData.TransporterTypeId = property.IntValue;
                                break;
                            case "DeliveryTypeId":
                                shipmentData.DeliveryTypeId = property.IntValue;
                                break;
                            case "DeliveryCostReasonId":
                                shipmentData.DeliveryCostReasonId = property.IntValue;
                                break;
                            case "DeliveryTimingId":
                                shipmentData.DeliveryTimingId = property.IntValue;
                                break;
                            case "DeliveryCostTypeId":
                                shipmentData.DeliveryCostTypeId = property.IntValue;
                                break;
                            case "DeliveryDelayedReasonId":
                                shipmentData.DeliveryDelayedReasonId = property.IntValue;
                                break;
                            case "WarehouseId":
                                shipmentData.WarehouseId = property.IntValue;
                                break;
                            case "DeliveryCost":
                                shipmentData.DeliveryCost = property.IntValue;
                                break;
                            case "FreightCharges":
                                shipmentData.FreightCharges = property.IntValue;
                                break;
                            case "LabourCharges":
                                shipmentData.LabourCharges = property.IntValue;
                                break;
                            case "ExpectedDateShipped":
                                shipmentData.ExpectedDateShipped = Convert.ToDateTime(property.PropertyValue == "" ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ExpectedDateDelivered":
                                shipmentData.ExpectedDateDelivered = Convert.ToDateTime(property.PropertyValue == "" ? DateTime.UtcNow : property.PropertyValue);
                                break;
                            case "ExpectedDeliveryCost":
                                shipmentData.ExpectedDeliveryCost = property.DecimalValue;
                                break;
                            case "ExpectedQuantity":
                                shipmentData.ExpectedQuantity = property.DecimalValue;
                                break;
                            case "DeliveredOrderAmount":
                                shipmentData.DeliveredAmount = property.DecimalValue;
                                break;
                            case "ShipmentTypeId":
                                shipmentData.ShipmentTypeId = property.IntValue;
                                break;
                            case "PaymentStatusid":
                                shipmentData.PaymentStatusId = property.IntValue;
                                break;
                            //case "ParentShipmentId":
                            //    shipmentData.CustomShipmentNumber = property.StringValue;
                            //    break;
                            case "quantity":
                                quantity = property.DecimalValue;
                                break;
                            case "IsDirectOrder":
                                shipmentData.IsDirectOrder = property.BooleanValue;
                                break;
                            case "BuyerId":
                                shipmentData.BuyerId = property.IntValue;
                                break;
                            case "DeliveryRequestId":
                                shipmentData.DeliveryRequestId = property.IntValue;
                                break;
                            case "ShipmentReturnTypeId":
                                shipmentData.ShipmentReturnTypeId = property.IntValue;
                                break;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(shipmentData.CustomShipmentNumber))
                    {
                        if (shipmentData.ShipmentTypeId == (int)ShipmentType.ReturnSaleOrder)
                        {
                            shipmentData.CustomShipmentNumber = _customNumberFormatter.GenerateSOShipmentCustomNumber(shipmentData);
                        }
                    }
                    try
                    {
                        var order = await _orderService.GetOrderByIdAsync(shipmentData.OrderId);
                        if (order != null)
                        {
                            var parentRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                            if (parentRequest is not null)
                            {
                                var returnOrderId = await _orderProcessingService.ReOrderAsync(order, Math.Abs(quantity));
                                shipmentData.OrderId = returnOrderId;
                                await _shipmentService.InsertShipmentAsync(shipmentData);

                                var shipmentItem = new ShipmentItem();
                                shipmentItem.ShipmentId = shipmentData.Id;
                                shipmentItem.OrderItemId = (await _orderService.GetOrderItemsAsync(returnOrderId)).FirstOrDefault().Id;
                                shipmentItem.Quantity = Math.Abs(quantity);
                                shipmentItem.WarehouseId = shipmentData.WarehouseId;
                                await _shipmentService.InsertShipmentItemAsync(shipmentItem);

                                var returnPurchaseOrder = await _orderService.GetOrderByIdAsync(returnOrderId);
                                if (returnPurchaseOrder is not null)
                                {
                                    var request = await _requestService.GetRequestByIdAsync(returnPurchaseOrder.RequestId);
                                    if (request is not null)
                                    {
                                        await _orderProcessingService.DeliverAsync(shipmentData, true, shipmentData.DeliveryDateUtc);

                                        var shipment = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).FirstOrDefault();
                                        //Update inventory inbound to 'InTransit'
                                        var outboundQuantity = shipmentItem.Quantity;
                                        var inventoryInbound = (await _inventoryService.GetAllInventoryInboundsAsync(shipmentNumber: shipment.Id)).FirstOrDefault();
                                        if (inventoryInbound != null)
                                        {
                                            var outboundQty = (await _inventoryService.GetAllInventoryOutboundsAsync(InventoryInboundId: inventoryInbound.Id)).Sum(x => x.OutboundQuantity);
                                            var balanceQuantity = inventoryInbound.StockQuantity - outboundQty;
                                            if (balanceQuantity > 0)
                                            {
                                                var remainingQty = Math.Min(outboundQuantity, balanceQuantity);
                                                outboundQuantity -= remainingQty;
                                                await _inventoryService.AddInventoryPurchaseReturnOutboundAsync(inventoryInbound, returnPurchaseOrder, shipmentData, (await _orderService.GetOrderItemsAsync(returnOrderId)).FirstOrDefault(), remainingQty);
                                            }
                                        }

                                        await _orderProcessingService.DeliverAsync(shipmentData, true, shipmentData.DeliveryDateUtc);

                                        //Add Customer Ledger for supplier delivery
                                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipmentData.DeliveryDateUtc.Value, customerId: returnPurchaseOrder.CustomerId, description: "Return", debit: shipmentData.DeliveredAmount, credit: 0, shipmentId: shipmentData.Id);

                                        //Add customer ledger for delivery cost on supplier
                                        if (shipmentData.DeliveryCostType == DeliveryCostType.FulfilledBySupplier && shipmentData.DeliveryCost > 0)
                                            await _customerLedgerService.AddCustomerLedgerAsync(date: shipmentData.DeliveryDateUtc.Value, customerId: returnPurchaseOrder.CustomerId, description: "Return delivery cost on supplier", debit: shipmentData.DeliveryCost, credit: 0, shipmentId: shipmentData.Id, updateRecord: true);

                                    }
                                }
                            }
                        }
                        else
                            logMessage.Append("Order Ids not found for Return Shipment: " + shipmentData.OrderId + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {

                        await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Return Shipment Info " + shipmentData.Id.ToString(), ex.Message);
                    }
                    iRow++;
                }
                if (logMessage.Length > 0)
                    await _logger.InsertLogAsync(LogLevel.Debug, "Debugging For Return Shipment ", logMessage.ToString());
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Nested classes

        protected class CampaignEmailModel
        {
            public int CampaignId { get; set; }
            public string Email { get; set; }
            public bool Active { get; set; }
            public DateTime CreatedOnUtc { get; set; }
        }

        protected class AreaModel
        {
            public string Area { get; set; }

            public string City { get; set; }
        }

        protected partial class ProductPictureMetadata
        {
            public Product ProductItem { get; set; }

            public string Picture1Path { get; set; }

            public string Picture2Path { get; set; }

            public string Picture3Path { get; set; }

            public bool IsNew { get; set; }
        }

        public partial class CategoryKey
        {
            /// <returns>A task that represents the asynchronous operation</returns>
            public static async Task<CategoryKey> CreateCategoryKeyAsync(Category category, ICategoryService categoryService, IList<Category> allCategories, IStoreMappingService storeMappingService)
            {
                return new CategoryKey(await categoryService.GetFormattedBreadCrumbAsync(category, allCategories), category.LimitedToStores ? (await storeMappingService.GetStoresIdsWithAccessAsync(category)).ToList() : new List<int>())
                {
                    Category = category
                };
            }

            public CategoryKey(string key, List<int> storesIds = null)
            {
                Key = key.Trim();
                StoresIds = storesIds ?? new List<int>();
            }

            public List<int> StoresIds { get; }

            public Category Category { get; private set; }

            public string Key { get; }

            public bool Equals(CategoryKey y)
            {
                if (y == null)
                    return false;

                if (Category != null && y.Category != null)
                    return Category.Id == y.Category.Id;

                if ((StoresIds.Any() || y.StoresIds.Any())
                    && (StoresIds.All(id => !y.StoresIds.Contains(id)) || y.StoresIds.All(id => !StoresIds.Contains(id))))
                    return false;

                return Key.Equals(y.Key);
            }

            public override int GetHashCode()
            {
                if (!StoresIds.Any())
                    return Key.GetHashCode();

                var storesIds = StoresIds.Select(id => id.ToString())
                    .Aggregate(string.Empty, (all, current) => all + current);

                return $"{storesIds}_{Key}".GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as CategoryKey;
                return other?.Equals(other) ?? false;
            }
        }

        protected class BuyerTypeData
        {
            public int BuyerId { get; set; }

            public string BuyerType { get; set; }
        }

        protected class BuyerData
        {
            public DateTime? Date { get; set; }

            public string CustomerName { get; set; }

            public string ContactNo { get; set; }

            public string Address { get; set; }

            public string City { get; set; }

            public string BuyerIndustry { get; set; }

            public string BuyerType { get; set; }
        }

        #endregion
    }
}