using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Payments;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Infrastructure;
using Zaraye.Data;
using Zaraye.Services.Orders;
using Zaraye.Services.Shipping.Pickup;
using Zaraye.Services.Shipping.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Shipping
{
    /// <summary>
    /// Shipment service
    /// </summary>
    public partial class ShipmentService : IShipmentService
    {
        #region Fields

        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<ShipmentReturn> _shipmentReturnRepository;
        private readonly IRepository<ShipmentItem> _siRepository;
        private readonly IRepository<ShipmentReturnReason> _shipmentReturnReasonRepository;
        private readonly IRepository<DeliveryCostReason> _deliveryCostReasonRepository;
        private readonly IRepository<DeliveryTimeReason> _deliveryTimeReasonRepository;
        private readonly IRepository<ShipmentDropOffHistory> _shipmentDropOffHistoryRepository;
        private readonly IRepository<ShipmentPaymentMapping> _shipmentPaymentMappingRepository;

        #endregion

        #region Ctor

        public ShipmentService(
            IRepository<Address> addressRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
            IRepository<Shipment> shipmentRepository,
            IRepository<ShipmentReturn> shipmentReturnRepository,
            IRepository<ShipmentItem> siRepository,
            IRepository<ShipmentReturnReason> shipmentReturnReasonRepository,
            IRepository<DeliveryCostReason> deliveryCostReasonRepository,
            IRepository<DeliveryTimeReason> deliveryTimeReasonRepository,
            IRepository<ShipmentDropOffHistory> shipmentDropOffHistoryRepository,
            IRepository<ShipmentPaymentMapping> shipmentPaymentMappingRepository)
        {
            _addressRepository = addressRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _shipmentRepository = shipmentRepository;
            _shipmentReturnRepository = shipmentReturnRepository;
            _siRepository = siRepository;
            _shipmentReturnReasonRepository = shipmentReturnReasonRepository;
            _deliveryCostReasonRepository = deliveryCostReasonRepository;
            _deliveryTimeReasonRepository = deliveryTimeReasonRepository;
            _shipmentDropOffHistoryRepository = shipmentDropOffHistoryRepository;
            _shipmentPaymentMappingRepository = shipmentPaymentMappingRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteShipmentAsync(Shipment shipment)
        {
            await _shipmentRepository.DeleteAsync(shipment);
        }

        /// <summary>
        /// Search shipments
        /// </summary>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier, only shipments with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="shippingCountryId">Shipping country identifier; 0 to load all records</param>
        /// <param name="shippingStateId">Shipping state identifier; 0 to load all records</param>
        /// <param name="shippingCounty">Shipping county; null to load all records</param>
        /// <param name="shippingCity">Shipping city; null to load all records</param>
        /// <param name="trackingNumber">Search by tracking number</param>
        /// <param name="loadNotShipped">A value indicating whether we should load only not shipped shipments</param>
        /// <param name="loadNotReadyForPickup">A value indicating whether we should load only not ready for pickup shipments</param>
        /// <param name="loadNotDelivered">A value indicating whether we should load only not delivered shipments</param>
        /// <param name="orderId">Order identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipments
        /// </returns>
        public virtual async Task<IPagedList<Shipment>> GetAllShipmentsAsync(int warehouseId = 0,
            int shipmentTypeId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCounty = null,
            string shippingCity = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            bool loadNotReadyForPickup = false,
            bool loadNotDelivered = false,
            int orderId = 0,
            int shipmentId = 0, int transporterId = 0,
             List<int> dsIds = null, List<int> psIds = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            bool loadOnlyDelivered = false,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var shipments = await _shipmentRepository.GetAllPagedAsync(query =>
            {
                if (shipmentTypeId > 0)
                    query = query.Where(o => o.ShipmentTypeId == shipmentTypeId);

                if (orderId > 0)
                    query = query.Where(o => o.OrderId == orderId);

                if (shipmentId > 0)
                    query = query.Where(o => o.Id == shipmentId);

                if (transporterId > 0)
                    query = query.Where(o => o.TransporterId == transporterId);

                if (!string.IsNullOrEmpty(trackingNumber))
                    query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));

                if (dsIds != null && dsIds.Any())
                {
                    if (dsIds.Contains(-1))
                        dsIds.Remove(-1);

                    if (dsIds.Any())
                        query = query.Where(o => dsIds.Contains(o.DeliveryStatusId));
                }

                if (psIds != null && psIds.Any())
                {
                    if (psIds.Contains(-1))
                        psIds.Remove(-1);

                    if (psIds.Any())
                        query = query.Where(o => psIds.Contains(o.PaymentStatusId));
                }

                if (shippingCountryId > 0)
                    query = from s in query
                            join o in _orderRepository.Table on s.OrderId equals o.Id
                            where _addressRepository.Table.Any(a =>
                                a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                                a.CountryId == shippingCountryId)
                            select s;

                if (shippingStateId > 0)
                    query = from s in query
                            join o in _orderRepository.Table on s.OrderId equals o.Id
                            where _addressRepository.Table.Any(a =>
                                a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                                a.StateProvinceId == shippingStateId)
                            select s;

                if (!string.IsNullOrWhiteSpace(shippingCounty))
                    query = from s in query
                            join o in _orderRepository.Table on s.OrderId equals o.Id
                            where _addressRepository.Table.Any(a =>
                                a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                                a.County.Contains(shippingCounty))
                            select s;

                if (!string.IsNullOrWhiteSpace(shippingCity))
                    query = from s in query
                            join o in _orderRepository.Table on s.OrderId equals o.Id
                            where _addressRepository.Table.Any(a =>
                                a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                                a.City.Contains(shippingCity))
                            select s;

                if (loadNotShipped)
                    query = from s in query
                            join o in _orderRepository.Table on s.OrderId equals o.Id
                            where !s.ShippedDateUtc.HasValue && !o.PickupInStore
                            select s;

                if (loadNotReadyForPickup)
                    query = from s in query
                            join o in _orderRepository.Table on s.OrderId equals o.Id
                            where !s.ReadyForPickupDateUtc.HasValue && o.PickupInStore
                            select s;

                if (loadNotDelivered)
                    query = query.Where(s => !s.DeliveryDateUtc.HasValue);

                if (loadOnlyDelivered)
                    query = query.Where(s => s.DeliveryDateUtc.HasValue);

                if (createdFromUtc.HasValue)
                    query = query.Where(s => createdFromUtc.Value <= s.CreatedOnUtc);

                if (createdToUtc.HasValue)
                    query = query.Where(s => createdToUtc.Value >= s.CreatedOnUtc);

                query = from s in query
                        join o in _orderRepository.Table on s.OrderId equals o.Id
                        where !o.Deleted
                        select s;

                query = query.Distinct();

                if (warehouseId > 0)
                {
                    query = from s in query
                            join si in _siRepository.Table on s.Id equals si.ShipmentId
                            where si.WarehouseId == warehouseId
                            select s;

                    query = query.Distinct();
                }

                query = query.OrderByDescending(s => s.CreatedOnUtc);

                return query;
            }, pageIndex, pageSize);

            return shipments;
        }

        /// <summary>
        /// Get shipment by identifiers
        /// </summary>
        /// <param name="shipmentIds">Shipment identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipments
        /// </returns>
        public virtual async Task<IList<Shipment>> GetShipmentsByIdsAsync(int[] shipmentIds)
        {
            return await _shipmentRepository.GetByIdsAsync(shipmentIds);
        }

        /// <summary>
        /// Gets a shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment
        /// </returns>
        public virtual async Task<Shipment> GetShipmentByIdAsync(int shipmentId)
        {
            return await _shipmentRepository.GetByIdAsync(shipmentId);
        }

        /// <summary>
        /// Gets a list of order shipments
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="shipped">A value indicating whether to count only shipped or not shipped shipments; pass null to ignore</param>
        /// <param name="readyForPickup">A value indicating whether to load only ready for pickup shipments; pass null to ignore</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<Shipment>> GetShipmentsByOrderIdAsync(int orderId, bool loadOnlyDelivered = false, bool loadOnlyShipped = false, bool? shipped = null, bool? readyForPickup = null, int vendorId = 0)
        {
            if (orderId == 0)
                return new List<Shipment>();

            var shipments = from s in _shipmentRepository.Table
                            select s;

            if (shipped.HasValue)
                shipments = shipments.Where(s => s.ShippedDateUtc.HasValue == shipped);

            if (loadOnlyShipped)
                shipments = shipments.Where(s => s.ShippedDateUtc.HasValue);

            if (loadOnlyDelivered)
                shipments = shipments.Where(s => s.ShippedDateUtc.HasValue && s.DeliveryDateUtc.HasValue);

            if (readyForPickup.HasValue)
                shipments = shipments.Where(s => s.ReadyForPickupDateUtc.HasValue == readyForPickup);

            return await shipments.Where(shipment => shipment.OrderId == orderId).ToListAsync();
        }

        /// <summary>
        /// Inserts a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertShipmentAsync(Shipment shipment)
        {
            var _orderService = EngineContext.Current.Resolve<IOrderService>();
            shipment.PaymentStatusId = (int)PaymentStatus.Pending;

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (order is not null)
            {
                if (order.OrderType == OrderType.PurchaseOrder)
                    shipment.ShipmentTypeId = (int)ShipmentType.PurchaseOrder;
                if (order.OrderType == OrderType.SaleOrder)
                    shipment.ShipmentTypeId = (int)ShipmentType.SaleOrder;
                if (order.OrderType == OrderType.ReturnPurchaseOrder)
                    shipment.ShipmentTypeId = (int)ShipmentType.ReturnPurchaseOrder;
                if (order.OrderType == OrderType.ReturnSaleOrder)
                    shipment.ShipmentTypeId = (int)ShipmentType.ReturnSaleOrder;
            }

            await _shipmentRepository.InsertAsync(shipment);
        }

        /// <summary>
        /// Updates the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateShipmentAsync(Shipment shipment)
        {
            await _shipmentRepository.UpdateAsync(shipment);
        }

        /// <summary>
        /// Gets a shipment items of shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment items
        /// </returns>
        public virtual async Task<IList<ShipmentItem>> GetShipmentItemsByShipmentIdAsync(int shipmentId)
        {
            if (shipmentId == 0)
                return null;

            return await _siRepository.Table.Where(si => si.ShipmentId == shipmentId).ToListAsync();
        }

        /// <summary>
        /// Inserts a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertShipmentItemAsync(ShipmentItem shipmentItem)
        {
            await _siRepository.InsertAsync(shipmentItem);
        }

        /// <summary>
        /// Deletes a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment Item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteShipmentItemAsync(ShipmentItem shipmentItem)
        {
            await _siRepository.DeleteAsync(shipmentItem);
        }

        /// <summary>
        /// Updates a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateShipmentItemAsync(ShipmentItem shipmentItem)
        {
            await _siRepository.UpdateAsync(shipmentItem);
        }

        /// <summary>
        /// Gets a shipment item
        /// </summary>
        /// <param name="shipmentItemId">Shipment item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment item
        /// </returns>
        public virtual async Task<ShipmentItem> GetShipmentItemByIdAsync(int shipmentItemId)
        {
            return await _siRepository.GetByIdAsync(shipmentItemId);
        }

        /// <summary>
        /// Get quantity in shipments. For example, get planned quantity to be shipped
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="ignoreShipped">Ignore already shipped shipments</param>
        /// <param name="ignoreDelivered">Ignore already delivered shipments</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the quantity
        /// </returns>
        public virtual async Task<int> GetQuantityInShipmentsAsync(Product product, int warehouseId,
            bool ignoreShipped, bool ignoreDelivered)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
                return 0;
            if (!product.UseMultipleWarehouses)
                return 0;

            const int cancelledOrderStatusId = (int)OrderStatus.Cancelled;

            var query = _siRepository.Table;

            query = from si in query
                    join s in _shipmentRepository.Table on si.ShipmentId equals s.Id
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where !o.Deleted && o.OrderStatusId != cancelledOrderStatusId
                    select si;

            query = query.Distinct();

            if (warehouseId > 0)
                query = query.Where(si => si.WarehouseId == warehouseId);
            if (ignoreShipped)
            {
                query = from si in query
                        join s in _shipmentRepository.Table on si.ShipmentId equals s.Id
                        where !s.ShippedDateUtc.HasValue
                        select si;
            }

            if (ignoreDelivered)
            {
                query = from si in query
                        join s in _shipmentRepository.Table on si.ShipmentId equals s.Id
                        where !s.DeliveryDateUtc.HasValue
                        select si;
            }

            var queryProductOrderItems = from orderItem in _orderItemRepository.Table
                                         where orderItem.ProductId == product.Id
                                         select orderItem.Id;
            query = from si in query
                    where queryProductOrderItems.Any(orderItemId => orderItemId == si.OrderItemId)
                    select si;

            //some null validation
            var result = Convert.ToInt32(await query.SumAsync(si => (int?)si.Quantity));
            return result;
        }

        /// <summary>
        /// Get the tracker of the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment tracker
        /// </returns>
        public virtual async Task<IShipmentTracker> GetShipmentTrackerAsync(Shipment shipment)
        {
            var order = await _orderRepository.GetByIdAsync(shipment.OrderId, cache => default);
            IShipmentTracker shipmentTracker = null;

            if (order.PickupInStore)
            {
                //var pickupPointProvider = await _pickupPluginManager
                //    .LoadPluginBySystemNameAsync(order.ShippingRateComputationMethodSystemName);

                //if (pickupPointProvider != null)
                //    shipmentTracker = await pickupPointProvider.GetShipmentTrackerAsync();
            }
            else
            {
                //var shippingRateComputationMethod = await _shippingPluginManager
                //    .LoadPluginBySystemNameAsync(order.ShippingRateComputationMethodSystemName);

                //if (shippingRateComputationMethod != null)
                //    shipmentTracker = await shippingRateComputationMethod.GetShipmentTrackerAsync();
            }

            return shipmentTracker;
        }

        public virtual async Task<decimal> GetOrderPaidAmount(Order order)
        {
            var orderShipments = await GetShipmentsByOrderIdAsync(orderId: order.Id);
            decimal totalpaidAmount = 0;

            foreach (var shipment in orderShipments)
            {
                totalpaidAmount += (from spm in _shipmentPaymentMappingRepository.Table
                                    where spm.ShipmentId == shipment.Id
                                    select spm).Sum(x => x.Amount);
            }
            return totalpaidAmount;
        }

        public virtual async Task<Shipment> GetShipmentByDeliveryRequestId(int deliveryRequestId)
        {
            return await (from spm in _shipmentRepository.Table
                          where spm.DeliveryRequestId == deliveryRequestId
                          select spm).FirstOrDefaultAsync();
        }

        public virtual async Task<decimal> GetOrderReturnAmount(Order order)
        {
            var returnOrders = from o in _orderRepository.Table
                               where o.ParentOrderId == order.Id
                               select o;

            decimal totalreturnAmount = 0;
            foreach (var returnOrder in returnOrders)
            {
                var orderShipments = (await GetShipmentsByOrderIdAsync(orderId: returnOrder.Id)).Where(x => x.ShipmentReturnTypeId == (int)ShipmentReturnType.Returned);
                foreach (var shipment in orderShipments)
                    totalreturnAmount += shipment.DeliveredAmount;
            }

            return totalreturnAmount;
        }

        #endregion

        #region Shipment Return Reason

        public virtual async Task DeleteShipmentReturnReasonAsync(ShipmentReturnReason shipmentReturnReason)
        {
            shipmentReturnReason.Deleted = true;
            await _shipmentReturnReasonRepository.UpdateAsync(shipmentReturnReason);
        }

        public virtual async Task<ShipmentReturnReason> GetShipmentReturnReasonByIdAsync(int shipmentReturnReasonId)
        {
            return await _shipmentReturnReasonRepository.GetByIdAsync(shipmentReturnReasonId);
        }

        public virtual async Task InsertShipmentReturnReasonAsync(ShipmentReturnReason shipmentReturnReason)
        {
            await _shipmentReturnReasonRepository.InsertAsync(shipmentReturnReason);
        }

        public virtual async Task UpdateShipmentReturnReasonAsync(ShipmentReturnReason shipmentReturnReason)
        {
            await _shipmentReturnReasonRepository.UpdateAsync(shipmentReturnReason);
        }

        public virtual async Task<IPagedList<ShipmentReturnReason>> GetAllShipmentReturnReasonAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from dc in _shipmentReturnReasonRepository.Table
                        where !dc.Deleted
                        select dc;

            if (!showHidden)
                query = query.Where(c => c.Published);

            //paging
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }
        #endregion

        #region Delivery Cost Reason

        public virtual async Task DeleteDeliveryCostReasonAsync(DeliveryCostReason deliveryCostReason)
        {
            deliveryCostReason.Deleted = true;
            await _deliveryCostReasonRepository.UpdateAsync(deliveryCostReason);
        }

        public virtual async Task<DeliveryCostReason> GetDeliveryCostReasonByIdAsync(int deliveryCostReasonId)
        {
            return await _deliveryCostReasonRepository.GetByIdAsync(deliveryCostReasonId);
        }

        public virtual async Task InsertDeliveryCostReasonAsync(DeliveryCostReason deliveryCostReason)
        {
            await _deliveryCostReasonRepository.InsertAsync(deliveryCostReason);
        }

        public virtual async Task UpdateDeliveryCostReasonAsync(DeliveryCostReason deliveryCostReason)
        {
            await _deliveryCostReasonRepository.UpdateAsync(deliveryCostReason);
        }

        public virtual async Task<IPagedList<DeliveryCostReason>> GetAllDeliveryCostReasonAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from dc in _deliveryCostReasonRepository.Table
                        where !dc.Deleted
                        select dc;

            if (!showHidden)
                query = query.Where(c => c.Published);

            var deliveryCostReason = await query.ToListAsync();

            //paging
            return new PagedList<DeliveryCostReason>(deliveryCostReason, pageIndex, pageSize);
        }


        #endregion

        #region Delivery Time Reason

        public virtual async Task DeleteDeliveryTimeReasonAsync(DeliveryTimeReason deliveryTimeReason)
        {
            deliveryTimeReason.Deleted = true;
            await _deliveryTimeReasonRepository.UpdateAsync(deliveryTimeReason);
        }

        public virtual async Task<DeliveryTimeReason> GetDeliveryTimeReasonByIdAsync(int deliveryTimeReasonId)
        {
            return await _deliveryTimeReasonRepository.GetByIdAsync(deliveryTimeReasonId);
        }

        public virtual async Task InsertDeliveryTimeReasonAsync(DeliveryTimeReason deliveryTimeReason)
        {
            await _deliveryTimeReasonRepository.InsertAsync(deliveryTimeReason);
        }

        public virtual async Task UpdateDeliveryTimeReasonAsync(DeliveryTimeReason deliveryTimeReason)
        {
            await _deliveryTimeReasonRepository.UpdateAsync(deliveryTimeReason);
        }

        public virtual async Task<IPagedList<DeliveryTimeReason>> GetAllDeliveryTimeReasonAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from dc in _deliveryTimeReasonRepository.Table
                        where !dc.Deleted
                        select dc;

            if (!showHidden)
                query = query.Where(c => c.Published);

            //paging
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }
        #endregion

        #region Shipment Dropoff History

        public virtual async Task InsertShipmentDropOffHistoryAsync(ShipmentDropOffHistory shipmentDropOffHistory)
        {
            await _shipmentDropOffHistoryRepository.InsertAsync(shipmentDropOffHistory);
        }

        public virtual async Task UpdateShipmentDropOffHistoryAsync(ShipmentDropOffHistory shipmentDropOffHistory)
        {
            await _shipmentDropOffHistoryRepository.UpdateAsync(shipmentDropOffHistory);
        }

        public virtual async Task DeleteShipmentDropOffHistoryAsync(ShipmentDropOffHistory shipmentDropOffHistory)
        {
            await _shipmentDropOffHistoryRepository.DeleteAsync(shipmentDropOffHistory);
        }

        public virtual async Task<ShipmentDropOffHistory> GetShipmentDropOffHistoryByIdAsync(int id)
        {
            return await _shipmentDropOffHistoryRepository.GetByIdAsync(id);
        }

        public virtual async Task<IPagedList<ShipmentDropOffHistory>> GetAllShipmentDropOffHistoriesAsync(int shipmentId, int transporterId = 0,
            int vehicleId = 0, string dropoffLocation = null, int transporterTypeId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (shipmentId == 0)
                return new PagedList<ShipmentDropOffHistory>(new List<ShipmentDropOffHistory>(), pageIndex, pageSize);

            var shipmentDropOffHistorys = await _shipmentDropOffHistoryRepository.GetAllPagedAsync(query =>
            {
                query = query.Where(o => o.ShipmentId == shipmentId && !o.Deleted);

                if (transporterId > 0)
                    query = query.Where(o => o.TransporterId == transporterId);

                if (vehicleId > 0)
                    query = query.Where(o => o.VehicleId == vehicleId);

                if (!string.IsNullOrWhiteSpace(dropoffLocation))
                    query = query.Where(o => o.DropoffLocation.Contains(dropoffLocation));

                if (transporterTypeId > 0)
                    query = query.Where(o => o.TransporterTypeId == transporterTypeId);

                if (createdFromUtc.HasValue)
                    query = query.Where(s => createdFromUtc.Value <= s.CreatedOnUtc);

                if (createdToUtc.HasValue)
                    query = query.Where(s => createdToUtc.Value >= s.CreatedOnUtc);

                query = query.OrderBy(s => s.CreatedOnUtc);

                return query;
            }, pageIndex, pageSize);

            return shipmentDropOffHistorys;
        }

        #endregion

        #region Shipment Return

        public virtual async Task DeleteShipmenReturntAsync(ShipmentReturn shipmentReturn)
        {
            await _shipmentReturnRepository.DeleteAsync(shipmentReturn);
        }

        public virtual async Task<IPagedList<ShipmentReturn>> GetAllShipmentReturnsAsync(int shipmentId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var shipmentReturns = await _shipmentReturnRepository.GetAllPagedAsync(query =>
            {
                if (shipmentId > 0)
                    query = query.Where(s => s.ShipmentId == shipmentId);

                query = query.OrderByDescending(s => s.ShipmentId);

                return query;
            }, pageIndex, pageSize);

            return shipmentReturns;
        }

        public virtual async Task<ShipmentReturn> GetShipmentReturnByIdAsync(int shipmentReturnId)
        {
            return await _shipmentReturnRepository.GetByIdAsync(shipmentReturnId);
        }

        public virtual async Task InsertShipmentReturnAsync(ShipmentReturn shipmentReturn)
        {
            await _shipmentReturnRepository.InsertAsync(shipmentReturn);
        }

        public virtual async Task UpdateShipmentReturnAsync(ShipmentReturn shipmentReturn)
        {
            await _shipmentReturnRepository.UpdateAsync(shipmentReturn);
        }

        #endregion
    }
}