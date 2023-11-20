using Zaraye.Core;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.Inventory
{
    public partial interface IInventoryService
    {
        #region Inventory


        #endregion

        #region Inventory Outbound

        /// <summary>
        /// Delete inventory outbound
        /// </summary>
        /// <param name="inventoryOutbound">InventoryOutbound</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteInventoryOutboundAsync(InventoryOutbound inventoryOutbound);

        /// <summary>
        /// Gets all inventory outbounds
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IPagedList<InventoryOutbound>> GetAllInventoryOutboundsAsync(int saleOrderId = 0, int InventoryInboundId = 0, int groupId = 0, int shipmentId = 0, bool excludeCancelled = true, int pageIndex = 0, int pageSize = int.MaxValue);

        decimal GetTotalInventoryOutboundQuantityAsync(int InventoryInboundId = 0);
        /// <summary>
        /// Gets a inventory outbound
        /// </summary>
        /// <param name="inventoryOutboundId">inventory outbound identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the industry
        /// </returns>
        Task<InventoryOutbound> GetInventoryOutboundByIdAsync(int inventoryOutboundId);

        /// <summary>
        /// Inserts inventory outbound
        /// </summary>
        /// <param name="inventoryOutboundId">inventory outbound</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertInventoryOutboundAsync(InventoryOutbound inventoryOutbound);

        /// <summary>
        /// Updates the inventory outbound
        /// </summary>
        /// <param name="inventoryOutboundId">inventory outbound</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateInventoryOutboundAsync(InventoryOutbound inventoryOutbound);

        Task AddInventoryOutboundAsync(InventoryInbound inventoryInbound, Order saleOrder, Shipment shipment, OrderItem orderItem, decimal outBoundQty);
        Task AddInventoryPurchaseReturnOutboundAsync(InventoryInbound inventoryInbound, Order purchaseOrder, Shipment shipment, OrderItem orderItem, decimal outBoundQty);
        Task AddInventorySaleReturnOutboundAsync(InventoryInbound inventoryInbound, Order purchaseOrder, Shipment shipment, OrderItem orderItem, decimal outBoundQty);
        #endregion

        #region Inventory Inbound

        /// <summary>
        /// Delete inventory inbound
        /// </summary>
        /// <param name="inventoryInbound">InventoryInbound</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteInventoryInboundAsync(InventoryInbound inventoryInbound);

        /// <summary>
        /// Gets a inventory inbound
        /// </summary>
        /// <param name="inventoryInboundId">inventory inbound identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the industry
        /// </returns>
        Task<InventoryInbound> GetInventoryInboundByIdAsync(int inventoryInboundId);

        Task<IList<InventoryInbound>> GetInventoryInboundByIdsAsync(int[] inventoryInboundId);
        /// <summary>
        /// Inserts inventory inbound
        /// </summary>
        /// <param name="inventoryInboundId">inventory inbound</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<IList<InventoryInbound>> GetAllInventoryInboundsAsync(int groupId = 0, int warehouseId = 0,
                  int inventoryTypeId = 0, int inventoryStatusId = 0, int industryId = 0, int categoryId = 0, int productId = 0, int brandId = 0, int shipmentNumber = 0,
                  DateTime? startDate = null, DateTime? endDate = null, int InboundStatusId = 0, bool isInboundStatus = false, bool groupby = false,
                  int purchaseOrderId = 0, int supplierId = 0, List<int> sIds = null);

        Task<IList<InventoryInbound>> GetAllInventoryInboundsByGroupIdsAsync(int[] groupIds = null, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        InventoryInbound GetInventoryInboundByShipmentId(int shipmentNumber = 0, int inventoryInboundStatusId = 0);

        /// <summary>
        /// Updates the inventory inbound
        /// </summary>
        /// <param name="inventoryInboundId">inventory inbound</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateInventoryInboundAsync(InventoryInbound inventoryInbound);
        Task InsertInventoryInboundAsync(InventoryInbound inventoryInbound);
        Task<IPagedList<InventoryInboundList>> GetAllInboundInventoriesAsync(int warehouseId = 0, int inventoryTypeId = 0, int inventoryStatusId = 0,
            int industryId = 0, int categoryId = 0, int productId = 0, int brandId = 0, int shipmentNumber = 0, DateTime? startDate = null, DateTime? endDate = null, int supplierId = 0,
            int businessModelId = 0, bool showBrokerInvetory = false, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<InventoryInboundList>> GetAllDetailInboundInventoriesAsync(int groupId = 0, int warehouseId = 0, int inventoryTypeId = 0, int inventoryStatusId = 0,
           int industryId = 0, int categoryId = 0, int productId = 0, int brandId = 0, int shipmentNumber = 0, int supplierId = 0, int businessModelId = 0,
           DateTime? startDate = null, DateTime? endDate = null, bool showBrokerInvetory = false,
           int pageIndex = 0, int pageSize = int.MaxValue);

        Task AddInventoryInboundAsync(Order purchaseOrder, RequestForQuotation requestForQuotation, Shipment shipment);
        Task AddInventoryInboundReturnAsync(decimal inboundQuantity, Order returnSaleOrder, Request request, Shipment shipment);
        Task<decimal> GetPurchaseOrderRemainigStockQuantity(Order purchaseOrder);
        decimal GetInventoryOutboundQuantity(int saleOrderId);

        decimal GetInventoryOutboundStockQuantityByInventoryInboundId(int saleOrderId, int inventoryInboundId);
        #endregion

        #region Inventory Group

        Task DeleteInventoryGroupAsync(InventoryGroup inventoryGroup);
        Task UpdateInventoryGroupAsync(InventoryGroup inventoryGroup);
        Task InsertInventoryGroupAsync(InventoryGroup inventoryGroup);
        Task<InventoryGroup> GetInventoryGroupByIdAsync(int inventoryGroupId);
        Task<IList<InventoryGroup>> GetAllInventoryGroupsAsync(int groupId = 0, int productId = 0, int brandId = 0);
        Task<IPagedList<InventoryGroup>> GetInventoryGroupsAsync(int groupId = 0, int productId = 0, int brandId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<InventoryGroup> GetInventoryGroupByBrandIdAndProductIdAsync(int productId = 0, int brandId = 0, string productAttributeXml = null);

        #endregion

        #region Cogs Inventory Tagging

        Task DeleteCogsInventoryTaggingAsync(CogsInventoryTagging cogsInventoryTagging);
        Task UpdateCogsInventoryTaggingAsync(CogsInventoryTagging cogsInventoryTagging);
        Task InsertCogsInventoryTaggingAsync(CogsInventoryTagging cogsInventoryTagging);
        Task<CogsInventoryTagging> GetCogsInventoryTaggingByIdAsync(int cogsInventoryTaggingId);
        Task<IList<CogsInventoryTagging>> GetCogsInventoryTaggingByIdsAsync(int[] cogsInventoryTaggingId);
        Task<IPagedList<CogsInventoryTagging>> GetAllCogsInventoryTaggingsAsync(int inventoryId = 0, int requestId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<IList<CogsInventoryTagging>> GetCogsInventoryTaggingsByInventoryIdsAsync(int[] inventoryIds = null);

        #endregion

        #region Direct Cogs Inventory Tagging

        Task DeleteDirectCogsInventoryTaggingAsync(DirectCogsInventoryTagging cogsInventoryTagging);
        Task UpdateDirectCogsInventoryTaggingAsync(DirectCogsInventoryTagging cogsInventoryTagging);
        Task InsertDirectCogsInventoryTaggingAsync(DirectCogsInventoryTagging cogsInventoryTagging);
        Task<DirectCogsInventoryTagging> GetDirectCogsInventoryTaggingByIdAsync(int cogsInventoryTaggingId);
        Task<IList<DirectCogsInventoryTagging>> GetDirectCogsInventoryTaggingByIdsAsync(int[] cogsInventoryTaggingId);
        Task<IPagedList<DirectCogsInventoryTagging>> GetAllDirectCogsInventoryTaggingsAsync(int inventoryId = 0, int requestId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion
    }
}
