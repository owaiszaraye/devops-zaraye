using FluentMigrator;
using LinqToDB.Mapping;
using Zaraye.Core;
using Zaraye.Core.Domain.Blogs;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Configuration;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Directory;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Logging;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.Messages;
using Zaraye.Core.Domain.News;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.PriceDiscovery;
using Zaraye.Core.Domain.Security;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Domain.Stores;
using Zaraye.Core.Domain.Tax;
using Zaraye.Core.Domain.Topics;
using Zaraye.Core.Infrastructure;
using Zaraye.Data.DataProviders;
using Zaraye.Data.Extensions;
using Zaraye.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;
using Zaraye.Core.Domain.CustomerTestimonial;
using Zaraye.Core.Domain.EmployeeInsights;

namespace Zaraye.Data.Migrations.UpgradeTo460
{
    [ZarayeMigration("2023-11-01 01:19:33", "SchemaMigration for 4.60.0", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        private readonly IZarayeDataProvider _dataProvider;

        public SchemaMigration(IZarayeDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        ///  <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            #region Adnan

            #region Decimal (18,8)

            //Assembly assembly = Assembly.GetAssembly(typeof(BaseEntity));
            //var derivedTypes = assembly.GetTypes()
            //.Where(p => typeof(BaseEntity).IsAssignableFrom(p) && !p.IsInterface);
            //foreach (Type derivedType in derivedTypes)
            //{
            //    var tableName = derivedType.Name;
            //    var baseNameCompatibility = new BaseNameCompatibility();
            //    if (baseNameCompatibility.TableNames.TryGetValue(derivedType, out var entityName))
            //    {
            //        tableName = entityName;
            //    }

            //    if (tableName == "InventoryInboundList" || tableName == "BrokerLedgerDetails"|| tableName == "BrokerLedgerList" || tableName == "BuyerLedgerDetails"|| tableName == "BuyerLedgerList"
            //        || tableName == "LabourLedgerDetails" || tableName == "LabourLedgerList" || tableName == "SupplierLedgerDetails" || tableName == "SupplierLedgerList" || tableName == "TransporterLedgerDetails"
            //        || tableName == "TransporterLedgerList" || tableName == "PaymentJson"|| tableName == "RateGroupList"|| tableName == "TodayRateList")
            //    {
            //        continue;
            //    }
            //    // Generate SQL statements to alter column lengths for decimal columns
            //    var alterStatements = new List<string>();

            //    var properties = derivedType.GetProperties();

            //    foreach (var property in properties)
            //    {
            //        if (property.PropertyType.Name == "Decimal")
            //        {
            //            // Modify the column length and generate ALTER statement
            //            var columnName = property.Name;
            //            var alterStatement = $"ALTER TABLE `{tableName}` MODIFY COLUMN {columnName} decimal(18,8)";
            //            if (tableName == "Request" || tableName == "RequestForQuotation" || tableName == "InventoryInbound" || tableName == "InventoryOutbound")
            //                alterStatement = $"ALTER TABLE `{tableName}` MODIFY COLUMN {columnName} decimal(24,8)";

            //            alterStatements.Add(alterStatement);
            //        }
            //    }

            //    // Execute the ALTER statements using your SQL execution mechanism
            //    foreach (var alterStatement in alterStatements)
            //    {
            //        await _dataProvider.ExecuteNonQueryAsync(alterStatement);
            //    }
            //}

            #endregion

            #region Miscellaneous Entities

            if (!Schema.Table(nameof(ShipmentReturn)).Exists())
            {
                Create.TableFor<ShipmentReturn>();
            }

            if (!Schema.Table("Currency_Rate_Config").Exists())
            {
                Create.TableFor<CurrencyRateConfig>();
            }
            if (!Schema.Table("msp_inventory_mapping").Exists())
            {
                Create.TableFor<MspInventoryMapping>();
            }
            if (!Schema.Table("customer_bankdetail").Exists())
            {
                Create.TableFor<BankDetail>();
            }

            #endregion

            #region Industry

            if (!Schema.Table(nameof(Industry)).Exists())
            {
                Create.TableFor<Industry>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageIndustries", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Industries", SystemName = "ManageIndustries", Category = "Catalog", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region Shipment

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "AllowEditDetails", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Allow Edit Detials", SystemName = "AllowEditDetails", Category = "Standard", Published = true, DisplayOrder = 0 });
            }

            var shipmentTableName = nameof(Shipment);

            var transporterIdColumnName = nameof(Shipment.TransporterId);
            if (!Schema.Table(shipmentTableName).Column(transporterIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(transporterIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var vehicleIdColumnName = nameof(Shipment.VehicleId);
            if (!Schema.Table(shipmentTableName).Column(vehicleIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(vehicleIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var vehicleNumberColumnName = nameof(Shipment.VehicleNumber);
            if (!Schema.Table(shipmentTableName).Column(vehicleNumberColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(vehicleNumberColumnName).AsString().Nullable().WithDefaultValue(null);
            }

            var deliveryStatusIdColumnName = nameof(Shipment.DeliveryStatusId);
            if (!Schema.Table(shipmentTableName).Column(deliveryStatusIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(deliveryStatusIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var onLabourColumnName = nameof(Shipment.OnLabourCharges);
            if (!Schema.Table(shipmentTableName).Column(onLabourColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(onLabourColumnName).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            var labourTypeIdColumnName = nameof(Shipment.LabourTypeId);
            if (!Schema.Table(shipmentTableName).Column(labourTypeIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(labourTypeIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var costOnZarayeIdIdColumnName = nameof(Shipment.CostOnZarayeId);
            if (!Schema.Table(shipmentTableName).Column(costOnZarayeIdIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(costOnZarayeIdIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var pickupAddressColumnName = nameof(Shipment.PickupAddress);
            if (!Schema.Table(shipmentTableName).Column(pickupAddressColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(pickupAddressColumnName).AsString().Nullable().WithDefaultValue(null);
            }

            var routeTypeIdColumnName = nameof(Shipment.RouteTypeId);
            if (!Schema.Table(shipmentTableName).Column(routeTypeIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(routeTypeIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var shipmentDeliveryAddressColumnName = nameof(Shipment.ShipmentDeliveryAddress);
            if (!Schema.Table(shipmentTableName).Column(shipmentDeliveryAddressColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(shipmentDeliveryAddressColumnName).AsString().Nullable().WithDefaultValue(null);
            }

            var pictureIdColumnName = nameof(Shipment.PictureId);
            if (!Schema.Table(shipmentTableName).Column(pictureIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(pictureIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var transporterTypeIdColumnName = nameof(Shipment.TransporterTypeId);
            if (!Schema.Table(shipmentTableName).Column(transporterTypeIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(transporterTypeIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var deliveryTypeIdColumnName = nameof(Shipment.DeliveryTypeId);
            if (!Schema.Table(shipmentTableName).Column(deliveryTypeIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(deliveryTypeIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var deliveryCostReasonIdColumnName = nameof(Shipment.DeliveryCostReasonId);
            if (!Schema.Table(shipmentTableName).Column(deliveryCostReasonIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(deliveryCostReasonIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var deliveryTimingIdColumnName = nameof(Shipment.DeliveryTimingId);
            if (!Schema.Table(shipmentTableName).Column(deliveryTimingIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(deliveryTimingIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var deliveryCostTypeIdColumnName = nameof(Shipment.DeliveryCostTypeId);
            if (!Schema.Table(shipmentTableName).Column(deliveryCostTypeIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(deliveryCostTypeIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var deliveryDelayedReasonIdColumnName = nameof(Shipment.DeliveryDelayedReasonId);
            if (!Schema.Table(shipmentTableName).Column(deliveryDelayedReasonIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(deliveryDelayedReasonIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var warehouseIdColumnName = nameof(Shipment.WarehouseId);
            if (!Schema.Table(shipmentTableName).Column(warehouseIdColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(warehouseIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var deliveryCostColumnName = nameof(Shipment.DeliveryCost);
            if (!Schema.Table(shipmentTableName).Column(deliveryCostColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(deliveryCostColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var freightChargesColumnName = nameof(Shipment.FreightCharges);
            if (!Schema.Table(shipmentTableName).Column(freightChargesColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(freightChargesColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var labourChargesColumnName = nameof(Shipment.LabourCharges);
            if (!Schema.Table(shipmentTableName).Column(labourChargesColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(labourChargesColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var expectedDateShippedColumnName = nameof(Shipment.ExpectedDateShipped);
            if (!Schema.Table(shipmentTableName).Column(expectedDateShippedColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(expectedDateShippedColumnName).AsDateTime().Nullable().WithDefaultValue(null);
            }

            var expectedDateDeliveredColumnName = nameof(Shipment.ExpectedDateDelivered);
            if (!Schema.Table(shipmentTableName).Column(expectedDateDeliveredColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(expectedDateDeliveredColumnName).AsDateTime().Nullable().WithDefaultValue(null);
            }

            var expectedDeliveryCostColumnName = nameof(Shipment.ExpectedDeliveryCost);
            if (!Schema.Table(shipmentTableName).Column(expectedDeliveryCostColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(expectedDeliveryCostColumnName).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            var expectedQuantityColumnName = nameof(Shipment.ExpectedQuantity);
            if (!Schema.Table(shipmentTableName).Column(expectedQuantityColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(expectedQuantityColumnName).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            var localTransporterNameColumnName = nameof(Shipment.LocalTransporterName);
            if (!Schema.Table(shipmentTableName).Column(localTransporterNameColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(localTransporterNameColumnName).AsString().Nullable().WithDefaultValue(null);
            }

            var localTransporterPhoneNumberColumnName = nameof(Shipment.LocalTransporterPhoneNumber);
            if (!Schema.Table(shipmentTableName).Column(localTransporterPhoneNumberColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(localTransporterPhoneNumberColumnName).AsString().Nullable().WithDefaultValue(null);
            }

            if (!Schema.Table(nameof(ShipmentDropOffHistory)).Exists())
            {
                Create.TableFor<ShipmentDropOffHistory>();
            }

            #endregion

            #region Product

            var productTableName = nameof(Product);

            var publishedOnAppColumnName = nameof(Product.AppPublished);
            if (!Schema.Table(productTableName).Column(publishedOnAppColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(publishedOnAppColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var unitIdColumnName = nameof(Product.UnitId);
            if (!Schema.Table(productTableName).Column(unitIdColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(unitIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region PurchaseOrder

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePurchaseOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage PurchaseOrders", SystemName = "ManagePurchaseOrders", Category = "Orders", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region SaleOrder

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSaleOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage SaleOrders", SystemName = "ManageSaleOrders", Category = "Orders", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region Order

            var orderTableName = nameof(Order);

            var requestIdColumnName = nameof(Order.RequestId);
            if (!Schema.Table(orderTableName).Column(requestIdColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(requestIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var rFQIdColumnName = nameof(Order.RFQId);
            if (!Schema.Table(orderTableName).Column(rFQIdColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(rFQIdColumnName).AsInt32().Nullable().WithDefaultValue(null);
            }

            var orderTypeIdColumnName = nameof(Order.OrderTypeId);
            if (!Schema.Table(orderTableName).Column(orderTypeIdColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(orderTypeIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Order Item

            var orderItemTableName = nameof(OrderItem);

            var brandIdColumnName = nameof(OrderItem.BrandId);
            if (!Schema.Table(orderItemTableName).Column(brandIdColumnName).Exists())
            {
                Alter.Table(orderItemTableName)
                    .AddColumn(brandIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }


            #endregion

            #region Order Calculation

            if (!Schema.Table(nameof(OrderCalculation)).Exists())
            {
                Create.TableFor<OrderCalculation>();
            }

            #endregion

            #region Setting

            if (!_dataProvider.GetTable<Setting>().Any(pr => string.Compare(pr.Name, "ordersettings.custompaymentnumbermask", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new Setting { Name = "ordersettings.custompaymentnumbermask", Value = "P-{YYYY}{MM}{DD}-{ID}" });

            if (!_dataProvider.GetTable<Setting>().Any(pr => string.Compare(pr.Name, "ordersettings.customposhipmentnumbermask", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new Setting { Name = "ordersettings.customposhipmentnumbermask", Value = "S-PO-{YYYY}{MM}{DD}-{ID}" });

            if (!_dataProvider.GetTable<Setting>().Any(pr => string.Compare(pr.Name, "ordersettings.customsoshipmentnumbermask", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new Setting { Name = "ordersettings.customsoshipmentnumbermask", Value = "S-SO-{YYYY}{MM}{DD}-{ID}" });

            //if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageReceivablePayments", StringComparison.InvariantCultureIgnoreCase) == 0))
            //{
            //    var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Receivable Payments", SystemName = "ManageReceivablePayments", Category = "Finance", Published = true, DisplayOrder = 0 });

            //    //add it to the Admin role by default
            //    var adminRole = _dataProvider
            //        .GetTable<CustomerRole>()
            //        .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

            //    _dataProvider.InsertEntity(
            //        new PermissionRecordCustomerRoleMapping
            //        {
            //            CustomerRoleId = adminRole.Id,
            //            PermissionRecordId = manageConnectionStringPermission.Id
            //        }
            //    );
            //}

            //if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageReceivableShipments", StringComparison.InvariantCultureIgnoreCase) == 0))
            //{
            //    var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Receivable Shipments", SystemName = "ManageReceivableShipments", Category = "Finance", Published = true, DisplayOrder = 0 });

            //    //add it to the Admin role by default
            //    var adminRole = _dataProvider
            //        .GetTable<CustomerRole>()
            //        .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

            //    _dataProvider.InsertEntity(
            //        new PermissionRecordCustomerRoleMapping
            //        {
            //            CustomerRoleId = adminRole.Id,
            //            PermissionRecordId = manageConnectionStringPermission.Id
            //        }
            //    );
            //}

            //if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePayableShipments", StringComparison.InvariantCultureIgnoreCase) == 0))
            //{
            //    var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Payable Shipments", SystemName = "ManagePayableShipments", Category = "Finance", Published = true, DisplayOrder = 0 });

            //    //add it to the Admin role by default
            //    var adminRole = _dataProvider
            //        .GetTable<CustomerRole>()
            //        .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

            //    _dataProvider.InsertEntity(
            //        new PermissionRecordCustomerRoleMapping
            //        {
            //            CustomerRoleId = adminRole.Id,
            //            PermissionRecordId = manageConnectionStringPermission.Id
            //        }
            //    );
            //}

            //if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePayablePayments", StringComparison.InvariantCultureIgnoreCase) == 0))
            //{
            //    var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Payable Payments", SystemName = "ManagePayablePayments", Category = "Finance", Published = true, DisplayOrder = 0 });

            //    //add it to the Admin role by default
            //    var adminRole = _dataProvider
            //        .GetTable<CustomerRole>()
            //        .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

            //    _dataProvider.InsertEntity(
            //        new PermissionRecordCustomerRoleMapping
            //        {
            //            CustomerRoleId = adminRole.Id,
            //            PermissionRecordId = manageConnectionStringPermission.Id
            //        }
            //    );
            //}

            #endregion

            #region Cogs Inventory Tagging

            if (!Schema.Table(nameof(CogsInventoryTagging)).Exists())
            {
                Create.TableFor<CogsInventoryTagging>();
            }

            #endregion

            #region Inventory Outbound

            var inventoryOutboundTableName = nameof(InventoryOutbound);

            var inventoryOutboundTypeIdColumnName = nameof(InventoryOutbound.InventoryOutboundTypeId);
            if (!Schema.Table(inventoryOutboundTableName).Column(inventoryOutboundTypeIdColumnName).Exists())
            {
                Alter.Table(inventoryOutboundTableName)
                    .AddColumn(inventoryOutboundTypeIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Business Model

            var orderCalculationTableName = nameof(OrderCalculation);

            var brokerIdColumnName = nameof(OrderCalculation.BrokerId);
            if (!Schema.Table(orderCalculationTableName).Column(brokerIdColumnName).Exists())
            {
                Alter.Table(orderCalculationTableName)
                    .AddColumn(brokerIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var calculatedSellingPriceOfProductColumnName = nameof(OrderCalculation.CalculatedSellingPriceOfProduct);
            if (!Schema.Table(orderCalculationTableName).Column(calculatedSellingPriceOfProductColumnName).Exists())
            {
                Alter.Table(orderCalculationTableName)
                    .AddColumn(calculatedSellingPriceOfProductColumnName).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            var buyerCommissionReceivableUserIdColumnName = nameof(OrderCalculation.BuyerCommissionReceivableUserId);
            if (!Schema.Table(orderCalculationTableName).Column(buyerCommissionReceivableUserIdColumnName).Exists())
            {
                Alter.Table(orderCalculationTableName)
                    .AddColumn(buyerCommissionReceivableUserIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var buyerCommissionPayableUserIdColumnName = nameof(OrderCalculation.BuyerCommissionPayableUserId);
            if (!Schema.Table(orderCalculationTableName).Column(buyerCommissionPayableUserIdColumnName).Exists())
            {
                Alter.Table(orderCalculationTableName)
                    .AddColumn(buyerCommissionPayableUserIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var supplierCommissionReceivableUserIdColumnName = nameof(OrderCalculation.SupplierCommissionReceivableUserId);
            if (!Schema.Table(orderCalculationTableName).Column(supplierCommissionReceivableUserIdColumnName).Exists())
            {
                Alter.Table(orderCalculationTableName)
                    .AddColumn(supplierCommissionReceivableUserIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            var supplierCommissionPayableUserIdColumnName = nameof(OrderCalculation.SupplierCommissionPayableUserId);
            if (!Schema.Table(orderCalculationTableName).Column(supplierCommissionPayableUserIdColumnName).Exists())
            {
                Alter.Table(orderCalculationTableName)
                    .AddColumn(supplierCommissionPayableUserIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Inbound Inventory

            var customerLedgerTableName = nameof(CustomerLedger);

            var inventoryIdColumnName = nameof(CustomerLedger.InventoryId);
            if (!Schema.Table(customerLedgerTableName).Column(inventoryIdColumnName).Exists())
            {
                Alter.Table(customerLedgerTableName)
                    .AddColumn(inventoryIdColumnName).AsInt32().ForeignKey<InventoryInbound>(onDelete: Rule.SetNull).Nullable();
            }

            #endregion

            #endregion

            #region Haider Ali

            #region Customer

            var customerTableName = nameof(Customer);

            var firstNameCustomerColumnName = nameof(Customer.FirstName);
            var lastNameCustomerColumnName = nameof(Customer.LastName);
            var genderCustomerColumnName = nameof(Customer.Gender);
            var dobCustomerColumnName = nameof(Customer.DateOfBirth);
            var companyCustomerColumnName = nameof(Customer.Company);
            var address1CustomerColumnName = nameof(Customer.StreetAddress);
            var address2CustomerColumnName = nameof(Customer.StreetAddress2);
            var zipCustomerColumnName = nameof(Customer.ZipPostalCode);
            var cityCustomerColumnName = nameof(Customer.City);
            var countyCustomerColumnName = nameof(Customer.County);
            var countryIdCustomerColumnName = nameof(Customer.CountryId);
            var stateIdCustomerColumnName = nameof(Customer.StateProvinceId);
            var phoneCustomerColumnName = nameof(Customer.Phone);
            var faxCustomerColumnName = nameof(Customer.Fax);
            var vatNumberCustomerColumnName = nameof(Customer.VatNumber);
            var vatNumberStatusIdCustomerColumnName = nameof(Customer.VatNumberStatusId);
            var timeZoneIdCustomerColumnName = nameof(Customer.TimeZoneId);
            var attributeXmlCustomerColumnName = nameof(Customer.CustomCustomerAttributesXML);
            var currencyIdCustomerColumnName = nameof(Customer.CurrencyId);
            var languageIdCustomerColumnName = nameof(Customer.LanguageId);
            var taxDisplayTypeIdCustomerColumnName = nameof(Customer.TaxDisplayTypeId);
            var commissionRateColumnName = nameof(Customer.CommissionRate);
            var sendWelcomeMessageColumnName = nameof(Customer.SendWelcomeMessage);
            var fullNameColumnName = nameof(Customer.FullName);
            var sourceColumnName = nameof(Customer.Source);
            var createdByColumnName = nameof(Customer.CreatedBy);
            var supportAgentIdColumnName = nameof(Customer.SupportAgentId);
            var parentIdColumnName = nameof(Customer.ParentId);
            var updatedOnUtcColumnName = nameof(Customer.UpdatedOnUtc);
            var cincColumnName = nameof(Customer.Cnic);
            var BookerTypeColumnName = nameof(Customer.BookerType);
            var AreaIdColumnName = nameof(Customer.AreaId);
            var IndustryIdColumnName = nameof(Customer.IndustryId);
            var UserTypeIdColumnName = nameof(Customer.UserTypeId);
            var GstColumnName = nameof(Customer.Gst);
            var PocColumnName = nameof(Customer.IsPoc);

            if (!Schema.Table(customerTableName).Column(firstNameCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(firstNameCustomerColumnName).AsString(1000).Nullable();
            }

            if (!Schema.Table(customerTableName).Column(lastNameCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(lastNameCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(genderCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(genderCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(dobCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(dobCustomerColumnName).AsDateTime2().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(cincColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(cincColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(BookerTypeColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(BookerTypeColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(companyCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(companyCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(address1CustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(address1CustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(address2CustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(address2CustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(zipCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(zipCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(cityCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(cityCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(countyCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(countyCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(countryIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(countryIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(customerTableName).Column(stateIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(stateIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(customerTableName).Column(phoneCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(phoneCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(faxCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(faxCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(vatNumberCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(vatNumberCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(vatNumberStatusIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(vatNumberStatusIdCustomerColumnName).AsInt32().NotNullable().SetExistingRowsTo((int)VatNumberStatus.Unknown);
            }
            if (!Schema.Table(customerTableName).Column(timeZoneIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(timeZoneIdCustomerColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(attributeXmlCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(attributeXmlCustomerColumnName).AsString(int.MaxValue).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(currencyIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(currencyIdCustomerColumnName).AsInt32().ForeignKey<Currency>(onDelete: Rule.SetNull).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(languageIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(languageIdCustomerColumnName).AsInt32().ForeignKey<Language>(onDelete: Rule.SetNull).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(taxDisplayTypeIdCustomerColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(taxDisplayTypeIdCustomerColumnName).AsInt32().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(commissionRateColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(commissionRateColumnName).AsDecimal().NotNullable();
            }
            if (!Schema.Table(customerTableName).Column(sendWelcomeMessageColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(sendWelcomeMessageColumnName).AsString(int.MaxValue).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(fullNameColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(fullNameColumnName).AsString(int.MaxValue).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(sourceColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(sourceColumnName).AsString(int.MaxValue).Nullable();
            }
            if (!Schema.Table(customerTableName).Column(createdByColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(createdByColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(customerTableName).Column(supportAgentIdColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(supportAgentIdColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(customerTableName).Column(parentIdColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(parentIdColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(customerTableName).Column(updatedOnUtcColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(updatedOnUtcColumnName).AsDateTime().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(IndustryIdColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(IndustryIdColumnName).AsInt32().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(GstColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(GstColumnName).AsString().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(UserTypeIdColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(UserTypeIdColumnName).AsInt32().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(AreaIdColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(AreaIdColumnName).AsInt32().Nullable();
            }
            if (!Schema.Table(customerTableName).Column(BookerTypeColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(BookerTypeColumnName).AsString().Nullable();
            }
            if (!Schema.Table(nameof(Customer)).Column(nameof(Customer.IsZarayeTransporter)).Exists())
            {
                Alter.Table(nameof(Customer))
                    .AddColumn(nameof(Customer.IsZarayeTransporter)).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            if (!Schema.Table(nameof(Customer)).Column(nameof(Customer.IsPoc)).Exists())
            {
                Alter.Table(nameof(Customer))
                    .AddColumn(nameof(Customer.IsPoc)).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            #endregion

            #region Discount

            //5705
            var discountTableName = nameof(Discount);
            var isActiveDiscountColumnName = nameof(Discount.IsActive);

            if (!Schema.Table(discountTableName).Column(isActiveDiscountColumnName).Exists())
            {
                Alter.Table(discountTableName)
                    .AddColumn(isActiveDiscountColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }

            #endregion

            #region ShipmentReturnReason

            if (!Schema.Table(nameof(ShipmentReturnReason)).Exists())
            {
                Create.TableFor<ShipmentReturnReason>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageShipmentReturnReason", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageShipmentReturnReason", SystemName = "ManageShipmentReturnReason", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region DeliverTimeReason

            if (!Schema.Table(nameof(DeliveryTimeReason)).Exists())
            {
                Create.TableFor<DeliveryTimeReason>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageDeliveryTimeReason", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageDeliveryTimeReason", SystemName = "ManageDeliveryTimeReason", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region DeliverCostReason

            if (!Schema.Table(nameof(DeliveryCostReason)).Exists())
            {
                Create.TableFor<DeliveryCostReason>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageDeliveryCostReason", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageDeliveryCostReason", SystemName = "ManageDeliveryCostReason", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region UserType

            if (!Schema.Table(nameof(UserType)).Exists())
            {
                Create.TableFor<UserType>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBuyerSupplierType", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageBuyerSupplierType", SystemName = "ManageBuyerSupplierType", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region AppVersion

            if (!Schema.Table(nameof(AppVersion)).Exists())
            {
                Create.TableFor<AppVersion>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageAppVersions", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Industries", SystemName = "ManageAppVersions", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region PermissionRecord

            if (!Schema.Table(nameof(PermissionRecord)).Column(nameof(PermissionRecord.Published)).Exists())
            {
                Alter.Table(nameof(PermissionRecord))
                    .AddColumn(nameof(PermissionRecord.Published)).AsBoolean().NotNullable().WithDefaultValue(true);
            }

            if (!Schema.Table(nameof(PermissionRecord)).Column(nameof(PermissionRecord.DisplayOrder)).Exists())
            {
                Alter.Table(nameof(PermissionRecord))
                    .AddColumn(nameof(PermissionRecord.DisplayOrder)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region ActivityLog

            var activityLogTableName = nameof(ActivityLog);

            var onlineLeadStatusId = nameof(ActivityLog.OnlineLeadStatusId);
            var data = nameof(ActivityLog.Data);
            var newData = nameof(ActivityLog.NewData);

            if (!Schema.Table(activityLogTableName).Column(onlineLeadStatusId).Exists())
            {
                Alter.Table(activityLogTableName)
                    .AddColumn(onlineLeadStatusId).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(activityLogTableName).Column(data).Exists())
            {
                Alter.Table(activityLogTableName)
                    .AddColumn(data).AsString(int.MaxValue).Nullable();
            }
            if (!Schema.Table(activityLogTableName).Column(newData).Exists())
            {
                Alter.Table(activityLogTableName)
                    .AddColumn(newData).AsString(int.MaxValue).Nullable();
            }

            #endregion

            #region Faq

            if (!Schema.Table(nameof(Faq)).Exists())
            {
                Create.TableFor<Faq>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageFaqs", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageFaqs", SystemName = "ManageFaqs", Category = "Content Management", Published = true, DisplayOrder = 0 });
            }
            if (!Schema.Table(nameof(Faq)).Column(nameof(Faq.Type)).Exists())
            {
                Alter.Table(nameof(Faq))
                    .AddColumn(nameof(Faq.Type)).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            #endregion

            #region CommodityData

            if (!Schema.Table(nameof(CommodityData)).Exists())
            {
                Create.TableFor<CommodityData>();
            }

            #endregion
            #region Jaiza

            if (!Schema.Table(nameof(Jaiza)).Exists())
            {
                Create.TableFor<Jaiza>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageJaizas", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageJaizas", SystemName = "ManageJaizas", Category = "Content Management", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region AppFeedBack

            if (!Schema.Table(nameof(AppFeedBack)).Exists())
            {
                Create.TableFor<AppFeedBack>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageFeedbacks", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageFeedbacks", SystemName = "ManageFeedbacks", Category = "Content Management", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region PushNotification

            if (!Schema.Table(nameof(PushNotification)).Exists())
            {
                Create.TableFor<PushNotification>();
            }
            if (!Schema.Table("pushnotification_device_mapping").Exists())
            {
                Create.TableFor<PushNotificationDevice>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePushNotifications", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Push Notifications", SystemName = "ManagePushNotifications", Category = "Promo", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region Country

            if (!Schema.Table(nameof(Country)).Column(nameof(Country.PublishedOnApp)).Exists())
            {
                Alter.Table(nameof(Country))
                    .AddColumn(nameof(Country.PublishedOnApp)).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            #endregion

            #region SateProvince

            if (!Schema.Table(nameof(StateProvince)).Column(nameof(StateProvince.ParentId)).Exists())
            {
                Alter.Table(nameof(StateProvince))
                    .AddColumn(nameof(StateProvince.ParentId)).AsInt32().NotNullable();
            }
            if (!Schema.Table(nameof(StateProvince)).Column(nameof(StateProvince.Deleted)).Exists())
            {
                Alter.Table(nameof(StateProvince))
                    .AddColumn(nameof(StateProvince.Deleted)).AsBoolean().NotNullable().WithDefaultValue(false);
            }
            //if (!Schema.Table(nameof(StateProvince)).Column(nameof(StateProvince.UpdatedOnUtc)).Exists())
            //{
            //    Alter.Table(nameof(StateProvince))
            //        .AddColumn(nameof(StateProvince.UpdatedOnUtc)).AsDateTime();
            //}

            #endregion

            #region SupportAgent

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSupportAgent", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage SupportAgent", SystemName = "ManageSupportAgent", Category = "Customers", Published = true, DisplayOrder = 0 });
            }

            #region Configuration

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageConfigurations", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageConfigurations", SystemName = "ManageConfigurations", Category = "Configuration", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #endregion

            #region Buyers

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBuyers", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Buyers", SystemName = "ManageBuyers", Category = "Sales", Published = true, DisplayOrder = 0 });
            }

            #region Configuration

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageConfigurations", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageConfigurations", SystemName = "ManageConfigurations", Category = "Configuration", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #endregion

            #region Suppliers

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSuppliers", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Suppliers", SystemName = "ManageSuppliers", Category = "Purchases", Published = true, DisplayOrder = 0 });
            }

            #region Configuration

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageConfigurations", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageConfigurations", SystemName = "ManageConfigurations", Category = "Configuration", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #endregion

            #region supplier_product_mapping

            if (!Schema.Table("supplier_product_mapping").Exists())
            {
                Create.TableFor<SupplierProduct>();
            }

            #endregion

            #region CustomerLedger

            if (!Schema.Table(nameof(CustomerLedger)).Exists())
            {
                Create.TableFor<CustomerLedger>();
            }

            #endregion

            #region Warehouse

            if (!Schema.Table(nameof(Warehouse)).Column(nameof(Warehouse.SupplierId)).Exists())
            {
                Alter.Table(nameof(Warehouse))
                    .AddColumn(nameof(Warehouse.SupplierId)).AsInt32().Nullable();
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.IsDirectOrder)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.IsDirectOrder)).AsBoolean().WithDefaultValue(false);
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.BuyerId)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.BuyerId)).AsInt32().Nullable();
            }


            #endregion

            #region Payment

            if (!Schema.Table(nameof(Payment)).Exists())
            {
                Create.TableFor<Payment>();
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.IsBusienssApproved)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.IsBusienssApproved)).AsBoolean().WithDefaultValue(false);
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.BusinessId)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.BusinessId)).AsInt32().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.BusinessComment)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.BusinessComment)).AsString(500).Nullable();
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.IsFinanceApproved)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.IsFinanceApproved)).AsBoolean().WithDefaultValue(false);
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.FinanceId)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.FinanceId)).AsInt32().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.FinanceComment)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.FinanceComment)).AsString(500).Nullable();
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.PaymentStatusId)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.PaymentStatusId)).AsInt32().WithDefaultValue(10);
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.BusinessActionDateUtc)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.BusinessActionDateUtc)).AsDateTime().Nullable();
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.FinanceActionDateUtc)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.FinanceActionDateUtc)).AsDateTime().Nullable();
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.AdjustAmount)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.AdjustAmount)).AsDecimal().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.CompanyBankAccountId)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.CompanyBankAccountId)).AsInt32().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.ChequeClearingDate)).Exists())
                Alter.Table(nameof(Payment)).AddColumn(nameof(Payment.ChequeClearingDate)).AsDateTime().Nullable().WithDefaultValue(null);

            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.OpsAmount)).Exists())
                Alter.Table(nameof(Payment)).AddColumn(nameof(Payment.OpsAmount)).AsDecimal().Nullable().WithDefaultValue(0);

            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.FinanceAmount)).Exists())
                Alter.Table(nameof(Payment)).AddColumn(nameof(Payment.FinanceAmount)).AsDecimal().Nullable().WithDefaultValue(0);

            #endregion

            #region Payment

            if (!Schema.Table("Shipment_Payment_Mapping").Exists())
            {
                Create.TableFor<ShipmentPaymentMapping>();
            }

            #endregion

            #region Generic Picture

            if (!Schema.Table("generic_picture_mapping").Exists())
            {
                Create.TableFor<GenericPicture>();
            }

            #endregion

            #region Returns

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageReturns", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Returns", SystemName = "ManageReturns", Category = "Return", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #region Contract

            if (!Schema.Table(nameof(Contract)).Exists())
            {
                Create.TableFor<Contract>();
            }

            #endregion

            #region OrderDeliverySchedule

            if (!Schema.Table(nameof(OrderDeliverySchedule)).Exists())
            {
                Create.TableFor<OrderDeliverySchedule>();
            }

            #endregion

            #region OrderDeliveryRequest

            if (!Schema.Table(nameof(OrderDeliveryRequest)).Exists())
            {
                Create.TableFor<OrderDeliveryRequest>();
            }

            #endregion

            #region OrderSalesReturnRequest

            if (!Schema.Table(nameof(OrderSalesReturnRequest)).Exists())
            {
                Create.TableFor<OrderSalesReturnRequest>();
            }

            #endregion

            #region ReturnSaleOrder

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageReturnSaleOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Return Sale Orders", SystemName = "ManageReturnSaleOrders", Category = "Return", Published = true, DisplayOrder = 0 });


                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #region ReturnPurchaseOrder

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageReturnPurchaseOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Return Purchase Orders", SystemName = "ManageReturnPurchaseOrders", Category = "Return", Published = true, DisplayOrder = 0 });


                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }
            #endregion


            #region TransporterLedger

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageTransporterLedger", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage TransporterLedger", SystemName = "ManageTransporterLedger", Category = "Ledger", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion.

            #region BrokerLedger

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBrokerLedger", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage BrokerLedger", SystemName = "ManageBrokerLedger", Category = "Ledger", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #region LabourLedger

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageLabourLedger", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage LabourLedger", SystemName = "ManageLabourLedger", Category = "Ledger", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #region DailyRate

            if (!Schema.Table(nameof(DailyRate)).Exists())
            {
                Create.TableFor<DailyRate>();
            }

            #endregion

            #region DailyRateMargin

            if (!Schema.Table(nameof(DailyRateMargin)).Exists())
            {
                Create.TableFor<DailyRateMargin>();
            }

            #endregion

            #region AppliedCreditCustomer

            if (!Schema.Table("applied_credit_customer").Exists())
            {
                Create.TableFor<AppliedCreditCustomer>();
            }
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageAppliedCreditCustomers", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageAppliedCreditCustomers", SystemName = "ManageAppliedCreditCustomers", Category = "Customers", Published = true, DisplayOrder = 0 });
            }

            #endregion
            #endregion
            #region Osama Maroof

            #region CustomerTestimonial

            if (!Schema.Table(nameof(CustomerTestimonial)).Exists())
            {
                Create.TableFor<CustomerTestimonial>();
            }
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageCustomerTestimonials", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage CustomerTestimonials", SystemName = "ManageCustomerTestimonials", Category = "Content Management", Published = true, DisplayOrder = 0 });
            }


            #endregion

            #endregion

            #region Sana Malik
            #region EmployeeInsights

            if (!Schema.Table(nameof(EmployeeInsights)).Exists())
            {
                Create.TableFor<EmployeeInsights>();
            }
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageEmployeeInsights", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage EmployeeInsights", SystemName = "ManageEmployeeInsights", Category = "Content Management", Published = true, DisplayOrder = 0 });
            }


            #endregion
            #endregion

            #region Zohaib Chohan

            var topicTableName = nameof(Topic);
            var IncludeInFooterColumn4CategoryColumnName = nameof(Topic.IncludeInFooterColumn4);
            var IncludeInFooterColumn5CategoryColumnName = nameof(Topic.IncludeInFooterColumn5);

            if (!Schema.Table(topicTableName).Column(IncludeInFooterColumn4CategoryColumnName).Exists())
            {
                Alter.Table(topicTableName)
                    .AddColumn(IncludeInFooterColumn4CategoryColumnName).AsBoolean().Nullable();
            }
            if (!Schema.Table(topicTableName).Column(IncludeInFooterColumn5CategoryColumnName).Exists())
            {
                Alter.Table(topicTableName)
                    .AddColumn(IncludeInFooterColumn5CategoryColumnName).AsBoolean().Nullable();
            }

            #endregion

            #region Miqdad

            #region shipmentpaymentmapping
            if (!Schema.Table("Shipment_Payment_Mapping").Column(nameof(ShipmentPaymentMapping.IsDeliveryCost)).Exists())
            {
                Alter.Table("Shipment_Payment_Mapping")
                    .AddColumn(nameof(ShipmentPaymentMapping.IsDeliveryCost)).AsBoolean().NotNullable().WithDefaultValue(false);
            }
            #endregion

            #region Customer

            if (!Schema.Table(nameof(Customer)).Column(nameof(Customer.IsAppActive)).Exists())
            {
                Alter.Table(nameof(Customer))
                    .AddColumn(nameof(Customer.IsAppActive)).AsBoolean().NotNullable().WithDefaultValue(false);
            }

            #endregion

            #region Category

            var categoryTableName = nameof(Category);
            var IndustryIdCategoryColumnName = nameof(Category.IndustryId);
            var IconIdCategoryColumnName = nameof(Category.IconId);
            var TicketExpiryDaysCategoryColumnName = nameof(Category.TicketExpiryDays);
            var TicketPirorityCategoryColumnName = nameof(Category.TicketPirority);
            var ExpiryDaysCategoryColumnName = nameof(Category.ExpiryDays);
            var AppPublishedColumnName = nameof(Category.AppPublished);
            var BookerIdColumnName = nameof(Category.BookerId);

            if (!Schema.Table(categoryTableName).Column(IndustryIdCategoryColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(IndustryIdCategoryColumnName).AsInt32().Nullable();
            }

            if (!Schema.Table(categoryTableName).Column(IconIdCategoryColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(IconIdCategoryColumnName).AsInt32().Nullable();
            }

            if (!Schema.Table(categoryTableName).Column(TicketExpiryDaysCategoryColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(TicketExpiryDaysCategoryColumnName).AsInt32().Nullable();
            }

            if (!Schema.Table(categoryTableName).Column(TicketPirorityCategoryColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(TicketPirorityCategoryColumnName).AsInt32().Nullable();
            }

            if (!Schema.Table(categoryTableName).Column(ExpiryDaysCategoryColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(ExpiryDaysCategoryColumnName).AsInt32().Nullable();
            }

            if (!Schema.Table(categoryTableName).Column(AppPublishedColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(AppPublishedColumnName).AsBoolean().NotNullable().WithDefaultValue(false);
            }

            if (!Schema.Table(categoryTableName).Column(BookerIdColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(BookerIdColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Campaigns

            if (!Schema.Table(nameof(CampaignEmail)).Exists())
            {
                Create.TableFor<CampaignEmail>();
            }

            #endregion

            #region topics
            var topicTableNameM = nameof(Topic);
            var CategoryTemplateIdTopicColumnName = nameof(Topic.CategoryTemplateId);
            if (!Schema.Table(topicTableNameM).Column(CategoryTemplateIdTopicColumnName).Exists())
            {
                Alter.Table(topicTableNameM)
                    .AddColumn(CategoryTemplateIdTopicColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }
            #endregion

            #region Newslettler
            var PictureIdNewsLetterColumnName = nameof(NewsItem.PictureId);
            var IsAppNewsColumnName = nameof(NewsItem.IsAppNews);
            if (!Schema.Table("News").Column(PictureIdNewsLetterColumnName).Exists())
            {
                Alter.Table("News")
                    .AddColumn(PictureIdNewsLetterColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table("News").Column(PictureIdNewsLetterColumnName).Exists())
            {
                Alter.Table("News")
                    .AddColumn(IsAppNewsColumnName).AsBoolean().NotNullable().WithDefaultValue(false);
            }
            #endregion

            #region blog post
            var BlogTableName = nameof(BlogPost);
            var PictureIdBlogColumnName = nameof(BlogPost.PictureId);
            var AuthorNameBlogColumnName = nameof(BlogPost.AuthorName);

            if (!Schema.Table(BlogTableName).Column(PictureIdBlogColumnName).Exists())
            {
                Alter.Table(BlogTableName)
                    .AddColumn(PictureIdBlogColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(BlogTableName).Column(AuthorNameBlogColumnName).Exists())
            {
                Alter.Table(BlogTableName)
                    .AddColumn(AuthorNameBlogColumnName).AsString(500).Nullable();
            }
            #endregion

            #region Booker
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBookers", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Bookers", SystemName = "ManageBookers", Category = "Customers", Published = true, DisplayOrder = 0 });
            }
            #endregion

            #region Transporter

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageTransporters", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Transporters", SystemName = "ManageTransporters", Category = "Customers", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region Broker 

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBroker", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Broker", SystemName = "ManageBroker", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region Request

            if (!Schema.Table(nameof(Request)).Exists())
            {
                Create.TableFor<Request>();
            }

            if (!Schema.Table(nameof(Request)).Column(nameof(Request.RequestTypeId)).Exists())
            {
                Alter.Table(nameof(Request))
                    .AddColumn(nameof(Request.RequestTypeId)).AsInt32().NotNullable();
            }

            if (!Schema.Table(nameof(Request)).Column(nameof(Request.LeadId)).Exists())
            {
                Alter.Table(nameof(Request))
                    .AddColumn(nameof(Request.LeadId)).AsInt32().Nullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(Request)).Column(nameof(Request.PocId)).Exists())
            {
                Alter.Table(nameof(Request))
                    .AddColumn(nameof(Request.PocId)).AsInt32().Nullable().WithDefaultValue(0);
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
            {

                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Request", SystemName = "ManageRequest", Category = "Sales", Published = true, DisplayOrder = 0 });


                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePurchaseRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Purchase Request", SystemName = "ManagePurchaseRequest", Category = "Orders", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "AllowEditDetails", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Allow Edit Detials", SystemName = "AllowEditDetails", Category = "Standard", Published = true, DisplayOrder = 0 });
            }



            #endregion

            #region RejectReason
            if (!Schema.Table(nameof(OnlineLeadRejectReason)).Exists())
            {
                Create.TableFor<OnlineLeadRejectReason>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageOnlineRejectReason", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageOnlineRejectReason", SystemName = "ManageOnlineRejectReason", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }


            #endregion

            #region RequestForQuotation

            if (!Schema.Table(nameof(RequestForQuotation)).Exists())
            {
                Create.TableFor<RequestForQuotation>();
            }


            #endregion

            #region Quotation

            if (!Schema.Table(nameof(Quotation)).Exists())
            {
                Create.TableFor<Quotation>();
            }

            #endregion

            #region RequestRfqQuotationMapping

            if (!Schema.Table("Request_Rfq_Quotation_Mapping").Exists())
            {
                Create.TableFor<RequestRfqQuotationMapping>();
            }


            #endregion

            #region order

            var quotationIdColumnName = nameof(Order.QuotationId);
            var rejectedReasonColumnName = nameof(Order.RejectedReason);
            if (!Schema.Table(orderTableName).Column(quotationIdColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(quotationIdColumnName).AsInt32().Nullable().WithDefaultValue(null);
            }

            if (!Schema.Table(orderTableName).Column(rejectedReasonColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(rejectedReasonColumnName).AsString().Nullable().WithDefaultValue(null);
            }

            #endregion

            #region Brand

            if (!Schema.Table(nameof(Manufacturer)).Column(nameof(Manufacturer.PublishedOnPriceDiscovery)).Exists())
            {
                Alter.Table(nameof(Manufacturer))
                    .AddColumn(nameof(Manufacturer.PublishedOnPriceDiscovery)).AsBoolean().NotNullable().WithDefaultValue(false);
            }

            if (!Schema.Table(nameof(ProductAttributeValue)).Column(nameof(ProductAttributeValue.PublishedOnPriceDiscovery)).Exists())
            {
                Alter.Table(nameof(ProductAttributeValue))
                    .AddColumn(nameof(ProductAttributeValue.PublishedOnPriceDiscovery)).AsBoolean().NotNullable().WithDefaultValue(false);
            }

            #endregion

            #region Inventory

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBrokerInventories", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage BrokerInventory", SystemName = "ManageBrokerInventories", Category = "Stock Management", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.SupplierCommissionReceivablePerBag)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.SupplierCommissionReceivablePerBag)).AsDecimal().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.SupplierCommissionPayablePerBag)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.SupplierCommissionPayablePerBag)).AsDecimal().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.SupplierCommissionReceivableUserId)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.SupplierCommissionReceivableUserId)).AsInt32().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.SupplierCommissionPayableUserId)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.SupplierCommissionPayableUserId)).AsInt32().WithDefaultValue(0);
            }


            #endregion

            //#region SupportAgent
            //if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSupportAgent", StringComparison.InvariantCultureIgnoreCase) == 0))
            //{
            //    _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage SupportAgent", SystemName = "ManageSupportAgent", Category = "Customers", Published = true, DisplayOrder = 0 });
            //}
            //#endregion

            #region Customer industry

            if (!Schema.Table("Customer_Industry_Mapping").Exists())
            {
                Create.TableFor<CustomerIndustryMapping>();

            }
            if (!Schema.Table(nameof(Industry)).Column(nameof(Industry.IsAllowCredit)).Exists())
            {
                Alter.Table(nameof(Industry))
                    .AddColumn(nameof(Industry.IsAllowCredit)).AsBoolean().Nullable().WithDefaultValue(0);
            }

            #endregion

            #region TransporterVehicleMapping

            if (!Schema.Table("Transporter_Vehicle_Mapping").Exists())
            {
                Create.TableFor<TransporterVehicleMapping>();
            }

            //if (Schema.Table("Transporter_Vehicle_Mapping").Exists())
            //{
            //    Alter.Table(nameof(TransporterVehicleMapping))
            //        .AddColumn(nameof(TransporterVehicleMapping.VehicleNumber)).AsString().Nullable().WithDefaultValue(null);
            //}

            #endregion

            #region TransporterCost

            if (!Schema.Table(nameof(TransporterCost)).Exists())
            {
                Create.TableFor<TransporterCost>();
            }

            #endregion

            #region Brand
            var manufacturerTableName = nameof(Manufacturer);
            var AppPublishedManufacturerColumnName = nameof(Manufacturer.AppPublished);
            var ContentPublishedManufacturerColumnName = nameof(Manufacturer.ContentPublished);
            var ShowOnHomePageManufacturerColumnName = nameof(Manufacturer.ShowOnHomePage);
            var IncludeInTopMenuManufacturerColumnName = nameof(Manufacturer.IncludeInTopMenu);
            var IndustryIdManufacturerColumnName = nameof(Manufacturer.IndustryId);
            var ShortDescriptionManufacturerColumnName = nameof(Manufacturer.ShortDescription);
            if (!Schema.Table(manufacturerTableName).Column(AppPublishedManufacturerColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(AppPublishedManufacturerColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            if (!Schema.Table(manufacturerTableName).Column(ContentPublishedManufacturerColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(ContentPublishedManufacturerColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            if (!Schema.Table(manufacturerTableName).Column(ShowOnHomePageManufacturerColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(ShowOnHomePageManufacturerColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            if (!Schema.Table(manufacturerTableName).Column(IncludeInTopMenuManufacturerColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(IncludeInTopMenuManufacturerColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            if (!Schema.Table(manufacturerTableName).Column(IndustryIdManufacturerColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(IndustryIdManufacturerColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(manufacturerTableName).Column(ShortDescriptionManufacturerColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(ShortDescriptionManufacturerColumnName).AsString(10000).Nullable();
            }
            #endregion

            #region ShoppingCartItem
            var ShoppingCartItemTableName = nameof(ShoppingCartItem);
            var BrandIdShoppingCartItemColumnName = nameof(ShoppingCartItem.BrandId);
            var OverridePriceShoppingCartItemColumnName = nameof(ShoppingCartItem.OverridePrice);
            var SupplierIdShoppingCartItemColumnName = nameof(ShoppingCartItem.SupplierId);
            if (!Schema.Table(ShoppingCartItemTableName).Column(BrandIdShoppingCartItemColumnName).Exists())
            {
                Alter.Table(ShoppingCartItemTableName)
                    .AddColumn(BrandIdShoppingCartItemColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(ShoppingCartItemTableName).Column(OverridePriceShoppingCartItemColumnName).Exists())
            {
                Alter.Table(ShoppingCartItemTableName)
                    .AddColumn(OverridePriceShoppingCartItemColumnName).AsDecimal().NotNullable().WithDefaultValue(0.0000);
            }
            if (!Schema.Table(ShoppingCartItemTableName).Column(SupplierIdShoppingCartItemColumnName).Exists())
            {
                Alter.Table(ShoppingCartItemTableName)
                    .AddColumn(SupplierIdShoppingCartItemColumnName).AsInt32().NotNullable().WithDefaultValue(0);
            }
            #endregion

            #region Inventory

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePriceDiscoveryRates", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage PriceDiscovery Rates", SystemName = "ManagePriceDiscoveryRates", Category = "Price Discovery", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePriceDiscoveryPendingRates", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage PriceDiscovery Pending Rates", SystemName = "ManagePriceDiscoveryPendingRates", Category = "Price Discovery", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePriceDiscoveryTodayRates", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage PriceDiscovery Today Rates", SystemName = "ManagePriceDiscoveryTodayRates", Category = "Price Discovery", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #region alter Columns

            Alter.Table(nameof(ShoppingCartItem))
                    .AlterColumn(nameof(ShoppingCartItem.Quantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);

            Alter.Table(nameof(ProductWarehouseInventory))
                    .AlterColumn(nameof(ProductWarehouseInventory.StockQuantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);

            Alter.Table(nameof(ProductWarehouseInventory))
                    .AlterColumn(nameof(ProductWarehouseInventory.ReservedQuantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);

            Alter.Table(nameof(OrderItem))
                    .AlterColumn(nameof(OrderItem.Quantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);

            Alter.Table(nameof(OrderItem))
                    .AlterColumn(nameof(OrderItem.Quantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);

            Alter.Table(nameof(Product))
                    .AlterColumn(nameof(Product.StockQuantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);

            Alter.Table(nameof(StockQuantityHistory))
                    .AlterColumn(nameof(StockQuantityHistory.QuantityAdjustment)).AsDecimal().NotNullable().WithDefaultValue(0.0000);

            Alter.Table(nameof(StockQuantityHistory))
                    .AlterColumn(nameof(StockQuantityHistory.StockQuantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);

            Alter.Table(nameof(ProductAttributeCombination))
                    .AlterColumn(nameof(ProductAttributeCombination.StockQuantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);


            Alter.Table(nameof(ShipmentItem))
                    .AlterColumn(nameof(ShipmentItem.Quantity)).AsDecimal().NotNullable().WithDefaultValue(0.0000);
            #endregion

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageUsers", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Users", SystemName = "ManageUsers", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }

            #region tijara 

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageTijara", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Tijara", SystemName = "ManageTijara", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBuyerRegistration", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Buyer registration", SystemName = "ManageBuyerRegistration", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePlaceRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Place request", SystemName = "ManagePlaceRequest", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePlaceOrder", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Place order", SystemName = "ManagePlaceOrder", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePlaceDeliveryRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Place delivery request", SystemName = "ManagePlaceDeliveryRequest", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePlaceSalesReturnRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Place sales return request", SystemName = "ManagePlaceSalesReturnRequest", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSupplierRegistration", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Supplier registration", SystemName = "ManageSupplierRegistration", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePlaceQuotation", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Place quotation", SystemName = "ManagePlaceQuotation", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageRaisePo", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Raise po", SystemName = "ManageRaisePo", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageUploadGrns", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Upload grn", SystemName = "ManageUploadGrns", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageUploadProofofPayments", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Upload proof of payment", SystemName = "ManageUploadProofofPayments", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageApproveorRejectPowithAttachments", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Approveor reject po with attachment", SystemName = "ManageApproveorRejectPowithAttachments", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageReassignonTickets", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Reassign on ticket", SystemName = "ManageReassignonTickets", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePOTicket", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Po ticket", SystemName = "ManagePOTicket", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSaleReturnTicket", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Sale Return Ticket", SystemName = "ManageSaleReturnTicket", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageDeliveryRequestTicket", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Delivery Request Ticket", SystemName = "ManageDeliveryRequestTicket", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageAgent", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Agent", SystemName = "ManageAgent", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageRaisePoCompleteAndIncomplete", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "RaisePoCompleteAndIncomplete", SystemName = "ManageRaisePoCompleteAndIncomplete", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSalesReturnCompleteAndIncomplete", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "SalesReturnCompleteAndIncomplete", SystemName = "ManageSalesReturnCompleteAndIncomplete", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageInventoryRate", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage ManageInventoryRate", SystemName = "ManageInventoryRate", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "InventoryRate", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage InventoryRate", SystemName = "InventoryRate", Category = "Tijara", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region daily rate
            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.DailyRateEnumId)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.DailyRateEnumId)).AsInt32().Nullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.PublishedById)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.PublishedById)).AsInt32().Nullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.PublishedDate)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.PublishedDate)).AsDateTime().Nullable();
            }

            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.RejectedById)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.RejectedById)).AsInt32().Nullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.RejectedDate)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.RejectedDate)).AsDateTime().Nullable();
            }

            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.IncludeGst)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.IncludeGst)).AsBoolean().WithDefaultValue(false);
            }

            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.IncludeFirstMile)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.IncludeFirstMile)).AsBoolean().WithDefaultValue(false);
            }

            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.Published)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.Published)).AsBoolean().WithDefaultValue(false);
            }
            #endregion

            #region Order Calculation

            if (!Schema.Table(nameof(OrderCalculation)).Column(nameof(OrderCalculation.DeliveryCost)).Exists())
            {
                Alter.Table(nameof(OrderCalculation))
                    .AddColumn(nameof(OrderCalculation.DeliveryCost)).AsDecimal().WithDefaultValue(0.0000);
            }

            #endregion

            #region payment

            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.IsTransporter)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.IsTransporter)).AsBoolean().WithDefaultValue(false);
            }

            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.ShipmentId)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.ShipmentId)).AsInt64().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.IsLabour)).Exists())
            {
                Alter.Table(nameof(Payment))
                    .AddColumn(nameof(Payment.IsLabour)).AsBoolean().WithDefaultValue(false);
            }


            #endregion

            Assembly assembly1 = Assembly.GetAssembly(typeof(StoreEntity));
            var derivedTypes1 = assembly1.GetTypes()
            .Where(p => typeof(StoreEntity).IsAssignableFrom(p) && !p.IsInterface);
            foreach (Type derivedType in derivedTypes1)
            {
                var tableName = derivedType.Name;
                var baseNameCompatibility = new BaseNameCompatibility();
                if (baseNameCompatibility.TableNames.TryGetValue(derivedType, out var entityName))
                {
                    tableName = entityName;
                }

                if (!Schema.Table(tableName).Column("StoreId").Exists())
                {
                    Alter.Table(tableName)
                        .AddColumn("StoreId").AsInt32().NotNullable().WithDefaultValue(1);
                }
            }

            #region Store

            if (!Schema.Table(nameof(Store)).Column(nameof(Store.AdminId)).Exists())
            {
                Alter.Table(nameof(Store))
                    .AddColumn(nameof(Store.AdminId)).AsInt64().WithDefaultValue(0);
            }

            #endregion

            #endregion

            #region Uzair

            #region Activity Log

            var activitylogTableName = nameof(ActivityLog);
            var oldJsonColumnName = nameof(ActivityLog.OldJson);
            var newJsonColumnName = nameof(ActivityLog.NewJson);

            if (!Schema.Table(activitylogTableName).Column(oldJsonColumnName).Exists())
            {
                Alter.Table(activitylogTableName)
                    .AddColumn(oldJsonColumnName).AsString(10000).Nullable();
            }
            if (!Schema.Table(activitylogTableName).Column(newJsonColumnName).Exists())
            {
                Alter.Table(activitylogTableName)
                    .AddColumn(newJsonColumnName).AsString(10000).Nullable();
            }

            #endregion

            #region Industry

            var industryTableName = nameof(Industry);
            var includeInTopMenuColumnName = nameof(Industry.IncludeInTopMenu);
            if (!Schema.Table(industryTableName).Column(includeInTopMenuColumnName).Exists())
            {
                Alter.Table(industryTableName)
                    .AddColumn(includeInTopMenuColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            #endregion

            #region Industry

            if (!Schema.Table(nameof(Category)).Column(nameof(Category.ShortDescription)).Exists())
            {
                Alter.Table(nameof(Category))
                    .AddColumn(nameof(Category.ShortDescription)).AsString().Nullable().WithDefaultValue(null);
            }

            #endregion

            #region Online Lead

            if (!Schema.Table(nameof(OnlineLead)).Exists())
            {
                Create.TableFor<OnlineLead>();
            }
            if (!Schema.Table(nameof(OnlineLead)).Column(nameof(OnlineLead.Email)).Exists())
            {
                Alter.Table(nameof(OnlineLead))
                    .AddColumn(nameof(OnlineLead.Email)).AsString().Nullable().WithDefaultValue(null);
            }
            if (!Schema.Table(nameof(OnlineLead)).Column(nameof(OnlineLead.CityName)).Exists())
            {
                Alter.Table(nameof(OnlineLead))
                    .AddColumn(nameof(OnlineLead.CityName)).AsString().Nullable().WithDefaultValue(false);
            }
            if (!Schema.Table(nameof(OnlineLead)).Column(nameof(OnlineLead.CustomerId)).Exists())
            {
                Alter.Table(nameof(OnlineLead))
                    .AddColumn(nameof(OnlineLead.CustomerId)).AsInt32().Nullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(OnlineLead)).Column(nameof(OnlineLead.Comment)).Exists())
            {
                Alter.Table(nameof(OnlineLead))
                    .AddColumn(nameof(OnlineLead.Comment)).AsString().Nullable();
            }

            if (!Schema.Table(nameof(OnlineLead)).Column(nameof(OnlineLead.ReasonId)).Exists())
            {
                Alter.Table(nameof(OnlineLead))
                    .AddColumn(nameof(OnlineLead.ReasonId)).AsInt32().Nullable().WithDefaultValue(0);
            }

            #endregion

            #region Message Template

            if (!Schema.Table(nameof(MessageTemplate)).Column(nameof(MessageTemplate.Published)).Exists())
            {
                Alter.Table(nameof(MessageTemplate))
                    .AddColumn(nameof(MessageTemplate.Published)).AsBoolean().NotNullable().WithDefaultValue(false);


            }
            if (!Schema.Table(nameof(MessageTemplate)).Column(nameof(MessageTemplate.ShowTemplate)).Exists())
            {

                Alter.Table(nameof(MessageTemplate))
                    .AddColumn(nameof(MessageTemplate.ShowTemplate)).AsBoolean().NotNullable().WithDefaultValue(false);
            }

            #endregion

            #region Inventory

            if (!Schema.Table(nameof(InventoryInbound)).Exists())
            {
                Create.TableFor<InventoryInbound>();
            }

            if (!Schema.Table(nameof(InventoryOutbound)).Exists())
            {
                Create.TableFor<InventoryOutbound>();
            }

            if (!Schema.Table(nameof(InventoryGroup)).Exists())
            {
                Create.TableFor<InventoryGroup>();
            }

            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.GstRate)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.GstRate)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.GstAmount)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.GstAmount)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.WhtRate)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.WhtRate)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.WhtAmount)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.WhtAmount)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.WholeSaleTaxRate)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.WholeSaleTaxRate)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.WholeSaleTaxAmount)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.WholeSaleTaxAmount)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageInventoryDashboard", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Inventory Dashboard", SystemName = "ManageInventoryDashboard", Category = "Stock Management", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageInventories", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Inventories", SystemName = "ManageInventories", Category = "Stock Management", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageTransporterPayables", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage TransporterPayables", SystemName = "ManageTransporterPayables", Category = "Finance", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageLabourPayables", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage LabourPayables", SystemName = "ManageLabourPayables", Category = "Finance", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #region Shipment Customer Ledger

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.DeliveredAmount)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.DeliveredAmount)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.ActualShippedQuantity)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.ActualShippedQuantity)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.ActualDeliveredQuantity)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.ActualDeliveredQuantity)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.ActualShippedQuantityReason)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.ActualShippedQuantityReason)).AsString().Nullable();
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.ActualDeliveredQuantityReason)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.ActualDeliveredQuantityReason)).AsString().Nullable();
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.ShipmentTypeId)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.ShipmentTypeId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.PaymentStatusId)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.PaymentStatusId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.Source)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.Source)).AsString().Nullable().WithDefaultValue(null);
            }

            #endregion

            #region RequestForQuotation

            if (!Schema.Table(nameof(RequestForQuotation)).Column(nameof(RequestForQuotation.RejectedReason)).Exists())
            {
                Alter.Table(nameof(RequestForQuotation))
                    .AddColumn(nameof(RequestForQuotation.RejectedReason)).AsString().Nullable().WithDefaultValue(null);
            }

            #endregion

            #region Direct Order

            if (!Schema.Table(nameof(DirectOrder)).Exists())
            {
                Create.TableFor<DirectOrder>();
            }

            if (!Schema.Table(nameof(DirectOrderDeliverySchedule)).Exists())
            {
                Create.TableFor<DirectOrderDeliverySchedule>();
            }

            if (!Schema.Table(nameof(DirectOrderCalculation)).Exists())
            {
                Create.TableFor<DirectOrderCalculation>();
            }

            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.InterestAccrued_Summary)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.InterestAccrued_Summary)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.WholesaleTaxAmount)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.WholesaleTaxAmount)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.WholesaletaxIncluded)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.WholesaletaxIncluded)).AsBoolean().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.WhtIncluded)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.WhtIncluded)).AsBoolean().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.SupplierCommissionBag_Summary)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.SupplierCommissionBag_Summary)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.WholesaleTaxRate)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.WholesaleTaxRate)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.TotalPerBag)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.TotalPerBag)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.WhtIncluded)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.WhtIncluded)).AsBoolean().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.GrossAmount)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.GrossAmount)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.TotalCommissionReceivableFromBuyerToZaraye)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.TotalCommissionReceivableFromBuyerToZaraye)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.BuyerCommissionPayable_Summary)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.BuyerCommissionPayable_Summary)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.BuyerCommissionReceivable_Summary)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.BuyerCommissionReceivable_Summary)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.BuyingPrice)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.BuyingPrice)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(OrderCalculation)).Column(nameof(OrderCalculation.GrossAmount)).Exists())
            {
                Alter.Table(nameof(OrderCalculation))
                    .AddColumn(nameof(OrderCalculation.GrossAmount)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(nameof(DirectOrderCalculation)).Column(nameof(DirectOrderCalculation.FinanceIncome)).Exists())
            {
                Alter.Table(nameof(DirectOrderCalculation))
                    .AddColumn(nameof(DirectOrderCalculation.FinanceIncome)).AsDecimal().NotNullable().WithDefaultValue(0);
            }
            #endregion

            #region Permissions

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageOnlineLead", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Online Lead", SystemName = "ManageOnlineLead", Category = "Common", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBuyerLedger", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage BuyerLedger", SystemName = "ManageBuyerLedger", Category = "Ledger", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSupplierLedger", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage SupplierLedger", SystemName = "ManageSupplierLedger", Category = "Ledger", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }


            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageQuotations", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Quotations", SystemName = "ManageQuotations", Category = "Purchases", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageUserRoles", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage User Roles", SystemName = "ManageUserRoles", Category = "Configuration", Published = true, DisplayOrder = 2 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            if (!_dataProvider.GetTable<MessageTemplate>().Any(mt => string.Compare(mt.Name, "OnlineLead.Notification", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new MessageTemplate { Name = "OnlineLead.Notification", BccEmailAddresses = null, Subject = "Welcome to %Store.Name%", EmailAccountId = 1, Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new online lead has been recieved.<br /><br /><strong>Service or Product</strong>: %OnlineLead.Service% <br /><strong>Quantity</strong>: %OnlineLead.Quantity%<br /><strong>Unit</strong>: %OnlineLead.Unit%<br /><strong>Name</strong>: %OnlineLead.Name%<br /><strong>Contact number</strong>: %OnlineLead.ContactNumber%<br /><strong>Created date</strong>: %OnlineLead.CreatedOn%</p>", IsActive = true, Published = true, DelayBeforeSend = null, DelayPeriodId = 0, AttachedDownloadId = 0, LimitedToStores = false });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "CreateRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Create Request", SystemName = "CreateRequest", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "RejectRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Reject Request", SystemName = "RejectRequest", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "InitiateRfq", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Initiate Rfq", SystemName = "InitiateRfq", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "CreateSalesOrder", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Create Sales Order", SystemName = "CreateSalesOrder", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "CreateQuotation", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Create Quotation", SystemName = "CreateQuotation", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ChooseQuotationtoCreatePurchaseOrder", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Choose Quotation to Create Purchase Order", SystemName = "ChooseQuotationtoCreatePurchaseOrder", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "CreatePurchaseOrder", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Create Purchase Order", SystemName = "CreatePurchaseOrder", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "GenerateDeliveryRequestOnSaleOrder", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Generate Delivery Request On Sale Order", SystemName = "GenerateDeliveryRequestOnSaleOrder", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "GeneratePickupRequestOnPurchaseOrder", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Generate Pickup Request On Purchase Order", SystemName = "GeneratePickupRequestOnPurchaseOrder", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "SignSalesOrderContract", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Sign Sales Order Contract", SystemName = "SignSalesOrderContract", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "SignPurchaseOrderContract", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Sign Purchase Order Contract", SystemName = "SignPurchaseOrderContract", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "RespondToDeleiveryRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Respond To Deleivery Request", SystemName = "RespondToDeleiveryRequest", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "RespondToPickupRequest", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Respond To Pickup Request", SystemName = "RespondToPickupRequest", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ReassignAgentOnOperationTicket", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Reassign Agent On Operation Ticket", SystemName = "ReassignAgentOnOperationTicket", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "PurchaseOrderShipmentRequestApproved", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Purchase Order Shipment Request Approved", SystemName = "PurchaseOrderShipmentRequestApproved", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "PurchaseOrderShipmentShipped", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Purchase Order Shipment Shipped", SystemName = "PurchaseOrderShipmentShipped", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "PurchaseOrderShipmentDelivered", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Purchase Order Shipment Delivered", SystemName = "PurchaseOrderShipmentDelivered", Category = "Tijara", Published = true, DisplayOrder = 0 });


            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "SaleOrderShipmentRequestApproved", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Sale Order Shipment Request Approved", SystemName = "SaleOrderShipmentRequestApproved", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "SaleOrderShipmentShipped", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Sale Order Shipment Shipped", SystemName = "SaleOrderShipmentShipped", Category = "Tijara", Published = true, DisplayOrder = 0 });

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "SaleOrderShipmentDelivered", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Sale Order Shipment Delivered", SystemName = "SaleOrderShipmentDelivered", Category = "Tijara", Published = true, DisplayOrder = 0 });
            #endregion

            #region Order

            if (!Schema.Table(nameof(Order)).Column(nameof(Order.ProcessingDateUtc)).Exists())
            {
                Alter.Table(nameof(Order))
                    .AddColumn(nameof(Order.ProcessingDateUtc)).AsDateTime().Nullable();
            }
            //if (!Schema.Table(nameof(OrderDeliveryRequest)).Column(nameof(OrderDeliveryRequest.WarehouseId)).Exists())
            //{
            //    Alter.Table(nameof(OrderDeliveryRequest))
            //        .AddColumn(nameof(OrderDeliveryRequest.WarehouseId)).AsInt32().NotNullable().WithDefaultValue(0);
            //}

            //if (!Schema.Table(nameof(OrderDeliverySchedule)).Column(nameof(OrderDeliverySchedule.ExpectedDeliveryCost)).Exists())
            //{
            //    Alter.Table(nameof(OrderDeliverySchedule))
            //        .AddColumn(nameof(OrderDeliverySchedule.ExpectedDeliveryCost)).AsDecimal().NotNullable().WithDefaultValue(0);
            //}
            //if (!Schema.Table(nameof(OrderDeliverySchedule)).Column(nameof(OrderDeliverySchedule.ExpectedShipmentDateUtc)).Exists())
            //{
            //    Alter.Table(nameof(OrderDeliverySchedule))
            //        .AddColumn(nameof(OrderDeliverySchedule.ExpectedShipmentDateUtc)).AsDateTime().Nullable();
            //}

            if (!Schema.Table(nameof(DirectOrderDeliverySchedule)).Column(nameof(DirectOrderDeliverySchedule.ExpectedShipmentDateUtc)).Exists())
            {
                Alter.Table(nameof(DirectOrderDeliverySchedule))
                    .AddColumn(nameof(DirectOrderDeliverySchedule.ExpectedShipmentDateUtc)).AsDateTime().Nullable();
            }

            if (!Schema.Table(nameof(DirectOrderDeliverySchedule)).Column(nameof(DirectOrderDeliverySchedule.ExpectedDeliveryCost)).Exists())
            {
                Alter.Table(nameof(DirectOrderDeliverySchedule))
                    .AddColumn(nameof(DirectOrderDeliverySchedule.ExpectedDeliveryCost)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            //if (!Schema.Table(nameof(OrderDeliveryRequest)).Column(nameof(OrderDeliveryRequest.OrderDeliveryScheduleId)).Exists())
            //{
            //    Alter.Table(nameof(OrderDeliveryRequest))
            //        .AddColumn(nameof(OrderDeliveryRequest.OrderDeliveryScheduleId)).AsInt32().NotNullable();
            //}
            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.DeliveryRequestId)).Exists())
            {
                Alter.Table(nameof(Shipment))
                    .AddColumn(nameof(Shipment.DeliveryRequestId)).AsInt32().Nullable();
            }

            if (!Schema.Table(nameof(OrderCancellation)).Exists())
            {
                Create.TableFor<OrderCancellation>();
            }

            #endregion

            #region Rate

            if (!Schema.Table(nameof(Rate)).Exists())
            {
                Create.TableFor<Rate>();
            }

            if (!Schema.Table(nameof(RateGroup)).Exists())
            {
                Create.TableFor<RateGroup>();
            }

            if (!Schema.Table(nameof(FavouriteRateGroup)).Exists())
            {
                Create.TableFor<FavouriteRateGroup>();
            }

            //if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageRates", StringComparison.InvariantCultureIgnoreCase) == 0))
            //{
            //    var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin.Catalog.Rategroup", SystemName = "ManageRates", Category = "Catalog", Published = true, DisplayOrder = 0 });

            //    //add it to the Admin role by default
            //    var adminRole = _dataProvider
            //        .GetTable<CustomerRole>()
            //        .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

            //    _dataProvider.InsertEntity(
            //        new PermissionRecordCustomerRoleMapping
            //        {
            //            CustomerRoleId = adminRole.Id,
            //            PermissionRecordId = manageConnectionStringPermission.Id
            //        }
            //    );
            //}

            #endregion

            #region Direct Cogs Inventory Tagging

            if (!Schema.Table(nameof(DirectCogsInventoryTagging)).Exists())
            {
                Create.TableFor<DirectCogsInventoryTagging>();
            }

            if (!Schema.Table(nameof(CogsInventoryTagging)).Column(nameof(CogsInventoryTagging.GrossQuantity)).Exists())
            {
                Alter.Table(nameof(CogsInventoryTagging))
                    .AddColumn(nameof(CogsInventoryTagging.GrossQuantity)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Customer Role

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "LiveOps", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Live Ops", SystemName = "LiveOps" });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "OrderBookerDemand", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Order Booker Demand", SystemName = "OrderBookerDemand", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "DemandAssociate", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Demand Associate", SystemName = "DemandAssociate", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "SupplyAssociate", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Supply Associate", SystemName = "SupplyAssociate", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "OPerationAssociate", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "OPeration Associate", SystemName = "OPerationAssociate", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "OPerationLead", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "OPeration Lead", SystemName = "OPerationLead", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "BusinessLead", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Business Lead", SystemName = "BusinessLead", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "ControlTower", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Control Tower", SystemName = "ControlTower", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "GroundOperations", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Ground Operations", SystemName = "GroundOperations", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "Broker", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Broker", SystemName = "Broker", Active = true });

            if (!_dataProvider.GetTable<CustomerRole>().Any(cr => string.Compare(cr.SystemName, "ThirdPartyLedger", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(new CustomerRole { Name = "Third Party Ledger", SystemName = "ThirdPartyLedger", Active = true });

            if (_dataProvider.GetTable<Customer>().Where(cr => cr.Email == "thirdpartyledger@zaraye.co").FirstOrDefault() == null)
            {
                var thirdpartyledgerUser = new Customer
                {
                    CustomerGuid = Guid.NewGuid(),
                    FullName = "Third Party Ledger",
                    Email = "thirdpartyledger@zaraye.co",
                    Username = "thirdpartyledger@zaraye.co",
                    Active = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    RegisteredInStoreId = 1,
                    AdminComment = "Built-in system guest record used for labour charges ledger.",
                    IsSystemAccount = true,
                    SystemName = ZarayeCustomerDefaults.ThirdPartyLedgerCustomerName
                };
                _dataProvider.InsertEntity(thirdpartyledgerUser);
            }

            #endregion

            #region Request For Quotation

            if (!Schema.Table(nameof(RequestForQuotation)).Column(nameof(RequestForQuotation.BookerId)).Exists())
            {
                Alter.Table(nameof(RequestForQuotation))
                     .AddColumn(nameof(RequestForQuotation.BookerId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Request

            if (!Schema.Table(nameof(Request)).Column(nameof(Request.IdealBuyingPrice)).Exists())
            {
                Alter.Table(nameof(Request))
                     .AddColumn(nameof(Request.IdealBuyingPrice)).AsDecimal().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Business Model

            var directorderCalculationTableName = nameof(DirectOrderCalculation);

            if (!Schema.Table(directorderCalculationTableName).Column(nameof(DirectOrderCalculation.BrokerId)).Exists())
            {
                Alter.Table(directorderCalculationTableName)
                    .AddColumn(nameof(DirectOrderCalculation.BrokerId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(directorderCalculationTableName).Column(nameof(DirectOrderCalculation.BuyerCommissionReceivableUserId)).Exists())
            {
                Alter.Table(directorderCalculationTableName)
                    .AddColumn(nameof(DirectOrderCalculation.BuyerCommissionReceivableUserId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(directorderCalculationTableName).Column(nameof(DirectOrderCalculation.BuyerCommissionPayableUserId)).Exists())
            {
                Alter.Table(directorderCalculationTableName)
                    .AddColumn(nameof(DirectOrderCalculation.BuyerCommissionPayableUserId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(directorderCalculationTableName).Column(nameof(DirectOrderCalculation.SupplierCommissionReceivableUserId)).Exists())
            {
                Alter.Table(directorderCalculationTableName)
                    .AddColumn(nameof(DirectOrderCalculation.SupplierCommissionReceivableUserId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            if (!Schema.Table(directorderCalculationTableName).Column(nameof(DirectOrderCalculation.SupplierCommissionPayableUserId)).Exists())
            {
                Alter.Table(directorderCalculationTableName)
                    .AddColumn(nameof(DirectOrderCalculation.SupplierCommissionPayableUserId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Order cancellation

            if (!Schema.Table(nameof(OrderCancellation)).Column(nameof(OrderCancellation.CreatedOnUtc)).Exists())
            {
                Alter.Table(nameof(OrderCancellation))
                 .AddColumn(nameof(OrderCancellation.CreatedOnUtc)).AsDateTime().NotNullable().WithDefaultValue(DateTime.UtcNow);
            }
            if (!Schema.Table(nameof(OrderCancellation)).Column(nameof(OrderCancellation.CreateById)).Exists())
            {
                Alter.Table(nameof(OrderCancellation))
                 .AddColumn(nameof(OrderCancellation.CreateById)).AsInt32().Nullable();
            }

            #endregion

            #endregion

            #region Zaeem

            #region SupplierProduct

            if (!Schema.Table("supplier_product_mapping").Exists())
            {
                Create.TableFor<SupplierProduct>();
            }

            #endregion

            #region BidRequestTracker

            if (!Schema.Table(nameof(BidRequestTracker)).Exists())
            {
                Create.TableFor<BidRequestTracker>();
            }

            #endregion

            #region AppSlider

            if (!Schema.Table(nameof(AppSlider)).Exists())
            {
                Create.TableFor<AppSlider>();
            }
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageAppSliders", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage AppSliders", SystemName = "ManageAppSliders", Category = "AppSliders", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region VehiclePortfolio

            if (!Schema.Table(nameof(VehiclePortfolio)).Exists())
            {
                Create.TableFor<VehiclePortfolio>();
            }
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageVehiclePortfolio", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage VehiclePortfolio", SystemName = "ManageVehiclePortfolio", Category = "Catalog", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region Brand

            //if (!Schema.Table(nameof(Industry)).Exists())
            //{
            //    Rename.Table("Manufacturer").To("Brand");
            //}

            #endregion

            #region BankAccount

            if (!Schema.Table(nameof(BankAccount)).Exists())
            {
                Create.TableFor<BankAccount>();
            }
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBankAccounts", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin.Configuration.Settings.BankAccounts", SystemName = "ManageBankAccounts", Category = "Configuration", Published = true, DisplayOrder = 0 });
            }

            #endregion

            #region Quotation

            if (!Schema.Table(nameof(Quotation)).Column(nameof(Quotation.BusinessModelId)).Exists())
            {
                Alter.Table(nameof(Quotation))
                    .AddColumn(nameof(Quotation.BusinessModelId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Product

            if (!Schema.Table(nameof(Product)).Column(nameof(Product.IndustryId)).Exists())
            {
                Alter.Table(nameof(Product))
                    .AddColumn(nameof(Product.IndustryId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #endregion

            #region Azeem

            #region MyRegion

            //if (Schema.Table("Vendor").Exists())
            //{
            //    Delete.<"Vendor">;
            //}

            //Delete.TableFor<Vendor>();
            //Create.TableFor<VendorAttribute>();
            //Create.TableFor<VendorAttributeValue>();
            //Create.TableFor<VendorNote>();

            #endregion

            #region Product

            var columnName1 = "IsGiftCard";
            if (Schema.Table(nameof(Product)).Column(columnName1).Exists())
            {
                Delete.Column(columnName1).FromTable(nameof(Product));
            }

            var columnName2 = "GiftCardTypeId";
            if (Schema.Table(nameof(Product)).Column(columnName2).Exists())
            {
                Delete.Column(columnName2).FromTable(nameof(Product));
            }

            var columnName3 = "OverriddenGiftCardAmount";
            if (Schema.Table(nameof(Product)).Column(columnName3).Exists())
            {
                Delete.Column(columnName3).FromTable(nameof(Product));
            }

            var columnName4 = "VendorId";
            if (Schema.Table(nameof(Product)).Column(columnName4).Exists())
            {
                Delete.Column(columnName4).FromTable(nameof(Product));
            }

            var columnName5 = "VendorId";
            if (Schema.Table(nameof(Customer)).Column(columnName5).Exists())
            {
                Delete.Column(columnName5).FromTable(nameof(Customer));
            }

            #endregion

            #region InventoryInbound

            if (!Schema.Table(nameof(InventoryInbound)).Column(nameof(InventoryInbound.BusinessModelId)).Exists())
            {
                Alter.Table(nameof(InventoryInbound))
                    .AddColumn(nameof(InventoryInbound.BusinessModelId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region Order

            if (!Schema.Table(nameof(Order)).Column(nameof(Order.ParentOrderId)).Exists())
            {
                Alter.Table(nameof(Order))
                    .AddColumn(nameof(Order.ParentOrderId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            #endregion

            #region ReturnSaleOrder

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageFirstMileConfigurations", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage First Mile Configurations", SystemName = "ManageFirstMileConfigurations", Category = "PriceDiscovery", Published = true, DisplayOrder = 0 });

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == ZarayeCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }

            #endregion

            #region StateProvince

            if (!Schema.Table(nameof(StateProvince)).Column(nameof(StateProvince.PublishedOnPriceDiscovery)).Exists())
            {
                Alter.Table(nameof(StateProvince))
                    .AddColumn(nameof(StateProvince.PublishedOnPriceDiscovery)).AsBoolean().NotNullable().WithDefaultValue(false);
            }

            if (!Schema.Table(nameof(Product)).Column(nameof(Product.PublishedOnPriceDiscovery)).Exists())
            {
                Alter.Table(nameof(Product))
                    .AddColumn(nameof(Product.PublishedOnPriceDiscovery)).AsBoolean().NotNullable().WithDefaultValue(false);
            }

            #endregion

            if (!Schema.Table(nameof(Order)).Column(nameof(Order.Source)).Exists())
            {
                Alter.Table(nameof(Order))
                    .AddColumn(nameof(Order.Source)).AsString().Nullable().WithDefaultValue(null);
            }

            if (!Schema.Table(nameof(Product)).Column(nameof(Product.Barcode)).Exists())
            {
                Alter.Table(nameof(Product))
                    .AddColumn(nameof(Product.Barcode)).AsString().Nullable().WithDefaultValue(null);
            }

            if (!Schema.Table(nameof(DailyRate)).Column(nameof(DailyRate.AttributeXml)).Exists())
            {
                Alter.Table(nameof(DailyRate))
                    .AddColumn(nameof(DailyRate.AttributeXml)).AsString().Nullable().WithDefaultValue(null);
            }

            if (!Schema.Table(nameof(Request)).Column(nameof(Request.Source)).Exists())
            {
                Alter.Table(nameof(Request))
                    .AddColumn(nameof(Request.Source)).AsString().Nullable().WithDefaultValue(null);
            }

            if (!Schema.Table(nameof(RequestForQuotation)).Column(nameof(RequestForQuotation.Source)).Exists())
            {
                Alter.Table(nameof(RequestForQuotation))
                    .AddColumn(nameof(RequestForQuotation.Source)).AsString().Nullable().WithDefaultValue(null);
            }

            if (!Schema.Table(nameof(Quotation)).Column(nameof(Quotation.Source)).Exists())
            {
                Alter.Table(nameof(Quotation))
                    .AddColumn(nameof(Quotation.Source)).AsString().Nullable().WithDefaultValue(null);
            }

            if (!Schema.Table(nameof(ReturnRequest)).Exists())
            {
                Create.TableFor<ReturnRequest>();
            }
            if (!Schema.Table(nameof(ReturnRequest)).Column(nameof(ReturnRequest.ReturnReasonId)).Exists())
            {
                Alter.Table(nameof(ReturnRequest))
                    .AddColumn(nameof(ReturnRequest.ReturnReasonId)).AsInt32().Nullable();
            }
            if (!Schema.Table(nameof(ReturnRequest)).Column(nameof(ReturnRequest.ReturnOrderId)).Exists())
            {
                Alter.Table(nameof(ReturnRequest))
                    .AddColumn(nameof(ReturnRequest.ReturnOrderId)).AsInt32().Nullable();
            }
            if (!Schema.Table(nameof(ReturnRequestReason)).Exists())
            {
                Create.TableFor<ReturnRequestReason>();
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageWarehouses", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Warehouses", SystemName = "ManageWarehouses", Category = "Stock Management", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageRequestForQuotations", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Manage Request For Quotations", SystemName = "ManageRequestForQuotations", Category = "Purchases", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageQuotations", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Quotations", SystemName = "ManageQuotations", Category = "Purchases", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePurchaseOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Purchase Orders", SystemName = "ManagePurchaseOrders", Category = "Purchases", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageSaleOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Sale Orders", SystemName = "ManageSaleOrders", Category = "Sales", Published = true, DisplayOrder = 0 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageReceivables", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Receivables", SystemName = "ManageReceivables", Category = "Finance", Published = true, DisplayOrder = 1 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManagePayables", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Payables", SystemName = "ManagePayables", Category = "Finance", Published = true, DisplayOrder = 2 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "AllowViewPaymentScheduler", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Allow View Payment Scheduler", SystemName = "AllowViewPaymentScheduler", Category = "Finance", Published = true, DisplayOrder = 2 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageLedgers", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Ledgers", SystemName = "ManageLedgers", Category = "Finance", Published = true, DisplayOrder = 4 });
            }

            if (!Schema.Table(nameof(Customer)).Column(nameof(Customer.IsGuestCustomer)).Exists())
            {
                Alter.Table(nameof(Customer))
                    .AddColumn(nameof(Customer.IsGuestCustomer)).AsBoolean().NotNullable().WithDefaultValue(0);
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageBlogSettings", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Blog Settings", SystemName = "ManageBlogSettings", Category = "Configuration", Published = true, DisplayOrder = 1 });
            }

            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageUnitSettings", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new PermissionRecord { Name = "Admin area. Manage Unit Settings", SystemName = "ManageUnitSettings", Category = "Configuration", Published = true, DisplayOrder = 2 });
            }

            if (!Schema.Table(nameof(Payment)).Column(nameof(Payment.CustomPaymentNumber)).Exists())
                Alter.Table(nameof(Payment)).AddColumn(nameof(Payment.CustomPaymentNumber)).AsString().Nullable().WithDefaultValue(null);

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.CustomShipmentNumber)).Exists())
                Alter.Table(nameof(Shipment)).AddColumn(nameof(Shipment.CustomShipmentNumber)).AsString().Nullable().WithDefaultValue(null);

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.ShipmentReturnTypeId)).Exists())
                Alter.Table(nameof(Shipment)).AddColumn(nameof(Shipment.ShipmentReturnTypeId)).AsInt32().Nullable().WithDefaultValue(null);

            if (!Schema.Table(nameof(Shipment)).Column(nameof(Shipment.CostBore)).Exists())
                Alter.Table(nameof(Shipment)).AddColumn(nameof(Shipment.CostBore)).AsDecimal().Nullable().WithDefaultValue(0);

            if (!Schema.Table(nameof(Category)).Column(nameof(Category.UnitId)).Exists())
            {
                Alter.Table(nameof(Category))
                    .AddColumn(nameof(Category.UnitId)).AsInt32().NotNullable().WithDefaultValue(0);
            }

            //Assembly assembly = Assembly.GetAssembly(typeof(DefaultColumns));
            //var derivedTypes = assembly.GetTypes()
            //.Where(p => typeof(DefaultColumns).IsAssignableFrom(p) && !p.IsInterface);
            //foreach (Type derivedType in derivedTypes)
            //{
            //    var tableName = derivedType.Name;
            //    var baseNameCompatibility = new BaseNameCompatibility();
            //    if (baseNameCompatibility.TableNames.TryGetValue(derivedType, out var entityName))
            //    {
            //        tableName = entityName;
            //    }
            //    if (!Schema.Table(tableName).Column("CreatedOnUtc").Exists())
            //    {
            //        Alter.Table(tableName)
            //            .AddColumn("CreatedOnUtc").AsDateTime().NotNullable().WithDefaultValue(DateTime.UtcNow);
            //    }

            //    if (!Schema.Table(tableName).Column("UpdatedOnUtc").Exists())
            //    {
            //        Alter.Table(tableName)
            //            .AddColumn("UpdatedOnUtc").AsDateTime().NotNullable().WithDefaultValue(DateTime.UtcNow);
            //    }

            //    if (!Schema.Table(tableName).Column("CreatedById").Exists())
            //    {
            //        Alter.Table(tableName)
            //            .AddColumn("CreatedById").AsInt32().NotNullable().WithDefaultValue(0);
            //    }

            //    if (!Schema.Table(tableName).Column("UpdatedById").Exists())
            //    {
            //        Alter.Table(tableName)
            //            .AddColumn("UpdatedById").AsInt32().NotNullable().WithDefaultValue(0);
            //    }

            //    if (!Schema.Table(tableName).Column("Deleted").Exists())
            //    {
            //        Alter.Table(tableName)
            //            .AddColumn("Deleted").AsBoolean().NotNullable().WithDefaultValue(0);
            //    }

            //    if (!Schema.Table(tableName).Column("DeletedById").Exists())
            //    {
            //        Alter.Table(tableName)
            //            .AddColumn("DeletedById").AsInt32().NotNullable().WithDefaultValue(0);
            //    }
            //}

            #endregion
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
