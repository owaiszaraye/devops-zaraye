using System;
using System.Collections.Generic;
using Zaraye.Core.Domain.Buyer;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Core.Domain.Forums;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.Messages;
using Zaraye.Core.Domain.News;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Security;
using Zaraye.Core.Domain.Shipping;

namespace Zaraye.Data.Mapping
{
    /// <summary>
    /// Base instance of backward compatibility of table naming
    /// </summary>
    public partial class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new()
        {
            { typeof(ProductAttributeMapping), "Product_ProductAttribute_Mapping" },
            { typeof(SupplierProduct), "supplier_product_mapping" },
            { typeof(ProductProductTagMapping), "Product_ProductTag_Mapping" },
            { typeof(CategoryCategoryTagMapping), "Category_CategoryTag_Mapping" },
            { typeof(IndustryIndustryTagMapping), "Industry_IndustryTag_Mapping" },
            { typeof(ManufacturerManufacturerTagMapping), "Manufacturer_ManufacturerTag_Mapping" },
            { typeof(TopicTopicTagMapping), "Topic_TopicTag_Mapping" },
            { typeof(ProductReviewReviewTypeMapping), "ProductReview_ReviewType_Mapping" },
            { typeof(CustomerAddressMapping), "CustomerAddresses" },
            { typeof(CustomerCustomerRoleMapping), "Customer_CustomerRole_Mapping" },
            { typeof(DiscountCategoryMapping), "Discount_AppliedToCategories" },
            { typeof(DiscountManufacturerMapping), "Discount_AppliedToManufacturers" },
            { typeof(DiscountProductMapping), "Discount_AppliedToProducts" },
            { typeof(PermissionRecordCustomerRoleMapping), "PermissionRecord_Role_Mapping" },
            { typeof(ShippingMethodCountryMapping), "ShippingMethodRestrictions" },
            { typeof(ProductCategory), "Product_Category_Mapping" },
            { typeof(ProductManufacturer), "Product_Manufacturer_Mapping" },
            { typeof(ProductPicture), "Product_Picture_Mapping" },
            { typeof(ProductSpecificationAttribute), "Product_SpecificationAttribute_Mapping" },
            { typeof(ForumGroup), "Forums_Group" },
            { typeof(Forum), "Forums_Forum" },
            { typeof(ForumPost), "Forums_Post" },
             { typeof(TransporterVehicleMapping), "Transporter_Vehicle_Mapping" },
            { typeof(ForumPostVote), "Forums_PostVote" },
            { typeof(ForumSubscription), "Forums_Subscription" },
            { typeof(ForumTopic), "Forums_Topic" },
            { typeof(PrivateMessage), "Forums_PrivateMessage" },
              { typeof(CurrencyRateConfig), "currency_rate_config" },
            { typeof(BankDetail), "Customer_BankDetail" },
            { typeof(MspInventoryMapping), "msp_inventory_mapping" },
            { typeof(NewsItem), "News" },
            { typeof(GenericPicture), "generic_picture_mapping" },
            { typeof(PushNotificationDevice), "Pushnotification_Device_Mapping" },
            { typeof(CustomerIndustryMapping), "Customer_Industry_Mapping" },
            { typeof(RequestRfqQuotationMapping), "Request_Rfq_Quotation_Mapping" },
            { typeof(ShipmentPaymentMapping), "Shipment_Payment_Mapping" },
            { typeof(AppliedCreditCustomer), "applied_credit_customer" },
            { typeof(OnlineLeadRejectReason), "onlineleadrejectreason" },

        };

        public Dictionary<(Type, string), string> ColumnName => new()
        {
            { (typeof(Customer), "BillingAddressId"), "BillingAddress_Id" },
            { (typeof(Customer), "ShippingAddressId"), "ShippingAddress_Id" },
            { (typeof(CustomerCustomerRoleMapping), "CustomerId"), "Customer_Id" },
            { (typeof(CustomerCustomerRoleMapping), "CustomerRoleId"), "CustomerRole_Id" },
            { (typeof(PermissionRecordCustomerRoleMapping), "PermissionRecordId"), "PermissionRecord_Id" },
            { (typeof(PermissionRecordCustomerRoleMapping), "CustomerRoleId"), "CustomerRole_Id" },
            { (typeof(ProductProductTagMapping), "ProductId"), "Product_Id" },
            { (typeof(ProductProductTagMapping), "ProductTagId"), "ProductTag_Id" },
            { (typeof(CategoryCategoryTagMapping), "CategoryId"), "Category_Id" },
            { (typeof(CategoryCategoryTagMapping), "CategoryTagId"), "CategoryTag_Id" },
            { (typeof(IndustryIndustryTagMapping), "IndustryId"), "Industry_Id" },
            { (typeof(IndustryIndustryTagMapping), "IndustryTagId"), "IndustryTag_Id" },
            { (typeof(ManufacturerManufacturerTagMapping), "ManufacturerId"), "Manufacturer_Id" },
            { (typeof(ManufacturerManufacturerTagMapping), "ManufacturerTagId"), "ManufacturerTag_Id" },
            { (typeof(TopicTopicTagMapping), "TopicId"), "Topic_Id" },
            { (typeof(TopicTopicTagMapping), "TopicTagId"), "TopicTag_Id" },
            { (typeof(DiscountCategoryMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountCategoryMapping), "EntityId"), "Category_Id" },
            { (typeof(DiscountManufacturerMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountManufacturerMapping), "EntityId"), "Manufacturer_Id" },
            { (typeof(DiscountProductMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountProductMapping), "EntityId"), "Product_Id" },
            { (typeof(CustomerAddressMapping), "AddressId"), "Address_Id" },
            { (typeof(CustomerAddressMapping), "CustomerId"), "Customer_Id" },
            { (typeof(ShippingMethodCountryMapping), "ShippingMethodId"), "ShippingMethod_Id" },
            { (typeof(ShippingMethodCountryMapping), "CountryId"), "Country_Id" },
        };
    }
}