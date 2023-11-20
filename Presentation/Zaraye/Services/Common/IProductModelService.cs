using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Orders;
using Zaraye.Models.Catalog;

namespace Zaraye.Services.Common
{
    public interface IProductModelService
    {
        Task<IEnumerable<Object>> PrepareProductModelsAsync(IEnumerable<Product> products,
         bool preparePriceModel = true, bool preparePictureModel = true,
         int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
         bool forceRedirectionAfterAddingToCart = false);
        Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IEnumerable<Product> products,
         bool preparePriceModel = true, bool preparePictureModel = true,
         int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
         bool forceRedirectionAfterAddingToCart = false);
        Task<ProductDetailsModel> PrepareProductDetailsModelAsync(Product product,
              ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false);
    }
}
