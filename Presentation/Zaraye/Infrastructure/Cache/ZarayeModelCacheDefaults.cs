using Zaraye.Core.Caching;

namespace Zaraye.Infrastructure.Cache
{
    public static partial class ZarayeModelCacheDefaults
    {
        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerNavigationModelKey => new("Zaraye.pres.manufacturer.navigation-{0}-{1}-{2}-{3}", ManufacturerNavigationPrefixCacheKey);
        public static string ManufacturerNavigationPrefixCacheKey => "Zaraye.pres.manufacturer.navigation";

        /// <summary>
        /// Key for list of CategorySimpleModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CategoryAllModelKey => new("Zaraye.pres.category.all-{0}-{1}-{2}", CategoryAllPrefixCacheKey);
        public static string CategoryAllPrefixCacheKey => "Zaraye.pres.category.all";

        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// {2} : picture size
        /// {3} : language ID
        /// {4} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public static CacheKey CategoryHomepageKey => new("Zaraye.pres.category.homepage-{0}-{1}-{2}-{3}-{4}", CategoryHomepagePrefixCacheKey);
        public static string CategoryHomepagePrefixCacheKey => "Zaraye.pres.category.homepage";

        /// <summary>
        /// Key for Xml document of CategorySimpleModels caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CategoryXmlAllModelKey => new("Zaraye.pres.categoryXml.all-{0}-{1}-{2}", CategoryXmlAllPrefixCacheKey);
        public static string CategoryXmlAllPrefixCacheKey => "Zaraye.pres.categoryXml.all";

        /// <summary>
        /// Key for bestsuppliers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public static CacheKey HomepageBestsuppliersIdsKey => new("Zaraye.pres.bestsuppliers.homepage-{0}", HomepageBestsuppliersIdsPrefixCacheKey);
        public static string HomepageBestsuppliersIdsPrefixCacheKey => "Zaraye.pres.bestsuppliers.homepage";

        /// <summary>
        /// Key for "also purchased" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey ProductsAlsoPurchasedIdsKey => new("Zaraye.pres.alsopuchased-{0}-{1}", ProductsAlsoPurchasedIdsPrefixCacheKey);
        public static string ProductsAlsoPurchasedIdsPrefixCacheKey => "Zaraye.pres.alsopuchased";

        /// <summary>
        /// Key for product picture caching on the product catalog pages (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : value indicating whether to display all product pictures
        /// {4} : language ID ("alt" and "title" can depend on localized product name)
        /// {5} : is connection SSL secured?
        /// {6} : current store ID
        /// </remarks>
        public static CacheKey ProductOverviewPicturesModelKey => new("Zaraye.pres.product.overviewpictures-{0}-{1}-{2}-{3}-{4}-{5}-{6}", ProductOverviewPicturesPrefixCacheKey, ProductOverviewPicturesPrefixCacheKeyById);
        public static string ProductOverviewPicturesPrefixCacheKey => "Zaraye.pres.product.overviewpictures";
        public static string ProductOverviewPicturesPrefixCacheKeyById => "Zaraye.pres.product.overviewpictures-{0}-";

        /// <summary>
        /// Key for product picture caching on the product details page (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : isAssociatedProduct?
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey ProductDetailsPicturesModelKey => new("Zaraye.pres.product.detailspictures-{0}-{1}-{2}-{3}-{4}-{5}", ProductDetailsPicturesPrefixCacheKey, ProductDetailsPicturesPrefixCacheKeyById);
        public static string ProductDetailsPicturesPrefixCacheKey => "Zaraye.pres.product.detailspictures";
        public static string ProductDetailsPicturesPrefixCacheKeyById => "Zaraye.pres.product.detailspictures-{0}-";

        /// <summary>
        /// Key for product reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey ProductReviewsModelKey => new("Zaraye.pres.product.reviews-{0}-{1}", ProductReviewsPrefixCacheKey, ProductReviewsPrefixCacheKeyById);

