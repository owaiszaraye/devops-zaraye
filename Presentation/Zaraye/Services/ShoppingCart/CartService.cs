using Zaraye.Core.Domain.Media;
using Zaraye.Core;
using Zaraye.Services.Catalog;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.Localization;
using Zaraye.Services.Media;
using Zaraye.Services.Seo;
using Zaraye.Core.Domain.Orders;
using Zaraye.Models.Api.V4.ShoppingCart;
using Zaraye.Services.Orders;
using Zaraye.Services.Utility;
using Zaraye.Core.Caching;

namespace Zaraye.Services.ShoppingCart
{
    public class CartService : ICartService
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly IStoreContext _storeContext;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ICategoryService _categoryService;
        private readonly IMeasureService _measureService;
        private readonly IUtilityService _utilityService;
        private readonly IStaticCacheManager _staticCacheManager;
        public CartService(
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            ICustomerService customerService, IProductService productService,
            IUrlRecordService urlRecordService, IPictureService pictureService,
             MediaSettings mediaSettings, IStoreContext storeContext
            , IProductAttributeService productAttributeService, IDownloadService downloadService,
             IProductAttributeParser productAttributeParser, ILocalizationService localizationService,
             IManufacturerService manufacturerService, ICategoryService categoryService, IMeasureService measureService,
             IUtilityService utilityService, IStaticCacheManager staticCacheManager
             )
        {
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _customerService = customerService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
            _storeContext = storeContext;
            _productAttributeService = productAttributeService;
            _downloadService = downloadService;
            _productAttributeParser = productAttributeParser;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _categoryService = categoryService;
            _measureService = measureService;
            _utilityService = utilityService;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<object> GetAllCartItems()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var cartitems = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart);
            var pictureSize = _mediaSettings.ProductThumbPictureSize;
            var productList = (await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart))
                .Select(async item =>
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    var brand = await _manufacturerService.GetManufacturerByIdAsync(item.BrandId);
                    var productSeName = await _urlRecordService.GetSeNameAsync(product, 0);
                    var picture = (await _productService.GetProductPicturesByProductIdAsync(item.ProductId)).FirstOrDefault();
                    var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId);
                    if (productCategories is null)
                        throw new ApplicationException("product Category not found");

                    var categories = await _categoryService.GetCategoriesByIdsAsync(productCategories.Select(x => x.CategoryId).ToArray());

                    var unit = await _measureService.GetMeasureWeightByIdAsync(product.UnitId);
                    return new
                    {
                        Name = product.Name,
                        SeName = productSeName,
                        Id = product.Id,
                        BrandId = item.BrandId,
                        BrandName = brand.Name,
                        CartItemId = item.Id,
                        Picture = await _pictureService.GetPictureUrlAsync(picture == null ? 0 : picture.PictureId, targetSize: pictureSize),
                        Quantity = item.Quantity,
                        ProductAttributes = await _utilityService.ParseAttributeXml(item.AttributesXml),
                        CategoryName = string.Join(", ", categories.Select(x => x.Name)),
                        UnitName = unit != null ? unit.Name : "",
                    };
                }).Select(t => t.Result).ToList();
            return productList;
        }
        public async Task<string> AddCartItem(CartItemRequestModel cartItemRequestModel)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var product = await _productService.GetProductByIdAsync(cartItemRequestModel.ProductId);
            if (product == null)
                throw new ApplicationException("Product not found");
            var store = await _storeContext.GetCurrentStoreAsync();
            var warnings = new List<string>();
            var attributesXml = await _utilityService.ConvertToXmlAsync(cartItemRequestModel.AttributesData, product.Id);
            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, product, 1, attributesXml));
            if (warnings.Any())
                throw new Exception("[" + string.Join(", ", warnings) + "]");
            var cartItems = await _shoppingCartService.AddToCartAsync(customer: customer, product: product, shoppingCartType: ShoppingCartType.ShoppingCart, storeId: store.Id, attributesXml: attributesXml, quantity: cartItemRequestModel.Quantity, brandId: cartItemRequestModel.BrandId);
            return "Items are successfully added in cart";
          
        }
        public async Task<string> UpdateCartItem(int cartItemId, decimal quantity)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var updateCartItem = await _shoppingCartService.UpdateShoppingCartItemAsync(customer: customer, shoppingCartItemId: cartItemId, quantity: quantity);
            return "Cart item updated successfully";
            
        }
        public async Task<string> DeleteCartItem(int cartItemId)
        {
            await _shoppingCartService.DeleteShoppingCartItemAsync(cartItemId);
            return "Cart item deleted successfully";
        }
    }
}
