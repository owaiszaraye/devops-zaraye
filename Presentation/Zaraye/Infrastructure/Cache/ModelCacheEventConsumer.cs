using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Blogs;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Configuration;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Media;
using Zaraye.Core.Domain.News;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Topics;
using Zaraye.Core.Events;
using Zaraye.Services.Events;

namespace Zaraye.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        //languages
        IConsumer<EntityInsertedEvent<Language>>,
        IConsumer<EntityUpdatedEvent<Language>>,
        IConsumer<EntityDeletedEvent<Language>>,
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //manufacturers
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        //categories
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        //industries
        IConsumer<EntityInsertedEvent<Industry>>,
        IConsumer<EntityUpdatedEvent<Industry>>,
        IConsumer<EntityDeletedEvent<Industry>>,
        //product categories
        IConsumer<EntityInsertedEvent<ProductCategory>>,
        IConsumer<EntityDeletedEvent<ProductCategory>>,
        //products
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>,
        //product tags
        IConsumer<EntityInsertedEvent<ProductTag>>,
        IConsumer<EntityUpdatedEvent<ProductTag>>,
        IConsumer<EntityDeletedEvent<ProductTag>>,
        //Product attribute values
        IConsumer<EntityUpdatedEvent<ProductAttributeValue>>,
        //Topics
        IConsumer<EntityInsertedEvent<Topic>>,
        IConsumer<EntityUpdatedEvent<Topic>>,
        IConsumer<EntityDeletedEvent<Topic>>,
        //Orders
        IConsumer<EntityInsertedEvent<Order>>,
        IConsumer<EntityUpdatedEvent<Order>>,
        IConsumer<EntityDeletedEvent<Order>>,
        //Picture
        IConsumer<EntityInsertedEvent<Picture>>,
        IConsumer<EntityUpdatedEvent<Picture>>,
        IConsumer<EntityDeletedEvent<Picture>>,
        //Product picture mapping
        IConsumer<EntityInsertedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<ProductPicture>>,
        IConsumer<EntityDeletedEvent<ProductPicture>>,
        //Product review
        IConsumer<EntityDeletedEvent<ProductReview>>,
        //blog posts
        IConsumer<EntityInsertedEvent<BlogPost>>,
        IConsumer<EntityUpdatedEvent<BlogPost>>,
        IConsumer<EntityDeletedEvent<BlogPost>>,
        //news items
        IConsumer<EntityInsertedEvent<NewsItem>>,
        IConsumer<EntityUpdatedEvent<NewsItem>>,
        IConsumer<EntityDeletedEvent<NewsItem>>,
        //shopping cart items
        IConsumer<EntityUpdatedEvent<ShoppingCartItem>>
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(CatalogSettings catalogSettings, IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        #region Languages

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        #endregion

        #region Setting

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.HomepageBestsuppliersIdsPrefixCacheKey); //depends on CatalogSettings.NumberOfBestsuppliersOnHomepage
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.BlogPrefixCacheKey); //depends on BlogSettings.NumberOfTags
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.NewsPrefixCacheKey); //depends on NewsSettings.MainPageNewsCount
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey); //depends on distinct sitemap settings
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.WidgetPrefixCacheKey); //depends on WidgetSettings and certain settings of widgets
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.StoreLogoPathPrefixCacheKey); //depends on StoreInformationSettings.LogoPictureId
        }

        #endregion

        #region  Manufacturers

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ManufacturerPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Categories

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.CategoryPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Product categories

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
                await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
                await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }
        }

        #endregion

        #region Products

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.HomepageBestsuppliersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ProductReviewsPrefixCacheKeyById, eventMessage.Entity.Id));
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.HomepageBestsuppliersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Product tags

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Product attributes

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeValue> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductAttributeImageSquarePicturePrefixCacheKey);
        }

        #endregion

        #region Topics

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Orders

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.HomepageBestsuppliersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.HomepageBestsuppliersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.HomepageBestsuppliersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        #endregion

        #region Pictures

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.OrderPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductOverviewPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.OrderPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductOverviewPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
        }

        #endregion

        #region Product picture mappings

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ProductOverviewPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ProductOverviewPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ProductOverviewPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        #endregion

        #region Blog posts

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region News items

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Shopping cart items

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        #endregion

        #region Product reviews

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductReview> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(ZarayeModelCacheDefaults.ProductReviewsPrefixCacheKeyById, eventMessage.Entity.ProductId));
        }

        #endregion

        #region Industries

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Industry> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Industry> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Industry> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        #endregion

        #endregion
    }
}