using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Blogs;
using Zaraye.Core.Domain.Catalog;

namespace Zaraye.Services.Catalog
{
    /// <summary>
    /// Represents default values related to catalog services
    /// </summary>
    public static partial class ZarayeCatalogDefaults
    {
        #region Common

        /// <summary>
        /// Gets a default price range 'from'
        /// </summary>
        public static decimal DefaultPriceRangeFrom => 0;

        /// <summary>
        /// Gets a default price range 'to'
        /// </summary>
        public static decimal DefaultPriceRangeTo => 10000;

        #endregion

        #region Products

        /// <summary>
        /// Gets a template of product name on copying
        /// </summary>
        /// <remarks>
        /// {0} : product name
        /// </remarks>
        public static string ProductCopyNameTemplate => "Copy of {0}";

        /// <summary>
        /// Gets default prefix for product
        /// </summary>
        public static string ProductAttributePrefix => "product_attribute_";

        #endregion

        #region Caching defaults

        #region Industries

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey IndustriesHomepageCacheKey => new("Zaraye.industry.homepage.", IndustriesHomepagePrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// </remarks>
        public static CacheKey IndustriesHomepageWithoutHiddenCacheKey => new("Zaraye.industry.homepage.withouthidden-{0}-{1}", IndustriesHomepagePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string IndustriesHomepagePrefix => "Zaraye.industry.homepage.";

        /// <summary>
        /// Key for caching of industry breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : industry id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static CacheKey IndustryBreadcrumbCacheKey => new("Zaraye.industry.breadcrumb.{0}-{1}-{2}-{3}", IndustryBreadcrumbPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string IndustryBreadcrumbPrefix => "Zaraye.industry.breadcrumb.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey IndustriesAllCacheKey => new("Zaraye.industry.all.{0}-{1}-{2}", ZarayeEntityCacheDefaults<Industry>.AllPrefix);

        public static CacheKey CategoriesByIndustryCacheKey => new("Zaraye.industry.byindustry.{0}-{1}-{2}", CategoriesByIndustryPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent industry ID
        /// </remarks>
        public static string CategoriesByIndustryPrefix => "Zaraye.industry.byindustry.{0}";

        #endregion

        #region Categories

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey CategoriesByParentCategoryCacheKey => new("Zaraye.category.byparent.{0}-{1}-{2}-{3}", CategoriesByParentCategoryPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// </remarks>
        public static string CategoriesByParentCategoryPrefix => "Zaraye.category.byparent.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesChildIdsCacheKey => new("Zaraye.category.childids.{0}-{1}-{2}-{3}", CategoriesChildIdsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// </remarks>
        public static string CategoriesChildIdsPrefix => "Zaraye.category.childids.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey CategoriesHomepageCacheKey => new("Zaraye.category.homepage.", CategoriesHomepagePrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// </remarks>
        public static CacheKey CategoriesHomepageWithoutHiddenCacheKey => new("Zaraye.category.homepage.withouthidden-{0}-{1}", CategoriesHomepagePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesHomepagePrefix => "Zaraye.category.homepage.";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static CacheKey CategoryBreadcrumbCacheKey => new("Zaraye.category.breadcrumb.{0}-{1}-{2}-{3}", CategoryBreadcrumbPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryBreadcrumbPrefix => "Zaraye.category.breadcrumb.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesAllCacheKey => new("Zaraye.category.all.{0}-{1}-{2}", ZarayeEntityCacheDefaults<Category>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey ProductCategoriesByProductCacheKey => new("Zaraye.productcategory.byproduct.{0}-{1}-{2}-{3}", ProductCategoriesByProductPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductCategoriesByProductPrefix => "Zaraye.productcategory.byproduct.{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer roles ID hash
        /// {1} : current store ID
        /// {2} : categories ID hash
        /// </remarks>
        public static CacheKey CategoryProductsNumberCacheKey => new("Zaraye.productcategory.products.number.{0}-{1}-{2}", CategoryProductsNumberPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryProductsNumberPrefix => "Zaraye.productcategory.products.number.";

        #endregion

        #region Manufacturers

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey ProductManufacturersByProductCacheKey => new("Zaraye.productmanufacturer.byproduct.{0}-{1}-{2}-{3}", ProductManufacturersByProductPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductManufacturersByProductPrefix => "Zaraye.productmanufacturer.byproduct.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : industry ID
        /// </remarks>
        public static CacheKey ManufacturersByCategoryCacheKey => new("Zaraye.manufacturer.byindustry.{0}", ManufacturersByCategoryPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ManufacturersByCategoryPrefix => "Zaraye.manufacturer.byindustry.";

        #endregion

        #region Products

        /// <summary>
        /// Key for "related" product displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey RelatedProductsCacheKey => new("Zaraye.relatedproduct.byproduct.{0}-{1}", RelatedProductsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string RelatedProductsPrefix => "Zaraye.relatedproduct.byproduct.{0}";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// </remarks>
        public static CacheKey TierPricesByProductCacheKey => new("Zaraye.tierprice.byproduct.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ProductsHomepageCacheKey => new("Zaraye.product.homepage.");

        /// <summary>
        /// Key for caching identifiers of industry featured products
        /// </summary>
        /// <remarks>
        /// {0} : industry id
        /// {1} : customer role Ids
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CategoryFeaturedProductsIdsKey => new("Zaraye.product.featured.byindustry.{0}-{1}-{2}", CategoryFeaturedProductsIdsPrefix, FeaturedProductIdsPrefix);
        public static string CategoryFeaturedProductsIdsPrefix => "Zaraye.product.featured.byindustry.{0}";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured products
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : customer role Ids
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerFeaturedProductIdsKey => new("Zaraye.product.featured.bymanufacturer.{0}-{1}-{2}", ManufacturerFeaturedProductIdsPrefix, FeaturedProductIdsPrefix);
        public static string ManufacturerFeaturedProductIdsPrefix => "Zaraye.product.featured.bymanufacturer.{0}";

        public static string FeaturedProductIdsPrefix => "Zaraye.product.featured.";

        /// <summary>
        /// Gets a key for product prices
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : overridden product price
        /// {2} : additional charge
        /// {3} : include discounts (true, false)
        /// {4} : quantity
        /// {5} : roles of the current user
        /// {6} : current store ID
        /// </remarks>
        public static CacheKey ProductPriceCacheKey => new("Zaraye.totals.productprice.{0}-{1}-{2}-{3}-{4}-{5}-{6}", ProductPricePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public static string ProductPricePrefix => "Zaraye.totals.productprice.{0}";

        /// <summary>
        /// Gets a key for product multiple prices
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : customer role ids
        /// {2} : store id
        /// </remarks>
        public static CacheKey ProductMultiplePriceCacheKey => new("Zaraye.totals.productprice.multiple.{0}-{1}-{2}", ProductMultiplePricePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public static string ProductMultiplePricePrefix => "Zaraye.totals.productprice.multiple.{0}";

        #endregion

        #region Product attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductAttributeMappingsByProductCacheKey => new("Zaraye.productattributemapping.byproduct.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute mapping ID
        /// </remarks>
        public static CacheKey ProductAttributeValuesByAttributeCacheKey => new("Zaraye.productattributevalue.byattribute.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductAttributeCombinationsByProductCacheKey => new("Zaraye.productattributecombination.byproduct.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Product attribute ID
        /// </remarks>
        public static CacheKey PredefinedProductAttributeValuesByAttributeCacheKey => new("Zaraye.predefinedproductattributevalue.byattribute.{0}");

        #endregion

        #region Product tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : hash of list of customer roles IDs
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey ProductTagCountCacheKey => new("Zaraye.producttag.count.{0}-{1}-{2}", ZarayeEntityCacheDefaults<ProductTag>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductTagsByProductCacheKey => new("Zaraye.producttag.byproduct.{0}", ZarayeEntityCacheDefaults<ProductTag>.Prefix);

        #endregion

        #region Category tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : hash of list of customer roles IDs
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey CategoryTagCountCacheKey => new("Zaraye.categorytag.count.{0}-{1}-{2}", ZarayeEntityCacheDefaults<CategoryTag>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        public static CacheKey CategoryTagsByCategoryCacheKey => new("Zaraye.categorytag.byproduct.{0}", ZarayeEntityCacheDefaults<CategoryTag>.Prefix);

        #endregion

        #region Industry tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : hash of list of customer roles IDs
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey IndustryTagCountCacheKey => new("Zaraye.industrytag.count.{0}-{1}-{2}", ZarayeEntityCacheDefaults<IndustryTag>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        public static CacheKey IndustryTagsByIndustryCacheKey => new("Zaraye.industrytag.byproduct.{0}", ZarayeEntityCacheDefaults<IndustryTag>.Prefix);

        #endregion

        #region Manufacturer tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : hash of list of customer roles IDs
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey ManufacturerTagCountCacheKey => new("Zaraye.manufacturertag.count.{0}-{1}-{2}", ZarayeEntityCacheDefaults<ManufacturerTag>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        public static CacheKey ManufacturerTagsByManufacturerCacheKey => new("Zaraye.manufacturertag.byproduct.{0}", ZarayeEntityCacheDefaults<ManufacturerTag>.Prefix);

        #endregion

        #region Topic tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : hash of list of customer roles IDs
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey TopicTagCountCacheKey => new("Zaraye.topictag.count.{0}-{1}-{2}", ZarayeEntityCacheDefaults<TopicTag>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        public static CacheKey TopicTagsByTopicCacheKey => new("Zaraye.topictag.byproduct.{0}", ZarayeEntityCacheDefaults<TopicTag>.Prefix);

        #endregion

        #region Review type

        /// <summary>
        /// Key for caching product review and review type mapping
        /// </summary>
        /// <remarks>
        /// {0} : product review ID
        /// </remarks>
        public static CacheKey ProductReviewTypeMappingByReviewTypeCacheKey => new("Zaraye.productreviewreviewtypemapping.byreviewtype.{0}");

        #endregion

        #region Specification attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : specification attribute option ID
        /// {2} : allow filtering
        /// {3} : show on product page
        /// {4} : specification attribute group ID
        /// </remarks>
        public static CacheKey ProductSpecificationAttributeByProductCacheKey => new("Zaraye.productspecificationattribute.byproduct.{0}-{1}-{2}-{3}-{4}", ProductSpecificationAttributeByProductPrefix, ProductSpecificationAttributeAllByProductPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductSpecificationAttributeByProductPrefix => "Zaraye.productspecificationattribute.byproduct.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {1} (not 0, see the <ref>ProductSpecificationAttributeAllByProductIdCacheKey</ref>) :specification attribute option ID
        /// </remarks>
        public static string ProductSpecificationAttributeAllByProductPrefix => "Zaraye.productspecificationattribute.byproduct.";

        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public static CacheKey SpecificationAttributesWithOptionsCacheKey => new("Zaraye.specificationattribute.withoptions.");

        /// <summary>
        /// Key for specification attributes caching
        /// </summary>
        /// <remarks>
        /// {0} : specification attribute ID
        /// </remarks>
        public static CacheKey SpecificationAttributeOptionsCacheKey => new("Zaraye.specificationattributeoption.byattribute.{0}");

        /// <summary>
        /// Key for specification attribute options by industry ID caching
        /// </summary>
        /// <remarks>
        /// {0} : industry ID
        /// </remarks>
        public static CacheKey SpecificationAttributeOptionsByCategoryCacheKey => new("Zaraye.specificationattributeoption.byindustry.{0}", FilterableSpecificationAttributeOptionsPrefix);

        /// <summary>
        /// Key for specification attribute options by manufacturer ID caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer ID
        /// </remarks>
        public static CacheKey SpecificationAttributeOptionsByManufacturerCacheKey => new("Zaraye.specificationattributeoption.bymanufacturer.{0}", FilterableSpecificationAttributeOptionsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string FilterableSpecificationAttributeOptionsPrefix => "Zaraye.specificationattributeoption";

        /// <summary>
        /// Gets a key for specification attribute groups caching by product id
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey SpecificationAttributeGroupByProductCacheKey => new("Zaraye.specificationattributegroup.byproduct.{0}", SpecificationAttributeGroupByProductPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string SpecificationAttributeGroupByProductPrefix => "Zaraye.specificationattributegroup.byproduct.";

        #endregion

        #region Product Suppliers

        public static CacheKey AllSuppliersByProductCacheKey => new("Zaraye.product.suppliers.all.{0}", ZarayeEntityCacheDefaults<SupplierProduct>.AllPrefix);

        #endregion

        public static CacheKey AllBlogPostCacheKey => new("Zaraye.blog.post.all.{0}-{1}", PageSizePrefix, PageIndexPrefix);
        public static string PageSizePrefix => "Zaraye.Pagesize.{0}";
        public static string PageIndexPrefix => "Zaraye.PageIndex.{0}";
        public static CacheKey ProductByIdCacheKey => new("Zaraye.product.Id.{0}", ProductId);
        public static string ProductId => "Zaraye.ProductId.{0}";
        public static CacheKey ProductCategoryIdCacheKey => new("Zaraye.product.category.{0}", categoryId);
        public static string categoryId => "Zaraye.categoryId.{0}";
        public static CacheKey AllProductCacheKey => new("Zaraye.product.all");
        public static CacheKey BestSellingProductsCacheKey => new("Zaraye.bestselling.prodcts.{0}-{1}", PageSizePrefix, PageIndexPrefix);
        public static CacheKey BestSellingProductsByIndustryIdCacheKey => new("Zaraye.bestselling.prodcts.by.industryId.{0}", industryId);
        public static string industryId => "Zaraye.industryId.{0}";
        public static CacheKey CategoriesByIndustryIdCacheKey => new("Zaraye.category.by.industryId.{0}", industryId);
        public static CacheKey productByKeywordCacheKey => new("Zaraye.product.by.keyword.{0}", keywordPrefix);
        public static string keywordPrefix => "Zaraye.keywordPrefix.{0}";
        public static CacheKey CategoriesByKeywordCacheKey => new ("Zaraye.Categories.by.keyword.{0}", keywordPrefix);
        public static CacheKey marketplaceByKeywordCacheKey => new("Zaraye.marketplace.by.keyword.{0}", keywordPrefix);
        public static CacheKey IndustriesByKeywordCacheKey => new("Zaraye.Industries.by.keyword.{0}", keywordPrefix);
        public static CacheKey ManufacturersByKeywordCacheKey => new("Zaraye.Manufacturers.by.keyword.{0}", keywordPrefix);
        public static CacheKey BlogsByKeywordCacheKey => new("Zaraye.Blogs.by.keyword.{0}", keywordPrefix);
        public static CacheKey TopicsByKeywordCacheKey => new("Zaraye.Topics.by.keyword.{0}", keywordPrefix);
        public static CacheKey MarketPlaceExchangeRateCacheKey => new("Zaraye.MarketPlace.ExchangeRate.{0}", DateTimePrefix);
        public static CacheKey EmployeeInsightsCacheKey => new("Zaraye.Employee.Insights.{0}-{1}", PageSizePrefix, PageIndexPrefix);
        public static CacheKey CustomerTestimonialsCacheKey => new("Zaraye.Customer.Testimonials.{0}-{1}", PageSizePrefix, PageIndexPrefix);
        public static CacheKey AllCartItemsCacheKey => new("Zaraye.all.cartItems.{0}-{1}", cartTypePrefix);
        public static string DateTimePrefix => "Zaraye.DateTime.{0}";
        public static string cartTypePrefix => "Zaraye.cartTypePrefix.";


        #endregion
    }
}