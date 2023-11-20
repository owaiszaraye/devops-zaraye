using System.Collections.Generic;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Security;

namespace Zaraye.Services.Security
{
    /// <summary>
    /// Standard permission provider
    /// </summary>
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions

        //stock => catalog
        public static readonly PermissionRecord ManageProducts = new() { Name = "Admin area. Manage Products", SystemName = "ManageProducts", Category = "Catalog" };
        public static readonly PermissionRecord ManageCategories = new() { Name = "Admin area. Manage Categories", SystemName = "ManageCategories", Category = "Catalog" };
        public static readonly PermissionRecord ManageIndustries = new() { Name = "Admin area. Manage Industries", SystemName = "ManageIndustries", Category = "Catalog" };
        public static readonly PermissionRecord ManageBrands = new PermissionRecord { Name = "Admin area. Manage Brands", SystemName = "ManageBrands", Category = "Catalog" };
        public static readonly PermissionRecord ManageVehiclePortfolio = new PermissionRecord { Name = "Admin area. Manage VehiclePortfolio", SystemName = "ManageVehiclePortfolio", Category = "Catalog" };
        public static readonly PermissionRecord ManageAttributes = new() { Name = "Admin area. Manage Attributes", SystemName = "ManageAttributes", Category = "Catalog" };
        public static readonly PermissionRecord ManageRates = new PermissionRecord { Name = "Admin area. Manage Rates", SystemName = "ManageRates", Category = "Catalog" };

        //stock => stock management
        public static readonly PermissionRecord ManageInventoryDashboard = new PermissionRecord { Name = "Admin area. Manage Inventory Dashboard", SystemName = "ManageInventoryDashboard", Category = "Stock Management" };
        public static readonly PermissionRecord ManageInventories = new PermissionRecord { Name = "Admin area. Manage Inventories", SystemName = "ManageInventories", Category = "Stock Management" };
        public static readonly PermissionRecord ManageBrokerInventories = new PermissionRecord { Name = "Admin area. Manage Broker Inventories", SystemName = "ManageBrokerInventories", Category = "Stock Management" };
        public static readonly PermissionRecord ManageWarehouses = new() { Name = "Admin area. Manage Warehouses", SystemName = "ManageWarehouses", Category = "Stock Management" };

        //Price Discovery
        public static readonly PermissionRecord ManagePriceDiscoveryRates = new PermissionRecord { Name = "Admin area. Manage PriceDiscovery Rates", SystemName = "ManagePriceDiscoveryRates", Category = "Price Discovery" };
        public static readonly PermissionRecord ManagePriceDiscoveryPendingRates = new PermissionRecord { Name = "Admin area. Manage PriceDiscovery Pending Rates", SystemName = "ManagePriceDiscoveryPendingRates", Category = "Price Discovery" };
        public static readonly PermissionRecord ManagePriceDiscoveryTodayRates = new PermissionRecord { Name = "Admin area. Manage PriceDiscovery Today Rates", SystemName = "ManagePriceDiscoveryTodayRates", Category = "Price Discovery" };
        public static readonly PermissionRecord ManageFirstMileConfigurations = new PermissionRecord { Name = "Admin area. Manage First Mile Configurations", SystemName = "ManageFirstMileConfigurations", Category = "Price Discovery" };

        //purchases
        public static readonly PermissionRecord ManageSuppliers = new PermissionRecord { Name = "Admin area. Manage Suppliers", SystemName = "ManageSuppliers", Category = "Purchases" };
        public static readonly PermissionRecord ManagePurchaseRequest = new() { Name = "Admin area. Manage Purchae Request", SystemName = "ManagePurchaseRequest", Category = "Purchases" };
        public static readonly PermissionRecord ManageRequestForQuotations = new() { Name = "Admin area. Manage Manage Request For Quotations", SystemName = "ManageRequestForQuotations", Category = "Purchases" };
        public static readonly PermissionRecord ManageQuotations = new() { Name = "Admin area. Manage Quotations", SystemName = "ManageQuotations", Category = "Purchases" };
        public static readonly PermissionRecord ManagePurchaseOrders = new() { Name = "Admin area. Manage Purchase Orders", SystemName = "ManagePurchaseOrders", Category = "Purchases" };

