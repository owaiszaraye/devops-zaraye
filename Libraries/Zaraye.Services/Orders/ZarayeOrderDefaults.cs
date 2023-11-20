using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Orders;

namespace Zaraye.Services.Orders
{
    /// <summary>
    /// Represents default values related to orders services
    /// </summary>
    public static partial class ZarayeOrderDefaults
    {
        #region Caching defaults

        #region ShoppingCart

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer ID
        /// {1} : shopping cart type
        /// {2} : store ID
        /// {3} : product ID
        /// {4} : created from date
        /// {5} : created to date
        /// </remarks>
        public static CacheKey ShoppingCartItemsAllCacheKey => new("Zaraye.shoppingcartitem.all.{0}-{1}-{2}-{3}-{4}-{5}", ShoppingCartItemsByCustomerPrefix, ZarayeEntityCacheDefaults<ShoppingCartItem>.AllPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static string ShoppingCartItemsByCustomerPrefix => "Zaraye.shoppingcartitem.all.{0}";


        #endregion

        #endregion
    }
}