        public static string ProductReviewsPrefixCacheKey => "Zaraye.pres.product.reviews";
        public static string ProductReviewsPrefixCacheKeyById => "Zaraye.pres.product.reviews-{0}-";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey ProductAttributePictureModelKey => new("Zaraye.pres.productattribute.picture-{0}-{1}-{2}", ProductAttributePicturePrefixCacheKey);
        public static string ProductAttributePicturePrefixCacheKey => "Zaraye.pres.productattribute.picture";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey ProductAttributeImageSquarePictureModelKey => new("Zaraye.pres.productattribute.imagesquare.picture-{0}-{1}-{2}", ProductAttributeImageSquarePicturePrefixCacheKey);
        public static string ProductAttributeImageSquarePicturePrefixCacheKey => "Zaraye.pres.productattribute.imagesquare.picture";

        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey CategoryPictureModelKey => new("Zaraye.pres.category.picture-{0}-{1}-{2}-{3}-{4}-{5}", CategoryPicturePrefixCacheKey, CategoryPicturePrefixCacheKeyById);
        public static string CategoryPicturePrefixCacheKey => "Zaraye.pres.category.picture";
        public static string CategoryPicturePrefixCacheKeyById => "Zaraye.pres.category.picture-{0}-";

        /// <summary>
        /// Key for manufacturer picture caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized manufacturer name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerPictureModelKey => new("Zaraye.pres.manufacturer.picture-{0}-{1}-{2}-{3}-{4}-{5}", ManufacturerPicturePrefixCacheKey, ManufacturerPicturePrefixCacheKeyById);
        public static string ManufacturerPicturePrefixCacheKey => "Zaraye.pres.manufacturer.picture";
        public static string ManufacturerPicturePrefixCacheKeyById => "Zaraye.pres.manufacturer.picture-{0}-";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : shopping cart item id
        /// P.S. we could cache by product ID. it could increase performance.
        /// but it won't work for product attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey CartPictureModelKey => new("Zaraye.pres.cart.picture-{0}-{1}-{2}-{3}-{4}-{5}", CartPicturePrefixCacheKey);
        public static string CartPicturePrefixCacheKey => "Zaraye.pres.cart.picture";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : order item id
        /// P.S. we could cache by product ID. it could increase performance.
        /// but it won't work for product attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey OrderPictureModelKey => new("Zaraye.pres.order.picture-{0}-{1}-{2}-{3}-{4}-{5}", OrderPicturePrefixCacheKey);
        public static string OrderPicturePrefixCacheKey => "Zaraye.pres.order.picture";

        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey HomepagePollsModelKey => new("Zaraye.pres.poll.homepage-{0}-{1}", PollsPrefixCacheKey);
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey PollBySystemNameModelKey => new("Zaraye.pres.poll.systemname-{0}-{1}-{2}", PollsPrefixCacheKey);
        public static string PollsPrefixCacheKey => "Zaraye.pres.poll";

        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey BlogMonthsModelKey => new("Zaraye.pres.blog.months-{0}-{1}", BlogPrefixCacheKey);
        public static string BlogPrefixCacheKey => "Zaraye.pres.blog";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey HomepageNewsModelKey => new("Zaraye.pres.news.homepage-{0}-{1}", NewsPrefixCacheKey);
        public static string NewsPrefixCacheKey => "Zaraye.pres.news";

        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : current theme
        /// {2} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public static CacheKey StoreLogoPath => new("Zaraye.pres.logo-{0}-{1}-{2}", StoreLogoPathPrefixCacheKey);
        public static string StoreLogoPathPrefixCacheKey => "Zaraye.pres.logo";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey SitemapPageModelKey => new("Zaraye.pres.sitemap.page-{0}-{1}-{2}", SitemapPrefixCacheKey);
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : sitemap identifier
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static CacheKey SitemapSeoModelKey => new("Zaraye.pres.sitemap.seo-{0}-{1}-{2}-{3}", SitemapPrefixCacheKey);
        public static string SitemapPrefixCacheKey => "Zaraye.pres.sitemap";

        /// <summary>
        /// Key for widget info
        /// </summary>
        /// <remarks>
        /// {0} : current customer role IDs hash
        /// {1} : current store ID
        /// {2} : widget zone
        /// {3} : current theme name
        /// </remarks>
        public static CacheKey WidgetModelKey => new("Zaraye.pres.widget-{0}-{1}-{2}-{3}", WidgetPrefixCacheKey);
        public static string WidgetPrefixCacheKey => "Zaraye.pres.widget";

    }
}
