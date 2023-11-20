using System;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Security;
using Zaraye.Core.Domain.Seo;
using Zaraye.Core.Domain.Stores;

namespace Zaraye.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a manufacturer
    /// </summary>
    public partial class Manufacturer : BaseEntity,StoreEntity, ILocalizedEntity, ISlugSupported, IAclSupported, IStoreMappingSupported, IDiscountSupported<DiscountManufacturerMapping>, ISoftDeletedEntity,DefaultColumns, IActiveActivityLogEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets a value of used manufacturer template identifier
        /// </summary>
        public int ManufacturerTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the parent picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        public string PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        public bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the price range filtering is enabled
        /// </summary>
        public bool PriceRangeFiltering { get; set; }

        /// <summary>
        /// Gets or sets the "from" price
        /// </summary>
        public decimal PriceFrom { get; set; }

        /// <summary>
        /// Gets or sets the "to" price
        /// </summary>
        public decimal PriceTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the price range should be entered manually
        /// </summary>
        public bool ManuallyPriceRange { get; set; }

        public bool AppPublished { get; set; }

        public bool PublishedOnPriceDiscovery { get; set; }

        public bool ContentPublished { get; set; }

        public bool ShowOnHomePage { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public int IndustryId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}