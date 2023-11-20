using Zaraye.Core;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Directory;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Payments;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data;
using Zaraye.Services.Catalog;
using Zaraye.Services.Html;
using Zaraye.Services.Shipping;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zaraye.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderService : IOrderService
    {
        #region Fields

        private readonly IHtmlFormatter _htmlFormatter;
        private readonly IProductService _productService;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        private readonly IRepository<RecurringPayment> _recurringPaymentRepository;
        private readonly IRepository<RecurringPaymentHistory> _recurringPaymentHistoryRepository;
        private readonly IShipmentService _shipmentService;
        private readonly IRepository<OrderCalculation> _orderCalculationRepository;
        private readonly IRepository<Contract> _contractRepository;
        private readonly IRepository<OrderDeliverySchedule> _orderDeliveryScheduleRepository;
        private readonly IRepository<OrderDeliveryRequest> _orderDeliveryRequestrepository;
        private readonly IRepository<OrderSalesReturnRequest> _orderSalesReturnRequestRepository;
        private readonly IRepository<DirectOrder> _directOrderrepository;
        private readonly IRepository<DirectOrderDeliverySchedule> _directOrderDelievrySceduleRepository;
        private readonly IRepository<DirectOrderCalculation> _directOrderCalculationRepository;
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<OrderCancellation> _orderCancellationRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;

        #endregion

        #region Ctor

        public OrderService(IHtmlFormatter htmlFormatter,
            IProductService productService,
            IRepository<Address> addressRepository,
            IRepository<Customer> customerRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IRepository<Product> productRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<RecurringPaymentHistory> recurringPaymentHistoryRepository,
            IShipmentService shipmentService,
            IRepository<OrderCalculation> orderCalculationRepository,
            IRepository<Contract> contractRepository,
            IRepository<OrderDeliverySchedule> orderDeliveryScheduleRepository,
            IRepository<OrderDeliveryRequest> orderDeliveryRequestrepository,
            IRepository<OrderSalesReturnRequest> orderSalesReturnRequestRepository,
            IRepository<DirectOrder> directOrderrepository,
            IRepository<DirectOrderDeliverySchedule> directOrderDelievrySceduleRepository,
            IRepository<DirectOrderCalculation> directOrderCalculationRepository,
            IRepository<Request> requestRepository,
            IRepository<OrderCancellation> orderCancellationRepository,
            IRepository<MeasureWeight> measureWeightRepository)
        {
            _htmlFormatter = htmlFormatter;
            _productService = productService;
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderNoteRepository = orderNoteRepository;
            _productRepository = productRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            _recurringPaymentRepository = recurringPaymentRepository;
            _recurringPaymentHistoryRepository = recurringPaymentHistoryRepository;
            _shipmentService = shipmentService;
            _orderCalculationRepository = orderCalculationRepository;
            _contractRepository = contractRepository;
            _orderDeliveryScheduleRepository = orderDeliveryScheduleRepository;
            _orderDeliveryRequestrepository = orderDeliveryRequestrepository;
            _orderSalesReturnRequestRepository = orderSalesReturnRequestRepository;
            _directOrderrepository = directOrderrepository;
            _directOrderDelievrySceduleRepository = directOrderDelievrySceduleRepository;
            _directOrderCalculationRepository = directOrderCalculationRepository;
            _requestRepository = requestRepository;
            _orderCancellationRepository = orderCancellationRepository;
            _measureWeightRepository = measureWeightRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets the value indicating whether there are shipment items with a positive quantity in order shipments.
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="predicate">Predicate to filter shipments or null to check all shipments</param>
        /// <returns>The <see cref="Task"/> containing the value indicating whether there are shipment items with a positive quantity in order shipments.</returns>
        protected virtual async Task<bool> HasShipmentItemsAsync(Order order, Func<Shipment, bool> predicate = null)
        {
            //var shipmentTypeId = order.OrderTypeId == (int)OrderType.PurchaseOrder ? (int)ShipmentType.PurchaseOrder : (int)ShipmentType.SaleOrder;
            var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(order.Id);
            if (shipments?.Any(shipment => predicate == null || predicate(shipment)) == true)
            {
                var orderItems = await GetOrderItemsAsync(order.Id, isShipEnabled: true);
                if (orderItems?.Any() == true)
                {
                    foreach (var shipment in shipments)
                    {
                        if (predicate?.Invoke(shipment) == false)
                            continue;

                        bool hasPositiveQuantity(ShipmentItem shipmentItem)
                        {
                            return orderItems.Any(orderItem => orderItem.Id == shipmentItem.OrderItemId && shipmentItem.Quantity > 0);
                        }

                        var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);
                        if (shipmentItems?.Any(hasPositiveQuantity) == true)
                            return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Methods

        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId,
                cache => cache.PrepareKeyForShortTermCache(ZarayeEntityCacheDefaults<Order>.ByIdCacheKey, orderId));
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<Order> GetOrderByCustomOrderNumberAsync(string customOrderNumber)
        {
            if (string.IsNullOrEmpty(customOrderNumber))
                return null;

            return await _orderRepository.Table
                .FirstOrDefaultAsync(o => o.CustomOrderNumber == customOrderNumber);
        }

        /// <summary>
        /// Gets an order by order item identifier
        /// </summary>
        /// <param name="orderItemId">The order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<Order> GetOrderByOrderItemAsync(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return await (from o in _orderRepository.Table
                          join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                          where oi.Id == orderItemId
                          select o).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<IList<Order>> GetOrdersByIdsAsync(int[] orderIds)
        {
            return await _orderRepository.GetByIdsAsync(orderIds, includeDeleted: false);
        }

        /// <summary>
        /// Get orders by guids
        /// </summary>
        /// <param name="orderGuids">Order guids</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the orders
        /// </returns>
        public virtual async Task<IList<Order>> GetOrdersByGuidsAsync(Guid[] orderGuids)
        {
            if (orderGuids == null)
                return null;

            var query = from o in _orderRepository.Table
                        where orderGuids.Contains(o.OrderGuid)
                        select o;
            var orders = await query.ToListAsync();

            return orders;
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<Order> GetOrderByGuidAsync(Guid orderGuid)
        {
            if (orderGuid == Guid.Empty)
                return null;

            var query = from o in _orderRepository.Table
                        where o.OrderGuid == orderGuid
                        select o;
            var order = await query.FirstOrDefaultAsync();

            return order;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteOrderAsync(Order order)
        {
            await _orderRepository.DeleteAsync(order);
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; 0 to load all orders</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="affiliateId">Affiliate identifier; 0 to load all orders</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier, only orders with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="osIds">Order status identifiers; null to load all orders</param>
        /// <param name="psIds">Payment status identifiers; null to load all orders</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all orders</param>
        /// <param name="billingPhone">Billing phone. Leave empty to load all records.</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the orders
        /// </returns>
        public virtual async Task<IPagedList<Order>> SearchOrdersAsync(int storeId = 0,
            int customerId = 0, int orderTypeId = 0, int industryId = 0, int categoryId = 0, string fullname = null,
            int productId = 0, int rfqId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null, List<int> bmIds = null,
            string billingPhone = null, string billingEmail = null, string billingLastName = "",
            string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool? getOnlyBuyersActiveOrdersForApi = null, int requestId = 0)
        {
            var query = _orderRepository.Table;

            if (getOnlyBuyersActiveOrdersForApi.HasValue && getOnlyBuyersActiveOrdersForApi.Value)
                query = query.Where(o => (o.OrderStatusId == (int)OrderStatus.Pending || o.OrderStatusId == (int)OrderStatus.Processing));

            if (getOnlyBuyersActiveOrdersForApi.HasValue && !getOnlyBuyersActiveOrdersForApi.Value)
                query = query.Where(o => (o.OrderStatusId == (int)OrderStatus.Cancelled || o.OrderStatusId == (int)OrderStatus.Complete));

            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);

            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);

            if (productId > 0)
            {
                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        where oi.ProductId == productId
                        select o;

                query = query.Distinct();
            }

            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;

                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        join pwi in _productWarehouseInventoryRepository.Table on p.Id equals pwi.ProductId into ps
                        from pwi in ps.DefaultIfEmpty()
                        where
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (p.ManageInventoryMethodId == manageStockInventoryMethodId && p.UseMultipleWarehouses && pwi.WarehouseId == warehouseId) ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((p.ManageInventoryMethodId != manageStockInventoryMethodId || !p.UseMultipleWarehouses) && p.WarehouseId == warehouseId)
                        select o;

                query = query.Distinct();
            }

            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

            if (orderTypeId > 0)
                query = query.Where(o => o.OrderTypeId == orderTypeId);

            if (rfqId > 0)
                query = query.Where(o => o.RFQId == rfqId);
            
            if (requestId > 0)
                query = query.Where(o => o.RequestId == requestId);

            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);

            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);

            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);

            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));

            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));

            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));

            if (bmIds != null && bmIds.Any())
            {
                query = from o in query
                        join oc in _orderCalculationRepository.Table on o.Id equals oc.OrderId
                        where bmIds.Contains(oc.BusinessModelId)
                        select o;
            }


            if (!string.IsNullOrEmpty(orderNotes))
                query = query.Where(o => _orderNoteRepository.Table.Any(oNote => oNote.OrderId == o.Id && oNote.Note.Contains(orderNotes)));

            if (!string.IsNullOrEmpty(fullname))
            {
                query = from o in query
                        join c in _customerRepository.Table on o.CustomerId equals c.Id
                        where c.FullName.Contains(fullname)
                        select o;
            }

            query = from o in query
                    join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
                    where
                        (billingCountryId <= 0 || (oba.CountryId == billingCountryId)) &&
                        (string.IsNullOrEmpty(billingPhone) || (!string.IsNullOrEmpty(oba.PhoneNumber) && oba.PhoneNumber.Contains(billingPhone))) &&
                        (string.IsNullOrEmpty(billingEmail) || (!string.IsNullOrEmpty(oba.Email) && oba.Email.Contains(billingEmail))) &&
                        (string.IsNullOrEmpty(billingLastName) || (!string.IsNullOrEmpty(oba.LastName) && oba.LastName.Contains(billingLastName)))
                    select o;

            if (industryId > 0)
            {
                query = from o in query
                        join r in _requestRepository.Table on o.RequestId equals r.Id
                        where r.IndustryId == industryId
                        select o;
            }

            if (categoryId > 0 && industryId > 0)
            {
                query = from o in query
                        join r in _requestRepository.Table on o.RequestId equals r.Id
                        where r.CategoryId == categoryId && r.IndustryId == industryId
                        select o;
            }

            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertOrderAsync(Order order)
        {
            await _orderRepository.InsertAsync(order);
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateAsync(order);
        }

        /// <summary>
        /// Parse tax rates
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="taxRatesStr"></param>
        /// <returns>Rates</returns>
        public virtual SortedDictionary<decimal, decimal> ParseTaxRates(Order order, string taxRatesStr)
        {
            var taxRatesDictionary = new SortedDictionary<decimal, decimal>();

            if (string.IsNullOrEmpty(taxRatesStr))
                return taxRatesDictionary;

            var lines = taxRatesStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;

                var taxes = line.Split(':');
                if (taxes.Length != 2)
                    continue;

                try
                {
                    var taxRate = decimal.Parse(taxes[0].Trim(), CultureInfo.InvariantCulture);
                    var taxValue = decimal.Parse(taxes[1].Trim(), CultureInfo.InvariantCulture);
                    taxRatesDictionary.Add(taxRate, taxValue);
                }
                catch
                {
                    // ignored
                }
            }

            //add at least one tax rate (0%)
            if (!taxRatesDictionary.Any())
                taxRatesDictionary.Add(decimal.Zero, decimal.Zero);

            return taxRatesDictionary;
        }

        /// <summary>
        /// Gets a value indicating whether an order has items to be added to a shipment
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to be added to a shipment
        /// </returns>
        public virtual async Task<bool> HasItemsToAddToShipmentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            foreach (var orderItem in await GetOrderItemsAsync(order.Id, isShipEnabled: true)) //we can ship only shippable products
            {
                var totalNumberOfItemsCanBeAddedToShipment = await GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
                if (totalNumberOfItemsCanBeAddedToShipment <= 0)
                    continue;

                //yes, we have at least one item to create a new shipment
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether an order has items to ship
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to ship
        /// </returns>
        public virtual async Task<bool> HasItemsToShipAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.PickupInStore)
                return false;

            return await HasShipmentItemsAsync(order, shipment => !shipment.ShippedDateUtc.HasValue);
        }

        /// <summary>
        /// Gets a value indicating whether there are shipment items to mark as 'ready for pickup' in order shipments.
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether there are shipment items to mark as 'ready for pickup' in order shipments.
        /// </returns>
        public virtual async Task<bool> HasItemsToReadyForPickupAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!order.PickupInStore)
                return false;

            return await HasShipmentItemsAsync(order, shipment => !shipment.ReadyForPickupDateUtc.HasValue);
        }

        /// <summary>
        /// Gets a value indicating whether an order has items to deliver
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to deliver
        /// </returns>
        public virtual async Task<bool> HasItemsToDeliverAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return await HasShipmentItemsAsync(order, shipment => (shipment.ShippedDateUtc.HasValue || shipment.ReadyForPickupDateUtc.HasValue) && !shipment.DeliveryDateUtc.HasValue);
        }

        public virtual async Task<IList<Order>> GetOrdersByRfqAsync(int rfqId)
        {
            if (rfqId == 0)
                return new List<Order>();

            return await (from o in _orderRepository.Table
                          where !o.Deleted && o.OrderStatusId != (int)OrderStatus.Cancelled && (o.RFQId == rfqId || (o.RFQId == null && rfqId == 0))
                          select o).ToListAsync();
        }

        public virtual async Task<IPagedList<Order>> GetOrdersByParentIdAsync(int parentOrderId, int orderTypeId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _orderRepository.Table;
            query = query.Where(o => !o.Deleted && o.ParentOrderId == parentOrderId && o.OrderTypeId == orderTypeId);

            //database layer paging
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #region Orders items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        public virtual async Task<OrderItem> GetOrderItemByIdAsync(int orderItemId)
        {
            return await _orderItemRepository.GetByIdAsync(orderItemId,
                cache => cache.PrepareKeyForShortTermCache(ZarayeEntityCacheDefaults<OrderItem>.ByIdCacheKey, orderItemId));
        }

        /// <summary>
        /// Gets a product of specify order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product
        /// </returns>
        public virtual async Task<Product> GetProductByOrderItemIdAsync(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return await (from p in _productRepository.Table
                          join oi in _orderItemRepository.Table on p.Id equals oi.ProductId
                          where oi.Id == orderItemId
                          select p).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Gets a list items of order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="isNotReturnable">Value indicating whether this product is returnable; pass null to ignore</param>
        /// <param name="isShipEnabled">Value indicating whether the entity is ship enabled; pass null to ignore</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<OrderItem>> GetOrderItemsAsync(int orderId, bool? isNotReturnable = null, bool? isShipEnabled = null)
        {
            if (orderId == 0)
                return new List<OrderItem>();

            return await (from oi in _orderItemRepository.Table
                          join p in _productRepository.Table on oi.ProductId equals p.Id
                          where
                          oi.OrderId == orderId &&
                          (!isShipEnabled.HasValue || (p.IsShipEnabled == isShipEnabled.Value)) &&
                          (!isNotReturnable.HasValue || (p.NotReturnable == isNotReturnable))
                          select oi).ToListAsync();
        }

        /// <summary>
        /// Gets an item
        /// </summary>
        /// <param name="orderItemGuid">Order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        public virtual async Task<OrderItem> GetOrderItemByGuidAsync(Guid orderItemGuid)
        {
            if (orderItemGuid == Guid.Empty)
                return null;

            var query = from orderItem in _orderItemRepository.Table
                        where orderItem.OrderItemGuid == orderItemGuid
                        select orderItem;
            var item = await query.FirstOrDefaultAsync();
            return item;
        }

        public virtual async Task<Order> GetOrderByQuotationIdAsync(int quotationId)
        {
            if (quotationId < 0)
                return null;

            var query = from o in _orderRepository.Table
                        where o.QuotationId == quotationId && o.OrderStatusId != (int)OrderStatus.Cancelled
                        select o;
            var item = await query.FirstOrDefaultAsync();
            return item;
        }

        /// <summary>
        /// Gets all downloadable order items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order items
        /// </returns>
        public virtual async Task<IList<OrderItem>> GetDownloadableOrderItemsAsync(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentOutOfRangeException(nameof(customerId));

            var query = from orderItem in _orderItemRepository.Table
                        join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                        join p in _productRepository.Table on orderItem.ProductId equals p.Id
                        where customerId == o.CustomerId &&
                        p.IsDownload &&
                        !o.Deleted
                        orderby o.CreatedOnUtc descending, orderItem.Id
                        select orderItem;

            var orderItems = await query.ToListAsync();
            return orderItems;
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteOrderItemAsync(OrderItem orderItem)
        {
            await _orderItemRepository.DeleteAsync(orderItem);
        }

        /// <summary>
        /// Gets a total number of items in all shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the total number of items in all shipments
        /// </returns>
        public virtual async Task<decimal> GetTotalNumberOfItemsInAllShipmentsAsync(OrderItem orderItem, bool loadOnlyShipped = false)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var order = await GetOrderByIdAsync(orderItem.OrderId);

            decimal totalInShipments = 0;
            var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(orderItem.OrderId, loadOnlyShipped: loadOnlyShipped);
            for (var i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                var si = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
                    .FirstOrDefault(x => x.OrderItemId == orderItem.Id);
                if (si != null)
                {
                    totalInShipments += si.Quantity;
                }
            }

            var returnOrders = await GetOrdersByParentIdAsync(order.Id, (order.OrderTypeId == (int)OrderType.SaleOrder ? (int)OrderType.ReturnSaleOrder : (int)OrderType.ReturnPurchaseOrder));
            if (returnOrders.Any())
            {
                foreach (var returnOrder in returnOrders)
                {
                    var returnShipments = (await _shipmentService.GetAllShipmentsAsync(orderId: returnOrder.Id)).Where(x => x.DeliveryDateUtc.HasValue).ToList();
                    foreach (var returnShipment in returnShipments)
                        totalInShipments -= (await _shipmentService.GetShipmentItemsByShipmentIdAsync(returnShipment.Id)).Sum(x => x.Quantity);

                    //totalInShipments -= (await GetOrderItemsAsync(returnOrder.Id)).FirstOrDefault().Quantity;
                }
            }

            return totalInShipments;
        }

        /// <summary>
        /// Gets a total number of already items which can be added to new shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the total number of already delivered items which can be added to new shipments
        /// </returns>
        public virtual async Task<decimal> GetTotalNumberOfItemsCanBeAddedToShipmentAsync(OrderItem orderItem, bool loadOnlyShipped = false)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var totalInShipments = await GetTotalNumberOfItemsInAllShipmentsAsync(orderItem, loadOnlyShipped);

            var qtyOrdered = orderItem.Quantity;
            var qtyCanBeAddedToShipmentTotal = qtyOrdered - totalInShipments;
            if (qtyCanBeAddedToShipmentTotal < 0)
                qtyCanBeAddedToShipmentTotal = 0;

            return qtyCanBeAddedToShipmentTotal;
        }

        /// <summary>
        /// Gets a value indicating whether download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true if download is allowed; otherwise, false.
        /// </returns>
        public virtual async Task<bool> IsDownloadAllowedAsync(OrderItem orderItem)
        {
            if (orderItem is null)
                return false;

            var order = await GetOrderByIdAsync(orderItem.OrderId);
            if (order == null || order.Deleted)
                return false;

            //order status
            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            if (product == null || !product.IsDownload)
                return false;

            //payment status
            switch (product.DownloadActivationType)
            {
                case DownloadActivationType.WhenOrderIsPaid:
                    if (order.PaymentStatus == PaymentStatus.Paid && order.PaidDateUtc.HasValue)
                    {
                        //expiration date
                        if (product.DownloadExpirationDays.HasValue)
                        {
                            if (order.PaidDateUtc.Value.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                    break;
                case DownloadActivationType.Manually:
                    if (orderItem.IsDownloadActivated)
                    {
                        //expiration date
                        if (product.DownloadExpirationDays.HasValue)
                        {
                            if (order.CreatedOnUtc.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                    break;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether license download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true if license download is allowed; otherwise, false.
        /// </returns>
        public virtual async Task<bool> IsLicenseDownloadAllowedAsync(OrderItem orderItem)
        {
            if (orderItem == null)
                return false;

            return await IsDownloadAllowedAsync(orderItem) &&
                orderItem.LicenseDownloadId.HasValue &&
                orderItem.LicenseDownloadId > 0;
        }

        /// <summary>
        /// Inserts a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertOrderItemAsync(OrderItem orderItem)
        {
            await _orderItemRepository.InsertAsync(orderItem);
        }

        /// <summary>
        /// Updates a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateOrderItemAsync(OrderItem orderItem)
        {
            await _orderItemRepository.UpdateAsync(orderItem);
        }

        public virtual int GetTotalOrderItemsCountAsync()
        {
            var totalOrderItemCount = Convert.ToInt32(_orderItemRepository.Table.Sum(x => x.Quantity));
            return totalOrderItemCount;
        }

        public virtual decimal GetlOrderItemsCountByOrder(int orderId)
        {
            var totalOrderItemCount = _orderItemRepository.Table.Where(x => x.OrderId == orderId).Sum(x => x.Quantity);
            return totalOrderItemCount;
        }

        #endregion

        #region Orders notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order note
        /// </returns>
        public virtual async Task<OrderNote> GetOrderNoteByIdAsync(int orderNoteId)
        {
            return await _orderNoteRepository.GetByIdAsync(orderNoteId);
        }

        /// <summary>
        /// Gets a list notes of order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="displayToCustomer">Value indicating whether a customer can see a note; pass null to ignore</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<OrderNote>> GetOrderNotesByOrderIdAsync(int orderId, bool? displayToCustomer = null)
        {
            if (orderId == 0)
                return new List<OrderNote>();

            var query = _orderNoteRepository.Table.Where(on => on.OrderId == orderId);

            if (displayToCustomer.HasValue)
            {
                query = query.Where(on => on.DisplayToCustomer == displayToCustomer);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteOrderNoteAsync(OrderNote orderNote)
        {
            await _orderNoteRepository.DeleteAsync(orderNote);
        }

        /// <summary>
        /// Formats the order note text
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatOrderNoteText(OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException(nameof(orderNote));

            var text = orderNote.Note;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = _htmlFormatter.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        /// <summary>
        /// Inserts an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertOrderNoteAsync(OrderNote orderNote)
        {
            await _orderNoteRepository.InsertAsync(orderNote);
        }

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteRecurringPaymentAsync(RecurringPayment recurringPayment)
        {
            await _recurringPaymentRepository.DeleteAsync(recurringPayment);
        }

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payment
        /// </returns>
        public virtual async Task<RecurringPayment> GetRecurringPaymentByIdAsync(int recurringPaymentId)
        {
            return await _recurringPaymentRepository.GetByIdAsync(recurringPaymentId, cache => default);
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertRecurringPaymentAsync(RecurringPayment recurringPayment)
        {
            await _recurringPaymentRepository.InsertAsync(recurringPayment);
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateRecurringPaymentAsync(RecurringPayment recurringPayment)
        {
            await _recurringPaymentRepository.UpdateAsync(recurringPayment);
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="storeId">The store identifier; 0 to load all records</param>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payments
        /// </returns>
        public virtual async Task<IPagedList<RecurringPayment>> SearchRecurringPaymentsAsync(int storeId = 0,
            int customerId = 0, int initialOrderId = 0, OrderStatus? initialOrderStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            int? initialOrderStatusId = null;
            if (initialOrderStatus.HasValue)
                initialOrderStatusId = (int)initialOrderStatus.Value;

            var query1 = from rp in _recurringPaymentRepository.Table
                         join o in _orderRepository.Table on rp.InitialOrderId equals o.Id
                         join c in _customerRepository.Table on o.CustomerId equals c.Id
                         where
                         !rp.Deleted &&
                         (showHidden || !o.Deleted) &&
                         (showHidden || !c.Deleted) &&
                         (showHidden || rp.IsActive) &&
                         (customerId == 0 || o.CustomerId == customerId) &&
                         (storeId == 0 || o.StoreId == storeId) &&
                         (initialOrderId == 0 || o.Id == initialOrderId) &&
                         (!initialOrderStatusId.HasValue || initialOrderStatusId.Value == 0 ||
                          o.OrderStatusId == initialOrderStatusId.Value)
                         select rp.Id;

            var query2 = from rp in _recurringPaymentRepository.Table
                         where query1.Contains(rp.Id)
                         orderby rp.StartDateUtc, rp.Id
                         select rp;

            var recurringPayments = await query2.ToPagedListAsync(pageIndex, pageSize);

            return recurringPayments;
        }

        #endregion

        #region Recurring payments history

        /// <summary>
        /// Gets a recurring payment history
        /// </summary>
        /// <param name="recurringPayment">The recurring payment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<RecurringPaymentHistory>> GetRecurringPaymentHistoryAsync(RecurringPayment recurringPayment)
        {
            if (recurringPayment is null)
                throw new ArgumentNullException(nameof(recurringPayment));

            return await _recurringPaymentHistoryRepository.Table
                .Where(rph => rph.RecurringPaymentId == recurringPayment.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Inserts a recurring payment history entry
        /// </summary>
        /// <param name="recurringPaymentHistory">Recurring payment history entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertRecurringPaymentHistoryAsync(RecurringPaymentHistory recurringPaymentHistory)
        {
            await _recurringPaymentHistoryRepository.InsertAsync(recurringPaymentHistory);
        }

        #endregion

        #endregion

        #region GetDollarRatePKR

        public virtual async Task<decimal> GetDollarRatePKR()
        {
            decimal dollarRatePKR = 0;

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage result = await client.GetAsync($"https://www.forex.pk/currency-usd-to-pkr-to-us-dollar.php");
                var content = (await result.Content.ReadAsStringAsync());
                Regex r = new Regex(@"<span[^>].*?>([^<]*)<\/span>", RegexOptions.IgnoreCase);

                foreach (Match matchedSpan in r.Matches(content).Where(x => x.Value.Contains("RATESPAN")))
                {
                    try
                    {
                        decimal.TryParse(matchedSpan.Groups[1].Value, out dollarRatePKR);
                    }
                    catch
                    {
                        continue;
                    }

                }
                return dollarRatePKR;
            }
            catch
            {
                return dollarRatePKR;
            }

        }

        #endregion

        #region Custom Order Methods

        public virtual async Task<IList<Order>> GetOrdersByRFQIdsAsync(int[] rfqIds)
        {
            return await _orderRepository.Table.Where(order => rfqIds.Contains(order.RFQId.Value)).ToListAsync();
        }


        public virtual async Task<Order> GetOrderByRequestIdAsync(int requestId)
        {
            if (requestId == 0)
                return null;

            return await _orderRepository.Table.FirstOrDefaultAsync(o => o.RequestId == requestId && o.OrderTypeId == (int)OrderType.SaleOrder);
        }

        public virtual async Task<Order> GetOrderByRFQIdAsync(int rfqId)
        {
            if (rfqId == 0)
                return null;

            return await _orderRepository.Table.FirstOrDefaultAsync(o => o.RFQId == rfqId && o.OrderTypeId == (int)OrderType.PurchaseOrder && o.OrderStatusId != (int)OrderStatus.Cancelled);
        }

        #endregion

        #region Order Calculation

        public virtual async Task<IList<OrderCalculation>> GetOrderCalculationsByIdsAsync(int[] orderCalculationIds)
        {
            return await _orderCalculationRepository.GetByIdsAsync(orderCalculationIds, cache => default, false);
        }

        public virtual async Task<OrderCalculation> GetOrderCalculationByIdAsync(int orderCalculationId)
        {
            return await _orderCalculationRepository.GetByIdAsync(orderCalculationId, cache => default);
        }

        public virtual async Task<OrderCalculation> GetOrderCalculationByOrderIdAsync(int orderId)
        {
            if (orderId == 0)
                return null;

            return await _orderCalculationRepository.Table.OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public virtual async Task InsertOrderCalculationAsync(OrderCalculation orderCalculation)
        {
            await _orderCalculationRepository.InsertAsync(orderCalculation);
        }

        public virtual async Task UpdateOrderCalculationAsync(OrderCalculation orderCalculation)
        {
            await _orderCalculationRepository.UpdateAsync(orderCalculation);
        }

        #endregion

        #region Order Contract

        public virtual async Task DeleteContractAsync(Contract contract)
        {
            await _contractRepository.UpdateAsync(contract);
        }

        public virtual async Task<Contract> GetContractByIdAsync(int contractId)
        {
            return await _contractRepository.GetByIdAsync(contractId);
        }

        public virtual async Task<Contract> GetContractByGuidAsync(Guid guid)
        {
            var query = from dc in _contractRepository.Table
                        where dc.ContractGuid == guid
                        select dc;

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<Contract> GetContractByOrderIdAsync(int orderId)
        {
            var query = from dc in _contractRepository.Table
                        where dc.OrderId == orderId
                        select dc;

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task InsertContractAsync(Contract contract)
        {
            await _contractRepository.InsertAsync(contract);
        }

        public virtual async Task UpdateContractAsync(Contract contract)
        {
            await _contractRepository.UpdateAsync(contract);
        }

        public virtual async Task<IPagedList<Contract>> GetAllContractAsync(int orderId = 0, int buyerId = 0, int supplierId = 0, string contractType = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from dc in _contractRepository.Table select dc;

            if (orderId > 0)
                query = query.Where(x => x.OrderId == orderId);

            if (buyerId > 0)
                query = query.Where(x => x.CreatedById == buyerId && x.ContractType == "Buyer");

            if (supplierId > 0)
                query = query.Where(x => x.CreatedById == supplierId && x.ContractType == "Supplier");

            if (!string.IsNullOrWhiteSpace(contractType))
                query = query.Where(x => x.ContractType == contractType);

            var contract = await query.ToListAsync();

            //paging
            return new PagedList<Contract>(contract, pageIndex, pageSize);
        }

        #endregion

        #region Order Delivery Schedule

        public virtual async Task InsertDeliveryScheduleAsync(OrderDeliverySchedule deliverySchedule)
        {
            await _orderDeliveryScheduleRepository.InsertAsync(deliverySchedule);
        }

        public virtual async Task UpdateDeliveryScheduleAsync(OrderDeliverySchedule deliverySchedule)
        {
            await _orderDeliveryScheduleRepository.UpdateAsync(deliverySchedule);
        }

        public virtual async Task DeleteDeliveryScheduleAsync(OrderDeliverySchedule deliverySchedule)
        {
            await _orderDeliveryScheduleRepository.DeleteAsync(deliverySchedule);
        }

        public virtual async Task<OrderDeliverySchedule> GetDeliveryScheduleByIdAsync(int id)
        {
            return await _orderDeliveryScheduleRepository.GetByIdAsync(id);
        }

        public virtual async Task<IPagedList<OrderDeliverySchedule>> GetAllDeliveryScheduleAsync(int orderId = 0, int pageIndex = 0,
            int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _orderDeliveryScheduleRepository.Table;

            if (orderId > 0)
            {
                query = from ds in query
                        join o in _orderRepository.Table on ds.OrderId equals o.Id
                        where !o.Deleted && o.Id == orderId
                        select ds;
            }

            query = query.Where(x => !x.Deleted);

            var deliverySchedule = await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
            return deliverySchedule;
        }

        public virtual decimal GetTotalOrderDeliveryScheduleQuantityByOrderIdAsync(int orderId)
        {
            return (from o in _orderDeliveryScheduleRepository.Table
                    where o.OrderId == orderId
                    select o).Sum(s => s.ExpectedQuantity);
        }

        #endregion

        #region OrderDeliveryRequest
        public virtual async Task DeleteOrderDeliveryRequestAsync(OrderDeliveryRequest orderDeliveryRequest)
        {
            await _orderDeliveryRequestrepository.DeleteAsync(orderDeliveryRequest);
        }

        public virtual async Task<OrderDeliveryRequest> GetOrderDeliveryRequestByIdAsync(int orderDeliveryRequestId)
        {
            return await _orderDeliveryRequestrepository.GetByIdAsync(orderDeliveryRequestId);
        }

        public virtual async Task InsertOrderDeliveryRequestAsync(OrderDeliveryRequest orderDeliveryRequest)
        {
            await _orderDeliveryRequestrepository.InsertAsync(orderDeliveryRequest);
        }

        public virtual async Task UpdateOrderDeliveryRequestAsync(OrderDeliveryRequest orderDeliveryRequest)
        {
            await _orderDeliveryRequestrepository.UpdateAsync(orderDeliveryRequest);
        }

        public virtual async Task<IPagedList<OrderDeliveryRequest>> SearchOrderDeliveryRequestsAsync(int orderDeliveryScheduleId = 0, int countryId = 0,
            int cityId = 0, int areaId = 0, int CreatedBy = 0,
            string streetAddress = "", string contactNumber = "", decimal quantity = 0,
            DateTime? shipmentFromUtc = null, DateTime? shipmentToUtc = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, int statusId = 0, int agentId = 0, List<int> sIds = null)
        {
            var query = _orderDeliveryRequestrepository.Table;

            if (orderDeliveryScheduleId > 0)
                query = query.Where(x => x.OrderDeliveryScheduleId == orderDeliveryScheduleId);

            if (countryId > 0)
                query = query.Where(x => x.CountryId == countryId);

            if (cityId > 0)
                query = query.Where(x => x.CityId == cityId);

            if (areaId > 0)
                query = query.Where(x => x.AreaId == areaId);

            if (quantity > 0)
                query = query.Where(x => x.Quantity == quantity);

            if (CreatedBy > 0)
                query = query.Where(x => x.CreatedById == CreatedBy);

            if (!string.IsNullOrEmpty(contactNumber))
                query = query.Where(x => x.ContactNumber.Contains(contactNumber));

            //if (shipmentFromUtc.HasValue)
            //    query = query.Where(o => shipmentFromUtc.Value <= o.ShipmentDateUtc);

            //if (shipmentToUtc.HasValue)
            //    query = query.Where(o => shipmentToUtc.Value >= o.ShipmentDateUtc);

            if (statusId > 0)
                query = query.Where(x => x.StatusId == statusId);

            if (agentId > 0)
                query = query.Where(x => x.AgentId == agentId);

            if (sIds != null && sIds.Any())
                query = query.Where(o => sIds.Contains(o.StatusId));

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<OrderDeliveryRequest> GetOrderDeliveryRequestByOrderIdAsync(int orderId)
        {
            return await (from o in _orderDeliveryRequestrepository.Table
                          where o.OrderId == orderId
                          select o).FirstOrDefaultAsync();
        }

        public virtual decimal GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(int orderId, int expectedShipmentId)
        {
            return (from odr in _orderDeliveryRequestrepository.Table
                    where
                    (odr.StatusId == (int)OrderDeliveryRequestEnum.Pending ||
                    odr.StatusId == (int)OrderDeliveryRequestEnum.Complete) &&
                    odr.OrderId == orderId && odr.OrderDeliveryScheduleId == expectedShipmentId
                    select odr).Sum(s => s.Quantity);
        }
        #endregion

        #region Order Sales Return Request

        public virtual async Task DeleteOrderSalesReturnRequestAsync(OrderSalesReturnRequest orderSalesReturnRequest)
        {
            await _orderSalesReturnRequestRepository.DeleteAsync(orderSalesReturnRequest);
        }

        public virtual async Task<OrderSalesReturnRequest> GetOrderSalesReturnRequestByIdAsync(int orderSalesReturnRequestId)
        {
            return await _orderSalesReturnRequestRepository.GetByIdAsync(orderSalesReturnRequestId);
        }

        public virtual async Task InsertOrderSalesReturnRequestAsync(OrderSalesReturnRequest orderSalesReturnRequest)
        {
            await _orderSalesReturnRequestRepository.InsertAsync(orderSalesReturnRequest);
        }

        public virtual async Task UpdateOrderSalesReturnRequestAsync(OrderSalesReturnRequest orderSalesReturnRequest)
        {
            await _orderSalesReturnRequestRepository.UpdateAsync(orderSalesReturnRequest);
        }

        public virtual async Task<IPagedList<OrderSalesReturnRequest>> SearchOrderSalesReturnRequestsAsync(int orderId = 0, int supplierId = 0,
            int quotationId = 0, int createdBy = 0,
            string pickupAddress = "", string dropOffAddress = "", decimal quantity = 0,
            DateTime? returnRequestDateFromUtc = null, DateTime? returnRequestDateToUtc = null, bool? isInventory = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int agentId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, List<int> sIds = null)
        {
            var query = _orderSalesReturnRequestRepository.Table;

            if (orderId > 0)
                query = query.Where(x => x.OrderId == orderId);

            if (supplierId > 0)
                query = query.Where(x => x.SupplierId == supplierId);

            if (quotationId > 0)
                query = query.Where(x => x.QuotationId == quotationId);

            if (quantity > 0)
                query = query.Where(x => x.Quantity == quantity);

            if (createdBy > 0)
                query = query.Where(x => x.CreatedById == createdBy);

            if (!string.IsNullOrEmpty(pickupAddress))
                query = query.Where(x => x.PickupAddress.Contains(pickupAddress));

            if (!string.IsNullOrEmpty(dropOffAddress))
                query = query.Where(x => x.DropOffAddress.Contains(dropOffAddress));

            if (quantity > 0)
                query = query.Where(x => x.Quantity == quantity);

            if (returnRequestDateFromUtc.HasValue)
                query = query.Where(o => returnRequestDateFromUtc.Value <= o.ReturnRequestDateUtc);

            if (returnRequestDateToUtc.HasValue)
                query = query.Where(o => returnRequestDateToUtc.Value >= o.ReturnRequestDateUtc);

            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);

            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value >= o.CreatedOnUtc);

            if (isInventory.HasValue && isInventory.Value)
                query = query.Where(x => x.IsInventory == isInventory);

            if (agentId > 0)
                query = query.Where(x => x.AgentId == agentId);

            if (sIds != null && sIds.Any())
                query = query.Where(o => sIds.Contains(o.StatusId));

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #region DirectOrder

        public virtual async Task DeleteDirectOrderAsync(DirectOrder directOrder)
        {
            await _directOrderrepository.DeleteAsync(directOrder);
        }

        public virtual async Task<DirectOrder> GetDirectOrderByIdAsync(int directOrderId)
        {
            return await _directOrderrepository.GetByIdAsync(directOrderId);
        }

        public virtual async Task InsertDirectOrderAsync(DirectOrder directOrder)
        {
            await _directOrderrepository.InsertAsync(directOrder);
        }

        public virtual async Task UpdateDirectOrderAsync(DirectOrder directOrder)
        {
            await _directOrderrepository.UpdateAsync(directOrder);
        }

        public virtual async Task<IPagedList<DirectOrder>> SearchDirectOrdersAsync(int buyerId = 0, int bookerId = 0, int industryId = 0, int rfqId = 0,
            int catgoryId = 0, int productId = 0, int brandId = 0, string otherBrand = "", decimal quantity = 0, bool step1 = false,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _directOrderrepository.Table;

            if (buyerId > 0)
                query = query.Where(x => x.BuyerId == buyerId);

            if (rfqId > 0)
                query = query.Where(x => x.RequestForQuotationId == rfqId);

            if (bookerId > 0)
                query = query.Where(x => x.BookerId == bookerId);

            if (industryId > 0)
                query = query.Where(x => x.IndustryId == industryId);

            if (catgoryId > 0)
                query = query.Where(x => x.CategoryId == catgoryId);

            if (productId > 0)
                query = query.Where(x => x.ProductId == productId);

            if (brandId > 0)
                query = query.Where(x => x.BrandId == brandId);

            if (quantity > 0)
                query = query.Where(x => x.Quantity == quantity);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<DirectOrder> GetDirectOrderByRequestId(int requestId)
        {
            return await (from o in _directOrderrepository.Table
                          where o.RequestId == requestId
                          select o).FirstOrDefaultAsync();
        }

        public virtual async Task<DirectOrder> GetDirectOrderByQuotationId(int quotationId)
        {
            return await (from o in _directOrderrepository.Table
                          where o.QuotationId == quotationId
                          select o).FirstOrDefaultAsync();
        }

        public virtual async Task<DirectOrder> GetDirectOrderAsync(int bookerId, int buyerId, int industryId)
        {
            return await (from o in _directOrderrepository.Table
                          where o.BookerId == bookerId && o.BuyerId == buyerId && o.IndustryId == industryId
                          select o).FirstOrDefaultAsync();
        }

        #endregion

        #region Direct Order Delivery Schedule

        public virtual async Task InsertDirectOrderDeliveryScheduleAsync(DirectOrderDeliverySchedule directOrderDeliverySchedule)
        {
            await _directOrderDelievrySceduleRepository.InsertAsync(directOrderDeliverySchedule);
        }

        public virtual async Task UpdateDirectOrderDeliveryScheduleAsync(DirectOrderDeliverySchedule directOrderDeliverySchedule)
        {
            await _directOrderDelievrySceduleRepository.UpdateAsync(directOrderDeliverySchedule);
        }

        public virtual async Task DeleteDirectOrderDeliveryScheduleAsync(DirectOrderDeliverySchedule directOrderDeliverySchedule)
        {
            await _directOrderDelievrySceduleRepository.DeleteAsync(directOrderDeliverySchedule);
        }

        public virtual async Task<DirectOrderDeliverySchedule> GetDirectOrderDeliveryScheduleByIdAsync(int id)
        {
            return await _directOrderDelievrySceduleRepository.GetByIdAsync(id);
        }
        public virtual async Task<IList<DirectOrderDeliverySchedule>> GetAllDirectOrderDeliveryScheduleAsync(int directOrderId)
        {
            var query = _directOrderDelievrySceduleRepository.Table;

            if (directOrderId > 0)
                query = query.Where(x => x.DirectOrderId == directOrderId);

            var deliverySchedule = await query.ToListAsync();

            return deliverySchedule;

        }

        #endregion

        #region Direct Order Calculation

        public virtual async Task InsertDirectOrderCalculationAsync(DirectOrderCalculation directOrderCalculation)
        {
            await _directOrderCalculationRepository.InsertAsync(directOrderCalculation);
        }

        public virtual async Task UpdateDirectOrderCalculationAsync(DirectOrderCalculation directOrderCalculation)
        {
            await _directOrderCalculationRepository.UpdateAsync(directOrderCalculation);
        }

        public virtual async Task DeleteDirectOrderCalculationAsync(DirectOrderCalculation directOrderCalculation)
        {
            await _directOrderCalculationRepository.DeleteAsync(directOrderCalculation);
        }

        public virtual async Task<DirectOrderCalculation> GetDirectOrderCalculationByIdAsync(int id)
        {
            return await _directOrderCalculationRepository.GetByIdAsync(id);
        }
        public virtual async Task<IList<DirectOrderCalculation>> GetAllDirectOrderCalculationAsync(int directOrderId)
        {
            var query = _directOrderCalculationRepository.Table;

            if (directOrderId > 0)
                query = query.Where(x => x.DirectOrderId == directOrderId);

            var Calculation = await query.ToListAsync();

            return Calculation;

        }
        public virtual async Task<DirectOrderCalculation> GetDirectOrderCalculationByDirectOrderIdAsync(int directOrderId)
        {
            return await (from oc in _directOrderCalculationRepository.Table
                          where oc.DirectOrderId == directOrderId
                          select oc).FirstOrDefaultAsync();
        }

        #endregion

        #region Order Cancellation

        public virtual async Task DeleteOrderCancellationAsync(OrderCancellation orderCancellation)
        {
            await _orderCancellationRepository.UpdateAsync(orderCancellation);
        }

        public virtual async Task<OrderCancellation> GetOrderCancellationByIdAsync(int orderCancellationId)
        {
            return await _orderCancellationRepository.GetByIdAsync(orderCancellationId);
        }

        public virtual async Task<OrderCancellation> GetOrderCancellationByOrderIdAsync(int orderId)
        {
            var query = from dc in _orderCancellationRepository.Table
                        where dc.OrderId == orderId
                        select dc;

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task InsertOrderCancellationAsync(OrderCancellation orderCancellation)
        {
            await _orderCancellationRepository.InsertAsync(orderCancellation);
        }

        public virtual async Task UpdateOrderCancellationAsync(OrderCancellation orderCancellation)
        {
            await _orderCancellationRepository.UpdateAsync(orderCancellation);
        }

        public virtual async Task<IPagedList<OrderCancellation>> GetAllOrderCancellationAsync(int orderId = 0, string reasen = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from dc in _orderCancellationRepository.Table select dc;

            if (orderId > 0)
                query = query.Where(x => x.OrderId == orderId);

            if (!string.IsNullOrWhiteSpace(reasen))
                query = query.Where(x => x.Reason == reasen);

            var OrderCancellation = await query.ToListAsync();

            //paging
            return new PagedList<OrderCancellation>(OrderCancellation, pageIndex, pageSize);
        }

        #endregion

        public virtual async Task<string> GetUnitTypeByOrderAsync(int orderId)
        {
            return await
                (from o in _orderRepository.Table
                 join r in _requestRepository.Table on o.RequestId equals r.Id
                 join p in _productRepository.Table on r.ProductId equals p.Id
                 join m in _measureWeightRepository.Table on p.UnitId equals m.Id
                 where o.Id == orderId
                 select m.SystemKeyword
            ).FirstOrDefaultAsync();
        }

        //public virtual async Task<decimal> GetTotalEnrouteShipmentsAmountBySupplier(int supplierId)
        //{
        //    var orders = from o in _orderRepository.Table
        //                 join s in _customerRepository.Table on o.CustomerId equals s.Id
        //                 where o.OrderTypeId == (int)OrderType.PurchaseOrder && !o.Deleted
        //                 select o;

        //    decimal totalreturnAmount = 0;
        //    foreach (var order in orders)
        //    {
        //        //Add shipment delivered amount
        //        var orderCalculation = await GetOrderCalculationByOrderIdAsync(order.Id);
        //        if (orderCalculation is null)
        //            continue;

        //        var orderEnrouteShipments = (await _shipmentService.GetShipmentsByOrderIdAsync(orderId: order.Id, loadOnlyShipped: true)).Where(x => !x.DeliveryDateUtc.HasValue && x.ShipmentTypeId == (int)ShipmentType.PurchaseOrder);
        //        foreach (var enrouteShipment in orderEnrouteShipments)
        //        {
        //            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(enrouteShipment.Id)).FirstOrDefault();
        //            if (shipmentItem == null)
        //                continue;

        //            var amount = orderCalculation.OrderTotal / GetlOrderItemsCountByOrder(order.Id);
        //            totalreturnAmount += amount * shipmentItem.Quantity;
        //        }
        //    }

        //    return totalreturnAmount;
        //}
    }
}