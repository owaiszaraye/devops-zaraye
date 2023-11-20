using System.Globalization;
using System.Net;
using Zaraye.Core;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Seo;
using Zaraye.Infrastructure.Cache;
using Zaraye.Models.Catalog;
using Zaraye.Models.Media;
using Zaraye.Services.Catalog;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.Localization;
using Zaraye.Services.Media;
using Zaraye.Services.Orders;
using Zaraye.Services.Security;
using Zaraye.Services.Seo;
using Zaraye.Services.Stores;
using Zaraye.Services.Tax;

namespace Zaraye.Services.Common
{
    public class ProductModelService : IProductModelService
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly MediaSettings _mediaSettings;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IPermissionService _permissionService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly OrderSettings _orderSettings;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly SeoSettings _seoSettings;
        private readonly ICategoryService _categoryService;
        private readonly IVideoService _videoService;
        private readonly IStoreService _storeService;
        private readonly IDownloadService _downloadService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;


        #endregion

        #region Ctor
        public ProductModelService(ILocalizationService localizationService, IUrlRecordService urlRecordService, MediaSettings mediaSettings,
            IStaticCacheManager staticCacheManager, CatalogSettings catalogSettings, IWorkContext workContext, IWebHelper webHelper,
            IStoreContext storeContext, IPictureService pictureService, IProductService productService, IPermissionService permissionService,
            IPriceCalculationService priceCalculationService, OrderSettings orderSettings, ICurrencyService currencyService,
            IPriceFormatter priceFormatter, ISpecificationAttributeService specificationAttributeService, ICustomerService customerService,
            IProductAttributeService productAttributeService, IProductAttributeParser productAttributeParser, IShoppingCartService shoppingCartService,
            SeoSettings seoSettings, ICategoryService categoryService, ITaxService taxService, IVideoService videoService, IStoreService storeService,
            IDownloadService downloadService, IManufacturerService manufacturerService, IMeasureService measureService)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _mediaSettings = mediaSettings;
            _staticCacheManager = staticCacheManager;
            _catalogSettings = catalogSettings;
            _workContext = workContext;
            _webHelper = webHelper;
            _storeContext = storeContext;
            _pictureService = pictureService;
            _productService = productService;
            _permissionService = permissionService;
            _priceCalculationService = priceCalculationService;
            _orderSettings = orderSettings;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _specificationAttributeService = specificationAttributeService;
            _customerService = customerService;
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
            _seoSettings = seoSettings;
            _categoryService = categoryService;
            _taxService = taxService;
            _videoService = videoService;
            _storeService = storeService;
            _downloadService = downloadService;
            _manufacturerService = manufacturerService;
            _measureService = measureService;
        }
        #endregion

        #region Methods

        public virtual async Task<IEnumerable<Object>> PrepareProductModelsAsync(IEnumerable<Product> products,
        bool preparePriceModel = true, bool preparePictureModel = true,
        int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
        bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));
            var customer = await _workContext.GetCurrentCustomerAsync();

            List<object> productData = new List<object>();
            foreach (var product in products)
            {
                var pictureSize = _mediaSettings.ProductThumbPictureSize;

                var picture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault();
                var data = new
                {
                    Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId: customer.LanguageId),
                    ShorDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription, languageId: customer.LanguageId),
                    LongDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId: customer.LanguageId),
                    Picture = await _pictureService.GetPictureUrlAsync(picture == null ? 0 : picture.PictureId, targetSize: pictureSize),
                    SeName = await _urlRecordService.GetSeNameAsync(product),
                };
                productData.Add(data);
            }
            return productData;

        }

        public virtual async Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IEnumerable<Product> products,
         bool preparePriceModel = true, bool preparePictureModel = true,
         int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
         bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var models = new List<ProductOverviewModel>();
            foreach (var product in products)
            {

                var model = new ProductOverviewModel
                {
                    Id = product.Id,
                    Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                    FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
                    SeName = await _urlRecordService.GetSeNameAsync(product),
                    Sku = product.Sku,
                    ProductType = product.ProductType,
                    MarkAsNew = product.MarkAsNew &&
                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };

                ////price
                //if (preparePriceModel)
                //{
                //    model.ProductPrice = await PrepareProductOverviewPriceModelAsync(product, forceRedirectionAfterAddingToCart);
                //}

                ////picture
                //if (preparePictureModel)
                //{
                //    model.PictureModels = await PrepareProductOverviewPicturesModelAsync(product, productThumbPictureSize);
                //}

                ////specs
                //if (prepareSpecificationAttributes)
                //{
                //    model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
                //}

                ////reviews
                //model.ReviewOverviewModel = await PrepareProductReviewOverviewModelAsync(product);

                models.Add(model);
            }

            return models;
        }
        protected virtual async Task<ProductOverviewModel.ProductPriceModel> PrepareProductOverviewPriceModelAsync(Product product, bool forceRedirectionAfterAddingToCart = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var priceModel = new ProductOverviewModel.ProductPriceModel
            {
                ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
            };

            switch (product.ProductType)
            {
                case ProductType.GroupedProduct:
                    //grouped product
                    await PrepareGroupedProductOverviewPriceModelAsync(product, priceModel);

                    break;
                case ProductType.SimpleProduct:
                default:
                    //simple product
                    await PrepareSimpleProductOverviewPriceModelAsync(product, priceModel);

                    break;
            }

            return priceModel;
        }

        protected virtual async Task<IList<PictureModel>> PrepareProductOverviewPicturesModelAsync(Product product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeModelCacheDefaults.ProductOverviewPicturesModelKey,
                product, pictureSize, true, _catalogSettings.DisplayAllPicturesOnCatalogPages, await _workContext.GetWorkingLanguageAsync(),
                _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

            var cachedPictures = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                async Task<PictureModel> preparePictureModelAsync(Picture picture)
                {
                    //we use the Task.WhenAll method to control that both image thumbs was created in same time.
                    //without this method, sometimes there were situations when one of the pictures was not generated on time
                    //this section of code requires detailed analysis in the future
                    var picResultTasks = await Task.WhenAll(_pictureService.GetPictureUrlAsync(picture, pictureSize), _pictureService.GetPictureUrlAsync(picture));

                    var (imageUrl, _) = picResultTasks[0];
                    var (fullSizeImageUrl, _) = picResultTasks[1];

                    return new PictureModel
                    {
                        ImageUrl = imageUrl,
                        FullSizeImageUrl = fullSizeImageUrl,
                        //"title" attribute
                        Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                            ? picture.TitleAttribute
                            : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"),
                                productName),
                        //"alt" attribute
                        AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                            ? picture.AltAttribute
                            : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"),
                                productName)
                    };
                }

                //all pictures
                var pictures = (await _pictureService
                    .GetPicturesByProductIdAsync(product.Id, _catalogSettings.DisplayAllPicturesOnCatalogPages ? 0 : 1))
                    .DefaultIfEmpty(null);
                var pictureModels = await pictures
                    .SelectAwait(async picture => await preparePictureModelAsync(picture))
                    .ToListAsync();
                return pictureModels;
            });

            return cachedPictures;
        }
        protected virtual async Task PrepareGroupedProductOverviewPriceModelAsync(Product product, ProductOverviewModel.ProductPriceModel priceModel)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id,
                store.Id);

            //add to cart button (ignore "DisableBuyButton" property for grouped products)
            priceModel.DisableBuyButton =
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
            priceModel.DisableWishlistButton =
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //compare products
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
            if (!associatedProducts.Any())
                return;

            //we have at least one associated product
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                //find a minimum possible price
                decimal? minPossiblePrice = null;
                Product minPriceProduct = null;
                var customer = await _workContext.GetCurrentCustomerAsync();
                foreach (var associatedProduct in associatedProducts)
                {
                    var (_, tmpMinPossiblePrice, _, _) = await _priceCalculationService.GetFinalPriceAsync(associatedProduct, customer, store);

                    if (associatedProduct.HasTierPrices)
                    {
                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        tmpMinPossiblePrice = Math.Min(tmpMinPossiblePrice,
                            (await _priceCalculationService.GetFinalPriceAsync(associatedProduct, customer, store, quantity: int.MaxValue)).finalPrice);
                    }

                    if (minPossiblePrice.HasValue && tmpMinPossiblePrice >= minPossiblePrice.Value)
                        continue;
                    minPriceProduct = associatedProduct;
                    minPossiblePrice = tmpMinPossiblePrice;
                }

                if (minPriceProduct == null || minPriceProduct.CustomerEntersPrice)
                    return;

                if (minPriceProduct.CallForPrice &&
                    //also check whether the current user is impersonated
                    (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                     _workContext.OriginalCustomerIfImpersonated == null))
                {
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
                    priceModel.PriceValue = null;
                }
                else
                {
                    //calculate prices
                    var (finalPriceBase, _) = await _taxService.GetProductPriceAsync(minPriceProduct, minPossiblePrice.Value);
                    var finalPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceBase, await _workContext.GetWorkingCurrencyAsync());

                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), await _priceFormatter.FormatPriceAsync(finalPrice));
                    priceModel.PriceValue = finalPrice;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceBase);
                    priceModel.BasePricePAngVValue = finalPriceBase;
                }
            }
            else
            {
                //hide prices
                priceModel.OldPrice = null;
                priceModel.OldPriceValue = null;
                priceModel.Price = null;
                priceModel.PriceValue = null;
            }
        }

        public virtual async Task<ProductSpecificationModel> PrepareProductSpecificationModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductSpecificationModel();

            // Add non-grouped attributes first
            model.Groups.Add(new ProductSpecificationAttributeGroupModel
            {
                Attributes = await PrepareProductSpecificationAttributeModelAsync(product, null)
            });

            // Add grouped attributes
            var groups = await _specificationAttributeService.GetProductSpecificationAttributeGroupsAsync(product.Id);
            foreach (var group in groups)
            {
                model.Groups.Add(new ProductSpecificationAttributeGroupModel
                {
                    Id = group.Id,
                    Name = await _localizationService.GetLocalizedAsync(group, x => x.Name),
                    Attributes = await PrepareProductSpecificationAttributeModelAsync(product, group)
                });
            }

            return model;
        }
        protected virtual async Task<IList<ProductSpecificationAttributeModel>> PrepareProductSpecificationAttributeModelAsync(Product product, SpecificationAttributeGroup group)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(
                    product.Id, specificationAttributeGroupId: group?.Id, showOnProductPage: true);

            var result = new List<ProductSpecificationAttributeModel>();

            foreach (var psa in productSpecificationAttributes)
            {
                var option = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId);

                var model = result.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
                if (model == null)
                {
                    var attribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                    model = new ProductSpecificationAttributeModel
                    {
                        Id = attribute.Id,
                        Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name)
                    };
                    result.Add(model);
                }

                var value = new ProductSpecificationAttributeValueModel
                {
                    AttributeTypeId = psa.AttributeTypeId,
                    ColorSquaresRgb = option.ColorSquaresRgb,
                    ValueRaw = psa.AttributeType switch
                    {
                        SpecificationAttributeType.Option => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(option, x => x.Name)),
                        SpecificationAttributeType.CustomText => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue)),
                        SpecificationAttributeType.CustomHtmlText => await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue),
                        SpecificationAttributeType.Hyperlink => $"<a href='{psa.CustomValue}' target='_blank'>{psa.CustomValue}</a>",
                        _ => null
                    }
                };

                model.Values.Add(value);
            }

            return result;
        }
        protected virtual async Task<ProductReviewOverviewModel> PrepareProductReviewOverviewModelAsync(Product product)
        {
            ProductReviewOverviewModel productReview;
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            if (_catalogSettings.ShowProductReviewsPerStore)
            {
                var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeModelCacheDefaults.ProductReviewsModelKey, product, currentStore);

                productReview = await _staticCacheManager.GetAsync(cacheKey, async () =>
                {
                    var productReviews = await _productService.GetAllProductReviewsAsync(productId: product.Id, approved: true, storeId: currentStore.Id);

                    return new ProductReviewOverviewModel
                    {
                        RatingSum = productReviews.Sum(pr => pr.Rating),
                        TotalReviews = productReviews.Count
                    };
                });
            }
            else
            {
                productReview = new ProductReviewOverviewModel
                {
                    RatingSum = product.ApprovedRatingSum,
                    TotalReviews = product.ApprovedTotalReviews
                };
            }

            if (productReview != null)
            {
                productReview.ProductId = product.Id;
                productReview.AllowCustomerReviews = product.AllowCustomerReviews;
                productReview.CanAddNewReview = await _productService.CanAddReviewAsync(product.Id, _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);
            }

            return productReview;
        }
        protected virtual async Task PrepareSimpleProductOverviewPriceModelAsync(Product product, ProductOverviewModel.ProductPriceModel priceModel)
        {
            //add to cart button
            priceModel.DisableBuyButton = product.DisableBuyButton ||
                                          !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                                          !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button
            priceModel.DisableWishlistButton = product.DisableWishlistButton ||
                                               !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                                               !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);
            //compare products
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;

            //rental
            priceModel.IsRental = product.IsRental;

            //pre-order
            if (product.AvailableForPreOrder)
            {
                priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                                  product.PreOrderAvailabilityStartDateTimeUtc.Value >=
                                                  DateTime.UtcNow;
                priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
            }

            //prices
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                if (product.CustomerEntersPrice)
                    return;

                if (product.CallForPrice &&
                    //also check whether the current user is impersonated
                    (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                     _workContext.OriginalCustomerIfImpersonated == null))
                {
                    //call for price
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
                    priceModel.PriceValue = null;
                }
                else
                {
                    var store = await _storeContext.GetCurrentStoreAsync();
                    var customer = await _workContext.GetCurrentCustomerAsync();

                    //prices
                    var (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (decimal.Zero, decimal.Zero);
                    var hasMultiplePrices = false;
                    if (_catalogSettings.DisplayFromPrices)
                    {
                        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
                        var cacheKey = _staticCacheManager
                            .PrepareKeyForDefaultCache(ZarayeCatalogDefaults.ProductMultiplePriceCacheKey, product, customerRoleIds, store);
                        if (!_catalogSettings.CacheProductPrices || product.IsRental)
                            cacheKey.CacheTime = 0;

                        var cachedPrice = await _staticCacheManager.GetAsync(cacheKey, async () =>
                        {
                            var prices = new List<(decimal PriceWithoutDiscount, decimal PriceWithDiscount)>();

                            // price when there are no required attributes
                            var attributesMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                            if (!attributesMappings.Any(am => !am.IsNonCombinable() && am.IsRequired))
                            {
                                (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                    .GetFinalPriceAsync(product, customer, store);
                                prices.Add((priceWithoutDiscount, priceWithDiscount));
                            }

                            var allAttributesXml = await _productAttributeParser.GenerateAllCombinationsAsync(product, true);
                            foreach (var attributesXml in allAttributesXml)
                            {
                                var warnings = new List<string>();
                                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer,
                                    ShoppingCartType.ShoppingCart, product, 1, attributesXml, true, true, true));
                                if (warnings.Count != 0)
                                    continue;

                                //get price with additional charge
                                var additionalCharge = decimal.Zero;
                                var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
                                if (combination?.OverriddenPrice.HasValue ?? false)
                                    additionalCharge = combination.OverriddenPrice.Value;
                                else
                                {
                                    var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        additionalCharge += await _priceCalculationService.
                                            GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store);
                                    }
                                }

                                if (additionalCharge != decimal.Zero)
                                {
                                    (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                        .GetFinalPriceAsync(product, customer, store, additionalCharge);
                                    prices.Add((priceWithoutDiscount, priceWithDiscount));
                                }
                            }

                            if (prices.Distinct().Count() > 1)
                            {
                                (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = prices.OrderBy(p => p.PriceWithDiscount).First();
                                return new
                                {
                                    PriceWithoutDiscount = minPossiblePriceWithoutDiscount,
                                    PriceWithDiscount = minPossiblePriceWithDiscount
                                };
                            }

                            // show default price when required attributes available but no values added
                            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store);

                            //don't cache (return null) if there are no multiple prices
                            return null;
                        });

                        if (cachedPrice is not null)
                        {
                            hasMultiplePrices = true;
                            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (cachedPrice.PriceWithoutDiscount, cachedPrice.PriceWithDiscount);
                        }
                    }
                    else
                        (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store);

                    if (product.HasTierPrices)
                    {
                        var (tierPriceMinPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, quantity: int.MaxValue);

                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        minPossiblePriceWithoutDiscount = Math.Min(minPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithoutDiscount);
                        minPossiblePriceWithDiscount = Math.Min(minPossiblePriceWithDiscount, tierPriceMinPossiblePriceWithDiscount);
                    }

                    var (oldPriceBase, _) = await _taxService.GetProductPriceAsync(product, product.OldPrice);
                    var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithoutDiscount);
                    var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithDiscount);
                    var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                    var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
                    var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
                    var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);

                    var strikeThroughPrice = decimal.Zero;

                    if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                        strikeThroughPrice = oldPrice;

                    if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                        strikeThroughPrice = finalPriceWithoutDiscount;

                    if (strikeThroughPrice > decimal.Zero)
                    {
                        priceModel.OldPrice = await _priceFormatter.FormatPriceAsync(strikeThroughPrice);
                    }
                    else
                    {
                        priceModel.OldPrice = null;
                        priceModel.OldPriceValue = null;
                    }

                    //do we have tier prices configured?
                    var tierPrices = product.HasTierPrices
                        ? await _productService.GetTierPricesAsync(product, customer, store)
                        : new List<TierPrice>();

                    //When there is just one tier price (with  qty 1), there are no actual savings in the list.
                    var hasTierPrices = tierPrices.Any() && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);

                    var price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                    priceModel.Price = hasTierPrices || hasMultiplePrices
                        ? string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), price)
                        : price;
                    priceModel.PriceValue = finalPriceWithDiscount;

                    if (product.IsRental)
                    {
                        //rental product
                        priceModel.OldPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceModel.OldPrice);
                        priceModel.OldPriceValue = priceModel.OldPriceValue;
                        priceModel.Price = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceModel.Price);
                        priceModel.PriceValue = priceModel.PriceValue;
                    }

                    //property for German market
                    //we display tax/shipping info only with "shipping enabled" for this product
                    //we also ensure this it's not free shipping
                    priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes && product.IsShipEnabled && !product.IsFreeShipping;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscount);
                    priceModel.BasePricePAngVValue = finalPriceWithDiscount;
                }
            }
            else
            {
                //hide prices
                priceModel.OldPrice = null;
                priceModel.OldPriceValue = null;
                priceModel.Price = null;
                priceModel.PriceValue = null;
            }
        }
        public virtual async Task<ProductDetailsModel> PrepareProductDetailsModelAsync(Product product,
              ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //standard properties
            var model = new ProductDetailsModel
            {
                Id = product.Id,
                Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
                MetaKeywords = await _localizationService.GetLocalizedAsync(product, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(product, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(product, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(product),
                ProductType = product.ProductType,
                //ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage,
                Sku = product.Sku,
                //ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                //FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                //ManufacturerPartNumber = product.ManufacturerPartNumber,
                //ShowGtin = _catalogSettings.ShowGtin,
                //Gtin = product.Gtin,
                //ManageInventoryMethod = product.ManageInventoryMethod,
                //StockAvailability = await _productService.FormatStockMessageAsync(product, string.Empty),
                //HasSampleDownload = product.IsDownload && product.HasSampleDownload,
                //DisplayDiscontinuedMessage = !product.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedProducts,
                //AvailableEndDate = product.AvailableEndDateTimeUtc,
                //VisibleIndividually = product.VisibleIndividually,
                AllowAddingOnlyExistingAttributeCombinations = product.AllowAddingOnlyExistingAttributeCombinations
            };

            //automatically generate product description?
            if (_seoSettings.GenerateProductMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //store name
            var store = await _storeContext.GetCurrentStoreAsync();
            model.CurrentStoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name);

            //do not prepare this model for the associated products. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
            {
                model.Breadcrumb = await PrepareProductBreadcrumbModelAsync(product);
            }

            //pictures and videos
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            IList<PictureModel> allPictureModels;
            IList<VideoModel> allVideoModels;
            (model.DefaultPictureModel, allPictureModels, allVideoModels) = await PrepareProductDetailsPictureModelAsync(product, isAssociatedProduct);
            model.PictureModels = allPictureModels;
            model.VideoModels = allVideoModels;

            ////price
            //model.ProductPrice = await PrepareProductPriceModelAsync(product);

            //'Add to cart' model
            model.AddToCart = await PrepareProductAddToCartModelAsync(product, updatecartitem);

            model.ProductAttributes = await PrepareProductAttributeModelsAsync(product, updatecartitem);

            model.ProductManufacturers = await PrepareProductManufacturerModelsAsync(product);

            return model;
        }
        protected virtual async Task<ProductDetailsModel.ProductBreadcrumbModel> PrepareProductBreadcrumbModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
            {
                Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                ProductId = product.Id,
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product)
            };
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);
            if (!productCategories.Any())
                return breadcrumbModel;

            var category = await _categoryService.GetCategoryByIdAsync(productCategories[0].CategoryId);
            if (category == null)
                return breadcrumbModel;

            foreach (var catBr in await _categoryService.GetCategoryBreadCrumbAsync(category))
            {
                breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                {
                    Id = catBr.Id,
                    Name = await _localizationService.GetLocalizedAsync(catBr, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(catBr),
                    IncludeInTopMenu = catBr.IncludeInTopMenu
                });
            }

            return breadcrumbModel;
        }
        protected virtual async Task<(PictureModel pictureModel, IList<PictureModel> allPictureModels, IList<VideoModel> allVideoModels)> PrepareProductDetailsPictureModelAsync(Product product, bool isAssociatedProduct)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //default picture size
            var defaultPictureSize = isAssociatedProduct ?
                _mediaSettings.AssociatedProductPictureSize :
                _mediaSettings.ProductDetailsPictureSize;

            //prepare picture models
            var productPicturesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeModelCacheDefaults.ProductDetailsPicturesModelKey
                , product, defaultPictureSize, isAssociatedProduct,
                await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
            var cachedPictures = await _staticCacheManager.GetAsync(productPicturesCacheKey, async () =>
            {
                var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);

                var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
                var defaultPicture = pictures.FirstOrDefault();

                string fullSizeImageUrl, imageUrl, thumbImageUrl;
                (imageUrl, defaultPicture) = await _pictureService.GetPictureUrlAsync(defaultPicture, defaultPictureSize, !isAssociatedProduct);
                (fullSizeImageUrl, defaultPicture) = await _pictureService.GetPictureUrlAsync(defaultPicture, 0, !isAssociatedProduct);

                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = imageUrl,
                    FullSizeImageUrl = fullSizeImageUrl
                };
                //"title" attribute
                defaultPictureModel.Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName);
                //"alt" attribute
                defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName);

                //all pictures
                var pictureModels = new List<PictureModel>();
                for (var i = 0; i < pictures.Count; i++)
                {
                    var picture = pictures[i];

                    (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, defaultPictureSize, !isAssociatedProduct);
                    (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (thumbImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage);

                    var pictureModel = new PictureModel
                    {
                        ImageUrl = imageUrl,
                        ThumbImageUrl = thumbImageUrl,
                        FullSizeImageUrl = fullSizeImageUrl,
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                        picture.TitleAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                        picture.AltAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName);

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });

            var allPictureModels = cachedPictures.PictureModels;

            //all videos
            var allvideoModels = new List<VideoModel>();
            var videos = await _videoService.GetVideosByProductIdAsync(product.Id);
            foreach (var video in videos)
            {
                var videoModel = new VideoModel
                {
                    VideoUrl = video.VideoUrl,
                    Allow = _mediaSettings.VideoIframeAllow,
                    Width = _mediaSettings.VideoIframeWidth,
                    Height = _mediaSettings.VideoIframeHeight
                };

                allvideoModels.Add(videoModel);
            }
            return (cachedPictures.DefaultPictureModel, allPictureModels, allvideoModels);
        }
        protected virtual async Task<ProductDetailsModel.AddToCartModel> PrepareProductAddToCartModelAsync(Product product, ShoppingCartItem updatecartitem)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductDetailsModel.AddToCartModel
            {
                ProductId = product.Id
            };
            model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity;

            if (product.OrderMinimumQuantity > 1)
            {
                model.MinimumQuantityNotification = string.Format(await _localizationService.GetResourceAsync("Products.MinimumQuantityNotification"), product.OrderMinimumQuantity);
            }

            return model;
        }
        protected virtual async Task<IList<ProductDetailsModel.ProductAttributeModel>> PrepareProductAttributeModelsAsync(Product product, ShoppingCartItem updatecartitem)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new List<ProductDetailsModel.ProductAttributeModel>();
            var store = updatecartitem != null ? await _storeService.GetStoreByIdAsync(updatecartitem.StoreId) : await _storeContext.GetCurrentStoreAsync();

            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            foreach (var attribute in productAttributeMapping)
            {
                var productAttrubute = await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);

                var attributeModel = new ProductDetailsModel.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = await _localizationService.GetLocalizedAsync(productAttrubute, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(productAttrubute, x => x.Description),
                    TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue),
                    HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
                };
                if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
                        {
                            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                            var customer = updatecartitem?.CustomerId is null ? currentCustomer : await _customerService.GetCustomerByIdAsync(updatecartitem.CustomerId);

                            var attributeValuePriceAdjustment = await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store, quantity: updatecartitem?.Quantity ?? 1);
                            var (priceAdjustmentBase, _) = await _taxService.GetProductPriceAsync(product, attributeValuePriceAdjustment);
                            var priceAdjustment = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await _workContext.GetWorkingCurrencyAsync());

                            if (attributeValue.PriceAdjustmentUsePercentage)
                            {
                                var priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                                if (attributeValue.PriceAdjustment > decimal.Zero)
                                    valueModel.PriceAdjustment = "+";
                                valueModel.PriceAdjustment += priceAdjustmentStr + "%";
                            }
                            else
                            {
                                if (priceAdjustmentBase > decimal.Zero)
                                    valueModel.PriceAdjustment = "+" + await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false);
                                else if (priceAdjustmentBase < decimal.Zero)
                                    valueModel.PriceAdjustment = "-" + await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false);
                            }

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        //"image square" picture (with with "image squares" attribute type only)
                        if (attributeValue.ImageSquaresPictureId > 0)
                        {
                            var productAttributeImageSquarePictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeModelCacheDefaults.ProductAttributeImageSquarePictureModelKey
                                , attributeValue.ImageSquaresPictureId,
                                    _webHelper.IsCurrentConnectionSecured(),
                                    await _storeContext.GetCurrentStoreAsync());
                            valueModel.ImageSquaresPictureModel = await _staticCacheManager.GetAsync(productAttributeImageSquarePictureCacheKey, async () =>
                            {
                                var imageSquaresPicture = await _pictureService.GetPictureByIdAsync(attributeValue.ImageSquaresPictureId);
                                string fullSizeImageUrl, imageUrl;
                                (imageUrl, imageSquaresPicture) = await _pictureService.GetPictureUrlAsync(imageSquaresPicture, _mediaSettings.ImageSquarePictureSize);
                                (fullSizeImageUrl, imageSquaresPicture) = await _pictureService.GetPictureUrlAsync(imageSquaresPicture);

                                if (imageSquaresPicture != null)
                                {
                                    return new PictureModel
                                    {
                                        FullSizeImageUrl = fullSizeImageUrl,
                                        ImageUrl = imageUrl
                                    };
                                }

                                return new PictureModel();
                            });
                        }

                        //picture of a product attribute value
                        valueModel.PictureId = attributeValue.PictureId;
                    }
                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = await _productAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml);
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
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    foreach (var attributeValue in (await _productAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml))
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
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }

                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
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
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var downloadGuidStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
                                    _ = Guid.TryParse(downloadGuidStr, out var downloadGuid);
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

                model.Add(attributeModel);
            }

            return model;
        }
        protected virtual async Task<IList<ManufacturerBriefInfoModel>> PrepareProductManufacturerModelsAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = await (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id))
                .SelectAwait(async pm =>
                {
                    var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
                    var modelMan = new ManufacturerBriefInfoModel
                    {
                        Id = manufacturer.Id,
                        Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(manufacturer)
                    };

                    return modelMan;
                }).ToListAsync();

            return model;
        }

        #endregion
    }
}
