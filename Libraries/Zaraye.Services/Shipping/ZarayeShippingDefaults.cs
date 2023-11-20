using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Shipping;

namespace Zaraye.Services.Shipping
{
    /// <summary>
    /// Represents default values related to shipping services
    /// </summary>
    public static partial class ZarayeShippingDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country identifier
        /// </remarks>
        public static CacheKey ShippingMethodsAllCacheKey => new("Zaraye.shippingmethod.all.{0}", ZarayeEntityCacheDefaults<ShippingMethod>.AllPrefix);

        #endregion
    }
}