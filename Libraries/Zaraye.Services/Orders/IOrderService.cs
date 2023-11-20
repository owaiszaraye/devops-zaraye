using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.Orders
{
    /// <summary>
    /// Order service interface
    /// </summary>
    public partial interface IOrderService
    {
        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<Order> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<Order> GetOrderByCustomOrderNumberAsync(string customOrderNumber);

        /// <summary>
        /// Gets an order by order item identifier
        /// </summary>
        /// <param name="orderItemId">The order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<Order> GetOrderByOrderItemAsync(int orderItemId);

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<IList<Order>> GetOrdersByIdsAsync(int[] orderIds);

        /// <summary>
        /// Get orders by guids
        /// </summary>
        /// <param name="orderGuids">Order guids</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the orders
        /// </returns>
        Task<IList<Order>> GetOrdersByGuidsAsync(Guid[] orderGuids);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<Order> GetOrderByGuidAsync(Guid orderGuid);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteOrderAsync(Order order);

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; null to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; null to load all orders</param>
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
        Task<IPagedList<Order>> SearchOrdersAsync(int storeId = 0,
            int customerId = 0, int orderTypeId = 0, int industryId = 0, int categoryId = 0, string fullname = null,
            int productId = 0, int rfqId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null, List<int> bmIds = null,
            string billingPhone = null, string billingEmail = null, string billingLastName = "",
            string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool? getOnlyBuyersActiveOrdersForApi = null, int requestId = 0);

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertOrderAsync(Order order);

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateOrderAsync(Order order);

        /// <summary>
        /// Parse tax rates
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="taxRatesStr"></param>
        /// <returns>Rates</returns>
        SortedDictionary<decimal, decimal> ParseTaxRates(Order order, string taxRatesStr);

        /// <summary>
        /// Gets a value indicating whether an order has items to be added to a shipment
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to be added to a shipment
        /// </returns>
        Task<bool> HasItemsToAddToShipmentAsync(Order order);

        /// <summary>
        /// Gets a value indicating whether an order has items to ship
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to ship
        /// </returns>
        Task<bool> HasItemsToShipAsync(Order order);

        /// <summary>
        /// Gets a value indicating whether there are shipment items to mark as 'ready for pickup' in order shipments.
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether there are shipment items to mark as 'ready for pickup' in order shipments.
        /// </returns>
        Task<bool> HasItemsToReadyForPickupAsync(Order order);

        /// <summary>
        /// Gets a value indicating whether an order has items to deliver
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to deliver
        /// </returns>
        Task<bool> HasItemsToDeliverAsync(Order order);

        Task<IList<Order>> GetOrdersByRfqAsync(int rfqId);

        Task<IPagedList<Order>> GetOrdersByParentIdAsync(int parentOrderId, int orderTypeId, int pageIndex = 0, int pageSize = int.MaxValue);

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
        Task<OrderItem> GetOrderItemByIdAsync(int orderItemId);

        /// <summary>
        /// Gets a product of specify order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product
        /// </returns>
        Task<Product> GetProductByOrderItemIdAsync(int orderItemId);

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
        Task<IList<OrderItem>> GetOrderItemsAsync(int orderId, bool? isNotReturnable = null, bool? isShipEnabled = null);

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemGuid">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        Task<OrderItem> GetOrderItemByGuidAsync(Guid orderItemGuid);
        Task<Order> GetOrderByQuotationIdAsync(int quotationId);

        /// <summary>
        /// Gets all downloadable order items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order items
        /// </returns>
        Task<IList<OrderItem>> GetDownloadableOrderItemsAsync(int customerId);

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteOrderItemAsync(OrderItem orderItem);

        /// <summary>
        /// Gets a total number of items in all shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the total number of items in all shipments
        /// </returns>
        Task<decimal> GetTotalNumberOfItemsInAllShipmentsAsync(OrderItem orderItem, bool loadOnlyShipped = false);

        /// <summary>
        /// Gets a total number of already items which can be added to new shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the total number of already delivered items which can be added to new shipments
        /// </returns>
        Task<decimal> GetTotalNumberOfItemsCanBeAddedToShipmentAsync(OrderItem orderItem, bool loadOnlyShipped = false);

        /// <summary>
        /// Gets a value indicating whether download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true if download is allowed; otherwise, false.
        /// </returns>
        Task<bool> IsDownloadAllowedAsync(OrderItem orderItem);

        /// <summary>
        /// Gets a value indicating whether license download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true if license download is allowed; otherwise, false.
        /// </returns>
        Task<bool> IsLicenseDownloadAllowedAsync(OrderItem orderItem);

        /// <summary>
        /// Inserts a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertOrderItemAsync(OrderItem orderItem);

        /// <summary>
        /// Updates a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateOrderItemAsync(OrderItem orderItem);

        int GetTotalOrderItemsCountAsync();

        decimal GetlOrderItemsCountByOrder(int orderId);

        #endregion

        #region Order notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order note
        /// </returns>
        Task<OrderNote> GetOrderNoteByIdAsync(int orderNoteId);

        /// <summary>
        /// Gets a list notes of order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="displayToCustomer">Value indicating whether a customer can see a note; pass null to ignore</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<OrderNote>> GetOrderNotesByOrderIdAsync(int orderId, bool? displayToCustomer = null);

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteOrderNoteAsync(OrderNote orderNote);

        /// <summary>
        /// Formats the order note text
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <returns>Formatted text</returns>
        string FormatOrderNoteText(OrderNote orderNote);

        /// <summary>
        /// Inserts an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertOrderNoteAsync(OrderNote orderNote);

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteRecurringPaymentAsync(RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payment
        /// </returns>
        Task<RecurringPayment> GetRecurringPaymentByIdAsync(int recurringPaymentId);

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertRecurringPaymentAsync(RecurringPayment recurringPayment);

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateRecurringPaymentAsync(RecurringPayment recurringPayment);

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
        Task<IPagedList<RecurringPayment>> SearchRecurringPaymentsAsync(int storeId = 0,
            int customerId = 0, int initialOrderId = 0, OrderStatus? initialOrderStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        #endregion

        #region Recurring payment history

        /// <summary>
        /// Inserts a recurring payment history entry
        /// </summary>
        /// <param name="recurringPaymentHistory">Recurring payment history entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertRecurringPaymentHistoryAsync(RecurringPaymentHistory recurringPaymentHistory);

        /// <summary>
        /// Gets a recurring payment history
        /// </summary>
        /// <param name="recurringPayment">The recurring payment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<RecurringPaymentHistory>> GetRecurringPaymentHistoryAsync(RecurringPayment recurringPayment);

        #endregion

        #region GetDollarRatePKR

        Task<decimal> GetDollarRatePKR();


        #endregion

        #region Custom Order Methods

        Task<IList<Order>> GetOrdersByRFQIdsAsync(int[] rfqIds);
        Task<Order> GetOrderByRequestIdAsync(int requestId);
        Task<Order> GetOrderByRFQIdAsync(int rfqId);

        #endregion

        #region Order Calculation

        Task<IList<OrderCalculation>> GetOrderCalculationsByIdsAsync(int[] orderCalculationIds);

        Task<OrderCalculation> GetOrderCalculationByIdAsync(int orderCalculationId);

        Task<OrderCalculation> GetOrderCalculationByOrderIdAsync(int orderId);

        Task InsertOrderCalculationAsync(OrderCalculation orderCalculation);

        Task UpdateOrderCalculationAsync(OrderCalculation orderCalculation);

        #endregion

        #region Order Contract

        Task DeleteContractAsync(Contract contract);

        Task<Contract> GetContractByIdAsync(int contractId);

        Task<Contract> GetContractByGuidAsync(Guid guid);

        Task<Contract> GetContractByOrderIdAsync(int orderId);

        Task InsertContractAsync(Contract contract);

        Task UpdateContractAsync(Contract contract);

        Task<IPagedList<Contract>> GetAllContractAsync(int orderId = 0, int buyerId = 0, int supplierId = 0, string contractType = "", int pageIndex = 0, int pageSize = int.MaxValue);


        #endregion

        #region Order Delivery Schedule

        Task InsertDeliveryScheduleAsync(OrderDeliverySchedule deliverySchedule);


        Task UpdateDeliveryScheduleAsync(OrderDeliverySchedule deliverySchedule);

        Task DeleteDeliveryScheduleAsync(OrderDeliverySchedule deliverySchedule);

        Task<OrderDeliverySchedule> GetDeliveryScheduleByIdAsync(int id);

        Task<IPagedList<OrderDeliverySchedule>> GetAllDeliveryScheduleAsync(int orderId = 0, int pageIndex = 0,
            int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        decimal GetTotalOrderDeliveryScheduleQuantityByOrderIdAsync(int orderId);
        #endregion

        #region OrderDeliveryRequest

        Task DeleteOrderDeliveryRequestAsync(OrderDeliveryRequest orderDeliveryRequest);

        Task<OrderDeliveryRequest> GetOrderDeliveryRequestByIdAsync(int orderDeliveryRequestId);

        Task InsertOrderDeliveryRequestAsync(OrderDeliveryRequest orderDeliveryRequest);

        Task UpdateOrderDeliveryRequestAsync(OrderDeliveryRequest orderDeliveryRequest);

        Task<IPagedList<OrderDeliveryRequest>> SearchOrderDeliveryRequestsAsync(int orderDeliveryScheduleId = 0, int countryId = 0,
           int cityId = 0, int areaId = 0, int CreatedBy = 0,
           string streetAddress = "", string contactNumber = "", decimal quantity = 0,
           DateTime? shipmentFromUtc = null, DateTime? shipmentToUtc = null,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           int pageIndex = 0, int pageSize = int.MaxValue, int statusId = 0, int agentId = 0, List<int> sIds = null);

        Task<OrderDeliveryRequest> GetOrderDeliveryRequestByOrderIdAsync(int orderId);
        decimal GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(int orderId, int expectedShipmentId);

        #endregion

        #region Order Sales Return Request

        Task DeleteOrderSalesReturnRequestAsync(OrderSalesReturnRequest orderSalesReturnRequest);

        Task<OrderSalesReturnRequest> GetOrderSalesReturnRequestByIdAsync(int orderSalesReturnRequestId);

        Task InsertOrderSalesReturnRequestAsync(OrderSalesReturnRequest orderSalesReturnRequest);

        Task UpdateOrderSalesReturnRequestAsync(OrderSalesReturnRequest orderSalesReturnRequest);

        Task<IPagedList<OrderSalesReturnRequest>> SearchOrderSalesReturnRequestsAsync(int orderId = 0, int supplierId = 0,
            int quotationId = 0, int createdBy = 0,
            string pickupAddress = "", string dropOffAddress = "", decimal quantity = 0,
            DateTime? returnRequestDateFromUtc = null, DateTime? returnRequestDateToUtc = null, bool? isInventory = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int agentId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, List<int> sIds = null);

        #endregion

        #region DirectOrder

        Task DeleteDirectOrderAsync(DirectOrder directOrder);
        Task<DirectOrder> GetDirectOrderByIdAsync(int directOrderId);
        Task InsertDirectOrderAsync(DirectOrder directOrder);
        Task UpdateDirectOrderAsync(DirectOrder directOrder);
        Task<IPagedList<DirectOrder>> SearchDirectOrdersAsync(int buyerId = 0, int bookerId = 0, int industryId = 0, int rfqId = 0,
             int catgoryId = 0, int productId = 0, int brandId = 0, string otherBrand = "", decimal quantity = 0, bool step1 = false,
             int pageIndex = 0, int pageSize = int.MaxValue);

        Task<DirectOrder> GetDirectOrderByRequestId(int requestId);
        Task<DirectOrder> GetDirectOrderByQuotationId(int quotationId);
        Task<DirectOrder> GetDirectOrderAsync(int bookerId, int buyerId, int industryId);

        #endregion

        #region Direct Order Delivery Schedule
        Task InsertDirectOrderDeliveryScheduleAsync(DirectOrderDeliverySchedule directOrderDeliverySchedule);
        Task UpdateDirectOrderDeliveryScheduleAsync(DirectOrderDeliverySchedule directOrderDeliverySchedule);
        Task DeleteDirectOrderDeliveryScheduleAsync(DirectOrderDeliverySchedule directOrderDeliverySchedule);
        Task<DirectOrderDeliverySchedule> GetDirectOrderDeliveryScheduleByIdAsync(int id);
        Task<IList<DirectOrderDeliverySchedule>> GetAllDirectOrderDeliveryScheduleAsync(int directOrderId);
        #endregion

        #region Direct Order Calculation

        Task InsertDirectOrderCalculationAsync(DirectOrderCalculation directOrderCalculation);
        Task UpdateDirectOrderCalculationAsync(DirectOrderCalculation directOrderCalculation);
        Task DeleteDirectOrderCalculationAsync(DirectOrderCalculation directOrderCalculation);
        Task<DirectOrderCalculation> GetDirectOrderCalculationByIdAsync(int id);
        Task<IList<DirectOrderCalculation>> GetAllDirectOrderCalculationAsync(int directOrderId);
        Task<DirectOrderCalculation> GetDirectOrderCalculationByDirectOrderIdAsync(int directOrderId);

        #endregion

        #region Order Cancellation

         Task DeleteOrderCancellationAsync(OrderCancellation orderCancellation);
         Task<OrderCancellation> GetOrderCancellationByIdAsync(int orderCancellationId);
         Task<OrderCancellation> GetOrderCancellationByOrderIdAsync(int orderId);
         Task InsertOrderCancellationAsync(OrderCancellation orderCancellation);
         Task UpdateOrderCancellationAsync(OrderCancellation orderCancellation);
         Task<IPagedList<OrderCancellation>> GetAllOrderCancellationAsync(int orderId = 0, string reasen = null, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion

        Task<string> GetUnitTypeByOrderAsync(int orderId);
    }
}