        //sales
        public static readonly PermissionRecord ManageBuyers = new PermissionRecord { Name = "Admin area. Manage Buyers", SystemName = "ManageBuyers", Category = "Sales" };
        public static readonly PermissionRecord ManageRequest = new() { Name = "Admin area. Manage Request", SystemName = "ManageRequest", Category = "Sales" };
        public static readonly PermissionRecord ManageSaleOrders = new() { Name = "Admin area. Manage Sale Orders", SystemName = "ManageSaleOrders", Category = "Sales" };
        public static readonly PermissionRecord ManageOnlineLead = new PermissionRecord { Name = "Admin area. Manage Online Lead", SystemName = "ManageOnlineLead", Category = "Common" };

        //finance
        public static readonly PermissionRecord ManageReceivables = new PermissionRecord { Name = "Admin area. Manage Receivables", SystemName = "ManageReceivables", Category = "Finance" };
        public static readonly PermissionRecord ManagePayables = new PermissionRecord { Name = "Admin area. Manage Payables", SystemName = "ManagePayables", Category = "Finance" };
        public static readonly PermissionRecord ManageTransporterPayables = new PermissionRecord { Name = "Admin area. Manage TransporterPayables", SystemName = "ManageTransporterPayables", Category = "Finance" };
        public static readonly PermissionRecord ManageLabourPayables = new PermissionRecord { Name = "Admin area. Manage LabourPayables", SystemName = "ManageLabourPayables", Category = "Finance" };
        public static readonly PermissionRecord AllowViewPaymentScheduler = new PermissionRecord { Name = "Admin area. Allow View Payment Scheduler", SystemName = "AllowViewPaymentScheduler", Category = "Finance" };
        public static readonly PermissionRecord ManageBuyerLedger = new() { Name = "Admin area. Manage Buyer Ledger", SystemName = "ManageBuyerLedger", Category = "Finance" };
        public static readonly PermissionRecord ManageSupplierLedger = new() { Name = "Admin area. Manage Supplier Ledger", SystemName = "ManageSupplierLedger", Category = "Finance" };
        public static readonly PermissionRecord ManageBorkerLedger = new() { Name = "Admin area. Manage Broker Ledger", SystemName = "ManageBrokerLedger", Category = "Finance" };
        public static readonly PermissionRecord ManageTransporterLedger = new() { Name = "Admin area. Manage Transporter Ledger", SystemName = "ManageTransporterLedger", Category = "Finance" };
        public static readonly PermissionRecord ManageLabourLedger = new() { Name = "Admin area. Manage Labour Ledger", SystemName = "ManageLabourLedger", Category = "Finance" };

        //Return
        public static readonly PermissionRecord ManageReturns = new PermissionRecord { Name = "Admin area. Manage Returns", SystemName = "ManageReturns", Category = "Return" };
        public static readonly PermissionRecord ManageReturnSaleOrders = new() { Name = "Admin area. Manage Return Sale Orders", SystemName = "ManageReturnSaleOrders", Category = "Return" };
        public static readonly PermissionRecord ManageReturnPurchaseOrders = new() { Name = "Admin area. Manage Return Purchase Orders", SystemName = "ManageReturnPurchaseOrders", Category = "Return" };

        //marketing => promotions
        public static readonly PermissionRecord ManageCampaigns = new() { Name = "Admin area. Manage Campaigns", SystemName = "ManageCampaigns", Category = "Promotions" };
        public static readonly PermissionRecord ManagePushNotifications = new PermissionRecord { Name = "Admin area. Manage Push Notifications", SystemName = "ManagePushNotifications", Category = "Promotions" };

