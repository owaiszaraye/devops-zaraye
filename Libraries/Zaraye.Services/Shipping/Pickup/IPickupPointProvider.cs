using System.Collections.Generic;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Orders;
using Zaraye.Services.Shipping.Tracking;

namespace Zaraye.Services.Shipping.Pickup
{
    /// <summary>
    /// Represents an interface of pickup point provider
    /// </summary>
    public partial interface IPickupPointProvider 
    {
        #region Methods

        /// <summary>
        /// Get pickup points for the address
        /// </summary>
        /// <param name="cart">Shopping Cart</param>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the represents a response of getting pickup points
        /// </returns>
        Task<GetPickupPointsResponse> GetPickupPointsAsync(IList<ShoppingCartItem> cart, Address address);

        /// <summary>
        /// Get associated shipment tracker
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment tracker
        /// </returns>
        Task<IShipmentTracker> GetShipmentTrackerAsync();

        #endregion
    }
}