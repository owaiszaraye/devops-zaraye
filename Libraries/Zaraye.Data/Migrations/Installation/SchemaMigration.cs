using FluentMigrator;
using Zaraye.Core.Domain.Affiliates;
using Zaraye.Core.Domain.Blogs;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Configuration;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Directory;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Core.Domain.Forums;
using Zaraye.Core.Domain.Gdpr;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Logging;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.Messages;
using Zaraye.Core.Domain.News;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.ScheduleTasks;
using Zaraye.Core.Domain.Security;
using Zaraye.Core.Domain.Seo;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Domain.Stores;
using Zaraye.Core.Domain.Tax;
using Zaraye.Core.Domain.Topics;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Migrations.Installation
{
    [ZarayeMigration("2020/01/31 11:24:20:2551771", "Zaraye.Data base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// <remarks>
        /// We use an explicit table creation order instead of an automatic one
        /// due to problems creating relationships between tables
        /// </remarks>
        /// </summary>
        public override void Up()
        {
            Create.TableFor<AddressAttribute>();
            Create.TableFor<AddressAttributeValue>();
            Create.TableFor<GenericAttribute>();
            Create.TableFor<SearchTerm>();
            Create.TableFor<Country>();
            Create.TableFor<Currency>();
            Create.TableFor<MeasureDimension>();
            Create.TableFor<MeasureWeight>();
            Create.TableFor<StateProvince>();
            Create.TableFor<Address>();
            Create.TableFor<Affiliate>();
            Create.TableFor<Language>();
            Create.TableFor<CustomerAttribute>();
            Create.TableFor<CustomerAttributeValue>();
            Create.TableFor<Customer>();
            Create.TableFor<CustomerPassword>();
            Create.TableFor<CustomerAddressMapping>();
            Create.TableFor<CustomerRole>();
            Create.TableFor<CustomerCustomerRoleMapping>();
            Create.TableFor<ExternalAuthenticationRecord>();
            Create.TableFor<ReturnRequest>();
            Create.TableFor<ReturnRequestReason>();
            Create.TableFor<ProductAttribute>();
            Create.TableFor<PredefinedProductAttributeValue>();
            Create.TableFor<ProductTag>();
            Create.TableFor<Product>();
            Create.TableFor<ProductTemplate>();
            Create.TableFor<BackInStockSubscription>();
            Create.TableFor<RelatedProduct>();
            Create.TableFor<ReviewType>();
            Create.TableFor<SpecificationAttributeGroup>();
            Create.TableFor<SpecificationAttribute>();
            Create.TableFor<ProductAttributeCombination>();
            Create.TableFor<ProductAttributeMapping>();
            Create.TableFor<ProductAttributeValue>();
            Create.TableFor<Order>();
            Create.TableFor<OrderItem>();
            Create.TableFor<RewardPointsHistory>();
            Create.TableFor<OrderNote>();
            Create.TableFor<RecurringPayment>();
            Create.TableFor<RecurringPaymentHistory>();
            Create.TableFor<ShoppingCartItem>();
            Create.TableFor<Store>();
            Create.TableFor<StoreMapping>();
            Create.TableFor<LocaleStringResource>();
            Create.TableFor<LocalizedProperty>();
            Create.TableFor<BlogPost>();
            Create.TableFor<BlogComment>();
            Create.TableFor<Category>();
            Create.TableFor<CategoryTemplate>();
            Create.TableFor<ProductCategory>();
            Create.TableFor<CrossSellProduct>();
            Create.TableFor<Manufacturer>();
            Create.TableFor<ManufacturerTemplate>();
            Create.TableFor<ProductManufacturer>();
            Create.TableFor<ProductProductTagMapping>();
            Create.TableFor<ProductReview>();
            Create.TableFor<ProductReviewHelpfulness>();
            Create.TableFor<ProductReviewReviewTypeMapping>();
            Create.TableFor<SpecificationAttributeOption>();
            Create.TableFor<ProductSpecificationAttribute>();
            Create.TableFor<TierPrice>();
            Create.TableFor<Warehouse>();
            Create.TableFor<DeliveryDate>();
            Create.TableFor<ProductAvailabilityRange>();
            Create.TableFor<Shipment>();
            Create.TableFor<ShipmentItem>();
            Create.TableFor<ShippingMethod>();
            Create.TableFor<ShippingMethodCountryMapping>();
            Create.TableFor<ProductWarehouseInventory>();
            Create.TableFor<StockQuantityHistory>();
            Create.TableFor<Download>();
            Create.TableFor<Picture>();
            Create.TableFor<PictureBinary>();
            Create.TableFor<ProductPicture>();
            Create.TableFor<Video>();
            Create.TableFor<ProductVideo>();
            Create.TableFor<Setting>();
            Create.TableFor<Discount>();
            Create.TableFor<DiscountCategoryMapping>();
            Create.TableFor<DiscountProductMapping>();
            Create.TableFor<DiscountRequirement>();
            Create.TableFor<DiscountUsageHistory>();
            Create.TableFor<DiscountManufacturerMapping>();
            Create.TableFor<PrivateMessage>();
            Create.TableFor<ForumGroup>();
            Create.TableFor<Forum>();
            Create.TableFor<ForumTopic>();
            Create.TableFor<ForumPost>();
            Create.TableFor<ForumPostVote>();
            Create.TableFor<ForumSubscription>();
            Create.TableFor<GdprConsent>();
            Create.TableFor<GdprLog>();
            Create.TableFor<ActivityLogType>();
            Create.TableFor<ActivityLog>();
            Create.TableFor<Log>();
            Create.TableFor<Campaign>();
            Create.TableFor<CampaignEmail>();
            Create.TableFor<EmailAccount>();
            Create.TableFor<MessageTemplate>();
            Create.TableFor<NewsLetterSubscription>();
            Create.TableFor<QueuedEmail>();
            Create.TableFor<NewsItem>();
            Create.TableFor<NewsComment>();
            Create.TableFor<AclRecord>();
            Create.TableFor<PermissionRecord>();
            Create.TableFor<PermissionRecordCustomerRoleMapping>();
            Create.TableFor<UrlRecord>();
            Create.TableFor<ScheduleTask>();
            Create.TableFor<TaxCategory>();
            Create.TableFor<TopicTemplate>();
            Create.TableFor<Topic>();
        }
    }
}