        //marketing => content management
        public static readonly PermissionRecord ManageTopics = new() { Name = "Admin area. Manage Topics", SystemName = "ManageTopics", Category = "Content Management" };
        public static readonly PermissionRecord ManageMessageTemplates = new() { Name = "Admin area. Manage Message Templates", SystemName = "ManageMessageTemplates", Category = "Content Management" };
        public static readonly PermissionRecord ManageNews = new() { Name = "Admin area. Manage News", SystemName = "ManageNews", Category = "Content Management" };
        public static readonly PermissionRecord ManageBlog = new() { Name = "Admin area. Manage Blog", SystemName = "ManageBlog", Category = "Content Management" };
        public static readonly PermissionRecord ManageAppSliders = new PermissionRecord { Name = "Admin area. Manage AppSliders", SystemName = "ManageAppSliders", Category = "AppSliders" };
        public static readonly PermissionRecord ManageFaqs = new PermissionRecord { Name = "Admin area. Manage ManageFaqs", SystemName = "ManageFaqs", Category = "Content Management" };
        public static readonly PermissionRecord ManageJaizas = new PermissionRecord { Name = "Admin area. Manage ManageJaizas", SystemName = "ManageJaizas", Category = "Content Management" };
        public static readonly PermissionRecord ManageFeedbacks = new PermissionRecord { Name = "Admin area. Manage ManageFeedbacks", SystemName = "ManageFeedbacks", Category = "Content Management" };

        //configuration
        public static readonly PermissionRecord AccessAdminPanel = new() { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };

