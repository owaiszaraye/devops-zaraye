using System;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Security;
using Zaraye.Core.Domain.Seo;

namespace Zaraye.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a Industry
    /// </summary>
    public partial class Industry : BaseEntity,StoreEntity, ILocalizedEntity, ISlugSupported, IAclSupported, ISoftDeletedEntity,DefaultColumns, IActiveActivityLogEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

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
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        public bool AppPublished { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the category on home page
        /// </summary>
        public bool ShowOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        public bool SubjectToAcl { get; set; }
        
        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }

        public bool IsAllowCredit { get; set; }
    }
}