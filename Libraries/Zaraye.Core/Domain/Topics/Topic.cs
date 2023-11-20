using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Security;
using Zaraye.Core.Domain.Seo;
using Zaraye.Core.Domain.Stores;
using System;

namespace Zaraye.Core.Domain.Topics
{
    /// <summary>
    /// Represents a topic
    /// </summary>
    public partial class Topic : BaseEntity,StoreEntity, ILocalizedEntity, ISlugSupported, IStoreMappingSupported, IAclSupported, DefaultColumns, IActiveActivityLogEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in sitemap
        /// </summary>
        public bool IncludeInSitemap { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in top menu
        /// </summary>
        public bool IncludeInTopMenu { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
        /// </summary>
        public bool IncludeInFooterColumn1 { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
        /// </summary>
        public bool IncludeInFooterColumn2 { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
        /// </summary>
        public bool IncludeInFooterColumn3 { get; set; }
        public bool IncludeInFooterColumn4 { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in footer (column 1)
        /// </summary>
        public bool IncludeInFooterColumn5 { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic is accessible when a store is closed
        /// </summary>
        public bool AccessibleWhenStoreClosed { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic is password protected
        /// </summary>
        public bool IsPasswordProtected { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value of used topic template identifier
        /// </summary>
        public int TopicTemplateId { get; set; }

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
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        public bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        public int CategoryTemplateId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int StoreId { get; set; }
    }
}