        //configuration => settings
        public static readonly PermissionRecord ManageBlogSettings = new PermissionRecord { Name = "Admin area. Manage Blog Settings", SystemName = "ManageBlogSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageUnitSettings = new() { Name = "Admin area. Manage Unit Settings", SystemName = "ManageUnitSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageConfigurations = new PermissionRecord { Name = "Admin area. Manage ManageConfigurations", SystemName = "ManageConfigurations", Category = "Configuration" };
        public static readonly PermissionRecord ManageBankAccounts = new PermissionRecord { Name = "Admin.Configuration.Settings.BankAccounts", SystemName = "ManageBankAccounts", Category = "Configuration" };

        //configuration => users
        public static readonly PermissionRecord ManageTransporter = new() { Name = "Admin area. Manage Transporters", SystemName = "ManageTransporters", Category = "Configuration" };
        public static readonly PermissionRecord ManageBookers = new() { Name = "Admin area. Manage Bookers", SystemName = "ManageBookers", Category = "Customers" };
        public static readonly PermissionRecord ManageSupportAgent = new PermissionRecord { Name = "Admin area. Manage SupportAgent", SystemName = "ManageSupportAgent", Category = "Configuration" };
        public static readonly PermissionRecord ManageCustomers = new() { Name = "Admin area. Manage Customers", SystemName = "ManageCustomers", Category = "Configuration" };
        public static readonly PermissionRecord ManageUsers = new() { Name = "Admin area. Manage Users", SystemName = "ManageUsers", Category = "Configuration" };
        public static readonly PermissionRecord ManageBroker = new() { Name = "Admin area. Manage Broker", SystemName = "ManageBroker", Category = "Configuration" };
        public static readonly PermissionRecord ManageUserRoles = new() { Name = "Admin area. Manage User Roles", SystemName = "ManageUserRoles", Category = "Configuration" };
        public static readonly PermissionRecord ManageActivityLog = new() { Name = "Admin area. Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration" };

        //configuration => others
        public static readonly PermissionRecord ManageEmailAccounts = new() { Name = "Admin area. Manage Email Accounts", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageStores = new() { Name = "Admin area. Manage Stores", SystemName = "ManageStores", Category = "Configuration" };
        public static readonly PermissionRecord ManageCountries = new() { Name = "Admin area. Manage Countries", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new() { Name = "Admin area. Manage Languages", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageCurrencies = new() { Name = "Admin area. Manage Currencies", SystemName = "ManageCurrencies", Category = "Configuration" };
        public static readonly PermissionRecord ManageAcl = new() { Name = "Admin area. Manage ACL", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageAppVersions = new() { Name = "Admin area. Manage App Settings", SystemName = "ManageAppVersions", Category = "Configuration" };

        //configuration => system
        public static readonly PermissionRecord ManageMaintenance = new() { Name = "Admin area. Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new() { Name = "Admin area. Manage System Log", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new() { Name = "Admin area. Manage Message Queue", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleTasks = new() { Name = "Admin area. Manage Schedule Tasks", SystemName = "ManageScheduleTasks", Category = "Configuration" };

        public static readonly PermissionRecord AllowCustomerImpersonation = new() { Name = "Admin area. Allow Customer Impersonation", SystemName = "AllowCustomerImpersonation", Category = "Customers" };

        //public static readonly PermissionRecord ManageManufacturers = new() { Name = "Admin area. Manage Manufacturers", SystemName = "ManageManufacturers", Category = "Catalog" };
        public static readonly PermissionRecord ManageProductReviews = new() { Name = "Admin area. Manage Product Reviews", SystemName = "ManageProductReviews", Category = "Catalog" };
        public static readonly PermissionRecord ManageProductTags = new() { Name = "Admin area. Manage Product Tags", SystemName = "ManageProductTags", Category = "Catalog" };

        public static readonly PermissionRecord ManageCurrentCarts = new() { Name = "Admin area. Manage Current Carts", SystemName = "ManageCurrentCarts", Category = "Orders" };
        public static readonly PermissionRecord ManageOrders = new() { Name = "Admin area. Manage Orders", SystemName = "ManageOrders", Category = "Orders" };

        public static readonly PermissionRecord SalesSummaryReport = new() { Name = "Admin area. Access sales summary report", SystemName = "SalesSummaryReport", Category = "Orders" };
        public static readonly PermissionRecord ManageRecurringPayments = new() { Name = "Admin area. Manage Recurring Payments", SystemName = "ManageRecurringPayments", Category = "Orders" };
        public static readonly PermissionRecord ManageGiftCards = new() { Name = "Admin area. Manage Gift Cards", SystemName = "ManageGiftCards", Category = "Orders" };
        public static readonly PermissionRecord ManageReturnRequests = new() { Name = "Admin area. Manage Return Requests", SystemName = "ManageReturnRequests", Category = "Orders" };
        public static readonly PermissionRecord OrderCountryReport = new() { Name = "Admin area. Access order country report", SystemName = "OrderCountryReport", Category = "Orders" };
        public static readonly PermissionRecord ManageAffiliates = new() { Name = "Admin area. Manage Affiliates", SystemName = "ManageAffiliates", Category = "Promo" };

        public static readonly PermissionRecord ManageDiscounts = new() { Name = "Admin area. Manage Discounts", SystemName = "ManageDiscounts", Category = "Promo" };
        public static readonly PermissionRecord ManageNewsletterSubscribers = new() { Name = "Admin area. Manage Newsletter Subscribers", SystemName = "ManageNewsletterSubscribers", Category = "Promo" };
        public static readonly PermissionRecord ManagePolls = new() { Name = "Admin area. Manage Polls", SystemName = "ManagePolls", Category = "Content Management" };

        public static readonly PermissionRecord ManageWidgets = new() { Name = "Admin area. Manage Widgets", SystemName = "ManageWidgets", Category = "Content Management" };

        public static readonly PermissionRecord ManageForums = new() { Name = "Admin area. Manage Forums", SystemName = "ManageForums", Category = "Content Management" };


        public static readonly PermissionRecord ManageSettings = new() { Name = "Admin area. Manage Settings", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManagePaymentMethods = new() { Name = "Admin area. Manage Payment Methods", SystemName = "ManagePaymentMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new() { Name = "Admin area. Manage External Authentication Methods", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageMultifactorAuthenticationMethods = new() { Name = "Admin area. Manage Multi-factor Authentication Methods", SystemName = "ManageMultifactorAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageTaxSettings = new() { Name = "Admin area. Manage Tax Settings", SystemName = "ManageTaxSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageShippingSettings = new() { Name = "Admin area. Manage Shipping Settings", SystemName = "ManageShippingSettings", Category = "Configuration" };


        public static readonly PermissionRecord ManageAppliedCreditCustomers = new() { Name = "Admin area. Manage ManageAppliedCreditCustomers", SystemName = "ManageAppliedCreditCustomers", Category = "Sales" };

        public static readonly PermissionRecord ManagePlugins = new() { Name = "Admin area. Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };

        public static readonly PermissionRecord HtmlEditorManagePictures = new() { Name = "Admin area. HTML Editor. Manage pictures", SystemName = "HtmlEditor.ManagePictures", Category = "Configuration" };

        public static readonly PermissionRecord ManageAppSettings = new() { Name = "Admin area. Manage App Settings", SystemName = "ManageAppSettings", Category = "Configuration" };

        // public static readonly PermissionRecord ManageSupportAgent = new () { Name = "Admin area. Manage SupportAgent", SystemName = "ManageSupportAgent", Category = "Customers" };

        public static readonly PermissionRecord AllowEditDetails = new() { Name = "Admin area. Allow Edit Detials", SystemName = "AllowEditDetails", Category = "Standard" };


        public static readonly PermissionRecord ManageBuyerSupplierType = new PermissionRecord { Name = "Admin area. Manage ManageBuyerSupplierType", SystemName = "ManageBuyerSupplierType", Category = "Configuration" };
        public static readonly PermissionRecord ManageShipmentReturnReason = new PermissionRecord { Name = "Admin area. Manage ManageShipmentReturnReason", SystemName = "ManageShipmentReturnReason", Category = "Configuration" };
        public static readonly PermissionRecord ManageDeliveryCostReason = new PermissionRecord { Name = "Admin area. Manage ManageDeliveryCostReason", SystemName = "ManageDeliveryCostReason", Category = "Configuration" };
        public static readonly PermissionRecord ManageDeliveryTimeReason = new PermissionRecord { Name = "Admin area. Manage ManageDeliveryTimeReason", SystemName = "ManageDeliveryTimeReason", Category = "Configuration" };
        public static readonly PermissionRecord AllowCalculationAccessForTaxasion = new PermissionRecord { Name = "Admin area. AllowCalculationAccessForTaxasion", SystemName = "AllowCalculationAccessForTaxasion", Category = "Configuration" };
        public static readonly PermissionRecord ManageOnlineRejectReason = new PermissionRecord { Name = "Admin area. Manage ManageOnlineRejectReason", SystemName = "ManageOnlineRejectReason", Category = "Configuration" };

        //public store permissions
        public static readonly PermissionRecord DisplayPrices = new() { Name = "Public store. Display Prices", SystemName = "DisplayPrices", Category = "PublicStore" };
        public static readonly PermissionRecord EnableShoppingCart = new() { Name = "Public store. Enable shopping cart", SystemName = "EnableShoppingCart", Category = "PublicStore" };
        public static readonly PermissionRecord EnableWishlist = new() { Name = "Public store. Enable wishlist", SystemName = "EnableWishlist", Category = "PublicStore" };
        public static readonly PermissionRecord PublicStoreAllowNavigation = new() { Name = "Public store. Allow navigation", SystemName = "PublicStoreAllowNavigation", Category = "PublicStore" };
        public static readonly PermissionRecord AccessClosedStore = new() { Name = "Public store. Access a closed store", SystemName = "AccessClosedStore", Category = "PublicStore" };
        public static readonly PermissionRecord AccessProfiling = new() { Name = "Public store. Access MiniProfiler results", SystemName = "AccessProfiling", Category = "PublicStore" };

        //Security
        public static readonly PermissionRecord EnableMultiFactorAuthentication = new() { Name = "Security. Enable Multi-factor authentication", SystemName = "EnableMultiFactorAuthentication", Category = "Security" };

        //Tijara
        public static readonly PermissionRecord ManageTijara = new PermissionRecord { Name = "Admin area. Manage Tijara", SystemName = "ManageTijara", Category = "Tijara" };

        //Demand
        public static readonly PermissionRecord ManageBuyerRegistration = new PermissionRecord { Name = "Buyer registration", SystemName = "ManageBuyerRegistration", Category = "Tijara" };
        public static readonly PermissionRecord ManagePlaceRequest = new PermissionRecord { Name = "Place request", SystemName = "ManagePlaceRequest", Category = "Tijara" };
        public static readonly PermissionRecord ManagePlaceOrder = new PermissionRecord { Name = "Place order", SystemName = "ManagePlaceOrder", Category = "Tijara" };
        public static readonly PermissionRecord ManagePlaceDeliveryRequest = new PermissionRecord { Name = "Place delivery request", SystemName = "ManagePlaceDeliveryRequest", Category = "Tijara" };
        public static readonly PermissionRecord ManagePlaceSalesReturnRequest = new PermissionRecord { Name = "Place sales return request", SystemName = "ManagePlaceSalesReturnRequest", Category = "Tijara" };

        //Supply
        public static readonly PermissionRecord ManageSupplierRegistration = new PermissionRecord { Name = "Supplier registration", SystemName = "ManageSupplierRegistration", Category = "Tijara" };
        public static readonly PermissionRecord ManagePlaceQuotation = new PermissionRecord { Name = "Place quotation", SystemName = "ManagePlaceQuotation", Category = "Tijara" };
        public static readonly PermissionRecord ManageRaisePo = new PermissionRecord { Name = "Raise po", SystemName = "ManageRaisePo", Category = "Tijara" };

        //Operations
        public static readonly PermissionRecord ManageUploadGrn = new PermissionRecord { Name = "Upload grn", SystemName = "ManageUploadGrns", Category = "Tijara" };
        public static readonly PermissionRecord ManageUploadProofofPayment = new PermissionRecord { Name = "Upload proof of payment", SystemName = "ManageUploadProofofPayments", Category = "Tijara" };

        //Fiance
        public static readonly PermissionRecord ManageApproveorRejectPowithAttachment = new PermissionRecord { Name = "Approveor reject po with attachment", SystemName = "ManageApproveorRejectPowithAttachments", Category = "Tijara" };

        //Business Head
        public static readonly PermissionRecord ManageReassignonTicket = new PermissionRecord { Name = "Reassign on ticket", SystemName = "ManageReassignonTickets", Category = "Tijara" };

        public static readonly PermissionRecord ManagePOTicket = new PermissionRecord { Name = "Po ticket", SystemName = "ManagePOTicket", Category = "Tijara" };
        public static readonly PermissionRecord ManageSaleReturnTicket = new PermissionRecord { Name = "Sale Return Ticket", SystemName = "ManageSaleReturnTicket", Category = "Tijara" };
        public static readonly PermissionRecord ManageDeliveryRequestTicket = new PermissionRecord { Name = "Delivery Request Ticket", SystemName = "ManageDeliveryRequestTicket", Category = "Tijara" };

        public static readonly PermissionRecord ManageAgent = new PermissionRecord { Name = "Agent", SystemName = "ManageAgent", Category = "Tijara" };
        public static readonly PermissionRecord ManageRaisePoCompleteAndIncomplete = new PermissionRecord { Name = "Raise Po Complete And Incomplete", SystemName = "ManageRaisePoCompleteAndIncomplete", Category = "Tijara" };
        public static readonly PermissionRecord ManageSalesReturnCompleteAndIncomplete = new PermissionRecord { Name = "Sales Return Complete And Incomplete", SystemName = "ManageSalesReturnCompleteAndIncomplete", Category = "Tijara" };

        public static readonly PermissionRecord ManageInventoryRate = new PermissionRecord { Name = "Manage Inventory Rate", SystemName = "ManageInventoryRate", Category = "Tijara" };
        public static readonly PermissionRecord InventoryRate = new PermissionRecord { Name = "Inventory Rate", SystemName = "InventoryRate", Category = "Tijara" };

        //New Persmissions For Tijara
        public static readonly PermissionRecord CreateRequest = new PermissionRecord { Name = "Create Request", SystemName = "CreateRequest", Category = "Tijara" };
        public static readonly PermissionRecord RejectRequest = new PermissionRecord { Name = "Reject Request", SystemName = "RejectRequest", Category = "Tijara" };
        public static readonly PermissionRecord InitiateRfq = new PermissionRecord { Name = "Initiate Rfq", SystemName = "InitiateRfq", Category = "Tijara" };
        public static readonly PermissionRecord CreateSalesOrder = new PermissionRecord { Name = "Create Sales Order", SystemName = "CreateSalesOrder", Category = "Tijara" };
        public static readonly PermissionRecord CreateQuotation = new PermissionRecord { Name = "Create Quotation", SystemName = "CreateQuotation", Category = "Tijara" };
        public static readonly PermissionRecord ChooseQuotationToCreatePurchaseOrder = new PermissionRecord { Name = "Choose Quotation To Create Purchase Order", SystemName = "ChooseQuotationToCreatePurchaseOrder", Category = "Tijara" };
        public static readonly PermissionRecord CreatePurchaseOrder = new PermissionRecord { Name = "Create Purchase Order", SystemName = "CreatePurchaseOrder", Category = "Tijara" };
        public static readonly PermissionRecord GenerateDeliveryRequestOnSaleOrder = new PermissionRecord { Name = "Generate Delivery Request On Sale Order", SystemName = "GenerateDeliveryRequestOnSaleOrder", Category = "Tijara" };
        public static readonly PermissionRecord GeneratePickupRequestOnPurchaseOrder = new PermissionRecord { Name = "Generate Pickup Request On Purchase Order", SystemName = "GeneratePickupRequestOnPurchaseOrder", Category = "Tijara" };
        public static readonly PermissionRecord SignSalesOrderContract = new PermissionRecord { Name = "Sign Sales Order Contract", SystemName = "SignSalesOrderContract", Category = "Tijara" };
        public static readonly PermissionRecord SignPurchaseOrderContract = new PermissionRecord { Name = "Sign Purchase Order Contract", SystemName = "SignPurchaseOrderContract", Category = "Tijara" };
        public static readonly PermissionRecord RespondToDeleiveryRequest = new PermissionRecord { Name = "Respond To Deleivery Request", SystemName = "RespondToDeleiveryRequest", Category = "Tijara" };
        public static readonly PermissionRecord RespondToPickupRequest = new PermissionRecord { Name = "Respond To Pickup Request", SystemName = "RespondToPickupRequest", Category = "Tijara" };
        public static readonly PermissionRecord ReassignAgentOnOperationTicket = new PermissionRecord { Name = "Reassign Agent On Operation Ticket", SystemName = "ReassignAgentOnOperationTicket", Category = "Tijara" };

        public static readonly PermissionRecord PurchaseOrderShipmentRequestApproved = new PermissionRecord { Name = "Purchase Order Shipment Request Approved", SystemName = "PurchaseOrderShipmentRequestApproved", Category = "Tijara" };
        public static readonly PermissionRecord PurchaseOrderShipmentShipped = new PermissionRecord { Name = "Purchase Order Shipment Shipped", SystemName = "PurchaseOrderShipmentShipped", Category = "Tijara" };
        public static readonly PermissionRecord PurchaseOrderShipmentDelivered = new PermissionRecord { Name = "Purchase Order Shipment Delivered", SystemName = "PurchaseOrderShipmentDelivered", Category = "Tijara" };

        public static readonly PermissionRecord SaleOrderShipmentRequestApproved = new PermissionRecord { Name = "Sale Order Shipment Request Approved", SystemName = "SaleOrderShipmentRequestApproved", Category = "Tijara" };
        public static readonly PermissionRecord SaleOrderShipmentShipped = new PermissionRecord { Name = "Sale Order Shipment Shipped", SystemName = "SaleOrderShipmentShipped", Category = "Tijara" };
        public static readonly PermissionRecord SaleOrderShipmentDelivered = new PermissionRecord { Name = "Sale Order Shipment Delivered", SystemName = "SaleOrderShipmentDelivered", Category = "Tijara" };

        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                AccessAdminPanel,
                AllowCustomerImpersonation,
                ManageProducts,
                ManageCategories,
                //ManageManufacturers,
                ManageProductReviews,
                ManageProductTags,
                ManageAttributes,
                ManageCustomers,
                ManageCurrentCarts,
                ManageOrders,
                ManageRecurringPayments,
                ManageGiftCards,
                ManageReturnRequests,
                OrderCountryReport,
                SalesSummaryReport,
                ManageAffiliates,
                ManageCampaigns,
                ManageDiscounts,
                ManageNewsletterSubscribers,
                ManagePolls,
                ManageNews,
                ManageBlog,
                ManageWidgets,
                ManageTopics,
                ManageForums,
                ManageMessageTemplates,
                ManageCountries,
                ManageLanguages,
                ManageSettings,
                ManagePaymentMethods,
                ManageExternalAuthenticationMethods,
                ManageMultifactorAuthenticationMethods,
                ManageTaxSettings,
                ManageShippingSettings,
                ManageCurrencies,
                ManageActivityLog,
                ManageAcl,
                ManageEmailAccounts,
                ManageStores,
                ManagePlugins,
                ManageSystemLog,
                ManageMessageQueue,
                ManageMaintenance,
                HtmlEditorManagePictures,
                ManageScheduleTasks,
                ManageAppSettings,
                DisplayPrices,
                EnableShoppingCart,
                EnableWishlist,
                PublicStoreAllowNavigation,
                AccessClosedStore,
                AccessProfiling,
                EnableMultiFactorAuthentication
            };
        }

        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string, PermissionRecord[])>
            {
                (
                    ZarayeCustomerDefaults.AdministratorsRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        AllowCustomerImpersonation,
                        ManageProducts,
                        ManageCategories,
                        //ManageManufacturers,
                        ManageProductReviews,
                        ManageProductTags,
                        ManageAttributes,
                        ManageCustomers,
                        ManageCurrentCarts,
                        ManageOrders,
                        ManageRecurringPayments,
                        ManageGiftCards,
                        ManageReturnRequests,
                        OrderCountryReport,
                        SalesSummaryReport,
                        ManageAffiliates,
                        ManageCampaigns,
                        ManageDiscounts,
                        ManageNewsletterSubscribers,
                        ManagePolls,
                        ManageNews,
                        ManageBlog,
                        ManageWidgets,
                        ManageTopics,
                        ManageForums,
                        ManageMessageTemplates,
                        ManageCountries,
                        ManageLanguages,
                        ManageSettings,
                        ManagePaymentMethods,
                        ManageExternalAuthenticationMethods,
                        ManageMultifactorAuthenticationMethods,
                        ManageTaxSettings,
                        ManageShippingSettings,
                        ManageCurrencies,
                        ManageActivityLog,
                        ManageAcl,
                        ManageEmailAccounts,
                        ManageStores,
                        ManagePlugins,
                        ManageSystemLog,
                        ManageMessageQueue,
                        ManageMaintenance,
                        HtmlEditorManagePictures,
                        ManageScheduleTasks,
                        ManageAppSettings,
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation,
                        AccessClosedStore,
                        AccessProfiling,
                        EnableMultiFactorAuthentication
                    }
                ),
                (
                    ZarayeCustomerDefaults.ForumModeratorsRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                ),
                (
                    ZarayeCustomerDefaults.GuestsRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                ),
                (
                    ZarayeCustomerDefaults.RegisteredRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation,
                        EnableMultiFactorAuthentication
                    }
                )
            };
        }
    }
}