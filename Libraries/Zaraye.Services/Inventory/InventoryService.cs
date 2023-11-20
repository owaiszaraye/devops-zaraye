using LinqToDB.Data;
using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data;
using Zaraye.Services.Orders;
using Zaraye.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Inventory
{
    public partial class InventoryService : IInventoryService
    {
        #region Fields

        private readonly IRepository<InventoryOutbound> _inventoryOutboundRepository;
        private readonly IRepository<InventoryInbound> _inventoryInboundRepository;
        private readonly IRepository<InventoryGroup> _inventoryGroupRepository;
        private readonly IRepository<CogsInventoryTagging> _cogsInventoryTaggingRepository;
        private readonly IRepository<DirectCogsInventoryTagging> _directCogsInventoryTaggingRepository;
        private readonly IZarayeDataProvider _zarayeDataProvider;
        private readonly IOrderService _orderService;
        private readonly IRequestService _requestService;
        private readonly IShipmentService _shipmentService;
        private readonly IQuotationService _quotationService;
        private readonly IShippingService _shippingService;

        #endregion

        #region Ctor

        public InventoryService(
            IRepository<InventoryInbound> inventoryInboundRepository,
            IRepository<InventoryOutbound> inventoryOutboundRepository,
            IRepository<InventoryGroup> inventoryGroupRepository,
            IRepository<CogsInventoryTagging> cogsInventoryTaggingRepository,
            IRepository<DirectCogsInventoryTagging> directCogsInventoryTaggingRepository,
            IZarayeDataProvider zarayeDataProvider,
            IOrderService orderService,
            IRequestService requestService,
            IShipmentService shipmentService,
            IQuotationService quotationService,
            IShippingService shippingService
            )
        {
            _inventoryInboundRepository = inventoryInboundRepository;
            _inventoryOutboundRepository = inventoryOutboundRepository;
            _inventoryGroupRepository = inventoryGroupRepository;
            _cogsInventoryTaggingRepository = cogsInventoryTaggingRepository;
            _directCogsInventoryTaggingRepository = directCogsInventoryTaggingRepository;
            _zarayeDataProvider = zarayeDataProvider;
            _orderService = orderService;
            _requestService = requestService;
            _shipmentService = shipmentService;
            _quotationService = quotationService;
            _shippingService = shippingService;
        }

        #endregion

        #region Inventory Group

        public virtual async Task DeleteInventoryGroupAsync(InventoryGroup inventoryGroup)
        {
            await _inventoryGroupRepository.DeleteAsync(inventoryGroup);
        }

        public virtual async Task UpdateInventoryGroupAsync(InventoryGroup inventoryGroup)
        {
            await _inventoryGroupRepository.UpdateAsync(inventoryGroup);
        }

        public virtual async Task InsertInventoryGroupAsync(InventoryGroup inventoryGroup)
        {
            await _inventoryGroupRepository.InsertAsync(inventoryGroup);
        }

        public virtual async Task<InventoryGroup> GetInventoryGroupByIdAsync(int inventoryGroupId)
        {
            return await _inventoryGroupRepository.GetByIdAsync(inventoryGroupId);
        }

        public virtual async Task<IList<InventoryGroup>> GetAllInventoryGroupsAsync(int groupId = 0, int productId = 0, int brandId = 0)
        {
            var inventoryGroups = await _inventoryGroupRepository.GetAllPagedAsync(query =>
            {
                if (groupId > 0)
                    query = query.Where(i => i.Id == groupId);
                if (brandId > 0)
                    query = query.Where(i => i.BrandId == brandId);
                if (productId > 0)
                    query = query.Where(i => i.ProductId == productId);

                query = query.OrderByDescending(i => i.ProductId);

                return query;
            });

            return inventoryGroups;
        }

        public virtual async Task<IPagedList<InventoryGroup>> GetInventoryGroupsAsync(int groupId = 0, int productId = 0, int brandId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var inventoryGroups = await _inventoryGroupRepository.GetAllPagedAsync(query =>
            {
                if (groupId > 0)
                    query = query.Where(i => i.Id == groupId);
                if (brandId > 0)
                    query = query.Where(i => i.BrandId == brandId);
                if (productId > 0)
                    query = query.Where(i => i.ProductId == productId);

                query = query.OrderByDescending(i => i.ProductId);
                return query;
            }, pageIndex, pageSize);

            return inventoryGroups;
        }

        public virtual async Task<InventoryGroup> GetInventoryGroupByBrandIdAndProductIdAsync(int productId = 0, int brandId = 0, string productAttributeXml = null)
        {
            return await (from inv in _inventoryGroupRepository.Table
                          where inv.BrandId == brandId &&
                          inv.ProductId == productId && inv.ProductAttributesXml == productAttributeXml
                          select inv).FirstOrDefaultAsync();
        }

        #endregion

        #region Inventory Inbound

        public virtual async Task DeleteInventoryInboundAsync(InventoryInbound inventoryInbound)
        {
            await _inventoryInboundRepository.DeleteAsync(inventoryInbound);
        }

        public virtual async Task UpdateInventoryInboundAsync(InventoryInbound inventoryInbound)
        {
            await _inventoryInboundRepository.UpdateAsync(inventoryInbound);
        }

        public virtual async Task InsertInventoryInboundAsync(InventoryInbound inventoryInbound)
        {
            //Add Group
            var inventoryGroup = await GetInventoryGroupByBrandIdAndProductIdAsync(inventoryInbound.ProductId, inventoryInbound.BrandId, inventoryInbound.ProductAttributesXml);
            if (inventoryGroup is null)
            {
                inventoryGroup = new InventoryGroup
                {
                    ProductAttributesXml = inventoryInbound.ProductAttributesXml,
                    ProductId = inventoryInbound.ProductId,
                    BrandId = inventoryInbound.BrandId,
                };
                await InsertInventoryGroupAsync(inventoryGroup);
            }

            //add inventory group Id
            inventoryInbound.InventoryGroupId = inventoryGroup.Id;
            await _inventoryInboundRepository.InsertAsync(inventoryInbound);

            //update last inventory inbound id
            inventoryGroup.LastInventoryId = inventoryInbound.Id;
            await UpdateInventoryGroupAsync(inventoryGroup);
        }

        public virtual async Task<InventoryInbound> GetInventoryInboundByIdAsync(int inventoryInboundId)
        {
            return await _inventoryInboundRepository.GetByIdAsync(inventoryInboundId);
        }

        public virtual async Task<IList<InventoryInbound>> GetInventoryInboundByIdsAsync(int[] inventoryInboundId)
        {
            return await _inventoryInboundRepository.GetByIdsAsync(inventoryInboundId, cache => default, false);
        }

        public virtual async Task<IList<InventoryInbound>> GetAllInventoryInboundsAsync(int groupId = 0, int warehouseId = 0,
                  int inventoryTypeId = 0, int inventoryStatusId = 0, int industryId = 0, int categoryId = 0, int productId = 0, int brandId = 0, int shipmentNumber = 0,
                  DateTime? startDate = null, DateTime? endDate = null, int InboundStatusId = 0, bool isInboundStatus = false, bool groupby = false,
                  int purchaseOrderId = 0, int supplierId = 0, List<int> sIds = null)
        {
            var inventoryInbounds = await _inventoryInboundRepository.GetAllPagedAsync(query =>
            {
                if (groupId > 0)
                    query = query.Where(i => i.InventoryGroupId == groupId);
                if (purchaseOrderId > 0)
                    query = query.Where(i => i.PurchaseOrderId == purchaseOrderId);
                if (warehouseId > 0)
                    query = query.Where(i => i.WarehouseId == warehouseId);
                if (inventoryTypeId > 0)
                    query = query.Where(i => i.InventoryTypeId == inventoryTypeId);
                if (inventoryStatusId > 0)
                    query = query.Where(i => i.InventoryInboundStatusId == inventoryStatusId);
                if (industryId > 0)
                    query = query.Where(i => i.IndustryId == industryId);
                if (categoryId > 0)
                    query = query.Where(i => i.CategoryId == categoryId);
                if (brandId > 0)
                    query = query.Where(i => i.BrandId == brandId);
                if (productId > 0)
                    query = query.Where(i => i.ProductId == productId);
                if (shipmentNumber > 0)
                    query = query.Where(i => i.ShipmentId == shipmentNumber);
                if (supplierId > 0)
                    query = query.Where(i => i.SupplierId == supplierId);
                if (sIds != null && sIds.Any())
                    query = query.Where(i => sIds.Contains(i.ShipmentId.Value));

                if (startDate.HasValue)
                    query = query.Where(o => startDate.Value <= o.CreatedOnUtc);

                if (endDate.HasValue)
                    query = query.Where(o => endDate.Value >= o.CreatedOnUtc);

                if (InboundStatusId > 0)
                    query = query.Where(o => o.InventoryInboundStatusId == InboundStatusId);

                if (isInboundStatus)
                    query = query.Where(i => i.InventoryInboundStatusId == (int)InventoryInboundStatusEnum.Booked || i.InventoryInboundStatusId == (int)InventoryInboundStatusEnum.InTransit || i.InventoryInboundStatusId == (int)InventoryInboundStatusEnum.Physical /*&& i.InventoryInboundStatusId != (int)InventoryInboundStatusEnum.Sold*/);

                query = query.OrderByDescending(i => i.ProductId);

                if (groupby)
                {
                    query = query.Select(x => new { x }).GroupBy(x => new { x.x.InventoryInboundStatusId }).Select(x => new InventoryInbound()
                    {
                        InventoryInboundStatusId = x.FirstOrDefault().x.InventoryInboundStatusId,
                    });
                }

                return query;
            });

            return inventoryInbounds;
        }

        public virtual async Task<IList<InventoryInbound>> GetAllInventoryInboundsByGroupIdsAsync(int[] groupIds = null, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var inventoryInbounds = await _inventoryInboundRepository.GetAllPagedAsync(query =>
            {
                if (groupIds != null && groupIds.Length > 0)
                    query = query.Where(i => groupIds.Contains(i.InventoryGroupId));

                if (startDate.HasValue)
                    query = query.Where(o => startDate.Value <= o.CreatedOnUtc);

                if (endDate.HasValue)
                    query = query.Where(o => endDate.Value >= o.CreatedOnUtc);

                return query;
            });

            return inventoryInbounds;
        }

        public virtual InventoryInbound GetInventoryInboundByShipmentId(int shipmentNumber = 0, int inventoryInboundStatusId = 0)
        {
            var query = from inv in _inventoryInboundRepository.Table where inv.ShipmentId == shipmentNumber select inv;

            if (inventoryInboundStatusId > 0)
                query = query.Where(x => x.InventoryInboundStatusId == inventoryInboundStatusId);

            return query.FirstOrDefault();
        }

        public virtual async Task<IPagedList<InventoryInboundList>> GetAllInboundInventoriesAsync(int warehouseId = 0, int inventoryTypeId = 0, int inventoryStatusId = 0,
            int industryId = 0, int categoryId = 0, int productId = 0, int brandId = 0, int shipmentNumber = 0, DateTime? startDate = null, DateTime? endDate = null, int supplierId = 0,
            int businessModelId = 0, bool showBrokerInvetory = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var inventories = await _zarayeDataProvider.QueryProcAsync<InventoryInboundList>("get_group_invetories",
                new DataParameter("@p_warehouseId", warehouseId > 0 ? warehouseId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_inventoryTypeId", inventoryTypeId > 0 ? inventoryTypeId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_inventoryStatusId", inventoryStatusId > 0 ? inventoryStatusId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_industryId", industryId > 0 ? industryId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_categoryId", categoryId > 0 ? categoryId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_productId", productId > 0 ? productId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_brandId", brandId > 0 ? brandId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_supplierId", supplierId > 0 ? supplierId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_businessModelId", businessModelId > 0 ? businessModelId.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_shipmentNumber", shipmentNumber > 0 ? shipmentNumber.ToString() : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_startDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_endDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ShowBrokerInvetory", showBrokerInvetory, LinqToDB.DataType.Boolean),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await inventories.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<InventoryInboundList>> GetAllDetailInboundInventoriesAsync(int groupId = 0, int warehouseId = 0, int inventoryTypeId = 0, int inventoryStatusId = 0,
           int industryId = 0, int categoryId = 0, int productId = 0, int brandId = 0, int shipmentNumber = 0, int supplierId = 0, int businessModelId = 0,
           DateTime? startDate = null, DateTime? endDate = null, bool showBrokerInvetory = false,
           int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var inventories = await _zarayeDataProvider.QueryProcAsync<InventoryInboundList>("get_detail_inventories",
                           new DataParameter("@p_groupId", groupId, LinqToDB.DataType.Int32),
                           new DataParameter("@p_warehouseId", warehouseId > 0 ? warehouseId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_inventoryTypeId", inventoryTypeId > 0 ? inventoryTypeId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_inventoryStatusId", inventoryStatusId > 0 ? inventoryStatusId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_industryId", industryId > 0 ? industryId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_categoryId", categoryId > 0 ? categoryId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_productId", productId > 0 ? productId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_brandId", brandId > 0 ? brandId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_supplierId", supplierId > 0 ? supplierId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_businessModelId", businessModelId > 0 ? businessModelId.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_shipmentNumber", shipmentNumber > 0 ? shipmentNumber.ToString() : null, LinqToDB.DataType.Int32),
                           new DataParameter("@p_startDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                           new DataParameter("@p_endDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                           new DataParameter("@p_ShowBrokerInvetory", showBrokerInvetory, LinqToDB.DataType.Boolean),
                           new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                           new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                           totalRecordsParam);

            return await inventories.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task AddInventoryInboundAsync(Order purchaseOrder, RequestForQuotation requestForQuotation, Shipment shipment)
        {
            if (purchaseOrder is null)
                throw new ArgumentNullException(nameof(purchaseOrder) + " is null ");

            if (shipment is null)
                throw new ArgumentNullException(nameof(shipment) + " is null ");

            if (requestForQuotation is null)
                throw new ArgumentNullException(nameof(requestForQuotation) + " is null ");

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                throw new ArgumentNullException(nameof(request) + " is null ");

            var orderItem = (await _orderService.GetOrderItemsAsync(purchaseOrder.Id)).FirstOrDefault();
            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem) + " is null ");

            var orderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(purchaseOrder.Id);
            if (orderCalculation is null)
                throw new ArgumentNullException(nameof(orderCalculation) + " is null ");

            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault(x => x.OrderItemId == orderItem.Id);
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem) + " is null ");

            var requestRfqQuotationMapping = await _requestService.GetRequestRfqQuotationMappingByOrderAsync(purchaseOrder.Id);
            if (requestRfqQuotationMapping == null)
                throw new ArgumentNullException(nameof(requestRfqQuotationMapping) + " is null ");

            var quotation = await _quotationService.GetQuotationByIdAsync(requestRfqQuotationMapping.QuotationId);
            if (quotation == null)
                throw new ArgumentNullException(nameof(quotation) + " is null ");

            var warehouse = await _shippingService.GetWarehouseByIdAsync(shipment.WarehouseId);
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse) + " is null ");

            var finalRate = orderCalculation.SubTotal / orderItem.Quantity;

            await InsertInventoryInboundAsync(new InventoryInbound
            {
                PurchaseOrderId = purchaseOrder.Id,
                SupplierId = purchaseOrder.CustomerId,
                ShipmentId = shipment.Id,
                IndustryId = request.IndustryId,
                CategoryId = request.CategoryId,
                ProductId = request.ProductId,
                ProductAttributesXml = request.ProductAttributeXml,
                BrandId = quotation.BrandId,
                WarehouseId = shipment.WarehouseId,
                StockQuantity = shipmentItem.Quantity,
                PurchaseRate = finalRate,
                TotalPurchaseValue = finalRate * shipmentItem.Quantity,
                InventoryInboundStatusEnum = warehouse.SupplierId.HasValue ? InventoryInboundStatusEnum.Virtual : InventoryInboundStatusEnum.Booked,
                InventoryTypeEnum = InventoryTypeEnum.Purchase,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                //GstRate = orderCalculation.GSTRate,
                //GstAmount = orderCalculation.GSTAmount,
                //WhtRate = orderCalculation.WHTRate,
                //WhtAmount = orderCalculation.WHTAmount,
                //WholeSaleTaxRate = orderCalculation.WholesaleTaxRate,
                //WholeSaleTaxAmount = orderCalculation.WholesaleTaxAmount,
                BusinessModelId = orderCalculation.BusinessModelId
            });
        }

        public virtual async Task AddInventoryInboundReturnAsync(decimal inboundQuantity, Order returnSaleOrder, Request request, Shipment shipment)
        {
            if (returnSaleOrder is null)
                throw new ArgumentNullException(nameof(returnSaleOrder) + " is null ");

            if (shipment is null)
                throw new ArgumentNullException(nameof(shipment) + " is null ");

            if (request is null)
                throw new ArgumentNullException(nameof(request) + " is null ");

            var orderItem = (await _orderService.GetOrderItemsAsync(returnSaleOrder.Id)).FirstOrDefault();
            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem) + " is null ");

            var orderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(returnSaleOrder.Id);
            if (orderCalculation is null)
                throw new ArgumentNullException(nameof(orderCalculation) + " is null ");

            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault(x => x.OrderItemId == orderItem.Id);
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem) + " is null ");

            var finalRate = orderCalculation.SubTotal / orderItem.Quantity;

            await InsertInventoryInboundAsync(new InventoryInbound
            {
                PurchaseOrderId = returnSaleOrder.Id,
                SupplierId = returnSaleOrder.CustomerId,
                ShipmentId = shipment.Id,
                IndustryId = request.IndustryId,
                CategoryId = request.CategoryId,
                ProductId = request.ProductId,
                ProductAttributesXml = request.ProductAttributeXml,
                BrandId = request.BrandId,
                WarehouseId = shipment.WarehouseId,
                StockQuantity = inboundQuantity,
                PurchaseRate = finalRate,
                TotalPurchaseValue = finalRate * inboundQuantity,
                InventoryInboundStatusEnum = InventoryInboundStatusEnum.Physical,
                InventoryTypeEnum = InventoryTypeEnum.SalesReturn,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                GstRate = orderCalculation.GSTRate,
                GstAmount = orderCalculation.GSTAmount,
                WhtRate = orderCalculation.WHTRate,
                WhtAmount = orderCalculation.WHTAmount,
                WholeSaleTaxRate = orderCalculation.WholesaleTaxRate,
                WholeSaleTaxAmount = orderCalculation.WholesaleTaxAmount,
                BusinessModelId = orderCalculation.BusinessModelId
            });
        }


        #endregion

        #region Inventory Outbound

        public virtual async Task DeleteInventoryOutboundAsync(InventoryOutbound inventoryOutbound)
        {
            await _inventoryOutboundRepository.DeleteAsync(inventoryOutbound);
        }

        public virtual async Task<IPagedList<InventoryOutbound>> GetAllInventoryOutboundsAsync(int saleOrderId = 0, int InventoryInboundId = 0, int groupId = 0, int shipmentId = 0, bool excludeCancelled = true, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var inventoryOutbounds = await _inventoryOutboundRepository.GetAllPagedAsync(query =>
            {
                if (excludeCancelled)
                    query = query.Where(i => i.InventoryOutboundStatusId != (int)InventoryOutboundStatusEnum.Cancelled);

                if (saleOrderId > 0)
                    query = query.Where(i => i.SaleOrderId == saleOrderId);

                if (InventoryInboundId > 0)
                    query = query.Where(i => i.InventoryInboundId == InventoryInboundId);

                if (groupId > 0)
                    query = query.Where(i => i.InventoryGroupId == groupId);

                if (shipmentId > 0)
                    query = query.Where(i => i.ShipmentId == shipmentId);

                query = query.OrderByDescending(i => i.InventoryInboundId);

                return query;
            });

            //paging
            return new PagedList<InventoryOutbound>(inventoryOutbounds, pageIndex, pageSize);
        }

        public virtual decimal GetTotalInventoryOutboundQuantityAsync(int InventoryInboundId = 0)
        {
            return (from invout in _inventoryOutboundRepository.Table
                    where invout.InventoryInboundId == InventoryInboundId
                    select invout).Sum(x => x.OutboundQuantity);
        }

        public virtual async Task<InventoryOutbound> GetInventoryOutboundByIdAsync(int inventoryOutboundId)
        {
            return await _inventoryOutboundRepository.GetByIdAsync(inventoryOutboundId);
        }

        public virtual async Task InsertInventoryOutboundAsync(InventoryOutbound inventoryOutbound)
        {
            await _inventoryOutboundRepository.InsertAsync(inventoryOutbound);
        }

        public virtual async Task UpdateInventoryOutboundAsync(InventoryOutbound inventoryOutbound)
        {
            await _inventoryOutboundRepository.UpdateAsync(inventoryOutbound);
        }

        public virtual async Task AddInventoryPurchaseReturnOutboundAsync(InventoryInbound inventoryInbound, Order purchaseOrder, Shipment shipment, OrderItem orderItem, decimal outBoundQty)
        {
            if (inventoryInbound is null)
                throw new ArgumentNullException(nameof(inventoryInbound) + " is null ");

            if (purchaseOrder is null)
                throw new ArgumentNullException(nameof(purchaseOrder) + " is null ");

            if (shipment is null)
                throw new ArgumentNullException(nameof(shipment) + " is null ");

            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem) + " is null ");

            var inventoryOutbound = new InventoryOutbound
            {
                InventoryInboundId = inventoryInbound.Id,
                InventoryGroupId = inventoryInbound.InventoryGroupId,
                SaleOrderId = purchaseOrder.Id,
                ShipmentId = shipment.Id,
                OrderItemId = orderItem.Id,
                OutboundQuantity = outBoundQty,
                UpdatedById = 0,
                Deleted = false,
                DeletedById = 0,
                InventoryOutboundTypeId = (int)InventoryOutboundTypeEnum.PurchaseReturn,
                InventoryOutboundStatusEnum = InventoryOutboundStatusEnum.Active,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            await InsertInventoryOutboundAsync(inventoryOutbound);
        }
        public virtual async Task AddInventorySaleReturnOutboundAsync(InventoryInbound inventoryInbound, Order purchaseOrder, Shipment shipment, OrderItem orderItem, decimal outBoundQty)
        {
            if (inventoryInbound is null)
                throw new ArgumentNullException(nameof(inventoryInbound) + " is null ");

            if (purchaseOrder is null)
                throw new ArgumentNullException(nameof(purchaseOrder) + " is null ");

            if (shipment is null)
                throw new ArgumentNullException(nameof(shipment) + " is null ");

            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem) + " is null ");

            var inventoryOutbound = new InventoryOutbound
            {
                InventoryInboundId = inventoryInbound.Id,
                InventoryGroupId = inventoryInbound.InventoryGroupId,
                SaleOrderId = purchaseOrder.Id,
                ShipmentId = shipment.Id,
                OrderItemId = orderItem.Id,
                OutboundQuantity = outBoundQty,
                UpdatedById = 0,
                Deleted = false,
                DeletedById = 0,
                InventoryOutboundTypeId = (int)InventoryOutboundTypeEnum.SaleReturn,
                InventoryOutboundStatusEnum = InventoryOutboundStatusEnum.Active,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            await InsertInventoryOutboundAsync(inventoryOutbound);
        }

        public virtual async Task AddInventoryOutboundAsync(InventoryInbound inventoryInbound, Order saleOrder, Shipment shipment, OrderItem orderItem, decimal outBoundQty)
        {
            if (inventoryInbound is null)
                throw new ArgumentNullException(nameof(inventoryInbound) + " is null ");

            if (saleOrder is null)
                throw new ArgumentNullException(nameof(saleOrder) + " is null ");

            if (shipment is null)
                throw new ArgumentNullException(nameof(shipment) + " is null ");

            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem) + " is null ");

            var inventoryOutbound = new InventoryOutbound
            {
                InventoryInboundId = inventoryInbound.Id,
                InventoryGroupId = inventoryInbound.InventoryGroupId,
                SaleOrderId = saleOrder.Id,
                ShipmentId = shipment.Id,
                OrderItemId = orderItem.Id,
                OutboundQuantity = outBoundQty,
                UpdatedById = 0,
                Deleted = false,
                DeletedById = 0,
                InventoryOutboundTypeId = (int)InventoryOutboundTypeEnum.Sale,
                InventoryOutboundStatusEnum = InventoryOutboundStatusEnum.Active,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            await InsertInventoryOutboundAsync(inventoryOutbound);

            var outboundQty = (await GetAllInventoryOutboundsAsync(InventoryInboundId: inventoryInbound.Id)).Sum(x => x.OutboundQuantity);
            var balanceQuantity = inventoryInbound.StockQuantity - outboundQty;
            if (balanceQuantity <= 0)
            {
                inventoryInbound.InventoryInboundStatusId = (int)InventoryInboundStatusEnum.Sold;
                await UpdateInventoryInboundAsync(inventoryInbound);
            }
        }

        public virtual async Task<decimal> GetPurchaseOrderRemainigStockQuantity(Order purchaseOrder)
        {
            if (purchaseOrder is null)
                throw new ArgumentException("Order is null", nameof(purchaseOrder));

            var totalPurchaseOrderInventoryQuantity = 0M;
            var request = await _requestService.GetRequestByIdAsync(purchaseOrder.RequestId);
            if (request is not null)
            {
                var inventoryGroup = await GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
                if (inventoryGroup is not null)
                {
                    var inventoryInbounds = (await GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id, purchaseOrderId: purchaseOrder.Id)).OrderBy(x => x.Id).ToList();
                    foreach (var inventoryInbound in inventoryInbounds)
                    {
                        var outboundQty = (await GetAllInventoryOutboundsAsync(InventoryInboundId: inventoryInbound.Id)).Sum(x => x.OutboundQuantity);
                        totalPurchaseOrderInventoryQuantity += inventoryInbound.StockQuantity - outboundQty;
                    }
                }
            }
            return totalPurchaseOrderInventoryQuantity;
        }

        public decimal GetInventoryOutboundQuantity(int saleOrderId)
        {
            return (from invout in _inventoryOutboundRepository.Table
                    where invout.SaleOrderId == saleOrderId && invout.InventoryOutboundStatusId != (int)InventoryOutboundStatusEnum.Cancelled
                    select invout).Sum(x => x.OutboundQuantity);
        }

        public decimal GetInventoryOutboundStockQuantityByInventoryInboundId(int saleOrderId, int inventoryInboundId)
        {
            return (from invout in _inventoryOutboundRepository.Table
                    where invout.SaleOrderId == saleOrderId && invout.InventoryInboundId == inventoryInboundId
                    && invout.InventoryOutboundTypeId == (int)InventoryOutboundTypeEnum.Sale
                    select invout).Sum(x => x.OutboundQuantity);
        }

        #endregion

        #region Cogs Inventory Tagging

        public virtual async Task DeleteCogsInventoryTaggingAsync(CogsInventoryTagging cogsInventoryTagging)
        {
            await _cogsInventoryTaggingRepository.DeleteAsync(cogsInventoryTagging);
        }

        public virtual async Task UpdateCogsInventoryTaggingAsync(CogsInventoryTagging cogsInventoryTagging)
        {
            await _cogsInventoryTaggingRepository.UpdateAsync(cogsInventoryTagging);
        }

        public virtual async Task InsertCogsInventoryTaggingAsync(CogsInventoryTagging cogsInventoryTagging)
        {
            await _cogsInventoryTaggingRepository.InsertAsync(cogsInventoryTagging);
        }

        public virtual async Task<CogsInventoryTagging> GetCogsInventoryTaggingByIdAsync(int cogsInventoryTaggingId)
        {
            return await _cogsInventoryTaggingRepository.GetByIdAsync(cogsInventoryTaggingId);
        }

        public virtual async Task<IList<CogsInventoryTagging>> GetCogsInventoryTaggingByIdsAsync(int[] cogsInventoryTaggingId)
        {
            return await _cogsInventoryTaggingRepository.GetByIdsAsync(cogsInventoryTaggingId, cache => default, false);
        }

        public virtual async Task<IPagedList<CogsInventoryTagging>> GetAllCogsInventoryTaggingsAsync(int inventoryId = 0, int requestId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _cogsInventoryTaggingRepository.Table;

            if (inventoryId > 0)
                query = query.Where(i => i.InventoryId == inventoryId);

            if (requestId > 0)
                query = query.Where(i => i.RequestId == requestId);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IList<CogsInventoryTagging>> GetCogsInventoryTaggingsByInventoryIdsAsync(int[] inventoryIds = null)
        {
            var query = _cogsInventoryTaggingRepository.Table;

            if (inventoryIds != null && inventoryIds.Length > 0)
                query = query.Where(i => inventoryIds.Contains(i.InventoryId));

            return await query.ToListAsync();
        }

        #endregion

        #region Direct Cogs Inventory Tagging

        public virtual async Task DeleteDirectCogsInventoryTaggingAsync(DirectCogsInventoryTagging cogsInventoryTagging)
        {
            await _directCogsInventoryTaggingRepository.DeleteAsync(cogsInventoryTagging);
        }

        public virtual async Task UpdateDirectCogsInventoryTaggingAsync(DirectCogsInventoryTagging cogsInventoryTagging)
        {
            await _directCogsInventoryTaggingRepository.UpdateAsync(cogsInventoryTagging);
        }

        public virtual async Task InsertDirectCogsInventoryTaggingAsync(DirectCogsInventoryTagging cogsInventoryTagging)
        {
            await _directCogsInventoryTaggingRepository.InsertAsync(cogsInventoryTagging);
        }

        public virtual async Task<DirectCogsInventoryTagging> GetDirectCogsInventoryTaggingByIdAsync(int cogsInventoryTaggingId)
        {
            return await _directCogsInventoryTaggingRepository.GetByIdAsync(cogsInventoryTaggingId);
        }

        public virtual async Task<IList<DirectCogsInventoryTagging>> GetDirectCogsInventoryTaggingByIdsAsync(int[] cogsInventoryTaggingId)
        {
            return await _directCogsInventoryTaggingRepository.GetByIdsAsync(cogsInventoryTaggingId, cache => default, false);
        }

        public virtual async Task<IPagedList<DirectCogsInventoryTagging>> GetAllDirectCogsInventoryTaggingsAsync(int inventoryId = 0, int requestId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _directCogsInventoryTaggingRepository.Table;

            if (inventoryId > 0)
                query = query.Where(i => i.InventoryId == inventoryId);

            if (requestId > 0)
                query = query.Where(i => i.RequestId == requestId);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion
    }
}
